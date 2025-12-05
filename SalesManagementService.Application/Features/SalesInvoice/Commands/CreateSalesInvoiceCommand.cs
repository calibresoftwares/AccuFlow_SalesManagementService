using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.SalesInvoice;
using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesInvoice.Commands
{
    public record CreateSalesInvoiceCommand(CreateSalesInvoiceDto SalesInvoiceDto) : IRequest<SalesInvoiceDto>;

    public class CreateSalesInvoiceCommandHandler : IRequestHandler<CreateSalesInvoiceCommand, SalesInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSalesInvoiceCommandHandler> _logger;
        private readonly IValidator<CreateSalesInvoiceCommand> _validator;
       
        public CreateSalesInvoiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateSalesInvoiceCommandHandler> logger, IValidator<CreateSalesInvoiceCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }
        
        public async Task<SalesInvoiceDto> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating New Sales Invoice");
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var salesInvoice = _mapper.Map<SalesManagementService.Domain.Entities.SalesInvoice>(request.SalesInvoiceDto);
            
            // Generate SalesInvoiceId if not already set
            if (salesInvoice.SalesInvoiceId == Guid.Empty)
            {
                salesInvoice.SalesInvoiceId = Guid.NewGuid();
            }
            
            // Generate Invoice Number if not provided
            if (string.IsNullOrEmpty(salesInvoice.InvoiceNumber))
            {
                salesInvoice.InvoiceNumber = GenerateInvoiceNumber();
            }

            // Set created date
            salesInvoice.CreatedDate = DateTime.UtcNow;

            // Map and calculate line items
            if (request.SalesInvoiceDto.LineItems != null && request.SalesInvoiceDto.LineItems.Any())
            {
                salesInvoice.LineItems = new List<SalesInvoiceLineItem>();
                foreach (var lineItemDto in request.SalesInvoiceDto.LineItems)
                {
                    var lineItem = _mapper.Map<SalesInvoiceLineItem>(lineItemDto);
                    // Generate SalesInvoiceLineItemId if not already set
                    if (lineItem.SalesInvoiceLineItemId == Guid.Empty)
                    {
                        lineItem.SalesInvoiceLineItemId = Guid.NewGuid();
                    }
                    lineItem.SalesInvoiceId = salesInvoice.SalesInvoiceId;
                    lineItem.TenantId = salesInvoice.TenantId;
                    lineItem.CreatedDate = DateTime.UtcNow;
                    lineItem.UpdatedDate = DateTime.UtcNow;
                    
                    // Calculate line item total
                    lineItem.Total = CalculateLineItemTotal(lineItem.Quantity, lineItem.UnitPrice, lineItem.Discount, lineItem.TaxAmount);
                    
                    salesInvoice.LineItems.Add(lineItem);
                }
            }

            // Calculate invoice totals
            CalculateInvoiceTotals(salesInvoice);

            await _unitOfWork.SalesInvoice.AddAsync(salesInvoice);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Reload to get the complete entity with line items
            var createdSalesInvoice = await _unitOfWork.SalesInvoice.GetByIdAsync(salesInvoice.SalesInvoiceId);
            return _mapper.Map<SalesInvoiceDto>(createdSalesInvoice);
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private decimal CalculateLineItemTotal(decimal quantity, decimal unitPrice, decimal discount, decimal taxAmount)
        {
            var subtotal = quantity * unitPrice;
            var afterDiscount = subtotal - discount;
            return afterDiscount + taxAmount;
        }

        private void CalculateInvoiceTotals(SalesManagementService.Domain.Entities.SalesInvoice salesInvoice)
        {
            if (salesInvoice.LineItems != null && salesInvoice.LineItems.Any())
            {
                salesInvoice.TotalAmount = salesInvoice.LineItems.Sum(li => li.Quantity * li.UnitPrice);
                salesInvoice.DiscountAmount = salesInvoice.LineItems.Sum(li => li.Discount);
                salesInvoice.TaxAmount = salesInvoice.LineItems.Sum(li => li.TaxAmount);
                salesInvoice.NetAmount = salesInvoice.TotalAmount - salesInvoice.DiscountAmount + salesInvoice.TaxAmount;
            }
            else
            {
                // Use provided values if no line items
                salesInvoice.NetAmount = salesInvoice.TotalAmount - salesInvoice.DiscountAmount + salesInvoice.TaxAmount;
            }
        }
    }
}


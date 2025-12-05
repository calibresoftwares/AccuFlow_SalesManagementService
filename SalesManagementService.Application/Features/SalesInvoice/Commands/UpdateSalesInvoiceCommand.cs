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
    public record UpdateSalesInvoiceCommand(SalesInvoiceDto SalesInvoiceDto) : IRequest<SalesInvoiceDto>;

    public class UpdateSalesInvoiceCommandHandler : IRequestHandler<UpdateSalesInvoiceCommand, SalesInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateSalesInvoiceCommandHandler> _logger;
        private readonly IValidator<UpdateSalesInvoiceCommand> _validator;
       
        public UpdateSalesInvoiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateSalesInvoiceCommandHandler> logger, IValidator<UpdateSalesInvoiceCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }
        
        public async Task<SalesInvoiceDto> Handle(UpdateSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating Sales Invoice with ID {request.SalesInvoiceDto.SalesInvoiceId}");
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get existing sales invoice
            var existingSalesInvoice = await _unitOfWork.SalesInvoice.GetByIdAsync(request.SalesInvoiceDto.SalesInvoiceId);
            if (existingSalesInvoice == null)
            {
                throw new KeyNotFoundException($"Sales Invoice with ID {request.SalesInvoiceDto.SalesInvoiceId} not found.");
            }

            // Update basic properties
            existingSalesInvoice.CustomerId = request.SalesInvoiceDto.CustomerId;
            existingSalesInvoice.InvoiceDate = request.SalesInvoiceDto.InvoiceDate;
            existingSalesInvoice.Status = request.SalesInvoiceDto.Status;
            existingSalesInvoice.UpdatedDate = DateTime.UtcNow;
            existingSalesInvoice.UpdatedBy = request.SalesInvoiceDto.UpdatedBy;
            
            // Update InvoiceNumber if provided
            if (!string.IsNullOrEmpty(request.SalesInvoiceDto.InvoiceNumber))
            {
                existingSalesInvoice.InvoiceNumber = request.SalesInvoiceDto.InvoiceNumber;
            }

            // Update SalesOrderId if provided
            if (request.SalesInvoiceDto.SalesOrderId.HasValue)
            {
                existingSalesInvoice.SalesOrderId = request.SalesInvoiceDto.SalesOrderId;
            }

            // Update invoice-level financial fields if provided (before line items calculation)
            existingSalesInvoice.TotalAmount = request.SalesInvoiceDto.TotalAmount;
            existingSalesInvoice.DiscountAmount = request.SalesInvoiceDto.DiscountAmount;
            existingSalesInvoice.TaxAmount = request.SalesInvoiceDto.TaxAmount;
            existingSalesInvoice.NetAmount = request.SalesInvoiceDto.NetAmount;

            // Update line items
            if (request.SalesInvoiceDto.LineItems != null && request.SalesInvoiceDto.LineItems.Any())
            {
                // Get existing line items to check for updates
                var existingLineItems = existingSalesInvoice.LineItems?.ToList() ?? new List<SalesInvoiceLineItem>();
                
                // Clear existing line items and rebuild from DTO
                existingSalesInvoice.LineItems = new List<SalesInvoiceLineItem>();
                
                foreach (var lineItemDto in request.SalesInvoiceDto.LineItems)
                {
                    var lineItem = _mapper.Map<SalesInvoiceLineItem>(lineItemDto);
                    
                    // Check if this line item already exists (by SalesInvoiceLineItemId)
                    var existingLineItem = existingLineItems.FirstOrDefault(li => li.SalesInvoiceLineItemId == lineItemDto.SalesInvoiceLineItemId);
                    
                    if (existingLineItem != null)
                    {
                        // Update existing line item properties
                        existingLineItem.ProductId = lineItem.ProductId;
                        existingLineItem.Quantity = lineItem.Quantity;
                        existingLineItem.UnitPrice = lineItem.UnitPrice;
                        existingLineItem.Discount = lineItem.Discount;
                        existingLineItem.TaxAmount = lineItem.TaxAmount;
                        existingLineItem.UpdatedDate = DateTime.UtcNow;
                        existingLineItem.UpdatedBy = request.SalesInvoiceDto.UpdatedBy ?? existingSalesInvoice.CreatedBy;
                        
                        // Calculate line item total
                        existingLineItem.Total = CalculateLineItemTotal(existingLineItem.Quantity, existingLineItem.UnitPrice, existingLineItem.Discount, existingLineItem.TaxAmount);
                        
                        existingSalesInvoice.LineItems.Add(existingLineItem);
                    }
                    else
                    {
                        // Create new line item
                        // Generate SalesInvoiceLineItemId if not already set
                        if (lineItem.SalesInvoiceLineItemId == Guid.Empty)
                        {
                            lineItem.SalesInvoiceLineItemId = Guid.NewGuid();
                        }
                        lineItem.SalesInvoiceId = existingSalesInvoice.SalesInvoiceId;
                        lineItem.TenantId = existingSalesInvoice.TenantId;
                        lineItem.CreatedDate = DateTime.UtcNow;
                        lineItem.UpdatedDate = DateTime.UtcNow;
                        var updateBy = request.SalesInvoiceDto.UpdatedBy ?? existingSalesInvoice.CreatedBy;
                        lineItem.CreatedBy = updateBy;
                        lineItem.UpdatedBy = updateBy;
                        
                        // Calculate line item total
                        lineItem.Total = CalculateLineItemTotal(lineItem.Quantity, lineItem.UnitPrice, lineItem.Discount, lineItem.TaxAmount);
                        
                        existingSalesInvoice.LineItems.Add(lineItem);
                    }
                }
                
                // Recalculate invoice totals from line items (overrides the values set above)
                CalculateInvoiceTotals(existingSalesInvoice);
            }
            else
            {
                // If no line items provided, clear existing line items and ensure NetAmount is calculated correctly
                if (existingSalesInvoice.LineItems != null && existingSalesInvoice.LineItems.Any())
                {
                    existingSalesInvoice.LineItems.Clear();
                }
                existingSalesInvoice.NetAmount = existingSalesInvoice.TotalAmount - existingSalesInvoice.DiscountAmount + existingSalesInvoice.TaxAmount;
            }

            await _unitOfWork.SalesInvoice.UpdateAsync(existingSalesInvoice);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Reload to get the complete entity with line items
            var updatedSalesInvoice = await _unitOfWork.SalesInvoice.GetByIdAsync(existingSalesInvoice.SalesInvoiceId);
            return _mapper.Map<SalesInvoiceDto>(updatedSalesInvoice);
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
                // If no line items, ensure NetAmount is calculated from TotalAmount, DiscountAmount, and TaxAmount
                salesInvoice.NetAmount = salesInvoice.TotalAmount - salesInvoice.DiscountAmount + salesInvoice.TaxAmount;
            }
        }
    }
}


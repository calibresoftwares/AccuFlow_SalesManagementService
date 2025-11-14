using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.SalesOrder;
using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesOrder.Commands
{
    public record CreateSalesOrderCommand(CreateSalesOrderDto SalesOrderDto) : IRequest<SalesOrderDto>;

    public class CreateSalesOrderCommandHandler : IRequestHandler<CreateSalesOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSalesOrderCommandHandler> _logger;
        private readonly IValidator<CreateSalesOrderCommand> _validator;
       
        public CreateSalesOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateSalesOrderCommandHandler> logger, IValidator<CreateSalesOrderCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }
        
        public async Task<SalesOrderDto> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating New Sales Order");
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var salesOrder = _mapper.Map<SalesManagementService.Domain.Entities.SalesOrder>(request.SalesOrderDto);
            
            // Generate Order Number if not provided
            if (string.IsNullOrEmpty(salesOrder.OrderNumber))
            {
                salesOrder.OrderNumber = GenerateOrderNumber();
            }

            // Map and calculate line items
            if (request.SalesOrderDto.SalesOrderLineItems != null && request.SalesOrderDto.SalesOrderLineItems.Any())
            {
                salesOrder.LineItems = new List<SalesOrderLineItem>();
                foreach (var lineItemDto in request.SalesOrderDto.SalesOrderLineItems)
                {
                    var lineItem = _mapper.Map<SalesOrderLineItem>(lineItemDto);
                    lineItem.SalesOrderId = salesOrder.SalesOrderId; // Will be set after save
                    lineItem.TenantId = salesOrder.TenantId;
                    
                    // Calculate line item total
                    lineItem.Total = CalculateLineItemTotal(lineItem.Quantity, lineItem.UnitPrice, lineItem.Discount, lineItem.Tax);
                    
                    salesOrder.LineItems.Add(lineItem);
                }
            }

            // Calculate order totals
            CalculateOrderTotals(salesOrder);

            await _unitOfWork.SalesOrder.AddAsync(salesOrder);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Reload to get the complete entity with line items
            var createdSalesOrder = await _unitOfWork.SalesOrder.GetByIdAsync(salesOrder.SalesOrderId);
            return _mapper.Map<SalesOrderDto>(createdSalesOrder);
        }

        private string GenerateOrderNumber()
        {
            return $"SO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private decimal CalculateLineItemTotal(int quantity, decimal unitPrice, decimal discount, decimal tax)
        {
            var subtotal = quantity * unitPrice;
            var afterDiscount = subtotal - discount;
            return afterDiscount + tax;
        }

        private void CalculateOrderTotals(SalesManagementService.Domain.Entities.SalesOrder salesOrder)
        {
            if (salesOrder.LineItems != null && salesOrder.LineItems.Any())
            {
                salesOrder.TotalAmount = salesOrder.LineItems.Sum(li => li.Quantity * li.UnitPrice);
                salesOrder.Discount = salesOrder.LineItems.Sum(li => li.Discount);
                salesOrder.Tax = salesOrder.LineItems.Sum(li => li.Tax);
                salesOrder.NetAmount = salesOrder.TotalAmount - salesOrder.Discount + salesOrder.Tax;
            }
            else
            {
                // Use provided values if no line items
                salesOrder.NetAmount = salesOrder.TotalAmount - salesOrder.Discount + salesOrder.Tax;
            }
        }
    }
}

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
    public record UpdateSalesOrderCommand(SalesOrderDto SalesOrderDto) : IRequest<SalesOrderDto>;

    public class UpdateSalesOrderCommandHandler : IRequestHandler<UpdateSalesOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateSalesOrderCommandHandler> _logger;
        private readonly IValidator<UpdateSalesOrderCommand> _validator;
       
        public UpdateSalesOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateSalesOrderCommandHandler> logger, IValidator<UpdateSalesOrderCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }
        
        public async Task<SalesOrderDto> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating Sales Order with ID {request.SalesOrderDto.SalesOrderId}");
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get existing sales order
            var existingSalesOrder = await _unitOfWork.SalesOrder.GetByIdAsync(request.SalesOrderDto.SalesOrderId);
            if (existingSalesOrder == null)
            {
                throw new KeyNotFoundException($"Sales Order with ID {request.SalesOrderDto.SalesOrderId} not found.");
            }

            // Update basic properties
            existingSalesOrder.CustomerId = request.SalesOrderDto.CustomerId;
            existingSalesOrder.OrderDate = request.SalesOrderDto.OrderDate;
            existingSalesOrder.Status = request.SalesOrderDto.Status;
            existingSalesOrder.UpdatedDate = DateTime.UtcNow;
            
            // Update OrderNumber if provided
            if (!string.IsNullOrEmpty(request.SalesOrderDto.OrderNumber))
            {
                existingSalesOrder.OrderNumber = request.SalesOrderDto.OrderNumber;
            }

            // Update order-level financial fields if provided (before line items calculation)
            // These will be recalculated from line items if line items are provided
            existingSalesOrder.TotalAmount = request.SalesOrderDto.TotalAmount;
            existingSalesOrder.Discount = request.SalesOrderDto.Discount;
            existingSalesOrder.Tax = request.SalesOrderDto.Tax;
            existingSalesOrder.NetAmount = request.SalesOrderDto.NetAmount;

            // Update line items
            if (request.SalesOrderDto.SalesOrderLineItems != null && request.SalesOrderDto.SalesOrderLineItems.Any())
            {
                // Get existing line items to check for updates
                var existingLineItems = existingSalesOrder.LineItems?.ToList() ?? new List<SalesOrderLineItem>();
                
                // Clear existing line items and rebuild from DTO
                existingSalesOrder.LineItems = new List<SalesOrderLineItem>();
                
                foreach (var lineItemDto in request.SalesOrderDto.SalesOrderLineItems)
                {
                    var lineItem = _mapper.Map<SalesOrderLineItem>(lineItemDto);
                    
                    // Check if this line item already exists (by LineItemId)
                    var existingLineItem = existingLineItems.FirstOrDefault(li => li.LineItemId == lineItemDto.LineItemId);
                    
                    if (existingLineItem != null)
                    {
                        // Update existing line item properties
                        existingLineItem.ProductId = lineItem.ProductId;
                        existingLineItem.Quantity = lineItem.Quantity;
                        existingLineItem.UnitPrice = lineItem.UnitPrice;
                        existingLineItem.Discount = lineItem.Discount;
                        existingLineItem.Tax = lineItem.Tax;
                        existingLineItem.UpdatedDate = DateTime.UtcNow;
                        
                        // Calculate line item total
                        existingLineItem.Total = CalculateLineItemTotal(existingLineItem.Quantity, existingLineItem.UnitPrice, existingLineItem.Discount, existingLineItem.Tax);
                        
                        existingSalesOrder.LineItems.Add(existingLineItem);
                    }
                    else
                    {
                        // Create new line item
                        // Generate LineItemId if not already set
                        if (lineItem.LineItemId == Guid.Empty)
                        {
                            lineItem.LineItemId = Guid.NewGuid();
                        }
                        lineItem.SalesOrderId = existingSalesOrder.SalesOrderId;
                        lineItem.TenantId = existingSalesOrder.TenantId;
                        lineItem.CreatedDate = DateTime.UtcNow;
                        lineItem.UpdatedDate = DateTime.UtcNow;
                        
                        // Calculate line item total
                        lineItem.Total = CalculateLineItemTotal(lineItem.Quantity, lineItem.UnitPrice, lineItem.Discount, lineItem.Tax);
                        
                        existingSalesOrder.LineItems.Add(lineItem);
                    }
                }
                
                // Recalculate order totals from line items (overrides the values set above)
                CalculateOrderTotals(existingSalesOrder);
            }
            else
            {
                // If no line items provided, clear existing line items and ensure NetAmount is calculated correctly
                if (existingSalesOrder.LineItems != null && existingSalesOrder.LineItems.Any())
                {
                    existingSalesOrder.LineItems.Clear();
                }
                existingSalesOrder.NetAmount = existingSalesOrder.TotalAmount - existingSalesOrder.Discount + existingSalesOrder.Tax;
            }

            await _unitOfWork.SalesOrder.UpdateAsync(existingSalesOrder);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Reload to get the complete entity with line items
            var updatedSalesOrder = await _unitOfWork.SalesOrder.GetByIdAsync(existingSalesOrder.SalesOrderId);
            return _mapper.Map<SalesOrderDto>(updatedSalesOrder);
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
                // If no line items, ensure NetAmount is calculated from TotalAmount, Discount, and Tax
                salesOrder.NetAmount = salesOrder.TotalAmount - salesOrder.Discount + salesOrder.Tax;
            }
        }
    }
}

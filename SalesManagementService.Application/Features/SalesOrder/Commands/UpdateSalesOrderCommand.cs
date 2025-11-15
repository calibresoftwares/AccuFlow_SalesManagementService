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

            // Update line items
            if (request.SalesOrderDto.SalesOrderLineItems != null && request.SalesOrderDto.SalesOrderLineItems.Any())
            {
                existingSalesOrder.LineItems = new List<SalesOrderLineItem>();
                foreach (var lineItemDto in request.SalesOrderDto.SalesOrderLineItems)
                {
                    var lineItem = _mapper.Map<SalesOrderLineItem>(lineItemDto);
                    lineItem.SalesOrderId = existingSalesOrder.SalesOrderId;
                    lineItem.TenantId = existingSalesOrder.TenantId;
                    lineItem.UpdatedDate = DateTime.UtcNow;
                    
                    // Calculate line item total
                    lineItem.Total = CalculateLineItemTotal(lineItem.Quantity, lineItem.UnitPrice, lineItem.Discount, lineItem.Tax);
                    
                    existingSalesOrder.LineItems.Add(lineItem);
                }
            }

            // Recalculate order totals
            CalculateOrderTotals(existingSalesOrder);

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
        }
    }
}

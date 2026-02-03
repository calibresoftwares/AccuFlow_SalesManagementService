using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesOrder.Commands
{
    public record DeleteSalesOrderCommand(Guid SalesOrderId) : IRequest<bool>;

    public class DeleteSalesOrderCommandHandler : IRequestHandler<DeleteSalesOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSalesOrderCommandHandler> _logger;
       
        public DeleteSalesOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSalesOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        public async Task<bool> Handle(DeleteSalesOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting Sales Order with ID {request.SalesOrderId}");
            return await _unitOfWork.SalesOrder.DeleteAsync(request.SalesOrderId);
        }
    }
}

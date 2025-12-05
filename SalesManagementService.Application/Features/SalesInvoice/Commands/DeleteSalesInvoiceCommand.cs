using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using System;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesInvoice.Commands
{
    public record DeleteSalesInvoiceCommand(Guid SalesInvoiceId) : IRequest<bool>;

    public class DeleteSalesInvoiceCommandHandler : IRequestHandler<DeleteSalesInvoiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteSalesInvoiceCommandHandler> _logger;
       
        public DeleteSalesInvoiceCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSalesInvoiceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        public async Task<bool> Handle(DeleteSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting Sales Invoice with ID {request.SalesInvoiceId}");
            return await _unitOfWork.SalesInvoice.DeleteAsync(request.SalesInvoiceId);
        }
    }
}


using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.SalesInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesInvoice.Queries
{
    public record GetSalesInvoiceQuery(Guid Id) : IRequest<SalesInvoiceDto>;

    public class GetSalesInvoiceQueryHandler : IRequestHandler<GetSalesInvoiceQuery, SalesInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSalesInvoiceQueryHandler> _logger;

        public GetSalesInvoiceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSalesInvoiceQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SalesInvoiceDto> Handle(GetSalesInvoiceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving sales invoice with ID {request.Id}");

            var salesInvoice = await _unitOfWork.SalesInvoice.GetByIdAsync(request.Id);
            if (salesInvoice == null)
            {
                throw new KeyNotFoundException($"Sales Invoice with ID {request.Id} not found.");
            }

            return _mapper.Map<SalesInvoiceDto>(salesInvoice);
        }
    }
}


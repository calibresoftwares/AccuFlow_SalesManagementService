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
    public record GetAllSalesInvoicesQuery() : IRequest<List<SalesInvoiceDto>>;

    public class GetAllSalesInvoicesQueryHandler : IRequestHandler<GetAllSalesInvoicesQuery, List<SalesInvoiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllSalesInvoicesQueryHandler> _logger;

        public GetAllSalesInvoicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllSalesInvoicesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SalesInvoiceDto>> Handle(GetAllSalesInvoicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all sales invoices");

            var salesInvoices = await _unitOfWork.SalesInvoice.GetSalesInvoicesAsync();
            return _mapper.Map<List<SalesInvoiceDto>>(salesInvoices);
        }
    }
}


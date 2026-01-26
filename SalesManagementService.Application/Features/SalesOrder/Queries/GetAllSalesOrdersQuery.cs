using AutoMapper;
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

namespace SalesManagementService.Application.Features.SalesOrder.Queries
{
    public record GetAllSalesOrdersQuery() : IRequest<List<SalesOrderDto>>;

    public class GetAllSalesOrdersQueryHandler : IRequestHandler<GetAllSalesOrdersQuery, List<SalesOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllSalesOrdersQueryHandler> _logger;

        public GetAllSalesOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllSalesOrdersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SalesOrderDto>> Handle(GetAllSalesOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all sales orders");

            var salesOrders = await _unitOfWork.SalesOrder.GetSalesOrdersAsync();
            return _mapper.Map<List<SalesOrderDto>>(salesOrders);
        }
    }
}

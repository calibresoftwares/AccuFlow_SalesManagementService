using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.SalesOrder;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.SalesOrder.Queries
{
    public record GetSalesOrdersByCustomerIdQuery(int CustomerId) : IRequest<List<SalesOrderDto>>;

    public class GetSalesOrdersByCustomerIdQueryHandler : IRequestHandler<GetSalesOrdersByCustomerIdQuery, List<SalesOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSalesOrdersByCustomerIdQueryHandler> _logger;

        public GetSalesOrdersByCustomerIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSalesOrdersByCustomerIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SalesOrderDto>> Handle(GetSalesOrdersByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving sales orders for Customer ID {request.CustomerId}");

            var allSalesOrders = await _unitOfWork.SalesOrder.GetSalesOrdersAsync();
            var customerSalesOrders = allSalesOrders
                .Where(so => so.CustomerId == request.CustomerId)
                .ToList();

            return _mapper.Map<List<SalesOrderDto>>(customerSalesOrders);
        }
    }
}


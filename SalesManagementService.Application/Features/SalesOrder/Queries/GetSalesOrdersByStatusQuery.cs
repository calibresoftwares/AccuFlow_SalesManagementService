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
    public record GetSalesOrdersByStatusQuery(string Status) : IRequest<List<SalesOrderDto>>;

    public class GetSalesOrdersByStatusQueryHandler : IRequestHandler<GetSalesOrdersByStatusQuery, List<SalesOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSalesOrdersByStatusQueryHandler> _logger;

        public GetSalesOrdersByStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSalesOrdersByStatusQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<SalesOrderDto>> Handle(GetSalesOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving sales orders with Status {request.Status}");

            var allSalesOrders = await _unitOfWork.SalesOrder.GetSalesOrdersAsync();
            var statusSalesOrders = allSalesOrders
                .Where(so => so.Status.Equals(request.Status, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            return _mapper.Map<List<SalesOrderDto>>(statusSalesOrders);
        }
    }
}


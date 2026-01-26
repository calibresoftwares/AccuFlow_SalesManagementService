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
    public record GetSalesOrderQuery(int Id) : IRequest<SalesOrderDto>;

    public class GetSalesOrderQueryHandler : IRequestHandler<GetSalesOrderQuery, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSalesOrderQueryHandler> _logger;

        public GetSalesOrderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSalesOrderQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SalesOrderDto> Handle(GetSalesOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving sales order with ID {request.Id}");

            var salesOrder = await _unitOfWork.SalesOrder.GetByIdAsync(request.Id);
            if (salesOrder == null)
            {
                throw new KeyNotFoundException($"Sales Order with ID {request.Id} not found.");
            }

            return _mapper.Map<SalesOrderDto>(salesOrder);
        }
    }
}

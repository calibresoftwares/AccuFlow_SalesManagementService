using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.Sales.Queries
{
    public record GetAllCustomersQuery() : IRequest<List<CustomerDto>>;

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger;

        public GetAllCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCustomersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all suppliers");

            var customers = await _unitOfWork.Customer.GetCustomerAsync();
            return _mapper.Map<List<CustomerDto>>(customers);
        }
    }
}

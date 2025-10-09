using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.DTOs.Customer;
using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.Sales.Queries
{
    public record GetCustomerQuery(int Id) : IRequest<CustomerDto>;

    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomerQueryHandler> _logger;

        public GetCustomerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCustomerQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving customer with ID {request.Id}");

            var customer = await _unitOfWork.Customer.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {request.Id} not found.");
            }

            return _mapper.Map<CustomerDto>(customer);
        }
    }
}

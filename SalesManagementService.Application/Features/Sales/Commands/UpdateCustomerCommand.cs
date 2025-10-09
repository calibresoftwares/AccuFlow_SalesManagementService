using AutoMapper;
using FluentValidation;
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

namespace SalesManagementService.Application.Features.Sales.Commands
{
    public record UpdateCustomerCommand(CustomerDto CustomerDto) : IRequest<CustomerDto>;

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;
        private readonly IValidator<UpdateCustomerCommand> _validator;

        public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateCustomerCommandHandler> logger, IValidator<UpdateCustomerCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating Customer with ID {request.CustomerDto.CustomerId}");

            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var customer = await _unitOfWork.Customer.GetByIdAsync(request.CustomerDto.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {request.CustomerDto.CustomerId} not found.");
            }

            _mapper.Map(request.CustomerDto, customer);
            await _unitOfWork.Customer.UpdateAsync(customer);
            return _mapper.Map<CustomerDto>(customer);
        }
    }
}


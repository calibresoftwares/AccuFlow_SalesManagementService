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
   
        public record CreateCustomerCommand(CreateCustomerDto CustomerDto) : IRequest<CustomerDto>;

        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;
            private readonly ILogger<CreateCustomerCommandHandler> _logger;
            private readonly IValidator<CreateCustomerCommand> _validator;
           
            public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCustomerCommandHandler> logger, IValidator<CreateCustomerCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
                _mapper = mapper;
                _validator = validator;
            }
            /// <summary>
            /// handle the customer creation event
            /// </summary>
            /// <param name="request"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            /// <exception cref="ValidationException"></exception>
            public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Creating New Customer");
                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }
                var customer = _mapper.Map<SalesManagementService.Domain.Entities.Customer>(request.CustomerDto);
                await _unitOfWork.Customer.AddAsync(customer);
                return _mapper.Map<CustomerDto>(customer);
            }
        }
    }


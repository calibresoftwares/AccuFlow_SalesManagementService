using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesManagementService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.Sales.Commands
{
    public record DeleteCustomerCommand(int Id) : IRequest<bool>;

    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCustomerCommandHandler> _logger;
        private readonly IValidator<DeleteCustomerCommand> _validator;


        public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeleteCustomerCommandHandler> logger, IValidator<DeleteCustomerCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }


        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Deleting customer for Id {request.Id}");
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var customer = await _unitOfWork.Customer.DeleteAsync(request.Id);

            _logger.LogInformation("Customer Deleted Successfully.");
            return true;
        }

    }






}


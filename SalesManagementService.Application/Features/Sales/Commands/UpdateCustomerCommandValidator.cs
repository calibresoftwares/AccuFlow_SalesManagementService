using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.Sales.Commands
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerDto.CustomerId).GreaterThan(0).WithMessage("Valid customer ID is required.");
            RuleFor(x => x.CustomerDto.CustomerName).NotEmpty().WithMessage("customer Name is required.");
            RuleFor(x => x.CustomerDto.Email).EmailAddress().WithMessage("Invalid email format.");
        }
    }
}

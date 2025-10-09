using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Application.Features.Sales.Commands
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerDto.CustomerName).NotEmpty().WithMessage("Customer Name is required.");
            RuleFor(x => x.CustomerDto.Email).EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.CustomerDto.MobileNumber).NotEmpty().WithMessage("Mobile number is required.");
            RuleFor(x => x.CustomerDto.GSTNumber).Length(15).WithMessage("GST Number must be 15 characters.");
            RuleFor(x => x.CustomerDto.PANNumber).Length(10).WithMessage("PAN Number must be 10 characters.");
        }
    }
}

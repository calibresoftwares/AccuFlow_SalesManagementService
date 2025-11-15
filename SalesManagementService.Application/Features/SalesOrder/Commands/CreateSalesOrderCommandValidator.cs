using FluentValidation;
using SalesManagementService.Domain.DTOs.SalesOrder;

namespace SalesManagementService.Application.Features.SalesOrder.Commands
{
    public class CreateSalesOrderCommandValidator : AbstractValidator<CreateSalesOrderCommand>
    {
        public CreateSalesOrderCommandValidator()
        {
            RuleFor(x => x.SalesOrderDto.CustomerId)
                .GreaterThan(0)
                .WithMessage("Customer ID is required and must be greater than 0.");

            RuleFor(x => x.SalesOrderDto.OrderDate)
                .NotEmpty()
                .WithMessage("Order Date is required.");

            RuleFor(x => x.SalesOrderDto.ExpectedDeliveryDate)
                .NotEmpty()
                .WithMessage("Expected Delivery Date is required.")
                .GreaterThan(x => x.SalesOrderDto.OrderDate)
                .WithMessage("Expected Delivery Date must be after Order Date.");

            RuleFor(x => x.SalesOrderDto.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total Amount must be greater than or equal to 0.");

            RuleFor(x => x.SalesOrderDto.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status => new[] { "Pending", "Confirmed", "Shipped", "Completed", "Cancelled" }.Contains(status))
                .WithMessage("Status must be one of: Pending, Confirmed, Shipped, Completed, Cancelled.");

            RuleFor(x => x.SalesOrderDto.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required.");

            RuleFor(x => x.SalesOrderDto.SalesOrderLineItems)
                .NotEmpty()
                .WithMessage("At least one line item is required.");

            RuleForEach(x => x.SalesOrderDto.SalesOrderLineItems)
                .SetValidator(new CreateSalesOrderLineItemDtoValidator());
        }
    }

    public class CreateSalesOrderLineItemDtoValidator : AbstractValidator<CreateSalesOrderLineItemDto>
    {
        public CreateSalesOrderLineItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Product ID is required and must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit Price must be greater than or equal to 0.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount must be greater than or equal to 0.");

            RuleFor(x => x.Tax)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax must be greater than or equal to 0.");

            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required for line item.");
        }
    }
}


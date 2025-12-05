using FluentValidation;
using SalesManagementService.Domain.DTOs.SalesInvoice;
using SalesManagementService.Domain.Entities;

namespace SalesManagementService.Application.Features.SalesInvoice.Commands
{
    public class UpdateSalesInvoiceCommandValidator : AbstractValidator<UpdateSalesInvoiceCommand>
    {
        public UpdateSalesInvoiceCommandValidator()
        {
            RuleFor(x => x.SalesInvoiceDto.SalesInvoiceId)
                .NotEmpty()
                .WithMessage("Sales Invoice ID is required.");

            RuleFor(x => x.SalesInvoiceDto.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required.");

            RuleFor(x => x.SalesInvoiceDto.InvoiceDate)
                .NotEmpty()
                .WithMessage("Invoice Date is required.");

            RuleFor(x => x.SalesInvoiceDto.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total Amount must be greater than or equal to 0.");

            RuleFor(x => x.SalesInvoiceDto.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount Amount must be greater than or equal to 0.");

            RuleFor(x => x.SalesInvoiceDto.TaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax Amount must be greater than or equal to 0.");

            RuleFor(x => x.SalesInvoiceDto.NetAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Net Amount must be greater than or equal to 0.");

            RuleFor(x => x.SalesInvoiceDto.Status)
                .IsInEnum()
                .WithMessage("Status must be a valid InvoiceStatus value.");

            RuleFor(x => x.SalesInvoiceDto.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required.");

            RuleForEach(x => x.SalesInvoiceDto.LineItems)
                .SetValidator(new SalesInvoiceLineItemDtoValidator());
        }
    }

    public class SalesInvoiceLineItemDtoValidator : AbstractValidator<SalesInvoiceLineItemDto>
    {
        public SalesInvoiceLineItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Unit Price must be greater than or equal to 0.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Discount must be greater than or equal to 0.");

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tax Amount must be greater than or equal to 0.");

            RuleFor(x => x.TenantId)
                .NotEmpty()
                .WithMessage("Tenant ID is required for line item.");
        }
    }
}


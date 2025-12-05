using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SalesManagementService.Application.Features.SalesInvoice.Commands;
using SalesManagementService.Application.Features.SalesInvoice.Queries;
using SalesManagementService.Domain.DTOs.SalesInvoice;
using Microsoft.Extensions.Logging;

namespace SalesManagementService.API.Endpoints
{
    public static class SalesInvoiceApi
    {
        public static WebApplication MapSalesInvoiceEndpoints(this WebApplication webApplication)
        {
            var salesInvoiceGroup = webApplication.MapGroup("api/salesinvoice");

            salesInvoiceGroup.MapPost("", [Authorize(Roles = "SuperAdmin,Admin")] async (CreateSalesInvoiceDto dto, ILogger<Program> logger, IMediator mediator, IValidator<CreateSalesInvoiceCommand> validator)
                => await CreateSalesInvoice(dto, logger, mediator, validator))
                .WithName("CreateSalesInvoice")
                .WithDisplayName("Create Sales Invoice");

            salesInvoiceGroup.MapPut("update", [Authorize(Roles = "SuperAdmin,Admin")] async (SalesInvoiceDto dto, ILogger<Program> logger, IMediator mediator, IValidator<UpdateSalesInvoiceCommand> validator)
                => await UpdateSalesInvoice(dto, logger, mediator, validator))
                .WithName("UpdateSalesInvoice")
                .WithDisplayName("Update Sales Invoice");

            salesInvoiceGroup.MapDelete("delete/{id:guid}", [Authorize(Roles = "SuperAdmin,Admin")] async (Guid id, ILogger<Program> logger, IMediator mediator)
                => await DeleteSalesInvoice(id, logger, mediator))
                .WithName("DeleteSalesInvoice")
                .WithDisplayName("Delete Sales Invoice");

            salesInvoiceGroup.MapGet("{id:guid}", [Authorize(Roles = "SuperAdmin,Admin")] async (Guid id, ILogger<Program> logger, IMediator mediator)
                => await GetSalesInvoice(id, logger, mediator))
                .WithName("GetSalesInvoice")
                .WithDisplayName("Get Sales Invoice");

            salesInvoiceGroup.MapGet("all", [Authorize(Roles = "SuperAdmin,Admin")] async (ILogger<Program> logger, IMediator mediator)
                => await GetAllSalesInvoices(logger, mediator))
                .WithName("GetAllSalesInvoices")
                .WithDisplayName("Get All Sales Invoices");

           return webApplication;
        }

        private static async Task<IResult> CreateSalesInvoice(CreateSalesInvoiceDto dto, ILogger<Program> logger, IMediator mediator, IValidator<CreateSalesInvoiceCommand> validator)
        {
            logger.LogInformation("Creating new sales invoice");
            try
            {
                var command = new CreateSalesInvoiceCommand(dto);
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }
                var createdSalesInvoice = await mediator.Send(command);
                return Results.Ok(createdSalesInvoice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create sales invoice");
                return Results.Problem("Failed to create sales invoice");
            }
        }
       

        private static async Task<IResult> UpdateSalesInvoice(SalesInvoiceDto dto, ILogger<Program> logger, IMediator mediator, IValidator<UpdateSalesInvoiceCommand> validator)
        {
            logger.LogInformation("Updating sales invoice");
            try
            {
                var command = new UpdateSalesInvoiceCommand(dto);
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }
                var updatedSalesInvoice = await mediator.Send(command);
                return Results.Ok(updatedSalesInvoice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update sales invoice");
                return Results.Problem("Failed to update sales invoice");
            }
        }

        private static async Task<IResult> DeleteSalesInvoice(Guid id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Deleting sales invoice with ID {id}");
            try
            {
                var command = new DeleteSalesInvoiceCommand(id);
                var deleted = await mediator.Send(command);
                return deleted ? Results.Ok() : Results.NotFound($"Sales Invoice with ID {id} not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete sales invoice");
                return Results.Problem("Failed to delete sales invoice");
            }
        }

        private static async Task<IResult> GetSalesInvoice(Guid id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Fetching sales invoice with ID {id}");
            try
            {
                var query = new GetSalesInvoiceQuery(id);
                var salesInvoice = await mediator.Send(query);
                return salesInvoice != null ? Results.Ok(salesInvoice) : Results.NotFound($"Sales Invoice with ID {id} not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch sales invoice");
                return Results.Problem("Failed to fetch sales invoice");
            }
        }

        private static async Task<IResult> GetAllSalesInvoices(ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation("Fetching all sales invoices");
            try
            {
                var query = new GetAllSalesInvoicesQuery();
                var salesInvoices = await mediator.Send(query);
                return Results.Ok(salesInvoices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch all sales invoices");
                return Results.Problem("Failed to fetch sales invoices");
            }
        }
    }
}


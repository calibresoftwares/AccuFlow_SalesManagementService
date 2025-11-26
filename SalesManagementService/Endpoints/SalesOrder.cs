using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SalesManagementService.Application.Features.SalesOrder.Commands;
using SalesManagementService.Application.Features.SalesOrder.Queries;
using SalesManagementService.Domain.DTOs.SalesOrder;
using Microsoft.Extensions.Logging;

namespace SalesManagementService.API.Endpoints
{
    public static class SalesOrderApi
    {
        public static WebApplication MapSalesOrderEndpoints(this WebApplication webApplication)
        {
            var salesOrderGroup = webApplication.MapGroup("api/salesorder");

            salesOrderGroup.MapPost("", [Authorize(Roles = "SuperAdmin,Admin")] async (CreateSalesOrderDto dto, ILogger<Program> logger, IMediator mediator, IValidator<CreateSalesOrderCommand> validator)
                => await CreateSalesOrder(dto, logger, mediator, validator))
                .WithName("CreateSalesOrder")
                .WithDisplayName("Create Sales Order");

            salesOrderGroup.MapPut("update", [Authorize(Roles = "SuperAdmin,Admin")] async (SalesOrderDto dto, ILogger<Program> logger, IMediator mediator, IValidator<UpdateSalesOrderCommand> validator)
                => await UpdateSalesOrder(dto, logger, mediator, validator))
                .WithName("UpdateSalesOrder")
                .WithDisplayName("Update Sales Order");

            salesOrderGroup.MapDelete("delete/{id:guid}", [Authorize(Roles = "SuperAdmin,Admin")] async (Guid id, ILogger<Program> logger, IMediator mediator)
                => await DeleteSalesOrder(id, logger, mediator))
                .WithName("DeleteSalesOrder")
                .WithDisplayName("Delete Sales Order");

            salesOrderGroup.MapGet("{id:guid}", [Authorize(Roles = "SuperAdmin,Admin")] async (Guid id, ILogger<Program> logger, IMediator mediator)
                => await GetSalesOrder(id, logger, mediator))
                .WithName("GetSalesOrder")
                .WithDisplayName("Get Sales Order");

            salesOrderGroup.MapGet("all", [Authorize(Roles = "SuperAdmin,Admin")] async (ILogger<Program> logger, IMediator mediator)
                => await GetAllSalesOrders(logger, mediator))
                .WithName("GetAllSalesOrders")
                .WithDisplayName("Get All Sales Orders");

           return webApplication;
        }

        private static async Task<IResult> CreateSalesOrder(CreateSalesOrderDto dto, ILogger<Program> logger, IMediator mediator, IValidator<CreateSalesOrderCommand> validator)
        {
            logger.LogInformation("Creating new sales order");
            try
            {
                var command = new CreateSalesOrderCommand(dto);
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }
                var createdSalesOrder = await mediator.Send(command);
                return Results.Ok(createdSalesOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create sales order");
                return Results.Problem("Failed to create sales order");
            }
        }
       

        private static async Task<IResult> UpdateSalesOrder(SalesOrderDto dto, ILogger<Program> logger, IMediator mediator, IValidator<UpdateSalesOrderCommand> validator)
        {
            logger.LogInformation("Updating sales order");
            try
            {
                var command = new UpdateSalesOrderCommand(dto);
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.Errors);
                }
                var updatedSalesOrder = await mediator.Send(command);
                return Results.Ok(updatedSalesOrder);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update sales order");
                return Results.Problem("Failed to update sales order");
            }
        }

        private static async Task<IResult> DeleteSalesOrder(Guid id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Deleting sales order with ID {id}");
            try
            {
                var command = new DeleteSalesOrderCommand(id);
                var deleted = await mediator.Send(command);
                return deleted ? Results.Ok() : Results.NotFound($"Sales Order with ID {id} not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete sales order");
                return Results.Problem("Failed to delete sales order");
            }
        }

        private static async Task<IResult> GetSalesOrder(Guid id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Fetching sales order with ID {id}");
            try
            {
                var query = new GetSalesOrderQuery(id);
                var salesOrder = await mediator.Send(query);
                return salesOrder != null ? Results.Ok(salesOrder) : Results.NotFound($"Sales Order with ID {id} not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch sales order");
                return Results.Problem("Failed to fetch sales order");
            }
        }

        private static async Task<IResult> GetAllSalesOrders(ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation("Fetching all sales orders");
            try
            {
                var query = new GetAllSalesOrdersQuery();
                var salesOrders = await mediator.Send(query);
                return Results.Ok(salesOrders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch all sales orders");
                return Results.Problem("Failed to fetch sales orders");
            }
        }
    }
}

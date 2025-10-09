using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SalesManagementService.Application.Features.Sales.Commands;
using SalesManagementService.Application.Features.Sales.Queries;
using SalesManagementService.Domain.DTOs.Customer;
using Shared.Common.Exceptions;
using Shared.Common.ResponseTypes;

namespace SalesManagementService.API.Endpoints
{ 
     public static class Sales
    {
        public static WebApplication MapCustomerEndpoints(this WebApplication webApplication)
        {
            var customerGroup = webApplication.MapGroup("api/customer");
            //.RequireAuthorization(new AuthorizeAttribute { Roles = "SuperAdmin,Admin"});

            _ = customerGroup.MapPost("", [Authorize(Roles = "SuperAdmin,Admin")] async (CreateCustomerDto dto, HttpContext context, ILogger<Program> logger, IMediator mediator ) 
                => await CreateCustomer(dto, logger, mediator))
                .WithName("CreateCustomer")
                .WithDisplayName("Create Customer");

            
            _ = customerGroup.MapPost("delete/{id:int}", [Authorize(Roles = "SuperAdmin,Admin")] (int id, ILogger<Program> logger, IMediator mediator) => DeleteCustomer(id, logger, mediator))
                .WithName("DeleteCustomer")
                .WithDisplayName("DeleteCustomer");


            _ = customerGroup.MapPut("update", [Authorize(Roles = "SuperAdmin,Admin")] (CustomerDto dto, ILogger<Program> logger, IMediator mediator) => UpdateCustomer(dto, logger, mediator))
                .WithName("UpdateCustomer")
                .WithDisplayName("Update Customer");

            _ = customerGroup.MapGet("{id:int}", [Authorize(Roles = "SuperAdmin,Admin")] (int id, ILogger<Program> logger, IMediator mediator) => GetCustomer(id, logger, mediator))
                .WithName("GetCustomer")
                .WithDisplayName("Get Customer");

            _ = customerGroup.MapGet("all", [Authorize(Roles = "SuperAdmin,Admin")] (ILogger<Program> logger, IMediator mediator) => GetAllCustomers(logger, mediator))
                .WithName("GetAllCustomers")
                .WithDisplayName("Get All Customers");

            return webApplication;
        }

        private static async Task<IResult> CreateCustomer(CreateCustomerDto dto, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation("createCustomer endpoint was invoked.");
            try
            {
                var command = new CreateCustomerCommand(dto);
                var createdCustomer = await mediator.Send(command);
                //return Results.Created($"/api/product/{createdProduct.CategoryID}", createdProduct);
                return Results.Ok(ApiResponse<CustomerDto>.Success(createdCustomer));
               
            }
            catch (NotFoundException ex)
            {
                //return Results.Problem(string.Format(CultureInfo.CurrentCulture, "{0} not found.", ex.InputType));
                return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
            catch (FluentValidation.ValidationException ex)
            {
                //return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
                var errorMessages = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(ApiResponse<string>.Failure("Validation error: " + errorMessages));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get account lookups.");
                 return Results.Problem(ApiResponse<string>.Failure("Failed Getting Lookups.").Message);
            }
        }

        private static async Task<IResult> DeleteCustomer(int id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation("Delete a customer endpoint was invoked.");
            try
            {
                var command = new DeleteCustomerCommand(id);
                var deleted = await mediator.Send(command);
                return deleted
                    ? Results.Ok(ApiResponse<string>.Success("", "user deleted successfully", 204))
                    : Results.NotFound(ApiResponse<string>.Failure($"User with ID {id} not found.", 404));
            }
            catch (NotFoundException ex)
            {
                //return Results.Problem(string.Format(CultureInfo.CurrentCulture, "{0} not found.", ex.InputType));
                return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
            catch (FluentValidation.ValidationException ex)
            {
                //return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
                var errorMessages = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(ApiResponse<string>.Failure("Validation error: " + errorMessages));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get account lookups.");
                return Results.Problem(ApiResponse<string>.Failure("Failed Getting Lookups.").Message);
            }

        }

        private static async Task<IResult> UpdateCustomer(CustomerDto dto, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Updating customer with ID {dto.CustomerId}");
            try
            {
                var command = new UpdateCustomerCommand(dto);
                var updatedCustomer = await mediator.Send(command);
                return Results.Ok(ApiResponse<CustomerDto>.Success(updatedCustomer));
            }
            catch (NotFoundException ex)
            {
                return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
            catch (FluentValidation.ValidationException ex)
            {
                
                var errorMessages = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(ApiResponse<string>.Failure("Validation error: " + errorMessages));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get account lookups.");
                return Results.Problem(ApiResponse<string>.Failure("Failed Getting Lookups.").Message);
            }
        }

        private static async Task<IResult> GetCustomer(int id, ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation($"Fetching customer with ID {id}");
            try
            {
                var query = new GetCustomerQuery(id);
                var customer = await mediator.Send(query);
                return customer != null ? Results.Ok(customer) : Results.NotFound($"Customer with ID {id} not found.");
            }
            catch (NotFoundException ex)
            {
                //return Results.Problem(string.Format(CultureInfo.CurrentCulture, "{0} not found.", ex.InputType));
                return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
            catch (FluentValidation.ValidationException ex)
            {
                //return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
                var errorMessages = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(ApiResponse<string>.Failure("Validation error: " + errorMessages));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get account lookups.");
                return Results.Problem(ApiResponse<string>.Failure("Failed Getting Lookups.").Message);
            }
        }

        private static async Task<IResult> GetAllCustomers(ILogger<Program> logger, IMediator mediator)
        {
            logger.LogInformation("Fetching all customers");
            try
            {
                var query = new GetAllCustomersQuery();
                var customers = await mediator.Send(query);
                //return Results.Ok(customers);
                return Results.Ok(ApiResponse<List<CustomerDto>>.Success(customers));
            }
            catch (NotFoundException ex)
            {
                //return Results.Problem(string.Format(CultureInfo.CurrentCulture, "{0} not found.", ex.InputType));
                return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
            catch (FluentValidation.ValidationException ex)
            {
                //return Results.BadRequest(ApiResponse<string>.Failure(ex.Message));
                var errorMessages = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(ApiResponse<string>.Failure("Validation error: " + errorMessages));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get account lookups.");
                return Results.Problem(ApiResponse<string>.Failure("Failed Getting Lookups.").Message);
            }
        }




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SalesManagementService.Application.Features.Sales.Commands;

namespace SalesManagementService.Application
{
    public static class ConfigureServices
    {
        public static void AddInjectionApplication(this IServiceCollection services)
        {
            //services.AddScoped<IJwtService, JwtService>();
            //var assembly = typeof(ConfigureServices).Assembly;
            ////_ = services.AddValidatorsFromAssembly(assembly);
            //_ = services.AddMediatR(configuration =>
            //    configuration.RegisterServicesFromAssembly(assembly));
            //_ = services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();




            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            //services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            //services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
    }
}

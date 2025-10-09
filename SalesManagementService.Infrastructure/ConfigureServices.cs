using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Domain.TenantSettings;
using SalesManagementService.Infrastructure.Kafka;

namespace SalesManagementService.Infrastructure
{
    public static class ConfigureServices
    {
        public static void AddInfrastructureInjectionServices(this IServiceCollection services, IConfiguration configuration)
        {
            IServiceCollection serviceCollection = services.Configure<KafkaSettings>(options => configuration.GetSection("Kafka"));
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
        }
    }   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Domain.TenantSettings;

namespace SalesManagementService.Infrastructure.TenantConfig
{
    public class DesignTimeTenantService : ITenantService
    {
        private readonly Domain.TenantSettings.Tenant _tenant;

        public DesignTimeTenantService(string tenantId, string connectionString)
        {
            _tenant = new Domain.TenantSettings.Tenant
            {
                TenantId = Guid.Parse(tenantId),
                ConnectionString = connectionString
            };
        }
        //public Tenant GetTenant() => _tenant;
        Guid ITenantService.TenantId => throw new NotImplementedException();

        public Task<Domain.TenantSettings.Tenant> GetConnectionStringAsync()
        {
            return Task.FromResult(new Tenant { ConnectionString = _tenant.ConnectionString, TenantId = _tenant.TenantId });
        }

        public Guid GetTenantId()
        {
            return Guid.Parse("00000000-0000-0000-0000-000000000000");
        }
    }
}

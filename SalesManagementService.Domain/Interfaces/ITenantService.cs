using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.TenantSettings;

namespace SalesManagementService.Domain.Interfaces
{
    public interface ITenantService
    {
        public Task<Tenant> GetConnectionStringAsync();

        public Guid GetTenantId();

        Guid TenantId { get; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.TenantSettings
{
    public class TenantSettings
    {
        public Configuration Defaults { get; set; }
        public List<Tenant> Tenants { get; set; }
    }
    public class Tenant
    {
        public string Name { get; set; }
        public Guid TenantId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class Configuration
    {
        public string DBProvider { get; set; }
        public string ConnectionString { get; set; }
    }
}

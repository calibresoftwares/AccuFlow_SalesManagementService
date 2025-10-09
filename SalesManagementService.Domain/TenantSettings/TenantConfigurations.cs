using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.TenantSettings
{
    public class TenantConfigurations
    {
        public Guid TenantId { get; set; }
        public string ConnectionString { get; set; }
    }
}

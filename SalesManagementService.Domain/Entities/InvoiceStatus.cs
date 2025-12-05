using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Entities
{
    public enum InvoiceStatus
    {
        Pending = 1,
        Generated = 2,
        Sent = 3,
        Paid = 4,
        Cancelled = 5,
        Closed = 6
    }
}


using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<List<SalesOrder>> GetSalesOrdersAsync();
        Task<SalesOrder> AddAsync(SalesOrder salesOrder);
        Task<SalesOrder> GetByIdAsync(int salesOrderId);
        Task<SalesOrder> UpdateAsync(SalesOrder salesOrder);
        Task<bool> DeleteAsync(int salesOrderId);
    }
}

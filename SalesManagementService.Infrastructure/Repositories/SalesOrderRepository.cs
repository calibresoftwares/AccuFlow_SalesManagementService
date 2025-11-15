using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.Entities;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Infrastructure.DatabaseManager;

namespace SalesManagementService.Infrastructure.Repositories
{
    public class SalesOrderRepository : Repository<SalesOrder>, ISalesOrderRepository
    {
        private readonly SalesManagementDbContext _dbContext;
        public SalesOrderRepository(SalesManagementDbContext salesManagementDbContext) : base(salesManagementDbContext)
        {
            _dbContext = salesManagementDbContext;
        }

        public async Task<SalesOrder> AddAsync(SalesOrder salesOrder)
        {
            try
            {
                _dbContext.SalesOrders.Add(salesOrder);
                await _dbContext.SaveChangesAsync();
                return salesOrder;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int salesOrderId)
        {
            var removeSalesOrder = _dbContext.SalesOrders.Find(salesOrderId);
            if (removeSalesOrder != null)
            {
                _dbContext.SalesOrders.Remove(removeSalesOrder);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<SalesOrder> GetByIdAsync(int salesOrderId)
        {
            return await _dbContext.SalesOrders
                .Include(so => so.LineItems)
                .FirstOrDefaultAsync(so => so.SalesOrderId == salesOrderId);
        }

        public async Task<List<SalesOrder>> GetSalesOrdersAsync()
        {
            return await _dbContext.SalesOrders
                .Include(so => so.LineItems)
                .ToListAsync();
        }

        public async Task<SalesOrder> UpdateAsync(SalesOrder salesOrder)
        {
            _dbContext.SalesOrders.Update(salesOrder);
            await _dbContext.SaveChangesAsync();
            return salesOrder;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.Entities;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Infrastructure.DatabaseManager;


namespace SalesManagementService.Infrastructure.Repositories
{
    public class SalesInvoiceRepository : Repository<SalesInvoice>, ISalesInvoiceRepository
    {
        private readonly SalesManagementDbContext _dbContext;
        private readonly ITenantService _tenantService;

        public SalesInvoiceRepository(SalesManagementDbContext salesManagementDbContext, ITenantService tenantService) : base(salesManagementDbContext)
        {
            _dbContext = salesManagementDbContext;
            _tenantService = tenantService;
        }

        public async Task<SalesInvoice> AddAsync(SalesInvoice salesInvoice)
        {
            try
            {
                salesInvoice.TenantId = _tenantService.TenantId;
                _dbContext.SalesInvoices.Add(salesInvoice);
                await _dbContext.SaveChangesAsync();
                return salesInvoice;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid salesInvoiceId)
        {
            var removeSalesInvoice = await _dbContext.SalesInvoices
                .Include(si => si.LineItems)
                .FirstOrDefaultAsync(si => si.SalesInvoiceId == salesInvoiceId);
            if (removeSalesInvoice != null)
            {
                // Remove line items first
                if (removeSalesInvoice.LineItems != null && removeSalesInvoice.LineItems.Any())
                {
                    _dbContext.SalesInvoiceLineItems.RemoveRange(removeSalesInvoice.LineItems);
                }
                _dbContext.SalesInvoices.Remove(removeSalesInvoice);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<SalesInvoice> GetByIdAsync(Guid salesInvoiceId)
        {
            return await _dbContext.SalesInvoices
                .Include(si => si.LineItems)
                .FirstOrDefaultAsync(si => si.SalesInvoiceId == salesInvoiceId);
        }

        public async Task<List<SalesInvoice>> GetSalesInvoicesAsync()
        {
            return await _dbContext.SalesInvoices
                .Include(si => si.LineItems)
                .ToListAsync();
        }

        public async Task<SalesInvoice> UpdateAsync(SalesInvoice salesInvoice)
        {
            _dbContext.SalesInvoices.Update(salesInvoice);
            await _dbContext.SaveChangesAsync();
            return salesInvoice;
        }
    }
}


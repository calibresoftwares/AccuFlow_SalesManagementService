using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Interfaces
{
    public interface ISalesInvoiceRepository
    {
        Task<List<SalesInvoice>> GetSalesInvoicesAsync();
        Task<SalesInvoice> AddAsync(SalesInvoice salesInvoice);
        Task<SalesInvoice> GetByIdAsync(Guid salesInvoiceId);
        Task<SalesInvoice> UpdateAsync(SalesInvoice salesInvoice);
        Task<bool> DeleteAsync(Guid salesInvoiceId);
    }
}


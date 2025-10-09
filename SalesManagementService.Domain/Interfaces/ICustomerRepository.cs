using SalesManagementService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetCustomerAsync();
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> GetByIdAsync(int customerId);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int customerId);
    }
}

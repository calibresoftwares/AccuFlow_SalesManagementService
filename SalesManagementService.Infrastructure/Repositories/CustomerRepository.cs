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
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly SalesManagementDbContext _dbContext;
        public CustomerRepository(SalesManagementDbContext salesManagementDbContext) : base(salesManagementDbContext)
        {
            _dbContext = salesManagementDbContext;
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            try
            {
                _dbContext.Customers.Add(customer);
                await _dbContext.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            var removecustomer = _dbContext.Customers.Find(customerId);
            if (removecustomer != null)
            {
                _dbContext.Customers.Remove(removecustomer);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<Customer> GetByIdAsync(int customersId)
        {
            return await _dbContext.Customers.FindAsync(customersId);
        }

        public async Task<List<Customer>> GetCustomerAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}


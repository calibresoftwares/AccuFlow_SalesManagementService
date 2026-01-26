using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.Common;
using SalesManagementService.Domain.Interfaces;
using SalesManagementService.Infrastructure.DatabaseManager;
using SalesManagementService.Infrastructure.Repositories;

namespace SalesManagementService.Infrastructure.Config
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly SalesManagementDbContext _salesManagementDbContext;
       
        private readonly IKafkaProducerService _kafkaProducerService;

        ICustomerRepository _customerRepository;
        ISalesOrderRepository _salesOrderRepository;
        //IGoodsReceivedNoteRepository _goodsReceivedNoteRepository;
        //ICustomerPaymentRepository _customerPaymentRepository;

        public UnitOfWork(SalesManagementDbContext salesManagementDbContext, IKafkaProducerService kafkaProducerService)
        {
            _salesManagementDbContext = salesManagementDbContext;
            _kafkaProducerService = kafkaProducerService;
        }

        public ICustomerRepository Customer => _customerRepository = _customerRepository ?? new CustomerRepository(_salesManagementDbContext);
        public ISalesOrderRepository SalesOrder => _salesOrderRepository = _salesOrderRepository ?? new SalesOrderRepository(_salesManagementDbContext);

       public int Commit()
        {
            return this._salesManagementDbContext.SaveChanges();
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await this._salesManagementDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (this._salesManagementDbContext != null)
            {
                this._salesManagementDbContext.Dispose();
            }
        }
    }
}

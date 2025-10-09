using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.Interfaces;

namespace SalesManagementService.Domain.Common
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customer { get; }
        

        //ICustomerPaymentRepository CustomerPaymentRepository { get; }
        //IGoodsReceivedNoteRepository GoodsReceivedNoteRepository { get; }
        int Commit();
        Task<int> CommitAsync(CancellationToken cancellationToken);
    }
}

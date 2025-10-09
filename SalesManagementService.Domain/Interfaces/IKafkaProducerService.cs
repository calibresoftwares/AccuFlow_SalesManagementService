using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Domain.Interfaces
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string topic, object message);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManagementService.Infrastructure.Kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
        public int MessageTimeoutMs { get; set; }
        public int RequestTimeoutMs { get; set; }
        public int SocketTimeoutMs { get; set; }

    }
}

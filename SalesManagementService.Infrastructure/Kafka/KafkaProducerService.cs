using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SalesManagementService.Domain.Interfaces;
using Newtonsoft.Json;

namespace SalesManagementService.Infrastructure.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly KafkaSettings _settings;
        public KafkaProducerService(IOptions<KafkaSettings> settings, IConfiguration configuration, ILogger<KafkaProducerService> logger) {
            _logger = logger;
            _settings = settings.Value;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                SecurityProtocol = SecurityProtocol.Plaintext,
                MessageTimeoutMs = _settings.MessageTimeoutMs,
                RequestTimeoutMs = _settings.RequestTimeoutMs,   // 5 seconds
                SocketTimeoutMs = _settings.SocketTimeoutMs,
                Acks = Acks.All,
                EnableDeliveryReports = true,
                Debug = "broker,security,protocol"  // Enable detailed logging
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig)
                    .SetLogHandler((_, log) => Console.WriteLine($"Kafka Log: {log.Message}"))
                    .SetErrorHandler((_, error) => Console.WriteLine($"Kafka Error: {error.Reason}"))
                    .Build();
            //.SetLogHandler((_, m) =>
            //    _logger.LogInformation($"Kafka Log: {m.Message}"))
            //.Build();
            // _topic = "customer-invoice-events";
        }

        public async Task ProduceAsync(string eventType, object payload)
        {
            try
            {
                var wrapper = new
                {
                    eventType,
                    timestamp = DateTime.UtcNow,
                    payload
                };

                var message = new Message<Null, string>
                {
                    Value = JsonConvert.SerializeObject(wrapper)
                };
                //var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                var deliveryResult = await _producer.ProduceAsync("stock-change-events", message);
                Console.WriteLine($"Delivered message to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}

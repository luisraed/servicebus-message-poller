using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MessagePollerService
{
    public interface IServiceBusClientFactory
    {
        ServiceBusClient? Create();
    }
    internal class ServiceBusClientFactory : IServiceBusClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceBusClientFactory> _logger;

        private ServiceBusClient? _client;
        public ServiceBusClientFactory(IConfiguration configuration, ILogger<ServiceBusClientFactory> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public ServiceBusClient? Create()
        {
            var serviceBusNamespace = _configuration["AzureServiceBus:FullyQualifiedNamespace"];
            
            if (string.IsNullOrEmpty(serviceBusNamespace))
            {
                _logger.LogError("Azure Service Bus configuration is missing. Check appsettings.json.");
                return null;
            }

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            return new ServiceBusClient(serviceBusNamespace, new DefaultAzureCredential(), clientOptions);
        }
    }
}

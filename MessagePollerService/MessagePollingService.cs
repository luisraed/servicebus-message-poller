using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagePollerService
{
    internal class MessagePollingService : BackgroundService
    {
        private readonly IServiceBusClientFactory _serviceBusClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessagePollingService> _logger;
        private ServiceBusClient? _client;
        private ServiceBusProcessor? _processor;

        public MessagePollingService(IServiceBusClientFactory serviceBusClientFactory, IConfiguration configuration, ILogger<MessagePollingService> logger)
        {
            _serviceBusClientFactory = serviceBusClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service Bus Message Polling Service...");

            if (_processor != null)
            {
                await _processor.StartProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }

            if (_client != null)
            {
                await _client.DisposeAsync();
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Service Bus Message Polling Service ...");

            _client = _serviceBusClientFactory.Create();

            if (_client == null)
            {
                _logger.LogError("Azure Service Bus Client could not be created.");
                return;
            }

            var queueName = _configuration["AzureServiceBus:QueueName"];
            _processor = _client.CreateProcessor(queueName);
            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                string body = args.Message.Body.ToString();
                _logger.LogInformation($"Received message: {body}");

                // Process the message

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Processing Message");
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus Error");
            return Task.CompletedTask;
        }


    }
}

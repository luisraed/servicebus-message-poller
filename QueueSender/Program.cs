using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace QueueSender
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var fullyQualifiedNamespace = configuration.GetValue<string>("AzureServiceBus:FullyQualifiedNamespace");
            var queueName = configuration.GetValue<string>("AzureServiceBus:QueueName");

            const int numOfMessages = 3;
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            ServiceBusClient client = new ServiceBusClient(
                                                           fullyQualifiedNamespace,
                                                           new DefaultAzureCredential(),
                                                           clientOptions);

            ServiceBusSender sender = client.CreateSender(queueName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 0; i < numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} has been published to the queue.");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("Press Any Key to end the application");
            Console.ReadKey();
        }
    }
}

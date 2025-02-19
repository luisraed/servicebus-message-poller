using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MessagePollerService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
             
            builder.Services.AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>();
            builder.Services.AddHostedService<MessagePollingService>();
            builder.Services.AddLogging(config => config.AddConsole());

            var app = builder.Build();
            app.Run();
        }
    }
}

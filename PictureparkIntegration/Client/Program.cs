using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Client.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmintIo.CLAPI.Consumer.Integration.Core.Factory;
using SmintIo.CLAPI.Consumer.Integration.Core.Services;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new HostBuilder()
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.SetBasePath(Directory.GetCurrentDirectory());
                        configHost.AddJsonFile("hostsettings.json", optional: true);

                        if (args != null)
                        {
                            configHost.AddCommandLine(args);
                        }
                    })
                    .ConfigureAppConfiguration((hostContext, configApp) =>
                    {
                        configApp.SetBasePath(Directory.GetCurrentDirectory());
                        configApp.AddJsonFile("appsettings.json", optional: false);
                        configApp.AddJsonFile("appsettings.local.json", optional: true);

                        if (args != null)
                        {
                            configApp.AddCommandLine(args);
                        }
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddOptions();

                        services.AddSingleton<ISyncClientFactory, PictureparkSyncClientFactory>();
                        services.AddHostedService<SingleTargetSyncService>();
                    })
                    .ConfigureLogging((hostContext, configLogging) =>
                    {
                        configLogging.AddConsole();
                        configLogging.AddDebug();
                    })
                    .UseConsoleLifetime()
                    .Build();

                await host.RunAsync();
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e}");
            }
        }
    }
}

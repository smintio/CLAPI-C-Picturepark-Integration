using System.IO;
using System.Threading.Tasks;
using Client.Jobs;
using Client.Jobs.Impl;
using Client.Options;
using Client.Providers;
using Client.Providers.Impl;
using Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
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

                    var smintIoSection = hostContext.Configuration.GetSection("SmintIo");
                    var ppSection = hostContext.Configuration.GetSection("Picturepark");

                    services.Configure<SmintIoAppOptions>(smintIoSection.GetSection("App"));
                    services.Configure<PictureparkAppOptions>(ppSection.GetSection("App"));

                    services.Configure<SmintIoAuthOptions>(smintIoSection.GetSection("Auth"));
                    services.Configure<PictureparkAuthOptions>(ppSection.GetSection("Auth"));
                    
                    services.AddSingleton<ISmintIoApiClientProvider, SmintIoApiClientProviderImpl>();
                    services.AddSingleton<ISyncJob, SyncJobImpl>();
                    services.AddSingleton<IPictureparkApiClientProvider, PictureparkApiClientProviderImpl>();
                    services.AddSingleton<ISyncDatabaseProvider, SyncDatabaseProviderImpl>();
                    services.AddSingleton<IAuthDataProvider, AuthDataProviderImpl>();
                    services.AddSingleton<IAuthenticator, AuthenticatorImpl>();

                    services.AddHostedService<TimedSynchronizerService>();
                    services.AddHostedService<PusherService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();

            IAuthDataProvider authData = host.Services.GetService<IAuthDataProvider>();
            IAuthenticator auth = host.Services.GetService<IAuthenticator>();

            authData.SmintIo = await auth.AuthenticateSmintIoAsync();
            authData.Picturepark = auth.AuthenticatePicturepark();

            IPictureparkApiClientProvider pictureparkApiClientProvider = host.Services.GetService<IPictureparkApiClientProvider>();

            await pictureparkApiClientProvider.InitSchemasAsync();

            await host.RunAsync();
        }
    }
}

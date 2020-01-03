using System;
using System.IO;
using System.Threading.Tasks;
using Client.Contracts;
using Client.Options;
using Client.Providers;
using Client.Providers.Impl;
using Client.Target.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator.Impl;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
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

                        services.Configure<PictureparkAppOptions>(ppSection.GetSection("App"));
                        services.Configure<PictureparkAuthOptions>(ppSection.GetSection("Auth"));

                        services.AddSingleton<IPictureparkApiClientProvider, PictureparkApiClientProviderImpl>();

                        services.Configure<SmintIoAppOptions>(smintIoSection.GetSection("App"));
                        services.Configure<SmintIoAuthOptions>(smintIoSection.GetSection("Auth"));
                        
                        services.AddSingleton<ISettingsDatabaseProvider, SettingsDatabaseProviderImpl>();
                        services.AddSingleton<ITokenDatabaseProvider, TokenDatabaseProviderImpl>();
                        services.AddSingleton<ISyncDatabaseProvider, SyncDatabaseProviderImpl>();

                        services.AddSingleton<ISyncTarget<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PictureparkSyncTargetImpl>();
                        services.AddSingleton<ISyncTargetDataFactory<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PicuterparkDataFactory>();

                        services.AddSmintIoClapicIntegrationCore<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>();

                        var descriptor = new ServiceDescriptor(typeof(ISmintIoAuthenticator), typeof(SmintIoSystemBrowserAuthenticatorImpl), ServiceLifetime.Singleton);

                        services.Replace(descriptor);
                    })
                    .ConfigureLogging((hostContext, configLogging) =>
                    {
                        configLogging.AddConsole();
                        configLogging.AddDebug();
                    })
                    .UseConsoleLifetime()
                    .Build();

                var authenticator = host.Services.GetRequiredService<ISmintIoAuthenticator>();

                // we have a system browser based authenticator here, which will work synchronously

                await ((SmintIoSystemBrowserAuthenticatorImpl)authenticator).InitSmintIoAuthenticationAsync();

                IPictureparkApiClientProvider pictureparkApiClientProvider = host.Services.GetService<IPictureparkApiClientProvider>();

                await pictureparkApiClientProvider.InitSchemasAsync();
                
                await host.RunAsync();
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e}");
            }
        }
    }
}

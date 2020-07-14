using Autofac;
using Autofac.Extensions.DependencyInjection;
using Client.Contracts;
using Client.Options;
using Client.Providers;
using Client.Providers.Impl;
using Client.SyncClient.Impl;
using Client.Target.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Impl;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Models;
using SmintIo.CLAPI.Consumer.Integration.Core.Factory;
using SmintIo.CLAPI.Consumer.Integration.Core.SyncClient;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;

namespace Client.Factory
{
    public class PictureparkSyncClientFactory : BaseSyncClientFactoryImpl<PictureparkSyncClientImpl>
    {
        public PictureparkSyncClientFactory(IConfiguration configuration) : base(configuration)
        { }

        public override ISyncClientFactory ConfigureContainerBuilder(ContainerBuilder containerBuilder)
        {
            base.ConfigureContainerBuilder(containerBuilder);

            var services = new ServiceCollection();

            var ppSection = Configuration.GetSection("Picturepark");
            services.AddSingleton(ppSection.GetSection("App").Get<PictureparkAppOptions>());
            services.AddSingleton(ppSection.GetSection("Auth").Get<PictureparkAuthOptions>());

            services.AddSingleton<ISyncTarget<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PictureparkSyncTargetImpl>();
            services.AddSingleton<ISyncTargetDataFactory<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PictureparkDataFactory>();

            services.AddSmintIoClapicIntegrationCore<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>(Configuration);

            services.AddSingleton<IPictureparkApiClientProvider, PictureparkApiClientProviderImpl>();
            services.AddSingleton<ISyncClient, PictureparkSyncClientImpl>();
            services.AddSingleton<ISyncTargetAuthenticationDatabaseProvider<SyncTargetAuthenticationDatabaseModel>, SyncTargetAuthenticationDataMemoryDatabase<SyncTargetAuthenticationDatabaseModel>>();
            services.AddSingleton<ISyncTargetAuthenticator, PictureparkAuthenticatorImpl>();

            containerBuilder.Populate(services);

            return this;
        }
    }
}

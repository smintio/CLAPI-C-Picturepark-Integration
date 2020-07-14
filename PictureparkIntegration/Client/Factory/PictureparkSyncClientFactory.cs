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
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator.Impl;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Impl;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Models;
using SmintIo.CLAPI.Consumer.Integration.Core.Factory;
using SmintIo.CLAPI.Consumer.Integration.Core.SyncClient;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Factory
{
    public class PictureparkSyncClientFactory : BaseSyncClientFactoryImpl<PictureparkSyncClientImpl>
    {
        public PictureparkSyncClientFactory(IConfiguration configuration) : base(configuration)
        { }

        public override ISyncClientFactory ConfigureContainerBuilder(ContainerBuilder containerBuilder)
        {
            base.ConfigureContainerBuilder(containerBuilder);

            var smintIoSection = Configuration.GetSection("SmintIo");
            var services = new ServiceCollection();

            services.Configure<SmintIoAppOptions>(smintIoSection.GetSection("App"));
            services.Configure<SmintIoAuthOptions>(smintIoSection.GetSection("Auth"));

            var ppSection = Configuration.GetSection("Picturepark");
            services.AddSingleton(ppSection.GetSection("App").Get<PictureparkAppOptions>());
            services.AddSingleton(ppSection.GetSection("Auth").Get<PictureparkAuthOptions>());

            // could be part of any base class init
            services.AddSingleton<ISettingsDatabaseProvider, SettingsDatabaseProviderImpl>();
            services.AddSingleton<ISmintIoTokenDatabaseProvider, SmintIoTokenMemoryDatabase>();
            services.AddSingleton<ISyncDatabaseProvider, SyncJsonDatabase>();

            // TODO: Rewrite sync job implementation to avoid depending on this
            // this is needed for SyncJob implementation now. The implementation should acquire the access data with
            // authenticator instead!
            services.AddSingleton<ITokenDatabaseProvider>(serviceProvider =>
                serviceProvider?.GetRequiredService<ISmintIoTokenDatabaseProvider>()
            );

            services.AddSingleton<ISyncTarget<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PictureparkSyncTargetImpl>();
            services.AddSingleton<ISyncTargetDataFactory<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>, PictureparkDataFactory>();

            services.AddSmintIoClapicIntegrationCore<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>();

            var descriptor = new ServiceDescriptor(typeof(ISmintIoAuthenticationRefresher), typeof(SmintIoSystemBrowserAuthenticatorImpl), ServiceLifetime.Singleton);
            services.Replace(descriptor);

            services.AddSingleton<IPictureparkApiClientProvider, PictureparkApiClientProviderImpl>();
            services.AddSingleton<ISyncClient, PictureparkSyncClientImpl>();
            services.AddSingleton<ISyncTargetAuthenticationDatabaseProvider<SyncTargetAuthenticationDatabaseModel>, SyncTargetAuthenticationDataMemoryDatabase<SyncTargetAuthenticationDatabaseModel>>();
            services.AddSingleton<ISyncTargetAuthenticator, PictureparkAuthenticatorImpl>();

            containerBuilder.Populate(services);

            return this;
        }
    }
}

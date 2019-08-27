using Client.Options;
using Microsoft.Extensions.Options;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Models;
using System.Threading.Tasks;

namespace Client.Providers.Impl
{
    // config file based implementation

    public class SettingsDatabaseProviderImpl : ISettingsDatabaseProvider
    {
        private SettingsDatabaseModel _settingsDatabaseModel;

        public SettingsDatabaseProviderImpl(
            IOptionsMonitor<SmintIoAppOptions> appOptionsAccessor,
            IOptionsMonitor<SmintIoAuthOptions> authOptionsAccessor)
        {
            var appOptions = appOptionsAccessor.CurrentValue;
            var authOptions = authOptionsAccessor.CurrentValue;

            _settingsDatabaseModel = new SettingsDatabaseModel()
            {
                TenantId = appOptions.TenantId,
                ChannelId = appOptions.ChannelId,
                ClientId = authOptions.ClientId,
                ClientSecret = authOptions.ClientSecret,
                RedirectUri = authOptions.RedirectUri,
                ImportLanguages = appOptions.ImportLanguages
            };
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task<SettingsDatabaseModel> GetSettingsDatabaseModelAsync()
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            return _settingsDatabaseModel;
        }
    }
}

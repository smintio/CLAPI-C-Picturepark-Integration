using Client.Options;
using SmintIo.CLAPI.Consumer.Integration.Core.Authenticator;
using System.Threading.Tasks;

namespace Client.Target.Impl
{
    public class PictureparkAuthenticatorImpl : ISyncTargetAuthenticator
    {
        private readonly PictureparkAuthOptions _authOptions;

        public PictureparkAuthenticatorImpl(
            PictureparkAuthOptions authOptions
            )
        {
            _authOptions = authOptions;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task InitializeAuthenticationAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task RefreshAuthenticationAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string> GetAccessTokenAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return _authOptions.AccessToken;
        }
    }
}

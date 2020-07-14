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

        public Task InitializeAuthenticationAsync()
        {
            // noop

            return Task.CompletedTask;
        }

        public Task RefreshAuthenticationAsync()
        {
            // noop

            return Task.CompletedTask;
        }

        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(_authOptions.AccessToken);
        }
    }
}

using Client.Auth;

namespace Client.Providers.Impl
{
    public class AuthDataProviderImpl: IAuthDataProvider
    {
        public AuthResult SmintIo { get; set; }

        public AuthResult Picturepark { get; set; }
    }
}

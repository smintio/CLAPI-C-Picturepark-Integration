using Client.Auth;

namespace Client.Providers
{
    public interface IAuthDataProvider
    {
        AuthResult SmintIo { get; set; }

        AuthResult Picturepark { get; set; }
    }
}

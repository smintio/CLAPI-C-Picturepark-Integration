using Client.Auth;
using System.Threading.Tasks;

namespace Client.Providers
{
    public interface IAuthenticator
    {
        Task<AuthResult> AuthenticateSmintIoAsync();
        Task<AuthResult> RefreshSmintIoTokenAsync(string refreshToken);

        AuthResult AuthenticatePicturepark();
    }
}

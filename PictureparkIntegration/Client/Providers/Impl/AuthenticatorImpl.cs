using Client.Auth;
using Client.Options;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Client.Providers.Impl
{
    public class AuthenticatorImpl: IAuthenticator
    {
        private readonly PictureparkAuthOptions _ppAuthOptions;

        private readonly SmintIoAppOptions _smintIoAppOptions;
        private readonly SmintIoAuthOptions _smintIoAuthOptions;

        private readonly ILogger _logger;

        public AuthenticatorImpl(
            IOptionsMonitor<PictureparkAuthOptions> ppAuthOptions,
            IOptionsMonitor<SmintIoAppOptions> smintIoAppOptions,
            IOptionsMonitor<SmintIoAuthOptions> smintIoAuthOptions,
            ILogger<PictureparkApiClientProviderImpl> logger)
        {
            _ppAuthOptions = ppAuthOptions.CurrentValue;

            _smintIoAppOptions = smintIoAppOptions.CurrentValue;
            _smintIoAuthOptions = smintIoAuthOptions.CurrentValue;

            _logger = logger;
        }

        public async Task<AuthResult> AuthenticateSmintIoAsync()
        {
            _logger.LogInformation("Authenticating with Smint.io...");

            try
            {
                var authority = $"https://{_smintIoAppOptions.TenantId}.smint.io/.well-known/openid-configuration";

                var clientOptions = new OidcClientOptions
                {
                    Authority = authority,
                    ClientId = _smintIoAuthOptions.ClientId,
                    ClientSecret = _smintIoAuthOptions.ClientSecret,
                    Scope = "smintio.full openid profile offline_access",
                    RedirectUri = _smintIoAuthOptions.RedirectUri,
                    FilterClaims = false,
                    Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                    ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost
                };

                clientOptions.Policy.Discovery.ValidateIssuerName = false;
                clientOptions.Policy.ValidateTokenIssuerName = false;

                var result = await LoginAsync(_smintIoAuthOptions.RedirectUri, clientOptions);

                return new AuthResult
                {
                    Success = !result.IsError,
                    ErrorMsg = result.Error,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    IdentityToken = result.IdentityToken,
                    Expiration = result.AccessTokenExpiration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating with Smint.io");
                throw;
            }
        }

        public AuthResult AuthenticatePicturepark()
        {
            return new AuthResult
            {
                AccessToken = _ppAuthOptions.AccessToken
            };
        }

        //public async Task<AuthResult> AuthenticatePictureparkAsync()
        //{
        //    _logger.LogInformation("Logging in to Picturepark");
        //    try
        //    {
        //        var clientOptions = new OidcClientOptions
        //        {
        //            Authority = _ppAuthOptions.Authority,
        //            ClientId = _ppAuthOptions.ClientId,
        //            ClientSecret = _ppAuthOptions.ClientSecret,
        //            Scope = _ppAuthOptions.Scope,
        //            RedirectUri = _ppAuthOptions.RedirectUri,
        //            FilterClaims = false,
        //            Flow = OidcClientOptions.AuthenticationFlow.Hybrid,
        //            ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost
        //        };

        //        var result = await LoginAsync(_ppAuthOptions, clientOptions);
        //        return  new AuthResult
        //        {
        //            Success = !result.IsError,
        //            ErrorMsg = result.Error,
        //            AccessToken = result.AccessToken,
        //            RefreshToken = result.RefreshToken,
        //            IdentityToken = result.IdentityToken,
        //            Expiration = result.AccessTokenExpiration
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error authenticating with Picturepark");
        //        throw;
        //    }
        //}

        public async Task<AuthResult> RefreshSmintIoTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Refreshing token for Smint.io");

            var tokenEndpoint = $"https://{_smintIoAppOptions.TenantId}.smint.io/connect/token";

            return await RefreshTokenAsync(
                tokenEndpoint,
                _smintIoAuthOptions.ClientId,
                _smintIoAuthOptions.ClientSecret, 
                refreshToken);
        }

        //public async Task<AuthResult> RefreshPictureparkTokenAsync(string refreshToken)
        //{
        //    _logger.LogInformation("Refreshing token for Picturepark");
        //
        //    return await RefreshTokenAsync(_ppAuthOptions, refreshToken);
        //}

        private async Task<AuthResult> RefreshTokenAsync(string tokenEndpoint, string clientId, string clientSecret, string refreshToken)
        {
            var client = new RestClient(tokenEndpoint);

            var request = new RestRequest(Method.POST);

            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);

            var response = await client.ExecuteTaskAsync<AuthResult>(request).ConfigureAwait(false);

            return response.Data;
        }

        private async Task<LoginResult> LoginAsync(string redirectUri, OidcClientOptions clientOptions)
        {
            IBrowser browser;

            if (int.TryParse(redirectUri.Substring(redirectUri.LastIndexOf(":") + 1), out int port))
            {
                browser = new SystemBrowser(port);
            }
            else
            {
                browser = new SystemBrowser();
            }

            clientOptions.Browser = browser;
            
            var client = new OidcClient(clientOptions);

            return await client.LoginAsync(new LoginRequest());
        }
    }
}

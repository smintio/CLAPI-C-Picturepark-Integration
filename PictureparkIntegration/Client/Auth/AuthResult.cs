using System;

namespace Client.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; } = false;
        public string ErrorMsg { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}

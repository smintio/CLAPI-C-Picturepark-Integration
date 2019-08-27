using System.Threading.Tasks;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Models;

namespace Client.Providers.Impl
{
    // in memory only implementation

    public class TokenDatabaseProviderImpl : ITokenDatabaseProvider
    {
        private TokenDatabaseModel _tokenDatabaseModel;

        public TokenDatabaseProviderImpl()
        {
            _tokenDatabaseModel = new TokenDatabaseModel();
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task<TokenDatabaseModel> GetTokenDatabaseModelAsync()
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            return _tokenDatabaseModel;
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task SetTokenDatabaseModelAsync(TokenDatabaseModel tokenDatabaseModel)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            _tokenDatabaseModel = tokenDatabaseModel;
        }
    }
}

using Domain.User;

namespace Application.Interfaces.Repository
{
    public interface ITokenRepository : IRepository<TokenInfo>
    {

        public Task<IEnumerable<TokenInfo>> GetTokens(Guid userId);
        public Task<IEnumerable<TokenInfo>> GetTokens(Guid userId, string deviceName);
    }
}

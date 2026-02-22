using Domain.User;
using Domain.ValueObjects;

namespace Application.Interfaces.Repository
{
    public interface ITokenRepository : IRepository<TokenInfo>
    {

        public Task<IEnumerable<TokenInfo>> GetTokens(Guid userId);

        public Task<IEnumerable<TokenInfo>> GetTokens(Guid userId, string deviceName);

        public Task<TokenInfo> GetByIdWithUserAsync(Guid id);

        public Task<TokenInfo> GetByRefreshToken(Token refresh);

        public Task RevokeAsync(Guid tokenId);

        public Task RevokeAllAsync(Guid userId);

        public Task RevokeAllAsync(Guid userId, string deviceName);
    }
}

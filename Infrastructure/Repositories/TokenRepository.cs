using Application.Interfaces.Repository;
using AutoMapper;
using Domain.User;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TokenRepository(GuessNumberContext c, IMapper m) : BaseRepository<TokenInfo, TokenEntity>(c, m), ITokenRepository
    {
        public async Task<IEnumerable<TokenInfo>> GetTokens(Guid userId)
        {
            var tokens = await _dbSet.Where(t => t.UserId == userId && !t.IsRevoked).ToHashSetAsync();

            return _mapper.Map<IEnumerable<TokenInfo>>(tokens);
        }

        public async Task<IEnumerable<TokenInfo>> GetTokens(Guid userId, string deviceName)
        {
            var tokens = await _dbSet.Where(t => t.UserId == userId && t.DeviceName.ToLower() == deviceName.ToLower() && !t.IsRevoked).ToHashSetAsync();

            return _mapper.Map<IEnumerable<TokenInfo>>(tokens);
        }
    }
}

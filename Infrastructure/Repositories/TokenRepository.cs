using Application.Interfaces.Repository;
using AutoMapper;
using Domain.User;
using Domain.ValueObjects;
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

        public async Task<TokenInfo> GetByIdWithUserAsync(Guid id)
        {
            var token = await _dbSet.Include(t => t.User).Where(t => t.Id == id).SingleOrDefaultAsync();

            return _mapper.Map<TokenInfo>(token);
        }

        public async Task<TokenInfo> GetByRefreshToken(Token refresh)
        {
            var plainToken = refresh.Value;
            var token = await _dbSet.AsNoTracking().Include(t => t.User).Where(t => t.RefreshToken == plainToken).SingleOrDefaultAsync();

            return _mapper.Map<TokenInfo>(token);
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            await _dbSet
                .Where(t => t.Id == tokenId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true));
        }

        public async Task RevokeAllAsync(Guid userId)
        {
            await _dbSet
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true));
        }

        public async Task RevokeAllAsync(Guid userId, string deviceName)
        {
            await _dbSet
                .Where(t => t.UserId == userId && t.DeviceName.ToLower() == deviceName.ToLower() && !t.IsRevoked)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsRevoked, true));
        }
    }
}

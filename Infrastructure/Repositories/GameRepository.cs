using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GameRepository(GuessNumberContext c, IMapper m) : BaseRepository<Game, GameEntity>(c, m), IGameRepository
    {
        public async Task<Game> FindByCode(string code)
        {
            var game = await _dbSet.Where(g => g.Code.ToLower() == code.ToLower()).SingleOrDefaultAsync();

            return _mapper.Map<Game>(game);
        }
    }
}

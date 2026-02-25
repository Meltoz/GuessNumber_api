using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Party;
using Infrastructure.Entities;

namespace Infrastructure.Repositories
{
    public class GameRepository(GuessNumberContext c, IMapper m) : BaseRepository<Game, GameEntity>(c, m), IGameRepository
    {
    }
}

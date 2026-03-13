using Domain.Party;

namespace Application.Interfaces.Repository
{
    public interface IGameRepository : IRepository<Game>
    {   
        public Task<Game> FindByCode(string code);
    }
}

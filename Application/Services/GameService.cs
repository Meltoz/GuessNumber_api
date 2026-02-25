using Application.Interfaces.Repository;
using Application.Interfaces.Web;
using Domain.Party;

namespace Application.Services
{
    public class GameService (IGameHubNotifier hn, IGameRepository gr)
    {
        private readonly IGameHubNotifier _hubNotifier = hn;
        private readonly IGameRepository _gameRepository = gr;

        public async Task<Game> CreateGame()
        {
            var game = Game.CreateNew();

            return await _gameRepository.InsertAsync(game);
        }
    }
}

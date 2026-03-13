using Application.Interfaces.Repository;
using Application.Interfaces.Web;
using Domain.Party;

namespace Application.Services;

public class GameService
{
    private readonly IGameHubNotifier _hubNotifier;
    private readonly IGameRepository _gameRepository;

    public GameService(IGameHubNotifier hn, IGameRepository gr)
    {
        _hubNotifier = hn;
        _gameRepository = gr;
    }

    public async Task<Game> CreateGame()
    {
        var game = Game.CreateNew();

        return await _gameRepository.InsertAsync(game);
    }

    public async Task<bool> GameIsJoinable(string code)
    {
        var game = await _gameRepository.FindByCode(code);

        return game.IsJoinable();
    }
}
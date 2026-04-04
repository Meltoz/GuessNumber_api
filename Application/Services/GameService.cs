using Application.Interfaces.Repository;
using Application.Interfaces.Web;
using Domain.Enums;
using Domain.Party;
using Domain.User;

namespace Application.Services;

public class GameService
{
    private readonly IGameHubNotifier _hubNotifier;
    private readonly IGameRepository _gameRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GameService(IGameHubNotifier hn, IGameRepository gr, ICategoryRepository cr)
    {
        _hubNotifier = hn;
        _gameRepository = gr;
        _categoryRepository = cr;
    }

    public async Task<Game> CreateGame()
    {
        var game = Game.CreateNew();

        var categories = await _categoryRepository.GetAllAsync();
        var categoryIds = categories.Select(c => c.Id).ToList();
        if (categoryIds.Count > 0)
            game.SetCategories(categoryIds);

        return await _gameRepository.InsertAsync(game);
    }

    public async Task<bool> GameIsJoinable(string code)
    {
        var game = await _gameRepository.FindByCode(code);

        return game.IsJoinable();
    }
    
    

    public async Task<Game> JoinGame(string code, User user, RoleParty role, string connectionId)
    {
        var game = await _gameRepository.FindByCode(code);

        game.AddPlayer(user.Id, user.Pseudo.ToString(), user.Avatar, connectionId, role);

        return await _gameRepository.UpdateAsync(game);
    }

    public async Task<Game?> LeaveGame(string connectionId)
    {
        var game = await _gameRepository.FindByConnectionId(connectionId);
        if (game is null)
            return null;

        game.RemovePlayer(connectionId);
        if (game.Players.Count == 0)
        {
            game.CancelGame();
        }

        return await _gameRepository.UpdateAsync(game);
    }
}
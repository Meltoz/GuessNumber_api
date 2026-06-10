using Application.Interfaces.Context;
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
    private readonly ICurrentGameContext _currentGameContext;

    public GameService(IGameHubNotifier hn, IGameRepository gr, ICategoryRepository cr, ICurrentGameContext cgc)
    {
        _hubNotifier = hn;
        _gameRepository = gr;
        _categoryRepository = cr;
        _currentGameContext = cgc;
    }

    public async Task<Game> CreateGame()
    {
        var game = Game.CreateNew();

        var allCategories = await _categoryRepository.GetAllAsync();
        var categoryIds = allCategories.Select(c => c.Id).ToList();
        if (categoryIds.Count > 0)
            game.UpdateSettings(s => s.SetCategories(categoryIds));

        await _gameRepository.InsertAsync(game);

        return await EnrichWithAllCategories(game, allCategories);
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

        await _gameRepository.UpdateAsync(game);

        return await EnrichWithAllCategories(game);
    }

    public async Task<Game?> LeaveGame(string connectionId)
    {
        var game = await _gameRepository.FindByConnectionId(connectionId);
        if (game is null)
            return null;

        game.RemovePlayer(connectionId);
        if (game.Players.Count == 0)
            game.CancelGame();

        await _gameRepository.UpdateAsync(game);

        return await EnrichWithAllCategories(game);
    }

    public async Task<Game> StartGame(string code)
    {
        return null;
    }

    public async Task VerifyAndCacheGame(string code, Guid userId)
    {
        var game = await _gameRepository.FindByCode(code);
        if (!game.IsOwner(userId))
            throw new UnauthorizedAccessException($"User '{userId}' is not owner of game '{code}'");
        _currentGameContext.SetGame(game);
    }

    public async Task<Game> UpdateGameSettings(string code, Guid userId, int maxPlayers, int totalQuestion, IReadOnlyCollection<Guid> categoryIds)
    {
        var game = await GetGameFromContextOrDb(code);
        if(!game.IsOwner(userId))
            throw new InvalidOperationException($"User '{userId}' is not owner of this game");

        game.UpdateSettings(s =>
        {
            s.ChangeMaxPlayers(maxPlayers);
            s.ChangeTotalQuestion(totalQuestion);
            s.SetCategories(categoryIds);
        });

        await _gameRepository.UpdateAsync(game);

        return await EnrichWithAllCategories(game);
    }

    private async Task<Game> GetGameFromContextOrDb(string code)
        => _currentGameContext.Game ?? await _gameRepository.FindByCode(code);

    private async Task<Game> EnrichWithAllCategories(Game game, IEnumerable<Category>? allCategories = null)
    {
        allCategories ??= await _categoryRepository.GetAllAsync();
        game.InitializeResolvedCategories(allCategories);
        return game;
    }
}
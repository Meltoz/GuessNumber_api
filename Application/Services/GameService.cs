using Application.Interfaces.Context;
using Application.Interfaces.Repository;
using Application.Interfaces.Web;
using Application.Results;
using Domain.Enums;
using Domain.Party;
using Domain.User;

namespace Application.Services;

public class GameService 
{
    private readonly IGameHubNotifier _hubNotifier;
    private readonly IGameRepository _gameRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly ICurrentGameContext _currentGameContext;

    public GameService(IGameHubNotifier hn, IGameRepository gr, ICategoryRepository cr, IQuestionRepository qr,  ICurrentGameContext cgc)
    {
        _hubNotifier = hn;
        _gameRepository = gr;
        _categoryRepository = cr;
        _questionRepository = qr;
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

    /// <summary>
    /// Commence la partie
    /// </summary>
    /// <param name="code">Code de la partie</param>
    /// <param name="userId">Utilisateur initiateur</param>
    /// <returns></returns>
    public async Task<Game> StartGame(string code, Guid userId)
    {
        var game = await VerifyAndCacheGame(code, userId);
        var categoriesIds = game.Categories.Select(c => c.Id).ToHashSet();

        var allQuestionIds = await _questionRepository.GetAllQuestionIds(categoriesIds);

        var selectedQuestionIds = SelectQuestion(allQuestionIds, game.Settings.TotalQuestion);
        var questionsSelected  = await _questionRepository.GetQuestionsByIds(selectedQuestionIds);
        game.Start(questionsSelected);
        await _gameRepository.UpdateAsync(game);
        
        return game;
    }

    /// <summary>
    /// Récuperation de la partie via BDD + vérification action owner
    /// </summary>
    /// <param name="code">Code de la partie</param>
    /// <param name="userId">ID du user qui effectue la requete</param>
    /// <returns>Game</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<Game> VerifyAndCacheGame(string code, Guid userId)
    {
        var game = await GetGameFromContextOrDb(code);
        if (!game.IsOwner(userId))
            throw new UnauthorizedAccessException($"User '{userId}' is not owner of game '{code}'");
        
        _currentGameContext.SetGame(game);
        return game;
    }

    public async Task<NextQuestionResult> GetNextQuestion(string code, Guid userIdCaller)
    {
        var game = await VerifyAndCacheGame(code, userIdCaller);
        if (game.IsLastQuestion)
        {
            // TODO : Update end Game
            return NextQuestionResult.GameOver();
        }
        game.NextQuestion();
        await _gameRepository.UpdateAsync(game);
        var question = game.Questions.ElementAt(game.CurrentQuestion -1);
        return NextQuestionResult.WithQuestion(question);
    }
    
    /// <summary>
    /// Mettre à jour les informations de la partie
    /// </summary>
    /// <param name="code">Code de la partie</param>
    /// <param name="userId">ID du user qui fait la demande de mise a jour</param>
    /// <param name="maxPlayers">Nombre maximum de joueurs de la partie</param>
    /// <param name="totalQuestion">Nombre totale de question</param>
    /// <param name="categoryIds">Liste des categories de la partie</param>
    /// <returns></returns>
    public async Task<Game> UpdateGameSettings(string code, Guid userId, int maxPlayers, int totalQuestion, IReadOnlyCollection<Guid> categoryIds)
    {
        var game = await VerifyAndCacheGame(code, userId);

        game.UpdateSettings(s =>
        {
            s.ChangeMaxPlayers(maxPlayers);
            s.ChangeTotalQuestion(totalQuestion);
            s.SetCategories(categoryIds);
        });

        await _gameRepository.UpdateAsync(game);

        return await EnrichWithAllCategories(game);
    }

    #region Private Methods
    /// <summary>
    /// Permet de récupérer la game via le cache backend
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private async Task<Game> GetGameFromContextOrDb(string code)
        => _currentGameContext.Game ?? await _gameRepository.FindByCode(code);

    /// <summary>
    /// Permet de renvoyer toutes les categories vers le client.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="allCategories"></param>
    /// <returns></returns>
    private async Task<Game> EnrichWithAllCategories(Game game, IEnumerable<Category>? allCategories = null)
    {
        allCategories ??= await _categoryRepository.GetAllAsync();
        game.InitializeResolvedCategories(allCategories);
        return game;
    }

    /// <summary>
    /// Permet de selectionner le nombre de question voulu dans un pool donné
    /// </summary>
    /// <param name="allQuestionIds"></param>
    /// <param name="maxQuestions"></param>
    /// <returns></returns>
    private IEnumerable<Guid> SelectQuestion(Dictionary<Guid, HashSet<Guid>> allQuestionIds, int maxQuestions)
    {
        var categoryNumber = allQuestionIds.Count;
        var rng = new Random();
        var questionsSelected = new List<Guid>(maxQuestions);
        
        
        var categories = allQuestionIds
            .Where(kv => kv.Value.Count > 0)
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToList());
        
        var totalAvailable = categories.Sum(c => c.Value.Count);
        var countToSelect = Math.Min(maxQuestions, totalAvailable);
        var result = new List<Guid>(countToSelect);
        var categoryKeys = categories.Keys.ToList();
        
        for(var i = 0; i < countToSelect; i++)
        {
            var indexCategory = Random.Shared.Next(categoryKeys.Count);
            var categoryKey = categoryKeys[indexCategory];
            var questions = categories[categoryKey];

            var questionIndex = Random.Shared.Next(questions.Count);
            var selected = questions[questionIndex];
            result.Add(selected);
            
            //SWAP REMOVE
            questions[questionIndex] = questions[^1];
            questions.RemoveAt(questions.Count - 1);
            
            // Si plus de questions dans la categorie
            if (questions.Count == 0)
            {
                categoryKeys.RemoveAt(indexCategory);
            }
        }

        return result;
    }
    
    #endregion
}
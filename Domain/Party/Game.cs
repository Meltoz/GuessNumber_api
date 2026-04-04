using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Party;

public class Game
{
    public Guid Id { get; private set; }

    public Code Code { get; private set; }

    public GameStatus Status { get; private set; }

    public GameType Type { get; private set; }

    public int TotalQuestion { get; private set; }

    public int CurrentQuestion { get; private set; }

    public int MaxPlayers { get; private set; }

    private readonly List<Player> _players = [];

    public IReadOnlyCollection<Player> Players => _players;

    private readonly List<Guid> _categoryIds = [];
    public IReadOnlyCollection<Guid> CategoryIds => _categoryIds;

    private readonly List<Category> _categories = [];
    public IReadOnlyCollection<Category> Categories => _categories;

    private  Game() { }
    
    public Game(string code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = Code.Create(code);
        Status = status;
        Type = type;
        ChangeTotalQuestion(maxQuestion);
        CurrentQuestion = 0;
        ChangeMaxPlayers(maxPlayers);
    }

    public Game(Code code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = code;
        Status = status;
        Type = type;
        ChangeTotalQuestion(maxQuestion);
        CurrentQuestion = 0;
        ChangeMaxPlayers(maxPlayers);
    }

    public Game(Guid id, string code, GameStatus status, GameType type, int maxQuestion, int maxPlayers) : this(code, status, type, maxQuestion, maxPlayers)
    {
        if(id != Guid.Empty) Id = id;
    }


    public static Game CreateNew()
    {
        var code = Code.Generate();
        return new Game(code, GameStatus.Creating, GameType.Private, 20, 8);
    }

    public void NextQuestion()
    {
        if (CurrentQuestion + 1 > TotalQuestion)
            throw new InvalidOperationException();

        CurrentQuestion++;

    }

    public void ChangeTotalQuestion(int totalQuestion)
    {
        if(totalQuestion < 0 || totalQuestion > 50)
            throw new ArgumentOutOfRangeException(nameof(totalQuestion));

        TotalQuestion = totalQuestion;
    }

    public void ChangeMaxPlayers(int maxPlayers)
    {
        if(maxPlayers > 10)
            throw new ArgumentOutOfRangeException(nameof(maxPlayers));

        MaxPlayers = maxPlayers;
    }

    public bool IsJoinable()
    {
        var statusUnjoinable = new HashSet<GameStatus>
        {
            GameStatus.Cancelled,
            GameStatus.Finished,
        };
        return _players.Count < MaxPlayers && !statusUnjoinable.Contains(Status);
    }

    public void AddPlayer(Guid userId, string pseudo, string avatar, string connectionId, RoleParty role = RoleParty.Player)
    {
        if (_players.Count >= MaxPlayers)
            throw new InvalidOperationException("Maximum number of players exceeded");

        _players.Add(new Player(userId, pseudo, avatar, role, connectionId));
    }

    public void RemovePlayer(string connectionId)
    {
        var player = _players.FirstOrDefault(p => p.ConnectionId == connectionId);
        if (player is null)
            throw new InvalidOperationException("Player not found");
        _players.Remove(player);
    }

    public void InitializePlayers(IEnumerable<Player> players)
    {
        _players.Clear();
        _players.AddRange(players);
    }

    public void InitializeCategories(IEnumerable<Guid> categoryIds)
    {
        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds);
    }

    public void InitializeResolvedCategories(IEnumerable<Category> categories)
    {
        _categories.Clear();
        _categories.AddRange(categories);
    }

    public void CancelGame()
    {
        Status = GameStatus.Cancelled;
        CurrentQuestion = TotalQuestion;
    }

    public void SetCategories(IReadOnlyCollection<Guid> categoryIds)
    {
        ArgumentNullException.ThrowIfNull(categoryIds);

        ArgumentEmptyException.ThrowIfEmpty(categoryIds, nameof(categoryIds));

        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds);
    }
}


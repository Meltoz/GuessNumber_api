using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Party;

public class Game
{
    public Guid Id { get; private set; }

    public Code Code { get; private set; }

    public GameStatus Status { get; private set; }
    
    public GamePhase Phase { get; private set; }

    public GameType Type { get; private set; }

    public int CurrentQuestion { get; private set; }
    
    public GameSettings Settings { get; private set; }
    
    private readonly List<Player> _players = [];

    public IReadOnlyCollection<Player> Players => _players;
    
    private readonly List<Category> _categories = [];
    public IReadOnlyCollection<Category> Categories => _categories;

    private readonly List<Question> _questions = [];

    public  IReadOnlyCollection<Question> Questions => _questions;

    private  Game() { }
    
    public Game(string code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = Code.Create(code);
        Status = status;
        Type = type;
        CurrentQuestion = 0;
        Settings = new GameSettings(maxPlayers, maxQuestion);
    }

    public Game(Code code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = code;
        Status = status;
        Type = type;
        CurrentQuestion = 0;
        Settings = new GameSettings(maxPlayers, maxQuestion);
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
        if (CurrentQuestion + 1 > Settings.TotalQuestion)
            throw new InvalidOperationException();
        
        CurrentQuestion++;
    }

    public void SetReviewPhase()
    {
        if (Status != GameStatus.Playing)
            throw new InvalidOperationException("Cannot pause game when not playing");

        Phase = GamePhase.Reviewing;
    }

    public void SetAnswerPhase()
    {
        if (Status != GameStatus.Playing)
            throw new InvalidOperationException("Cannot set answer phase when not playing");
        
        Phase = GamePhase.Answering;
    }

    public void SetWaitingNextRoundPhase()
    {
        if (Status != GameStatus.Playing)
            throw new InvalidOperationException("Cannot set waiting round phase when not playing");

        Phase = GamePhase.TransitionToNext;
    }

    public bool IsJoinable()
    {
        var statusUnjoinable = new HashSet<GameStatus>
        {
            GameStatus.Cancelled,
            GameStatus.Finished,
        };
        return _players.Count < Settings.MaxPlayers && !statusUnjoinable.Contains(Status);
    }

    public void AddPlayer(Guid userId, string pseudo, string avatar, string connectionId, RoleParty role = RoleParty.Player)
    {
        if (_players.Count >= Settings.MaxPlayers)
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

    public void CancelGame()
    {
        Status = GameStatus.Cancelled;
        CurrentQuestion = Settings.TotalQuestion;
    }

    public bool IsOwner(Guid userId) => 
        _players.Any(p => p.UserId == userId && p.Role == RoleParty.Owner);

    public void UpdateSettings(Action<GameSettings> configure)
    {
        if(Status != GameStatus.Creating)
            throw new InvalidOperationException("Cannot edit settings after game has started.");
        
        configure(Settings);
    }

    public void Start(IEnumerable<Question> questions)
    {
        if(Status != GameStatus.Creating)
            throw new InvalidOperationException("Cannot start game after game has started.");
        
        if(questions.Count() != Settings.TotalQuestion)
            throw new InvalidOperationException("The number of questions must match the number of questions");

        Status = GameStatus.Playing;
        InitializeQuestions(questions);

    }
    
    internal void InitializePlayers(IEnumerable<Player> players)
    {
        _players.Clear();
        _players.AddRange(players);
    }

    internal void InitializeResolvedCategories(IEnumerable<Category> categories)
    {
        _categories.Clear();
        _categories.AddRange(categories);
    }

    internal void InitializeQuestions(IEnumerable<Question> questions)
    {
        _questions.Clear();
        _questions.AddRange(questions);
    }
}


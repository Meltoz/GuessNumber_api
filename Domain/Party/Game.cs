using Domain.Enums;
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

    private  Game() { }
    
    public Game(string code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = Code.Create(code);
        ChangeTotalQuestion(maxQuestion);
        CurrentQuestion = 0;
        ChangeMaxPlayers(maxPlayers);
    }

    public Game(Code code, GameStatus status, GameType type, int maxQuestion, int maxPlayers)
    {
        Code = code;
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
        if(TotalQuestion < 0 && TotalQuestion > 50)
            throw new ArgumentOutOfRangeException(nameof(TotalQuestion));

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
        // TODO : avec les joueurs implementer il faudra vérifier qu'on dépasse pas le nombre max de joueur
        return true;
    }
}


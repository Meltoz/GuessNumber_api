namespace Domain.Party;

public class Answer
{
    public Guid Id { get; private set; }
    
    public Player Player { get; private set; }
    
    public Game Game { get; private set; }
    
    public Question Question { get; private set; }
    
    public int IndexQuestion { get; private set; }
    
    public string? ResponseUser { get; private set; }

    internal Answer()
    {
        
    }

    public Answer(Game game, Player player, Question question, int questionIndex, string? responseUser)
    {
        Game = game;
        Player = player;
        Question = question;
        IndexQuestion = questionIndex;
        ResponseUser = responseUser;
    }
    
    public Answer(Guid id, Game game, Player player, Question question, int questionIndex, string? responseUser) : this(game, player, question, questionIndex, responseUser)
    {
        if (id == Guid.Empty)
            throw new ArgumentNullException(nameof(id));
        
        Id = id;
    }
}
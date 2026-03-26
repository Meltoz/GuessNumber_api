using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Party;

public class Player
{
    public Guid Id { get; private set; }
    public int Score { get; private set; }
    
    public RoleParty Role { get; private set; }
    
    public string ConnectionId { get; private set; }
    
    public User.User User { get; private set; }
    
    private Player() { }

    public Player(User.User user, RoleParty role, string connectionId)
    {
        Role = role;
        User = user;
        Score = 0;
        
        if(string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentNullException("ConnectionId can't be null or empty");
        ConnectionId = connectionId;
    }

    public Player(Guid id, User.User user, RoleParty role, string connectionId)
    {
        Id = id;
        Role = role;
        User = user;
        Score = 0;
        
        if(string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentNullException("ConnectionId can't be null or empty");
        ConnectionId = connectionId;
    }
    public void ChangeScore(int newScore)
    {
        if (newScore < Score)
            throw new InvalidScoreUpdateException($"invalid score update trying : {newScore} from {Score}");
        
        Score = newScore;
    }
}
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Party;

public class Player
{
    public Guid Id { get; private set; }
    public int Score { get; private set; }
    public RoleParty Role { get; private set; }
    public string ConnectionId { get; private set; }
    public Guid UserId { get; private set; }
    public string Pseudo { get; private set; }
    public string Avatar { get; private set; }

    private Player() { }

    public Player(Guid userId, string pseudo, string avatar, RoleParty role, string connectionId)
    {
        UserId = userId;
        Pseudo = pseudo;
        Avatar = avatar;
        Role = role;
        Score = 0;

        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentNullException("ConnectionId can't be null or empty");
        ConnectionId = connectionId;
    }

    public Player(Guid id, Guid userId, string pseudo, string avatar, RoleParty role, string connectionId)
        : this(userId, pseudo, avatar, role, connectionId)
    {
        Id = id;
    }
    public void ChangeScore(int newScore)
    {
        if (newScore < Score)
            throw new InvalidScoreUpdateException($"invalid score update trying : {newScore} from {Score}");
        
        Score = newScore;
    }
}
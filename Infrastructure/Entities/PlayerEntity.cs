using Domain.Enums;

namespace Infrastructure.Entities;

public class PlayerEntity : BaseEntity
{
    public int Score { get; set; }
    
    public RoleParty Role { get; set; }
    
    public string ConnectionId { get; set; }
    
    public Guid GameId { get; set; }
    public GameEntity Game { get; set; }
    
    public Guid UserId { get; set; }
    
    public UserEntity User { get; set; }
    
}
namespace Infrastructure.Entities;

public class AnswerEntity : BaseEntity
{
    public Guid PlayerId { get; set; }
    
    public PlayerEntity Player { get; set; }
    
    public Guid GameId { get; set; }
    
    public GameEntity Game { get; set; }
    
    public Guid QuestionId { get; set; }
    
    public QuestionEntity Question { get; set; }
    
    public string? UserResponse { get; set; }
    
    public int IndexQuestion { get; set; }
}
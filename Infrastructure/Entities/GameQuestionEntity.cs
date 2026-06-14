namespace Infrastructure.Entities;

public class GameQuestionEntity
{
    public Guid GameId { get; set; }
    
    public GameEntity Game { get; set; }
    
    public Guid QuestionId { get; set; }
    
    public QuestionEntity Question { get; set; }
    
    public int Order { get; set; }
}
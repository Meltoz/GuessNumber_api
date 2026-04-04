namespace Infrastructure.Entities;

public class GameCategoriesEntity
{
    public Guid GameId { get; set; }
    
    public GameEntity Game { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public CategoryEntity Category { get; set; }
    
}
namespace Web.ViewModels;

public class GameConfigurationVM
{
    public int TotalQuestion { get; set; }

    public int MaxPlayers { get; set; }
    
    public bool HasReview { get; set; }

    public IReadOnlyCollection<CategoryVM> Categories { get; set; }
}

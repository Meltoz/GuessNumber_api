namespace Web.ViewModels;

public class GameVM
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public int Status { get; set; }

    public int CurrentQuestion { get; set; }

    public GameConfigurationVM Configuration { get; set; }

    public IReadOnlyCollection<PlayerVM> Players { get; set; }
}


namespace Web.ViewModels;

public class QuestionVM
{
    public Guid Id { get; set; }

    public string Libelle { get; set; } = string.Empty;
    
    public int Duration { get; set; }

    public string Author { get; set; } = string.Empty;
}
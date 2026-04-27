using Domain.Exceptions;

namespace Domain.Party;

public class GameSettings
{
    public int MaxPlayers { get; private set; }
    
    public int TotalQuestion { get; private set; }
    
    private readonly List<Guid> _categoryIds = [];
    public IReadOnlyCollection<Guid> CategoryIds => _categoryIds;
    
    
    public void ChangeTotalQuestion(int totalQuestion)
    {
        if(totalQuestion < 0 || totalQuestion > 50)
            throw new ArgumentOutOfRangeException(nameof(totalQuestion));

        TotalQuestion = totalQuestion;
    }
    
    public void ChangeMaxPlayers(int maxPlayers)
    {
        if(maxPlayers > 10)
            throw new ArgumentOutOfRangeException(nameof(maxPlayers));

        MaxPlayers = maxPlayers;
    }
    
    public void SetCategories(IReadOnlyCollection<Guid> categoryIds)
    {
        ArgumentNullException.ThrowIfNull(categoryIds);

        ArgumentEmptyException.ThrowIfEmpty(categoryIds, nameof(categoryIds));

        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds);
    }
    
    internal void InitializeCategories(IEnumerable<Guid> categoryIds)
    {
        _categoryIds.Clear();
        _categoryIds.AddRange(categoryIds);
    }


    private GameSettings() { }

    internal GameSettings(int maxPlayers, int totalQuestion)
    {
        ChangeMaxPlayers(maxPlayers);
        ChangeTotalQuestion(totalQuestion);
    }
        
}
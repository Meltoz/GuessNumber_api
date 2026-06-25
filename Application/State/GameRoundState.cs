using System.Collections.Concurrent;

namespace Application.State;

public class GameRoundState
{
    public string GameCode { get; }
    public Guid QuestionId { get; }
    public ConcurrentDictionary<Guid, string?> SubmittedAnswers { get; }
    public IReadOnlySet<Guid> ExceptedPlayersIds { get; }
    public CancellationTokenSource Cts { get; }
    private int _triggered = 0;

    public GameRoundState(string code, Guid questionId, IEnumerable<Guid> playersIds)
    {
        GameCode = code;
        QuestionId = questionId;
        ExceptedPlayersIds = playersIds.ToHashSet();
        SubmittedAnswers = new ConcurrentDictionary<Guid, string?>();
        Cts = new CancellationTokenSource();
        
    }
    
    public bool AllAnswered => 
    ExceptedPlayersIds.Count > 0 && 
    ExceptedPlayersIds.All(id => SubmittedAnswers.ContainsKey(id));
    
    public bool TryClaimAdvance() 
        => Interlocked.CompareExchange(ref _triggered, 1, 0) == 0;

}
using System.Collections.Concurrent;
using Application.Interfaces.Web;
using Application.State;
using Domain.Party;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class GameTimerService
{
    private readonly ConcurrentDictionary<string, GameRoundState> _games;
    private readonly IServiceScopeFactory _scopeFactory;

    public GameTimerService(IServiceScopeFactory scopeFactory)
    {
        _games = new ConcurrentDictionary<string, GameRoundState>();
        _scopeFactory = scopeFactory;
    }
    
    public void StartRound(Game game)
    {
        var question = game.Questions.ElementAt(game.CurrentQuestion - 1);
        var roundState = new GameRoundState(game.Code, question.Id, game.Players.Select(x => x.Id));
        _games[game.Code] = roundState;
        
        var delay = game.PhaseEndedAt!.Value - DateTime.UtcNow + TimeSpan.FromMilliseconds(500);
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, roundState.Cts.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await AdvanceRoundAsync(roundState);
        });
    }

    public async Task RecordAnswer(string gameCode, Guid playerId, string? answer)
    {
        if (!_games.TryGetValue(gameCode, out var roundState)) return;
        
        roundState.SubmittedAnswers[playerId] = answer;

        if (roundState.AllAnswered)
        {
            roundState.Cts.Cancel();
            _ = Task.Run(() => AdvanceRoundAsync(roundState));
        }
    }

    private async Task AdvanceRoundAsync(GameRoundState state)
    {
        if (!state.TryClaimAdvance()) return;
        _games.TryRemove(state.GameCode, out _);
        
        using var scope = _scopeFactory.CreateScope();
        var gameService = scope.ServiceProvider.GetRequiredService<GameService>();

        await gameService.AdvancePhaseAsync(state.GameCode);

    }
}
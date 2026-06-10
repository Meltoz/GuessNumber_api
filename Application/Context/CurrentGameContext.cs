using Application.Interfaces.Context;
using Domain.Party;

namespace Application.Context;

public class CurrentGameContext : ICurrentGameContext
{
    public Game? Game { get; private set; }
    public void SetGame(Game game) => Game = game;
}

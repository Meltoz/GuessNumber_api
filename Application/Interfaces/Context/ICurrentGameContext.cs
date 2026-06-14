using Domain.Party;

namespace Application.Interfaces.Context;

public interface ICurrentGameContext
{
    Game? Game { get; }
    void SetGame(Game game);
}

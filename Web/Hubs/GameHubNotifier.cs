using Application.Interfaces.Web;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class GameHubNotifier(IHubContext<GameHub, IGameHubClient> hc) : IGameHubNotifier
    {
        private readonly IHubContext<GameHub, IGameHubClient> _hubContext = hc;
        public async Task NotifyGameEnd(Guid gameId)
        {
            await _hubContext.Clients
                .Group(gameId.ToString())
                .CancelGame();
        }
    }
}

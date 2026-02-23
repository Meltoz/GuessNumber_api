using Application.Interfaces.Web;
using Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class GameHub(GameService gm) : Hub<IGameHubClient>
    {
        private readonly GameService _gameService = gm;

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}

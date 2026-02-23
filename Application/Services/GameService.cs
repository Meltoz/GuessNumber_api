using Application.Interfaces.Web;

namespace Application.Services
{
    public class GameService (IGameHubNotifier hn)
    {
        private readonly IGameHubNotifier _hubNotifier = hn;
    }
}

using Web.ViewModels;

namespace Web.Hubs.Interfaces
{
    public interface IGameHubClient
    {
        public Task CancelGame();

        public Task UpdateParty(GameVM game);
    }
}

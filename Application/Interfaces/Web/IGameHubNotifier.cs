namespace Application.Interfaces.Web
{
    public interface IGameHubNotifier
    {

        public Task NotifyGameEnd(Guid gameId);
    }
}

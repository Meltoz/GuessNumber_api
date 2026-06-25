using Domain.Party;

namespace Application.Interfaces.Web
{
    public interface IGameHubNotifier
    {

        public Task NotifyGameEnd(string code);

        public Task NotifyGameUpdate(Game game);
        
        public Task NotifyNextQuestion(string gameCode, Question question);
    }
}

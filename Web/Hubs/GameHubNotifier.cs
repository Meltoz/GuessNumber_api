using Application.Interfaces.Web;
using AutoMapper;
using Domain.Party;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs.Interfaces;
using Web.ViewModels;

namespace Web.Hubs
{
    public class GameHubNotifier: IGameHubNotifier
    {
        private readonly IHubContext<GameHub, IGameHubClient> _hubContext;
        private readonly IMapper _mapper;

        public GameHubNotifier(IHubContext<GameHub, IGameHubClient> hc, IMapper m)
        {
            _hubContext = hc;
            _mapper = m;
        }
        
        public async Task NotifyGameEnd(string code)
        {
            await _hubContext.Clients
                .Group(code)
                .CancelGame();
        }

        public async Task NotifyNextQuestion(string code, Question question)
        {
            await _hubContext.Clients
                .Group(code)
                .ReceiveQuestion(_mapper.Map<QuestionVM>(question));
        }

        public async Task NotifyGameUpdate(Game game)
        {
            await _hubContext.Clients
                .Group(game.Code)
                .UpdateParty(_mapper.Map<GameVM>(game));
        }
    }
}

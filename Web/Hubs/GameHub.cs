using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Web.Constants;
using Web.Hubs.Interfaces;
using Web.ViewModels;

namespace Web.Hubs
{
    public class GameHub : Hub<IGameHubClient>
    {
        private readonly GameService _gameService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public GameHub (GameService gm, UserService us, IMapper m)
        {
            _gameService = gm;
            _userService = us;
            _mapper = m;
        }

        [Authorize(Policy =ApiConstants.AuthenticatedUserPolicy)]
        public async Task CreatePartyAsync()
        {
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var game = await _gameService.CreateGame();

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
            await Clients.Group(game.Code).UpdateParty(_mapper.Map<GameVM>(game));
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}

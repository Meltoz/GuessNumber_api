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
    public class GameHub(GameService gm, UserService us, IMapper m) : Hub<IGameHubClient>
    {
        private readonly GameService _gameService = gm;
        private readonly UserService _userService = us;
        private readonly IMapper _mapper = m;

        [Authorize(Policy =ApiConstants.AuthenticatedUserPolicy)]
        public async Task CreatePartyAsync()
        {
            var userId = Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier));

            var game = await _gameService.CreateGame();

            await Clients.Caller.UpdateParty(_mapper.Map<GameVM>(game));
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}

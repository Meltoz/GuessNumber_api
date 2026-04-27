using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Domain.Enums;
using Domain.User;
using Web.Constants;
using Web.Hubs.Interfaces;
using Web.ViewModels;

namespace Web.Hubs;

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

    [Authorize(Policy = ApiConstants.AuthenticatedUserPolicy)]
    public async Task CreatePartyAsync()
    {
        var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.GetDetail(userId) as AuthUser;
        var game = await _gameService.CreateGame();

        game = await _gameService.JoinGame(game.Code, user, RoleParty.Owner, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
        await Clients.Group(game.Code).UpdateParty(_mapper.Map<GameVM>(game));
    }

    [Authorize(Policy = ApiConstants.AuthenticatedUserPolicy)]
    public async Task UpdatePartyAsync(GameVM game)
    {
        var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var gameUpdated = await _gameService.UpdateGameSettings(game.Code, userId, game.Configuration.MaxPlayers, game.Configuration.TotalQuestion, game.Configuration.Categories.Select(c => c.Id).ToList());

        await Clients.Group(game.Code).UpdateParty(_mapper.Map<GameVM>(gameUpdated));
    }
    
    public async Task JoinPartyAsync(string code)
    {
        var claim = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = claim != null ? await _userService.GetDetail(Guid.Parse(claim)) 
                : await _userService.CreateDefaultUser();
        
        
        var game = await _gameService.JoinGame(code, user, RoleParty.Player, Context.ConnectionId);

        
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Code);
        await Clients.Group(game.Code).UpdateParty(_mapper.Map<GameVM>(game));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var game = await _gameService.LeaveGame(Context.ConnectionId);

        if (game is not null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Code);
            await Clients.Group(game.Code).UpdateParty(_mapper.Map<GameVM>(game));
        }

        await base.OnDisconnectedAsync(exception);
    }
}


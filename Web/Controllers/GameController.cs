using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gs)
    {
        _gameService = gs;
    }
    
    [HttpGet]
    public async Task<IActionResult> PartyJoinable(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("Code is required");
        
        var isJoinable = await _gameService.GameIsJoinable(code);
        return Ok(isJoinable);
    }
} 
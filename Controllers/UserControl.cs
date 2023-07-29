using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;
using System.Linq;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserControl : ControllerBase
{
    private readonly IUserService _userService;

    private readonly IGameSessionService _gameSessionService;

    public UserControl(UserService userService, GameSessionService gameSessionService)
    {
        _userService = userService;
        _gameSessionService = gameSessionService;
    }
       

    [HttpPost]
    [Route("update-nickname/{userId}")]
    public async Task<IActionResult> UpdateNickName(string userId, [FromForm]string newName)
    {
        if (!await _userService.IsNickNameValid(newName)) 
        {
            return BadRequest("Nickname is already used !");
        }
   
        var user = await _userService.GetUserByUserIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        await _userService.ChangeNickname(userId, newName);

        return Ok();
    }

    [HttpPost]
    [Route("add-gold")]
    public async Task<IActionResult> AddGold(string userId, int addedGold)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var newGold = user.gold + addedGold;

        await _userService.UpdateGold(userId, newGold);

        return Ok(newGold);
    }
    [HttpPost]
    [Route("create-gamesession")]
    public async Task<IActionResult> CreateGameSession([FromBody]GameSessionModel gameModel)
    {
        if (gameModel == null) return BadRequest();
        await _gameSessionService.CreateAsync(gameModel);
        return Ok();
    }

    [HttpGet]
    [Route("get-gamesession")]
    public async Task<IActionResult> GetGameSession(string userId)
    {
        var gameSession = await _gameSessionService.GetGameSessionBySessionIdAsync(userId);
        if (gameSession is null)
        {
            return NotFound();
        }
        return Ok(gameSession);
    }
}
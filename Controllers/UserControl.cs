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

/*    [HttpPost]
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
    }*/
    private async Task AddGoldForUser(string userId, int addedGold)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);

        if (user is null)
        {
            return;
        }

        var newGold = user.gold + addedGold;

        await _userService.UpdateGold(userId, newGold);

    }
    private async Task AddRankForUser(string userId, int addedRank)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);

        if (user is null)
        {
            return;
        }

        var newRank = user.rank + addedRank;
        
        await _userService.UpdateRank(userId, newRank > 0 ? newRank:0);

    }
    [HttpPost]
    [Route("create-gamesession")]
    public async Task<IActionResult> CreateGameSession([FromBody]GameSessionModel gameModel)
    {
        if (gameModel == null) return BadRequest();
        if(gameModel.mode == ModeGame.Adventure)
        {
            if(gameModel.playerA == gameModel.playerWin)
            {
                await AddGoldForUser(gameModel.playerA, 100);
            }
            else
            {
                await AddGoldForUser(gameModel.playerA, 50);

            }
        }
        else if(gameModel.mode == ModeGame.Arena)
        {
            if (gameModel.playerA == gameModel.playerWin)
            {
                await AddGoldForUser(gameModel.playerA, 100);
                await AddRankForUser(gameModel.playerA, 1);

                await AddGoldForUser(gameModel.playerB, 50);
                await AddRankForUser(gameModel.playerB, -1);

            }
            else
            {
                await AddGoldForUser(gameModel.playerA, 50);
                await AddRankForUser(gameModel.playerA, -1);

                await AddGoldForUser(gameModel.playerB, 100);
                await AddRankForUser(gameModel.playerB, 1);


            }
        }
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
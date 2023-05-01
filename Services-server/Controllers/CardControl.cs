using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardControl : ControllerBase
{
    public readonly UserService _userService;

    public CardControl(UserService usersService)
    {
        _userService = usersService;
    }

    [HttpPost]
    [Route("upgradecard")]
    public async Task<IActionResult> UpgradeCard(string userId, int cardId)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        await _userService.UpgradeCard(userId, cardId);
        
        return Ok();
    }

    [HttpPost]
    [Route("buygacha")]
    public async Task<IActionResult> BuyGacha(string userId, int packType)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var receivedCard = await _userService.BuyGacha(userId, packType);
        return Ok(receivedCard);
    }

}

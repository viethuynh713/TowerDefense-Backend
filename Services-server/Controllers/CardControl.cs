using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardControl : ControllerBase
{
    public readonly IUserService _userService;

    public readonly ICardService _cardService;

    public CardControl(UserService userService, CardService cardService)
    {
        _userService = userService;
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<List<CardModel>> GetAllCards() =>
        await _cardService.GetAllCardsAsync();

    [HttpPost]
    [Route("addcard")]
    public async Task<IActionResult> AddCard([FromBody] CardModel newCard)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();

        }
        if (newCard.CardId != null)
        {
            var card = await _cardService.GetCard(newCard.CardId);
            if (card is not null)
            {
                return BadRequest("Duplicate card !");
            }
        }
        await _cardService.CreateCardAsync(newCard);
        return Ok(newCard);
    }

    [HttpPost]
    [Route("upgradecard")]
    public async Task<IActionResult> UpgradeCard(string userId, string oldCardId)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (!user.cardListID.Contains(oldCardId))
        {
            return BadRequest("Error when trying to get card info");
        }

        var newCardId = await _cardService.GetUpgradedCardId(oldCardId);

        await _userService.UpgradeCard(userId, oldCardId, newCardId);

        return Ok(newCardId);
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

        if (user.gold < _cardService.GetPriceOfGachaPack(packType))
        {
            return BadRequest("Not enough gold for this transaction!");
        }

        var receivedCardId = await _cardService.GenerateCardId(packType);
        return Ok(receivedCardId);
    }

}

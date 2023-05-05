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

    public CardControl(IUserService usersService, ICardService cardService)
    {
        _userService = usersService;
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<List<CardModel>> GetAllCards() =>
        await _cardService.GetAllCardsAsync();

    [HttpPost]
    [Route("addcard")]
    public async Task<IActionResult> AddCard(string cardId, string cardName, int cardStar, CardType typeOfCard, RarityCard cardRarity)
    {
        var card = await _cardService.GetCard(cardId);
        if (card is not null) 
        {
            return BadRequest("Duplicated card !");
        }
        var newCard = new CardModel
        {
            CardId = cardId,
            CardName = cardName,
            CardStar = cardStar,
            CardRarity = cardRarity,
            TypeOfCard = typeOfCard
        };

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

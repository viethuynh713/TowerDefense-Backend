using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardControl : ControllerBase
{
    private readonly IUserService _userService;

    private readonly ICardService _cardService;

    public CardControl(UserService userService, CardService cardService)
    {
        _userService = userService;
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<List<CardModel>> GetAllCards() =>
        await _cardService.GetAllCardsAsync();

    [HttpPost]
    [Route("add-card-to-database")]
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

    [HttpPut]
    [Route("upgrade-card/{userId}")]
    public async Task<IActionResult> UpgradeCard(string userId, [FromForm]string oldCardId)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (user.cardListID != null && !user.cardListID.Contains(oldCardId))
        {
            return BadRequest("Error when trying to get card info");
        }

        var oldCard = await _cardService.GetCard(oldCardId);

        if (oldCard != null)
        {
            var price = GetUpdatePrice(oldCard.CardStar);
            if (user.gold < price)
            {
                return BadRequest("Not enough gold for this transaction!");
            }
            var newCardId = await _cardService.GetUpgradedCardId(oldCardId);

            if (newCardId is null)
            {
                return BadRequest("The card is max level");
            }
            await _userService.UpgradeCard(userId, oldCardId, newCardId);
            await _userService.UpdateGold(userId, user.gold - price);
        }

        return Ok(await _userService.GetUserByUserIdAsync(userId));
    }

    [HttpPost]
    [Route("buy-gacha/{userId}")]
    public async Task<IActionResult> BuyGacha(string userId, [FromForm]GachaType packType)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        int pricePack = _cardService.GetPriceOfGachaPack(packType);
        if (user.gold < pricePack)
        {
            return BadRequest("Not enough gold for this transaction!");
        }

        var receivedCardId = await _cardService.GenerateCardId(packType);
        if (receivedCardId != null)
        {
            await _userService.AddCard(userId, receivedCardId);
            await _userService.UpdateGold(userId, user.gold - pricePack);
            return Ok(await _userService.GetUserByUserIdAsync(userId));
        }

        return BadRequest();
    }

    [HttpPost]
    [Route("buy-card/{userId}")]
    public async Task<IActionResult> BuyGacha([FromRoute]string userId, [FromForm]string cardId)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        var card = await _cardService.GetCard(cardId);
        if (card is null)
        {
            return NotFound();
        }
        var price = GetBuyPrice(card.CardRarity);
        if (user.gold < price)
        {
            return BadRequest("Not enough gold for this transaction!");
        }
        await _userService.AddCard(userId, cardId);
        await _userService.UpdateGold(userId, user.gold - price);
        return Ok(await _userService.GetUserByUserIdAsync(userId));
    }

    private int GetBuyPrice(RarityCard cardCardRarity)
    {
        switch (cardCardRarity)
        {
            case RarityCard.Common : return 200;
            case RarityCard.Rare : return 400;
            case RarityCard.Mythic : return 600;
            case RarityCard.Legend : return 800;
        }

        return -1;
    }

    private int GetUpdatePrice(int star)
    {
        switch (star)
        {
            case 0: return 50;
            case 1: return 100;
            case 2: return 200;
            case 3: return 300;
            case 4: return 500;
        }

        return -1;
    }
}

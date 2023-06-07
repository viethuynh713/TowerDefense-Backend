using Service.Models;

namespace Service.Services
{
    public interface ICardService
    {

        Task<List<CardModel>> GetAllCardsAsync();

        Task CreateCardAsync(CardModel newCard);
        int GetPriceOfGachaPack(GachaType packType);

        Task<CardModel?> GetCard(string cardId);

        Task<string?> GetUpgradedCardId(string cardId);

        Task<string?> GenerateCardId(GachaType packType);

        Task<List<string>> GetInitCardIdAsync();
    }
}

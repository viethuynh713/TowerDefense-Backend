using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Service.Services;


public class CardService : ICardService
{
    private readonly IMongoCollection<CardModel> _cardModelCollection;

    public CardService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _cardModelCollection = mongoDatabase.GetCollection<CardModel>(
            databaseSettings.Value.CardModelCollectionName);
    }
    public async Task<List<CardModel>> GetAllCardsAsync() =>
        await _cardModelCollection.Find(_ => true).ToListAsync();

    public async Task CreateCardAsync(CardModel newCard) =>
        await _cardModelCollection.InsertOneAsync(newCard);

    private List<int> GetRateList(int packType)
    {
        switch(packType)
        {
            case 0: return new List<int>() { 70, 25, 5 };
            case 1: return new List<int>() { 40, 50, 10 };
            case 2: return new List<int>() { 10, 75, 15 };
            default: return new List<int>() { };
        }
    }

    public int GetPriceOfGachaPack(int packType)
    {
        switch (packType)
        {
            case 0: return 200;
            case 1: return 400;
            case 2: return 600;
            default: return -1;
        }
    }

    //random
    private async Task<string> CalculateNextCardLevel(string oldCardId)
    {
        var oldCard = await _cardModelCollection.Find(x => x.CardId == oldCardId).FirstOrDefaultAsync();
        if (oldCard == null || oldCard.CardStar >= 5)
        {
            return "";
        }
        var filter = Builders<CardModel>.Filter.And(
            Builders<CardModel>.Filter.Eq("CardStar", oldCard.CardStar + 1),
            Builders<CardModel>.Filter.Eq("CardName", oldCard.CardName),
            Builders<CardModel>.Filter.Eq("TypeOfCard", oldCard.TypeOfCard),
            Builders<CardModel>.Filter.Eq("CardRarity", oldCard.CardRarity)
        );
        var newCard = await _cardModelCollection.Find(filter).FirstOrDefaultAsync();
        return newCard.CardId;
    }

    private async Task<string?> CalculateRandomCard(List<int> rateList)
    {
        var rand = new Random();
        var rarity = rand.Next(1, 101); // generate a random number between 1 and 100

        var filter = Builders<CardModel>.Filter.And(
            Builders<CardModel>.Filter.Eq("CardStar", 0),
            Builders<CardModel>.Filter.Eq("CardRarity", 1) // default value if none of the other conditions are met
        );

        if (rarity <= rateList[0])
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", 1)
            );
        }
        else if (rarity <= rateList[1] + rateList[0] )
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", 2)
            );
        }
        else
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", 3)
            );
        }

        var document = await _cardModelCollection.Find(filter).Limit(1).FirstOrDefaultAsync();
        return document.CardId;
    }

    public async Task<CardModel?> GetCard(string cardId) =>
       await _cardModelCollection.Find(x => x.CardId == cardId).FirstOrDefaultAsync();

    public async Task<string> GetUpgradedCardId(string cardId)
    {
        return await CalculateNextCardLevel(cardId);
    }

    public async Task<string?> GenerateCardId(int packType) 
    {
        var rateList = GetRateList(packType);
        return await CalculateRandomCard(rateList);
    }
    
}


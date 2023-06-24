using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System;

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

    private List<int> GetRateList(GachaType packType)
    {
        switch(packType)
        {
            case GachaType.Common: return new List<int>() { 70, 25, 5, 0 };
            case GachaType.Rare: return new List<int>() { 40, 50, 8, 2 };
            case GachaType.Legend: return new List<int>() { 10, 75, 10, 5 };
            default: return new List<int>() { };
        }
    }

    public int GetPriceOfGachaPack(GachaType packType)
    {
        switch (packType)
        {
            case GachaType.Common: return 200;
            case GachaType.Rare: return 400;
            case GachaType.Legend: return 600;
            default: return -1;
        }
    }

    
    private async Task<string?> CalculateNextCardLevel(string oldCardId)
    {
        var oldCard = await _cardModelCollection.Find(x => x.CardId == oldCardId).FirstOrDefaultAsync();
        if (oldCard == null || oldCard.CardStar >= 5)
        {
            return null;
        }
        var filter = Builders<CardModel>.Filter.And(
            Builders<CardModel>.Filter.Eq("CardStar", oldCard.CardStar + 1),
            Builders<CardModel>.Filter.Eq("CardName", oldCard.CardName),
            Builders<CardModel>.Filter.Eq("TypeOfCard", oldCard.TypeOfCard),
            Builders<CardModel>.Filter.Eq("CardRarity", oldCard.CardRarity)
        );
        var newCard = await _cardModelCollection.Find(filter).FirstOrDefaultAsync();
        if (newCard is null)
        {
            return null;
        }
        return newCard.CardId;
    }

    private async Task<string?> CalculateRandomCard(List<int> rateList)
    {
        var rand = new Random();
        var rarity = rand.Next(1, rateList.Sum() + 1); // generate a random number between 1 and 100

        FilterDefinition<CardModel> filter;

        if (rarity <= rateList[0])
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", RarityCard.Common)
            );
        }
        else if (rarity <= (rateList[1] + rateList[0]) )
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", RarityCard.Rare)
            );
        }
        else if (rarity <= (rateList[1] + rateList[0] + rateList[2]))
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", RarityCard.Mythic)
            );
        }
        else
        {
            filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", RarityCard.Legend)
            );
        }

        var document = await _cardModelCollection.Find(filter).ToListAsync();
        var randomNumber = rand.Next(0, document.Count);
        return document[randomNumber].CardId;
        
        
        
    }

    public async Task<CardModel?> GetCard(string cardId) =>
       await _cardModelCollection.Find(x => x.CardId == cardId).FirstOrDefaultAsync();

    public async Task<string?> GetUpgradedCardId(string cardId)
    {
        return await CalculateNextCardLevel(cardId);
    }

    public async Task<string?> GenerateCardId(GachaType packType) 
    {
        var rateList = GetRateList(packType);
        return await CalculateRandomCard(rateList);
    }

    public async Task<List<string>> GetInitCardIdAsync()
    {
        FilterDefinition<CardModel> filter = Builders<CardModel>.Filter.And(
                Builders<CardModel>.Filter.Eq("CardStar", 0),
                Builders<CardModel>.Filter.Eq("CardRarity", RarityCard.Common)
            );

        var document = await _cardModelCollection.Find(filter).ToListAsync();  
        Random random = new Random();
        List<string> selectedCardId = new List<string>();

        while (selectedCardId.Count < 4)
        {
            int rndIdx = random.Next(document.Count);
            var selectedElement = document[rndIdx].CardId;
            if (selectedElement != null && !selectedCardId.Contains(selectedElement))
            {
                selectedCardId.Add(selectedElement);
            } 
        }    

        return selectedCardId;
    }


}


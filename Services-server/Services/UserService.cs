using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Service.Services;


public class UserService
{
    private readonly IMongoCollection<UserModel> _userModelCollection;

    private readonly IMongoCollection<CardModel> _cardModelCollection;

    private int CalculateNextCardLevel(int cardId)
    {
        return cardId + 10; // test only, will do when listcard becomes available
    }

    private int CalculateReceivedCard(List<int> ownedCard, int gold, int packType)
    {
        var calculatedCardId = -1;
        Random rnd = new Random();
        while (ownedCard.Contains(calculatedCardId))
        {
            calculatedCardId = rnd.Next(1, 1001);
        }    
          // test only, will do when listcard becomes available
        return calculatedCardId;
    }

    public UserService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _userModelCollection = mongoDatabase.GetCollection<UserModel>(
            databaseSettings.Value.UserModelCollectionName);

        _cardModelCollection = mongoDatabase.GetCollection<CardModel>(
            databaseSettings.Value.CardModelCollectionName);
    }

    public async Task<List<UserModel>> GetAllUsersAsync() =>
        await _userModelCollection.Find(_ => true).ToListAsync();

    public async Task<List<CardModel>> GetAllCardsAsync() =>
        await _cardModelCollection.Find(_ => true).ToListAsync();

    public async Task CreateCardAsync(CardModel newCard) =>
        await _cardModelCollection.InsertOneAsync(newCard);

    public async Task RemoveAsync(string userId) =>
        await _userModelCollection.DeleteOneAsync(x => x.userId == userId);

    public async Task CreateUserAsync(UserModel newUser) =>
        await _userModelCollection.InsertOneAsync(newUser);
    // Get User method
    public async Task<UserModel?> GetUserByEmailAsync(string email) =>
        await _userModelCollection.Find(x => x.email == email).FirstOrDefaultAsync();

    public async Task<UserModel?> GetUserByUserIdAsync(string userId) =>
        await _userModelCollection.Find(x => x.userId == userId).FirstOrDefaultAsync();

    public async Task<UserModel?> GetUserByEmailPasswordAsync(string email, string password) =>
        await _userModelCollection.Find(x => x.email == email && x.password == password).FirstOrDefaultAsync();


    public async Task<CardModel?> GetCard(string cardId, string cardName, int cardLevel) =>
        await _cardModelCollection.Find(x => x.cardId == cardId || x.cardName == cardName && x.cardLevel == cardLevel).FirstOrDefaultAsync();
    // Change and update method
    public async Task ChangePassword(string email, string newPassword) 
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.email, email);
        var update = Builders<UserModel>.Update
            .Set(x => x.password, newPassword);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }
    public async Task ChangeNickname(string userId, string newName)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var update = Builders<UserModel>.Update
            .Set(x => x.nickName, newName);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateGold(string userId, int newGold)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var update = Builders<UserModel>.Update
            .Set(x => x.gold, newGold);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }

    public async Task UpgradeCard(string userId, int cardId)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var user = await _userModelCollection.Find(filter).FirstOrDefaultAsync();
        var list = user.cardListID;
        list.Remove(cardId);
        list.Add(CalculateNextCardLevel(cardId));

        var update = Builders<UserModel>.Update
            .Set(x => x.cardListID, list);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }

    public async Task BuyCard(string userId, int cardId)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var user = await _userModelCollection.Find(filter).FirstOrDefaultAsync();
        var price = cardId * 100;
    }

    public async Task<int> BuyGacha(string userId, int packType)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var user = await _userModelCollection.Find(filter).FirstOrDefaultAsync();
        var receivedCardId = CalculateReceivedCard(user.cardListID, user.gold, packType);
        if (receivedCardId == -1)
        {
            return -1;
        }
        var list = user.cardListID;
        list.Add(receivedCardId);

        var update = Builders<UserModel>.Update
            .Set(x => x.cardListID, list);
        await _userModelCollection.UpdateOneAsync(filter, update);
        return receivedCardId;
    }

}

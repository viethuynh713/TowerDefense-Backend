using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Service.Services;


public class UserService : IUserService
{
    private readonly IMongoCollection<UserModel> _userModelCollection;

    public UserService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _userModelCollection = mongoDatabase.GetCollection<UserModel>(
            databaseSettings.Value.UserModelCollectionName);
    }

    public async Task<List<UserModel>> GetAllUsersAsync() =>
        await _userModelCollection.Find(_ => true).ToListAsync();

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

    // Check and validate
    public bool IsValidEmail(string email)
    {
        // Use a regular expression to check if the email is valid
        string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);
        return match.Success;
    }
    public async Task<bool> IsNickNameValid(string nickName)
    {
        var user = await _userModelCollection.Find(x => x.nickName == nickName).FirstOrDefaultAsync();
        if (user is null) 
        {
            return true;
        }
        return false;
    }

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

    public async Task UpgradeCard(string userId, string oldCardId, string? newCardId)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var user = await _userModelCollection.Find(filter).FirstOrDefaultAsync();
        var list = user.cardListID;
        list?.Remove(oldCardId);
        if (newCardId != null) list?.Add(newCardId);

        var update = Builders<UserModel>.Update
            .Set(x => x.cardListID, list);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }

    public async Task AddCard(string userId, string newCardId)
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.userId, userId);
        var user = await _userModelCollection.Find(filter).FirstOrDefaultAsync();
        var list = user.cardListID;
        list.Add(newCardId);

        var update = Builders<UserModel>.Update
            .Set(x => x.cardListID, list);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }
}

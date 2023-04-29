using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Service.Services;


public class UserService
{
    private readonly IMongoCollection<UserModel>? _userModelCollection;
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

    public async Task<List<UserModel>> GetAsync() =>
        await _userModelCollection.Find(_ => true).ToListAsync();

    public async Task RemoveAsync(string userId) =>
        await _userModelCollection.DeleteOneAsync(x => x.userId == userId);

    public async Task CreateUserAsync(UserModel newUser) =>
        await _userModelCollection.InsertOneAsync(newUser);
    // Get User method
    public async Task<UserModel?> GetUserByEmailAsync(string i_email) =>
        await _userModelCollection.Find(x => x.email == i_email).FirstOrDefaultAsync();

    public async Task<UserModel?> GetUserByUserIdAsync(string userId) =>
        await _userModelCollection.Find(x => x.userId == userId).FirstOrDefaultAsync();

    public async Task<UserModel?> GetUserByEmailPasswordAsync(string i_email, string i_password) =>
        await _userModelCollection.Find(x => x.email == i_email && x.password == i_password).FirstOrDefaultAsync();

    public async Task ChangePassword(string i_email, string i_newPassword) 
    {
        var filter = Builders<UserModel>.Filter.Eq(x => x.email, i_email);
        var update = Builders<UserModel>.Update
            .Set(x => x.password, i_newPassword);

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

}

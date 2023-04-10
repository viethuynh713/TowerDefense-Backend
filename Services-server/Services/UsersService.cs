using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;



namespace Service.Services;


public class UsersService
{
    private readonly IMongoCollection<Users>? _userProfileCollection;
    public UsersService(
        IOptions<UsersDatabaseSettings> userProfileDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userProfileDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userProfileDatabaseSettings.Value.DatabaseName);

        _userProfileCollection = mongoDatabase.GetCollection<Users>(
            userProfileDatabaseSettings.Value.MythicEmpireCollectionName);
    }

    public async Task<List<Users>> GetAsync() =>
        await _userProfileCollection.Find(_ => true).ToListAsync();

    public async Task<Users?> GetAsync(string id) =>
        await _userProfileCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Users newUserProfile) =>
        await _userProfileCollection.InsertOneAsync(newUserProfile);

    public async Task UpdateAsync(string id, Users updatedUserProfile) =>
        await _userProfileCollection.ReplaceOneAsync(x => x.Id == id, updatedUserProfile);

    //public async Task RemoveAsync(string id) =>
    //    await _userProfileCollection.DeleteOneAsync(x => x.Id == id);
}

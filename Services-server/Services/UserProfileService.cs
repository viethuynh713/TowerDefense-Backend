using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;



namespace Service.Services;


public class UserProfileService
{
    private readonly IMongoCollection<UserProfile>? _userProfileCollection;
    public UserProfileService(
        IOptions<UserProfileDatabaseSettings> userProfileDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userProfileDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userProfileDatabaseSettings.Value.DatabaseName);

        _userProfileCollection = mongoDatabase.GetCollection<UserProfile>(
            userProfileDatabaseSettings.Value.UserProfileCollectionName);
    }

    public async Task<List<UserProfile>> GetAsync() =>
        await _userProfileCollection.Find(_ => true).ToListAsync();

    public async Task<UserProfile?> GetAsync(string id) =>
        await _userProfileCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(UserProfile newUserProfile) =>
        await _userProfileCollection.InsertOneAsync(newUserProfile);

    public async Task UpdateAsync(string id, UserProfile updatedUserProfile) =>
        await _userProfileCollection.ReplaceOneAsync(x => x.Id == id, updatedUserProfile);

    public async Task RemoveAsync(string id) =>
        await _userProfileCollection.DeleteOneAsync(x => x.Id == id);
}

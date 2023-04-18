using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;



namespace Service.Services;


public class UsersService
{
    private readonly IMongoCollection<Users>? _usersCollection;
    public UsersService(
        IOptions<UsersDatabaseSettings> userProfileDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userProfileDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userProfileDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<Users>(
            userProfileDatabaseSettings.Value.MythicEmpireCollectionName);
    }

    public async Task<List<Users>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<Users?> GetAsync(string userId) =>
        await _usersCollection.Find(x => x.userId == userId).FirstOrDefaultAsync();

    public async Task<Users?> GetAsync(string email, string password) => 
        await _usersCollection.Find(x => x.email == email && x.password == password).FirstOrDefaultAsync();
    public async Task CreateAsync(Users newUserProfile) =>
        await _usersCollection.InsertOneAsync(newUserProfile);

    public async Task UpdateAsync(string id, Users updatedUserProfile) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUserProfile);

    //public async Task RemoveAsync(string id) =>
    //    await _userProfileCollection.DeleteOneAsync(x => x.Id == id);
}

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

    public async Task CreateUserAsync(Users newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task<Users?> GetEmailAsync(string i_email) =>
    await _usersCollection.Find(x => x.email == i_email).FirstOrDefaultAsync();

    public async Task<Users?> GetEmailPasswordAsync(string i_email, string i_password) =>
        await _usersCollection.Find(x => x.email == i_email && x.password == i_password).FirstOrDefaultAsync();
    public async Task ChangePassword(string i_email, string i_newPassword) 
    {
        var filter = Builders<Users>.Filter.Eq(x => x.email, i_email);
        var update = Builders<Users>.Update
            .Set(x => x.password, i_newPassword);

        await _usersCollection.UpdateOneAsync(filter, update);
    }
    //public async Task UpdateAsync(string i_email, Users i_updatedUser) =>
    // await _usersCollection.ReplaceOneAsync(x => x.email == i_email, i_updatedUser);

    //public async Task RemoveAsync(string id) =>
    //    await _userProfileCollection.DeleteOneAsync(x => x.Id == id);
}

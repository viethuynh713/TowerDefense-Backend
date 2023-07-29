using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Service.Services;


public class GameSessionService : IGameSessionService
{
    private readonly IMongoCollection<GameSessionModel>? _gameSessionModelCollection;
    public GameSessionService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _gameSessionModelCollection = mongoDatabase.GetCollection<GameSessionModel>(
            databaseSettings.Value.GameSessionModelCollectionName);
    }

    public async Task<List<GameSessionModel>> GetAsync() =>
        await _gameSessionModelCollection.Find(_ => true).ToListAsync();

    public async Task RemoveAsync(string gameId) =>
        await _gameSessionModelCollection.DeleteOneAsync(x => x.gameId == gameId);

    public async Task CreateAsync(GameSessionModel newGameSession) =>
        await _gameSessionModelCollection?.InsertOneAsync(newGameSession)!;
    // Get User method

    public async Task<List<GameSessionModel>?> GetGameSessionBySessionIdAsync(string userId) =>
        await _gameSessionModelCollection.Find(x => x.playerA == userId || x.playerB == userId).ToListAsync();
}

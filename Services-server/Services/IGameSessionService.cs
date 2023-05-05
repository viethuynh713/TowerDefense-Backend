using Service.Models;

namespace Service.Services
{
    public interface IGameSessionService
    {
        Task<List<GameSessionModel>> GetAsync();
        Task RemoveAsync(string sessionId);
        Task CreateAsync(GameSessionModel newGameSession);

        Task<List<GameSessionModel>?> GetGameSessionBySessionIdAsync(string userId);
    }
}

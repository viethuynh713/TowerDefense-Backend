using Service.Models;

namespace Service.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllUsersAsync();

        Task RemoveAsync(string userId);

        Task CreateUserAsync(UserModel newUser);
        Task<UserModel?> GetUserByEmailAsync(string email);

        Task<UserModel?> GetUserByUserIdAsync(string userId);

        Task<UserModel?> GetUserByEmailPasswordAsync(string email, string password);

        bool IsValidEmail(string email);

        Task<bool> IsNickNameValid(string nickName);

        Task ChangePassword(string email, string newPassword);

        Task ChangeNickname(string userId, string newName);

        Task UpdateGold(string userId, int newGold);

        Task UpgradeCard(string userId, string oldCardId, string? newCardId);

        Task AddCard(string userId, string newCardId);

        void SendOTP(string email, string otp);

        bool IsValidOTP(string email, string otpCode);
    }
}

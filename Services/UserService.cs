using Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System;
using System.Text;
using System.Timers;

namespace Service.Services;


public class UserService : IUserService
{
    private readonly IMongoCollection<UserModel> _userModelCollection;

    private readonly Dictionary<string, string> _dictionaryOTP;

    public UserService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _userModelCollection = mongoDatabase.GetCollection<UserModel>(
            databaseSettings.Value.UserModelCollectionName);

        _dictionaryOTP = new Dictionary<string, string>();
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
        list?.Add(newCardId);

        var update = Builders<UserModel>.Update
            .Set(x => x.cardListID, list);

        await _userModelCollection.UpdateOneAsync(filter, update);
    }

    public async void SendOTP(string email, string otpCode)
    {
        string senderEmail = "no.reply.mythicempire@gmail.com";
        string senderPassword = "ppwoaslpjefvayhk";

        var user = await GetUserByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        string subject = "Your Mythic Empire account: Reset your password";
        string body =  "Dear " + user.nickName + ",\r\n\r\n" +
                       "We have received a request to reset your password for your Mythic Empire account.\r\n\r\n" +
                       "Here is your OTP code:    " + otpCode + "\r\n\r\n" +
                       "Please note that the OTP is only valid for a limited time and will expire if not used within 5 minutes. If you do not attempt to reset your password, please disregard this email.\r\n\r\n" + 
                       "If you did not initiate this password reset request, we recommend that you contact our support team immediately to ensure the security of your account.\r\n\r\n" + 
                       "Thank you for using Mythic Empire. If you have any further questions or require assistance, please don't hesitate to reach out to our support team.\r\n\r\n" + 
                       "Best regards,\r\n\r\n" + 
                       "Mythic Empire Support Team";
                        

        if (_dictionaryOTP.ContainsKey(email))
        {
            _dictionaryOTP[email] = otpCode;

        }
        else
        {
            _dictionaryOTP.Add(email, otpCode);
        }

        System.Timers.Timer timer = new System.Timers.Timer(5 * 60 * 1000); 
        timer.AutoReset = false;
        timer.Elapsed += (sender, e) => TimerElapsed(sender, e, email, otpCode);
        timer.Start();

        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);

        MailMessage mail = new MailMessage(senderEmail, email, subject, body);
        mail.BodyEncoding = Encoding.UTF8;
        mail.HeadersEncoding = Encoding.UTF8;
        try
        {
            client.Send(mail);
        }
        catch (Exception ex)
        {
            throw new Exception("Email is not sent with error: " + ex.ToString());
        }
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e, string email, string otpCode)
    {
        RemoveOldOTP(email, otpCode);
    }

    public bool IsValidOTP(string email, string otpCode)
    {
        if(_dictionaryOTP.ContainsKey(email) && _dictionaryOTP[email] == otpCode)
        {
            return true;
        }
        return false;
    }
    private void RemoveOldOTP(string email, string otpCode)
    {
        if (_dictionaryOTP.ContainsKey(email))
        {
            if (_dictionaryOTP[email] == otpCode)
            {
                _dictionaryOTP.Remove(email);
                // Console.WriteLine("Removed " + email + " with OTP code: " + otpCode);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;
using System.Text.RegularExpressions;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenControl : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICardService _cardService;
    private Dictionary<string, string> _dictionaryOTP;

    public AuthenControl(UserService userService, CardService cardService)
    {
        _userService = userService;
        _cardService = cardService;
        _dictionaryOTP = new Dictionary<string, string>();
    }

    [HttpGet]
    public async Task<List<UserModel>> GetAll() =>
        await _userService.GetAllUsersAsync();

    // [HttpGet]
    // [Route("delete")]
    // public async Task Delete(string userId) =>
    //     await _userService.RemoveAsync(userId);

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(string email, string nickName, string password)
    {

        if (!_userService.IsValidEmail(email))
        {
            return BadRequest("Wrong email format !");
        }

        if (!await _userService.IsNickNameValid(nickName))
        {
            return BadRequest("Nickname is already used !");
        }

        var user = await _userService.GetUserByEmailAsync(email);
        if (user is not null)
        {
            return BadRequest("This email is already used !");
        }

        var initCard = await _cardService.GetInitCardIdAsync();

        var newUser = new UserModel
        {
            email = email,
            password = password,
            userId = Guid.NewGuid().ToString(),
            nickName = nickName,
            gold = 0,
            cardListID = initCard, //todo: 4 card (CardStar = 0, CardRarity = 1)
            friendListID = new List<string>()
        };

        await _userService.CreateUserAsync(newUser);
        return Ok("Register user successfully !");
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login(string email, string userPassword)
    {
        if (!_userService.IsValidEmail(email))
        {
            return BadRequest("Wrong email format !");
        }
        
        var user = await _userService.GetUserByEmailPasswordAsync(email, userPassword);
        if (user is null)
        {
            return BadRequest("Email or password is incorrect");
        }
        return Ok(user);
    }
    [HttpGet]
    [Route("login-id")]
    public async Task<IActionResult> LoginById(string userId)
    {
        var user = await _userService.GetUserByUserIdAsync(userId);
        if (user is null)
        {
            return NotFound("The user does not exist");
        }

        return Ok(user);
    }
    [HttpPut]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword(string email, string newPassword)
    {
        var user = await _userService.GetUserByEmailAsync(email);

        if (user is null)
        {
            return NotFound();
        }

        await _userService.ChangePassword(email, newPassword);
        return Ok();
    }

    [HttpGet]
    [Route("send-otp")]
    public async Task<ActionResult> SendOTP(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        // TODO: Gen OTP
        var otp = "1";
        
        // TODO: Send otp to email
        // TODO: Save OTP  to _dictionaryOTP
        if (_dictionaryOTP.ContainsKey(email))
        {
            _dictionaryOTP[email] = otp;
            
        }
        else
        {
            _dictionaryOTP.Add(email,otp);
        }

        return Ok("");
    }
    [HttpPost]
    [Route("valid-otp")]
    public Task<ActionResult> IsValidOTP(string email, string otp)
    {
        if (_dictionaryOTP.ContainsKey(email))
        {
            if (_dictionaryOTP[email] == otp)
            {
                return Task.FromResult<ActionResult>(Ok());
            }
        }
        return Task.FromResult<ActionResult>(BadRequest());
    }
}
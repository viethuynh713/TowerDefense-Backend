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

    public AuthenControl(UserService usersService) =>
        _userService = usersService;

    [HttpGet]
    public async Task<List<UserModel>> GetAll() =>
        await _userService.GetAllUsersAsync();

    [HttpGet]
    [Route("delete")]
    public async Task Delete(string userId) =>
        await _userService.RemoveAsync(userId);

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

        var newUser = new UserModel
        {
            email = email,
            password = password,
            userId = Guid.NewGuid().ToString(),
            nickName = nickName,
            gold = 0,
            cardListID = new List<string>(), //todo: 4 card (CardStar = 0, CardRarity = 1)
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

    [HttpPost]
    [Route("resetpw")]
    public async Task<IActionResult> ChangePassword(string email, string oldPassword, string newPassword)
    {
        var oldUser = await _userService.GetUserByEmailAsync(email);

        if (oldUser is null)
        {
            return NotFound();
        }

        if (oldUser.password != oldPassword)
        {
            return BadRequest("Wrong password");
        }    

        await _userService.ChangePassword(email, newPassword);

        return Ok();
    }

}
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenControl : ControllerBase
{
    private readonly UserService _userService;

    public AuthenControl(UserService usersService) =>
        _userService = usersService;

    [HttpGet]
    public async Task<List<UserModel>> Get() =>
        await _userService.GetAsync();

    [HttpGet]
    [Route("delete")]
    public async Task Delete(string userId) =>
        await _userService.RemoveAsync(userId);

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(string email, string nickName, string password)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user is not null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new UserModel
        {
            email = email,
            password = password,
            userId = Guid.NewGuid().ToString(),
            nickName = nickName,
            gold = 0,
            cardListID = new List<int>(),
            friendListID = new List<string>()
        };

        await _userService.CreateUserAsync(newUser);
        return Ok(newUser);
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login(string email, string userPassword)
    {
        var user = await _userService.GetUserByEmailPasswordAsync(email, userPassword);
        if (user is null)
        {
            return BadRequest("Username or password is incorrect");
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
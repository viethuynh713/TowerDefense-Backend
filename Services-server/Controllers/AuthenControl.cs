using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenControl : ControllerBase
{
    private readonly UserService _usersService;

    public AuthenControl(UserService usersService) =>
        _usersService = usersService;

    [HttpGet]
    public async Task<List<UserModel>> Get() =>
        await _usersService.GetAsync();

    [HttpGet]
    [Route("delete")]
    public async Task Delete(string userId) =>
        await _usersService.RemoveAsync(userId);

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(string email, string nickName, string password)
    {
        var user = await _usersService.GetUserByEmailAsync(email);
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

        await _usersService.CreateUserAsync(newUser);
        return Ok(newUser);
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login(string email, string userPassword)
    {
        var user = await _usersService.GetUserByEmailPasswordAsync(email, userPassword);
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
        var oldUser = await _usersService.GetUserByEmailAsync(email);

        if (oldUser is null)
        {
            return NotFound();
        }

        if (oldUser.password != oldPassword)
        {
            return BadRequest("Wrong password");
        }    

        await _usersService.ChangePassword(email, newPassword);

        return Ok();
    }

}
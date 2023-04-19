using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService booksService) =>
        _usersService = booksService;

    [HttpGet]
    public async Task<List<Users>> Get() =>
        await _usersService.GetAsync();

    //public async Task<ActionResult<Users>> Get(string i_email)
    //{
    //    var user = await _usersService.GetEmailAsync(i_email);

    //    if (user is null)
    //    {
    //        return NotFound();
    //    }

    //    return user;
    //}

    //[HttpPost]
    //public async Task<IActionResult> Post(Users i_newUser)
    //{
    //    await _usersService.CreateUserAsync(i_newUser);

    //    return Ok(i_newUser);
    //}


    //public async Task<IActionResult> Update(string id, Users updatedUserProfile)
    //{
    //    var oldUserProfile = await _usersService.GetAsync(id);

    //    if (oldUserProfile is null)
    //    {
    //        return NotFound();
    //    }

    //    updatedUserProfile.Id = oldUserProfile.Id;

    //    await _usersService.UpdateAsync(id, updatedUserProfile);

    //    return Ok();
    //}

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(string i_email, string i_nickName, string i_password)
    {
        var user = await _usersService.GetEmailAsync(i_email);
        if (user is not null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new Users{
            email = i_email,
            password = i_password,
            userId = Guid.NewGuid().ToString(),
            nickName = i_nickName,
            currency = 0,
            cards = new List<int>(),
            friends = new List<string>()
        };

        await _usersService.CreateUserAsync(newUser);
        return Ok(newUser);
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> Login(string i_email, string i_userPassword)
    {
        var user = await _usersService.GetEmailPasswordAsync(i_email, i_userPassword);
        if (user is null)
        {
            return BadRequest("Username or password is incorrect");
        }
        return Ok(user);
    }

    [HttpPost]
    [Route("resetpw")]
    public async Task<IActionResult> ChangePassword(string i_email, string i_oldPassword, string i_newPassword)
    {
        var oldUser = await _usersService.GetEmailAsync(i_email);

        if (oldUser is null)
        {
            return NotFound();
        }

        if (oldUser.password != i_oldPassword)
        {
            return BadRequest("Wrong password");
        }    

        await _usersService.ChangePassword(i_email, i_newPassword);

        return Ok();
    }

    //[HttpDelete("{id:length(24)}")]
    //public async Task<IActionResult> Delete(string id)
    //{
    //    var userProfile = await _userProfileService.GetAsync(id);

    //    if (userProfile is null)
    //    {
    //        return NotFound();
    //    }

    //    await _userProfileService.RemoveAsync(id);

    //    return Ok();
    //}
}
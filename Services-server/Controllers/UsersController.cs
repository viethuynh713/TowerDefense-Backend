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

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Users>> Get(string id)
    {
        var userProfile = await _usersService.GetAsync(id);

        if (userProfile is null)
        {
            return NotFound();
        }

        return userProfile;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Users newUserProfile)
    {
        await _usersService.CreateAsync(newUserProfile);

        return CreatedAtAction(nameof(Get), new { id = newUserProfile.Id }, newUserProfile);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Users updatedUserProfile)
    {
        var oldUserProfile = await _usersService.GetAsync(id);

        if (oldUserProfile is null)
        {
            return NotFound();
        }

        updatedUserProfile.Id = oldUserProfile.Id;

        await _usersService.UpdateAsync(id, updatedUserProfile);

        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> OnRegister(string i_email, string i_userId, string i_password)
    {
        var user = _usersService.GetAsync(i_userId);
        if (user is not null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new Users{
            email = i_email,
            userId = i_userId,
            password = i_password
        };

        _usersService.CreateAsync(newUser);
        return Ok(newUser);
    }

    [HttpGet]
    [Route("login")]
    public async Task<IActionResult> OnLogin(string i_email, string i_userPassword)
    {
        var user = _usersService.GetAsync(i_email, i_userPassword);
        if (user is null)
        {
            return BadRequest("Username or password is incorrect");
        }
        return Ok(user);
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
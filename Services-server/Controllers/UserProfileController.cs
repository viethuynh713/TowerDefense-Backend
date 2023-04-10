using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Service.Models;

namespace Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly UserProfileService _userProfileService;

    public UserProfileController(UserProfileService booksService) =>
        _userProfileService = booksService;

    [HttpGet]
    public async Task<List<UserProfile>> Get() =>
        await _userProfileService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<UserProfile>> Get(string id)
    {
        var userProfile = await _userProfileService.GetAsync(id);

        if (userProfile is null)
        {
            return NotFound();
        }

        return userProfile;
    }

    [HttpPost]
    public async Task<IActionResult> Post(UserProfile newUserProfile)
    {
        await _userProfileService.CreateAsync(newUserProfile);

        return CreatedAtAction(nameof(Get), new { id = newUserProfile.Id }, newUserProfile);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, UserProfile updatedUserProfile)
    {
        var oldUserProfile = await _userProfileService.GetAsync(id);

        if (oldUserProfile is null)
        {
            return NotFound();
        }

        updatedUserProfile.Id = oldUserProfile.Id;

        await _userProfileService.UpdateAsync(id, updatedUserProfile);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userProfile = await _userProfileService.GetAsync(id);

        if (userProfile is null)
        {
            return NotFound();
        }

        await _userProfileService.RemoveAsync(id);

        return NoContent();
    }
}
using IssuePortal.Api.Models;
using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var createdUser = await _userService.CreateUserAsync(user);

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.Id },
            createdUser
        );
    }

    [HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, User user)
{
    var updatedUser = await _userService.UpdateUserAsync(id, user);

    if (updatedUser == null)
    {
        return NotFound();
    }

    return Ok(updatedUser);
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    var deleted = await _userService.DeleteUserAsync(id);

    if (!deleted)
    {
        return NotFound();
    }

    return NoContent();
}
}
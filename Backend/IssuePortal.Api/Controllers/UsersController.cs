using IssuePortal.Api.Models;
using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IssuePortal.Api.Extensions;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
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

    [HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
{
    var result = await _userService.UpdateUserAsync(id, dto);

    if (result.User == null)
    {
        if (result.Error == null)
        {
            return NotFound();
        }

        return BadRequest(result.Error);
    }

    return Ok(result.User);
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

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDto dto)
    {
        var result = await _userService.UpdateRoleAsync(id, dto, User.ToCurrentUser());

        if (result.User == null)
        {
            if (result.Error == null)
            {
                return NotFound();
            }

            return BadRequest(result.Error);
        }

        return Ok(result.User);
    }
}

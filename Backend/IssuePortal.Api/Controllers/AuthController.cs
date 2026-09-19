using Microsoft.AspNetCore.Mvc;
using IssuePortal.Api.DTOs;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(RegisterDto registerDto)
    {
        return Ok(registerDto);
    }
}
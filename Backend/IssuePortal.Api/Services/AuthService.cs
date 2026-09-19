using IssuePortal.Api.Data;
using IssuePortal.Api.DTOs;
using IssuePortal.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class AuthService
{
    private readonly IssuePortalDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IssuePortalDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            registerDto.Password
        );

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<User> LoginAsync(LoginDto loginDto)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

    if (user == null)
    {
        throw new InvalidOperationException("Invalid email or password.");
    }

    var result = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        loginDto.Password
    );

    if (result == PasswordVerificationResult.Failed)
    {
        throw new InvalidOperationException("Invalid email or password.");
    }

    return user;
}
}
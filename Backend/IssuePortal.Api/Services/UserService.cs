using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class UserService
{
    private readonly IssuePortalDbContext _context;

    public UserService(IssuePortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);

        if (existingUser == null)
        {
            return null;
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;

        await _context.SaveChangesAsync();

        return existingUser;
    }



    public async Task<bool> DeleteUserAsync(int id)
{
    var user = await _context.Users.FindAsync(id);

    if (user == null)
    {
        return false;
    }

    _context.Users.Remove(user);

    await _context.SaveChangesAsync();

    return true;
}
}
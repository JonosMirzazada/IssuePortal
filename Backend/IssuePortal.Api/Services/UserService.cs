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

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.AssignedIssues)
            .ToListAsync();

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,

            AssignedIssues = user.AssignedIssues.Select(i => new UserIssueDto
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status,
                Priority = i.Priority
            }).ToList()
        }).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.AssignedIssues)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,

            AssignedIssues = user.AssignedIssues.Select(i => new UserIssueDto
            {
                Id = i.Id,
                Title = i.Title,
                Status = i.Status,
                Priority = i.Priority
            }).ToList()
        };
    }

    public async Task<UserDto> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            AssignedIssues = new List<UserIssueDto>()
        };
    }

    public async Task<UserDto?> UpdateUserAsync(int id, User user)
{
    var existingUser = await _context.Users.FindAsync(id);

    if (existingUser == null)
    {
        return null;
    }

    existingUser.Name = user.Name;
    existingUser.Email = user.Email;

    await _context.SaveChangesAsync();

    return new UserDto
    {
        Id = existingUser.Id,
        Name = existingUser.Name,
        Email = existingUser.Email,
        CreatedAt = existingUser.CreatedAt,
        AssignedIssues = new List<UserIssueDto>()
    };
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
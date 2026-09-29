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
            Role = user.Role,
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
            Role = user.Role,
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

    public async Task<(UserDto? User, string? Error)> UpdateUserAsync(int id, UpdateUserDto dto)
{
    var existingUser = await _context.Users.FindAsync(id);

    if (existingUser == null)
    {
        return (null, null);
    }

    var emailTaken = await _context.Users
        .AnyAsync(u => u.Email == dto.Email && u.Id != id);

    if (emailTaken)
    {
        return (null, "Email already exists.");
    }

    existingUser.Name = dto.Name;
    existingUser.Email = dto.Email;

    await _context.SaveChangesAsync();

    return (new UserDto
    {
        Id = existingUser.Id,
        Name = existingUser.Name,
        Email = existingUser.Email,
        Role = existingUser.Role,
        CreatedAt = existingUser.CreatedAt,
        AssignedIssues = new List<UserIssueDto>()
    }, null);
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

    public async Task<(UserDto? User, string? Error)> UpdateRoleAsync(int id, UpdateUserRoleDto dto, CurrentUser currentUser)
    {
        // Prevents an admin from accidentally locking themselves out
        if (id == currentUser.Id)
        {
            return (null, "You cannot change your own role.");
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return (null, null);
        }

        user.Role = dto.Role;

        await _context.SaveChangesAsync();

        return (new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            AssignedIssues = new List<UserIssueDto>()
        }, null);
    }
}

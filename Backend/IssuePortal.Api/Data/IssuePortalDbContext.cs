using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Data;

public class IssuePortalDbContext : DbContext
{
    public IssuePortalDbContext(DbContextOptions<IssuePortalDbContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues { get; set; }
}
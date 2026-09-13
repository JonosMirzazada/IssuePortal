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

    public DbSet<Project> Projects { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Comment> Comments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Comment -> Issue
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Issue)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment -> User
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Issue -> Assigned User
        modelBuilder.Entity<Issue>()
            .HasOne<User>()
            .WithMany(u => u.AssignedIssues)
            .HasForeignKey(i => i.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
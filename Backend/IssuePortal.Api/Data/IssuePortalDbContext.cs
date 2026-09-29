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

    public DbSet<ProjectMember> ProjectMembers { get; set; }


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

        // ProjectMember -> Project
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProjectMember -> User
        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // A user can only be a member of a project once
        modelBuilder.Entity<ProjectMember>()
            .HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();
    }
}
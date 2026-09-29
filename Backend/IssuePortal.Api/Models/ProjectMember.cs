using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IssuePortal.Api.Models;

public class ProjectMember
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    [ValidateNever]
    public Project? Project { get; set; }

    public int UserId { get; set; }

    [ValidateNever]
    public User? User { get; set; }

    public DateTime JoinedAt { get; set; }
}

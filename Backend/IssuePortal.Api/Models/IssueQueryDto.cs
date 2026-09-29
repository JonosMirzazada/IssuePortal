namespace IssuePortal.Api.Models;

// Optional filters for GET /api/issues. Filters that are not set are ignored.
public class IssueQueryDto
{
    public int? ProjectId { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public int? AssignedUserId { get; set; }

    // Shortcut for AssignedUserId = the current user
    public bool AssignedToMe { get; set; }
}

namespace IssuePortal.Api.Models;

public class Issue
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";

    public string Priority { get; set; } = "Medium";

     public DateTime CreatedAt { get; set; }
     public DateTime UpdatedAt { get; set; }
     public int ProjectId { get; set; }
     public int? AssignedUserId { get; set; }
}
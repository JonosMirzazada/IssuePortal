namespace IssuePortal.Api.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int IssueId { get; set; }

    public int UserId { get; set; }
}
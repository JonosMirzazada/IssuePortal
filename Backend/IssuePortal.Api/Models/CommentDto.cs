namespace IssuePortal.Api.Models;

public class CommentDto
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int IssueId { get; set; }

    public string IssueTitle { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;
}
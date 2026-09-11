namespace IssuePortal.Api.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;
    public Issue Issue { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int IssueId { get; set; }
    public User User { get; set; } = null!;

    public int UserId { get; set; }
}
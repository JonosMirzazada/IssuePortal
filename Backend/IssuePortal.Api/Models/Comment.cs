using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IssuePortal.Api.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int IssueId { get; set; }

    [ValidateNever]
    public Issue? Issue { get; set; }

    public int UserId { get; set; }

    [ValidateNever]
    public User? User { get; set; }
}
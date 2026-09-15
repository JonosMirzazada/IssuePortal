using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class Issue
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Open";

    [Required]
    public string Priority { get; set; } = "Medium";

    public List<Comment> Comments { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int ProjectId { get; set; }

    public int? AssignedUserId { get; set; }
}
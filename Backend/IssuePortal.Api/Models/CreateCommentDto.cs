using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class CreateCommentDto
{
    public int IssueId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;
}

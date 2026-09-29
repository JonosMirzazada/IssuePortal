using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class UpdateCommentDto
{
    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class UpdateIssueDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [AllowedValues(IssueStatuses.Open, IssueStatuses.InProgress, IssueStatuses.Closed)]
    public string Status { get; set; } = IssueStatuses.Open;

    [AllowedValues(IssuePriorities.Low, IssuePriorities.Medium, IssuePriorities.High)]
    public string Priority { get; set; } = IssuePriorities.Medium;

    public int ProjectId { get; set; }

    public int? AssignedUserId { get; set; }
}

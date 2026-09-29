using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class UpdateUserRoleDto
{
    [Required]
    [AllowedValues(Roles.User, Roles.Developer, Roles.Admin)]
    public string Role { get; set; } = string.Empty;
}

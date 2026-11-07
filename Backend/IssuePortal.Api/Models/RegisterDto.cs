using System.ComponentModel.DataAnnotations;

namespace IssuePortal.Api.Models;

public class RegisterDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;
}
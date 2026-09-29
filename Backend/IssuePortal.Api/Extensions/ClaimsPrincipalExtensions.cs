using System.Security.Claims;
using IssuePortal.Api.Models;

namespace IssuePortal.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal principal)
    {
        var id = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return new CurrentUser(id, principal.IsInRole("Admin"));
    }
}

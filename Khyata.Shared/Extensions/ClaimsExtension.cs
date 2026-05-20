using khyata.Domain.Enums;
using System.Security.Claims;

namespace khyata.Application.Extensions
{
    public static class ClaimsExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal user) =>
       Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? user.FindFirstValue("sub")!);

        public static Guid GetWorkspaceId(this ClaimsPrincipal user)
        {
            var workspaceId = user.FindFirstValue("wid");

            if (string.IsNullOrWhiteSpace(workspaceId))
                throw new UnauthorizedAccessException("Workspace claim is missing.");

            return Guid.Parse(workspaceId);
        }
        public static string GetRole(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.Role) ?? UserRole.Employee.ToString();
    }
}

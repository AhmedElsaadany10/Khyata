using Khyata.Application.Helpers;
using Khyata.Domain.Enums;
using System.Security.Claims;

namespace Khyata.Application.Extensions
{
    public static class MainClaimsExtension
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
            user.FindFirstValue("workspace_role") ?? WorkspaceRole.Employee.ToString();
    }
    // ── JWT claim helpers for admin controllers ───────────────────────────────────
    public static class AdminMainClaimsExtensions
    {
        public static Guid GetAdminId(this ClaimsPrincipal user) =>
            Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? user.FindFirstValue("sub")!);

        public static string GetAdminUsername(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("name") ?? string.Empty;

        public static string[] GetAdminRoles(this ClaimsPrincipal user) =>
            user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

        public static bool IsSuperAdmin(this ClaimsPrincipal user) =>
            user.IsInRole(AdminRoles.SuperAdmin);
    }
}

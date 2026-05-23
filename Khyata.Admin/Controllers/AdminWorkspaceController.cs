using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.DTOs.Admin.Workspace_User;
using Khyata.Application.DTOs.Admin.WorkspaceUser;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Domain.Enums;
using Khyata.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.Admin.Controllers
{
    [ApiController]
    [Route("admin/workspace")]
    [Authorize(Policy = AdminPolicies.AnyAdmin)]
    public class AdminWorkspaceController : ControllerBase
    {
        private readonly IAdminWorkspaceRepository _adminWorkspaceRepository;

        public AdminWorkspaceController(IAdminWorkspaceRepository adminWorkspaceRepository)
        {
            _adminWorkspaceRepository = adminWorkspaceRepository;
        }
        /// <summary>
        /// List all workspace users across the system.
        /// Moderators: includeDeleted = false only.
        /// SuperAdmins: can pass includeDeleted = true to see soft-deleted users.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(PagedResult<SystemUserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsers(
            [FromQuery] Guid? workspaceId = null,
            [FromQuery] string? role = null,
            [FromQuery] bool includeDeleted = false,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            // Only SuperAdmins can view deleted users
            var canViewDeleted = User.IsSuperAdmin() && includeDeleted;

            var result = await _adminWorkspaceRepository.GetAllUsersAsync(
                workspaceId, role, canViewDeleted,
                new PaginationQuery { Page = page, Limit = limit });

            return this.ToActionResult(result);
        }
        /// <summary>Get a single workspace user by id (visible even if soft-deleted).</summary>
        [HttpGet("users/{id:guid}")]
        [ProducesResponseType(typeof(SystemUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _adminWorkspaceRepository.GetUserAsync(id);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Reset a workspace user's password (owner or employee).
        /// SuperAdmin only — no current password needed.
        /// </summary>
        [HttpPatch("users/{id:guid}/reset-password")]
        [Authorize(Policy = AdminPolicies.SuperAdminOnly)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetWorkspaceUserPasswordDto dto)
        {
            var result = await _adminWorkspaceRepository.ResetWorkspaceUserPasswordAsync(id, dto);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// List all workspaces. Filter: status = PendingActivation | Active | Suspended
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<WorkspaceSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListWorkspaces(
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var result = await _adminWorkspaceRepository.GetWorkspacesAsync(
                status, new PaginationQuery { Page = page, Limit = limit });
            return this.ToActionResult(result);
        }

        /// <summary>Get full details + stats for one workspace.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(WorkspaceSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWorkspaceById(Guid id)
        {
            var result = await _adminWorkspaceRepository.GetWorkspaceAsync(id);
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Activate or suspend a workspace.
        /// Activating resets the 30-day suspension clock.
        /// Only admins can reactivate a suspended workspace.
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        [Authorize(Policy = AdminPolicies.SuperAdminOnly)]
        [ProducesResponseType(typeof(WorkspaceSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateWorkspaceStatusDto dto)
        {
            var result = await _adminWorkspaceRepository.UpdateWorkspaceStatusAsync(id, dto);
            return this.ToActionResult(result);
        }
        /// <summary>System-wide stats: workspaces, users, orders, revenue.</summary>
        [HttpGet("system-stats")]
        [ProducesResponseType(typeof(SystemStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSystemStats()
        {
            var result = await _adminWorkspaceRepository.GetSystemStatsAsync();
            return this.ToActionResult(result);
        }
        /// <summary>
        /// Full audit trail. Filter by entity type and/or entity id.
        /// Returns newest entries first. SuperAdmin only.
        /// </summary>
        [HttpGet("audit-logs")]
        [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAuditLogs(
            [FromQuery] Guid? entityId = null,
            [FromQuery] string? entityType = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var result = await _adminWorkspaceRepository.GetAuditLogsAsync(
                entityId, entityType,
                new PaginationQuery { Page = page, Limit = limit });
            return this.ToActionResult(result);
        }
    }
}

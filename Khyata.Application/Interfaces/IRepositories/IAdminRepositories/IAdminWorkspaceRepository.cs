using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.DTOs.Admin.Workspace_User;
using Khyata.Application.DTOs.Admin.WorkspaceUser;
using Khyata.Domain.Entities;
using Khyata.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.IAdminRepositories
{
    public interface IAdminWorkspaceRepository
    { 
        // ── Workspace user password reset (admin resets owner/employee passwords) ─
        Task<Result> ResetWorkspaceUserPasswordAsync(Guid userId, ResetWorkspaceUserPasswordDto dto);
        Task<Result<PagedResult<WorkspaceSummaryDto>>> GetWorkspacesAsync(string? status, PaginationQuery query);
        Task<Result<WorkspaceSummaryDto>> GetWorkspaceAsync(Guid id);
        Task<Result<WorkspaceSummaryDto>> UpdateWorkspaceStatusAsync(Guid id, UpdateWorkspaceStatusDto dto);
        Task<Result<PagedResult<SystemUserDto>>> GetAllUsersAsync(Guid? workspaceId, string? role, bool includeDeleted, PaginationQuery query);
        Task<Result<SystemUserDto>> GetUserAsync(Guid userId);
        Task<Result<SystemStatsDto>> GetSystemStatsAsync();
    }
}

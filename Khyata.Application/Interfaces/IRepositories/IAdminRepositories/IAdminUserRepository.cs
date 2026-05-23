using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Admin_User;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Role;
using Khyata.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.IAdminRepositories
{
    public interface IAdminUserRepository
    {
        Task<Result<AdminProfileDto>> GetAdminByIdAsync(Guid adminId);
        Task<Result<AdminProfileDto>> ToggleActiveAsync(Guid adminId, ToggleAdminActiveDto dto);
        Task<Result<AdminProfileDto>> UpdateAsync(Guid adminId, UpdateAdminDto dto);
        Task<Result<PagedResult<AdminProfileDto>>> ListAdminsAsync(PaginationQuery query);
        Task<Result> DeleteAdminAsync(Guid adminId);
        Task<Result> ChangePasswordAsync(Guid adminId, ChangeAdminPasswordDto dto);

        Task<Result> AssignRoleAsync(Guid adminId, string role);
        Task<Result> RemoveRoleAsync(Guid adminId, string role);
        Task<Result<RoleDto[]>> GetRolesAsync();
    }
}

using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Admin_User;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Role;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
using Khyata.Infrastructure.Helpers;
using Khyata.Shared.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Repositories.AdminRepositories
{

    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IAdminProfileBuilder _profileBuilder;

        public AdminUserRepository(UserManager<AdminUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager, IAdminProfileBuilder profileBuilder)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _profileBuilder = profileBuilder;
        }

        public async Task<Result<AdminProfileDto>> GetAdminByIdAsync(Guid adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null)
                return Result<AdminProfileDto>.Failure(ApiError.NotFound("Admin user not found."));

            return Result<AdminProfileDto>.Success(await _profileBuilder.BuildAsync(user));
        }

        public async Task<Result<PagedResult<AdminProfileDto>>> ListAdminsAsync(PaginationQuery query)
        {
            var q = _userManager.Users.OrderBy(u => u.UserName);

            var result = await PaginationHelper.ToPagedResultAsync(
                q,
                query.SafePage,
                query.SafeLimit,
                u => new AdminProfileDto
                {
                    Id = u.Id,
                    Username = u.UserName!,
                    DisplayName = u.DisplayName,
                    Email = u.Email ?? string.Empty,
                    Roles = _userManager.GetRolesAsync(u).Result.ToArray(),
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                });

            return Result<PagedResult<AdminProfileDto>>.Success(result);
        }
        public async Task<Result<AdminProfileDto>> ToggleActiveAsync( Guid adminId,ToggleAdminActiveDto dto)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());

            if (user is null)
                return Result<AdminProfileDto>.Failure(
                    ApiError.NotFound("Admin user not found."));

            user.IsActive = dto.IsActive;


            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Result<AdminProfileDto>.Failure(
                    ApiError.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description))));

            return Result<AdminProfileDto>.Success(
                await _profileBuilder.BuildAsync(user));
        }
        public async Task<Result<AdminProfileDto>> UpdateAsync(Guid adminId, UpdateAdminDto dto)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null)
                return Result<AdminProfileDto>.Failure(ApiError.NotFound("Admin user not found."));

            if (dto.DisplayName is not null) user.DisplayName = dto.DisplayName;
            if (dto.Email is not null) user.Email = dto.Email;
            //if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;


            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result<AdminProfileDto>.Failure(
                    ApiError.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description))));

            return Result<AdminProfileDto>.Success(await _profileBuilder.BuildAsync(user));
        }
        public async Task<Result> DeleteAdminAsync(Guid adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null)
                return Result.Failure(ApiError.NotFound("Admin user not found."));

            var allAdmins = await _userManager.GetUsersInRoleAsync(AdminRoles.SuperAdmin);
            if (allAdmins.Count == 1 && allAdmins.First().Id == adminId)
                return Result.Failure(
                    ApiError.Forbidden("Cannot delete the last SuperAdmin account."));

            await _userManager.DeleteAsync(user);
            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(Guid adminId, ChangeAdminPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null)
                return Result.Failure(ApiError.NotFound("Admin user not found."));

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return Result.Failure(
                    ApiError.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description))));

            return Result.Success();
        }
        public async Task<Result> AssignRoleAsync(Guid adminId, string role)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null) return Result.Failure(ApiError.NotFound("Admin user not found."));

            if (!await _roleManager.RoleExistsAsync(role))
                return Result.Failure(ApiError.BadRequest($"Role '{role}' does not exist."));

            if (await _userManager.IsInRoleAsync(user, role))
                return Result.Failure(ApiError.Conflict("User is already in this role."));

            await _userManager.AddToRoleAsync(user, role);
            return Result.Success();
        }

        public async Task<Result<RoleDto[]>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name! })
            .ToArrayAsync();
            return Result<RoleDto[]>.Success(roles);
        }

        public async Task<Result> RemoveRoleAsync(Guid adminId, string role)
        {
            var user = await _userManager.FindByIdAsync(adminId.ToString());
            if (user is null) return Result.Failure(ApiError.NotFound("Admin user not found."));

            if (!await _userManager.IsInRoleAsync(user, role))
                return Result.Failure(ApiError.BadRequest("User is not in this role."));

            await _userManager.RemoveFromRoleAsync(user, role);
            return Result.Success();
        }
    }
}

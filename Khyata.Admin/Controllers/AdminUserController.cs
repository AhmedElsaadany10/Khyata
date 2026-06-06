using Khyata.Application.DTOs.Admin.Admin_User;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Role;
using Khyata.Application.Extensions;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Domain.Enums;
using Khyata.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.Admin.Controllers
{
    [ApiController]
    [Route("admin/users")]
   // [Authorize(Policy = AdminPolicies.SuperAdminOnly)]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserRepository _adminUserRepository;

        public AdminUserController(IAdminUserRepository adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }
        /// <summary>Get a single admin by id.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(AdminProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _adminUserRepository.GetAdminByIdAsync(id);
            return this.ToActionResult(result);
        }
        /// <summary>List all admin users with their roles.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<AdminProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] int page = 1, [FromQuery] int limit = 20)
        {
            var result = await _adminUserRepository.ListAdminsAsync(
                new PaginationQuery { Page = page, Limit = limit });
            return this.ToActionResult(result);
        }
        /// <summary>Activate or deactivate an admin account.</summary>
        [HttpPatch("{id:guid}/active")]
        public async Task<IActionResult> ToggleActive(Guid id, [FromBody] ToggleAdminActiveDto dto)
        {
            var result = await _adminUserRepository.ToggleActiveAsync(id, dto);
            return this.ToActionResult(result);
        }
        /// <summary>Update an admin's display name, email, or active status.</summary>
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(AdminProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminDto dto)
        {
            var result = await _adminUserRepository.UpdateAsync(id, dto);
            return this.ToActionResult(result);
        }
        /// <summary>Force-reset any admin's password (no current password needed).</summary>
        [HttpPatch("{id:guid}/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePasswordPassword(Guid id, [FromBody] ChangeAdminPasswordDto dto)
        {
            var result = await _adminUserRepository.ChangePasswordAsync(id, dto);
            return this.ToActionResult(result);
        }
        /// <summary>Delete an admin account (cannot delete the last SuperAdmin).</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Prevent self-deletion
            if (User.GetAdminId() == id)
                return new ObjectResult(new { code = 400, status = "BadRequest", message = "You cannot delete your own account." }) { StatusCode = 400 };

            var result = await _adminUserRepository.DeleteAdminAsync(id);
            return this.ToActionResult(result);
        }
        /// <summary>Assign a role to an admin user.</summary>
        [HttpPost("{id:guid}/roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleDto dto)
        {
            var result = await _adminUserRepository.AssignRoleAsync(id, dto.Role);
            return this.ToActionResult(result);
        }

        /// <summary>Remove a role from an admin user.</summary>
        [HttpDelete("{id:guid}/roles/{role}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRole(Guid id, string role)
        {
            var result = await _adminUserRepository.RemoveRoleAsync(id, role);
            return this.ToActionResult(result);
        }

        /// <summary>List all available admin roles.</summary>
        [HttpGet("roles")]
        [ProducesResponseType(typeof(RoleDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _adminUserRepository.GetRolesAsync();
            return this.ToActionResult(result);
        }
    }
}

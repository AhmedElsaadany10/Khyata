using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.DTOs.Admin.Auth;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Khyata.Infrastructure.Repositories.AdminRepositories
{
    public class AdminAuthRepository : IAdminAuthRepository
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IAdminProfileBuilder _adminProfileBuilder;

        public AdminAuthRepository(UserManager<AdminUser> userManager, RoleManager<IdentityRole<Guid>> roleManager,
            ITokenService tokenService, IAdminProfileBuilder adminProfileBuilder)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _adminProfileBuilder = adminProfileBuilder;
        }

        public async Task<Result<AdminLoginResponseDto>> LoginAsync(AdminLoginRequestDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Result<AdminLoginResponseDto>.Failure(
                    ApiError.Unauthorized("Invalid username or password."));

            if (!user.IsActive)
                return Result<AdminLoginResponseDto>.Failure(
                    ApiError.Forbidden("This admin account has been deactivated."));
            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            var token = _tokenService.GenerateAdminToken(user, roles);

            return Result<AdminLoginResponseDto>.Success(new AdminLoginResponseDto
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = 3600, // 1 hour
                Profile = await _adminProfileBuilder.BuildAsync(user)
            });
               
        }

        public async Task<Result<AdminProfileDto>> RegisterAsync(RegisterAdminDto dto)
        {
            // Validate role exists before creating user
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return Result<AdminProfileDto>.Failure(
                    ApiError.BadRequest($"Role '{dto.Role}' does not exist. Valid roles: SuperAdmin, Moderator."));
            var existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser is not null)
                return Result<AdminProfileDto>.Failure(
                    ApiError.Conflict("An admin with this username already exists."));
            var user = new AdminUser
            {
                UserName = dto.Username,
                DisplayName = dto.DisplayName,
                Email = dto.Email,
                IsActive = false, // New admins are inactive by default until approved by a SuperAdmin
            };
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return Result<AdminProfileDto>.Failure(
                    ApiError.BadRequest(string.Join("; ", createResult.Errors.Select(e => e.Description))));
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // rollback
                return Result<AdminProfileDto>.Failure(
                    ApiError.Internal("Failed to assign role to admin user."));
            }
            return Result<AdminProfileDto>.Success(await _adminProfileBuilder.BuildAsync(user));
        }
        
    }
}

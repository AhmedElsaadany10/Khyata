using Khyata.Application.DTOs.Admin.AdminUser;
using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Services
{
    public class AdminProfileBuilder : IAdminProfileBuilder
    {
        private readonly UserManager<AdminUser> _userManager;

        public AdminProfileBuilder(UserManager<AdminUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AdminProfileDto> BuildAsync(AdminUser user)
        {
            var roles = (await _userManager.GetRolesAsync(user)).ToArray();

            return new AdminProfileDto
            {
                Id = user.Id,
                Username = user.UserName!,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}

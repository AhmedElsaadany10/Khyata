using Khyata.Application.Helpers;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.DataSeeding
{
    public static class AdminSeeding
    {
        public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager, 
            UserManager<AdminUser> userManager, IConfiguration configuration, ILogger logger)
        {
            // ── 1. Seed roles ─────────────────────────────────────────────────────
            foreach (var roleName in AdminRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });

                    if (result.Succeeded)
                        logger.LogInformation("Created admin role: {Role}", roleName);
                    else
                        logger.LogError("Failed to create role {Role}: {Errors}",
                            roleName,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // ── 2. Seed default SuperAdmin ────────────────────────────────────────
            // Override these via appsettings or environment variables in production.
            var existing = await userManager.FindByNameAsync("admin");
            if (existing is null)
            {
                var superAdmin = new AdminUser
                {
                    UserName = "admin",
                    Email = "admin@khayata.com",
                    DisplayName = "Super Admin",
                    IsActive = true,
                    EmailConfirmed = true
                };
                var Password = "Admin@123456!";
                var createResult = await userManager.CreateAsync(superAdmin, Password);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdmin, AdminRoles.SuperAdmin);
                    logger.LogInformation(
                        "Default SuperAdmin seeded. Username: '{Username}'. " +
                        "CHANGE THIS PASSWORD IMMEDIATELY IN PRODUCTION.",superAdmin.UserName);
                }
                else
                {
                    logger.LogError("Failed to seed SuperAdmin: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
        }

    }
}

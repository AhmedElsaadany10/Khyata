using Khyata.Application.Helpers;
using Khyata.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Services
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAdminAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Any authenticated admin (SuperAdmin or Moderator)
            options.AddPolicy(AdminPolicies.AnyAdmin, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("type", "admin"));

            // SuperAdmin only — for destructive or sensitive operations
            options.AddPolicy(AdminPolicies.SuperAdminOnly, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("type", "admin")
                      .RequireRole(AdminRoles.SuperAdmin));

            // Moderator or higher
            options.AddPolicy(AdminPolicies.ModeratorOrAbove, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("type", "admin")
                      .RequireRole(AdminRoles.Moderator, AdminRoles.SuperAdmin));
        });

        return services;
    }
        public static IServiceCollection AddWorkspaceAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Any authenticated workspace user
                options.AddPolicy(WorkspacePolicies.Employee, policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("type", "workspace"));

                // Owner only — for destructive or sensitive operations
                options.AddPolicy(WorkspacePolicies.OwnerOnly, policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("type", "workspace")
                          .RequireClaim("workspace_role", WorkspaceRole.Owner.ToString()));

            });

            return services;
        }
    }
}

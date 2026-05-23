using Khyata.Application.Common;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Application.Interfaces.IServices;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.BackgroundServices;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Repositories.AdminRepositories;
using Khyata.Infrastructure.Repositories.SystemRepositories;
using Khyata.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    { /// <summary>
      /// Registers everything needed by Khayata.Api (main workspace API).
      /// Does NOT register Identity — that is only needed by Khayata.Admin.
      /// </summary>
        public static IServiceCollection AddMainInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
        {
            // AppDbContext (workspace data)
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // ===================== Repositories =====================
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Token service (workspace JWT)
            services.AddScoped<ITokenService, TokenService>();

            // Background service — suspension timer
            services.AddHostedService<WorkspaceSuspensionService>();

            return services;
        }
      
        /// <summary>
        /// Registers everything needed by Khayata.Admin.
        /// Includes ASP.NET Core Identity, AdminIdentityDbContext, and admin services.
        /// </summary>
        public static IServiceCollection AddAdminInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Workspace AppDbContext (needed for cross-context queries, e.g. resetting workspace user passwords)
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Identity DbContext (admin users, roles)
            services.AddDbContext<AdminDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // ASP.NET Core Identity
            services.AddIdentity<AdminUser, IdentityRole<Guid>>(opt =>
            {
                // Password policy
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = true;

                // Lockout
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                // User
                opt.User.RequireUniqueEmail = false; // username is the unique identifier
            })
            .AddEntityFrameworkStores<AdminDbContext>()
            .AddDefaultTokenProviders();

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // ===================== Repositories =====================
            services.AddScoped<IAdminProfileBuilder, AdminProfileBuilder>();
            services.AddScoped<IAdminAuthRepository, AdminAuthRepository>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            services.AddScoped<IAdminWorkspaceRepository, AdminWorkspaceRepository>();

            // Token service (workspace JWT)
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}

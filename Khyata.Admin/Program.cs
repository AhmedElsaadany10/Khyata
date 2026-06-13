using Khyata.Application.Common;
using Khyata.Application.Interfaces.IServices;
using Khyata.Application.Middlewares;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.DataSeeding;
using Khyata.Infrastructure.Extensions;
using Khyata.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Khyata.Admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===================== AddCors =====================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "https://tailoring-97.vercel.app")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // ===================== DB =====================
            // ===================== DI =====================
            // ===================== AutoMapper =============
            builder.Services.AddAdminInfrastructure(builder.Configuration);


            builder.Services.AddControllers();

            // ===================== JWT =====================
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAdminAuthorization();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

         
            var app = builder.Build();

            //  Migrate + seed roles + seed default SuperAdmin =====================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                // Run Identity migrations (AdminUsers, AdminRoles, etc.)
                var identityDb = services.GetRequiredService<AdminDbContext>();
                await identityDb.Database.MigrateAsync();

                // Seed roles and default SuperAdmin account
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = services.GetRequiredService<UserManager<AdminUser>>();
                await AdminSeeding.SeedAsync(roleManager, userManager, builder.Configuration, logger);
            }

            //  Middleware pipeline 
            app.UseMiddleware<ExceptionMiddleware>();   // global exception handler — must be first

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Khayata Admin v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();   
            app.UseCors("AllowAngular");
            // app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins());
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
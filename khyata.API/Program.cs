using khyata.Infrastructure.Persistence;
using khyata.Application.Interfaces.Repositories;
using khyata.Application.Interfaces.Services;
using khyata.Application.Middlewares;
using khyata.Infrastructure.Repositories;
using khyata.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Khyata.Application.Common;

namespace khyata.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ===================== DB =====================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            // ===================== JWT =====================
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthorization();

            // Add services to the container.
            // ===================== DI =====================
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var firstError = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault() ?? "Invalid request.";

                    var apiError = ApiError.BadRequest(firstError);

                    return new ObjectResult(apiError)
                    {
                        StatusCode = apiError.Code
                    };
                };
            });            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Global exception middleware should be first so it can catch exceptions from everything
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Authentication must be enabled before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
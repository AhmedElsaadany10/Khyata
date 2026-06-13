using Khyata.Application.Common;
using Khyata.Application.Interfaces.IRepositories;
using Khyata.Application.Interfaces.IServices;
using Khyata.Application.Middlewares;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Extensions;
using Khyata.Infrastructure.Repositories;
using Khyata.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Khyata.API
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddMainInfrastructure(builder.Configuration);


            // ===================== JWT =====================
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();
            builder.Services.AddWorkspaceAuthorization();


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
            app.UseCors("AllowAngular");
            // Authentication must be enabled before Authorization
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuditMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
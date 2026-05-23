using Khyata.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using static Khyata.Application.Exceptions.ExceptionError;

namespace Khyata.Application.Middlewares
{
    /// <summary>
    /// Catches all unhandled exceptions and maps them to the standard ApiError shape:
    ///   { code, status, message }
    /// Domain exceptions (NotFoundException, ForbiddenException, etc.) are mapped
    /// to their specific HTTP codes.  All other exceptions become 500 Internal Server Error.
    /// Stack traces are only included in Development.
    /// </summary>
    public  class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleAsync(context, ex);
            }
        }

        private async Task HandleAsync(HttpContext context, Exception ex)
        {
            var (statusCode, apiError) = ex switch
            {
                NotFoundException nfe => (HttpStatusCode.NotFound, ApiError.NotFound(nfe.Message)),
                ConflictException cfe => (HttpStatusCode.Conflict, ApiError.Conflict(cfe.Message)),
                ForbiddenException ffe => (HttpStatusCode.Forbidden, ApiError.Forbidden(ffe.Message)),
                ValidationException vfe => (HttpStatusCode.BadRequest, ApiError.BadRequest(vfe.Message)),
                BusinessRuleException bre => (HttpStatusCode.UnprocessableEntity, ApiError.UnprocessableEntity(bre.Message)),
                UnauthorizedException uae => (HttpStatusCode.Unauthorized, ApiError.Unauthorized(uae.Message)),
                _ => (HttpStatusCode.InternalServerError, ApiError.Internal(
                                                  env.IsDevelopment() ? ex.Message : "An unexpected error occurred."))
            };

            // Only log unexpected errors as errors; domain errors as warnings
            if (statusCode == HttpStatusCode.InternalServerError)
                logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            else
                logger.LogWarning("Domain exception on {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, ex.Message);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(apiError, JsonOptions));
        }
    }
}
using Khyata.Domain.Entities;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace Khyata.Application.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditRepo)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.StartsWith("/health") || path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            // ===================== REQUEST =====================
            context.Request.EnableBuffering();
            var requestBodyRaw = await ReadBodyAsync(context.Request.Body);
            context.Request.Body.Position = 0;

            // ===================== RESPONSE =====================
            var originalResponseBody = context.Response.Body;
            await using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            buffer.Position = 0;
            var responseRaw = await new StreamReader(buffer).ReadToEndAsync();

            // return response to client first (IMPORTANT)
            buffer.Position = 0;
            await buffer.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            // ===================== SANITIZE =====================
            var safeRequest = Sanitize(requestBodyRaw);
            var safeResponse = Sanitize(responseRaw);

            // ===================== USER =====================
            var actorIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var actorName =
                context.User?.FindFirst(ClaimTypes.Name)?.Value ??
                context.User?.FindFirst("name")?.Value;

            Guid.TryParse(actorIdClaim, out var actorId);

            // ===================== LOG =====================
            var entry = new AuditLog
            {
                ActorId = actorId == Guid.Empty ? null : actorId,
                ActorName = actorName,
                Action = ResolveAction(context.Request.Method, path),
                HttpMethod = context.Request.Method,
                Endpoint = path,
                RequestBody = Truncate(safeRequest),
                ResponseBody = Truncate(safeResponse),
                StatusCode = context.Response.StatusCode,
                ExecutionTimeMs = sw.ElapsedMilliseconds,
                Timestamp = DateTime.UtcNow
            };

            await auditRepo.LogAsync(entry);
        }

        // ===================== READ BODY =====================
        private static async Task<string> ReadBodyAsync(Stream body)
        {
            using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        // ===================== SAFE SANITIZE =====================
        private static string Sanitize(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return string.Empty;

            try
            {
                var node = JsonNode.Parse(body);
                if (node == null)
                    return body;

                var cleaned = CleanNode(node);
                return cleaned?.ToJsonString() ?? body;
            }
            catch
            {
                return body;
            }
        }

        // ===================== IMMUTABLE CLEANING =====================
        private static JsonNode? CleanNode(JsonNode? node)
        {
            if (node is JsonObject obj)
            {
                var newObj = new JsonObject();

                foreach (var kv in obj)
                {
                    var key = kv.Key.ToLower();

                    if (IsSensitiveKey(key))
                    {
                        newObj[kv.Key] = "***";
                    }
                    else
                    {
                        newObj[kv.Key] = CleanNode(kv.Value);
                    }
                }

                return newObj;
            }

            if (node is JsonArray arr)
            {
                var newArr = new JsonArray();

                foreach (var item in arr)
                {
                    newArr.Add(CleanNode(item));
                }

                return newArr;
            }

            return node;
        }

        // ===================== SENSITIVE KEYS =====================
        private static bool IsSensitiveKey(string key)
        {
            return key.Contains("password")
                || key.Contains("token")
                || key.Contains("secret")
                || key.Contains("authorization")
                || key.Contains("access");
        }

        // ===================== TRUNCATE =====================
        private static string Truncate(string body, int maxLength = 2000)
        {
            if (string.IsNullOrEmpty(body))
                return string.Empty;

            return body.Length > maxLength
                ? body[..maxLength] + "..."
                : body;
        }

        // ===================== ACTION RESOLVER =====================
        private static string ResolveAction(string method, string path)
        {
            return (method.ToUpper(), path) switch
            {
                ("POST", var p) when p.Contains("employees") => "Create Employee",
                ("PATCH", var p) when p.Contains("employees") => "Update Employee",
                ("DELETE", var p) when p.Contains("employees") => "Delete Employee",

                ("POST", var p) when p.Contains("orders") => "Create Order",
                ("PATCH", var p) when p.Contains("orders") => "Update Order",

                ("POST", var p) when p.Contains("customers") => "Create Customer",
                ("PATCH", var p) when p.Contains("customers") => "Update Customer",
                ("DELETE", var p) when p.Contains("customers") => "Delete Customer",

                ("POST", var p) when p.Contains("payments") => "Add Payment",

                ("POST", var p) when p.Contains("auth/login") => "Login",
                ("POST", var p) when p.Contains("auth/register") => "Register",

                _ => $"{method} {path}"
            };
        }
    }
}
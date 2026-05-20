namespace Khyata.Application.Common
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Unified API error response
    // Matches the shape requested:  { code, status, message }
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// The single error shape returned by every endpoint, including the global
    /// exception middleware.  Never expose stack traces in production.
    /// </summary>
    public  class ApiError
    {
        public int Code { get; init; }
        public string Status { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;

        public static ApiError NotFound(string message) => new() { Code = 404, Status = "NotFound", Message = message };
        public static ApiError BadRequest(string message) => new() { Code = 400, Status = "BadRequest", Message = message };
        public static ApiError Conflict(string message) => new() { Code = 409, Status = "Conflict", Message = message };
        public static ApiError Forbidden(string message) => new() { Code = 403, Status = "Forbidden", Message = message };
        public static ApiError Unauthorized(string message) => new() { Code = 401, Status = "Unauthorized", Message = message };
        public static ApiError UnprocessableEntity(string message) => new() { Code = 422, Status = "UnprocessableEntity", Message = message };
        public static ApiError Internal(string message) => new() { Code = 500, Status = "InternalServerError", Message = message };
    }
}

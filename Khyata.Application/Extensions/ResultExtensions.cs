using Khyata.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace khyata.Application.Extensions
{
    public static class ResultExtensions
    {/// <summary>
     /// Converts a Result{T} into the correct HTTP response.
     /// Maps ApiError.Code directly to the HTTP status code so there is one
     /// single place that owns the code→status mapping.
     /// </summary>
        public static IActionResult ToActionResult<T>(
            this ControllerBase controller,
            Result<T> result,
            int successStatusCode = 200)
        {
            if (result.IsSuccess)
                return new ObjectResult(result.Value) { StatusCode = successStatusCode };

            return MapError(controller, result.Error!);
        }

        public static IActionResult ToActionResult(
            this ControllerBase controller,
            Result result)
        {
            return result.IsSuccess
                ? controller.NoContent()
                : MapError(controller, result.Error!);
        }

        private static IActionResult MapError(ControllerBase controller, ApiError error) =>
            new ObjectResult(error) { StatusCode = error.Code };
    }
}

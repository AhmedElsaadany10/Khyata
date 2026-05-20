namespace Khyata.Application.Common
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Result<T> — railway-oriented, no exceptions for expected failures
    // ─────────────────────────────────────────────────────────────────────────────

    public  class Result<T>
    {
        public T? Value { get; }
        public ApiError? Error { get; }
        public bool IsSuccess => Error is null;

        private Result(T? value, ApiError? error)
        {
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(value, null);
        public static Result<T> Failure(ApiError error) => new(default, error);

        public Result<TOut> Map<TOut>(Func<T, TOut> projection) =>
            IsSuccess ? Result<TOut>.Success(projection(Value!)) : Result<TOut>.Failure(Error!);
    }

    // Non-generic convenience for void operations
    public  class Result
    {
        public ApiError? Error { get; }
        public bool IsSuccess => Error is null;

        private Result(ApiError? error) => Error = error;

        public static Result Success() => new(null);
        public static Result Failure(ApiError error) => new(error);
    }
}

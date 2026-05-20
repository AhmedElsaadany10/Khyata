namespace Khyata.Application.Exceptions
{
    public class ExceptionError
    {
        /// <summary>Base for all domain exceptions. The global middleware maps these to HTTP responses.</summary>
        public abstract class BaseException(string message) : Exception(message);

        public  class NotFoundException(string entity, object key)
            : BaseException($"{entity} with id '{key}' was not found.")
        { }

        public  class ConflictException(string message) : BaseException(message) { }

        public  class ForbiddenException(string message) : BaseException(message) { }

        public  class ValidationException(string message) : BaseException(message) { }

        public  class BusinessRuleException(string message) : BaseException(message) { }

        public  class UnauthorizedException(string message) : BaseException(message) { }
    }
}

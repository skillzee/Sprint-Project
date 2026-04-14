using System.Net;

namespace TravelApp.Shared.Exceptions;

/// <summary>
/// Base exception for application-specific errors that can be handled by the global exception handler
/// to return specific HTTP status codes.
/// </summary>
public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    protected AppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404).
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
}

/// <summary>
/// Exception thrown when a user is not authorized to perform an action (HTTP 401).
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized) { }
}

/// <summary>
/// Exception thrown when a request is invalid or violates business rules (HTTP 400).
/// </summary>
public class ValidationException : AppException
{
    public ValidationException(string message) : base(message, HttpStatusCode.BadRequest) { }
}

/// <summary>
/// Exception thrown when there is a conflict with the current state of the resource (HTTP 409).
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, HttpStatusCode.Conflict) { }
}

using System.Net;


namespace WebApi.Models;

public class ErrorModel
{
    public string Message { get; }
    public HttpStatusCode Status { get; }
    public Exception? Exception { get; } = null;
    public int StatusCode => (int)Status;

    private ErrorModel(string message, HttpStatusCode status, Exception? exception = null)
    {
        Message = message;
        Status = status;
        Exception = exception;
    }

    public static ErrorModel NotFound(string apiSlug) => new($"No API found with the slug `{apiSlug}`.", HttpStatusCode.NotFound);
    public static ErrorModel BadRequest(string message) => new(message, HttpStatusCode.BadRequest);
    public static ErrorModel Unauthorized() => new("Invalid credentials.", HttpStatusCode.Unauthorized);
    public static ErrorModel Forbidden() => new("You do not have access to this API.", HttpStatusCode.Forbidden);
    public static ErrorModel Timeout() => new("The query took more than the timeout to execute.", HttpStatusCode.RequestTimeout);
    public static ErrorModel MethodNotAllowed() => new("The API does not support this HTTP verb.", HttpStatusCode.MethodNotAllowed);
    public static ErrorModel InternalServerError(string? message, Exception? ex = null) => new(message ?? "Oops! Something went wrong on our side...", HttpStatusCode.InternalServerError, ex);
    public static ErrorModel UnprocessableEntity(string? message = null) => new(message ?? "Payload data is invalid.", HttpStatusCode.UnprocessableEntity);
}
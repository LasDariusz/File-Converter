using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Api.Shared.Application.Exceptions;

namespace Api.GlobalMiddlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception caught by global handler");

        var (statusCode, message) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetials = new ProblemDetails
        {
            Status = statusCode,
            Title = message
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        { 
            HttpContext = httpContext,
            ProblemDetails = problemDetials
        });
    }

    private static (int StatusCode, string Message) MapException(Exception ex) => ex switch
    {
        AppException appException =>
            (MapStatusCode(appException.ErrorType), appException.Message),

        _ => (StatusCodes.Status500InternalServerError, "An unexpected error has occured")
    };

    private static int MapStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

}
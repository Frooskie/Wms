using System.Net;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Exceptions;

namespace Wms.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            BusinessRuleException or ModelValidationException => HttpStatusCode.BadRequest,
            ForbiddenAccessException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        
        if (exception is ModelValidationException)
            logger.LogWarning(exception, "Model validation failed: {Message}", exception.Message);
        else
            logger.LogError(exception, "An unhandled exception occurred.");
        
        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = exception.Message,
            Status = (int)statusCode,
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };
        
        if (exception is ModelValidationException validationEx && validationEx.Errors.Any())
        {
            problemDetails.Extensions["errors"] = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        // Детали для внутренних ошибок (только в dev)
        if (statusCode == HttpStatusCode.InternalServerError && exception.InnerException != null)
        {
            problemDetails.Detail = exception.InnerException.Message;
        }

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
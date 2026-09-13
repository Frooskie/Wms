using System.Net;
using Microsoft.AspNetCore.Mvc;
using Wms.Core.Exceptions;

namespace Wms.API.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger, 
    IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(ex, "Response has already started, cannot write error details.");
                throw;
            }
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
            UnauthorizedException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        
        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            logger.LogWarning(exception, "Client error ({Status}): {Message}", (int)statusCode, exception.Message);
        
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
        
        problemDetails.Extensions["code"] = exception is BaseException baseEx
            ? baseEx.Code
            : "INTERNAL_ERROR";
        
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
        if (statusCode == HttpStatusCode.InternalServerError 
            && exception.InnerException != null 
            && env.IsDevelopment())
        {
            problemDetails.Detail = exception.InnerException.Message;
        }

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
using System.Net;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wms.API.DTOs.Common;
using Wms.Core.Constants;
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

            ex = TranslateDbException(ex);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Exception TranslateDbException(Exception ex)
    {
        if (ex is DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.RestrictViolation or PostgresErrorCodes.ForeignKeyViolation } })
        {
            return new BusinessRuleException(
                ErrorMessages.Common.CannotDeleteReferenced,
                ErrorCodes.CannotDeleteReferenced);
        }

        return ex;
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
        
        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        else
            logger.LogWarning(exception, "Client error ({Status}): {Message}", (int)statusCode, exception.Message);
        
        var problem = new WmsProblemDetails
        {
            Type = exception.GetType().Name,
            Title = exception.Message,
            Status = (int)statusCode,
            Instance = context.Request.Path,
            Code = (exception as BaseException)?.Code ?? ErrorCodes.InternalError,
            TraceId = context.TraceIdentifier,
            Errors = BuildErrors(exception)
        };
        
        if (statusCode == HttpStatusCode.InternalServerError
            && exception.InnerException != null
            && env.IsDevelopment())
        {
            problem.Detail = exception.InnerException.Message;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(problem);
    }

    private static IDictionary<string, string[]>? BuildErrors(Exception exception)
    {
        if (exception is not ModelValidationException mve || !mve.Errors.Any())
            return null;

        return mve.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
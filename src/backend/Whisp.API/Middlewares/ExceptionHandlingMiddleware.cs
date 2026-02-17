using System.Net;
using Microsoft.AspNetCore.Mvc;
using Whisp.Domain.Exceptions;

namespace Whisp.API.Middlewares;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ConversationNotFoundException ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Not Found",
                Detail = ex.Message
            },
            UnauthorizedMemberException ex => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Forbidden,
                Title = "Forbidden",
                Detail = ex.Message
            },
            _ => CreateInternalServerError(exception)
        };

        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private ProblemDetails CreateInternalServerError(Exception exception)
    {
        logger.LogError(exception, "An unhandled exception occurred");

        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred."
        };
    }
}

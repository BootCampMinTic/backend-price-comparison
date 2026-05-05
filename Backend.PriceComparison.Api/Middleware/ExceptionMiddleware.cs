using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Backend.PriceComparison.Application.Common.Constants;

namespace Backend.PriceComparison.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred during the request: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblemDetails(validationException),
            _ => CreateInternalServerErrorProblemDetails()
        };

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateValidationProblemDetails(ValidationException validationException)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Type = TypeExeptionConstants.VALIDATION_ERROR,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred."
        };

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        problemDetails.Extensions.Add("errors", errors);

        return problemDetails;
    }

    private static ProblemDetails CreateInternalServerErrorProblemDetails()
    {
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = TypeExeptionConstants.INTERNAL_SERVER_ERROR,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred while processing your request."
        };
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}

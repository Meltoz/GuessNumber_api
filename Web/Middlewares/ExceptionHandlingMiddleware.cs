using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Web.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Entity Not Found: {Message}", ex.Message);
                await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Entity not found", ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                await WriteProblemDetails(context, StatusCodes.Status400BadRequest, "Validation error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteProblemDetails(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}

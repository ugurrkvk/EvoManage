using EvoManage.Application.Common.Exceptions;
using EvoManage.Domain.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace EvoManage.API.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict"),

            DomainException => (
                StatusCodes.Status422UnprocessableEntity,
                "Domain validation error"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error")
        };

        var response = new ApiErrorResponse(
            statusCode,
            title,
            [exception.Message]);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}
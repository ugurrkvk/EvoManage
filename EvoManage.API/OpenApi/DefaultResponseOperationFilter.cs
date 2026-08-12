using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EvoManage.API.OpenApi;

public sealed class DefaultResponseOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        // Success response
        AddSuccessResponse(operation, context);

        // Her endpoint için ortak 500
        AddResponse(
            operation,
            StatusCodes.Status500InternalServerError,
            "Internal Server Error");

        // Endpoint'e özel hata response'ları
        var apiErrorsAttribute = context.MethodInfo
            .GetCustomAttributes(typeof(ApiErrorsAttribute), false)
            .Cast<ApiErrorsAttribute>()
            .FirstOrDefault();

        if (apiErrorsAttribute is null)
            return;

        foreach (var statusCode in apiErrorsAttribute.StatusCodes)
        {
            AddResponse(
                operation,
                statusCode,
                GetDescription(statusCode));
        }
    }

    private static void AddSuccessResponse(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        // Swagger'ın otomatik eklediği yanlış success response'ları temizle.
        var successResponses = operation.Responses.Keys
            .Where(key =>
                int.TryParse(key, out var statusCode) &&
                statusCode >= 200 &&
                statusCode < 300)
            .ToList();

        foreach (var response in successResponses)
            operation.Responses.Remove(response);

        var httpMethod = context.ApiDescription.HttpMethod;

        switch (httpMethod)
        {
            case "GET":
                AddResponse(
                    operation,
                    StatusCodes.Status200OK,
                    "OK");
                break;

            case "POST":
                AddResponse(
                    operation,
                    StatusCodes.Status201Created,
                    "Created");
                break;

            case "PUT":
            case "PATCH":
            case "DELETE":
                AddResponse(
                    operation,
                    StatusCodes.Status204NoContent,
                    "No Content");
                break;
        }
    }

    private static void AddResponse(
        OpenApiOperation operation,
        int statusCode,
        string description)
    {
        operation.Responses.TryAdd(
            statusCode.ToString(),
            new OpenApiResponse
            {
                Description = description
            });
    }

    private static string GetDescription(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest
                => "Bad Request",

            StatusCodes.Status404NotFound
                => "Not Found",

            StatusCodes.Status409Conflict
                => "Conflict",

            StatusCodes.Status422UnprocessableEntity
                => "Unprocessable Content",

            StatusCodes.Status500InternalServerError
                => "Internal Server Error",

            _ => "Error"
        };
    }
}
using EvoManage.API.ExceptionHandling;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EvoManage.API.Filters;

public sealed class ValidationFilter(IServiceProvider serviceProvider)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new List<string>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argument.GetType());

            if (serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);

            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                errors.AddRange(
                    validationResult.Errors.Select(error => error.ErrorMessage));
            }
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                new ApiErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "Validation error",
                    errors));

            return;
        }

        await next();
    }
}
namespace EvoManage.API.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ApiErrorsAttribute(params int[] statusCodes) : Attribute
{
    public IReadOnlyCollection<int> StatusCodes { get; } = statusCodes;
}
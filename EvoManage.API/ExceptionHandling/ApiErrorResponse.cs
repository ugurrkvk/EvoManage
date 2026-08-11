namespace EvoManage.API.ExceptionHandling;

public sealed record ApiErrorResponse(
    int Status,
    string Title,
    IReadOnlyCollection<string> Errors);
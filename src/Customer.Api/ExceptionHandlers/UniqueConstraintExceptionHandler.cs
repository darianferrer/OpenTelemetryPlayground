using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.ExceptionHandlers;

public class UniqueConstraintExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken cancellationToken)
    {
        if (ex is not UniqueConstraintException uniqueConstrainEx)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status409Conflict;
        var title = $"Unique constraint [{uniqueConstrainEx.ConstraintName}] violated";
        var detail = $"There's already an item in table [{uniqueConstrainEx.SchemaQualifiedTableName}] that have the same values in these properties: [{string.Join(", ", uniqueConstrainEx.ConstraintProperties)}]";
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Type = ex.GetType().Name,
                Status = context.Response.StatusCode,
            },
            cancellationToken);

        return true;
    }
}

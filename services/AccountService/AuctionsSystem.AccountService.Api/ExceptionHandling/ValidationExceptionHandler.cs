using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;



namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ValidationException validationException)
            {
                return false;
            }

            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(e => e.ErrorMessage).ToArray()
                );

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred."
            };

            problemDetails.Extensions.Add("errors", errors);

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

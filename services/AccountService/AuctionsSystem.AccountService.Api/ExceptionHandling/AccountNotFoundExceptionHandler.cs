using AuctionsSystem.AccountService.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class AccountNotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not AccountNotFoundException accountNotFoundException) 
            {
                return false;
            }

            var problemDetails = new ProblemDetails
            {
                Detail = exception.Message,
                Title = "Account Not Found",
                Status = StatusCodes.Status404NotFound
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

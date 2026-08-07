using AuctionsSystem.AccountService.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class TokenRevocationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not TokenRevocationException tokenException)
            {
                return false;
            }

            var problemDetails = new ProblemDetails
            {
                Detail = "User Logged out Successfully",
                Title = "Logout Accepted",
                Status = StatusCodes.Status202Accepted
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

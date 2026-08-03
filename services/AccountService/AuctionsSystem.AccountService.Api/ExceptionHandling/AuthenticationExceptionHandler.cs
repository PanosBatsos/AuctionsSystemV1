using AuctionsSystem.AccountService.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class AuthenticationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case InvalidCredentialsException:
                    return await WriteResponseAsync(httpContext, StatusCodes.Status401Unauthorized, exception.Message, cancellationToken);

                case LockedOutAccountException:
                case InactiveAccountException:
                case NotVerifiedAccountException:
                    return await WriteResponseAsync(httpContext, StatusCodes.Status403Forbidden, exception.Message, cancellationToken);

                default:
                    return false;
            }
        }

        private static async ValueTask<bool> WriteResponseAsync(HttpContext httpContext, int statusCode, string message, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "AuthenticationFailed",
                Detail = message
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

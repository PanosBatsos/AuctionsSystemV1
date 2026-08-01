using AuctionsSystem.AccountService.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class PropertyAlreadyInUseExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not PropertyAlreadyInUseException conflictException)
            {
                return false;
            }

          
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Resource Conflict",
                Detail = conflictException.Message
            };

           
            problemDetails.Extensions.Add("conflictField", conflictException.FieldName);

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            
            return true;
        }
    }
}

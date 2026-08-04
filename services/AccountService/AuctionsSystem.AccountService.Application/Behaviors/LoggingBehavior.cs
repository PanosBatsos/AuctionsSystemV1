using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            var maskedRequest = MaskSensitiveData(request);

            _logger.LogInformation(
                "[START] Executing request {RequestName} with payload: {@Request}",
                requestName,
                maskedRequest);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation(
                    "[END] Executing request {RequestName} finished successfully in {ElapsedMilliseconds} ms.",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                return response;
            } catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "[ERROR] Request {RequestName} failed after {ElapsedMilliseconds} ms.",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        private Dictionary<string, object?> MaskSensitiveData(TRequest request)
        {
            var maskedData = new Dictionary<string, object?>();
            PropertyInfo[] properties = typeof(TRequest).GetProperties();

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(request);


                if (propName.Contains("Password", StringComparison.OrdinalIgnoreCase))
                {
                    maskedData[propName] = "***MASKED***";
                }
                else
                {
                    maskedData[propName] = propValue;
                }
            }

            return maskedData;
        }
    }
}

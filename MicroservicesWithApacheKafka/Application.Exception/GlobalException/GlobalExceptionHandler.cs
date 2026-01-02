using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exception.GlobalException
{
    public class GlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly RequestDelegate _nextCall;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, RequestDelegate nextCall)
        {
            this._logger = logger;
            this._nextCall = nextCall;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _nextCall(httpContext);
            }
            catch (System.Exception exception)
            {
                ApplicationError applicationError = new()
                {
                    Id = Guid.NewGuid(),
                    When = DateTime.Now,
                    Message = exception.Message,
                };
                _logger.LogError(exception: exception, message: "Application Error", args: applicationError);

                await httpContext.Response.WriteAsJsonAsync(applicationError);
            }
        }
    }
}

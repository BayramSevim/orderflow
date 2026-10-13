using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Api.ErrorHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problem;

            if (exception is NotFoundException)
            {
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource not found",
                    Detail = exception.Message
                };
            }
            else if (exception is ConflictException)
            {
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = exception.Message
                };
            }
            else if (exception is DomainException)
            {
                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Business rule violation",
                    Detail = exception.Message
                };
            }
            else
            {
                _logger.LogError(exception, "Unhandled exception");

                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred"
                };
            }

            httpContext.Response.StatusCode = problem.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}
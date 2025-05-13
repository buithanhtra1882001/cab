using CabIdentityService.Infrastructures.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.Exceptions;

namespace CabIdentityService.Infrastructures.Middlewares
{
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AppException appEx)
            {
                _logger.LogInformation(appEx, appEx.Message);
                await HandleExceptionAsync(context, appEx);
            }
            catch (Exception e)
            {
                string message = e.Message;
                _logger.LogError(e, message);
                await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext httpContext,
            Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var response = new
            {
                title = GetTitle(exception),
                status = statusCode,
                detail = exception.Message,
                errors = GetErrors(exception),
                innerMessage = exception.InnerException?.Message,
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                ApiValidationException => StatusCodes.Status422UnprocessableEntity,
                AppException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

        private static string GetTitle(Exception exception)
        {
            var title = exception switch
            {
                AppException applicationException => Enum.GetName(applicationException.Error),
                _ => "Server Error"
            };

            return title ?? string.Empty;
        }

        private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
        {
            if (exception is ValidationException validationException)
                return validationException.ErrorsDictionary;

            return new Dictionary<string, string[]>();
        }
    }
}

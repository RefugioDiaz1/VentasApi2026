using Azure;
using System.Net;
using System.Text.Json;
using VentasApi2026.Common;
using VentasApi2026.Exceptions;

namespace VentasApi2026.Middleware
{
    public class ExceptionMiddleware
    {

//🧠 Qué hace esto

//Pipeline ASP.NET:

//Request → Middleware → Controller → Response

//Si algo explota:

//ExceptionMiddleware captura TODO

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
            this._next = next;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }

        }

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal server error";

            switch (exception)
            {

                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case ConflictException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Internal server error";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message

            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }


    }
}

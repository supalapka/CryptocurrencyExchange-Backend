using CryptocurrencyExchange.Exceptions;
using System.Net;
using System.Text.Json;

namespace CryptocurrencyExchange.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                UserNotFoundException => HttpStatusCode.NotFound,
                ApiKeyNotFoundException => HttpStatusCode.NotFound,
                WalletItemNotFoundException => HttpStatusCode.NotFound,
                StakingCoinNotFoundException => HttpStatusCode.NotFound,
                TransferNotFoundException => HttpStatusCode.NotFound,
                UserAlreadyExistsException => HttpStatusCode.Conflict,
                InvalidPasswordException => HttpStatusCode.Unauthorized,
                InvalidEmailException => HttpStatusCode.BadRequest,
                InvalidVerificationCodeException => HttpStatusCode.BadRequest,
                InsufficientFundsException => HttpStatusCode.UnprocessableEntity,
                SelfTransferException => HttpStatusCode.UnprocessableEntity,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled exception");

            var message = statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new { message }));
        }
    }
}

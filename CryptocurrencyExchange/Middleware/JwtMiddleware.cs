using System.Security.Claims;

namespace CryptocurrencyExchange.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                string userIdClaimValue = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdClaimValue);
                context.Items["UserId"] = userId;
            }

            await _next(context);
        }
    }

}

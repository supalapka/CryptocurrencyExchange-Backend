using CryptocurrencyExchange.Middleware;

namespace CryptocurrencyExchange.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication SetupWebPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<JwtMiddleware>();
            app.MapControllers();

            return app;
        }
    }
}

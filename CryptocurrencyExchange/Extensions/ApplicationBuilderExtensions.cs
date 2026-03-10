using Microsoft.AspNetCore.HttpOverrides;

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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCors();
            app.UseRateLimiter();
            app.UseOutputCache();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}

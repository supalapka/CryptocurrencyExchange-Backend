using CryptocurrencyExchange.Infrastructure.Logging;
using CryptocurrencyExchange.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace CryptocurrencyExchange.Extensions
{
    public static class LoggingCollectionExtensions
    {
        public static IServiceCollection AddDatabaseLogging(this IServiceCollection services)
        {
            services.AddSingleton<LogQueue>();
            services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
            services.AddHostedService<DatabaseLogWriterService>();
            return services;
        }

        public static IHostBuilder AddElasticLogging(
            this IHostBuilder hostBuilder,
            IConfiguration configuration)
        {
            hostBuilder.ConfigureServices((_, services) =>
            {
                services
                    .AddOptions<ElasticsearchOptions>()
                    .Bind(configuration.GetSection("Elasticsearch"))
                    .Validate(o => !string.IsNullOrWhiteSpace(o.Uri),
                              "Elasticsearch:Uri is required")
                    .Validate(o => Uri.TryCreate(o.Uri, UriKind.Absolute, out var uri),
                              "Elasticsearch:Uri must be a valid absolute URI")
                    .ValidateOnStart();
            });

            hostBuilder.UseSerilog(
                (hostContext, _, loggerConfig) =>
                {
                    var elasticOptions = hostContext.Configuration
                        .GetSection("Elasticsearch")
                        .Get<ElasticsearchOptions>()
                        ?? throw new InvalidOperationException(
                            "Elasticsearch configuration section is missing");

                    loggerConfig
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                            new Uri(elasticOptions.Uri))
                        {
                            IndexFormat = elasticOptions.IndexFormat,
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                            NumberOfShards = 1,
                            NumberOfReplicas = 0,
                            ModifyConnectionSettings = conn =>
                            {
                                if (!string.IsNullOrWhiteSpace(elasticOptions.Username))
                                    conn.BasicAuthentication(
                                        elasticOptions.Username,
                                        elasticOptions.Password);
                                return conn;
                            }
                        });
                },
                writeToProviders: true);

            return hostBuilder;
        }
    }
}

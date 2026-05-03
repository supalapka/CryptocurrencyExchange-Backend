using CryptocurrencyExchange.Extensions;
using CryptocurrencyExchange.NotificationService.Consumers;
using CryptocurrencyExchange.NotificationService.Interfaces;
using CryptocurrencyExchange.NotificationService.Outbox;
using CryptocurrencyExchange.NotificationService.Persistence;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .AddElasticLogging(configuration)
    .ConfigureServices((context, services) =>
    {
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(context.Configuration.GetSection("RabbitMq"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password))
            .ValidateOnStart();

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("NotificationDb")));

        services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
        services.AddScoped<INotificationOutboxRepository, NotificationOutboxRepository>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<TransferCompletedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var options = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(options.Host, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddHostedService<NotificationOutboxDispatcher>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await db.Database.MigrateAsync();
}

await host.RunAsync();

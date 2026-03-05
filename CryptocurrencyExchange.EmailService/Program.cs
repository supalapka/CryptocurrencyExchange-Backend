using CryptocurrencyExchange.EmailService.Interfaces;
using CryptocurrencyExchange.EmailService.Consumers;
using CryptocurrencyExchange.EmailService.Infrastructure;
using CryptocurrencyExchange.EmailService.Options;
using CryptocurrencyExchange.Options;
using MassTransit;
using Microsoft.Extensions.Options;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, _, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration)
              .Enrich.FromLogContext();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services
            .AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection("Smtp"))
            .Configure(o => o.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? o.Password)
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Smtp:Host is required")
            .Validate(o => o.Port > 0, "Smtp:Port must be positive")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "Smtp:Username is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "Smtp:Password is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.FromAddress), "Smtp:FromAddress is required")
            .ValidateOnStart();

        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection("RabbitMq"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password))
            .ValidateOnStart();

        services.AddSingleton<IEmailSender, MailKitEmailSender>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendVerificationEmailConsumer>();
            x.AddConsumer<SendWelcomeEmailConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var options = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(options.Host, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.ReceiveEndpoint("email.send-verification", e =>
                {
                    e.ConfigureConsumer<SendVerificationEmailConsumer>(ctx);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    })
    .Build();

await host.RunAsync();

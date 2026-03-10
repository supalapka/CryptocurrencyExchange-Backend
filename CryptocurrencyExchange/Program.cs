using CryptocurrencyExchange.Extensions;

var builder = WebApplication.CreateBuilder(args);

// infrastructure
builder.Logging.ClearProviders();
builder.Host.AddElasticLogging(builder.Configuration);

builder.Services.AddPersistenceInfrastructureServices(builder.Configuration);
builder.Services.AddExternalApiInfrastructureServices();
builder.Services.AddMessagingInfrastructure(builder.Configuration);
builder.Services.AddSecurityInfrastructure(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddBackgroundJobs();
builder.Services.AddDatabaseLogging();

// application
builder.Services.AddApplicationServices();
builder.Services.AddWebServices();

var app = builder.Build();
app.SetupWebPipeline();
app.Run();
using CryptocurrencyExchange.Extensions;

var builder = WebApplication.CreateBuilder(args);

// logging
builder.Logging.ClearProviders();
builder.Host.AddElasticLogging(builder.Configuration);
builder.Services.AddDatabaseLogging();

// persistence & external APIs
builder.Services.AddPersistenceInfrastructureServices(builder.Configuration);
builder.Services.AddExternalApiInfrastructureServices();

// messaging & background jobs
builder.Services.AddMessagingInfrastructure(builder.Configuration);
builder.Services.AddBackgroundJobs();

// security & rate limiting
builder.Services.AddSecurityInfrastructure(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);

// configuration
builder.Services.AddStakingPromotionOptions(builder.Configuration);
builder.Services.AddOutputCaching();

// application
builder.Services.AddApplicationServices();
builder.Services.AddWebServices();

var app = builder.Build();
app.SetupWebPipeline();
app.Run();

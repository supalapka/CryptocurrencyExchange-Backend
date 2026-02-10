using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Data.Intefraces;
using CryptocurrencyExchange.Middleware;
using CryptocurrencyExchange.Services;
using CryptocurrencyExchange.Services.Authorization;
using CryptocurrencyExchange.Services.Futures;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Shcedulers;
using CryptocurrencyExchange.Services.Wallet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddHttpClient<IMarketService, MarketService>(cient =>
{
    cient.BaseAddress = new Uri("https://api.binance.com/api/v3/");
});

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFuturesService, FuturesService>();
builder.Services.AddScoped<IStakingService, StakingService>();
builder.Services.AddScoped<IWalletItemRepository, WalletItemRepository>();
builder.Services.AddScoped<IWalletDomainService, WalletDomainService>();
builder.Services.AddScoped<IUnitOfWork, EfUniOfWork>();
builder.Services.AddScoped<IFutureRepository, EfFutureRepository>();
builder.Services.AddScoped<IFuturesDomainService, FuturesDomainService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthDomainService, AuthDomainService>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddHostedService<StakingScheduler>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.
        GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();

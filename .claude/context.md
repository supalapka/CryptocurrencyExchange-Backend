# Codebase Context

## Projects
- Solution: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange.sln
- CryptocurrencyExchange: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/CryptocurrencyExchange.csproj | net7.0 (Web)
- CryptocurrencyExchange.EmailService: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange.EmailService/CryptocurrencyExchange.EmailService.csproj | net7.0 (Worker)
- CryptocurrencyExchange.Tests: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange.Tests/CryptocurrencyExchange.Tests.csproj | net7.0

## Layer Paths
- Core: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core
- Application: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Application
- Infrastructure: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Infrastructure
- Presentation: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Presentation
- Tests: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange.Tests
- Options (config binding): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Options
- Extensions (DI registration): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Extensions
- Migrations: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Migrations

## Naming Conventions
- Namespaces: CryptocurrencyExchange.<Layer>[.<SubFolder>] — e.g. CryptocurrencyExchange.Application.Auth, CryptocurrencyExchange.Infrastructure.Persistence.Repositories
- Entities namespace: CryptocurrencyExchange.Core.Models (files live in Core/Entities/)
- Controllers: <Feature>Controller : ApiControllerBase — e.g. AuthController, WalletController
- Interfaces: I<Name> — repository interfaces in Core/Interfaces/Repositories/, service interfaces in Core/Interfaces/Services/, domain service interfaces in Core/Interfaces/
- Repositories (EF): Ef<Feature>Repository — e.g. EfUserRepository, EfFutureRepository (exception: WalletItemRepository)
- Application services: <Feature>Service — e.g. AuthService, WalletService
- Options classes: <Topic>Options — e.g. JwtOptions, RabbitMqOptions, ElasticsearchOptions
- Value objects: readonly record struct in Core/ValueObject/
- Test classes: <Subject>Tests with NUnit [TestFixture]

## Key Patterns
- Controller base class: ApiControllerBase (abstract, inherits ControllerBase, exposes UserId from JWT claim)
- Test framework: NUnit 3 with Moq for mocking; test runner: NUnit3TestAdapter + Microsoft.NET.Test.Sdk
- DI registration: split across static extension methods in Extensions/ — AddApplicationServices, AddPersistenceInfrastructureServices, AddExternalApiInfrastructureServices, AddMessagingInfrastructure, AddSecurityInfrastructure, AddRateLimiting, AddBackgroundJobs, AddDatabaseLogging, AddStakingPromotionOptions
- Options pattern: IOptions<T> with .AddOptions<T>().Bind(...).Validate(...).ValidateOnStart() — never inject IConfiguration into services
- Logging: Serilog via UseSerilog() + Elasticsearch sink configured in LoggingCollectionExtensions.AddElasticLogging(); secondary DatabaseLogger writes to DB via a background queue
- Messaging: MassTransit over RabbitMQ; consumers in Infrastructure/News/ and Infrastructure/Wallets/ (main project) and EmailService/Consumers/ (worker)
- EF DbContext: DataContext is confined to Infrastructure/Persistence; accessed only through repositories and IUnitOfWork
- Domain failures: domain-specific exceptions in Core/Exceptions/ — never null returns
- Value objects: readonly record struct with validation in constructor (Balance, CoinSymbol, Email, Password, VerificationCode, etc.)
- HTTP pipeline setup: ApplicationBuilderExtensions.SetupWebPipeline
- Rate limiting: fixed-window per IP, configured via RateLimitingOptions
- Migrations: generated via EF CLI (dotnet ef migrations add); files live in CryptocurrencyExchange/Migrations/

## Key Locations
- Controllers: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Presentation/Controllers
- Interfaces (repositories): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Interfaces/Repositories
- Interfaces (services): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Interfaces/Services
- Interfaces (domain services + other): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Interfaces
- Repositories: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Infrastructure/Persistence/Repositories
- Migrations: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Migrations
- Entities (files): D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Entities (namespace: CryptocurrencyExchange.Core.Models)
- Value objects: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/ValueObject
- Domain exceptions: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Exceptions
- Domain services: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/DomainServices
- Application services: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Application
- Options classes: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Options
- MassTransit events/commands: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Core/Events
- DataContext: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange/Infrastructure/Persistence/DataContext.cs
- EmailService consumers: D:/GitHub/CryptocurrencyExchange-Backend/CryptocurrencyExchange.EmailService/Consumers

## Key Packages
- Microsoft.EntityFrameworkCore.SqlServer 7.0.20: EF Core ORM with SQL Server provider
- Microsoft.AspNetCore.Authentication.JwtBearer 7.0.20: JWT bearer authentication
- Serilog.AspNetCore 6.1.0: structured logging pipeline
- Serilog.Sinks.Elasticsearch 9.0.3: ships logs to Elasticsearch / ELK stack
- MassTransit 8.2.5 + MassTransit.RabbitMQ 8.2.5: message bus (publish/consume over RabbitMQ)
- MailKit 4.3.0: SMTP email sending in EmailService worker
- Swashbuckle.AspNetCore 6.5.0: Swagger/OpenAPI docs
- NUnit 3.13.3 + NUnit3TestAdapter 4.5.0: test framework and runner
- Moq 4.18.4: mocking in unit tests
- Microsoft.EntityFrameworkCore.InMemory + Sqlite 7.0.20: in-process DB for tests
- System.IdentityModel.Tokens.Jwt 7.0.3: JWT token creation/validation

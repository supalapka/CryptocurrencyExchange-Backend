# Cryptocurrency Exchange 
This is a back-end part of the Cryptocurrency Exchange project.

Front-end parts:
- https://github.com/supalapka/CryptocurrencyExchange-Frontend
- https://github.com/supalapka/CryptocurrencyExchange-Frontend-React

External services connected via RabbitMQ:
- https://github.com/supalapka/Crawler — crawls and parses cryptocurrency news based on filters

## Overview

#### This project is a backend API for a cryptocurrency trading platform. It provides functionality for user authentication, wallet management, futures trading, staking, and integration with external market data providers.

The project is built as a modular monolith with a clear separation between HTTP API, business logic, data access, and infrastructure concerns.

---
## Core Features

- User authentication using JWT
- Wallets and balance management (demo account)
- Futures trading and trading history (demo account)
- Staking with scheduled reward (demo account)
- Notifications and news
- Integration with external market APIs (Binance)
- Asynchronous processing and service communication using **RabbitMQ**

---
## Architecture

The project follows a layered architecture:
- **Controllers** – HTTP endpoints and request/response handling
- **Core** – business logic and domain rules
- **Data** – Entity Framework Core, repositories, Unit of Work, migrations
- **Infrastructure** – external API integrations, security, background jobs

---
## Data & Persistence

- Entity Framework Core is used for database access
- Database schema is managed via migrations
- Unit of Work ensures transactional consistency across operations

---
## Security

- JWT-based authentication
- Custom middleware for token validation

---
## Background Jobs

The project includes scheduled background jobs used for staking reward calculation and distribution.

---
## How to Run

### Requirements:
- .NET 6
- RabbitMQ (used for crypto-news only; configured in `Program.cs`; requires a running external crawler service)
- Database connection string (configured via `appsettings.json`)

### Steps:
#### 1. Apply database migrations using `update-database` command in PM console
#### 2. Configure RabbitMQ host and virtual host in `appsettings.json`:
   ```json
   "RabbitMq": {
     "Host": "localhost",
     "VirtualHost": "/"
   }
   ```
   Configure RabbitMQ credentials using .NET user-secrets:

    dotnet user-secrets set "RabbitMq:Username" "YourUsername"
    dotnet user-secrets set "RabbitMq:Password" "YourPassword"

#### 3. Start the API

---
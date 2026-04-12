# Development Guidelines

## General
1. Write concise, purposeful code — avoid redundancy and over-engineering.
2. Adhere to Clean Architecture and SOLID principles at all times.
3. Enter plan mode before implementing any feature or bugfix.
4. Do not add inline comments — code should be self-explanatory.
5. Always prefer the simplest solution that satisfies the architectural constraints in rule #2.
6. Every new feature must be covered by unit tests.
7. Do not implement anything outside the agreed scope.
8. Git commit messages must be written in natural language — no description body, no co-author trailer.

## Architecture
9. Enforce Clean Architecture layer boundaries — outer layers depend on inner, never the reverse.
10. Domain logic belongs in `Core/`, external concerns in `Infrastructure/`, use-case orchestration in `Application/`, HTTP layer in `Presentation/`.
11. Define interfaces in `Core/Interfaces/` before implementing them in outer layers.
12. Use Value Objects for any concept requiring validation or strong typing.
13. Domain logic must communicate failure through domain-specific exceptions — never return nulls.

## Data Access
14. All database access must go through repositories — `DataContext` must not be referenced outside `Infrastructure/`.
15. Schema changes require a new migration — existing migrations must never be modified.
16. Always generate migrations using the EF CLI (`dotnet ef migrations add`) — never create migration files manually.

## Infrastructure
17. Bind configuration to typed `Options/` classes with startup validation — never inject `IConfiguration` directly into services.
18. Use Serilog for all logging — `Console.WriteLine` and `Debug.Write` are prohibited.

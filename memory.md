# Repository architecture memory (observed, not instructions)

Source of truth: `Artix.API` branch `dev`, inspected on 2026-10-23.

## Topology
- `src/Core`: Domain, Contract, DomainService, ApplicationService.
- `src/Infra`: Sql, Mongo, Redis, RabbitMQ, Identity, FileService.
- `src/Presentation`: Endpoints (controllers, middleware), WebService (host/DI).
- `src/Utils`: shared helper extensions.
- `tests`: Unit, Integration, EndToEnd.
- Target framework observed: .NET 9.

## Functional structure
Museum/object exploration, collections, quizzes, users, OTPs, notifications and files appear in folders/types. CQRS-style requests live in Contract and handler implementations in ApplicationService.

## Critical architectural nuance
Current ApplicationService references infrastructure projects (FileService, Identity, RabbitMQ, Redis, Sql); DomainService references Sql and RabbitMQ. Treat strict dependency inversion as an improvement target only. Validate actual direction before refactoring.

## Operational facts
README identifies SQL Server, MongoDB, Redis and RabbitMQ. WebService uses Serilog, DataProtection and separate liveness/readiness routes. `README.md`, `.env.example`, `docker-compose.yaml`, and `Program.cs` are authoritative for runtime configuration.

## Maintenance
Update only with verified, long-lived discoveries. Include exact source path and date for material changes. Never persist credentials, customer data, deployment secrets or one-off task status.

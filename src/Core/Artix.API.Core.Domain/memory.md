# Domain model — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Core/Artix.API.Core.Domain`

## Observed role
Entities, aggregate roots, domain events, invariants, and domain enums.

## Evidence / discovery anchors
Collection, Museum, MuseumObject, OTP, Notification, JournalEntry, UploadSession

## Architectural cautions
No HTTP, EF Core, Redis, RabbitMQ, Mongo, or service orchestration. Protect invariants inside domain behavior.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

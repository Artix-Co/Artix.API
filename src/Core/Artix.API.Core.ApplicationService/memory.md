# Application services — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Core/Artix.API.Core.ApplicationService`

## Observed role
Command/query handlers and use-case orchestration for admin/client features.

## Evidence / discovery anchors
Collections, Museums, Objects, Quizzes, Users; existing references to multiple infrastructure projects

## Architectural cautions
Handle authorization assumptions, transactions, validation and cancellation; avoid controllers and direct transport coupling. Preserve existing DI while reducing new infrastructure coupling.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

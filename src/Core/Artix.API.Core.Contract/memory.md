# Application contracts — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Core/Artix.API.Core.Contract`

## Observed role
CQRS requests and DTOs, repository/service interfaces, configuration models, and shared primitives.

## Evidence / discovery anchors
MediatR, FluentValidation, feature-oriented contracts, command/query repositories

## Architectural cautions
Maintain backward compatibility; avoid exposing persistence entities or transport-specific implementation details.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

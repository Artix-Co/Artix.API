# Domain services — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Core/Artix.API.Core.DomainService`

## Observed role
Cross-entity domain operations and policies that do not naturally belong to one aggregate.

## Evidence / discovery anchors
XPRules and domain service coordination; currently references SQL and RabbitMQ projects

## Architectural cautions
Keep domain decisions independently testable; existing infrastructure coupling is technical debt, not a required pattern.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

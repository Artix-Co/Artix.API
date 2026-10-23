# End-to-end tests — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`tests/Artix.API.Tests.EndToEnd`

## Observed role
HTTP-facing user journeys and composition-root verification.

## Evidence / discovery anchors
Current UnitTest1 placeholder

## Architectural cautions
Exercise authorization, status codes, schema, and real hosting behavior; keep test data isolated.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

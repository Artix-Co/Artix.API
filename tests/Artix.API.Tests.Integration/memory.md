# Integration tests — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`tests/Artix.API.Tests.Integration`

## Observed role
Persistence and multi-component contracts against realistic dependencies.

## Evidence / discovery anchors
XPRulesServiceTests; current UnitTest1 placeholder

## Architectural cautions
Prefer isolated test infrastructure and reproducible seed data; validate error cases and clean up state.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

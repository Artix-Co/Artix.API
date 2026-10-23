# Composition root / runtime host — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Presentation/Artix.API.WebService`

## Observed role
Program.cs, configuration, DI composition, middleware, health checks, Serilog, Kestrel and hosting.

## Evidence / discovery anchors
AddArtixServices, DataProtection, /health/live, /health/ready, Serilog and optional Elasticsearch

## Architectural cautions
Preserve middleware ordering, secure configuration precedence, environment boundaries, readiness semantics and graceful shutdown.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

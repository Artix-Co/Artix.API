# Redis integration — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Infra/Artix.API.Infra.Redis`

## Observed role
Cache, distributed state and Redis-backed infrastructure implementations.

## Evidence / discovery anchors
Redis options and integration with existing caching abstractions

## Architectural cautions
Use explicit TTL and key namespace; avoid treating cache as source of truth; review stampede, eviction and atomicity.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

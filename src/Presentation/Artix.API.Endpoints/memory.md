# HTTP contracts and endpoints — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Presentation/Artix.API.Endpoints`

## Observed role
AdminPanel, Client and Shared controllers, API middlewares, filters and route conventions.

## Evidence / discovery anchors
AdminBaseController, ClientBaseController, API key authentication middleware, API version checks, Swagger filter

## Architectural cautions
Controllers remain thin; do not embed persistence workflows, avoid leaking internal exceptions and require explicit auth.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

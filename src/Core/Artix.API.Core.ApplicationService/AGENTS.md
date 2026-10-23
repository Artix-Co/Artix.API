# Artix.API.Core.ApplicationService — Codex Agent Instructions

> Scope: `src/Core/Artix.API.Core.ApplicationService/**`
> Applies to Commands, Queries, Handlers, Validators, and application orchestration.
> Repository baseline: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.

## 1. Agent mandate and precedence

You are editing the existing Artix application-services layer, not generating a new architecture. Read the root `AGENTS.md` (when present), this file, and sibling `memory.md` before changes. Inspect the current request type, matching handler, base class, repository contract, DI registrations, and tests. The actual code and business requirements override examples in this document where signatures or semantics differ.

Primary goals, in order: **correctness, security, compatibility, observability, maintainability, performance**. Follow the smallest complete change principle (Ponytail), but never use small diffs as an excuse to omit input validation or testing. Graphify may help navigate dependencies only when installed and refreshed; verify links against source.

## 2. Actual structural conventions

- Feature directories follow `Features/<Feature>/<Admin|Client>/<Commands|Queries>/<UseCase>/` (with some existing variations). Keep existing paths stable.
- Request/response contracts, DTOs, and repository interfaces commonly live under `Artix.API.Core.Contract`.
- Handler implementations and validators live under `Artix.API.Core.ApplicationService`.
- Existing command and query base classes are in `Primitives/CommandHandlerBase.cs` and `Primitives/QueryHandlerBase.cs`.
- `ApplicationService/DependencyInjection.cs` calls `AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly)`.
- `Contract/DependencyInjection.cs` registers `FluentValidationBehavior<,>` as an `IPipelineBehavior<,>`.
- ApplicationService currently references some Infrastructure projects directly; treat this as observed coupling, not a policy for new use cases.

## 3. Required files for EVERY new or modified CQRS use case

When working on any Command or Query:

1. Identify the request type and its full input graph (primitive fields, nested objects, collections, route/query/body-bound values).
2. Locate or create **one request-specific `AbstractValidator<TRequest>` beside the corresponding handler** using the same namespace.
3. Name it `<ExactRequestTypeName>Validator.cs` (e.g., `GetTransactionByConfirmationCodeQueryValidator.cs`). Keep the corresponding handler named `<ExactRequestTypeName>Handler.cs` where this is consistent with existing patterns.
4. Validate every client-controlled field using rules appropriate to its domain. Use `RuleForEach`, `ChildRules`, `SetValidator`, `When`, and `DependentRules` for collection, nested, conditional and dependent fields as appropriate. Never invent arbitrary validation limits; seek existing requirements/schema, and explicitly flag unresolved constraints.
5. Distinguish **mandatory**, **optional**, **nullable**, **server-generated**, and **trusted/internal** fields. Do not add meaningless `NotEmpty()` rules to optional fields or demand client validation for server-derived claims.
6. Ensure validators are discoverable by assembly scanning and execute via the MediatR validation pipeline **before handler side effects**. Never assume registration is working solely because a validator class exists.
7. Add validator tests covering accepted input, boundaries, null/blank, invalid encoding/characters, cross-field conditions, and attempts to bypass expected constraints.
8. Add handler tests separately for business outcomes, cancellation, authorization, repository calls and error propagation.
9. Resolve legacy `// TODO: develop validator for this handler` only when a valid matching validator is implemented and tested. Do not delete TODOs merely for appearance.

**Prohibition:** Do not add an `AbstractValidator<Handler>` or validators of responses in place of `AbstractValidator<Request>`. Do not perform database writes in validator constructors or rule callbacks.

## 4. Critical distinction: validation vs normalization vs security encoding

**Validation** rejects structurally or semantically invalid data. FluentValidation does not implicitly sanitize or transform a request. `Must(...)` returns a pass/fail decision, not rewritten input.

**Normalization** is a documented, explicit transformation performed at a defined boundary: e.g., an allowed trim for display names, case folding where domain semantics permit, or canonicalization of a user-facing slug. It must be deterministic, covered by tests, and must not silently mutate immutable request records inside a validator.

**Encoding** must be context-specific **at the output/sink** (HTML, SQL parameters, URL, shell invocation). Neither regex validation nor a generic suspicious-patterns function replaces parameterized queries, safe HTML handling, authorization, or secure file paths.

For identifier-like values (confirmation codes, transaction IDs, OTPs, tokens, signatures, credentials, hashes), **reject invalid characters; do not strip or rewrite them**. If the business contract defines a canonical representation, normalize once at a documented boundary and validate that exact representation before lookup.

Never assume all free-text inputs should match `^[a-zA-Z0-9]+$`; that would incorrectly reject Persian, whitespace, punctuation, internationalized names and legitimate content. Apply allowlists to machine-readable identifiers only when the domain requires them. Guard maximum lengths and avoid expensive or unbounded regular expressions. If `ValidationHelper.IsNotContainSuspiciousPatterns` exists, treat it as defense-in-depth, not an injection defense; confirm behavior and performance and avoid false positives.

## 5. Query handler contract

- Follow the actual `QueryHandlerBase<TQuery,TResponse>` generic contract and its `Handle` return type. In the inspected `dev` branch it returns `Task<Result<TResponse>>`, **not** necessarily `Task<TResponse>` as in other project snapshots.
- Keep query handlers read-oriented. Do not introduce writes or domain side effects without explicit requirements and architecture review (logging/metrics are not domain state changes).
- Use the query repository abstraction; propagate `CancellationToken` when supported.
- Enforce tenant/user/object authorization independently from input validation; a well-formed ID does not imply access.
- Clarify not-found behavior, nullable DTOs, pagination limits, filtering and sort-field allowlists; do not quietly return arbitrary or unbounded datasets.
- Prefer projections/read models and `AsNoTracking` where implemented in persistence layers; do not move EF queries into handlers.
- Cache only when correctness, key scoping, invalidation, privacy and TTL are defined. Never cache user-specific results under a global key.

### Query example pattern

```csharp
// Place beside the query handler, using the request type defined in Contract.
using FluentValidation;

public sealed class GetTransactionByConfirmationCodeQueryValidator
    : AbstractValidator<GetTransactionByConfirmationCodeQuery>
{
    public GetTransactionByConfirmationCodeQueryValidator()
    {
        RuleFor(x => x.ConfirmationCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(10, 80)
            .Matches("^[A-Za-z0-9]+$")
            .WithMessage("Confirmation code must contain only ASCII letters and digits.");
    }
}
```

> This example assumes the specified code format (10–80 ASCII alphanumeric) is the agreed business contract. It does **not** mutate/sanitize the code. Do not apply the example's numeric limits to unrelated requests. Add `Must(ValidationHelper.IsNotContainSuspiciousPatterns)` only if that helper exists, has verified semantics, and adds useful coverage beyond the allowlist.

## 6. Command handler contract

- Follow the actual `CommandHandlerBase<TCommand>` and specific `ICommand` return contract. On the inspected `dev` snapshot, its abstract `Handle` returns `Task<Guid>`; avoid assuming every command produces the same type across future revisions.
- Validate incoming fields **before** executing writes or publishing events.
- Enforce authorization, ownership, invariants and optimistic concurrency in application/domain logic; validation alone is insufficient.
- Decide transaction boundaries across SQL operations; never assume sending RabbitMQ events and committing SQL are atomic.
- Use the existing outbox pattern where applicable; publish safely and design downstream delivery for duplicates.
- For user-initiated retryable commands, consider idempotency keys and unique constraints, especially around purchases, balances or mutable collection operations.
- Propagate cancellation where safe; do not cancel midway through a committed multi-step operation without documenting recovery semantics.
- Update relevant cache entries only under a defined invalidation policy after successful persistence.

### Command validator example (illustrative; replace with actual request fields)

```csharp
using FluentValidation;

public sealed class AddObjectToCollectionCommandValidator
    : AbstractValidator<AddObjectToCollectionCommand>
{
    public AddObjectToCollectionCommandValidator()
    {
        RuleFor(x => x.ObjectId).GreaterThan(0);
        // Add validation for other actual request properties after inspecting the contract.
        // Do not invent property names, numeric types or constraints.
    }
}
```

> This is an illustrative pattern, not a drop-in replacement. Verify `ObjectId` exists and is a positive numeric identifier before applying it.

## 7. Validator design and performance

- Prefer `internal sealed`/`public sealed` according to the existing DI scanner and conventions; **verify discoverability**, because FluentValidation assembly scanning typically registers public non-abstract validators and may need explicit registration for internal types.
- Validators must be deterministic, side-effect-free and independently testable.
- Use `.Cascade(CascadeMode.Stop)` when subsequent rules assume earlier non-null/shape constraints.
- For async rules use `MustAsync` plus `ValidateAsync`; inject only necessary read abstractions, and keep network/database checks limited. Avoid duplicate repository queries in validator and handler unless unavoidable.
- Use domain-appropriate regexes with known bounded input lengths; consider generated/compiled regex if supported and actually beneficial. Keep a bounded input length ahead of costly pattern matching.
- Do not log raw personal, secret or financial identifiers as part of validation failures.
- Error messages should be safe, stable, readable and not reveal existence of protected records.
- Multiple validators per request are legal in FluentValidation, but the existing pipeline injects a **single** `IValidator<TRequest>?`; do not presume all validators run. Prefer one composed validator per request until the pipeline intentionally supports `IEnumerable<IValidator<TRequest>>`.
- Check behavior order and exception mapping in the API layer, ensuring `ValidationException` becomes a safe 4xx response rather than an unhandled 500.

## 8. Handler and DI discipline

- Keep handlers `internal sealed` when consistent with existing code; avoid heavy constructors or duplicated infrastructure concerns.
- Do not change base-class constructor or return signatures to match an example from another revision. Inspect exact definitions first.
- Preserve existing `AddApplicationServices()` and `AddContractServices()` registration; if adding new registrations, prove there is no duplicate registration or missing dependency.
- Avoid direct `HttpContext` business logic; use current-user abstractions where already available and improve isolation incrementally.
- Do not introduce new infrastructure project references for convenience. Prefer existing contract interfaces for new features.

## 9. Mandatory testing matrix

| Area | Minimum cases |
|---|---|
| Command validation | Required/optional fields; null/empty/whitespace; min/max; invalid types/formats; nested and cross-field conditions |
| Query validation | IDs/code boundaries; filter/sort allowlist; pagination and date-range constraints; invalid search/pattern input |
| Validation pipeline | Validator discovered; invalid request never reaches handler; valid request does; errors surface correctly |
| Security | Authorization even when input passes; no log leaks; no untrusted SQL/HTML/path concatenation |
| Commands | Success, duplicate/retry, concurrency/conflict, transaction or outbox failure, cancellation |
| Queries | Success, not found, unauthorized, cancellation, scoped cache consistency |

Execute from repository root when practical:

```bash
dotnet build Artix.API.sln -c Release
dotnet test Artix.API.sln -c Release --no-build
```

Some integration/E2E tests need SQL Server, MongoDB, Redis and RabbitMQ; report missing prerequisites, don't claim success without running. Write focused tests in `tests/Artix.API.Tests.Unit` and integration tests when behavior spans DI/persistence/messaging.

## 10. Agent execution checklist

Before editing:
- [ ] Inspect exact request contract, handler, repository interface, base class and DI.
- [ ] Identify all untrusted fields, trust boundaries and real domain constraints.
- [ ] Confirm whether normalization is permitted; never silently change identifiers.

After editing:
- [ ] Every modified Command/Query has an adjacent functional validator or a documented reason why it has no untrusted input.
- [ ] Validation is registered and executed ahead of handler invocation.
- [ ] Contracts and existing public behavior remain compatible (or intentional changes documented).
- [ ] Tests cover valid/invalid and domain/security failure paths.
- [ ] Build/test outcomes are reported truthfully.
- [ ] Update `memory.md` only with verified durable findings and source paths.

## 11. Change report format

Include (1) changed files; (2) validated request fields and constraints; (3) normalization policy if any; (4) behavioral and security implications; (5) tests actually executed and results; (6) unresolved risks. Never write 'sanitized' when the validator only rejects invalid values.

# Artix.API.Core.ApplicationService — Architecture Memory

> Baseline: repository `Artix-Co/Artix.API`, branch `dev`, inspected 2026-10-23.
> Purpose: **verified durable project facts and design decisions**, not task scratch notes or instructions that override `AGENTS.md`.

## 1. Role and topology

Path: `src/Core/Artix.API.Core.ApplicationService/`.

The layer hosts use-case implementations and orchestration, organized under `Features/` by business feature and Admin/Client access path. Request/response contracts and repository interfaces are generally in `src/Core/Artix.API.Core.Contract/`. Existing feature names include Collections, Museums, Objects, Quizzes and Users.

The layer uses CQRS-style command and query handlers; do not assume strict command/query read/write separation has been mechanically enforced throughout the codebase.

## 2. Concrete verified source anchors

| Path | Observed contract/behavior |
|---|---|
| `Primitives/QueryHandlerBase.cs` | `QueryHandlerBase<TQuery,TResponse>` implements query handler; `Handle` returns `Task<Result<TResponse>>` on `dev`; constructor takes `IHttpContextAccessor` and `UserManager<AppUser>`; helpers read current user/role. |
| `Primitives/CommandHandlerBase.cs` | `CommandHandlerBase<TCommand>`; constructor takes `IHttpContextAccessor`, `UserManager<AppUser>`, logger; `Handle` returns `Task<Guid>` on inspected branch. |
| `DependencyInjection.cs` | `services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);`. |
| `Features/Users/Client/Queries/GetLogout/GetLogoutQueryValidator.cs` | File exists but contains only an empty class, so it is **not** a functional FluentValidation validator in inspected snapshot. |
| `Features/Museums/Client/Queries/GetAll/GetAllMuseumsQueryHandler.cs` | Has `// TODO: develop validator for this handler`, extends query base, and calls `IMuseumQueryRepository`. |
| `../Artix.API.Core.Contract/Primitives/Validations/FluentValidationBehavior.cs` | Pipeline accepts a nullable **single** `IValidator<TRequest>` and executes `ValidateAsync`; throws `ValidationException` on invalid result. |
| `../Artix.API.Core.Contract/DependencyInjection.cs` | Registers MediatR and `FluentValidationBehavior<,>`; scans assembly set for repository abstractions. |

## 3. Important contrast with supplied transaction example

An externally supplied sample `GetTransactionByConfirmationCodeQueryHandler` returns `Task<TransactionByConfirmationCodeDto>` and uses a query base constructor with `IMemoryCache`, `ITokenService`, and `IHttpContextAccessor`. This **does not match** the `dev` branch `QueryHandlerBase` signature verified above. Treat it as a **desired style example for adjacent validators**, not as evidence that the present base class has those signatures. Re-read source before generating code; never force `Task<Result<T>>` or `Task<T>` generically across different revisions.

The example validator's ConfirmationCode rules (`NotEmpty`, length 10–80, ASCII alphanumeric) are **user-provided illustrative requirements**, not verified domain-wide rules for all Artix identifiers. They must be confirmed against the actual transaction request/business contract before implementation.

## 4. Dependency and orchestration caveat

The inspected `Artix.API.Core.ApplicationService.csproj` references Contract, DomainService and also FileService, Identity, RabbitMQ, Redis and Sql projects. This is real coupling and a modernization candidate; it is not evidence that clean dependency inversion is already implemented. Preserve compatibility during feature work and assess refactor impact separately.

## 5. Validation pipeline facts and gaps

- `AddValidatorsFromAssembly` is present, but **runtime effectiveness must be verified**: ensure composition root calls `AddApplicationServices` and `AddContractServices`, validators are discovered, and the MediatR pipeline runs for the relevant request.
- The observed pipeline takes one `IValidator<TRequest>?`; if several validators are registered, do not assume all execute. Changing it to `IEnumerable<IValidator<TRequest>>` requires explicit design and tests.
- FluentValidation **does not mutate or sanitize input by default**. A `Must(...)` rule rejects suspicious content; it does not rewrite that content.
- Handle `ValidationException` consistently at the HTTP boundary and avoid exposing sensitive validation values.
- Review request-specific validators whenever input contracts, schemas or business invariants change.

## 6. Local design contract for future Command and Query work

1. **Co-location:** Place `<RequestTypeName>Validator.cs` beside `<RequestTypeName>Handler.cs` in the same feature/use-case folder and namespace; request DTOs/contracts can remain in Contract.
2. **Coverage:** Validate each client-controlled input according to real business semantics, including nested objects, collections and cross-field rules. Optional fields should remain optional.
3. **Separation:** Validation rejects; normalization is an explicit, approved, deterministic operation at a boundary; output encoding and parameterized persistence are separate security controls.
4. **Identity-like data:** Transaction confirmation codes, OTPs, identifiers and tokens should not be silently stripped/rewritten; reject invalid formats.
5. **Queries:** Maintain authz, cancellation, scoping, pagination and not-found semantics. Avoid side effects.
6. **Commands:** Enforce invariants, authorization, transaction boundaries, duplicate protection, outbox/event safety and cache invalidation as appropriate.
7. **Coverage:** Test validator, MediatR validation pipeline and handler independently; integration-test persistence/event interactions when changed.

## 7. Examples and boundaries

**Approved conceptual query shape** (only after verifying the actual request):

```csharp
public sealed class GetTransactionByConfirmationCodeQueryValidator
    : AbstractValidator<GetTransactionByConfirmationCodeQuery>
{
    public GetTransactionByConfirmationCodeQueryValidator()
    {
        RuleFor(x => x.ConfirmationCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(10, 80)
            .Matches("^[A-Za-z0-9]+$");
    }
}
```

This rejects bad input. It is **not** a sanitizer. Any project-specific `ValidationHelper.IsNotContainSuspiciousPatterns` requires separate verification before including it, and should not substitute for boundary protections.

**Command counterpart:** Place `SomeCommandValidator : AbstractValidator<SomeCommand>` beside `SomeCommandHandler`. Use the request's true property types and rules; the presence of a handler does not justify inventing validation constraints.

## 8. Verification checklist for memory updates

When architecture changes, verify and update with dated paths:

- Current request/handler/validator location and type signatures.
- DI composition and validator scanning/execution behavior.
- Security decisions: allowed normalization, cross-field invariants, authorization policy.
- Transactions/cache/outbox integration and durable consistency choices.
- What actually passed: tests/CI results belong in a task report, **not** permanent architecture memory unless describing stable test infrastructure.

Never store secrets, tokens, customer information, environment-specific credentials, speculative observations or noisy change logs. Remove obsolete claims when a refactor changes the source of truth.

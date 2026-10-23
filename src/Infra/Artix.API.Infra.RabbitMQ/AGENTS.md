# Messaging and notifications — scoped Codex instructions

Applies to `src/Infra/Artix.API.Infra.RabbitMQ/**`. Read root `/AGENTS.md` and `/memory.md` first.

## Ownership
RabbitMQ connection, event publishing, outbox processors, hosted consumers and SignalR notification integrations.

## Local engineering rules
- Inspect existing classes and contracts before editing; preserve public behavior unless the task explicitly changes it.
- Use the smallest complete change, without creating abstractions for hypothetical future use.
- Run the relevant build/tests; if dependency services are absent, report blocked verification instead of marking green.
- Assess downstream consumers before changing public types, serialized representations or persistence contracts.
- Consumers must be idempotent; enforce ack/nack and retry/DLQ policy; outbox dispatch must tolerate duplicates.
- Respect source-specific dependency direction: never add a reference merely to work around an awkward boundary.
- Do not write secrets or personal data to files, logs, test snapshots or memory.

## Change checklist
1. Locate affected contracts, callers and runtime registration.
2. Verify module-specific invariants and compatibility.
3. Implement minimal code plus a regression test when behavior changes.
4. Refresh Graphify output if it is configured for the repository; check graph freshness.
5. Document durable confirmed changes in sibling `memory.md`.

## Completion
Report implementation paths, validation command/results, compatibility implications and outstanding risks.

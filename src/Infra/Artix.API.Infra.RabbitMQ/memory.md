# Messaging and notifications — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Infra/Artix.API.Infra.RabbitMQ`

## Observed role
RabbitMQ connection, event publishing, outbox processors, hosted consumers and SignalR notification integrations.

## Evidence / discovery anchors
NotificationConsumerHostedService, NotificationOutboxProcessor, OutboxProcessor, RabbitMqEventPublisher, NotificationHub

## Architectural cautions
Consumers must be idempotent; enforce ack/nack and retry/DLQ policy; outbox dispatch must tolerate duplicates.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

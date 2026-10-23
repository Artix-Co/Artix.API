# File storage — verified context and upkeep

> Repository snapshot: `Artix-Co/Artix.API`, `dev`, inspected 2026-10-23.
> This is a navigational knowledge note, not a claim about every class's behavior.

## Project path
`src/Infra/Artix.API.Infra.FileService`

## Observed role
Upload and storage integration, file handling and filesystem-facing operations.

## Evidence / discovery anchors
FileSettings, FileEntity, UploadSession, shared upload controller interactions

## Architectural cautions
Validate file path canonicalization, file type, size limits, ownership, malicious content and transactional cleanup.

## Investigation checklist
- Read the `.csproj`, local dependency registrations and representative implementations.
- Trace actual caller/callee relationships before asserting a design rule is implemented.
- Check integration touchpoints and tests before API/schema changes.

## Memory update protocol
Append only verified, stable facts with a dated source file reference; remove facts invalidated by refactoring. Do not store secrets, full config values, transient bugs or unverifiable intent.

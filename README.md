# Artix API

Artix is a museum companion platform. It turns a museum visit into a guided game.

A user explores museums, scans QR codes, collects objects, earns XP, completes tasks, and returns for new rewards. The API manages users, museums, objects, scans, collections, seasons, quizzes, uploads, authentication, and progress.

## What the product does

### Explore museums

The client can:

- List museums.
- Open a museum page.
- View the objects in a museum.
- Open an object page.
- See recent museums and objects.

A museum or object enters recent visits only when the user opens its detail page. List requests do not add every result to recent visits.

### Scan QR codes and earn XP

A user scans an object or QR code from the client app.

The API handles:

- First scans.
- Repeat scans.
- Object upgrades.
- Golden-level scans.
- XP multipliers.
- Seasonal XP.
- Global and seasonal leaderboards.
- Duplicate-event protection.
- Concurrent scan protection.

The XP rules depend on the object type, tier, scan state, upgrade state, feature flags, and the active season.

### Build a collection

Users can add objects to their collection. A repeated object can still have value because repeat scans can upgrade the object and grant XP.

### Use quizzes and seasonal tasks

The API supports quiz and task flows for product features such as QR hunts, Co-Up, Strike, and the last quiz. A season can contain free and paid progress paths with XP and rewards.

### Authenticate users

Client authentication uses OTP. The flow is:

1. The client requests an OTP for a phone number.
2. The API creates a short-lived OTP.
3. The user submits the OTP.
4. The API creates or finds the user.
5. The API returns an access token and a refresh token.

OTP codes use a cryptographic random generator. The stored code is hashed. OTP send and verify operations have phone and IP rate limits.

Refresh tokens are stored as hashes in user sessions. Refresh requests use rotation, expiry checks, session checks, and a distributed lock.

### Manage content

Admin users can manage museums and objects. Admin access uses policies and permissions. Museum administrators can be limited to assigned museums.

Admin operations include:

- Create and remove museums.
- Create, upgrade, and remove objects.
- Read museum and object data.
- Manage uploaded digital assets.
- Review user data within the allowed policy scope.

### Upload digital assets

Admin users can upload images, documents, and 3D files in chunks.

The upload flow is:

1. Start an upload session.
2. Send chunks.
3. Complete the session.
4. Attach the completed file to a museum or object.

The API checks ownership, file size, chunk size, chunk index, file extension, and selected file signatures. It stores files under the configured application storage root. File names are unique.

## API areas

The main client routes are:

| Area | Example routes | Purpose |
|---|---|---|
| Authentication | `/api/client/authentication/*` | Send OTP, verify OTP, renew tokens, logout |
| Museums | `/api/client/museum/*` | List, view, and browse museums |
| Objects | `/api/client/object/*` | List, view, scan, and collect objects |
| User | `/api/client/user/*` | Profile, profile changes, delete, and login history |
| Quizzes | `/api/client/quest/*` | Read shuffled quizzes and tasks |
| Recent visits | `/api/client/museum/recent`, `/api/client/object/recent` | Read recent detail visits |

The main admin routes are:

| Area | Example routes | Purpose |
|---|---|---|
| Admin authentication | `/api/admin/authentication/*` | Admin login, protected registration, logout |
| Museums | `/api/admin/museum/*` | Manage museums and museum objects |
| Objects | `/api/admin/object/*` | Manage and inspect objects |
| Users | `/api/admin/user/*` | Read admin user data |
| Uploads | `/api/shared/upload/*` | Start, send, complete, and inspect uploads |

## Architecture

The solution uses layered .NET services and vertical slices.

```text
src/
  Core/
    Artix.API.Core.Domain              Domain entities and rules
    Artix.API.Core.Contract             Commands, queries, DTOs, repositories
    Artix.API.Core.DomainService        Cross-aggregate domain services
    Artix.API.Core.ApplicationService   MediatR handlers and validators
  Infra/
    Artix.API.Infra.Sql                 EF Core write and read stores
    Artix.API.Infra.Mongo               MongoDB read models
    Artix.API.Infra.Redis               Cache, locks, rate limits, sessions
    Artix.API.Infra.RabbitMQ             Messaging and notification support
    Artix.API.Infra.Identity             Identity, JWT, sessions, device trust
    Artix.API.Infra.FileService          Chunked upload and file storage
  Presentation/
    Artix.API.Endpoints                  Controllers and middleware
    Artix.API.WebService                 Application host and startup
  Utils/                                 Shared utilities

tests/
  Artix.API.Tests.Unit
  Artix.API.Tests.Integration
  Artix.API.Tests.EndToEnd
```

The application uses:

- ASP.NET Core 9.
- MediatR 12.
- SQL Server for command and relational read data.
- MongoDB for document-oriented read data.
- Redis for cache, rate limits, distributed locks, sessions, deduplication, and leaderboards.
- RabbitMQ for messaging and notification work.
- ASP.NET Identity and JWT for authentication.
- Docker Compose for local infrastructure.

Write operations use command repositories and the write database context. Read operations use query repositories and the read database context. Controllers do not access a database context directly.

## Run the project

### Development with Docker

Copy the Dev template:

```bash
cp .env.development.example .env
```

Set the values marked with `__SET_DEV_...__`. Then run:

```bash
docker compose up --build -d
```

Useful URLs:

- API: `http://localhost:8080`
- Liveness: `http://localhost:8080/health/live`
- Readiness: `http://localhost:8080/health/ready`
- Full health report: `http://localhost:8080/health`
- Swagger: enabled in Development

### Development without Docker

Start SQL Server, Redis, MongoDB, and RabbitMQ. Then run:

```bash
cd src/Presentation/Artix.API.WebService
dotnet run
```

The app loads `appsettings.Development.json` after `appsettings.json`. Environment variables have the highest priority.

### Production

Use `.env.production.example` as the release template:

```bash
cp .env.production.example .env
```

Set every production placeholder with the release secret manager. Do not commit `.env`.

The production image should be built by CI. The server should deploy the image and inject runtime configuration through environment variables or a secret manager.

## Configuration rules

Configuration is split by purpose:

- `appsettings.json` contains shared, non-environment-specific values.
- `appsettings.Development.json` contains Development overrides.
- `appsettings.Production.json` contains Production overrides.
- `.env.development.example` documents Dev container variables.
- `.env.production.example` documents Production release variables.
- `.env` contains local or release secrets and is never committed.

Environment variables use the .NET double-underscore format. Examples:

```text
ConnectionStrings__CommandConnectionString
AuthenticationApi__ApiKey
RedisOptions__Password
Authentication__IssuerSigningKey
```

## Health checks

`/health/live` checks that the process is alive.

`/health/ready` checks required dependencies such as SQL Server, MongoDB, Redis, and RabbitMQ.

`/health` returns the complete health report.

## Test and build

Run the closest test project first:

```bash
dotnet test tests/Artix.API.Tests.Unit/Artix.API.Tests.Unit.csproj
dotnet test tests/Artix.API.Tests.Integration/Artix.API.Tests.Integration.csproj
dotnet test tests/Artix.API.Tests.EndToEnd/Artix.API.Tests.EndToEnd.csproj
dotnet build Artix.API.sln
```

Integration and end-to-end tests need the configured database, Redis, RabbitMQ, MongoDB, and secret values.

## Security notes

The API applies these controls:

- Policy-based Admin authorization.
- Museum scope checks.
- OTP hashing, expiry, single use, and rate limits.
- Refresh-token hashing, expiry, rotation, and reuse protection.
- JWT user, security-stamp, session, and revocation checks.
- Upload ownership checks and size limits.
- File signature checks for supported binary formats.
- Production-safe error responses with correlation IDs.
- Trusted remote IP handling.
- Short private file-cache headers.

Review [SECURITY_BACKLOG.md](SECURITY_BACKLOG.md) before a release. Runtime security tests still need the required infrastructure and secrets.
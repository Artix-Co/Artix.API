# Artix.API

ASP.NET Core 9 museum/object API with SQL Server, MongoDB, Redis, and RabbitMQ.

## Quick start (Docker)

```bash
cp .env.example .env
# Edit .env — set strong passwords and keep MSSQL_SA_PASSWORD in sync with connection strings.

docker compose up --build -d
```

- API (via nginx): http://localhost:8080  
- Liveness (`self` only): http://localhost:8080/health/live  
- Readiness (SQL, Mongo, Redis, RabbitMQ): http://localhost:8080/health/ready  
- Full report (JSON): http://localhost:8080/health  
- Swagger (dev only): set `EnableSwagger=true` in `.env`

## Local development (without Docker)

1. Start dependencies (SQL on `localhost:1434`, Redis, Mongo, RabbitMQ) or use `docker compose up -d` for infra only.
2. Run the API:

```bash
cd src/Presentation/Artix.API.WebService
dotnet run
```

Uses `appsettings.Development.json` — do not use dev secrets in production.

## Configuration

| Source | Purpose |
|--------|---------|
| `.env` | Docker Compose secrets and overrides (not committed) |
| `.env.example` | Template — copy and customize |
| `appsettings.json` | Non-secret defaults; production values come from env |
| `appsettings.Development.json` | Local dev only |

Required production settings (via `.env` or environment):

- `ConnectionStrings__CommandConnectionString` / `QueryConnectionString`
- `Authentication__IssuerSigningKey`
- `RedisOptions__Password`, `RabbitMqOptions__Password`
- `MongoDbSettings__ConnectionString`

Elasticsearch is optional; leave `Elasticsearch__Uri` empty to disable.

## Deploy

**Principle:** CI builds the image once (on the GitLab runner / host Podman). Production only restarts with the new tag — no rebuild on deploy.

### Production URL

- https://api.studioartix.ir  
- Health: https://api.studioartix.ir/health/ready  

Server path: `/root/artix-api`  
Compose: `docker-compose -p artix ...`

### GitLab CI (primary)

1. Create project `studioartix/api` (or import from GitHub `Artix-Co/Artix.API`).
2. Push `main` / `dev` to GitLab.
3. Set CI/CD variables:
   - `DEPLOY_HOST` (e.g. `127.0.0.1` or server public IP)
   - `DEPLOY_USER` (`root`)
   - `DEPLOY_PATH` (`/root/artix-api`)
   - `SSH_PRIVATE_KEY_BASE64` (same pattern as frontend projects)
4. Pipeline:
   - `feat/*`, `fix/*`, `dev`, `main` → **build** image
   - `main` → **deploy_production** (manual)

On the server after first manual bring-up, keep `.env` in place (never commit it).

```bash
cd /root/artix-api
docker-compose -p artix ps
curl -fsS http://127.0.0.1:8080/health/ready
```

### GitHub Actions (optional)

- Workflow: `.github/workflows/ci.yml`
- Can publish to GHCR; not required if GitLab is the source of truth.

## Project layout

```
src/
  Core/           Domain, contracts, application services
  Infra/          SQL, Mongo, Redis, RabbitMQ, Identity, files
  Presentation/   WebService host + HTTP endpoints
tests/            Unit, integration, E2E
```

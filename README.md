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

**Principle:** CI builds and pushes the image once. The server only pulls and restarts — no `docker compose build` on the server.

### GitHub Actions

- Workflow: `.github/workflows/ci.yml`
- Publishes to `ghcr.io/<owner>/<repo>` on push to `dev`
- Deploy job needs repository secrets/variables:
  - `SERVER_SSH_KEY`
  - `DEPLOY_HOST`, `DEPLOY_PATH` (repository variables)

### GitLab CI

- Pipeline: `.gitlab-ci.yml`
- Uses built-in container registry
- Manual deploy job; set `SSH_PRIVATE_KEY`, `DEPLOY_HOST`, `DEPLOY_PATH`, `DEPLOY_USER`

On the server:

```bash
export APP_IMAGE=registry.example.com/artix/api:latest
docker compose pull app1
docker compose up -d --no-build
```

## Project layout

```
src/
  Core/           Domain, contracts, application services
  Infra/          SQL, Mongo, Redis, RabbitMQ, Identity, files
  Presentation/   WebService host + HTTP endpoints
tests/            Unit, integration, E2E
```

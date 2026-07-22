# syntax=docker/dockerfile:1.7

# ==============================
# Build
# ==============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Directory.Packages.props .
COPY Artix.API.sln .

COPY src/Core/Artix.API.Core.Contract/Artix.API.Core.Contract.csproj src/Core/Artix.API.Core.Contract/
COPY src/Core/Artix.API.Core.Domain/Artix.API.Core.Domain.csproj src/Core/Artix.API.Core.Domain/
COPY src/Core/Artix.API.Core.DomainService/Artix.API.Core.DomainService.csproj src/Core/Artix.API.Core.DomainService/
COPY src/Core/Artix.API.Core.ApplicationService/Artix.API.Core.ApplicationService.csproj src/Core/Artix.API.Core.ApplicationService/

COPY src/Infra/Artix.API.Infra.FileService/Artix.API.Infra.FileService.csproj src/Infra/Artix.API.Infra.FileService/
COPY src/Infra/Artix.API.Infra.Identity/Artix.API.Infra.Identity.csproj src/Infra/Artix.API.Infra.Identity/
COPY src/Infra/Artix.API.Infra.Mongo/Artix.API.Infra.Mongo.csproj src/Infra/Artix.API.Infra.Mongo/
COPY src/Infra/Artix.API.Infra.RabbitMQ/Artix.API.Infra.RabbitMQ.csproj src/Infra/Artix.API.Infra.RabbitMQ/
COPY src/Infra/Artix.API.Infra.Redis/Artix.API.Infra.Redis.csproj src/Infra/Artix.API.Infra.Redis/
COPY src/Infra/Artix.API.Infra.Sql/Artix.API.Infra.Sql.csproj src/Infra/Artix.API.Infra.Sql/

COPY src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj src/Presentation/Artix.API.WebService/
COPY src/Presentation/Artix.API.Endpoints/Artix.API.Endpoints.csproj src/Presentation/Artix.API.Endpoints/
COPY src/Utils/Artix.API.Utils/Artix.API.Utils.csproj src/Utils/Artix.API.Utils/

# Restore only the host project graph (no test projects → less NuGet traffic)
RUN dotnet restore src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj -v:m

COPY src/ ./src/

RUN dotnet publish src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj \
    -c Release \
    --no-restore \
    /p:UseAppHost=false \
    -o /app/publish

# ==============================
# Runtime
# ==============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is only for container healthchecks; keep the layer small and cacheable
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

COPY --from=build /app/publish .

# Stay root: named volumes for /app/files and dataprotection-keys are root-owned by default.
# Switch to a non-root user only after an entrypoint chown strategy exists.

ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]

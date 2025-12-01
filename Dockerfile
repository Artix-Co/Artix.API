# syntax=docker/dockerfile:1.4

# ------------------------------
# Build stage
# ------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ------------------------------
# Copy solution and NuGet config (for private feeds)
# ------------------------------
COPY Directory.Packages.props .

COPY Artix.API.sln .

# ------------------------------
# Copy only csproj files for restore
# ------------------------------
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

COPY src/Orchestration/Artix.API.Orchestration.AppHost/Artix.API.Orchestration.AppHost.csproj src/Orchestration/Artix.API.Orchestration.AppHost/
COPY src/Orchestration/Artix.API.Orchestration.ServiceDefaults/Artix.API.Orchestration.ServiceDefaults.csproj src/Orchestration/Artix.API.Orchestration.ServiceDefaults/

COPY src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj src/Presentation/Artix.API.WebService/
COPY src/Presentation/Artix.API.Endpoints/Artix.API.Endpoints.csproj src/Presentation/Artix.API.Endpoints/
COPY src/Utils/Artix.API.Utils/Artix.API.Utils.csproj src/Utils/Artix.API.Utils/

# copy test projects
COPY tests/Artix.API.Tests.EndToEnd/Artix.API.Tests.EndToEnd.csproj tests/Artix.API.Tests.EndToEnd/
COPY tests/Artix.API.Tests.Integration/Artix.API.Tests.Integration.csproj tests/Artix.API.Tests.Integration/
COPY tests/Artix.API.Tests.Unit/Artix.API.Tests.Unit.csproj tests/Artix.API.Tests.Unit/
COPY tests/Directory.Build.props tests/

# ------------------------------
# Restore all packages
# ------------------------------
RUN dotnet restore Artix.API.sln -v:m

# ------------------------------
# Copy all source code
# ------------------------------
COPY src/ src/
COPY tests/ tests/

# ------------------------------
# Publish WebService
# ------------------------------
RUN dotnet publish src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj \
    -c Release \
    /p:UseAppHost=false \
    -o /app/publish

# ------------------------------
# Runtime stage
# ------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]

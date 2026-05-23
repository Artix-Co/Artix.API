# syntax=docker/dockerfile:1.4
# ==============================
# Dependencies Stage - Maximum cache reuse
# ==============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dependencies
WORKDIR /src

# Copy only required files for restore
COPY Directory.Packages.props .
COPY Artix.API.sln .

# Copy all csproj files in one optimized layer
COPY src/ ./src/
COPY tests/ ./tests/

RUN --mount=type=cache,target=/root/.nuget/packages,id=nuget-packages \
    --mount=type=cache,target=/root/.local/share/NuGet,id=nuget \
    for proj in $(find . -name "*.csproj"); do \
        dotnet restore "$proj" -v:m --packages /root/.nuget/packages; \
    done

# ==============================
# Build Stage - Parallel builds
# ==============================
FROM dependencies AS builder
WORKDIR /src

# Copy source code
COPY src/ ./src/

# Build specific project with all dependencies already restored
RUN --mount=type=cache,target=/root/.nuget/packages,id=nuget-packages \
    dotnet build src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj \
    -c Release --no-restore

# ==============================
# Publish Stage
# ==============================
FROM builder AS publisher
WORKDIR /src

RUN --mount=type=cache,target=/root/.nuget/packages,id=nuget-packages \
    dotnet publish src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj \
    -c Release \
    /p:UseAppHost=false \
    /p:TieredPGO=true \
    /p:ReadyToRun=true \
    -o /app/publish \
    --no-build

# ==============================
# Optimized Runtime - Alpine (smaller and faster)
# ==============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install ICU for Alpine
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_URLS=http://+:80 \
    COMPlus_ReadyToRun=1

EXPOSE 80

COPY --from=publisher /app/publish .

ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]
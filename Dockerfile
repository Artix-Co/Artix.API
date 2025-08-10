FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Artix.API.sln ./
COPY Directory.Packages.props ./
COPY src/ ./src/
#COPY test/ ./test/

RUN dotnet restore Artix.API.sln

WORKDIR /src/src/Presentation/Artix.API.WebService
RUN dotnet publish Artix.API.WebService.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]

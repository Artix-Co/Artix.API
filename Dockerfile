# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj
RUN dotnet publish ./src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80



ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]

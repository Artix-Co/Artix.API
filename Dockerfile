FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# کپی فایل‌های solution و پروژه‌ها
COPY ["Artix.API.sln", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/Presentation/Artix.API.WebService/Artix.API.WebService.csproj", "src/Presentation/Artix.API.WebService/"]
COPY ["Artix.ServiceDefaults/Artix.ServiceDefaults.csproj", "Artix.ServiceDefaults/"]
COPY ["Artix.AppHost/Artix.AppHost.csproj", "Artix.AppHost/"]

# کپی محتوای پروژه‌ها
COPY ["src/", "src/"]
COPY ["Artix.ServiceDefaults/", "Artix.ServiceDefaults/"]
COPY ["Artix.AppHost/", "Artix.AppHost/"]

# Restore پروژه‌ها
RUN dotnet restore "Artix.API.sln"

# Build و Publish پروژه Artix.API.WebService
WORKDIR "/src/src/Presentation/Artix.API.WebService"
RUN dotnet publish "Artix.API.WebService.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Artix.API.WebService.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# --- csproj copies for restore (API + UI + deps) ---
COPY ["src/ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj", "ThunderbirdsBoardGameEngine.Api/"]
COPY ["src/ThunderbirdsBoardGameEngine.PublishedLanguage/ThunderbirdsBoardGameEngine.PublishedLanguage.csproj", "ThunderbirdsBoardGameEngine.PublishedLanguage/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Application/ThunderbirdsBoardGameEngine.Catalog.Application.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Application/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Infrastructure/ThunderbirdsBoardGameEngine.Catalog.Infrastructure.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Infrastructure/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Contracts/ThunderbirdsBoardGameEngine.Catalog.Contracts.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Contracts/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Domain/ThunderbirdsBoardGameEngine.Catalog.Domain.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Domain/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Format/ThunderbirdsBoardGameEngine.Catalog.Format.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Format/"]

COPY ["src/ThunderbirdsBoardGameEngine.UI/ThunderbirdsBoardGameEngine.UI.csproj", "ThunderbirdsBoardGameEngine.UI/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Client/ThunderbirdsBoardGameEngine.Catalog.Client.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Client/"]

RUN dotnet restore "./ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj"

# --- copy full source ---
COPY src/ .

# Build API (same as before)
WORKDIR "/src/ThunderbirdsBoardGameEngine.Api"
RUN dotnet build "./ThunderbirdsBoardGameEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish API (unchanged)
WORKDIR "/src/ThunderbirdsBoardGameEngine.Api"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish Blazor WASM UI
WORKDIR "/src/ThunderbirdsBoardGameEngine.UI"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.UI.csproj" -c $BUILD_CONFIGURATION -o /app/ui-publish

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy API output
COPY --from=publish /app/publish .

# Copy Blazor static files into wwwroot so the API can serve them
COPY --from=publish /app/ui-publish/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "ThunderbirdsBoardGameEngine.Api.dll"]

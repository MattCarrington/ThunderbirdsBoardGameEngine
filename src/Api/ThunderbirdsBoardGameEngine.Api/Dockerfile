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
COPY ["src/Api/ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj", "Api/ThunderbirdsBoardGameEngine.Api/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.csproj", "ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Runtime/ThunderbirdsBoardGameEngine.ReferenceData.Runtime.csproj", "ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Runtime/"]
COPY ["src/Common/ThunderbirdsBoardGameEngine.PublishedLanguage/ThunderbirdsBoardGameEngine.PublishedLanguage.csproj", "Common/ThunderbirdsBoardGameEngine.PublishedLanguage/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Application/ThunderbirdsBoardGameEngine.Catalog.Application.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Application/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Infrastructure/ThunderbirdsBoardGameEngine.Catalog.Infrastructure.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Infrastructure/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Contracts/ThunderbirdsBoardGameEngine.Catalog.Contracts.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Contracts/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Domain/ThunderbirdsBoardGameEngine.Catalog.Domain.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Domain/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Format/ThunderbirdsBoardGameEngine.Catalog.Format.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Format/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Application/ThunderbirdsBoardGameEngine.Rules.Application.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Application/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Infrastructure/ThunderbirdsBoardGameEngine.Rules.Infrastructure.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Infrastructure/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/ThunderbirdsBoardGameEngine.Rules.Contracts.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Domain/ThunderbirdsBoardGameEngine.Rules.Domain.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Domain/"]

COPY ["src/Ui/ThunderbirdsBoardGameEngine.UI/ThunderbirdsBoardGameEngine.UI.csproj", "Ui/ThunderbirdsBoardGameEngine.UI/"]
COPY ["src/Catalog/ThunderbirdsBoardGameEngine.Catalog.Client/ThunderbirdsBoardGameEngine.Catalog.Client.csproj", "Catalog/ThunderbirdsBoardGameEngine.Catalog.Client/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/ThunderbirdsBoardGameEngine.Rules.Client.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Client/"]
COPY ["src/Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/ThunderbirdsBoardGameEngine.Client.Infrastructure.csproj", "Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/"]

RUN dotnet restore "./Api/ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj"

# --- copy full source ---
COPY src/ .

# Build API (same as before)
WORKDIR "/src/Api/ThunderbirdsBoardGameEngine.Api"
RUN dotnet build "./ThunderbirdsBoardGameEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish API (unchanged)
WORKDIR "/src/Api/ThunderbirdsBoardGameEngine.Api"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish Blazor WASM UI
WORKDIR "/src/Ui/ThunderbirdsBoardGameEngine.UI"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.UI.csproj" -c $BUILD_CONFIGURATION -o /app/ui-publish

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy API output
COPY --from=publish /app/publish .

# Copy Blazor static files into wwwroot so the API can serve them
COPY --from=publish /app/ui-publish/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "ThunderbirdsBoardGameEngine.Api.dll"]

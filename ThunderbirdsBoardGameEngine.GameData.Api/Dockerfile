# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

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
COPY ["ThunderbirdsBoardGameEngine.GameData.Api/ThunderbirdsBoardGameEngine.GameData.Api.csproj", "ThunderbirdsBoardGameEngine.GameData.Api/"]
COPY ["ThunderbirdsBoardGameEngine.Catalog.Application/ThunderbirdsBoardGameEngine.Catalog.Application.csproj", "ThunderbirdsBoardGameEngine.Catalog.Application/"]
COPY ["ThunderbirdsBoardGameEngine.Catalog.Infrastructure/ThunderbirdsBoardGameEngine.Catalog.Infrastructure.csproj", "ThunderbirdsBoardGameEngine.Catalog.Infrastructure/"]
COPY ["ThunderbirdsBoardGameEngine.Catalog.Contracts/ThunderbirdsBoardGameEngine.Catalog.Contracts.csproj", "ThunderbirdsBoardGameEngine.Catalog.Contracts/"]
COPY ["ThunderbirdsBoardGameEngine.Catalog.Domain/ThunderbirdsBoardGameEngine.Catalog.Domain.csproj", "ThunderbirdsBoardGameEngine.Catalog.Domain/"]
COPY ["ThunderbirdsBoardGameEngine.Serialization/ThunderbirdsBoardGameEngine.Serialization.csproj", "ThunderbirdsBoardGameEngine.Serialization/"]
RUN dotnet restore "./ThunderbirdsBoardGameEngine.GameData.Api/ThunderbirdsBoardGameEngine.GameData.Api.csproj"
COPY . .
WORKDIR "/src/ThunderbirdsBoardGameEngine.GameData.Api"
RUN dotnet build "./ThunderbirdsBoardGameEngine.GameData.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThunderbirdsBoardGameEngine.GameData.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThunderbirdsBoardGameEngine.GameData.Api.dll"]
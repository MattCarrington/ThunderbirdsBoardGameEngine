FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

COPY ["src/Api/ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj", "Api/ThunderbirdsBoardGameEngine.Api/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Core/ThunderbirdsBoardGameEngine.ReferenceData.Core.csproj", "ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Core/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Runtime/ThunderbirdsBoardGameEngine.ReferenceData.Runtime.csproj", "ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Runtime/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Application/ThunderbirdsBoardGameEngine.Rules.Application.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Application/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Infrastructure/ThunderbirdsBoardGameEngine.Rules.Infrastructure.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Infrastructure/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/ThunderbirdsBoardGameEngine.Rules.Contracts.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Domain/ThunderbirdsBoardGameEngine.Rules.Domain.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Domain/"]

COPY ["src/Ui/ThunderbirdsBoardGameEngine.UI/ThunderbirdsBoardGameEngine.UI.csproj", "Ui/ThunderbirdsBoardGameEngine.UI/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/ThunderbirdsBoardGameEngine.Rules.Client.csproj", "Rules/ThunderbirdsBoardGameEngine.Rules.Client/"]
COPY ["src/Common/ThunderbirdsBoardGameEngine.Client.Core/ThunderbirdsBoardGameEngine.Client.Core.csproj", "Common/ThunderbirdsBoardGameEngine.Client.Core/"]

RUN dotnet restore "./Api/ThunderbirdsBoardGameEngine.Api/ThunderbirdsBoardGameEngine.Api.csproj"

COPY src/ .

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR "/src/Api/ThunderbirdsBoardGameEngine.Api"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

WORKDIR "/src/Ui/ThunderbirdsBoardGameEngine.UI"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.UI.csproj" -c $BUILD_CONFIGURATION -o /app/ui-publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY --from=publish /app/ui-publish/wwwroot ./wwwroot

ENTRYPOINT ["dotnet", "ThunderbirdsBoardGameEngine.Api.dll"]
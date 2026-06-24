FROM mcr.microsoft.com/playwright/dotnet:v1.61.0-noble

ARG BUILD_CONFIGURATION=Release
ARG GITHUB_PACKAGES_USERNAME

ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION

WORKDIR /src

COPY ["NuGet.config", "."]
COPY ["Directory.Packages.props", "."]

COPY ["tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj", "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/"]

RUN --mount=type=secret,id=github_packages_token \
    sh -c 'set -eu \
      && cp NuGet.config /tmp/nuget.config \
      && dotnet nuget update source GitHub \
           --configfile /tmp/nuget.config \
           --username "$GITHUB_PACKAGES_USERNAME" \
           --password "$(cat /run/secrets/github_packages_token)" \
           --store-password-in-clear-text \
      && dotnet restore "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj" \
           --configfile /tmp/nuget.config \
      && rm /tmp/nuget.config'

COPY . .

WORKDIR /src/tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests

RUN dotnet build "ThunderbirdsBoardGameEngine.SmokeTests.csproj" \
    -c $BUILD_CONFIGURATION \
    --no-restore

ENTRYPOINT ["sh", "-c", "dotnet test ThunderbirdsBoardGameEngine.SmokeTests.csproj --no-build -c \"$BUILD_CONFIGURATION\" -v normal"]
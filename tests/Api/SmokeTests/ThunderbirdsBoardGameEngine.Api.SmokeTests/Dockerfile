# Use SDK for tests (not runtime)
FROM mcr.microsoft.com/dotnet/sdk:8.0
ARG BUILD_CONFIGURATION=Release

# Install Playwright dependencies first
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Install PowerShell (needed for playwright.ps1)
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Copy solution structure
COPY ["Directory.Packages.props", "."]
COPY ["tests/Api/SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests.csproj", "tests/Api/SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/ThunderbirdsBoardGameEngine.Rules.Client.csproj", "src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/"]
COPY ["src/Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/ThunderbirdsBoardGameEngine.Client.Infrastructure.csproj", "src/Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/ThunderbirdsBoardGameEngine.Rules.Contracts.csproj", "src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/"]
COPY ["tests/Common/ThunderbirdsBoardGameEngine.TestUtils/ThunderbirdsBoardGameEngine.TestUtils.csproj", "tests/Common/ThunderbirdsBoardGameEngine.TestUtils/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.csproj", "src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/"]

# Restore dependencies
RUN dotnet restore "tests/Api/SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests.csproj"

# Copy all source
COPY . .

# Build the test project
WORKDIR /src/tests/Api/SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests
RUN dotnet build "ThunderbirdsBoardGameEngine.Api.SmokeTests.csproj" -c $BUILD_CONFIGURATION

# Install Playwright browsers
RUN pwsh bin/$BUILD_CONFIGURATION/net8.0/playwright.ps1 install chromium --with-deps

# Set working directory (stays here for entrypoint)
WORKDIR /src/tests/Api/SmokeTests/ThunderbirdsBoardGameEngine.Api.SmokeTests

# Run tests
ENTRYPOINT ["dotnet", "test", "ThunderbirdsBoardGameEngine.Api.SmokeTests.csproj", "--no-build", "-c", "Release", "-v", "detailed"]
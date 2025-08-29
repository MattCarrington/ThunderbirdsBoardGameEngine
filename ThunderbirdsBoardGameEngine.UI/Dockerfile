# Build WASM
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# cache restore
COPY ["ThunderbirdsBoardGameEngine.UI/ThunderbirdsBoardGameEngine.UI.csproj", "ThunderbirdsBoardGameEngine.UI/"]
COPY ["ThunderbirdsBoardGameEngine.Catalog.Client/ThunderbirdsBoardGameEngine.Catalog.Client.csproj", "ThunderbirdsBoardGameEngine.Catalog.Client/"]
# COPY other referenced csproj if needed
RUN dotnet restore "./ThunderbirdsBoardGameEngine.UI/ThunderbirdsBoardGameEngine.UI.csproj"

# build & publish
COPY . .
WORKDIR "/src/ThunderbirdsBoardGameEngine.UI"
RUN dotnet publish "./ThunderbirdsBoardGameEngine.UI.csproj" \
    -c Release -o /app/publish

# Serve static site
FROM nginx:alpine AS final
# SPA fallback
COPY "./ThunderbirdsBoardGameEngine.UI/nginx.conf" /etc/nginx/conf.d/default.conf
# static files
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80

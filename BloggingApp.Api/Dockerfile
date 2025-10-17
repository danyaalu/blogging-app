# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY BloggingApp.Api.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install sqlite3 and wget for database operations and health checks
RUN apt-get update && apt-get install -y sqlite3 wget && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Create data directory for SQLite database
RUN mkdir -p /app/data && chmod 777 /app/data

# Expose port 8080 (HTTP only - reverse proxy will handle HTTPS)
EXPOSE 8080

ENTRYPOINT ["dotnet", "BloggingApp.Api.dll"]

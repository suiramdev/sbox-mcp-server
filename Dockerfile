# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (better layer caching)
COPY ["modelcontextprotocol.csproj", "./"]
RUN dotnet restore "modelcontextprotocol.csproj" --runtime linux-x64

# Copy source code and build the application
COPY . .
RUN dotnet publish "modelcontextprotocol.csproj" -c Release -o /app/publish \
    --no-restore \
    --self-contained true \
    --runtime linux-x64 \
    /p:PublishTrimmed=false

# Use a minimal base image for self-contained deployment
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' --uid 1001 appuser && \
    chown -R appuser:appuser /app
USER appuser

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true

# Add health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the self-contained application
ENTRYPOINT ["./SandboxModelContextProtocol.Server"]
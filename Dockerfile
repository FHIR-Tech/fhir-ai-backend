# Use the official .NET 8.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files
COPY ["src/HealthTech.API/HealthTech.API.csproj", "src/HealthTech.API/"]
COPY ["src/HealthTech.Application/HealthTech.Application.csproj", "src/HealthTech.Application/"]
COPY ["src/HealthTech.Infrastructure/HealthTech.Infrastructure.csproj", "src/HealthTech.Infrastructure/"]
COPY ["src/HealthTech.Domain/HealthTech.Domain.csproj", "src/HealthTech.Domain/"]
COPY ["src/HealthTech.Database/HealthTech.Database.csproj", "src/HealthTech.Database/"]

# Restore dependencies
RUN dotnet restore "src/HealthTech.API/HealthTech.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/HealthTech.API"
RUN dotnet build "HealthTech.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "HealthTech.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "HealthTech.API.dll"]

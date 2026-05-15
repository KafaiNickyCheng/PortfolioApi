# ── Stage 1: Build ──────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first
COPY ["portfolio-api.csproj", "."]
RUN dotnet restore

# Copy everything else and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ── Stage 2: Runtime ─────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy built output from stage 1
COPY --from=build /app/publish .

# Port the container listens on
EXPOSE 8080

# Set to production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "portfolio-api.dll"]
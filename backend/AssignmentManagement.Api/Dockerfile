# ─── Build Stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AssignmentManagement.Api.csproj", "./"]
RUN dotnet restore "AssignmentManagement.Api.csproj"

COPY . .
RUN dotnet build "AssignmentManagement.Api.csproj" -c Release -o /app/build

# ─── Publish Stage ────────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "AssignmentManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ─── Final Stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Create logs directory
RUN mkdir -p /app/logs && chown appuser:appgroup /app/logs

COPY --from=publish /app/publish .

USER appuser
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AssignmentManagement.Api.dll"]

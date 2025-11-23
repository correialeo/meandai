FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

# Copiar os arquivos .csproj e restaurar dependências em camada separada para cache
COPY ["MeandAI.Api/MeandAI.Api.csproj", "MeandAI.Api/"]
COPY ["MeandAI.Application/MeandAI.Application.csproj", "MeandAI.Application/"]
COPY ["MeandAI.Infrastructure/MeandAI.Infrastructure.csproj", "MeandAI.Infrastructure/"]
COPY ["MeandAI.Domain/MeandAI.Domain.csproj", "MeandAI.Domain/"]
RUN dotnet restore "MeandAI.Api/MeandAI.Api.csproj"

# Copiar o código fonte
COPY ["MeandAI.Domain/", "MeandAI.Domain/"]
COPY ["MeandAI.Application/", "MeandAI.Application/"]
COPY ["MeandAI.Infrastructure/", "MeandAI.Infrastructure/"]
COPY ["MeandAI.Api/", "MeandAI.Api/"]

WORKDIR /MeandAI.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

RUN addgroup --system --gid 1001 dotnetuser && \
    adduser --system --uid 1001 --ingroup dotnetuser dotnetuser

WORKDIR /app

COPY --from=build /app/publish .

RUN chown -R dotnetuser:dotnetuser /app
USER dotnetuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Development

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MeandAI.Api.dll"]
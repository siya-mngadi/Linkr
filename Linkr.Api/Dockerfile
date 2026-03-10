FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 7410

# 1️ Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Linkr.Infrastructure/Linkr.Infrastructure.csproj", "Linkr.Infrastructure/"]
COPY ["Linkr.Domain/Linkr.Domain.csproj", "Linkr.Domain/"]
COPY ["Linkr.Api/Linkr.Api.csproj", "Linkr.Api/"]

RUN dotnet restore "./Linkr.Api/Linkr.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Linkr.Api"
RUN dotnet build "./Linkr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 2 Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Linkr.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 3 Runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN chown -R app:app /app

USER app
ENTRYPOINT ["dotnet", "Linkr.Api.dll"]
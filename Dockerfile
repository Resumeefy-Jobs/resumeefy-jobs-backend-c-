FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Resumeefy.API/Resumeefy.API.csproj Resumeefy.API/
COPY Resumeefy.Application/Resumeefy.Application.csproj Resumeefy.Application/
COPY Resumeefy.Core/Resumeefy.Core.csproj Resumeefy.Core/
COPY Resumeefy.Infrastructure/Resumeefy.Infrastructure.csproj Resumeefy.Infrastructure/

RUN dotnet restore Resumeefy.API/Resumeefy.API.csproj

COPY Resumeefy.API Resumeefy.API
COPY Resumeefy.Application Resumeefy.Application
COPY Resumeefy.Core Resumeefy.Core
COPY Resumeefy.Infrastructure Resumeefy.Infrastructure

WORKDIR /src/Resumeefy.API
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]

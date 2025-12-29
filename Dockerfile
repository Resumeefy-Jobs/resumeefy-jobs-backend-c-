# ---------------- BASE RUNTIME ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# ---------------- BUILD ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution + project files
COPY resumeefy-jobs-backend.sln .
COPY Resumeefy.API/Resumeefy.API.csproj Resumeefy.API/
COPY Resumeefy.Application/Resumeefy.Application.csproj Resumeefy.Application/
COPY Resumeefy.Core/Resumeefy.Core.csproj Resumeefy.Core/
COPY Resumeefy.Infrastructure/Resumeefy.Infrastructure.csproj Resumeefy.Infrastructure/

# Restore
RUN dotnet restore resumeefy-jobs-backend.sln

# Copy everything else (obj/bin ignored by .dockerignore)
COPY . .

# Build & publish
RUN dotnet publish resumeefy-jobs-backend.sln \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---------------- FINAL ----------------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]

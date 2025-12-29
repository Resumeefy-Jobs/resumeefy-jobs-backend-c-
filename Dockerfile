# ---------------- BASE RUNTIME ----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ---------------- BUILD ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1️⃣ Copy ONLY project files (no obj/bin)
COPY Resumeefy.API/Resumeefy.API.csproj Resumeefy.API/
COPY Resumeefy.Application/Resumeefy.Application.csproj Resumeefy.Application/
COPY Resumeefy.Core/Resumeefy.Core.csproj Resumeefy.Core/
COPY Resumeefy.Infrastructure/Resumeefy.Infrastructure.csproj Resumeefy.Infrastructure/

# 2️⃣ Restore dependencies
RUN dotnet restore Resumeefy.API/Resumeefy.API.csproj

# 3️⃣ Copy source code AFTER restore
COPY Resumeefy.API/ Resumeefy.API/
COPY Resumeefy.Application/ Resumeefy.Application/
COPY Resumeefy.Core/ Resumeefy.Core/
COPY Resumeefy.Infrastructure/ Resumeefy.Infrastructure/

# 4️⃣ Build
WORKDIR /src/Resumeefy.API
RUN dotnet build Resumeefy.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# ---------------- PUBLISH ----------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Resumeefy.API.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ---------------- FINAL ----------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]

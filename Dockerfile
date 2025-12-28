# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copy ALL source code first (including the "bad" obj folders)
COPY . .

# 2. NUCLEAR CLEANUP: Force delete all local 'bin' and 'obj' folders.
#    This guarantees we start fresh and avoid "Duplicate Attribute" errors.
RUN find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

# 3. Restore dependencies (Now safe because the folders are clean)
#    We point directly to the API project, which will restore all referenced projects automatically.
WORKDIR "/src/Resumeefy.API"
RUN dotnet restore "./Resumeefy.API.csproj"

# 4. Build the project
RUN dotnet build "./Resumeefy.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Resumeefy.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]
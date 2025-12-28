# See https://aka.ms/customizecontainer to learn how to customize your debug container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD STAGE ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. COPY EVERYTHING FIRST
# We copy all source code (and the junk obj folders) immediately.
COPY . .

# 2. NUCLEAR CLEANUP
# We delete any 'bin' or 'obj' folders that came from your computer.
# This ensures the next steps start with a 100% clean slate.
RUN find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

# 3. RESTORE
# Now that the folders are clean, we restore the dependencies safely.
WORKDIR "/src/Resumeefy.API"
RUN dotnet restore "./Resumeefy.API.csproj"

# 4. BUILD
# Finally, we build the project using the fresh restore.
RUN dotnet build "./Resumeefy.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- PUBLISH STAGE ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Resumeefy.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL STAGE ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]
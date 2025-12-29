# ---------------- BUILD ----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY resumeefy-jobs-backend.sln .
COPY Resumeefy.API/Resumeefy.API.csproj Resumeefy.API/
COPY Resumeefy.Application/Resumeefy.Application.csproj Resumeefy.Application/
COPY Resumeefy.Core/Resumeefy.Core.csproj Resumeefy.Core/
COPY Resumeefy.Infrastructure/Resumeefy.Infrastructure.csproj Resumeefy.Infrastructure/

RUN dotnet restore Resumeefy.API/Resumeefy.API.csproj

COPY . .

RUN dotnet publish Resumeefy.API/Resumeefy.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ---------------- FINAL ----------------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "Resumeefy.API.dll"]

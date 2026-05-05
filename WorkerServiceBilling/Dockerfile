#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WorkerServiceBilling/WorkerService.csproj", "WorkerServiceBilling/"]
COPY ["Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj", "Backend.PriceComparison.Application/"]
COPY ["Backend.PriceComparison.Domain/Backend.PriceComparison.Domain.csproj", "Backend.PriceComparison.Domain/"]
COPY ["Backend.PriceComparison.Infraestructure.External.Plemsi/Backend.PriceComparison.Infraestructure.External.Plemsi.csproj", "Backend.PriceComparison.Infraestructure.External.Plemsi/"]
COPY ["Backend.PriceComparison.Infraestructure.Persistence.Mysql/Backend.PriceComparison.Infraestructure.Persistence.Mysql.csproj", "Backend.PriceComparison.Infraestructure.Persistence.Mysql/"]
RUN dotnet restore "./WorkerServiceBilling/WorkerService.csproj"
COPY . .
WORKDIR "/src/WorkerServiceBilling"
RUN dotnet build "./WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerService.dll"]

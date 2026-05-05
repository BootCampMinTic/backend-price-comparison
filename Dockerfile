FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend-price-comparison.sln", "."]
COPY ["Backend.PriceComparison.Api/Backend.PriceComparison.Api.csproj", "Backend.PriceComparison.Api/"]
COPY ["Backend.PriceComparison.Application/Backend.PriceComparison.Application.csproj", "Backend.PriceComparison.Application/"]
COPY ["Backend.PriceComparison.Domain/Backend.PriceComparison.Domain.csproj", "Backend.PriceComparison.Domain/"]
COPY ["Backend.PriceComparison.Infraestructure.Persistence.Mysql/Backend.PriceComparison.Infraestructure.Persistence.Mysql.csproj", "Backend.PriceComparison.Infraestructure.Persistence.Mysql/"]
RUN dotnet restore "./Backend.PriceComparison.Api/Backend.PriceComparison.Api.csproj"
COPY . .
RUN dotnet build "./Backend.PriceComparison.Api/Backend.PriceComparison.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Backend.PriceComparison.Api/Backend.PriceComparison.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.PriceComparison.Api.dll"]

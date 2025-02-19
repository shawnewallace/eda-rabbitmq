FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ../. ./
RUN dotnet restore
RUN dotnet publish WarehouseConsumer/WarehouseConsumer.csproj -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WarehouseConsumer.dll"]

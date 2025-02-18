FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY WarehouseConsumer/WarehouseConsumer.csproj WarehouseConsumer/
COPY Core/Core.csproj Core/
COPY Services/Services.csproj Services/
RUN dotnet restore "WarehouseConsumer/WarehouseConsumer.csproj"
COPY . .
WORKDIR "/src/WarehouseConsumer"
RUN dotnet build "WarehouseConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WarehouseConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarehouseConsumer.dll"]

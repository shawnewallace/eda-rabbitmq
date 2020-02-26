FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ShippingConsumer/ShippingConsumer.csproj ShippingConsumer/
COPY Core/Core.csproj Core/
COPY Services/Services.csproj Services/
RUN dotnet restore "ShippingConsumer/ShippingConsumer.csproj"
COPY . .
WORKDIR "/src/ShippingConsumer"
RUN dotnet build "ShippingConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShippingConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShippingConsumer.dll"]

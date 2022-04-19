FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY InvoicingConsumer/InvoicingConsumer.csproj InvoicingConsumer/
COPY Core/Core.csproj Core/
COPY Services/Services.csproj Services/
RUN dotnet restore "InvoicingConsumer/InvoicingConsumer.csproj"
COPY . .
WORKDIR "/src/InvoicingConsumer"
RUN dotnet build "InvoicingConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InvoicingConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvoicingConsumer.dll"]

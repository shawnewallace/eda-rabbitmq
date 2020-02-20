FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY LoggingConsumer/LoggingConsumer.csproj LoggingConsumer/
COPY Core/Core.csproj Core/
COPY Services/Services.csproj Services/
RUN dotnet restore "LoggingConsumer/LoggingConsumer.csproj"
COPY . .
WORKDIR "/src/LoggingConsumer"
RUN dotnet build "LoggingConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoggingConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingConsumer.dll"]

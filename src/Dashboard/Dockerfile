FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Dashboard/Dashboard.csproj Dashboard/
COPY Core/Core.csproj Core/
COPY Services/Services.csproj Services/
RUN dotnet restore "Dashboard/Dashboard.csproj"
COPY . .
WORKDIR "/src/Dashboard"
RUN dotnet build "Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dashboard.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "Authorization/AuthorizationAPI/nuget.config"
RUN dotnet restore "Authorization/AuthorizationAPI/AuthorizationAPI.csproj" --configfile "Authorization/AuthorizationAPI/nuget.config"
WORKDIR "/src/Authorization/AuthorizationAPI"
RUN dotnet build "AuthorizationAPI.csproj" --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthorizationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YardLight.AuthorizationAPI.dll"]
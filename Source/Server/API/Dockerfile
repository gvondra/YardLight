FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Source/Server/API"
RUN dotnet nuget add source "/src/NugetPackages" -n "LocalPackages" --configfile "nuget.config"
RUN dotnet build "API.csproj" -c Release -r linux-x64 --no-self-contained --configfile "nuget.config"

FROM build AS publish
RUN dotnet publish "API.csproj" --no-restore --no-build -c Release  -r linux-x64 --no-self-contained -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YardLight.API.dll"]
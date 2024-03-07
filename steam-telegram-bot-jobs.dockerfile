FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish "SteamTelegramBot.Jobs/SteamTelegramBot.Jobs.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN apt-get update

ENTRYPOINT ["dotnet", "SteamTelegramBot.Jobs.dll"]
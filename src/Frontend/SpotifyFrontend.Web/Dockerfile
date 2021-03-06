#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Frontend/SpotifyFrontend.Web/SpotifyFrontend.Web.csproj", "Frontend/SpotifyFrontend.Web/"]
COPY ["Shared/Spotify.Shared/Spotify.Shared.csproj", "Shared/Spotify.Shared/"]
RUN dotnet restore "Frontend/SpotifyFrontend.Web/SpotifyFrontend.Web.csproj"
COPY . .
WORKDIR "/src/Frontend/SpotifyFrontend.Web"
RUN dotnet build "SpotifyFrontend.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotifyFrontend.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotifyFrontend.Web.dll"]
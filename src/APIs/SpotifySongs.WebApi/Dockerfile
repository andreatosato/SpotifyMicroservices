#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["APIs/SpotifySongs.WebApi/SpotifySongs.WebApi.csproj", "APIs/SpotifySongs.WebApi/"]
COPY ["Shared/Spotify.Shared/Spotify.Shared.csproj", "Spotify.Shared/"]
RUN dotnet restore "APIs/SpotifySongs.WebApi/SpotifySongs.WebApi.csproj"
COPY . .
WORKDIR "/src/APIs/SpotifySongs.WebApi"
RUN dotnet build "SpotifySongs.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotifySongs.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotifySongs.WebApi.dll"]
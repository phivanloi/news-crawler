#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Pl.Crawler.Downloader/Pl.Crawler.Downloader.csproj", "Pl.Crawler.Downloader/"]
COPY ["Pl.Crawler.Core/Pl.Crawler.Core.csproj", "Pl.Crawler.Core/"]
COPY ["Pl.Crawler.Data/Pl.Crawler.Data.csproj", "Pl.Crawler.Data/"]
COPY ["Pl.Crawelr.Caching/Pl.Crawelr.Caching.csproj", "Pl.Crawelr.Caching/"]
COPY ["Pl.Crawler.MessageQueue/Pl.Crawler.MessageQueue.csproj", "Pl.Crawler.MessageQueue/"]
RUN dotnet restore "Pl.Crawler.Downloader/Pl.Crawler.Downloader.csproj"
COPY . .
WORKDIR "/src/Pl.Crawler.Downloader"
RUN dotnet build "Pl.Crawler.Downloader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pl.Crawler.Downloader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pl.Crawler.Downloader.dll"]

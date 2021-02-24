#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/CloudEng.PingPong.Player/CloudEng.PingPong.Player.csproj", "src/CloudEng.PingPong.Player/"]
RUN dotnet restore "src/CloudEng.PingPong.Player/CloudEng.PingPong.Player.csproj"
COPY . .
WORKDIR "/src/src/CloudEng.PingPong.Player"
RUN dotnet build "CloudEng.PingPong.Player.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudEng.PingPong.Player.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudEng.PingPong.Player.dll"]
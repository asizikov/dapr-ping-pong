#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/CloudEng.PingPong.GameManager/CloudEng.PingPong.GameManager.csproj", "src/CloudEng.PingPong.GameManager/"]
COPY ["src/CloudEng.PingPong.Messaging/CloudEng.PingPong.Messaging.csproj", "src/CloudEng.PingPong.Messaging/"]
RUN dotnet restore "src/CloudEng.PingPong.GameManager/CloudEng.PingPong.GameManager.csproj"
COPY . .
WORKDIR "/src/src/CloudEng.PingPong.GameManager"
RUN dotnet build "CloudEng.PingPong.GameManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudEng.PingPong.GameManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudEng.PingPong.GameManager.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TranqService/TranqService.csproj", "TranqService/"]
RUN dotnet restore "TranqService/TranqService.csproj"
COPY . .
WORKDIR "/src/TranqService"
RUN dotnet build "TranqService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TranqService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TranqService.dll"]
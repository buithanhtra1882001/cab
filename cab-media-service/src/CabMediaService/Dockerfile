#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CabMediaService/CabMediaService.csproj", "CabMediaService/"]
COPY ["BuildingBlocks/EventBus/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus/"]
COPY ["BuildingBlocks/EventBus/EventBusRegistration/EventBusRegistration.csproj", "BuildingBlocks/EventBus/EventBusRegistration/"]
COPY ["BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj", "BuildingBlocks/EventBus/EventBusServiceBus/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj", "BuildingBlocks/WebHost.Customization/"]
RUN dotnet restore "CabMediaService/CabMediaService.csproj"
COPY . .
WORKDIR "/src/CabMediaService"
RUN dotnet build "CabMediaService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CabMediaService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CabMediaService.dll"]
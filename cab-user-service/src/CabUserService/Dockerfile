#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CabUserService/CabUserService.csproj", "CabUserService/"]
COPY ["BuildingBlocks/EventBus/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus/"]
COPY ["BuildingBlocks/EventBus/EventBusRegistration/EventBusRegistration.csproj", "BuildingBlocks/EventBus/EventBusRegistration/"]
COPY ["BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj", "BuildingBlocks/EventBus/EventBusServiceBus/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj", "BuildingBlocks/WebHost.Customization/"]
RUN dotnet restore "CabUserService/CabUserService.csproj"
COPY . .
WORKDIR "/src/CabUserService"
RUN dotnet build "CabUserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CabUserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CabUserService.dll"]
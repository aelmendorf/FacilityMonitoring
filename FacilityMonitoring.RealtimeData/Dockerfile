#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FacilityMonitoring.RealtimeData/FacilityMonitoring.RealtimeData.csproj", "FacilityMonitoring.RealtimeData/"]
RUN dotnet restore "FacilityMonitoring.RealtimeData/FacilityMonitoring.RealtimeData.csproj"
COPY . .
WORKDIR "/src/FacilityMonitoring.RealtimeData"
RUN dotnet build "FacilityMonitoring.RealtimeData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacilityMonitoring.RealtimeData.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacilityMonitoring.RealtimeData.dll"]
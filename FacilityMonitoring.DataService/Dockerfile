#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FacilityMonitoring.DataService/FacilityMonitoring.DataService.csproj", "FacilityMonitoring.DataService/"]
RUN dotnet restore "FacilityMonitoring.DataService/FacilityMonitoring.DataService.csproj"
COPY . .
WORKDIR "/src/FacilityMonitoring.DataService"
RUN dotnet build "FacilityMonitoring.DataService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacilityMonitoring.DataService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacilityMonitoring.DataService.dll"]
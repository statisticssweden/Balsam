#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.8-alpine3.15-amd64 AS base
WORKDIR /app

RUN apk add --no-cache icu-libs curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0.400-1-alpine3.16-amd64 AS build
WORKDIR /src
COPY ["Balsam.Api/Balsam.Api.csproj", "Balsam.Api/"]
RUN dotnet restore "Balsam.Api/Balsam.Api.csproj"
COPY . .
WORKDIR "/src/Balsam.Api"
RUN dotnet build "Balsam.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Balsam.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Balsam.Api.dll"]
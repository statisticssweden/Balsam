#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.8-alpine3.15-amd64 AS base
WORKDIR /app

RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY GitLabProvider/certs/SCB_ROOT_CA6.crt /usr/local/share/ca-certificates/SCB_ROOT_CA6.crt
COPY GitLabProvider/certs/SCB_SUB_CA6.crt /usr/local/share/ca-certificates/SCB_SUB_CA6.crt

RUN cat /usr/local/share/ca-certificates/SCB_ROOT_CA6.crt >> /etc/ssl/certs/ca-certificates.crt
RUN cat /usr/local/share/ca-certificates/SCB_SUB_CA6.crt >> /etc/ssl/certs/ca-certificates.crt

#EXPOSE 80
#EXPOSE 443

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0.400-1-alpine3.16-amd64 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "GitLabProvider/GitLabProvider.csproj"

WORKDIR "/src/GitLabProvider"
RUN dotnet build "GitLabProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GitLabProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GitLabProvider.dll"]
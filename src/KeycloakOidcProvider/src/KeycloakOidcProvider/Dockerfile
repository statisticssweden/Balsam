FROM mcr.microsoft.com/dotnet/aspnet:6.0.8-alpine3.15-amd64 AS base
WORKDIR /app

RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY KeycloakOidcProvider/certs/SCB_ROOT_CA6.crt /usr/local/share/ca-certificates/SCB_ROOT_CA6.crt
COPY KeycloakOidcProvider/certs/SCB_SUB_CA6.crt /usr/local/share/ca-certificates/SCB_SUB_CA6.crt
COPY KeycloakOidcProvider/certs/AI_SWEDEN.crt /usr/local/share/ca-certificates/AI_SWEDEN.crt

RUN cat /usr/local/share/ca-certificates/SCB_ROOT_CA6.crt >> /etc/ssl/certs/ca-certificates.crt
RUN cat /usr/local/share/ca-certificates/SCB_SUB_CA6.crt >> /etc/ssl/certs/ca-certificates.crt
RUN cat /usr/local/share/ca-certificates/AI_SWEDEN.crt >> /etc/ssl/certs/ca-certificates.crt

#EXPOSE 80
#EXPOSE 443

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0.400-1-alpine3.16-amd64 AS build
WORKDIR /src
COPY ["KeycloakOidcProvider/Keycloak.OidcProvider.csproj", "KeycloakOidcProvider/"]
RUN dotnet restore "KeycloakOidcProvider/Keycloak.OidcProvider.csproj"
COPY . .
WORKDIR "/src/KeycloakOidcProvider"
RUN dotnet build "Keycloak.OidcProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Keycloak.OidcProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keycloak.OidcProvider.dll"]
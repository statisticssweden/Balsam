:: Generated by: https://openapi-generator.tech
::

@echo off

dotnet restore src\OidcProvider
dotnet build src\OidcProvider
echo Now, run the following to start the project: dotnet run -p src\OidcProvider\OidcProvider.csproj --launch-profile web.
echo.
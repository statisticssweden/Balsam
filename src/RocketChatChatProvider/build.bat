:: Generated by: https://openapi-generator.tech
::

@echo off

dotnet restore src\ChatProvider
dotnet build src\ChatProvider
echo Now, run the following to start the project: dotnet run -p src\ChatProvider\ChatProvider.csproj --launch-profile web.
echo.

cd /D %~dp0

@echo off

echo Please select an option:
echo 1. Client
echo 2. Server
echo 3. All

set /p choice=Please select an option:

if "%choice%"=="1" (
    echo Client
    cd /D %~dp0/Exporter/Exporter_Build/
    dotnet Exporter.dll --ExportPlatform 1
) else if "%choice%"=="2" (
    echo Server
    cd /D %~dp0/Exporter/Exporter_Build/
    dotnet Exporter.dll --ExportPlatform 2
) else if "%choice%"=="3" (
    echo All
    cd /D %~dp0/Exporter/Exporter_Build/
    dotnet Exporter.dll --ExportPlatform 3
) else (
    echo Invalid option
)

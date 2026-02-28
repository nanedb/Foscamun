@echo off
REM Foscamun Quick Deployment Script
REM Crea una versione Windows x64 self-contained

echo ==============================================
echo    Foscamun Quick Deployment
echo ==============================================
echo.

REM Pulisci directory precedente
if exist ".\Publish\win-x64" rmdir /s /q ".\Publish\win-x64"
mkdir ".\Publish\win-x64" 2>nul

echo Building Windows 64-bit version...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o .\Publish\win-x64

if %errorlevel% equ 0 (
    echo.
    echo ==============================================
    echo    Build completata!
    echo ==============================================
    echo.
    echo File disponibili in: .\Publish\win-x64\
    echo.
    explorer .\Publish\win-x64
) else (
    echo.
    echo ERRORE durante il build!
    echo.
    pause
)

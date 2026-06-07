@echo off
chcp 65001 >nul
cd /d "%~dp0\FarshGenerator"
start "FarshGenerator Server" cmd /k "dotnet run --launch-profile https"
timeout /t 6 /nobreak >nul
start https://localhost:7153
@echo off
chcp 65001 >nul
cd /d "%~dp0"

if not exist "desktop\PIM.Desktop.exe" goto faltando

where curl >nul 2>&1
if errorlevel 1 goto abrir
curl -s -f -o nul --max-time 2 http://localhost:5080/health
if errorlevel 1 goto servidor_parado

:abrir
start "" /D "%~dp0desktop" "%~dp0desktop\PIM.Desktop.exe"
exit /b 0

:servidor_parado
echo.
echo  O servidor do TechSolve não está no ar.
echo  Abra primeiro o "Iniciar TechSolve.bat" e depois este arquivo.
echo.
pause
exit /b 1

:faltando
echo.
echo  ERRO: a pasta "desktop" precisa estar ao lado deste arquivo.
echo.
pause
exit /b 1

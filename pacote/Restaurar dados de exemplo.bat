@echo off
chcp 65001 >nul
title TechSolve - Restaurar dados de exemplo
cd /d "%~dp0"

echo.
echo  Restaurar dados de exemplo
echo  --------------------------
echo  Apaga tudo o que foi criado durante os testes (chamados, usuários,
echo  conversas e anexos) e deixa o sistema como veio, com os dados de exemplo.
echo  As datas dos chamados de exemplo passam a contar a partir de hoje.
echo.
choice /C SN /N /M " Deseja continuar? (S/N) "
if errorlevel 2 goto cancelado

taskkill /IM PIM.Web.exe /F >nul 2>&1
taskkill /IM PIM.Api.exe /F >nul 2>&1
ping -n 3 127.0.0.1 >nul

del /q "api\techsolve.db" "api\techsolve.db-shm" "api\techsolve.db-wal" 2>nul
if exist "api\wwwroot\uploads" rmdir /s /q "api\wwwroot\uploads"

if exist "api\techsolve.db" goto falha

echo.
echo  Pronto. Abra o "Iniciar TechSolve.bat" para subir o sistema com os dados de exemplo.
echo.
pause
exit /b 0

:cancelado
echo.
echo  Nada foi alterado.
ping -n 3 127.0.0.1 >nul
exit /b 0

:falha
echo.
echo  ERRO: não foi possível apagar o banco de dados.
echo  Feche as janelas do TechSolve e tente de novo.
echo.
pause
exit /b 1

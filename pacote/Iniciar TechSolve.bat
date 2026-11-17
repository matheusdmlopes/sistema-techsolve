@echo off
chcp 65001 >nul
title TechSolve
cd /d "%~dp0"

echo.
echo  ==============================================
echo    TechSolve - Sistema de Chamados
echo  ==============================================
echo.

if not exist "api\PIM.Api.exe" goto faltando
if not exist "site\PIM.Web.exe" goto faltando

rem Fecha uma execução anterior que tenha ficado aberta.
taskkill /IM PIM.Web.exe /F >nul 2>&1
taskkill /IM PIM.Api.exe /F >nul 2>&1

echo  [1/2] Iniciando o servidor e o banco de dados...
start "TechSolve - Servidor" /D "%~dp0api" /MIN "%~dp0api\PIM.Api.exe"
call :aguardar http://localhost:5080/health
if errorlevel 1 goto falha_servidor

echo  [2/2] Iniciando o site...
start "TechSolve - Site" /D "%~dp0site" /MIN "%~dp0site\PIM.Web.exe"
call :aguardar http://localhost:5000/
if errorlevel 1 goto falha_site

start "" "http://localhost:5000"

echo.
echo  Sistema no ar. O site foi aberto no navegador.
echo.
echo    Site ........ http://localhost:5000
echo    Desktop ..... abra o arquivo "Abrir Desktop.bat"
echo    Celular ..... no aplicativo, informe o endereço do servidor:
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /C:"IPv4"') do for /f "tokens=* delims= " %%b in ("%%a") do echo                    http://%%b:5080
echo.
echo    Administrador .. admin@empresa.com        senha: admin123
echo    Técnico ........ joao.silva@empresa.com   senha: senha123
echo    Usuário ........ pedro.costa@empresa.com  senha: senha123
echo.
echo  ----------------------------------------------
echo   Deixe esta janela aberta durante a apresentação.
echo   Para encerrar o sistema, pressione qualquer tecla aqui.
echo  ----------------------------------------------
pause >nul

taskkill /IM PIM.Web.exe /F >nul 2>&1
taskkill /IM PIM.Api.exe /F >nul 2>&1
echo.
echo  Sistema encerrado.
ping -n 3 127.0.0.1 >nul
exit /b 0


:aguardar
rem Espera até 60 segundos o endereço informado responder.
set /a tentativas=0
:aguardar_repetir
where curl >nul 2>&1
if errorlevel 1 goto aguardar_powershell
curl -s -f -o nul --max-time 2 %1
if not errorlevel 1 exit /b 0
goto aguardar_proxima
:aguardar_powershell
powershell -NoProfile -Command "try { Invoke-WebRequest -UseBasicParsing -TimeoutSec 2 '%1' | Out-Null; exit 0 } catch { exit 1 }"
if not errorlevel 1 exit /b 0
:aguardar_proxima
set /a tentativas+=1
if %tentativas% geq 60 exit /b 1
rem A pausa usa o ping porque o comando timeout falha quando a janela não tem teclado.
ping -n 2 127.0.0.1 >nul
goto aguardar_repetir


:faltando
echo  ERRO: as pastas "api" e "site" precisam estar ao lado deste arquivo.
echo.
echo  Extraia o arquivo .zip inteiro para uma pasta, como a Área de Trabalho,
echo  e abra o "Iniciar TechSolve.bat" de dentro da pasta extraída.
echo.
pause
exit /b 1

:falha_servidor
echo.
echo  ERRO: o servidor não respondeu.
echo.
echo  O que conferir:
echo    1. Abra a janela "TechSolve - Servidor" na barra de tarefas e leia a mensagem de erro.
echo    2. A pasta do TechSolve precisa estar em um lugar onde você pode gravar arquivos,
echo       como a Área de Trabalho ou Documentos.
echo    3. Outro programa pode estar usando a porta 5080. Reinicie o computador e tente de novo.
echo.
pause
exit /b 1

:falha_site
echo.
echo  ERRO: o site não respondeu.
echo.
echo  O que conferir:
echo    1. Abra a janela "TechSolve - Site" na barra de tarefas e leia a mensagem de erro.
echo    2. Outro programa pode estar usando a porta 5000. Reinicie o computador e tente de novo.
echo.
taskkill /IM PIM.Api.exe /F >nul 2>&1
pause
exit /b 1

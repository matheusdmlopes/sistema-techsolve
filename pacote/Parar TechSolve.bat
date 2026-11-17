@echo off
chcp 65001 >nul
title TechSolve - Parar

taskkill /IM PIM.Web.exe /F >nul 2>&1
taskkill /IM PIM.Api.exe /F >nul 2>&1

echo.
echo  Sistema encerrado.
ping -n 3 127.0.0.1 >nul

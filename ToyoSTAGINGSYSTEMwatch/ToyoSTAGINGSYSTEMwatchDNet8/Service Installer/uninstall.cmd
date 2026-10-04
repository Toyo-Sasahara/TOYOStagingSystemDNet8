@echo off
setlocal

set "SERVICE_NAME=ToyoSTAGINGSYSTEMwatchService"

echo Stopping and removing %SERVICE_NAME% service...

sc stop "%SERVICE_NAME%"
sc delete "%SERVICE_NAME%"

echo.
echo Service uninstalled successfully.
pause

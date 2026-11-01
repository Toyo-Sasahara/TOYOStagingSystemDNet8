@echo off
setlocal

for /f "tokens=2 delims=:." %%I in ('chcp') do set "OLD_CODEPAGE=%%I"
set "OLD_CODEPAGE=%OLD_CODEPAGE: =%"
chcp 65001 >nul

set "SERVICE_NAME=TOYONETPRINTSVC"

echo Stopping and removing %SERVICE_NAME% service...

sc stop "%SERVICE_NAME%"
sc delete "%SERVICE_NAME%"

echo.
echo Service uninstalled successfully.
chcp %OLD_CODEPAGE% >nul
endlocal
pause

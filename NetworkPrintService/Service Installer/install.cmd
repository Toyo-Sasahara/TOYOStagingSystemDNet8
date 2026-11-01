@echo off
setlocal

for /f "tokens=2 delims=:." %%I in ('chcp') do set "OLD_CODEPAGE=%%I"
set "OLD_CODEPAGE=%OLD_CODEPAGE: =%"
chcp 65001 >nul

set "SERVICE_NAME=TOYONETPRINTSVC"
set "DISPLAY_NAME=東陽ネットワークプリントサービス"
set "DESCRIPTION=東陽ネットワークプリントサービス"

for %%I in ("%~dp0..") do set "PUBLISH_DIR=%%~fI"
set "EXE_PATH=%PUBLISH_DIR%\NetworkPrintService.exe"

if not exist "%EXE_PATH%" (
	echo NetworkPrintService.exe was not found: "%EXE_PATH%"
	chcp %OLD_CODEPAGE% >nul
	endlocal
	exit /b 1
)

sc create "%SERVICE_NAME%" binPath= "\"%EXE_PATH%\"" start= auto obj= "NT AUTHORITY\NetworkService" DisplayName= "%DISPLAY_NAME%"
sc description "%SERVICE_NAME%" "%DESCRIPTION%"
sc failure "%SERVICE_NAME%" actions= restart/6000/restart/60000/restart/600000 reset= 86400
icacls "%PUBLISH_DIR%" /grant "NT AUTHORITY\NetworkService:(OI)(CI)RX" /T

echo.
echo Service installed successfully.
chcp %OLD_CODEPAGE% >nul
endlocal
pause

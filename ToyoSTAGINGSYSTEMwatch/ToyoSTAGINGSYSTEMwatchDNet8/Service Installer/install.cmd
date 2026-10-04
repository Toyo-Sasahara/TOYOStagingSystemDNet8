@echo off
setlocal

set "SERVICE_NAME=ToyoSTAGINGSYSTEMwatchService"
set "DISPLAY_NAME=ToyoSTAGINGSYSTEMwatch Service"
set "DESCRIPTION=ToyoSTAGINGSYSTEMwatch  service"

for %%I in ("%~dp0..") do set "PUBLISH_DIR=%%~fI"
set "EXE_PATH=%PUBLISH_DIR%\ToyoSTAGINGSYSTEMwatchDNet8.exe"

if not exist "%EXE_PATH%" (
    echo ToyoSTAGINGSYSTEMwatchDNet8.exe was not found: "%EXE_PATH%"
    exit /b 1
)

sc create "%SERVICE_NAME%" binPath= "\"%EXE_PATH%\"" start= auto obj= "NT AUTHORITY\NetworkService" DisplayName= "%DISPLAY_NAME%"
sc description "%SERVICE_NAME%" "%DESCRIPTION%"
sc failure "%SERVICE_NAME%" actions= restart/6000/restart/60000/restart/600000 reset= 86400
icacls "%PUBLISH_DIR%" /grant "NT AUTHORITY\NetworkService:(OI)(CI)RX" /T

echo.
echo Service installed successfully.
pause

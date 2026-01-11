@ECHO OFF
SET DRGETATTRCMD="C:\Program Files (x86)\Fuji Xerox\ArcSuite\Tool\dRepTools\bin\drgetattr.bat"


SET ASUSER=%1
SET ASPASS=%2
SET ZUBAN=%3

(
ECHO."targetServiceId","targetCabinetId","targetSearchFile","targetCondition"
ECHO.ass1,3e6f0b6f002b,,"user:zuban LIKE ""%ZUBAN%"" system:latestEditionFlag true"
)>%TEMP%\drgetattr.att

(
ECHO.^<?xml version="1.0" encoding="Shift_JIS" ?^>
ECHO.^<parameter^>
ECHO.^<logFile^>%TEMP%\drgetattr.log^</logFile^>
ECHO.^<resultFile^>%TEMP%\drgetattr.result^</resultFile^>
ECHO.^<getAtomId^>user:zuban^|user:torokubi^</getAtomId^>
ECHO.^</parameter^>
)>%TEMP%\drgetattr.xml

CALL %DRGETATTRCMD% -param %TEMP%\drgetattr.xml -csv %TEMP%\drgetattr.att -user %ASUSER% -passwd %ASPASS%
type %TEMP%\drgetattr.result
REM del %TEMP%\drgetattr.att
REM del %TEMP%\drgetattr.xml
REM del %TEMP%\drgetattr.result
REM del %TEMP%\drgetattr.log
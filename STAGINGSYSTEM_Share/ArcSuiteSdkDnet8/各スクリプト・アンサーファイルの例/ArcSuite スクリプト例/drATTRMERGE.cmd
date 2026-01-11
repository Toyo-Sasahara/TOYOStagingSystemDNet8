REM アークスイート属性変更コマンド
REM マージ：ドキュメント管理サービスに登録済みのオブジェクトが持つ属性値を変更します。
REM %1 ユーザーID
REM %2 パスワード
REM %3 図面番号(user:zuban) ""で囲みません
REM %4 属性名(例：user:status_explanation) ""で囲みません
REM %5 属性値文字列 ""で文字列を囲むこと　ブール値は、true か false　を指定


@ECHO OFF
SET DRREGISTCMD="C:\Program Files (x86)\Fuji Xerox\ArcSuite\Tool\dRepTools\bin\drregist.bat"


SET ASUSER=%1
SET ASPASS=%2

SET ZUBAN=%3
REM %3は""で囲みません
REM TR-6021-001

SET ATTRIBUTENAME=%4
REM %4は""で囲みません
REM 
REM user:status_explanation 状態変更理由
REM user:partName 部品名
REM user:material 材質
REM user:Model3D
REM user:Tourokubi 登録日
REM user:customername 顧客名

SET ATTRIBUTEVALUE=%5
REM %5は文字列は""で囲みます。
REM ブール値は、true か false　を指定

(
ECHO.entryMode,targetLatestEdition,%ATTRIBUTENAME%
ECHO.merge,user:zuban "%ZUBAN%",%ATTRIBUTEVALUE%
)>%TEMP%\drregist.csv

(
ECHO.^<?xml version="1.0" encoding="Shift_JIS" ?^>
ECHO.    ^<drregistParameter^>
ECHO.    ^<executionParameter^>
ECHO.      ^<logFile^>%TEMP%\drregist.log^</logFile^>
ECHO.      ^<logMode^>all^</logMode^>
ECHO.      ^<encoding^>Shift_JIS^</encoding^>
ECHO.      ^<entryErrorIgnore^>true^</entryErrorIgnore^>
ECHO.    ^</executionParameter^>
ECHO.  ^<defaultParameter^>
ECHO.    ^<locationServiceId^>ass1^</locationServiceId^>
ECHO.    ^<locationCabinetId^>3e6f0b6f002b^</locationCabinetId^>
ECHO.  ^</defaultParameter^>
ECHO.^</drregistParameter^>
)>%TEMP%\drregist.xml

del %TEMP%\drregist.log

CALL  %DRREGISTCMD% -param %TEMP%\drregist.xml -csv %TEMP%\drregist.csv -user %ASUSER% -passwd %ASPASS%

type %TEMP%\drregist.log


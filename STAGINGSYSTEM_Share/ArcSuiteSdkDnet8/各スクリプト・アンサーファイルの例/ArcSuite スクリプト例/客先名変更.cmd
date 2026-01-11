@echo off
echo %~nx0 図面番号 客先名 ユーザーID パスワード
call %~dp0\drATTRchange.cmd  %3 %4 %1 user:customername "%~2"

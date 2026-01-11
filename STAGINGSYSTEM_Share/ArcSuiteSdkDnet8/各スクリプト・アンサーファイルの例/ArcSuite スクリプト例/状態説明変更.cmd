@echo off
echo %~nx0 図面番号 状態変更理由 ユーザーID パスワード
call %~dp0\drATTRchange.cmd %3 %4 %1 user:status_explanation "%~2"


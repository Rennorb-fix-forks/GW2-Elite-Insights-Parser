@echo off
setlocal EnableDelayedExpansion
pushd "%~dp0"

WHERE protoc >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    SET MISSING=protoc
    GOTO ERROR
)

WHERE npm >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    SET MISSING=npm
    GOTO ERROR
)

ECHO compiling .proto to .cs ...
call protoc --csharp_out=. -I=. EXTHealingStats.proto

ECHO compiling .proto to .js ...
pushd ..\Resources\_compiler
call npm install --no-fund --no-audit
call npm run compile

ECHO OK.


EXIT /B

:ERROR
ECHO.
ECHO [WARN] "!MISSING!" does not exist within the path, make sure you have it installed and accessible. .proto files will not be recompiled!
ECHO.

@echo off
setlocal
pushd "%~dp0"

SET ROOT_PB=EXTHealingStats.proto
:: NOTE(Rennorb): It's arbitrary that one is a path and the other one is global.
SET PROTOC=G:\privat\programms\protoc\bin\protoc.exe

IF NOT EXIST "%PROTOC%" (
    ECHO "%PROTOC%" does not exist, make sure this points to a protoc binary.
    EXIT /B 1
)

WHERE npm >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    ECHO "npm" does not exist within the path, make sure you have it installed.
    EXIT /B 1
)

call npm list -g protobufjs-cli >nul 2>nul
IF %ERRORLEVEL% NEQ 0 (
    ECHO "protobufjs-cli" is not gloablly installed into node, make sure you install it by calling "npm install -g protobufjs-cli".
    EXIT /B 1
)

ECHO compiling .proto to .cs ...
call "%PROTOC%" --csharp_out=. -I=. %ROOT_PB%

ECHO compiling .proto to .json ...
call pbjs -t static-module -w closure --es6 ^
    --keep-case --no-create --no-encode --no-verify --no-beautify --no-comments --no-service --no-delimited -l "" ^
    -p . -o ../Resources/logData.proto.js %ROOT_PB%
::TODO(Rennorb) auto-strip 'export const GW2EIBuilders = ' from the result

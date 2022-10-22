@ECHO OFF
SET ThisScriptsDirectory=%~dp0
SET PowerShellScriptPath=%ThisScriptsDirectory%upgradecodeuninstall.ps1
%WINDIR%\SysNative\WindowsPowershell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File """"%PowerShellScriptPath%"""" """"{0123A210-9B73-46E7-B5CE-7F33630300E7}""""' -Verb RunAs}"
Cd /d %~dp0
echo %CD%

set WORKSPACE=..\..

set LUBAN_DLL=%WORKSPACE%\Tools\Luban\LubanRelease\Luban.dll
set CONF_ROOT=%WORKSPACE%\Config\Excel\GameConfig

::Client
dotnet %LUBAN_DLL% ^
    --customTemplateDir CustomTemplate ^
    -t Client ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\__luban__.conf ^
    -x outputCodeDir=%WORKSPACE%\Unity\Assets\GameScripts\GameProto\Generate\Config ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\GameConfig\c ^
    -x lineEnding=CRLF ^
    


echo ==================== FuncConfig : GenClientFinish ====================

pause
Cd /d %~dp0
echo %CD%

set WORKSPACE=..\..

set LUBAN_DLL=%WORKSPACE%\Tools\Luban\LubanRelease\Luban.dll
set CONF_ROOT=%WORKSPACE%\Config\Excel




::Server
dotnet %LUBAN_DLL% ^
    --customTemplateDir ServerTemplate ^
    -t All ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\__luban__.conf ^
    -x outputCodeDir=%WORKSPACE%\Server\Model\Generate\Config ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\ ^
    -x lineEnding=CRLF ^
    

echo ==================== FuncConfig : GenServerFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

::StartConfig Release
dotnet %LUBAN_DLL% ^
    --customTemplateDir ServerTemplate ^
    -t Release ^
    -c cs-bin ^
    -d bin ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x outputCodeDir=%WORKSPACE%\Server\Model\Generate\Config\StartConfig ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\StartConfig\Release ^
    -x lineEnding=CRLF ^
    

echo ==================== StartConfig : GenReleaseFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

::StartConfig Benchmark
dotnet %LUBAN_DLL% ^
    --customTemplateDir ServerTemplate ^
    -t Benchmark ^
    -d bin ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\StartConfig\Benchmark ^
    

echo ==================== StartConfig : GenBenchmarkFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

::StartConfig Localhost
dotnet %LUBAN_DLL% ^
    --customTemplateDir ServerTemplate ^
    -t Localhost ^
    -d bin ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\StartConfig\Localhost ^
    

echo ==================== StartConfig : GenLocalhostFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)

::StartConfig RouterTest
dotnet %LUBAN_DLL% ^
    --customTemplateDir ServerTemplate ^
    -t RouterTest ^
    -d bin ^
    --conf %CONF_ROOT%\StartConfig\__luban__.conf ^
    -x bin.outputDataDir=%WORKSPACE%\Config\Generate\StartConfig\RouterTest ^
    

echo ==================== StartConfig : GenRouterTestFinish ====================

if %ERRORLEVEL% NEQ 0 (
    echo An error occurred, press any key to exit.
    pause
    exit /b
)
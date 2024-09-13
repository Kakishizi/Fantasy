set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Core\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=..\..\Unity\Assets\MMORes\Luban\data^
    -x outputCodeDir=..\..\Unity\Assets\Scripts\Hotfix\Generate\LubanCode
pause
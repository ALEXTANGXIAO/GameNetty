@echo off
setlocal

@echo off
setlocal

set "targetDir=%~dp0"

for /r "%targetDir%" %%i in (*.meta) do (
    del "%%i"
)

for /d /r "%targetDir%" %%i in (*) do (
    for %%j in ("%%i\*.meta") do (
        del "%%j"
    )
)

echo All .meta files have been deleted.

pause
exit
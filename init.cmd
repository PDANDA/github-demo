echo off
for /D %%d in (*) do (
    cd %%d
    if exist ".\init.cmd" (
        init.cmd
    )
    cd ..
)
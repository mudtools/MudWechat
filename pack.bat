@echo off
REM ===============================================================
REM Mud.Wechat pack-only script
REM
REM Usage: pack.bat [version] [/nopause]
REM   [version]  Package version (e.g. 1.0.3). Default: <Version> from
REM              Directory.Build.props.
REM   /nopause   Do not pause at the end (for scripting / CI).
REM
REM Packs every packable SDK source project into .\artifacts. No quality
REM gate, no tests, no push - build validation is your own responsibility.
REM ===============================================================
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "OUTPUT_DIR=artifacts"
set "SOLUTION=Mud.Wechat.slnx"
REM The packable set is **derived on the fly** from Src\**\*.csproj (a project carrying
REM <IsPackable>false</IsPackable> is a build-time tool and produces no package), and every
REM derived project is then checked for its .nupkg in the VERIFY section below.
REM No hard-coded name list on purpose: the list and the project tree drifting apart is
REM exactly what went stale twice (CI stuck at 18, this file stuck at 11). Single source
REM of truth shared with the CI package-count guard and AB-G6 / AB-G7.
REM A package disappears only via a missing slnx entry, IsPackable=false, or an
REM unresolvable literal asset path inside it - see AB-G7.
set "VERSION=%~1"
set "NO_PAUSE=%~2"
set "EC=0"

where dotnet >nul 2>nul || (
    echo Error: 'dotnet' was not found. Install the .NET SDK first.
    set "EC=1"
    goto :done
)

if not defined VERSION (
    for /f "usebackq delims=" %%V in (`dotnet msbuild "Src\Work\Mud.Wechat.Work\Mud.Wechat.Work.csproj" -getProperty:Version -nologo`) do set "VERSION=%%V"
)
if not defined VERSION (
    echo Error: cannot resolve the package version. Pass it explicitly, e.g. pack.bat 1.0.3
    set "EC=1"
    goto :done
)
echo Package version : %VERSION%

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"
del /q "%OUTPUT_DIR%\*%VERSION%.nupkg" 2>nul

dotnet pack "%SOLUTION%" -c Release --nologo -o "%OUTPUT_DIR%" -p:Version=%VERSION%
if errorlevel 1 (
    echo Error: dotnet pack failed for %SOLUTION%.
    set "EC=1"
    goto :done
)

REM ---- VERIFY: derive the packable set from the source tree, then check each product ----
set "EXPECTED=0"
set "MISSING="
for /f "delims=" %%F in ('dir /s /b "Src\*.csproj" 2^>nul') do (
    findstr /m /c:"<IsPackable>false" "%%F" >nul 2>nul
    if errorlevel 1 (
        set /a EXPECTED+=1
        if not exist "%OUTPUT_DIR%\%%~nF.%VERSION%.nupkg" set "MISSING=!MISSING! %%~nF"
    )
)
if defined MISSING (
    echo Error: expected !EXPECTED! packages derived from Src\**\*.csproj, missing:!MISSING!
    echo          A package disappears when its csproj sets IsPackable=false,
    echo          when it drops out of %SOLUTION%, or when a literal asset path
    echo          inside it no longer resolves - see AB-G7.
    set "EC=1"
    goto :done
)

echo.
echo Produced %EXPECTED% packages in %cd%\%OUTPUT_DIR%:
dir /b "%OUTPUT_DIR%\*%VERSION%.nupkg"

:done
echo.
if "%EC%"=="0" (
    echo PACK SUCCEEDED  ^(version %VERSION%^)
) else (
    echo PACK FAILED
)
if not "%NO_PAUSE%"=="/nopause" pause
endlocal & exit /b %EC%

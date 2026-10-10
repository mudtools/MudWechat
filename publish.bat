@echo off
REM ===============================================================
REM Mud.Wechat publish
REM
REM Usage: publish.bat [version] [/preview] [/skipcheck] [/push] [/nopause]
REM   [version]   Package version (e.g. 1.0.3). Default: <Version> from
REM               Directory.Build.props.
REM   /preview    Append "-preview.<yyyyMMdd-HHmmss>" to the version.
REM   /skipcheck  Skip scripts\verify-build.ps1 quality gate (faster).
REM   /push       Push produced packages to nuget.org after packing.
REM               Requires the NUGET_API_KEY environment variable.
REM   /nopause    Do not pause at the end (for scripting / CI).
REM
REM Notes:
REM   * Every packable source project is packed into .\artifacts - six product lines
REM     (.Work.* / .OfficialAccount.* / .MiniProgram.* / .Pay.* / .OpenPlatform.*
REM     / .Ads.*) plus the shared .Abstractions leaf and .OpenTelemetry.
REM   * The **solution** drives the packing, and the expected set is **derived from
REM     Src\**\*.csproj** (a project with <IsPackable>false</IsPackable> is a build-time
REM     tool and produces no package): moving a csproj between Src/<Area> folders
REM     therefore cannot silently drop it from the release (that is exactly how the
REM     hard-coded 11-package list went stale).
REM   * Version is passed as -p:Version=... so assembly and package
REM     versions cannot drift apart (Directory.Build.props pins <Version>,
REM     which makes --version-suffix silently ignored).
REM   * The quality gate already builds the whole solution; unless you
REM     /skipcheck, a compile error anywhere aborts the run.
REM ===============================================================
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "OUTPUT_DIR=artifacts"
set "SOLUTION=Mud.Wechat.slnx"
REM No hard-coded package checklist: the expected set is derived from Src\**\*.csproj
REM in the VERIFY section below. Single source of truth shared with the CI package-count
REM guard and AB-G6 / AB-G7.

set "VERSION="
set "PREVIEW=0"
set "SKIP_CHECK=0"
set "DO_PUSH=0"
set "NO_PAUSE=0"
set "EC=0"

REM ------------------------------------------------------------- args
:parse_args
if "%~1"=="" goto :args_done
if /i "%~1"=="/preview"   ( set "PREVIEW=1"    & shift & goto :parse_args )
if /i "%~1"=="/skipcheck" ( set "SKIP_CHECK=1" & shift & goto :parse_args )
if /i "%~1"=="/push"      ( set "DO_PUSH=1"    & shift & goto :parse_args )
if /i "%~1"=="/nopause"   ( set "NO_PAUSE=1"   & shift & goto :parse_args )
if "%~1"=="/?" goto :usage
if defined VERSION (
    echo Error: version already set to "%VERSION%" ^(received "%~1"^).
    goto :usage
)
set "VERSION=%~1"
shift
goto :parse_args
:args_done

REM ---------------------------------------------------------- version
if not defined VERSION (
    for /f "usebackq delims=" %%V in (`dotnet msbuild "Src\Work\Mud.Wechat.Work\Mud.Wechat.Work.csproj" -getProperty:Version -nologo`) do set "VERSION=%%V"
)
if not defined VERSION (
    echo Error: cannot resolve the package version. Pass it explicitly, e.g. publish.bat 1.0.3
    set "EC=1"
    goto :finish
)
if "%PREVIEW%"=="1" (
    for /f "usebackq delims=" %%T in (`powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"`) do set "TS=%%T"
    if not defined TS (
        echo Error: cannot build the preview timestamp.
        set "EC=1"
        goto :finish
    )
    set "VERSION=!VERSION!-preview.!TS!"
)
echo Package version : %VERSION%

REM ---------------------------------------------------------- sanity
if not exist "Mud.Wechat.slnx" (
    echo Error: not running from the repository root ^(Mud.Wechat.slnx not found^).
    set "EC=1"
    goto :finish
)
where dotnet >nul 2>nul || (
    echo Error: 'dotnet' was not found. Install the .NET SDK first.
    set "EC=1"
    goto :finish
)

REM ----------------------------------------------------- quality gate
if "%SKIP_CHECK%"=="1" (
    echo Quality gate    : SKIPPED ^(/skipcheck^).
) else (
    echo Quality gate    : scripts\verify-build.ps1 ...
    set "PS="
    where pwsh >nul 2>nul && set "PS=pwsh"
    if not defined PS where powershell >nul 2>nul && set "PS=powershell"
    if not defined PS (
        echo Error: PowerShell ^(pwsh / powershell^) was not found.
        set "EC=1"
        goto :finish
    )
    "!PS!" -NoProfile -ExecutionPolicy Bypass -File ".\scripts\verify-build.ps1" || (
        echo Error: quality gate failed - publish aborted. Use /skipcheck only if you know what you are doing.
        set "EC=1"
        goto :finish
    )
)

REM ------------------------------------------------------------- pack
echo Packing packages ^(expected set derived from Src\**\*.csproj^) ...
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"
del /q "%OUTPUT_DIR%\*%VERSION%.nupkg" 2>nul
dotnet pack "%SOLUTION%" -c Release --nologo -o "%OUTPUT_DIR%" -p:Version=%VERSION% || (
    echo Error: dotnet pack failed for %SOLUTION%.
    set "EC=1"
    goto :finish
)

REM ---- VERIFY: derive the packable set from the source tree, then check each product ----
set "COUNT=0"
set "EXPECTED=0"
set "MISSING="
for /f "delims=" %%F in ('dir /s /b "Src\*.csproj" 2^>nul') do (
    findstr /m /c:"<IsPackable>false" "%%F" >nul 2>nul
    if errorlevel 1 (
        set /a EXPECTED+=1
        if exist "%OUTPUT_DIR%\%%~nF.%VERSION%.nupkg" ( set /a COUNT+=1 ) else set "MISSING=!MISSING! %%~nF"
    )
)
if defined MISSING (
    echo Error: expected !EXPECTED! packages derived from Src\**\*.csproj, missing:!MISSING!
    echo          A package disappears when its csproj sets IsPackable=false, when it
    echo          drops out of %SOLUTION%, or when a literal asset path inside it no
    echo          longer resolves - see AB-G7.
    set "EC=1"
    goto :finish
)
echo Produced !COUNT! of !EXPECTED! derived packages:
dir /b "%OUTPUT_DIR%\*%VERSION%.nupkg"

REM ------------------------------------------------------------- push
if "%DO_PUSH%"=="1" (
    if not defined NUGET_API_KEY (
        echo Error: /push requires the NUGET_API_KEY environment variable.
        set "EC=1"
        goto :finish
    )
    echo Pushing to nuget.org ...
    dotnet nuget push "%OUTPUT_DIR%\*.nupkg" --api-key "%NUGET_API_KEY%" --source https://api.nuget.org/v3/index.json --skip-duplicate || (
        echo Error: push failed.
        set "EC=1"
        goto :finish
    )
)

REM ----------------------------------------------------------- finish
:finish
echo.
if "%EC%"=="0" (
    echo PUBLISH SUCCEEDED  ^(version %VERSION%, output %cd%\%OUTPUT_DIR%^)
) else (
    echo PUBLISH FAILED
)
if "%NO_PAUSE%"=="0" pause
endlocal & exit /b %EC%

:usage
echo.
echo Usage: publish.bat [version] [/preview] [/skipcheck] [/push] [/nopause]
echo   [version]   Package version, e.g. 1.0.3. Default: from Directory.Build.props.
echo   /preview    Append -preview.<timestamp> to the version.
echo   /skipcheck  Skip the verify-build.ps1 quality gate.
echo   /push       Push to nuget.org after packing ^(needs NUGET_API_KEY^).
echo   /nopause    Do not pause at the end.
echo.
set "EC=1"
goto :finish
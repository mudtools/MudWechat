@echo off

REM ===============================================================
REM Auto-run Mud.Wechat project tests batch file
REM Author: Mud Studio
REM ===============================================================

echo ===============================================================
echo Starting Mud.Wechat project tests
echo Current directory: %cd%
echo Execution time: %date% %time%
echo ===============================================================

REM Check if we're in the project root directory
if not exist "Tests" (
    echo Error: Current directory is not the project root. Please run this script in the MudWechat directory
    pause
    exit /b 1
)

REM Check if dotnet command is available
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo Error: dotnet command not found. Please ensure .NET SDK is installed
    pause
    exit /b 1
)

echo 1. Running all tests with reports...
echo ===============================================================

REM Define test report directory
set TEST_REPORT_DIR=test-reports

REM Create test report directory
if not exist "%TEST_REPORT_DIR%" mkdir "%TEST_REPORT_DIR%"

REM Run the entire test suite with XML logger
dotnet test Mud.Wechat.slnx --logger "trx;LogFileName=all-tests.trx" --results-directory "%TEST_REPORT_DIR%"

REM Check test results
if %errorlevel% neq 0 (
    echo ===============================================================
    echo Test execution failed! Please check the error messages above
    echo ===============================================================
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\all-tests.trx
    pause
    exit /b 1
) else (
    echo ===============================================================
    echo All tests executed successfully!
    echo ===============================================================
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\all-tests.trx
)

REM Check if ReportGenerator is available
where ReportGenerator >nul 2>nul
if %errorlevel% equ 0 (
    echo.
    echo Generating HTML test report...
    echo ===============================================================

    REM Generate HTML report
    ReportGenerator "-reports:%TEST_REPORT_DIR%\*.trx" "-targetdir:%TEST_REPORT_DIR%\html" "-reporttypes:Html"

    if %errorlevel% neq 0 (
        echo Warning: Failed to generate HTML report
    ) else (
        echo HTML test report generated at: %cd%\%TEST_REPORT_DIR%\html\index.htm
    )
) else (
    echo.
    echo Info: ReportGenerator not found. To generate HTML reports, install it with: dotnet tool install -g dotnet-reportgenerator-globaltool
)

REM Run specific project tests with reports (optional)
echo.
echo 2. Running specific project tests with reports...
echo ===============================================================

echo Running Work tests...
dotnet test Tests\Mud.Wechat.Work.Tests --logger "trx;LogFileName=work-tests.trx" --results-directory "%TEST_REPORT_DIR%"
if %errorlevel% neq 0 (
    echo Work tests failed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\work-tests.trx
) else (
    echo Work tests passed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\work-tests.trx
)

echo.
echo Running Work.Callback tests...
dotnet test Tests\Mud.Wechat.Work.Callback.Tests --logger "trx;LogFileName=work-callback-tests.trx" --results-directory "%TEST_REPORT_DIR%"
if %errorlevel% neq 0 (
    echo Work.Callback tests failed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\work-callback-tests.trx
) else (
    echo Work.Callback tests passed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\work-callback-tests.trx
)

echo.
echo Running OfficialAccount tests...
dotnet test Tests\Mud.Wechat.OfficialAccount.Tests --logger "trx;LogFileName=officialaccount-tests.trx" --results-directory "%TEST_REPORT_DIR%"
if %errorlevel% neq 0 (
    echo OfficialAccount tests failed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\officialaccount-tests.trx
) else (
    echo OfficialAccount tests passed!
    echo Test report generated at: %cd%\%TEST_REPORT_DIR%\officialaccount-tests.trx
)

REM Generate combined HTML report if ReportGenerator is available
where ReportGenerator >nul 2>nul
if %errorlevel% equ 0 (
    echo.
    echo Generating combined HTML test report...
    echo ===============================================================

    REM Generate combined HTML report
    ReportGenerator "-reports:%TEST_REPORT_DIR%\*.trx" "-targetdir:%TEST_REPORT_DIR%\html-combined" "-reporttypes:Html"

    if %errorlevel% neq 0 (
        echo Warning: Failed to generate combined HTML report
    ) else (
        echo Combined HTML test report generated at: %cd%\%TEST_REPORT_DIR%\html-combined\index.htm
    )
)

echo ===============================================================
echo Test execution completed
echo Execution time: %date% %time%
echo ===============================================================

pause
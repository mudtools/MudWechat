# -----------------------------------------------------------------------
#  作者：Mud Studio  版权所有 (c) Mud Studio 2026
#  Mud.Wechat 项目构建门禁脚本（对齐 MudFeishu/FeishuV3 scripts/verify-build.ps1）。
#
#  门禁内容：
#    步骤 1  Release 全量构建：0 编译错误、0 NU1603（依赖版本降级）
#    步骤 2  AOT strict 冒烟：逐源项目 net8.0 AotStrictMode 构建（IL*/AOT* 0 诊断）
#    步骤 3  单元测试：逐 (测试工程, TFM) 运行并断言 TRX 存在、failed = 0
#
#  用法：pwsh ./scripts/verify-build.ps1 [-SkipTests]
# -----------------------------------------------------------------------
[CmdletBinding()]
param(
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$solution = 'Mud.Wechat.slnx'
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure {
    param([string]$Step, [string]$Message)
    Write-Host "  [x] [$Step] $Message" -ForegroundColor Red
    $script:failures.Add("[$Step] $Message")
}

function Assert-Zero {
    param([string]$Step, [string]$Name, [int]$Value)
    if ($Value -ne 0) {
        Add-Failure -Step $Step -Message "$Name = $Value（应为 0）"
    }
    else {
        Write-Host "  [ok] $Name = 0" -ForegroundColor Green
    }
}

# 原生命令调用包装：Windows PowerShell 5.1 下 `2>&1` 会把原生命令的 stderr 行包装成 ErrorRecord，
# 顶层 $ErrorActionPreference='Stop' 时首行 stderr 即抛 NativeCommandError 杀死脚本 —— 步骤 3 中断、
# 无汇总、退出码不可靠（假绿）。此处局部降级为 Continue，让 stderr 以文本并入日志，失败判定仍由
# 编译错误计数 / TRX counters 承担（两道断言不因此弱化）。真实 cmdlet 错误不受影响（finally 恢复）。
function Invoke-Native {
    param([scriptblock]$Command)
    $previousEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        & $Command 2>&1 | Out-String
    }
    finally {
        $ErrorActionPreference = $previousEap
    }
}

Write-Host '== Mud.Wechat verify-build ==' -ForegroundColor Cyan

# ------------------------------------------------------------------ 步骤 1
Write-Host "`n[步骤 1] Release 全量构建" -ForegroundColor Cyan
$buildLog = Invoke-Native { dotnet build $solution -c Release --nologo }
$buildLog | Out-File -FilePath (Join-Path $env:TEMP 'mudwechat-verify-build.log') -Encoding utf8

$errorCount = ([regex]::Matches($buildLog, ': error ')).Count
$nu1603Count = ([regex]::Matches($buildLog, 'NU1603')).Count

Assert-Zero -Step '步骤1' -Name '编译错误' -Value $errorCount
Assert-Zero -Step '步骤1' -Name 'NU1603 依赖降级' -Value $nu1603Count

# ------------------------------------------------------------------ 步骤 2
Write-Host "`n[步骤 2] AOT strict 冒烟（net8.0，逐源项目）" -ForegroundColor Cyan
$sourceProjects = Get-ChildItem -Path $repoRoot -Filter '*.csproj' -File -Recurse |
    Where-Object { $_.FullName -notmatch '\\(Tests|Demos)\\' -and $_.FullName -notmatch '\\(obj|bin)\\' -and $_.FullName -notmatch '\\\.codeartsdoer\\' }
# 排除 .codeartsdoer（工具临时目录，含评测 harness 生成的 ses_*.csproj 临时产物，非源项目）。
# Roslyn 扩展工程（.Generator.csproj 源生成器 / .Analyzers.csproj 诊断分析器）为 netstandard2.0 单 TFM
# （跨宿主加载硬约束），无运行时 AOT 语义；其正确性由步骤 1 全量构建（随 Callback 编译触发）+ 步骤 3 守卫闭环承担。
# 此排除是范围修正 —— 防假绿三条设置（双断言 / 递归 / --no-incremental）全部保留。
$sourceProjects = $sourceProjects | Where-Object { $_.Name -notmatch '\.(Generator|Analyzers)\.csproj$' }

foreach ($project in $sourceProjects) {
    Write-Host "  AOT strict 构建：$($project.Name)"
    $aotLog = Invoke-Native { dotnet build $project.FullName -c Release -f net8.0 -p:AotStrictMode=true --no-incremental --nologo }
    $aotLog | Out-File -FilePath (Join-Path $env:TEMP "mudwechat-aot-$($project.BaseName).log") -Encoding utf8

    $aotErrors = ([regex]::Matches($aotLog, ': error ')).Count
    $ilErrors = ([regex]::Matches($aotLog, ': (error|warning) IL\d{4}')).Count
    $il3050 = ([regex]::Matches($aotLog, ': (error|warning) IL3050')).Count

    Assert-Zero -Step '步骤2' -Name "$($project.BaseName) 编译错误" -Value $aotErrors
    Assert-Zero -Step '步骤2' -Name "$($project.BaseName) IL 诊断" -Value $ilErrors
    Assert-Zero -Step '步骤2' -Name "$($project.BaseName) IL3050" -Value $il3050
}

# ------------------------------------------------------------------ 步骤 3
if (-not $SkipTests) {
    Write-Host "`n[步骤 3] 单元测试（逐测试工程）" -ForegroundColor Cyan
    $testProjects = Get-ChildItem -Path (Join-Path $repoRoot 'Tests') -Filter '*.csproj' -File -Recurse
    $trxDir = Join-Path $repoRoot 'test-reports'
    New-Item -ItemType Directory -Path $trxDir -Force | Out-Null

    foreach ($testProject in $testProjects) {
        Write-Host "  测试：$($testProject.BaseName)"
        $tfms = @('net8.0')   # 测试项目统一单 TFM（Tests/Directory.Build.props 遮蔽根 props，见其注释）

        foreach ($tfm in $tfms) {
            $trxFile = Join-Path $trxDir "$($testProject.BaseName)_$tfm.trx"
            $testLog = Invoke-Native { dotnet test $testProject.FullName -f $tfm --no-build -c Release `
                --logger "trx;LogFileName=$(Split-Path -Leaf $trxFile)" `
                --results-directory $trxDir --nologo }

            if (-not (Test-Path $trxFile)) {
                Add-Failure -Step '步骤3' -Message "$($testProject.BaseName)[$tfm] 未生成 TRX（测试未执行）"
                continue
            }

            [xml]$trx = Get-Content $trxFile -Raw
            $counters = $trx.TestRun.ResultSummary.Counters
            if ([int]$counters.total -le 0) {
                Add-Failure -Step '步骤3' -Message "$($testProject.BaseName)[$tfm] total = 0（无测试被执行）"
            }
            elseif ([int]$counters.failed -gt 0) {
                Add-Failure -Step '步骤3' -Message "$($testProject.BaseName)[$tfm] failed = $($counters.failed)"
                ($testLog -split "`n" | Select-String 'FAIL') | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkRed }
            }
            else {
                Write-Host "  [ok] $($testProject.BaseName)[$tfm] total=$($counters.total) passed=$($counters.passed)" -ForegroundColor Green
            }
        }
    }
}

# ------------------------------------------------------------------ 汇总
Write-Host "`n== 汇总 ==" -ForegroundColor Cyan
if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host "`nverify-build 失败（$($failures.Count) 项）。" -ForegroundColor Red
    exit 1
}

Write-Host 'verify-build 全部通过。' -ForegroundColor Green
exit 0

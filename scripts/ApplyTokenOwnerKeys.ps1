<#
.SYNOPSIS
  为「应用类型子接口」批量落地令牌归属域查找键（TokenAttribute.TokenManagerKey）。

.DESCRIPTION
  背景与设计见 .docs\MudWechatWork-接口应用类型契约与令牌归属域方案-v1.md。

  落地规则（仅当接口级 [Token] 的 TokenType 为 WechatTokenTypes.AccessToken 时才改写）：
    接口名族别 Internal   -> TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken
    接口名族别 ThirdParty -> TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken
    接口名族别 Provider   -> TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken

  族别判定取自「接口名」而非文件名，兼容两种既有命名风格：
    后缀风格：IWechatWorkXxxService_Internal / _ThirdParty / _Provider
    前缀风格：IWechatWorkInternalXxxService / IWechatWorkThirdPartyXxxService / IWechatWorkProviderXxxService

  不改动 TokenType / InjectionMode / Name（官方契约锁定）；不触碰方法级 [Token]；不改公共父接口。
  脚本幂等：已含 TokenManagerKey 的接口跳过。BOM 与行尾风格按原文件保持。

.PARAMETER RepoRoot
  仓库根目录，默认取脚本所在目录的上级。

.PARAMETER DryRun
  只输出分类报告，不写盘。

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\ApplyTokenOwnerKeys.ps1 -DryRun
  powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\ApplyTokenOwnerKeys.ps1
#>
[CmdletBinding()]
param(
    [string]$RepoRoot,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

# PS 5.1 下 $PSScriptRoot 在 param 默认值求值期可能为空，故在体内解析。
if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $RepoRoot = Split-Path -Parent $PSScriptRoot
}

$interfacesRoot = Join-Path $RepoRoot 'Src\Work\Mud.Wechat.Work\Interfaces'
if (-not (Test-Path -LiteralPath $interfacesRoot)) {
    throw "接口目录不存在：$interfacesRoot"
}

# 与 Mud.Wechat.Work.Abstractions\WechatTokenManagerKeys.cs 的常量名逐字一致。
$internalKeyExpression = 'WechatTokenManagerKeys.InternalAccessToken'
$corpKeyExpression = 'WechatTokenManagerKeys.CorpAccessToken'

# 接口级 [Token]：紧跟其后（仅允许夹带其它特性块）必须是 public interface。
$attributePattern = [System.Text.RegularExpressions.Regex]::new(
    '\[Token\((?<body>(?:[^()]|\([^()]*\))*)\)\](?<gap>\s*(?:\[[^\]]*\]\s*)*)public interface\s+(?<iface>\w+)',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

$tokenTypePattern = [System.Text.RegularExpressions.Regex]::new(
    'TokenType\s*=\s*WechatTokenTypes\.(?<tt>\w+)\s*(?<comma>,)?')

function Get-FacadeFamily {
    param([string]$InterfaceName)

    if ($InterfaceName -match '_Internal$' -or $InterfaceName -match '^IWechatWorkInternal') { return 'Internal' }
    if ($InterfaceName -match '_ThirdParty$' -or $InterfaceName -match '^IWechatWorkThirdParty') { return 'ThirdParty' }
    if ($InterfaceName -match '_Provider$' -or $InterfaceName -match '^IWechatWorkProvider') { return 'Provider' }
    return $null
}

function Get-DominantNewLine {
    param([string]$Text)

    $crlf = ([regex]::Matches($Text, "`r`n")).Count
    $lf = ([regex]::Matches($Text, "(?<!`r)`n")).Count
    if ($crlf -ge $lf) { return "`r`n" }
    return "`n"
}

$stats = [ordered]@{
    FilesScanned = 0
    Applied      = 0
    Kept         = 0
    Skipped      = 0
    Anomaly      = 0
}

$reports = New-Object System.Collections.Generic.List[string]
$anomalies = New-Object System.Collections.Generic.List[string]

$files = Get-ChildItem -LiteralPath $interfacesRoot -Recurse -Filter *.cs -File | Sort-Object FullName
foreach ($file in $files) {
    $stats.FilesScanned++

    $text = [System.IO.File]::ReadAllText($file.FullName)
    $matches = $attributePattern.Matches($text)
    if ($matches.Count -eq 0) { continue }

    $newLine = Get-DominantNewLine -Text $text
    $edits = New-Object System.Collections.Generic.List[object]
    $seen = New-Object System.Collections.Generic.HashSet[string]

    foreach ($match in $matches) {
        $interfaceName = $match.Groups['iface'].Value
        if (-not $seen.Add($interfaceName)) { continue }

        $family = Get-FacadeFamily -InterfaceName $interfaceName
        if ($null -eq $family) { continue }

        $bodyGroup = $match.Groups['body']
        $body = $bodyGroup.Value

        $tokenTypeMatch = $tokenTypePattern.Match($body)
        if (-not $tokenTypeMatch.Success) {
            $anomalies.Add("$($file.FullName) :: $interfaceName :: 接口级 [Token] 未声明 TokenType（无法判定归属域）")
            $stats.Anomaly++
            continue
        }

        $tokenType = $tokenTypeMatch.Groups['tt'].Value

        if ($body -match 'TokenManagerKey') {
            $stats.Skipped++
            $reports.Add("[Skip] $($file.Name) :: $interfaceName :: 已声明 TokenManagerKey（幂等跳过）")
            continue
        }

        if ($tokenType -ne 'AccessToken') {
            # ProviderAccessToken / SuiteAccessToken 本身已无歧义，保持原键；仅校验族别合法性。
            $isInternalFamily = ($family -eq 'Internal')
            if ($isInternalFamily) {
                $anomalies.Add("$($file.FullName) :: $interfaceName :: Internal 族不得消费 $tokenType（该管理器仅第三方/代开发可用）")
                $stats.Anomaly++
            }
            else {
                $stats.Kept++
                $reports.Add("[Keep] $($file.Name) :: $interfaceName :: TokenType=$tokenType 已无歧义，保持原键")
            }
            continue
        }

        $keyExpression = if ($family -eq 'Internal') { $internalKeyExpression } else { $corpKeyExpression }

        # 插入点：TokenType 值与其后的逗号之后。
        $insertOffsetInBody = $tokenTypeMatch.Index + $tokenTypeMatch.Length
        $hasComma = $tokenTypeMatch.Groups['comma'].Success

        $after = $body.Substring($tokenTypeMatch.Index + $tokenTypeMatch.Length)
        $followIndex = 0
        while ($followIndex -lt $after.Length -and ($after[$followIndex] -eq ' ' -or $after[$followIndex] -eq "`t" -or $after[$followIndex] -eq ',')) {
            $followIndex++
        }

        $inline = $true
        $indent = '      '
        if ($followIndex -lt $after.Length -and ($after[$followIndex] -eq "`r" -or $after[$followIndex] -eq "`n")) {
            $inline = $false
            $indentMatch = [regex]::Match($after.Substring($followIndex), "^(`r`n|`n)(?<indent>[ `t]+)\S")
            if ($indentMatch.Success) { $indent = $indentMatch.Groups['indent'].Value }
        }

        $separator = if ($hasComma) { '' } else { ',' }
        $insertion = if ($inline) {
            "$separator TokenManagerKey = $keyExpression,"
        }
        else {
            "$separator$newLine$indent" + "TokenManagerKey = $keyExpression,"
        }

        $absoluteIndex = $bodyGroup.Index + $insertOffsetInBody
        $edits.Add([pscustomobject]@{
                Index     = $absoluteIndex
                Insertion = $insertion
                Interface = $interfaceName
            })
    }

    if ($edits.Count -eq 0) { continue }

    # 自后向前插入，保证偏移量不失效。
    $updated = $text
    foreach ($edit in ($edits | Sort-Object -Property Index -Descending)) {
        $updated = $updated.Substring(0, $edit.Index) + $edit.Insertion + $updated.Substring($edit.Index)
        $stats.Applied++
        $reports.Add("[Apply] $($file.Name) :: $($edit.Interface) :: +(TokenManagerKey)")
    }

    if (-not $DryRun) {
        $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
        $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
        $encoding = [System.Text.UTF8Encoding]::new($hasBom)
        [System.IO.File]::WriteAllText($file.FullName, $updated, $encoding)
    }
}

$reports | ForEach-Object { Write-Host $_ }

Write-Host ''
Write-Host "扫描文件：$($stats.FilesScanned)  落地声明：$($stats.Applied)  无需改动：$($stats.Kept)  幂等跳过：$($stats.Skipped)  异常：$($stats.Anomaly)"
if ($DryRun) { Write-Host '（DryRun：未写入任何文件）' }

if ($stats.Anomaly -gt 0) {
    Write-Host ''
    Write-Host '存在异常项（未自动修改，请人工判定）：'
    $anomalies | ForEach-Object { Write-Host "  - $_" }
    exit 1
}

exit 0

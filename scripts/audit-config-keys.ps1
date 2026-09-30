# -----------------------------------------------------------------------
#  作者：Mud Studio  版权所有 (c) Mud Studio 2026
#  配置键审计脚本（对齐 Feishu audit-config-keys.ps1）：
#  校验 WechatAppConfig / WechatCallbackOptions 的每个公开可写属性
#  在 SDK 源码或文档中都有消费点（删除死配置后不回退，对齐 Feishu R5 配置面治理）。
#
#  用法：pwsh ./scripts/audit-config-keys.ps1
# -----------------------------------------------------------------------
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$failures = New-Object System.Collections.Generic.List[string]

$configFiles = @(
    'Mud.Wechat.Work.Abstractions/Configuration/WechatAppConfig.cs',
    'Mud.Wechat.Work.Callback/WechatCallbackOptions.cs'
)

foreach ($file in $configFiles) {
    Write-Host "审计：$file" -ForegroundColor Cyan
    $fullPath = Join-Path $repoRoot $file
    $content = Get-Content $fullPath -Raw

    # 提取 public string/int/bool 属性名（配置 DTO 全部为可写基元属性）。
    $propNames = [regex]::Matches($content, 'public\s+(?:string|int|bool)\s+(\w+)\s*\{\s*get;\s*set;') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

    if ($propNames.Count -eq 0) {
        $failures.Add("$file 未发现配置属性（解析器失效？）")
        continue
    }

    # 消费点搜索范围：全部源码（排除配置 DTO 自身与生成目录）。
    $searchRoots = @('Mud.Wechat.Work', 'Mud.Wechat.Work.Abstractions', 'Mud.Wechat.Work.Callback')

    foreach ($prop in $propNames) {
        $consumed = $false
        foreach ($root in $searchRoots) {
            $hits = Get-ChildItem -Path (Join-Path $repoRoot $root) -Filter '*.cs' -Recurse -File |
                Where-Object { $_.FullName -notmatch 'obj|bin' -and $_.FullName -notlike "*$file*" } |
                Where-Object { (Get-Content $_.FullName -Raw) -match "\.$prop\b" }
            if ($hits) { $consumed = $true; break }
        }

        if ($consumed) {
            Write-Host "  [ok] $prop" -ForegroundColor Green
        }
        else {
            Write-Host "  [!] $prop 无消费点" -ForegroundColor Yellow
            $failures.Add("$file 属性 $prop 无消费点（若为预留字段请显式消费或删除）")
        }
    }
}

Write-Host ''
if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    exit 1
}

Write-Host '配置键审计通过：所有配置属性均有消费点。' -ForegroundColor Green
exit 0

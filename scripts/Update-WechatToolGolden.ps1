#requires -Version 5.1
<#
.SYNOPSIS
    重固化 AI 工具面 golden 快照（Generated/WechatToolSchemas.golden.txt）。

.DESCRIPTION
    为什么必须绕一圈：golden 漂移时构建**失败**，而失败构建不写 obj/…/generated，于是
    「想看新描述符」与「比对必须先过」互为死锁。本脚本先用 -p:WechatToolGoldenUpdate=true
    跳过 AdditionalFiles 比对、并打开 EmitCompilerGeneratedFiles 跑一次构建，再从生成物的
    *SchemaJson 常量重建快照（与 README「golden 快照」一节的说明一致）。

    快照形态保持既有约定：`<工具名>\t<SchemaJson>\n`，按工具名 Ordinal 升序、LF 行尾、UTF-8 无 BOM。

    跑完务必 `git diff` 逐行复核 —— 快照是契约，不是构建副产物。

.PARAMETER ProjectPath
    AI 工具面工程目录（默认仓库内 Src/AI/Mud.Wechat.Work.AI.Tools）。

.PARAMETER Configuration
    构建配置（默认 Release）。

.PARAMETER TargetFramework
    用于取生成物的单档 TFM（默认 net8.0；多 TFM 下各档产物同源，取一档即可）。
#>
[CmdletBinding()]
param(
    [string] $ProjectPath,
    [string] $Configuration = 'Release',
    [string] $TargetFramework = 'net8.0'
)

$ErrorActionPreference = 'Stop'

# 注意：param 默认值里取不到 $PSScriptRoot（Windows PowerShell 5.1 绑定参数时尚未填充），
# 故默认值留空、在脚本体里补齐（写进 param 默认值会让脚本以 -File 方式调用时直接抛绑定错误）。
if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..\Src\AI\Mud.Wechat.Work.AI.Tools'
}

$ProjectPath = (Resolve-Path $ProjectPath).Path
$GoldenPath = Join-Path $ProjectPath 'Generated\WechatToolSchemas.golden.txt'
$ProjectFile = Get-ChildItem -Path $ProjectPath -Filter '*.csproj' -File |
    Select-Object -First 1
if ($null -eq $ProjectFile) {
    throw "未找到工程文件：$ProjectPath"
}

Write-Host "构建（跳过 golden 比对，发射生成物）：$($ProjectFile.Name)"
$buildArgs = @(
    'build', $ProjectFile.FullName,
    '-c', $Configuration,
    '-f', $TargetFramework,
    '--nologo', '-v', 'q',
    # --no-incremental 不可省略：增量构建在「只改了 obj 里已发射的生成物上游」时可能整体跳过 CoreCompile，
    # 于是本脚本解析到的是上一次发射的旧 WechatToolSchemas.g.cs，写出的快照会静默漏掉新增工具
    # （实测：新增 5 个工具后脚本仍报「已重固化 56 条」）。与 verify-build.ps1 的同类教训一致。
    '--no-incremental',
    '-p:WechatToolGoldenUpdate=true',
    '-p:EmitCompilerGeneratedFiles=true'
)
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) {
    throw "构建失败（exit $LASTEXITCODE），未重写 golden。"
}

$generated = Get-ChildItem -Path (Join-Path $ProjectPath "obj\$Configuration\$TargetFramework\generated") `
    -Recurse -File -Filter 'WechatToolSchemas.g.cs' |
    Select-Object -First 1
if ($null -eq $generated) {
    throw "未找到生成物 WechatToolSchemas.g.cs（检查 EmitCompilerGeneratedFiles 是否生效）。"
}

# 逐行解析 `public const string xxxSchemaJson = """<json>""";`（JSON 恒为单行原始字符串字面量）。
$entries = [System.Collections.Generic.SortedDictionary[string, string]]::new([System.StringComparer]::Ordinal)
foreach ($match in [System.Text.RegularExpressions.Regex]::Matches(
        [System.IO.File]::ReadAllText($generated.FullName),
        'SchemaJson\s*=\s*"""(.*?)""";')) {
    $json = $match.Groups[1].Value
    if (-not ($json -match '^\{"name":"(?<name>[^"]+)"')) {
        throw "无法从生成物解析工具名：$($json.Substring(0, [Math]::Min(80, $json.Length)))"
    }

    $entries[$Matches['name']] = $json
}

if ($entries.Count -eq 0) {
    throw '生成物里没有任何 SchemaJson 常量，拒绝写出空快照。'
}

$builder = [System.Text.StringBuilder]::new()
foreach ($pair in $entries.GetEnumerator()) {
    [void]$builder.Append($pair.Key).Append("`t").Append($pair.Value).Append("`n")
}

[System.IO.File]::WriteAllText($GoldenPath, $builder.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "已重固化 $($entries.Count) 条：$GoldenPath"
Write-Host '下一步：git diff 逐行复核（增删工具、描述改动、路由/风险漂移都应体现在 diff 里）。'

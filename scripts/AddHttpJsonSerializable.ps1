<#
.SYNOPSIS
    为 Mud.Wechat.Work.DataModels 项目中所有 DTO 类型添加 [HttpJsonSerializable] 特性。
.DESCRIPTION
    - 遍历指定根目录下所有 .cs 文件（排除 obj / Generated 目录与已标注文件）。
    - SerializerClassName 取该文件**命名空间的最后一段**（即域段：Users / Department /
      Tags / ContactRules / Batch / Export / FollowUser / Customer / CorpGroup /
      ChainContacts / Rules / CorpTokenAuthentication / InternalAppAuthentication /
      ProviderAuthentication）；位于根命名空间 Mud.Wechat.Work.DataModels 的文件
      （如 WechatWorkResponse.cs）归入 "Common" 组。
      如此每个分组与 DTO 命名空间一一对应，mud-jsonctx 生成的上下文即落在同一
      命名空间（对齐 Mud.Feishu.DataModels「上下文与模块同命名空间」的效果；
      注意 Contracts.Users 命名空间例外属既存形态，沿用其域段 Users 即可）。
    - 仅对顶层的 class 添加特性；跳过 enum / interface / delegate / static class、
      嵌套类型，以及开放泛型定义（如 WechatChatbotResponse<TData> —— 源生成上下文
      以闭合构造为注册单元，开放泛型不直接登记）。
    - 幂等：文件已包含 [HttpJsonSerializable 则跳过。
    - 新增 DTO 后重跑本脚本，再运行 GenerateJsonContext.ps1 重新生成 Generated/ 目录。
#>

param(
    # 数据模型根目录；留空则取「仓库根目录\Mud.Wechat.Work.DataModels」。
    # 也可传入相对路径（相对仓库根目录）或绝对路径。
    # 公众号产品线用法：-RootPath Mud.Wechat.OfficialAccount.DataModels -RootNamespace Mud.Wechat.OfficialAccount.DataModels
    [string]$RootPath,
    # 根命名空间：命中该命名空间的直属文件（无域段）归入 "Common" 组。
    # 多产品线必须显式传入，否则非默认产品线的根级 DTO 会被打成 "DataModels" 等错误分组
    # （生成上下文与 DTO 命名空间错位 ⇒ AOT resolver 合并后类型不可解析）。
    [string]$RootNamespace = 'Mud.Wechat.Work.DataModels'
)

# 脚本位于 <仓库根>\scripts\ 下，仓库根为其上一级目录
$RepoRoot = Split-Path $PSScriptRoot -Parent

# ---- 解析根目录为绝对路径（相对路径基于仓库根目录，不依赖当前工作目录）----
if ([string]::IsNullOrWhiteSpace($RootPath)) {
    $RootPath = Join-Path $RepoRoot 'Mud.Wechat.Work.DataModels'
} elseif (-not [System.IO.Path]::IsPathRooted($RootPath)) {
    $RootPath = Join-Path $RepoRoot $RootPath
}
$RootPath = [System.IO.Path]::GetFullPath($RootPath).TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)

if (-not (Test-Path -LiteralPath $RootPath -PathType Container)) {
    throw "根目录不存在：$RootPath（可用 -RootPath 指定）"
}
Write-Host "扫描根目录：$RootPath"

$attributeName = 'HttpJsonSerializable'
$attrPattern  = [regex]'(?<![\w.])HttpJsonSerializable\b'

# 匹配类型声明行：捕获 (1) 修饰符  (2) 类型种类  (3) 类型名（含泛型参数可选项）
$typeRegex = [regex]::new(
    '^(?<indent>\s*)' +
    '(?<mods>(?:(?:public|internal|protected|private|file|sealed|abstract|static|partial|readonly|unsafe)\s+)*)' +
    '(?<kind>class|record|struct|enum|interface|delegate)\s+(?<name>\w+)(?<generic><[^>]*>)?',
    [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
)

$files = Get-ChildItem -Path $RootPath -Filter *.cs -Recurse |
    Where-Object { $_.FullName -notmatch '\\(obj|Generated)\\' }

$stats = @{ Total = 0; Changed = 0; Skipped = 0; NoType = 0 }

foreach ($file in $files) {
    $stats.Total++

    # ReadAllLines 按 UTF-8 读取（无 BOM 亦可），WriteAllLines 以无 BOM UTF-8 回写，
    # 保持与仓库既有源文件（无 BOM + CRLF，git 侧 i/lf）一致，避免全文件 diff。
    $lines = [System.IO.File]::ReadAllLines($file.FullName)

    # 已标注则跳过（幂等）
    if (($lines -join "`n") -match $attrPattern) {
        $stats.Skipped++
        continue
    }

    $candidates = @()   # 收集需要标注的行号（1-based）

    foreach ($i in 0..($lines.Count - 1)) {
        $line = $lines[$i]
        $m = $typeRegex.Match($line)
        if (-not $m.Success) { continue }

        $kind = $m.Groups['kind'].Value.ToLower()
        $mods = $m.Groups['mods'].Value

        # 跳过非 DTO 类型
        if ($kind -in @('enum', 'interface', 'delegate')) { continue }
        # 跳过 static class / static record / static struct
        if ($mods -match '\bstatic\b') { continue }
        # 跳过开放泛型定义（源生成以闭合构造为注册单元）
        if ($m.Groups['generic'].Success) { continue }

        $candidates += ($i + 1)   # 1-based 行号
    }

    if ($candidates.Count -eq 0) {
        $stats.NoType++
        continue
    }

    # SerializerClassName = 命名空间最后一段（域段）；根命名空间 Mud.Wechat.Work.DataModels
    # 直属文件归入 Common 组。上下文与 DTO 命名空间一一对应，mud-jsonctx 生成的上下文即落在同一命名空间。
    $nsLine = $lines | Where-Object { $_ -match '^namespace\s+([\w.]+)\s*;' } | Select-Object -First 1
    if ($null -eq $nsLine) {
        throw "文件含 DTO 类型但未声明命名空间（file-scoped namespace 为本仓库约定）：$($file.FullName)"
    }
    $namespace = ($nsLine -replace '^namespace\s+', '' -replace '\s*;.*$', '').Trim()
    if ($namespace -eq $RootNamespace) { $module = 'Common' } else { $module = $namespace.Split('.')[-1] }

    # 通过缩进过滤嵌套类型：取所有候选行的最小缩进作为"顶层"基准
    $minIndent = ($candidates | ForEach-Object { $lines[$_ - 1].Length - $lines[$_ - 1].TrimStart().Length } | Measure-Object -Minimum).Minimum

    # 从后往前插入，避免行号偏移
    $newLines = [System.Collections.ArrayList]::new($lines)
    $added = 0
    foreach ($ln in ($candidates | Sort-Object -Descending)) {
        $classLine = $lines[$ln - 1]
        $indentLen = $classLine.Length - $classLine.TrimStart().Length
        # 仅标注顶层（缩进 == 最小缩进）的类型，跳过嵌套类型
        if ($indentLen -gt $minIndent) { continue }

        $indent = ' ' * $indentLen
        $attrLine = "$indent[$attributeName(SerializerClassName = `"$module`")]"
        $newLines.Insert($ln - 1, $attrLine)
        $added++
    }

    if ($added -gt 0) {
        [System.IO.File]::WriteAllLines($file.FullName, $newLines)
        $stats.Changed++
        Write-Host "已修改 [$module] $($file.Name)  (+$added 特性)"
    } else {
        $stats.Skipped++
    }
}

Write-Host "`n==== 统计 ===="
Write-Host "扫描文件总数 : $($stats.Total)"
Write-Host "成功修改     : $($stats.Changed)"
Write-Host "已标注跳过   : $($stats.Skipped)"
Write-Host "无DTO类型    : $($stats.NoType)"

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

# 源码归类（2026-10）后工程目录迁入 Src/<Area>/<ProjectName>。
# 一律按 csproj 名称定位工程目录并缓存，不再硬编码层级 —— 目录再迁移时本脚本不随之漂移。
$projectDirCache = @{}

function Get-SourceProjectDir([string]$projectName) {
    if ($projectDirCache.ContainsKey($projectName)) {
        return $projectDirCache[$projectName]
    }

    # 只在 Src 下按**有界深度**定位工程（Src/<Area>/<ProjectName>/<ProjectName>.csproj）。
    # **不要**改成 Get-ChildItem -Path $repoRoot -Recurse：那会进入 obj/generated-probe 下
    # Microsoft.Extensions.Configuration.Binder 源生成器留下的深路径（实测 223 字符），
    # 叠加长工程名（合并前的 Mud.Wechat.OfficialAccount.Abstractions.csproj，现为 Mud.Wechat.OfficialAccount.csproj）后超过 MAX_PATH(260)，
    # 在 Windows PowerShell 5.1（.NET Framework，无长路径支持）下抛 DirectoryNotFoundException
    # —— 而 README / AGENTS §1 记载的调用方式恰是 powershell(5.1)，即该形态必失败
    # （pwsh 因 .NET Core 的长路径支持侥幸通过，掩盖了问题）。
    # 本脚本解析的工程名全部落在 Src 下（配置 DTO 与消费点均不出现在 Tests）。
    $csproj = Get-ChildItem -Path (Join-Path $repoRoot 'Src') -Filter "$projectName.csproj" -Recurse -File -Depth 2 -ErrorAction SilentlyContinue |
        Select-Object -First 1
    if ($null -eq $csproj) {
        throw "未找到工程 $projectName.csproj（源码归类目录漂移，审计脚本定位失效）"
    }

    $projectDirCache[$projectName] = $csproj.DirectoryName
    return $csproj.DirectoryName
}

# 消费点搜索根：由 slnx /src/ 现场推导（审查报告 v1 F1 接入口收敛 —— 新增产品线只登记 slnx，
# 搜索根零手工维护；字面根清单已两次漂移，OpenPlatform.DataModels 拆包后未补登记即为实证）。
# 仅取 /src/* Folder 下工程：**不得**纳入 /tests/ 与 /Demos/ —— 测试对配置属性名的引用会被算作
# 「消费点」、守卫源文件自身含被扫描字面量，纳入即反向削弱审计；Demo 非交付物同理。
# slnx 为标准 XML，PS 5.1 的 [xml] 直接可解；不用 Split-Path -LeafBase（PS 6.0 才有）。
function Get-SearchRootsFromSlnx {
    [xml]$doc = Get-Content (Join-Path $repoRoot 'Mud.Wechat.slnx') -Raw
    $roots = @(
        $doc.Solution.Folder |
            Where-Object { $_.Name -like '/src/*' } |
            ForEach-Object { $_.Project } |
            Where-Object { $null -ne $_ } |
            ForEach-Object { [IO.Path]::GetFileNameWithoutExtension($_.Path) }
    ) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Sort-Object -Unique
    # PS XML 适配器陷阱：自闭合空 Folder（<Folder Name="/src/" />）的 .Project 返回 $null 而非 $null 集合
    # （@($null).Count = 1），须显式过滤空投影，否则空字符串根会让 Get-SourceProjectDir 抛「未找到工程」。
    if ($roots.Count -lt 20) {
        throw "slnx /src/ 推导出的搜索根仅 $($roots.Count) 个（应 ≥ 20）：slnx 解析失效或结构漂移，审计面不可信"
    }
    return $roots
}

$configFiles = @(
    # 公用层配置基座：公共形状（AppKey/BaseUrl/AllowCustomBaseUrl/TimeoutSeconds/TokenRefreshThreshold/IsDefault）
    # 已上移至本文件，若不同批纳入扫描即为门禁盲区。
    'Mud.Wechat.Abstractions/Configuration/WechatAppConfigBase.cs',
    'Mud.Wechat.Work/Configuration/WechatAppConfig.cs',
    'Mud.Wechat.OfficialAccount/Configuration/MpAppConfig.cs',
    'Mud.Wechat.Work.Callback/WechatCallbackOptions.cs',
    # 智能机器人长连接配置面（P3，官方 101463）：BotId / 长连接专用 BotSecret / EnableLongConnection /
    # 心跳与重连退避参数。BotSecret 是**凭据**（经 aibot_subscribe 帧上送）——不得进日志/遥测/异常消息；
    # 与回调地址模式「二选一」的跨面互斥由长连接启动期校验（同 BotKey 双配置即 fail-fast）。
    'Mud.Wechat.Work/Configuration/WechatBotOptions.cs',
    # 会话内容存档 C SDK 封装的配置面（Work 主包 ExtendedSDK/Finance/）。存档 secret、代理口令与
    # RSA 私钥**永不落 DTO**（只登记 ISecretProvider 中的密钥名，误填原文由 Validate 启动期点名拒绝）；
    # Robots / PrivateKeySecretNames 两支字典非基元属性不入本正则，其消费点分别由工厂与解密链路承载。
    'Mud.Wechat.Work/ExtendedSDK/Finance/WechatFinanceOptions.cs',
    # 公众号回调配置面（与企微 WechatCallbackOptions 同层同形）：路由前缀/超时/白名单/
    # 逐应用凭据（Token/EncodingAESKey/AppId）——新增配置属性必须有真实消费点，否则本脚本 fail-closed。
    'Mud.Wechat.OfficialAccount.Callback/MpCallbackOptions.cs',
    # 微信支付产品线（P0-c 已落地）：商户凭据配置面。私钥与 APIv3 密钥**永不落 DTO**
    # （经组件 ISecretProvider 取用，字段只存密钥名）——若在配置 DTO 出现密钥原文即属凭据面越界，
    # 由 WechatPayMerchantConfig.ValidateSecretName 启动期点名拒绝。
    'Mud.Wechat.Pay/Configuration/WechatPayMerchantConfig.cs',
    # 微信支付回调配置面（P1-b）：路由前缀 / 体长上限 / 时效窗 / 指纹保留窗 / 分发软超时 / 指纹闸开关。
    # 全部属性均有真实消费点（中间件与接收器），无消费点则本脚本 fail-closed。
    # 与【文件创建同批】登记 —— AB-G6 的双向不变式（存在 ⇔ 已登记）会拦下另一方向。
    'Mud.Wechat.Pay.Callback/WechatPayCallbackOptions.cs',
    # 微信小程序产品线的配置 DTO 登记位：P1-c 落地时**创建文件的同批**必须在此登记，
    # 否则 AB-G6 的双向不变式（文件存在 ⇔ 已登记）会失败。本脚本对不存在的文件是 fail-closed 硬错误 ⇒ 不得预登记。
    'Mud.Wechat.Redis/Configuration/WechatRedisOptions.cs',
    'Mud.Wechat.Redis/Configuration/WechatRedisConnectionOptions.cs',
    # 官方分布式令牌存储（Mud.HttpUtils.Redis，A-5 分层共存）在下游的配置映射面：
    # KeyPrefix/AppKey/Database 与 U-2 两开关（UseVersionedEnvelope / AcknowledgeVersionedEnvelopeReadersUpgraded）
    # 均有真实消费点（AddWechatRedisCore 的 RedisTokenStoreOptions 映射；两开关另经 WechatRedisOptions.Validate
    # 两阶段确认闸，映射为权威消费点）；
    # DefaultTtl 为 TimeSpan? 不入本脚本基元正则（可空非基元语义），消费点同处。
    'Mud.Wechat.Redis/Configuration/WechatRedisTokenStoreOptions.cs',
    'Mud.Wechat.OpenTelemetry/WechatOpenTelemetryOptions.cs',
    # 微信小店/视频号（channels 生态）产品线配置面（P0-b 已落地）：小店应用配置（AppId/AppSecret/UseStableToken）。
    # 与【文件创建同批】登记 —— 若只建文件不登记，AB-G6 的双向不变式（存在 ⇔ 已登记）会失败。
    'Mud.Wechat.Channels/Configuration/ChannelsAppConfig.cs',
    # 微信小店回调配置面（2026-10 P3 收口同批登记）：回调凭据 Apps 字典（Token/EncodingAESKey/AppId）。
    # 与 ChannelsAppConfig 同批纪律 —— 只建文件不登记即成门禁盲区（AB-G6 双向不变式锁定）。
    'Mud.Wechat.Channels.Callback/ChannelsCallbackOptions.cs',
    # 腾讯广告产品线配置面（2026-10-10 补登记）：应用凭据（ClientId/ClientSecret/RedirectUri），
    # 与 ChannelsAppConfig 同批纪律 —— 只建文件不登记即成门禁盲区（AB-G6 双向不变式锁定）。
    'Mud.Wechat.Ads/Configuration/AdsAppConfig.cs',
    # 微信开放平台产品线配置面（2026-10-11 B1 补登记）：平台凭据（ComponentAppId/ComponentAppSecret/
    # Token/EncodingAesKey）。ComponentAppSecret 与回调 Token/EncodingAesKey 均为**凭据**，
    # 不得进日志/遥测/异常消息（EnsureValid 只做必填与长度校验、不重写 ToString 防泄密）。
    'Mud.Wechat.OpenPlatform/OpenPlatformAppConfig.cs',
    # 企微 AI Agent 配置面（2026-10-17 S1 落地，同批登记）：系统指令 / 会话 TTL / 历史裁剪与摘要阈值 /
    # 单轮工具预算 / 两个**产品事实状态键**（history、outbox 补发）。全部属性均有真实消费点
    # （WechatAgent 投影构造 AgentRuntimeOptions 与会话门面、DI 的会话 TTL 工厂），无消费点则本脚本 fail-closed。
    # 与【文件创建同批】登记 —— AB-G6 的双向不变式（存在 ⇔ 已登记）会拦下另一方向。
    'Mud.Wechat.Work.AI/Configuration/WechatAgentOptions.cs'
)

# 搜索根在循环外推导一次（字面清单已删除 —— 历史版本在此处维护 26 个根，新增产品线须手工补登，
# 该负担即 F1 要消除的漂移源；现由 Get-SearchRootsFromSlnx 从 slnx 现场推导）。
$searchRoots = Get-SearchRootsFromSlnx

foreach ($file in $configFiles) {
    Write-Host "审计：$file" -ForegroundColor Cyan
    # 登记格式：<ProjectName>/<工程内相对路径>；首段为工程名，按 csproj 定位后拼接。
    $segments = $file -split '/', 2
    $projectDir = Get-SourceProjectDir $segments[0]
    $fullPath = if ($segments.Count -gt 1) { Join-Path $projectDir $segments[1] } else { Join-Path $projectDir (Split-Path $file -Leaf) }
    $content = Get-Content $fullPath -Raw

    # 提取 public string/int/bool 属性名（配置 DTO 全部为可写基元属性）；
    # 回调域新增 AppType/Channel 两个枚举配置属性（区分企业自建/第三方/代开发 × 回调通道），一并纳入扫描。
    # 注：公众号侧 SecurityMode 为**可空枚举**（未设置 = 回落配置级默认值），不匹配本正则——其消费点由
    # ResolveMode 承载，属有意不入扫描（可空语义无法用「必有消费点」表达）。
    # 开放平台 OpenPlatformAppConfig（2026-10-11 B1 登记）的基元属性全部声明为可空（string?）——
    # 注册期 EnsureValid 统一点名必填，故正则放宽「\??」以纳管可空基元（可空枚举仍不入）。
    $propNames = [regex]::Matches($content, 'public\s+(?:string|int|bool|WechatAppType|WechatCallbackChannel)\??\s+(\w+)\s*\{\s*get;\s*set;') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

    if ($propNames.Count -eq 0) {
        $failures.Add("$file 未发现配置属性（解析器失效？）")
        continue
    }

    # 消费点搜索范围：由 slnx /src/ 推导（见 Get-SearchRootsFromSlnx），循环外已就绪。
    # 历史字面清单（26 根，含逐线注释）已删除 —— 其覆盖语义由 AB-G6 的「活性一致」断言承接。

    foreach ($prop in $propNames) {
        $consumed = $false
        foreach ($root in $searchRoots) {
            # 排除配置 DTO 自身：**必须按叶子文件名**比较。
            # 原写法 -notlike "*$file*" 用的是仓库相对路径（正斜杠），而 $_.FullName 是 Windows 反斜杠路径，
            # 两者永不相等 ⇒ 该排除**从未生效**（静默假绿：属性只在自身 DTO 内被引用也会判为「有消费点」）。
            $dtoLeaf = Split-Path $file -Leaf
            $searchDir = Get-SourceProjectDir $root
            $hits = Get-ChildItem -Path $searchDir -Filter '*.cs' -Recurse -File |
                Where-Object { $_.FullName -notmatch 'obj|bin' -and $_.Name -ne $dtoLeaf } |
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

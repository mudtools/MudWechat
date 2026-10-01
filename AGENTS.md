# AGENTS.md

Guidance for AI coding agents working on the **Mud.Wechat** codebase.

## Project Overview

Mud.Wechat 是**企业微信（WeCom）**的 .NET SDK，架构对齐 `D:/Repos/MudFeishu/FeishuV3`（包家族、令牌基座、契约守卫、门禁脚本模式）。覆盖三类应用：

| 应用类型 | `WechatAppType` | 令牌链 |
|---|---|---|
| 企业内部自建应用 | `Internal` | `access_token`（`corpid` + `corpsecret`） |
| 第三方应用（Suite） | `ThirdParty` | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发 | `Provider` | 同 `ThirdParty`，但企业令牌走 `gettoken(corpsecret = permanent_code)` |

## Build / Test / Gate Commands

```bash
dotnet build Mud.Wechat.slnx -c Release                     # 全量构建
dotnet build Mud.Wechat.Work/Mud.Wechat.Work.csproj         # 单项目
dotnet test  Mud.Wechat.slnx                                # 全部测试（4 个测试工程）
dotnet test  Tests/Mud.Wechat.Work.Tests -c Release -f net8.0
dotnet test  Tests/Mud.Wechat.Work.Tests -c Release --filter "FullyQualifiedName~ContractGuards"  # 守卫子集
pwsh ./scripts/verify-build.ps1                             # 质量门禁（构建 + AOT + 测试）
pwsh ./scripts/verify-build.ps1 -SkipTests                  # 仅构建 + AOT
pwsh ./scripts/audit-config-keys.ps1                        # 配置属性消费点审计（CI 同款判据）
dotnet format Mud.Wechat.slnx                               # 格式化（未纳入门禁）
```

**本机未安装 pwsh 7**：上述 `pwsh` 命令在此环境会 `CommandNotFoundException`，改用
`powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1`（5.1）。

## Quality Gate (`scripts/verify-build.ps1`)

三步，**必须全绿**：

1. **Release 全量构建** — 断言 `: error ` 计数 = 0、`NU1603`（依赖降级）计数 = 0。
2. **AOT strict 冒烟** — `Get-ChildItem -Recurse` 定位全部源项目（排除 `Tests`/`Demos`/`obj`/`bin`），逐项目以
   `-c Release -f net8.0 -p:AotStrictMode=true --no-incremental` 构建，断言每项目的
   **编译错误 = 0**、`IL\d{4}` 诊断 = 0、`IL3050` = 0。
   - `-Recurse` 必须保留：去掉后 `Get-ChildItem` 找不到嵌套的 `.csproj`，AOT 步骤会静默空跑（假绿）。
   - `--no-incremental` 必须保留：MSBuild 的 `CoreCompile` 只比对输入/输出时间戳、**不比对 csc 命令行**，
     紧接在步骤 1 之后发起的 strict 构建会被判定为「已最新」而整体跳过编译，报告 0 诊断（假绿）。
3. **单元测试** — 逐测试工程以单 TFM `net8.0` 运行（`Tests/Directory.Build.props` 遮蔽根 props），
   断言 TRX 存在、`total > 0`、`failed = 0`。产物落 `test-reports/`（已 gitignore）。

**已知假绿陷阱（勿"优化"掉）**：只断言诊断计数、不断言编译错误 —— 构建本身失败时诊断计数仍为 0。
本脚本对步骤 2 **同时**断言「编译错误」与「IL 诊断」。

**门禁脚本可直接运行（Windows PowerShell 5.1 与 pwsh 7 均可）**：`scripts/*.ps1` 为 UTF-8 **含 BOM** + 中文注释
（BOM 于 2026-09-30 补齐）。历史坑：**无 BOM** 的 UTF-8 中文脚本在 5.1 下被按 ANSI 解码，报
`Missing ')' in method call` / `The string is missing the terminator`（解析失败，非脚本逻辑问题）——
改写脚本后请确认编辑器没有把 BOM 剥掉。
`audit-config-keys.ps1` **无 `-Strict` 开关**：任何「无消费点」的配置属性都会使其 `exit 1`（与 CI 判据一致）。

## CI（`.github/workflows/dotnet-publish.yml`）

步骤顺序：Build → **诊断白名单断言** → `verify-build.ps1 -SkipTests` → `audit-config-keys.ps1` → `dotnet test --no-build`（TRX 解析摘要）。

- Release 构建日志**必须为 0** 的桶：`NU1603`、`CS1750`、`HTTPCLIENT0\d\d`、`FORM0\d\d`、`EHSG0\d\d`、
  `MUD00[1-4]`、`IL\d{4}`、`AOT00[1-5]|AOT007`。
- **已接受风险、断言为 0 即假红**的两桶：`AOT006`（未登记的 `[HttpJsonSerializable]` DTO，Warning；
  strict 步骤才升为 Error）、`MUD005`（Query/Path 令牌注入，企业微信官方契约要求）。CI 只 INFO 打印其计数。
- 发布链：`workflow_dispatch` → semantic-release 改写 `Directory.Build.props` 的 `<Version>`（**版本唯一来源**）→
  tag `v*` → `dotnet pack` 后断言恰 **4 个 nupkg**（Tests 均 `IsPackable=false`）→ OIDC 临时 Key 推 nuget.org。
- `DOTNET_VERSIONS` 必须**多行**（每行一个 SDK 版本）；写成空格分隔会静默归一为单版本，随后以
  「testhost 无法启动」形式炸开（见 workflow 内注释）。
- workflow 头部注释称「nuget.config 只声明 nuget.org」**已过时**（见 Dependency Version Policy）——以 nuget.config 实际内容为准。

**AOT006（P0-5）已修复**：`Abstractions` 的 **8 个** `[HttpJsonSerializable]` 领域模型由
`Authentication/Models/AuthenticationJsonContext.cs` 覆盖（主包 `WechatJsonResolverExtensions` 已将其与
`WechatWorkJsonContext` 一并合并进组件序列化管线）。**新增 `[HttpJsonSerializable]` 类型必须同步登记到该上下文**，
否则 `AotStrictMode=true` 下 `AOT006`（severity = error）会让门禁步骤 2 直接失败 —— 该诊断本身就是漂移守卫。
脚手架核对（组件官方工具）：`dotnet tool install -g Mud.HttpUtils.JsonContextScaffolder` →
`mud-jsonctx --project Mud.Wechat.Work.Abstractions\Mud.Wechat.Work.Abstractions.csproj --dry-run`。

## Target Frameworks & Language Constraints

`Directory.Build.props`：`TargetFrameworks = netstandard2.0;net6.0;net8.0;net10.0`、
`LangVersion = 13.0`、`Nullable = enable`、`ImplicitUsings = enable`、`TreatWarningsAsErrors = false`、`Version = 1.0.3`。

**`netstandard2.0` 无 `IsExternalInit` polyfill**，因此在该 TFM 下：

- **禁止 `init` 访问器、禁止 `record` 类型 / `with` 表达式**（会报 `CS0518`/`CS0656`）。用 `get; set;` 或构造器。
- `ArgumentNullException.ThrowIfNull` 不可用；用 `x ?? throw new ArgumentNullException(nameof(x))`。
- `string.IsNullOrEmpty` / `IsNullOrWhiteSpace` **无 `[NotNullWhen]` 标注**，流分析不会收窄可空引用。
  需显式 `x == null` 判断或 `is { Length: > 0 }` 模式。
- 条件编译用 `#if NET6_0_OR_GREATER` / `#if NET8_0_OR_GREATER`；**禁用** `Math.Clamp`（用 `Math.Min/Max`）。

`netstandard2.0` 存在**大量既有 CS86xx 可空警告**（Abstractions 的令牌/仓储/回调相关文件）。属既存形态，
门禁只统计「编译错误」与 AOT IL 诊断，**不因 CS 警告失败** —— 不要为消警告做大范围重构。

**`Tests/Directory.Build.props` 遮蔽根 props**：测试项目单 TFM（`net8.0`）。在此新增任何治理属性
（如 `WarningsAsErrors`）必须同时写入 `Tests/Directory.Build.props`，否则测试工程成为门禁盲区。

## AOT / Trim 合规

- `net8.0` / `net10.0` 默认启用 AOT/裁剪分析；`AotStrictMode=true` 时把 `IL2026;IL2046;IL2050;IL2057;IL2067;IL2070;IL2072;IL2075;IL2080;IL3050` 提升为错误。
- `WarningsAsErrors` 常驻含 `AOT001;AOT002;AOT003;AOT004;AOT007`。
- **禁止**调用反射版 `JsonSerializer.Serialize<T>(T, JsonSerializerOptions)` / `Deserialize<T>`。
  统一走 `JsonTypeInfo`（`WechatWorkJsonContext` 源生成）或 `WechatJsonResolverExtensions` 合并解析器。
- 配置绑定为**源生成**（`EnableConfigurationBindingGenerator=true`）。后果：
  - 配置 DTO **禁止 `required`**（生成器以 `new T()` 构造 → `CS9035`）；改在 `Validate()` 里校验（见 `WechatAppConfig`）。
  - 绑定必须走 `Configure<T>(o => section.Bind(o))`，**不要**用 `Configure<T>(IConfiguration)` 重载
    —— 其反射绑定调用点无法被源生成器拦截，会破坏 `IL2026`/`IL3050` 净零。
- `UnconditionalSuppressMessageAttribute` 在 `net10.0` 为 `internal`，用户代码不可引用；必要时用
  `#pragma warning disable IL2026, IL3050` 并附理由注释。

## Project Structure

```
Mud.Wechat/
├── Mud.Wechat.Work/             # 主包：接口声明、服务、DI、模块注册
│   └── Interfaces/              # 按功能族分目录：Authentication/、Contacts/（通讯录五域接口平铺）
├── Mud.Wechat.Work.Abstractions/# 抽象：令牌基座、多应用、配置、仓储、枚举、异常
├── Mud.Wechat.Work.DataModels/  # 官方 DTO + WechatWorkJsonContext（AOT 源生成）
│   └── Contacts/                # 通讯录域 DTO 分组：Users / Department / Tags / ContactRules / Batch（RequestModel/ 仅作目录组织）
├── Mud.Wechat.Work.Callback/    # 回调接收（AES 解密、事件解析、分发）
├── Tests/                       # 测试工程（镜像源结构，单 TFM net8.0）
├── scripts/                     # verify-build.ps1 / audit-config-keys.ps1
├── .docs/                       # 方案与设计文档（中文）
├── Directory.Build.props        # 全局 MSBuild 属性
└── Mud.Wechat.slnx              # 解决方案
```

命名空间与包名一致：`Mud.Wechat.Work` / `Mud.Wechat.Work.Abstractions` / `Mud.Wechat.Work.DataModels` / `Mud.Wechat.Work.Callback`。
依赖方向单向：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`。
**`Callback` 不得引用主包 `Work`**（授权自动化解耦即为此，见下）。

**目录与命名空间现状（2026-10-01 重构后）**：通讯录五域接口平铺于 `Interfaces/Contacts/`（命名空间仍为
`Mud.Wechat.Work`）；DTO 迁入 `DataModels/Contacts/{域}/`，其中 Department/Tags/ContactRules/Batch 的
命名空间随之迁移为 `Mud.Wechat.Work.DataModels.Contacts.{域}`，**Users 域 40 个文件命名空间仍保留
`Mud.Wechat.Work.DataModels.Users`**（与目录不一致，属既存形态；新增 Users DTO 请沿用旧命名空间直至统一决策）。
`RequestModel/`、`ResponseModel/` 仅作目录组织，**命名空间不含该目录段**。

## Dependency Version Policy (Mud.HttpUtils)

- 全仓库锁定 `Mud.HttpUtils` / `Mud.HttpUtils.Generator` **同一版本**（当前 **3.0.0**），由
  `WechatContractGuards.MudHttpUtils_PackageReference_ShouldBeSingleVersion` 守卫「全仓库单一版本」
  （防混版 `TypeLoadException`）。守卫只断言「版本集合大小 = 1」，不硬编码版本号，升级时无需改守卫；
  运行时版本由 `MudHttpUtilsUpgradeSmokeTests` 断言（≥2.0.9 且 Major ≥3）。
- **`nuget.config` 当前含本机源 `mudhttputils-local` → `D:\Repos\MudHttpUtils\artifacts`**（3.0.0 尚未上架
  nuget.org，见 nuget.config 内注释）。restore **只在存在该路径的机器上可用**；既定收尾动作是官方包上架后
  删除该源、把 `PackageReference` 指回 nuget.org 版本——勿在上架前擅自删除（会断 restore），也勿以为这是长期形态。
- 本地组件开发时的缓存陷阱（组件仓库 `D:/Repos/MudHttpUtils`）：NuGet 全局包缓存按 `id + version` 计价，
  **同版本重新打包不会失效下游缓存**；须 `dotnet nuget locals global-packages --clear` → `dotnet clean` → `dotnet build`，
  `clean` 步骤**不可省**（增量构建会把新旧组件程序集并置，运行时爆 `TypeLoadException`）。

## 企业微信领域契约（不可违反）

- **K1**：代开发 `permanent_code` 语义是「应用 secret」→ 走 `gettoken`（`corpsecret = permanent_code`）；
  第三方应用 `permanent_code` 是「授权码」→ 走 `get_corp_token`。二者都在 `CorpTokenManager` 内按 `AppType` 分流，**不新建管理器**。
- **K2**：代开发 `template_id` 即 `suite_id`（`dk` 开头）⇒ **`WechatAppConfig` 不提供独立 `TemplateId` 配置项**
  （2026-09-30 删除：原属性仅被 `Validate()` 用于与非空 `SuiteId` 比对 = 死配置，且使 `audit-config-keys.ps1` 判红）。
  删除后「模板 id ≠ suite_id」这一非法状态**在类型层面不可表达**（强于"启动期校验一致性"）；
  需要模板 id 的**接口参数**（如 `get_customized_auth_url` 的 `templateid_list`）由宿主显式传入，与配置面无关。
- 授权安装链接前缀：`https://open.work.weixin.qq.com/3rdapp/install`。
- 令牌注入统一走 **Query**（企业微信契约，非 Header），触发组件 `MUD005` 已知接受风险；注入白名单由
  `WechatContractGuards.QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` 锁定为
  **授权流接口 + 通讯录两域（成员/部门）父/三子接口共 9 个**（见 Contract Guards 表 G5），新增 Query 注入接口须评估后显式扩展守卫。
- v2 端点：`/cgi-bin/service/v2/get_permanent_code`、`/cgi-bin/service/v2/get_auth_info`；
  `get_customized_auth_url` 以**显式 Query 参数** `provider_access_token` 传令牌（**不带 `[Token]`**，不放宽白名单）。
- `TokenKey` 布局：**三段式 `{tokenType}:{appKey}:{scopeKey}`**（如 `Wechat.AccessToken:default:default`；
  由 `WechatAppTokenManagerBase.BuildCache` 的 `storeKeyMapper` 构造）；`WechatTokenTypes` 一律 `"Wechat."` 前缀，与组件通用 `TokenTypes` 隔离。
- **`AppKey` 形状受约束**：`[A-Za-z0-9]` 开头 + 仅 `[A-Za-z0-9._-]` + ≤128（`WechatAppKeyValidator`，经
  `WechatAppConfig.Validate()` 单点收敛）。理由：AppKey 参与持久化键与命名 HttpClient 名，含 `:` 会造成**键别名**（跨应用令牌串号）。
- 企业级令牌**一企一份**由 `TokenManagerBase` 的 scope 机制承担（`scopeKey = authCorpId`），不依赖 `IWechatTokenStore`。
  企业级令牌**不经声明式 `[Token]`**（由宿主/编排服务显式 `GetTokenAsync(new[]{ authCorpId })` 获取）⇒
  errcode 恢复**必须显式传 scope**：`InvalidateTokenAsync(appKey, AccessToken, new[]{ authCorpId })`；
  以默认作用域失效对已缓存的企业令牌是**空转**（能力边界，由 `CorpTokenManagerScopeIsolationTests` 锁定）。
  `CorpTokenManager` 读取环境 `SetCorp` 上下文须通过**两级校验**：① 上下文 `AppKey` 归属一致；② 上下文 `authCorpId` 与 scope 一致。
- `IWechatCorpAuthStore` 为**复合键 `(AppKey, AuthCorpId)`**；`IWechatSuiteTicketStore` **按 `suiteId` 分槽**（多套件/多代开发模板互不覆盖）。
  默认实现仅进程内，多实例须宿主提供分布式实现（`TryAdd` 前置注册覆盖）。
  `IWechatCallbackReplayGuard` 同款约定（多实例须分布式实现，否则重放窗口失效）。
- **DI 桥接不变量**：`IAppContextHolder`、`IAppContextSwitcher`、`IWechatAppContextSwitcher` **必须是同一实例**
  （组件 `AddMudHttpClient` 内部会 `TryAdd IAppContextHolder`，故 SDK 的注册必须在 `AddMudHttpClient` **之前**）。
  破坏该不变量 ⇒ 声明式 `[Token]` 客户端读到的环境上下文恒为 `null`，多套件静默回退默认应用令牌。
- **`WechatAppManager` 直接实现 `IAppManager<IWechatAppContext>`（不继承组件 `DefaultAppManager<T>`）**：
  基类另有影子注册表（`_apps`）与非 virtual 写入口，会让 `RegisterApp`/`UpdateApp`/`TrySetDefaultApp` 静默写影子表
  而 `GetApp`/`DefaultAppKey` 读不到。故：注册表**单一来源**（本类 `_configs` + `_lazyContexts`）；
  `RegisterApp`/`UpdateApp`/`RegisterSwitcherFactory` 显式 `NotSupportedException`。
  配置读取一律走 `ConfiguredConfigs` / `TryGetConfig`（**非物化**）；`TryGetApp` 会构造命名 HttpClient/DI scope/Timer，
  仅用于「确实需要上下文」的场景（回调的 `SuiteId → appKey` 匹配不得使用它）。
- 授权编排：`IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举）；换码**单飞门 + 结果记忆**以
  **`(appKey, authCode)` 复合键**为粒度（含长度前缀拼接）；共享任务用 `CancellationToken.None` 承载，
  各调用者经 `AwaitSharedAsync` 独立取消；飞行条目**仅创建者移除**。`RevokeAuthorizationAsync` 顺序为
  **先失效令牌、后删库**，且**仅失效本 `appKey`** 令牌。
  策略统一落 `WechatAuthorizationOptions`（`WechatAuthorization` 节），**不得**把编排字段塞进 `WechatAppConfig`。
- 回调自动化：Abstractions 定义 `IWechatAuthorizationCoordinator`，**主包实现**，`Callback` 经
  `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()` **惰性可选解析**；未安装主包授权模块时
  首次 `Warning` 后**降级不抛**。`change_auth` 本地无记录时仅告警、**不发 `get_auth_info`**；
  `cancel_auth` **清理范围恒为 `SuiteId` 命中集**：未命中（含 `SuiteId` 缺失）**只告警不删库**
  ——`(AppKey, authCorpId)` 是**每套件独立**的授权记录，回退「全部应用清理」会删除其它套件的有效授权。
- **回调抗重放不变量**：验签通过后必须过两道 fail-closed 闸——① 时间戳时效窗口 ±300s（缺失/非数字即拒）；
  ② 一次性指纹去重（SHA1 指纹，不得落盘密文本身）。修改回调入口时不得绕过。
- **`WechatCallbackOptions.CorpId` 语义是「接收方 ID」**：企业自建回调为企业 `CorpId`，**套件回调为 `SuiteId`**。
  非空时校验解密明文的 `receiveid`，不一致即拒（该属性是唯一消费点，不得改为死配置）。

## Code Style

### 文件头（所有源文件必须以此开头）

```csharp
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
```

### 命名与风格

- **文件域命名空间**（`namespace X.Y;`）；公共导入集中 `GlobalUsings.cs`。
- 文件头版权块 ≠ `<auto-generated>`；生成文件（`WechatWorkJsonContext` 等）不要手写。
- 命名：类型/方法/属性 PascalCase，接口 `I` + PascalCase，私有字段 `_camelCase`，参数 camelCase，异步方法 `...Async` + `CancellationToken cancellationToken = default`。
- 常量类用 `static class` + `public const string`（如 `WechatTokenTypes`、`WechatErrorCodes`）。
- 错误：参数用 `ArgumentNullException`，状态用 `InvalidOperationException`；统一异常类型 `WechatWorkException`（含 `ThrowIfFailed`）。
- DI：构造注入、字段 `readonly`；多应用基座相关 Singleton 必须经 `IServiceScopeFactory` 建 scope（防 Captive Dependency），退役上下文入退役队列（默认 300s 宽限）后再 Dispose。
- 公共 API 必须写 XML 文档（`<summary>` / `<param>` / `<returns>` / `<exception>`）；门禁开启 `GenerateDocumentationFile`。
  注意 cref 必须可解析（`<paramref name="x"/>` 要求参数存在，否则 `CS1734`；跨包 cref 写全限定或用 `<c>` 兜底）。

## Contract Guards（`Tests/**/ContractGuards/`）

新增/修改契约面时**必须同批**更新守卫。现有 9 条通用守卫（`WechatContractGuards.cs`，G1~G9；
G8 的运行期同实例断言在 `WechatServiceCollectionExtensionsTests`）+ 域守卫组
（`WechatUsersContractGuards.cs` U1~U4、`WechatDepartmentsContractGuards.cs` D1~D4、
`WechatTagsContractGuards.cs` T1~T4、`WechatContactRulesContractGuards.cs` CR1~CR4、
`WechatBatchContractGuards.cs` B1~B4）：

| 编号 | 守卫 | 约束 |
|---|---|---|
| G1 | `MudHttpUtils_PackageReference_ShouldBeSingleVersion` | 全仓库 `Mud.HttpUtils*` 单一版本 |
| G2 | `ConfigDtos_ShouldNotUseRequired` | 配置 DTO 禁用 `required` |
| G3 | `WechatTokenTypes_ShouldUseWechatPrefixedNamespace` | `"Wechat."` 前缀隔离 |
| G4 | `WechatErrorCodes_ShouldAlignWithDetectorCollection` | 失效码 `{40014,42001,42007,42009,42011}` 与判定器同源 |
| G5 | `QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` | Query 注入白名单未放宽（现为授权接口 + 成员/部门/标签/通讯录查看权限/异步导入五域共 18 接口；**应用类型子接口仅覆盖官方实际开放的应用类型**，官方无对应 API 的应用类型不设子接口） |
| G6 | `AuthorizationEndpoints_ShouldMatchOfficialRoutes` | 授权端点路由 + `get_customized_auth_url` 不带 `[Token]` |
| G7 | `QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted` | Query 承载凭据的参数名 ⊆ 组件脱敏词表 **∪ 显式豁免清单**（豁免项须附追踪号；清单已清空——组件 3.0.0 含 C-01，`access_token` 亦在词表内）。新增 Query 凭据参数必须做「补齐词表 / 登记豁免」二选一决策。**豁免自过期**：豁免项一旦被组件词表覆盖即失败，不得静默遗留 |
| G8 | `AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource`（源码顺序断言，`WechatContractGuards.cs`）；运行期同实例断言（2 例）在 `WechatServiceCollectionExtensionsTests` | DI 桥接不变量（见「企业微信领域契约」）。**原计划中的 G8-B（`IAppManager<T>` 反射对齐守卫）已撤回**——`WechatAppManager` 直连实现后不存在影子注册表可能，改由行为用例锁定 |
| G9 | `CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys`（源码文本） | `cancel_auth` 不得再引入「未命中回退全部应用」的越权删除（行为用例在 `WechatCallbackAuthorizationDispatchTests`） |
| U1~U4 | `WechatUsersContractGuards`（成员管理域） | U1 全域路由表 23 条逐一断言（子接口重复声明以 `DeclaredOnly` 限定）；U2 父接口 `IsAbstract` + 三子挂 `Contact` 组并 `InheritedFrom` 父实现类 + **代开发子接口零端点**；U3 四接口统一 `Wechat.AccessToken` + Query 注入 `access_token`；U4 经 `JsonSerializerContext.GetTypeInfo` 断言 DTO 已登记 JSON 上下文。新增成员管理端点/DTO 必须同批更新；**新增 Query 注入接口同样须评估后扩展 G5** |
| D1~D4 | `WechatDepartmentsContractGuards`（部门管理域） | 与 U1~U4 同构：D1 路由表 9 条；D2 层级 + `Contact` 组 + 代开发零端点；D3 令牌绑定；D4 JSON 上下文登记（8 型）。部门域与成员域共挂 `Contact` 注册组，共用 `AddContactApi()` 注册入口 |
| T1~T4 | `WechatTagsContractGuards`（标签管理域） | 与 U1~U4 同构，但**形态特殊**：官方对三类应用开放完全一致的 7 个端点 ⇒ 全部端点收敛父接口，T1 路由表 7 条（全在父接口、互不重复）；T2 层级 + `Contact` 组 + **三个子接口全部零端点**（空标记；任何子接口新增端点 = 能力漂移，先核对官方文档再落位）；T3 令牌绑定；T4 JSON 上下文登记（10 型）。同挂 `Contact` 注册组共用 `AddContactApi()` |
| CR1~CR4 | `WechatContactRulesContractGuards`（通讯录查看权限管理域） | **形态为父接口零端点 + 端点全落自建子接口**（官方仅向自建/通讯录同步应用开放，第三方/代开发无文档 ⇒ **不设第三方/代开发子接口**）：CR1 路由表 4 条（全在 Internal，POST `/cgi-bin/contactrule/*`，其中 list 亦为 POST 无请求体）；CR2 父接口 IsAbstract + **父接口零端点** + Internal 恰好 4 端点；CR3 令牌绑定（父/自建两接口）；CR4 JSON 上下文登记（7 型）。同挂 `Contact` 注册组共用 `AddContactApi()` |
| B1~B4 | `WechatBatchContractGuards`（异步导入接口域） | 形态与标签域同构：官方对自建与第三方开放一致的 4 个端点（`/cgi-bin/batch/syncuser`、`replaceuser`、`replaceparty`、`getresult`；代开发无文档 ⇒ **不设代开发子接口**）⇒ 全部端点收敛父接口，B1 路由表 4 条（3 POST + 1 GET）；B2 层级 + `Contact` 组 + **自建/第三方子接口零端点**；B3 令牌绑定（父/自建/第三方三接口）；B4 JSON 上下文登记（6 型）。同挂 `Contact` 注册组共用 `AddContactApi()`。**危险操作警示**：全量覆盖成员会删除文件外成员（官方对删除比例有熔断），接口注释必须保留该警示 |

## Test Guidelines

- xUnit + FluentAssertions + Moq；测试镜像源结构；类名 `{ClassName}Tests`，方法名 `{Method}_Should{Behavior}_When{Condition}`。
- 已知陷阱（本项目已踩过，勿重复）：
  - **FluentAssertions `Equal` 重载误绑定**：`Equal("a","b","because 文本")` 会绑到 `params object[]` 重载。
    集合断言须写 `Equal(new[]{ "a","b" }, "because 文本")`。
  - **Moq 的 `out` 参数**：不能直接用局部变量，须 `It.Ref<T>.IsAny` + 自定义 delegate 回调。
  - **DI 解析守卫**：新增 Singleton 服务时补「DI 可解析 + 跨 scope 同一实例」用例（`ValidateScopes = true` 变体）。
  - **可选依赖**：走 `IServiceProvider.GetService<T>()`（返回 null 降级），**不**依赖带默认值的可选构造参数。
  - `Moq` 的 `Setup` 与真实 DI 装配顺序：先 `AddWechatWorkServices(...)` 完成真实装配，再用 `AddSingleton(mock.Object)` 覆盖外部边界。

## Configuration Surface

- 配置面唯一公共 API：`WechatAppConfig`（数组节 `WechatApps`）+ `Validate()`；编排策略 `WechatAuthorizationOptions`（节 `WechatAuthorization`）。
- **禁止新增「日志开关」类配置属性**（历史死配置反模式）；日志级别统一由 `Logging:LogLevel:{Category}` 控制。
- 每个公开配置属性必须有真实消费点（`Validate`/`ToString` 不算）。**`scripts/audit-config-keys.ps1` 的口径是「消费点扫描」，并无 `$strictPatterns` 白名单** —— 删除配置键后若脚本报「无消费点」，正确处置是补消费点或删除该属性，而不是加模式。**现状：全绿**（`WechatAppConfig` 12 + `WechatCallbackOptions` 3，权威计数以脚本输出为准）。
  - `WechatCallbackOptions.CorpId` 的消费点是 `receiveid` 校验（不是日志开关）；接收方 ID 语义：企业自建填 `CorpId`，**套件回调填 `SuiteId`**。
  - `WechatAppConfig.TemplateId` 曾因「只被 `Validate()` 使用」被判红 → 已删除（见 K2），不是加白名单绕开。
- 安全默认不得削弱：`BaseUrl` 必须 HTTPS + 白名单（`AllowCustomBaseUrl=false` 为默认 SSRF 防线）。
  `AllowCustomBaseUrl=true` 的应用主机在注册期登记到 `WechatCustomBaseUrlRegistry`，供 errcode 判定器的同步预过滤放行（否则私有化部署静默失去令牌恢复能力）。

## Security

- 绝不记录或暴露 `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`；日志脱敏。
- **Query 承载凭据的脱敏登记**：`corpsecret` / `suite_access_token` / `provider_access_token` 由企业微信契约强制放在 **Query**，
  而组件 `SensitiveUrlRedactor` 是**精确匹配**词表——在组件 3.0.0（C-01）之前不覆盖这三者 ⇒ 随 `ApiException.RequestUri` /
  遥测 URL 明文外泄，故 G7 要求逐参数「补齐词表 / 登记豁免（附追踪号）」。**新增任何 Query 凭据参数仍必须做该二选一决策**。
  SDK 侧不得抢占组件 `IExceptionRedactor`（会丢掉 `Content`/`RequestContent` 的词表擦除 = 削弱安全默认）。
- `WechatWorkException.RequestUri` 在构造期剥离 query 与 userinfo；不得把原始 URI 直接传出。
- 授权链接参数校验：`get_customized_auth_url` 的 `state` ≤ 32 字节且仅 `[a-zA-Z0-9]`（`ValidateCustomizedState`）；
  安装链接 `state` ≤ 128 字节（`WechatAuthorizationUrlRequest.State`）。两条规则**不同**，勿互相套用。
- 回调入口必须保留抗重放两道闸（时效窗口 + 一次性标记），不得为兼容而降级为 fail-open。
- 不要绕过契约守卫与门禁（禁 `--no-verify`、禁删除断言）。

## Docs

方案与设计文档在 `.docs/`（中文）：`MudWechatWork-授权功能方案-v1.md`（规格 + 评审记录 R1~R15）、
`MudWechatWork-详细设计文档-v1.md`、`MudWechatWork-产品规划方案-v1.md`；
`MudWechatWork-审查缺陷修复与完善方案-v1.md`（P0/P1/P2 修复方案 + **评审记录 R1~R26** + 附录 D 实施记录）。
**`.docs/` 被 `.gitignore` 忽略（本地文档，fresh clone 无此目录）**。
**代码变更若触及契约面，须同批同步对应章节。**
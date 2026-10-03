# AGENTS.md

Guidance for AI coding agents working on the **Mud.Wechat** codebase.

> **读法**：动手前读 §1（定位）、§2（怎么跑门禁）、§5（新文件落位）、§6（不可违反的领域契约）；改契约面时读 §7 并**打开守卫源文件核对断言**。
> **路由字符串、端点计数、官方文档链接、官方业务限制一律以守卫源文件与接口 XML 注释为准，本文档不复述。**

## 1 概览

Mud.Wechat 是**企业微信（WeCom）** .NET SDK，架构对齐 `D:/Repos/MudFeishu/FeishuV3`（包家族、令牌基座、契约守卫、门禁脚本模式）。

| 应用类型 | `WechatAppType` | 令牌链 |
|---|---|---|
| 自建应用 | `Internal` | `access_token`（`corpid` + `corpsecret`） |
| 第三方（Suite） | `ThirdParty` | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发 | `Provider` | 同上；企业令牌走 `gettoken(corpsecret = permanent_code)` |

包家族（包名 = 命名空间）：`Mud.Wechat.Work`（主包）、`.Work.Abstractions`、`.Work.DataModels`、`.Work.Callback`、`Mud.Wechat.Redis`。

**最重要的改动方式**：能力增量 = **在既有域内加端点 + 同批更新契约守卫**，不引入新范式。契约面任何改动都会被守卫与门禁打红 —— 守卫不是「改完再补」的收尾项。

## 2 命令与门禁

```bash
dotnet build Mud.Wechat.slnx -c Release                        # 全量构建
dotnet build Mud.Wechat.Work/Mud.Wechat.Work.csproj            # 单项目
dotnet test  Mud.Wechat.slnx                                   # 全部测试（5 个测试工程）
dotnet test  Tests/Mud.Wechat.Work.Tests -c Release -f net8.0 --filter "FullyQualifiedName~ContractGuards"
pwsh ./scripts/verify-build.ps1                                # 门禁：构建 + AOT strict + 测试（必须全绿）
pwsh ./scripts/verify-build.ps1 -SkipTests
pwsh ./scripts/AddHttpJsonSerializable.ps1                     # 新 DTO 批量标注（幂等）
pwsh ./scripts/GenerateJsonContext.ps1                         # 重生成 Generated/*JsonContext.g.cs
pwsh ./scripts/audit-config-keys.ps1                           # 配置消费点审计（CI 同款判据）
```

**本机无 pwsh 7**：改用 `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/xxx.ps1`（5.1 可跑全部脚本）。

### 2.1 门禁三步（`verify-build.ps1`）

1. **Release 全量构建** — 断言 `: error ` = 0、`NU1603`（依赖降级）= 0。
2. **AOT strict 冒烟** — 逐源项目（排除 `Tests`/`Demos`/`obj`/`bin` 与 `*.Generator.csproj`）以 `-c Release -f net8.0 -p:AotStrictMode=true --no-incremental` 构建，断言**编译错误 = 0 且 `IL\d{4}` 诊断 = 0**。生成器工程为 netstandard2.0 单 TFM（Roslyn 组件跨宿主硬约束），无运行时 AOT 语义，其正确性由步骤 1（随 Callback 编译触发）+ 步骤 3 守卫闭环承担 —— 该排除是范围修正，不是免责。
3. **单元测试** — 逐测试工程单 TFM `net8.0`（`Tests/Directory.Build.props` 遮蔽根 props），断言 TRX 存在、`total > 0`、`failed = 0`。产物落 `test-reports/`（已 gitignore）。

**防假绿三设置，删掉即假绿，不许「优化」掉**：

- 步骤 2 须**同时**断言「编译错误」与「IL 诊断」—— 构建本身失败时诊断计数仍是 0。
- `Get-ChildItem -Recurse` 须保留 —— 否则找不到嵌套 `.csproj`，AOT 步骤静默空跑。
- `--no-incremental` 须保留 —— MSBuild `CoreCompile` 只比对时间戳、**不比对 csc 命令行**，紧跟步骤 1 的 strict 构建会被判「已最新」整体跳过。

**脚本编码**：`scripts/*.ps1` 为 UTF-8 **含 BOM** + 中文注释。无 BOM 在 5.1 下按 ANSI 解码，报 `Missing ')' in method call` / `The string is missing the terminator`（解析失败，非逻辑问题）—— 改写后确认编辑器没剥掉 BOM。
`audit-config-keys.ps1` **无 `-Strict`**：任何「无消费点」的配置属性都会使其 `exit 1`。

### 2.2 CI（`.github/workflows/dotnet-publish.yml`）

- 顺序：Build → **诊断白名单断言** → `verify-build.ps1 -SkipTests` → `audit-config-keys.ps1` → `dotnet test --no-build`（TRX 解析摘要）。
- 必须为 **0** 的日志桶：`NU1603`、`CS1750`、`HTTPCLIENT0\d\d`、`FORM0\d\d`、`EHSG0\d\d`、`MUD00[1-4]`、`IL\d{4}`、`AOT00[1-5]|AOT007`。
- **断言为 0 即假红**的两桶（CI 只 INFO 打印计数）：`AOT006`（未登记的 `[HttpJsonSerializable]` DTO，strict 步骤才升为 Error —— 它本身就是漂移守卫）、`MUD005`（Query 令牌注入，企微官方契约要求）。
- 发布链：`workflow_dispatch` → semantic-release 改写 `Directory.Build.props` 的 `<Version>`（**版本唯一来源**）→ tag `v*` → `dotnet pack` 后断言恰 **5 个 nupkg**（Tests 均 `IsPackable=false`）→ OIDC 临时 Key 推 nuget.org；少包即 fail-closed。`DOTNET_VERSIONS` 必须**多行**（每行一个 SDK），写成空格分隔会静默归一为单版本、随后以「testhost 无法启动」形式炸开。

## 3 框架与语言约束

`Directory.Build.props`：`TargetFrameworks = netstandard2.0;net6.0;net8.0;net10.0`、`LangVersion = 13.0`、`Nullable = enable`、`ImplicitUsings = enable`、`TreatWarningsAsErrors = false`、`Version = 1.0.3`。

**`netstandard2.0` 无 `IsExternalInit` polyfill**，该 TFM 下：

- **禁 `init` 访问器、禁 `record` / `with`**（`CS0518`/`CS0656`）—— 用 `get; set;` 或构造器。
- 无 `ArgumentNullException.ThrowIfNull` —— 用 `x ?? throw new ArgumentNullException(nameof(x))`。
- `string.IsNullOrEmpty` / `IsNullOrWhiteSpace` **无 `[NotNullWhen]`**，流分析不收窄 —— 显式 `x == null` 或 `is { Length: > 0 }`。
- 条件编译用 `#if NET6_0_OR_GREATER` / `#if NET8_0_OR_GREATER`；**禁 `Math.Clamp`**（用 `Math.Min/Max`）。

`netstandard2.0` 存在**大量既有** CS86xx 可空警告（Abstractions 令牌/仓储/回调相关）。属既存形态，门禁只统计编译错误与 AOT IL 诊断，**不因 CS 警告失败** —— 不要为消警告做大范围重构。

**`Tests/Directory.Build.props` 遮蔽根 props**：新增任何治理属性（如 `WarningsAsErrors`）必须同时写入该文件，否则测试工程成为门禁盲区。

## 4 AOT / Trim 合规

- `net8.0` / `net10.0` 默认启用 AOT/裁剪分析；`AotStrictMode=true` 把 `IL2026;IL2046;IL2050;IL2057;IL2067;IL2070;IL2072;IL2075;IL2080;IL3050` 升为错误；`WarningsAsErrors` 常驻含 `AOT001;AOT002;AOT003;AOT004;AOT007`。
- **禁止**反射版 `JsonSerializer.Serialize<T>(T, JsonSerializerOptions)` / `Deserialize<T>` —— 一律走 `JsonTypeInfo`（域 `JsonContext`）或 `WechatJsonResolverExtensions` 合并解析器。
- **JSON 上下文是生成物**：DTO 标 `[HttpJsonSerializable]`（`SerializerClassName` = 命名空间域段，根命名空间直属文件归 `Common`）；`Generated/*JsonContext.g.cs` 由脚本生成、**提交进版本控制、勿手改**；上下文 `internal`，经 `InternalsVisibleTo` 供主包与测试直读。主包合并 **31 个生成上下文 + 1 个手写上下文**（`Abstractions` 的 `Authentication/Models/AuthenticationJsonContext.cs`）。
  - **新增 `[HttpJsonSerializable]` 必须同批重跑** `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`（Abstractions 域手写登记进 `AuthenticationJsonContext`），否则 `AotStrictMode` 下 `AOT006`（error）打红门禁步骤 2。
  - 已知边界：开放泛型 `WechatChatbotResponse<>` 不登记（STJ 源生成器不生成其元数据，SYSLIB1030）；关闭 `--auto-derived-types`；工具运行期 `AOT003`（多态缺 `[JsonDerivedType]`）为**已知误报**。
  - 核对工具：`dotnet tool install -g Mud.HttpUtils.JsonContextScaffolder` → `mud-jsonctx --project Mud.Wechat.Work.DataModels\Mud.Wechat.Work.DataModels.csproj --dry-run`。
- 配置绑定为**源生成**（`EnableConfigurationBindingGenerator=true`）：配置 DTO **禁 `required`**（生成器以 `new T()` 构造 → `CS9035`），校验写进 `Validate()`；绑定必须走 `Configure<T>(o => section.Bind(o))`，**不要**用 `Configure<T>(IConfiguration)` 重载（其反射绑定调用点无法被源生成器拦截，破坏 `IL2026`/`IL3050` 净零）。
- `UnconditionalSuppressMessageAttribute` 在 `net10.0` 为 `internal`，用户代码不可引用；必要时 `#pragma warning disable IL2026, IL3050` 并附理由注释。

## 5 结构与依赖方向

```
Mud.Wechat/
├── Mud.Wechat.Work/              # 主包：接口声明、服务、DI、模块注册；Interfaces/{域}/ 按功能族分目录
├── Mud.Wechat.Work.Abstractions/ # 令牌基座、多应用、配置、存储端口、枚举、异常、回调事件信封
├── Mud.Wechat.Work.DataModels/   # 官方 DTO（[HttpJsonSerializable]）+ Generated/ 域 JsonContext（生成物）
├── Mud.Wechat.Work.Callback/     # 回调接收（AES 解密、事件解析、分发）+ HTTP 中间件；Events/ 强类型事件 DTO
├── Mud.Wechat.Work.Callback.Generator/  # 回调契约登记生成器（IsPackable=false；发射 RegisterAll）
├── Mud.Wechat.Redis/             # 四个存储端口的 Redis 实现 + 连接基座 + DI 编排
├── Tests/                        # 5 个测试工程，镜像源结构，单 TFM net8.0
├── scripts/                      # verify-build / audit-config-keys / GenerateJsonContext / AddHttpJsonSerializable
├── .docs/                        # 方案与设计文档（中文；已 gitignore，fresh clone 无此目录）
├── Directory.Build.props         # 全局 MSBuild 属性（版本唯一来源）
└── Mud.Wechat.slnx
```

**依赖单向**：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`、`Redis → Abstractions`。硬边界：**`Callback` 不得引用 `Work`**；**`Redis` 不得引用 `Work` / `Callback`**。

**域 → 目录 / 命名空间 / 注册入口**（新文件必须照此落位）：

| 域 | 接口目录 | DTO 目录（命名空间后缀） | 模块 / 注册入口 |
|---|---|---|---|
| 通讯录 | `Interfaces/Contacts/`（Export 独立子目录） | `Contacts/{Users,Department,Tags,ContactRules,Batch,Export}/` | `Contact` / `AddContactApi()` |
| 客户联系 | `Interfaces/ExternalContact/` | `ExternalContact/{FollowUser,Customer,Tag,JobInheritance,ResignedInheritance,GroupChat,ContactWay,Moment,CustomerAcquisition,AcquisitionComponent,GroupMsg,Statistics,ProductAlbum,InterceptRule,Attachment,ServedContact}/` | `ExternalContact` / `AddExternalContactApi()` |
| 上下游 | `Interfaces/CorpGroup/` | `CorpGroup/{基础,ChainContacts,Rules}/` | `CorpGroup` / `AddCorpGroupApi()` |
| 安全管理 | `Interfaces/Security/` | `Security/`（单一命名空间） | `Security` / `AddSecurityApi()` |
| 消息推送 | `Interfaces/Message/` | `Message/`（单一命名空间） | `Message` / `AddMessageApi()` |

- 接口命名空间一律 `Mud.Wechat.Work`（**不含** `Interfaces` 段）；DTO 命名空间为 `Mud.Wechat.Work.DataModels.{域}[.{子域}]`。
- `RequestModel/`、`ResponseModel/` **仅作目录组织，命名空间不含该目录段**。
- **唯一例外**：通讯录 Users 域命名空间为 `Mud.Wechat.Work.DataModels.Contracts.Users`（既存形态，新增沿用至统一决策）。
- 模块注册三段式：`WechatModule.{域}` 枚举值 + `Add{域}Api()` + 生成器 `Add{域}WebApiHttpClient()`，注册组名与域同名。

## 6 领域契约（不可违反）

### 6.1 令牌与凭据

- **K1 令牌分流**：`CorpTokenManager` 内按 `AppType` 分流 —— 代开发 `permanent_code` 是「应用 secret」→ `gettoken(corpsecret = permanent_code)`；第三方是「授权码」→ `get_corp_token`。**不新建管理器。**
- **K2 模板 id**：代开发 `template_id` 即 `suite_id`（`dk` 开头）⇒ `WechatAppConfig` **不提供独立 `TemplateId`**（非法状态类型层面不可表达）。接口级模板 id 参数（如 `templateid_list`）由宿主显式传入。
- **令牌注入统一走 Query**（企微契约，非 Header，触发 `MUD005`）。白名单 = 授权流 + 通讯录六域 + 客户联系六域 + 上下游三族 + 安全管理三族 + 消息推送四族，由 G5 锁定；**新增须先评估、再显式扩展 G5**。例外：`get_customized_auth_url` 以**显式 Query 参数** `provider_access_token` 传令牌（**不带 `[Token]`**、不放宽白名单）。v2 端点 `/cgi-bin/service/v2/get_permanent_code`、`/cgi-bin/service/v2/get_auth_info`；安装链接前缀 `https://open.work.weixin.qq.com/3rdapp/install`。
- **`TokenKey` 三段式 `{tokenType}:{appKey}:{scopeKey}`**（如 `Wechat.AccessToken:default:default`，由 `WechatAppTokenManagerBase.BuildCache` 的 `storeKeyMapper` 构造）；`WechatTokenTypes` 一律 `"Wechat."` 前缀，与组件通用 `TokenTypes` 隔离。
- **`AppKey` 形状受约束**：`[A-Za-z0-9]` 开头 + 仅 `[A-Za-z0-9._-]` + ≤128（`WechatAppKeyValidator`，经 `Validate()` 单点收敛）。含 `:` 会造成**键别名** ⇒ 跨应用令牌串号。
- **企业级令牌一企一份**（`scopeKey = authCorpId`，由 `TokenManagerBase` 的 scope 机制承担），**不经声明式 `[Token]`**：显式 `GetTokenAsync(new[]{ authCorpId })`。errcode 恢复**必须显式传 scope**（`InvalidateTokenAsync(appKey, AccessToken, new[]{ authCorpId })`）—— 默认作用域对已缓存企业令牌是**空转**（`CorpTokenManagerScopeIsolationTests` 锁定）。
- `CorpTokenManager` 读 `SetCorp` 上下文须过**两级校验**：① 上下文 `appKey` 归属一致；② `authCorpId` 与 scope 一致。`SetCorp` 的 `authCorpId` **必填**（null/空白即抛），与 `appKey=null`「未声明归属」显式区分；`InvalidateTokenAsync` 的 `scopes` 非空但全为空/空白串判为编程错误 fail-fast（`[]`/null 保持「全部」）。

### 6.2 存储端口与多实例

- `IWechatCorpAuthStore` 为**复合键 `(AppKey, AuthCorpId)`**；`IWechatSuiteTicketStore` **按 `suiteId` 分槽**（多套件/多代开发模板互不覆盖）。
- 默认实现仅进程内 ⇒ 多实例部署**引用 `Mud.Wechat.Redis`**（`AddWechatRedis`，**必须先于** `AddWechatApp`/`AddWechatCallback`，颠倒即注册期 fail-fast），或由宿主前置 `TryAdd` 分布式实现覆盖（宿主预注册者按契约胜出）。
- 四个存储端口（含 `IWechatCallbackReplayGuard`：**接口本体落 `Abstractions.TokenManager`，InMemory 实现留 `Callback` 包**）由 Redis 包单依赖 Abstractions 实现并共享连接基座。
- **退役清库批量能力（可选）**：`IWechatTokenStoreBatchRemove : IWechatTokenStore` —— 管理器先 `is` 探测，命中走一次 `RemoveRangeAsync`，未实现回退逐键（宿主自定义 store 零破坏）；形态为「调用方算键 + 实现方批删」（键按中间段 appKey 匹配，前缀扫描不适用）。
- **SE.Redis 3.3.0 陷阱**（改 Redis 包必读）：① `RedisTimeoutException` 继承 `TimeoutException` 而非 `RedisException` —— 捕获须用 `WechatRedisErrors.ShouldWrap`（`is RedisException or RedisTimeoutException`），裸 `catch (RedisException)` 会漏超时；② `StringSetAsync` 有经典 4/5 参、keepTtl 6 参等四套重载 —— **生产调用一律用命名参数钉住形态**（`keepTtl: false` / `when: ...`），裸位置参的重载决胜不确定。

### 6.3 多应用基座与 DI 装配

- **DI 桥接不变量**：`IAppContextHolder` / `IAppContextSwitcher` / `IWechatAppContextSwitcher` **必须是同一实例**。组件 `AddMudHttpClient` 内部会 `TryAdd IAppContextHolder`，故 SDK 注册**必须先于** `AddMudHttpClient`。破坏 ⇒ 声明式 `[Token]` 客户端读到的环境上下文恒 `null`，多套件静默回退默认应用令牌。
- **`WechatAppManager` 直接实现 `IAppManager<IWechatAppContext>`**（**不继承**组件 `DefaultAppManager<T>`：基类另有影子注册表 `_apps` 与非 virtual 写入口，会让注册静默写进读不到的表）。注册表**单一来源** = `_configs` + `_lazyContexts`；`RegisterApp`/`UpdateApp`/`RegisterSwitcherFactory` 显式 `NotSupportedException`。
- 配置读取一律走 `ConfiguredConfigs` / `TryGetConfig`（**非物化**）；`TryGetApp` 会构造命名 HttpClient/DI scope/Timer，仅用于确需上下文处（回调 `SuiteId → appKey` 匹配**不得**用它）。
- 已锁定细则：全部应用经 `RemoveApp` 移除后 `DefaultAppKey` 置 `null`、`GetDefaultApp` 抛明确错误、重新 `AddApp` 用 `??=` 兜底提升；重建白名单仅**瞬时 IO 型异常组**（`HttpRequestException`/`TimeoutException`/`IOException`/`SocketException`），`InvalidOperationException` 直抛；退役队列 fire-and-forget + 停机限时（默认 5s）排空，停机后到达的退役立即 `Dispose`、清库任务丢弃；`RemoveApp` 顺序**恒为 `TryRemove` 先于配置删除**；`CreateAppContext` 经 `WechatOwnedResourceTracker` 登记 scope 之外产物，装配中途失败逆序回收后原样重抛。

### 6.4 授权编排与回调自动化

- **授权编排** `IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举）：换码**单飞门 + 结果记忆**以 `(appKey, authCode)` 复合键为粒度（含长度前缀拼接）；共享任务用 `CancellationToken.None` 承载，各调用者经 `AwaitSharedAsync` 独立取消；飞行条目**仅创建者移除**。`RevokeAuthorizationAsync` 顺序**先失效令牌、后删库**，且仅失效本 `appKey` 令牌。策略统一落 `WechatAuthorizationOptions`（`WechatAuthorization` 节），**不得**把编排字段塞进 `WechatAppConfig`。
- **回调自动化协调器** `IWechatAuthorizationCoordinator`：Abstractions 定义、**主包实现**，`Callback` 经 `IServiceProvider.GetService<>()` **惰性可选解析**（缺主包授权模块时首次 `Warning` 后降级不抛）。`change_auth` 本地无记录仅告警、**不发 `get_auth_info`**；`cancel_auth` 清理范围**恒为 `SuiteId` 命中集**，未命中（含 `SuiteId` 缺失）只告警不删库 —— 授权记录每套件独立，回退「全部应用清理」会删掉其它套件的有效授权。三个逐 appKey 循环（换码/刷新/撤销）**单应用失败记 Error 后继续**；`OperationCanceledException` **不在捕获面**（取消必须穿透）。

### 6.5 回调接收面

| 面 | 约束 |
|---|---|
| 包形态 | 引入 ASP.NET（`netstandard2.0` 用 `Microsoft.AspNetCore.Http 2.3.9`，`net6+` 用 `FrameworkReference`）；中间件**经典约定式** `RequestDelegate`（不进 DI），宿主 `UseWechatWebhook()` 接入 |
| 凭据来源 | `WechatCallbackOptions.Apps` 字典是**唯一来源**（`WechatAppConfig` 无 Push 属性、应用级配置无 `AppKey` 属性）；路由 `/{GlobalRoutePrefix}/{AppKey}` |
| 注册 | `AddWechatCallback` 返回建造者（链式注册处理器/拦截器）；`IConfiguration` 重载为**惰性** `Configure(o => section.Bind(o))`（AOT 安全）；`IOptionsMonitor` 支持路由前缀与凭据**请求期热更** —— **勿改回注册期急切绑定**。**不得引入「接收方 ID 统一注册表 / `IWechatCallbackUrlVerifier`」**（多套件由「每套件一个 `Apps` 条目 + 各自独立 Token/AESKey/接收方 ID」覆盖） |
| 校验 | 请求期单应用（`ResolveApp` 命中后 `app.Validate(appKey)`，单应用配置错误不拖垮全部回调路由）+ `WechatCallbackOptions.Validate()` 启动期全量入口；接收失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 映射失败类别） |
| 加解密 | 按官方 **32 字节块 PKCS7** 手工补位/剥离；**禁用 .NET 内置 16 块 `PaddingMode.PKCS7`**（会误拒官方 pad∈[17..32] 报文） |
| 抗重放 | 两道 fail-closed 闸：① 时间戳窗口 ±300s（缺失/非数字即拒）② 一次性 SHA1 指纹（**不得落盘密文本身**）。**指纹闸在「解密 + receiveid 校验成功」之后、事件返回之前**（解密失败不消耗指纹，官方重试可重入）。**GET echo 只过时效闸、不消费指纹**（同 echostr 二次保存配置必须成功）。分布式守卫异常**必须上抛**（→ 5xx → 官方 96238 重试）；**禁止**吞异常返回 `true` 放行重放或返回 `false` 静默丢事件；空键返回 `false` 且不触达存储 |
| 事件信封 | `WechatCallbackEvent` / `IWechatCallbackEventHandler` / `IWechatCallbackEventInterceptor` / `WechatCallbackEventTypes` 落 `Abstractions.Callback`（该目录**含子目录、不得出现 XML 类型**，CB7 已递归）；XML→信封解析留 `Callback.WechatCallbackReceiver`。`EventTypeKey`：客户联系/获客族以**外层事件值**为键（`Event` 优先、套件信封回退 `InfoType`，两信封**同键**），其余 = `InfoType`（非空）→ `ChangeType` → `Event`；`SupportedEventType` 空串 = 兜底（内置处理器即此形态，文件**留 `Callback` 包根**）。`AuthCorpId ← FromUserName` 兜底**仅限授权族**（`change_contact` 恒 `sys`）。信封另带 `AppKey`/`AppType`/`Channel` 只读快照，**配置权威仍是** `WechatAppManager` / `WechatCallbackOptions` |
| 分发 | 组合根期急切注册、**无 Freeze**；通配键 `"*"` 双语义（通讯录同步助手路由 + 全局处理器/拦截器桶）；匹配序 appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底。同步分发 + 软超时（默认 4500ms，**必须 < 5s 契约**）；超时/拦截器中断 → 503 触发重推；**指纹在分发前消费** ⇒ 重推同指纹被 403（fail-closed 优先于 at-least-once，**处理器须幂等**）；单处理器异常隔离（LogError 后继续，结果仍 Handled）；`MaxConcurrentEvents` 容量为构造期快照（热更不改容量） |
| 配置面 | `WechatAppCallbackOptions`：`PushToken`/`PushEncodingAESKey`/`ReceiveId`/`AppType`/`Channel`，**必须与主配置同类文件**才纳入 audit 扫描。`ReceiveId` 是接收方 ID（自建填企业 `CorpId`、**套件填 `SuiteId`**）；非空时校验解密明文 `receiveid`，不一致即拒；留空（通讯录同步助手）或明文未携带时跳过校验并一次性告警（兼容官方「个人主体第三方为空串」） |
| AppType × Channel | `AppType` 默认 `Internal`；`Channel`（`WechatCallbackChannel`，`App=1` 应用数据通道 / `Suite=2` 套件指令通道，默认 `App`）。`Validate()` 拒绝「自建应用占用套件通道」「套件通道非第三方/代开发」。`ValidateReceiveId` 三元分流：自建·App 与第三方·代开发·Suite = 静态 `ReceiveId`；第三方·代开发·App = 动态授权企业 CorpId（比对外层 `ToUserName`）。`IsEventFamilyAllowed` 矩阵：授权族→Suite+第三方/代开发；上下游→App+自建；通讯录/异步→App；客户联系/获客族→自建·代开发×App + 第三方×Suite（官方 92277 推送至指令回调 URL）；Unknown→不拦截。合法性闸**先于**拦截器 `BeforeHandleAsync`，不适用族返回 `Rejected`（→200 不重推） |

**事件载荷体系（v2.2）**：**不得**再新增「逐事件 DTO + 手写 `ParseXxx`」，**不得**手写多级嵌套解析或手改 `OfficialPayloadContracts.RegisterAll` 方法体。载荷按官方**报文结构族**建于 `Events/Payloads/`（子目录 `Contacts/`、`ExternalContact/`、`CorpGroup/`、`AsyncJobs/`、`Messages/`、`Contracts/`；**目录仅作组织**，命名空间恒为 `Mud.Wechat.Work.Callback.Events.Payloads`），字段映射由**上游** `Mud.HttpUtils.PayloadFieldMapGenerator` 依 `[PayloadContract]`/`[PayloadField]` 编译期生成 ⇒ 元素名 ↔ 属性名配对受编译器校验。

1. 载荷 `partial` + `[PayloadContract(Converter = typeof(WechatPayloadConverter))]`；转换器落 `Abstractions/Callback/Payloads/`（纯转换语义、零 Callback 依赖）。标量方法须 **`static`、非泛型、恰 1 参**（首参 `PayloadNode` 或 `string?`，生成器据此决定传 `n` 还是 `n?.Value`）。
2. **嵌套走声明化通道**（由 `WechatPayloadConverter` 提供，内层字段递归 `Bind`）：单对象 `Object<TSingle>` —— 内层 DTO 标 `[PayloadContract]`、属性须可空、禁 `ItemName`；对象列表 `ItemsObject<TItem>` —— 须 `ItemName`、元素实参非可空。内层 DTO 升级 `partial` + `[PayloadContract]` 后映射表自动生成（`WechatCallbackExtAttrItem` 的 `ItemsWithAttributes` 形态除外）。
3. 映射表须在**具体类型**处取 `XxxPayload.PayloadFieldMap`（泛型上下文禁取类型参数静态成员 CS0712；`static abstract` 需 net7+，本仓含 ns2.0 不可用）。类型化处理器继承**抽象基类** `WechatCallbackPayloadHandler<TPayload>` —— 「泛型接口 + 显式默认实现」在 ns2.0 报 **CS8701**。
4. **契约登记单一来源**：事件键 + 族前置条件 + 事件键级开放面声明在载荷类的 **`[WechatCallbackContract]`**（`AllowMultiple`：同键/不同键子集均可叠加声明 —— 同键多声明由生成器**合并去重**为一条多组开放面登记，如客户联系族两段；`requiredEvent` 缺省 = 逐键自指），生成器发射 `OfficialPayloadContracts.RegisterAll`（每键一条 `CreateWithOpenSurfaces`）。运行期「不宽于族默认」校验不变（CB4e）；**声明不完整由诊断 MUDCB001 打红**。
5. `Callback.csproj` 必须**显式**引用 `Mud.HttpUtils.Generator`（Abstractions 侧带 `PrivateAssets="all"`，**不流向** Callback）与**本仓生成器工程** `Mud.Wechat.Work.Callback.Generator`（`OutputItemType="Analyzer"`、`IsPackable=false`，不进「恰 5 nupkg」）；上游生成器引用与 `Mud.HttpUtils` 包版本须**全仓单一**（守卫 `MudHttpUtils_PackageReference_ShouldBeSingleVersion`，`NU1605` 视为错误）。

- 载荷**不复用** DataModels 的 JSON DTO（XML vs JSON、逗号/竖线串 vs List、权限降权语义三重差异）。
- **三模式无关性（ADR-14）**：三种 `AppType` 报文**结构同一**，差异只是「值是否出现」⇒ 一份可空超集载荷覆盖三模式；**载荷与转换器层禁止出现 `WechatAppType` / `WechatCallbackChannel` 分支**（CB4d），需按模式分支时在**处理器层**读 `evt.AppType`。
- **开放面粒度是事件键（ADR-15）**：`IsEventFamilyAllowed` 为**族级默认**，事件键级声明（`OpenSurfaces` 多组「模式集合×通道」组合对 + `RequiredEvent` / `RequiredFamily`）在其上叠加，两闸均**先于**拦截器 `BeforeHandleAsync`（CB13b）。宿主注册新 `Event` 值会落 `Unknown` 族被族闸放行 ⇒ **必须**在 `[WechatCallbackContract]` 显式声明，不得依赖族默认；CB4b 双面锁定（官方清单锚点 + 特性并集一致）。**开放面为「（模式集合, 通道）」组合对**（`WechatOpenSurface`）：客户联系/获客族官方按模式分通道，单一「模式集合×通道」无法表达。
- **客户联系/获客族以族事件值为键**：官方裸 `ChangeType` 跨族同名（`create`/`update`/`delete` 在客户群与标签族之间、`del_follow_user` 在客户联系与获客助手之间）⇒ 逐 `ChangeType` 键无法消歧，事件键为 `change_external_contact` / `change_external_chat` / `change_external_tag` / `customer_acquisition` / `customer_acquisition_permit_change` 五个族事件值，具体类别由信封 `ChangeType` 判别（同 `ChainChangedPayload` 结构族模型）；第三方套件信封（92277/97402/99485，无 `Event` 节点、外层事件值在 `InfoType`）产出**同一事件键**，`MatchesEnvelope` 的 `RequiredEvent` 比对口径为 `Event ?? InfoType`。
- **上下游事件族**：`Event=change_chain` + 9 个 ChangeType（空间/分组/企业三族），信封带 `ChainId`/`IsChangeChain`；载荷按官方结构族建 **1 型**（`ChainChangedPayload`，含可空 `GroupIds`/`CorpIds`，ChangeType 经信封判别）；`batch_job_result` 官方存在**双报文布局**（通讯录 90973 顶层节点 vs 上下游 95797 `BatchJob` 包装节点）⇒ 契约声明 `ScopeFallback = "BatchJob"`，三级作用域判定由上游 `ResolveScope` 完成。**`JobType` 级差异属语义过滤、不得做成安全闸**（官方约束的是事件面，非某个 `JobType` 值）⇒ 处理器按需自行判别。

### 6.6 消息推送落位决策

- **不做运行时多态**（AOT 源生成按声明类型序列化）：每个 msgtype 一个端点方法 + 请求 DTO（同路由多方法），官方各 msgtype 参数表差异（`safe` 有无、id 转译支持面、`mentioned_list` 仅群聊）在 DTO 层面精确表达；模板卡片 `TemplateCardBody` 为发送/更新两端点共用的扁平结构（`card_type` 判别 + 全可选嵌套，`replace_text`/`disable` 仅更新接口支持）。
- **响应形态陷阱**：`message/send` 的 `invaliduser` 为**竖线分隔字符串**、`update_template_card` 的为**字符串数组** ⇒ 两类响应 DTO 不共用；学校通知响应为 `invalid_parent_userid` / `invalid_student_userid` / `invalid_party` 三个数组。
- **「接收消息与事件」不是本域接口**：消息接收是企微回调推送（XML），由 `Mud.Wechat.Work.Callback` 承载；本域全部是发送侧 HTTP API。

## 7 契约守卫（`Tests/**/ContractGuards/`）

**规则**：改动契约面（接口/端点/路由/DTO/注册组/配置面）必须**同批**更新对应守卫。守卫是路由表、端点计数、分层形态、令牌绑定、JSON 上下文登记的**权威描述**；各接口的官方文档 URL/ID 与官方业务限制（频率上限、串行要求、覆盖删除、权限可见范围等）**已落在接口 XML 注释中**，新增端点照此格式补齐、不得删减或弱化。

### 7.1 通用守卫（`WechatContractGuards.cs`，G1~G9）

| 编号 | 锁定的约束 |
|---|---|
| G1 | 全仓库 `Mud.HttpUtils*` 单一版本（防混版 `TypeLoadException`） |
| G2 | 配置 DTO 禁用 `required` |
| G3 | `WechatTokenTypes` 一律 `"Wechat."` 前缀 |
| G4 | 失效码 `{40014,42001,42007,42009,42011}` 与判定器同源 |
| G5 | Query 令牌注入白名单未放宽；**应用类型子接口仅覆盖官方实际开放的应用类型** |
| G6 | 授权端点路由与官方一致；`get_customized_auth_url` 不带 `[Token]` |
| G7 | Query 承载凭据的参数名 ⊆ 组件脱敏词表 ∪ 显式豁免清单（豁免须附追踪号；**豁免自过期**，被词表覆盖即失败） |
| G8 | DI 桥接三接口同实例（源码顺序断言 + 运行期用例） |
| G9 | `cancel_auth` 不得引入「未命中回退全部应用」的越权删除 |

### 7.2 域守卫

**跨域通用断言**：令牌绑定（`Wechat.AccessToken` + Query 注入）、新增 DTO 的 JSON 上下文登记、子接口「官方未开放 ⇒ 零端点」、继承链上子接口集合不漂移；下表只列该域的**特殊**断言，端点计数以守卫源文件为准。

| 守卫 | 域 | 特殊断言 |
|---|---|---|
| U1~U4 / D1~D4 | 通讯录·成员 / 部门 | 父接口 `IsAbstract`，三子挂 `Contact` 组并 `InheritedFrom` 父实现类（子接口 `DeclaredOnly` 限定声明位置）；路由表 23 / 9 条；代开发零端点 |
| T / CT / JI / RI / GC | 标签 / 客户标签 / 在职继承 / 离职继承 / 客户群 | 官方三类应用开放完全一致 ⇒ 全收敛父接口（7 / 9 / 3 / 4 / 3 条） |
| CW / MO / CA / GM / ST / PA / IR / UA / SC | 客户联系·联系我与客户入群方式 / 客户朋友圈 / 获客助手 / 消息推送（群发） / 统计管理 / 商品图册 / 聊天敏感词 / 上传附件资源 / 获取已服务的外部联系人 | 官方三类应用开放完全一致 ⇒ 全收敛父接口（10 / 14 / 9 / 11 / 3 / 5 / 5 / 1 / — 条；SC 官方仅自建开放，第三方/代开发暂不支持 ⇒ 零端点父接口 + 仅自建子接口）；`get_moment_task_result`（jobid 走 Query）、`customer_acquisition_quota`、`get_intercept_rule_list` 为 GET、其余全 POST；`get_contact_way` 官方即 POST（勿改 GET）；群发记录列表为 `get_groupmsg_list_v2`（带 v2 后缀）；敏感词删除路由为 `del_intercept_rule`（非 delete）；群聊统计用 offset + limit 分页（区别于本模块其它域的 cursor + limit）；上传附件资源在 `/cgi-bin/media/upload_attachment`（multipart，文件参数须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的 `GetFormDataContentAsync` 调用在运行时接口不存在（CS1061），`[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync` 通路） |
| CR1~CR4 | 通讯录·查看权限 | 父接口零端点 + Internal 恰 4 条（全 POST）；**不设**第三方/代开发子接口 |
| AC1~AC4 | 客户联系·获客助手组件（仅第三方应用开放：6 端点 + 代支付流水 1 端点） | 两族父接口零端点 + 仅第三方子接口承载（继承链恰 1 子）；代支付流水 `get_bill_list` 走 **`suite_access_token`**（获客助手组件的应用凭证，路由在 `/cgi-bin/service/` 下、授权企业以请求体 `auth_corpid` 指定）⇒ 独立接口族/令牌路由键，其余 6 端点走企业级 `access_token`；组件版 `list_link`/`get`/`statistic`/`create_once_key`/`get_chat_info` 与获客助手直连版**共用路由**（组件仅可见授权给组件的链接）；组件版 `get` 响应仅 `link_name`+`url`、`get_chat_info` 响应**无**顶层 `userid`/`external_userid`（与直连版差异点）；获客助手事件通知（99485）为回调推送，由 Callback 包承载、不设 HTTP 端点 |
| B1~B4 / E1~E4 | 通讯录·异步导入 / 导出 | 父接口 4 条（3 POST + 1 GET）/ 5 条（4 POST + 1 GET）；导入不设代开发子接口 |
| FU / CU | 客户联系·服务人员 / 客户管理 | 父 1 + 第三方 1 差异端点（自建零）；父 10 + 第三方 3 身份转换差异端点（自建、代开发零） |
| CG1~CG11 | 上下游 | 基础：父 6 条全 POST + 继承链恰 3 子接口；通讯录：父 4 + 自建 5（代开发零）；规则：父零端点 + Internal 恰 5 |
| SEC1~SEC4 | 安全管理 | 三族父接口零端点 + 自建恰 9 / 5 / 2 条；**不设**第三方/代开发子接口（官方无文档） |
| MSG1~MSG4 | 消息推送 | 应用消息族父 13 + 第三方恰 1；AppChat / SchoolMessage / SmartSheetGroupChat 父零端点 + 仅 Internal 承载 |
| CB1~CB13 | 回调 | 包依赖边界、53 个事件键与 `EventTypeKey` 优先级、兜底处理器形态与文件路径、事件 DTO 字段、凭据唯一来源、echo 不消费指纹、32 字节块填充（禁内置 PKCS7）、指纹闸次序、通道枚举与配置面、`receiveid` 三元分流、开放面矩阵、合法性闸次序 |
| MA1~MA4 | 多应用（`Abstractions.Tests`） | `RemoveApp` 删除顺序、重建异常白名单、退役队列 `_disposed` 闸、`SetCorp` 参数校验。守卫为**方法体文本断言**（花括号配平），签名漂移须同步更新 |
| RD-G1~RD-G6 | Redis（`Redis.Tests`） | SCAN 仅经 `WechatRedisKeyBuilder.Pattern` 单一出口、配置无 `required`、重放守卫 fail-closed 上抛、凭据不进日志、全名探测防漂移、单依赖 Abstractions |

### 7.3 官方契约陷阱

- **拼写照抄官方原文**（无法从语义推出，只能照抄）：`universal_domian`（域名 IP 响应字段）；`admin_oper_log` 游标官方作 `cusor`（SDK 以官方 JSON 示例为准用 `cursor`）；在职群接替 `groupchat/onjob_transfer` vs 离职群接替 `groupchat/transfer`；`corpgroup/getresult` / `batch/getresult` / `export/get_result` 三处互不相同（另 `batch/get_by_user` 易混）。

## 8 编码风格

### 8.1 文件头（所有源文件必须以此开头）

```csharp
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
```

### 8.2 命名与风格

- 文件域命名空间（`namespace X.Y;`）；公共导入集中 `GlobalUsings.cs`。
- 文件头版权块 ≠ `<auto-generated>`；生成文件（`Generated/*JsonContext.g.cs` 等）**不要手写**，改数据后重跑脚本。
- 命名：类型/方法/属性 PascalCase；接口 `I` + PascalCase；私有字段 `_camelCase`；参数 camelCase；异步方法 `...Async` + `CancellationToken cancellationToken = default`。
- 常量类用 `static class` + `public const string`（如 `WechatTokenTypes`、`WechatErrorCodes`）。
- 错误：参数用 `ArgumentNullException`，状态用 `InvalidOperationException`；统一异常类型 `WechatWorkException`（含 `ThrowIfFailed`）。
- DI：构造注入、字段 `readonly`；多应用基座相关 Singleton 必须经 `IServiceScopeFactory` 建 scope（防 Captive Dependency），退役上下文入退役队列（默认 300s 宽限）后再 `Dispose`。
- 公共 API 必须写 XML 文档（`<summary>`/`<param>`/`<returns>`/`<exception>`；门禁开启 `GenerateDocumentationFile`）。cref 必须可解析（`<paramref name="x"/>` 要求参数存在，否则 `CS1734`；跨包 cref 写全限定或用 `<c>` 兜底）。

## 9 测试规范

- xUnit + FluentAssertions + Moq；测试镜像源结构；类名 `{ClassName}Tests`，方法名 `{Method}_Should{Behavior}_When{Condition}`。
- 已踩陷阱（勿重复）：FluentAssertions `Equal("a","b","because…")` 会误绑 `params object[]` ⇒ 集合断言写 `Equal(new[]{ "a","b" }, "because…")`；Moq 的 `out` 参数不能用局部变量，须 `It.Ref<T>.IsAny` + 自定义 delegate 回调；新增 Singleton 服务必补「DI 可解析 + 跨 scope 同一实例」用例（`ValidateScopes = true` 变体）；可选依赖走 `IServiceProvider.GetService<T>()`（null 降级），**不**依赖带默认值的可选构造参数；Moq 与真实装配的顺序是**先** `AddWechatWorkServices(...)` 完成真实装配、**再** `AddSingleton(mock.Object)` 覆盖外部边界。

## 10 配置面与依赖版本

- 唯一公共配置 API：`WechatAppConfig`（数组节 `WechatApps`，含 `Validate()`）；编排策略 `WechatAuthorizationOptions`（节 `WechatAuthorization`）；回调凭据 `WechatCallbackOptions`。
- **禁止新增「日志开关」类配置属性**（历史死配置反模式）；日志级别统一由 `Logging:LogLevel:{Category}` 控制。
- **每个公开配置属性必须有真实消费点**（`Validate`/`ToString` **不算**）。`audit-config-keys.ps1` 是纯消费点扫描、**无白名单无 `-Strict`** ⇒ 报「无消费点」时的正确处置是**补消费点或删除该属性**，不是加模式绕开（历史教训：`WechatAppConfig.TemplateId` 只被 `Validate()` 使用 ⇒ 删除；`WechatAppCallbackOptions` 不得有 `AppKey`，字典键是唯一权威）。消费点口径示例：`ReceiveId` → `receiveid` 校验（**不是**日志开关）；`AppType`/`Channel` → `Validate()` + `ValidateReceiveId` + `IsEventFamilyAllowed`。
- **安全默认不得削弱**：`BaseUrl` 必须 HTTPS + 白名单（`AllowCustomBaseUrl=false` 是 SSRF 防线）；`true` 的应用主机在注册期登记到 `WechatCustomBaseUrlRegistry`，供 errcode 判定器同步预过滤放行（否则私有化部署静默失去令牌恢复能力）。
- **`Mud.HttpUtils` / `Mud.HttpUtils.Generator` 全仓锁定同一版本**（当前 `3.0.1`），由 G1 断言「版本集合大小 = 1」（不硬编码版本号，升级无需改守卫）；运行时版本由 `MudHttpUtilsUpgradeSmokeTests` 断言（≥2.0.9 且 Major ≥3）。该系列 3.0.1 已在 nuget.org 正式发布（含 `Mud.HttpUtils` / `.Generator` / `.Attributes` / `.Abstractions` / `.Client` **五个包均需存在**）；`nuget.config` **只声明 nuget.org、无本机开发源**，勿加回；本地组件迭代用临时源验证后必须清掉。
- **同版本重打包不失效缓存（最易踩的坑）**：NuGet 全局包缓存按 `id + version` 计价 —— 版本相同但内容不同时下游 restore 不重新下载，会用本地构建的旧位编译/测试 ⇒ **假绿**。核验：比对 `<globalPackages>/{id}/{ver}/{id}.{ver}.nupkg.sha512` 与 `https://api.nuget.org/v3-flatcontainer/{id}/{ver}/{id}.{ver}.nupkg` 下载包的 SHA512（base64）。处置：移除该 `{id}/{ver}` 缓存目录 → `dotnet restore --no-cache` → `dotnet clean`（**不可省**，增量构建会把新旧程序集并置、运行时爆 `TypeLoadException`）→ `dotnet build`。

## 11 安全

- 绝不记录或暴露 `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`；日志脱敏。回调域同理：`WechatCallbackEvent` 的 `DecryptedXml` / `SuiteTicket` / `AuthCode` 不得写入日志、遥测或异常消息。
- **Query 承载凭据的脱敏登记**：`corpsecret` / `suite_access_token` / `provider_access_token` 被企微契约强制放在 **Query**，而组件 `SensitiveUrlRedactor` 是**精确匹配**词表 ⇒ G7 要求逐参数「补齐词表 / 登记豁免（附追踪号）」二选一，**新增任何 Query 凭据参数仍必须做该决策**。
- SDK 侧**不得抢占组件 `IExceptionRedactor`** —— 会丢掉 `Content`/`RequestContent` 的词表擦除 = 削弱安全默认。
- `WechatWorkException.RequestUri` 在构造期剥离 query 与 userinfo；不得把原始 URI 直接传出。
- 授权链接 state 校验：`get_customized_auth_url` 的 `state` ≤ 32 字节且仅 `[a-zA-Z0-9]`（`ValidateCustomizedState`）；安装链接 `state` ≤ 128 字节（`WechatAuthorizationUrlRequest.State`）。**两条规则不同，勿互相套用。**
- 回调入口必须保留抗重放两道闸，**不得为兼容降级为 fail-open**；不要绕过契约守卫与门禁（禁 `--no-verify`、禁删除断言）。

## 12 文档

方案与设计文档在 `.docs/`（中文）：授权功能方案、详细设计、产品规划、回调域功能完善/审查缺陷修复、多应用管理域审查缺陷修复、Redis 分布式存储模块方案、AI 代码审查提示词、`Mud.HttpUtils` 所需改动清单等。**`.docs/` 被 `.gitignore` 忽略**（fresh clone 无此目录），不要当作可外部引用的路径。**代码变更若触及契约面，须同批同步对应文档章节。**

## 13 提交前自检

1. 契约面改动是否**同批**更新了对应守卫（§7）？新增端点是否补了官方文档链接与业务警示？
2. 新增 DTO 是否跑了 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`（Abstractions 域手写登记）？
3. 新增公开配置属性是否有真实消费点（否则删除）？
4. 新文件是否落在 §5 的域目录 / 命名空间 / 注册入口三处一致？
5. `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1` 是否全绿？
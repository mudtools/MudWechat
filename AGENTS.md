# AGENTS.md

Guidance for AI coding agents working on the **Mud.Wechat** codebase.

## 1 概览

Mud.Wechat 是**企业微信（WeCom）**的 .NET SDK，架构对齐 `D:/Repos/MudFeishu/FeishuV3`（包家族、令牌基座、契约守卫、门禁脚本模式）。

| 应用类型 | `WechatAppType` | 令牌链 |
|---|---|---|
| 企业内部自建应用 | `Internal` | `access_token`（`corpid` + `corpsecret`） |
| 第三方应用（Suite） | `ThirdParty` | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发 | `Provider` | 同 `ThirdParty`，企业令牌走 `gettoken(corpsecret = permanent_code)` |

包家族（包名 = 命名空间）：`Mud.Wechat.Work`（主包）、`Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`、`Mud.Wechat.Work.Callback`、`Mud.Wechat.Redis`。

**改动方式（最重要的一条）**：能力增量一律「在既有域内加端点 + 同批更新契约守卫」，不引入新范式。契约面任何改动都会被守卫与门禁打红 —— **先打开对应守卫文件读它的断言，再动代码**；守卫不是「改完再补」的收尾项。

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
dotnet format Mud.Wechat.slnx                                  # 格式化（未纳入门禁）
```

**本机无 pwsh 7**：本环境改用 `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/xxx.ps1`（5.1 可跑全部脚本）。

### 2.1 门禁三步（`verify-build.ps1`）

1. **Release 全量构建** — 断言 `: error ` 计数 = 0、`NU1603`（依赖降级）计数 = 0。
2. **AOT strict 冒烟** — 逐源项目（排除 `Tests`/`Demos`/`obj`/`bin`，另按 `*.Generator.csproj` 后缀排除 Roslyn 生成器工程）以 `-c Release -f net8.0 -p:AotStrictMode=true --no-incremental` 构建，断言**编译错误 = 0 且 `IL\d{4}` 诊断 = 0**。生成器工程是 netstandard2.0 单 TFM（Roslyn 组件跨宿主硬约束），无运行时 AOT 语义，其正确性由步骤 1 全量构建（随 Callback 编译触发）+ 步骤 3 守卫闭环承担 —— 该排除是范围修正，**防假绿三条设置全部保留**。
3. **单元测试** — 逐测试工程单 TFM `net8.0`（`Tests/Directory.Build.props` 遮蔽根 props），断言 TRX 存在、`total > 0`、`failed = 0`。产物落 `test-reports/`（已 gitignore）。

**防假绿设置（删掉即假绿，不许「优化」掉）**：

- 步骤 2 必须**同时**断言「编译错误」与「IL 诊断」—— 构建本身失败时诊断计数仍是 0。
- `Get-ChildItem -Recurse` 必须保留 —— 否则找不到嵌套 `.csproj`，AOT 步骤静默空跑。
- `--no-incremental` 必须保留 —— MSBuild 的 `CoreCompile` 只比对输入/输出时间戳、**不比对 csc 命令行**，紧跟步骤 1 发起的 strict 构建会被判「已最新」而整体跳过编译。

**脚本编码**：`scripts/*.ps1` 为 UTF-8 **含 BOM** + 中文注释。无 BOM 的 UTF-8 中文脚本在 5.1 下被按 ANSI 解码，报 `Missing ')' in method call` / `The string is missing the terminator`（解析失败，非脚本逻辑问题）—— 改写脚本后确认编辑器没把 BOM 剥掉。
`audit-config-keys.ps1` **无 `-Strict` 开关**：任何「无消费点」的配置属性都会使其 `exit 1`。

### 2.2 CI（`.github/workflows/dotnet-publish.yml`）

顺序：Build → **诊断白名单断言** → `verify-build.ps1 -SkipTests` → `audit-config-keys.ps1` → `dotnet test --no-build`（TRX 解析摘要）。

- 必须为 **0** 的日志桶：`NU1603`、`CS1750`、`HTTPCLIENT0\d\d`、`FORM0\d\d`、`EHSG0\d\d`、`MUD00[1-4]`、`IL\d{4}`、`AOT00[1-5]|AOT007`。
- **断言为 0 即假红**的两桶（CI 只 INFO 打印计数）：`AOT006`（未登记的 `[HttpJsonSerializable]` DTO，strict 步骤才升为 Error —— 它本身就是漂移守卫）、`MUD005`（Query 令牌注入，企业微信官方契约要求）。
- 发布链：`workflow_dispatch` → semantic-release 改写 `Directory.Build.props` 的 `<Version>`（**版本唯一来源**）→ tag `v*` → `dotnet pack` 后断言恰 **5 个 nupkg**（Tests 均 `IsPackable=false`）→ OIDC 临时 Key 推 nuget.org。少包即 fail-closed，不静默发漏包。
- `DOTNET_VERSIONS` 必须**多行**（每行一个 SDK 版本）；写成空格分隔会静默归一为单版本，随后以「testhost 无法启动」形式炸开。

## 3 框架与语言约束

`Directory.Build.props`：`TargetFrameworks = netstandard2.0;net6.0;net8.0;net10.0`、`LangVersion = 13.0`、`Nullable = enable`、`ImplicitUsings = enable`、`TreatWarningsAsErrors = false`、`Version = 1.0.3`。

**`netstandard2.0` 无 `IsExternalInit` polyfill**，该 TFM 下：

- **禁 `init` 访问器、禁 `record` 类型 / `with` 表达式**（报 `CS0518`/`CS0656`）—— 用 `get; set;` 或构造器。
- 无 `ArgumentNullException.ThrowIfNull` —— 用 `x ?? throw new ArgumentNullException(nameof(x))`。
- `string.IsNullOrEmpty` / `IsNullOrWhiteSpace` **无 `[NotNullWhen]` 标注**，流分析不收窄可空引用 —— 显式 `x == null` 判断或 `is { Length: > 0 }` 模式。
- 条件编译用 `#if NET6_0_OR_GREATER` / `#if NET8_0_OR_GREATER`；**禁 `Math.Clamp`**（用 `Math.Min/Max`）。

`netstandard2.0` 存在**大量既有** CS86xx 可空警告（Abstractions 令牌/仓储/回调相关文件）。属既存形态，门禁只统计编译错误与 AOT IL 诊断，**不因 CS 警告失败** —— 不要为消警告做大范围重构。

**`Tests/Directory.Build.props` 遮蔽根 props**（测试工程单 TFM `net8.0`）：新增任何治理属性（如 `WarningsAsErrors`）必须同时写入该文件，否则测试工程成为门禁盲区。

## 4 AOT / Trim 合规

- `net8.0` / `net10.0` 默认启用 AOT/裁剪分析；`AotStrictMode=true` 把 `IL2026;IL2046;IL2050;IL2057;IL2067;IL2070;IL2072;IL2075;IL2080;IL3050` 提升为错误；`WarningsAsErrors` 常驻含 `AOT001;AOT002;AOT003;AOT004;AOT007`。
- **禁止**调用反射版 `JsonSerializer.Serialize<T>(T, JsonSerializerOptions)` / `Deserialize<T>` —— 一律走 `JsonTypeInfo`（域 `JsonContext`）或 `WechatJsonResolverExtensions` 合并解析器。
- **JSON 上下文是生成物**：DTO 标 `[HttpJsonSerializable]`（`SerializerClassName` = 命名空间域段，根命名空间直属文件归 `Common`），`Generated/*JsonContext.g.cs` 由 `scripts/GenerateJsonContext.ps1` 生成，**提交进版本控制、勿手改**；上下文为 `internal`，经 `InternalsVisibleTo` 供主包与测试直读。主包 `WechatJsonResolverExtensions` 合并 **21 个生成上下文 + 1 个手写上下文**（`Abstractions` 的 `Authentication/Models/AuthenticationJsonContext.cs`）。
  - **新增 `[HttpJsonSerializable]` 类型必须同批重跑** `AddHttpJsonSerializable.ps1`（标注）+ `GenerateJsonContext.ps1`（登记；Abstractions 域则手写登记进 `AuthenticationJsonContext`），否则 `AotStrictMode=true` 下 `AOT006`（error）直接打红门禁步骤 2。
  - 已知边界：开放泛型 `WechatChatbotResponse<>` 不登记（STJ 源生成器不生成其元数据，SYSLIB1030）；关闭 `--auto-derived-types`；工具运行期 `AOT003`（多态缺 `[JsonDerivedType]`）在本仓库为**已知误报** —— 反序列化目标恒为具体 DTO 类型。
  - 核对工具：`dotnet tool install -g Mud.HttpUtils.JsonContextScaffolder` → `mud-jsonctx --project Mud.Wechat.Work.DataModels\Mud.Wechat.Work.DataModels.csproj --dry-run`。
- 配置绑定为**源生成**（`EnableConfigurationBindingGenerator=true`）：
  - 配置 DTO **禁 `required`**（生成器以 `new T()` 构造 → `CS9035`）；校验写进 `Validate()`（见 `WechatAppConfig`）。
  - 绑定必须走 `Configure<T>(o => section.Bind(o))`，**不要**用 `Configure<T>(IConfiguration)` 重载 —— 其反射绑定调用点无法被源生成器拦截，会破坏 `IL2026`/`IL3050` 净零。
- `UnconditionalSuppressMessageAttribute` 在 `net10.0` 为 `internal`，用户代码不可引用；必要时 `#pragma warning disable IL2026, IL3050` 并附理由注释。

## 5 结构与依赖方向

```
Mud.Wechat/
├── Mud.Wechat.Work/              # 主包：接口声明、服务、DI、模块注册；Interfaces/{域}/ 按功能族分目录
├── Mud.Wechat.Work.Abstractions/ # 令牌基座、多应用、配置、存储端口、枚举、异常、回调事件信封
├── Mud.Wechat.Work.DataModels/   # 官方 DTO（[HttpJsonSerializable]）+ Generated/ 域 JsonContext（生成物）
├── Mud.Wechat.Work.Callback/     # 回调接收（AES 解密、事件解析、分发）+ HTTP 中间件；Events/ 强类型事件 DTO
├── Mud.Wechat.Work.Callback.Generator/  # 回调契约登记生成器（IsPackable=false；依据 [WechatCallbackContract] 发射 RegisterAll）
├── Mud.Wechat.Redis/             # 四个存储端口的 Redis 实现 + 连接基座 + DI 编排
├── Tests/                        # 5 个测试工程，镜像源结构，单 TFM net8.0
├── scripts/                      # verify-build / audit-config-keys / GenerateJsonContext / AddHttpJsonSerializable
├── .docs/                        # 方案与设计文档（中文；已 gitignore，fresh clone 无此目录）
├── Directory.Build.props         # 全局 MSBuild 属性（版本唯一来源）
└── Mud.Wechat.slnx
```

**依赖单向**：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`、`Redis → Abstractions`。
硬边界：**`Callback` 不得引用主包 `Work`**；**`Redis` 不得引用主包 / `Callback`**。

**域 → 目录 / 命名空间 / 注册入口**（新文件必须照此落位）：

| 域（接口族数） | 接口目录 | DTO 目录（命名空间后缀） | 模块 / 注册入口 |
|---|---|---|---|
| 通讯录（6） | `Interfaces/Contacts/`（Export 独立子目录） | `Contacts/{Users,Department,Tags,ContactRules,Batch,Export}/` | `Contact` / `AddContactApi()` |
| 客户联系（6） | `Interfaces/ExternalContact/` | `ExternalContact/{FollowUser,Customer,Tag,JobInheritance,ResignedInheritance,GroupChat}/` | `ExternalContact` / `AddExternalContactApi()` |
| 上下游（3） | `Interfaces/CorpGroup/` | `CorpGroup/{基础,ChainContacts,Rules}/` | `CorpGroup` / `AddCorpGroupApi()` |
| 安全管理（3） | `Interfaces/Security/` | `Security/`（单一命名空间） | `Security` / `AddSecurityApi()` |
| 消息推送（4） | `Interfaces/Message/` | `Message/`（单一命名空间） | `Message` / `AddMessageApi()` |

- 接口命名空间一律 `Mud.Wechat.Work`（**不含** `Interfaces` 段）；DTO 命名空间为 `Mud.Wechat.Work.DataModels.{域}[.{子域}]`。
- `RequestModel/`、`ResponseModel/` **仅作目录组织，命名空间不含该目录段**。
- **唯一例外**：通讯录 Users 域命名空间为 `Mud.Wechat.Work.DataModels.Contracts.Users`（与目录名不一致，属既存形态，新增 DTO 沿用至统一决策）。
- 模块注册三段式：`WechatModule.{域}` 枚举值 + `Add{域}Api()` + 生成器 `Add{域}WebApiHttpClient()`，注册组名与域同名。

## 6 领域契约（不可违反）

### 6.1 令牌与凭据

- **K1 令牌分流**：代开发 `permanent_code` 语义是「应用 secret」→ `gettoken(corpsecret = permanent_code)`；第三方 `permanent_code` 是「授权码」→ `get_corp_token`。二者都在 `CorpTokenManager` 内按 `AppType` 分流，**不新建管理器**。
- **K2 模板 id**：代开发 `template_id` 即 `suite_id`（`dk` 开头）⇒ `WechatAppConfig` **不提供独立 `TemplateId` 配置项** —— 「模板 id ≠ suite_id」这一非法状态在类型层面不可表达。需要模板 id 的**接口参数**（如 `get_customized_auth_url` 的 `templateid_list`）由宿主显式传入，与配置面无关。
- **令牌注入统一走 Query**（企业微信契约，非 Header），触发组件 `MUD005`（已知接受风险）。白名单由守卫 G5 锁定为：授权流接口 + 通讯录六域 + 客户联系六域 + 上下游三接口族 + 安全管理三接口族 + 消息推送四接口族；**新增 Query 注入接口必须先评估、再显式扩展 G5**。
- v2 端点：`/cgi-bin/service/v2/get_permanent_code`、`/cgi-bin/service/v2/get_auth_info`；`get_customized_auth_url` 以**显式 Query 参数** `provider_access_token` 传令牌（**不带 `[Token]`**，不放宽白名单）。授权安装链接前缀：`https://open.work.weixin.qq.com/3rdapp/install`。
- **`TokenKey` 三段式 `{tokenType}:{appKey}:{scopeKey}`**（如 `Wechat.AccessToken:default:default`，由 `WechatAppTokenManagerBase.BuildCache` 的 `storeKeyMapper` 构造）；`WechatTokenTypes` 一律 `"Wechat."` 前缀，与组件通用 `TokenTypes` 隔离。
- **`AppKey` 形状受约束**：`[A-Za-z0-9]` 开头 + 仅 `[A-Za-z0-9._-]` + ≤128（`WechatAppKeyValidator`，经 `WechatAppConfig.Validate()` 单点收敛）。理由：AppKey 参与持久化键与命名 HttpClient 名，含 `:` 会造成**键别名** ⇒ 跨应用令牌串号。
- **企业级令牌一企一份**（由 `TokenManagerBase` 的 scope 机制承担，`scopeKey = authCorpId`），**不经声明式 `[Token]`**：宿主/编排服务显式 `GetTokenAsync(new[]{ authCorpId })` 获取。errcode 恢复**必须显式传 scope**（`InvalidateTokenAsync(appKey, AccessToken, new[]{ authCorpId })`）—— 以默认作用域失效对已缓存的企业令牌是**空转**（能力边界，由 `CorpTokenManagerScopeIsolationTests` 锁定）。
- `CorpTokenManager` 读取环境 `SetCorp` 上下文须过**两级校验**：① 上下文 `AppKey` 归属一致；② 上下文 `authCorpId` 与 scope 一致。`WechatCorpContext.SetCorp` 的 `authCorpId` **必填**（null/空白即抛），与 `appKey=null`「未声明归属」语义显式区分；`InvalidateTokenAsync` 的 `scopes` 非空但全为空/空白串判为编程错误 fail-fast（`scopes=[]`/null 保持「全部」既有语义）。

### 6.2 存储端口与多实例

- `IWechatCorpAuthStore` 为**复合键 `(AppKey, AuthCorpId)`**；`IWechatSuiteTicketStore` **按 `suiteId` 分槽**（多套件/多代开发模板互不覆盖）。
- 默认实现仅进程内 ⇒ 多实例部署**引用 `Mud.Wechat.Redis`**（`AddWechatRedis`，**必须先于** `AddWechatApp`/`AddWechatCallback` 调用，颠倒顺序即注册期 fail-fast）或由宿主提供分布式实现（`TryAdd` 前置注册覆盖；宿主预注册的自定义实现按契约胜出）。
- 四个存储端口（含 `IWechatCallbackReplayGuard`，**接口本体落 `Abstractions.TokenManager` 域**；InMemory 实现留 `Callback` 包）由 Redis 包单依赖 Abstractions 实现并共享连接基座。
- 退役清库批量能力：`IWechatTokenStoreBatchRemove : IWechatTokenStore`（可选实现）—— 管理器先 `is` 探测，命中走一次 `RemoveRangeAsync`，未实现回退逐键（宿主自定义 store 零破坏）；形态为「调用方算键 + 实现方批删」（键按中间段 appKey 匹配，前缀扫描不适用）。
- **SE.Redis 3.3.0 两个陷阱**（改 Redis 包代码必读）：① `RedisTimeoutException` 直接继承 `TimeoutException` 而非 `RedisException` —— 存储层捕获须用 `WechatRedisErrors.ShouldWrap`（`is RedisException or RedisTimeoutException`），裸 `catch (RedisException)` 会漏超时；② `StringSetAsync` 存在经典 4/5 参（无默认值、隐藏）、keepTtl 6 参、`Expiration`/`ValueCondition` 全默认四套重载 —— **生产调用一律以命名参数显式钉住形态**（`keepTtl: false` / `when: ...`），裸位置参的重载决胜不确定。

### 6.3 多应用基座与 DI 装配

- **DI 桥接不变量**：`IAppContextHolder`、`IAppContextSwitcher`、`IWechatAppContextSwitcher` **必须是同一实例**；组件 `AddMudHttpClient` 内部会 `TryAdd IAppContextHolder`，故 SDK 的注册**必须先于** `AddMudHttpClient`。破坏该不变量 ⇒ 声明式 `[Token]` 客户端读到的环境上下文恒为 `null`，多套件静默回退默认应用令牌。
- **`WechatAppManager` 直接实现 `IAppManager<IWechatAppContext>`**（**不继承**组件 `DefaultAppManager<T>` —— 基类另有影子注册表 `_apps` 与非 virtual 写入口，会让注册静默写进读不到的表）。故：注册表**单一来源** = 本类 `_configs` + `_lazyContexts`；`RegisterApp`/`UpdateApp`/`RegisterSwitcherFactory` 显式 `NotSupportedException`。
- 配置读取一律走 `ConfiguredConfigs` / `TryGetConfig`（**非物化**）；`TryGetApp` 会构造命名 HttpClient/DI scope/Timer，仅用于「确实需要上下文」的场景（回调的 `SuiteId → appKey` 匹配**不得**使用它）。
- 已锁定的行为细则：全部应用经 `RemoveApp` 移除后 `DefaultAppKey` 置 `null`（对齐组件契约）、`GetDefaultApp` 抛明确错误、重新 `AddApp` 时兜底提升（`??=`）；重建白名单仅**瞬时 IO 型异常组**（`HttpRequestException`/`TimeoutException`/`IOException`/`SocketException`），`InvalidOperationException` 属确定性装配错误直抛不重建；退役队列脱泵 fire-and-forget（飞行登记）+ 停机限时（默认 5s）排空，停机后到达的退役入队立即 `Dispose`、下线清库任务丢弃（令牌随 TTL 过期）；`RemoveApp` 删除顺序**恒为 `TryRemove` 先于配置删除**（「返回 false ⇒ 零突变」）；`CreateAppContext` 经 `WechatOwnedResourceTracker` 登记 scope 之外产物，装配中途失败逆序确定性回收后原样重抛。

### 6.4 授权编排与回调自动化

- **授权编排** `IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举）：换码**单飞门 + 结果记忆**以 **`(appKey, authCode)` 复合键**为粒度（含长度前缀拼接）；共享任务用 `CancellationToken.None` 承载，各调用者经 `AwaitSharedAsync` 独立取消；飞行条目**仅创建者移除**。`RevokeAuthorizationAsync` 顺序为**先失效令牌、后删库**，且**仅失效本 `appKey`** 令牌。策略统一落 `WechatAuthorizationOptions`（`WechatAuthorization` 节），**不得**把编排字段塞进 `WechatAppConfig`。
- **回调自动化协调器**：`IWechatAuthorizationCoordinator` 由 Abstractions 定义、**主包实现**，`Callback` 经 `IServiceProvider.GetService<>()` **惰性可选解析** —— 未安装主包授权模块时首次 `Warning` 后**降级不抛**。`change_auth` 本地无记录时仅告警、**不发 `get_auth_info`**；`cancel_auth` 清理范围**恒为 `SuiteId` 命中集**：未命中（含 `SuiteId` 缺失）**只告警不删库** —— `(AppKey, authCorpId)` 是**每套件独立**的授权记录，回退「全部应用清理」会删除其它套件的有效授权。三个逐 appKey 循环（换码/刷新/撤销）**单应用失败必须记 Error 后继续**；`OperationCanceledException` **不在捕获面**（取消必须穿透）。

### 6.5 回调接收面

- **包形态**：`Callback` 引入 ASP.NET（`netstandard2.0` 用 `Microsoft.AspNetCore.Http 2.3.9` 包、`net6+` 用 `FrameworkReference`）；中间件为**经典约定式**（`RequestDelegate`，不进 DI），宿主 `UseWechatWebhook()` 一行接入。
- **凭据与路由**：多应用凭据 = `WechatCallbackOptions.Apps` 字典（**唯一来源**；`WechatAppConfig` 无 Push 属性、应用级配置无 `AppKey` 属性），路由 = `/{GlobalRoutePrefix}/{AppKey}`；`AddWechatCallback` 注册并返回建造者链式注册处理器/拦截器（`IConfiguration` 重载为惰性委托绑定 `Configure(o => section.Bind(o))`，AOT 安全；`IOptionsMonitor` 请求期热更——路由前缀与应用凭据均支持热更，**勿改回注册期急切绑定**，会冻结热更）；配置校验 = 请求期单应用校验（`ResolveApp` 命中后 `app.Validate(appKey)`，单应用配置错误不拖垮全部回调路由）+ `WechatCallbackOptions.Validate()` 宿主启动期全量校验入口；接收失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 映射失败类别）。**不得引入「接收方 ID 统一注册表 / `IWechatCallbackUrlVerifier`」形态** —— 多套件能力由「每套件一个 `Apps` 条目 + 各自独立 Token/AESKey/接收方 ID」覆盖。
- **加解密**：按官方 **32 字节块 PKCS7** 手工补位/剥离；**禁用 .NET 内置 16 块 `PaddingMode.PKCS7`**（内置 16 块校验会误拒官方 pad∈[17..32] 报文）。
- **抗重放（两道 fail-closed 闸）**：① 时间戳时效窗口 ±300s（缺失/非数字即拒）；② 一次性指纹去重（SHA1 指纹，不得落盘密文本身）。**指纹闸位于「解密 + receiveid 校验成功」之后、事件返回之前** —— 解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信；解密失败不消耗指纹，官方重试可重新进入管线。**不得**把指纹闸移回解密之前，也不得绕过。**GET URL 验证（echo）只过时效闸、不消费指纹**（同 echostr 二次保存配置必须成功）。分布式实现（`Mud.Wechat.Redis`）故障时守卫异常**必须上抛**（→ 回调 5xx → 官方 96238 重试）；**禁止**吞异常返回 `true` 放行重放、或返回 `false` 静默丢事件；空键返回 `false` 且不触达存储。
- **事件信封与事件键**：`WechatCallbackEvent` / `IWechatCallbackEventHandler` / `IWechatCallbackEventInterceptor` / `WechatCallbackEventTypes` 落 `Abstractions.Callback`（该目录**含子目录，不得出现 XML 类型**，守卫 CB7 已递归）；XML→信封解析留 `Callback.WechatCallbackReceiver`。`EventTypeKey` = `InfoType`（非空）→ `ChangeType` → `Event`；处理器 `SupportedEventType` 空串 = 兜底（内置处理器即此形态，文件**留 `Callback` 包根目录**）。`AuthCorpId ← FromUserName` 兜底**仅限授权族**（`change_contact` 的 `FromUserName` 固定 `sys`，无差别兜底会伪造授权企业）。信封另带 `AppKey` / `AppType` / `Channel`（只读快照，**配置权威仍是** `WechatAppManager` / `WechatCallbackOptions`）。
- **事件载荷体系（v2.2；P2 后嵌套与登记全面声明化）**：**不得**再新增「逐事件 DTO + 手写 `ParseXxx` 方法」，也**不得**手写多级嵌套解析或手改 `OfficialPayloadContracts.RegisterAll` 方法体。载荷按官方**报文结构族**建（`Events/Payloads/`，13 型 + `GenericCallbackPayload`），字段映射由**上游** `Mud.HttpUtils.PayloadFieldMapGenerator` 依 `[PayloadContract]`/`[PayloadField]` 在编译期生成 ⇒ 元素名 ↔ 属性名配对受编译器校验。要点：
  1. 载荷类型须 `partial` 且标注 `[PayloadContract(Converter = typeof(WechatPayloadConverter))]`（转换器落 `Abstractions/Callback/Payloads/`，纯转换语义、零 Callback 依赖）；
  2. 转换器标量方法须 **`static`、非泛型、恰 1 参**（首参 `PayloadNode` 或 `string?`，生成器按首参类型决定传 `n` 还是 `n?.Value`）；**多级嵌套走 G-ADR-17 声明化通道**：单对象用 `Object<TSingle>`（内层 DTO 标 `[PayloadContract]`、属性须可空标注、禁 `ItemName`），对象列表用 `ItemsObject<TItem>`（须 `ItemName`、元素实参非可空）—— 二者由本仓 `WechatPayloadConverter` 提供（内层字段递归 `Bind`），嵌套 DTO 升级 `partial` + `[PayloadContract]` 后映射表自动生成，`WechatCallbackExtAttrItem`（`ItemsWithAttributes` 形态）除外；
  3. 映射表须在**具体类型**处取 `XxxPayload.PayloadFieldMap` —— C# 禁止泛型上下文访问类型参数静态成员（CS0712），且 `static abstract` 需 net7+（本仓含 ns2.0 不可用）；
  4. 类型化处理器继承**抽象基类** `WechatCallbackPayloadHandler<TPayload>` —— 「泛型接口＋显式默认实现」的桥接模式是 C# 8 默认接口实现，`netstandard2.0` 报 **CS8701**；
  5. `Callback.csproj` 必须**显式**引用 `Mud.HttpUtils.Generator`（Abstractions 的引用带 `PrivateAssets="all"`，**不流向** Callback）与**本仓生成器工程** `Mud.Wechat.Work.Callback.Generator`（`OutputItemType="Analyzer"`，`IsPackable=false` 不进「恰 5 nupkg」）；
  6. 上游生成器引用与 `Mud.HttpUtils` 包版本须**全仓单一**（守卫 `MudHttpUtils_PackageReference_ShouldBeSingleVersion`，NU1605 视为错误）。
  7. **契约登记单一来源（P2）**：事件键 + 族前置条件 + 事件键级开放面声明在载荷类的 **`[WechatCallbackContract]` 特性**（`AllowMultiple`：同载荷不同键子集开放面不同时叠加声明，如 `PlainEventPayload` 两段）；本仓生成器发射 `OfficialPayloadContracts.RegisterAll` 方法体（每键一条 `CreateWithOpenSurface`，`requiredEvent` 缺省 = 逐键自指）。运行期「不宽于族默认」校验不变（CB4e）；**特性声明不完整由生成器诊断 MUDCB001 打红**（开放面/通道/事件键缺失）。
  - **载荷目录归类（源文件分目录，命名空间不分段）**：`Events/Payloads/` 下按官方事件族分子目录 —— `Contacts/`（通讯录变更族）、`CorpGroup/`（上下游变更族）、`AsyncJobs/`（异步任务族）、`Messages/`（消息与事件族 90240 共 8 型）、`Contracts/`（跨族基座：`OfficialPayloadContracts` partial 声明）。**目录仅作组织**（同 `RequestModel/`/`ResponseModel/` 口径）：命名空间恒为 `Mud.Wechat.Work.Callback.Events.Payloads`，**不随目录分段** —— 宿主 `using` 与守卫（CB4d/CB23 按文件名递归定位）均不受分目录影响。新增载荷按事件族落位，勿再平铺回 `Payloads/` 根。
- **注册表与分发**：组合根期急切注册、**无 Freeze**；通配键 `"*"`（`WechatCallbackOptions.WildcardAppKey`）双重语义 = 通讯录同步助手路由 + 全局处理器/拦截器桶；匹配顺序：appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底。同步分发 + 软超时（默认 4500ms，**必须 < 企业微信 5s 契约**）；超时/拦截器中断 → 503 触发重推；指纹在分发前消费，重推同指纹将被 403（fail-closed 优先于 at-least-once，**处理器须幂等**）；单处理器异常隔离（LogError 后继续，结果仍 Handled）；`MaxConcurrentEvents` 信号量容量为构造期快照（热更不改容量）。
- **`WechatAppCallbackOptions` 配置面**：`PushToken`/`PushEncodingAESKey`/`ReceiveId`/`AppType`/`Channel`（**必须与主配置同类文件**才会纳入 audit 扫描）。`ReceiveId` 是「接收方 ID」：企业自建回调填企业 `CorpId`、**套件回调填 `SuiteId`**；非空时校验解密明文的 `receiveid`，不一致即拒；留空（通讯录同步助手）或明文未携带 `receiveid` 时跳过校验并一次性告警（官方「个人主体第三方为空串」兼容）。
- **应用类型 × 回调通道**：`AppType`（`WechatAppType`，默认 `Internal`）+ `Channel`（`WechatCallbackChannel`，`App=1` 应用数据通道 / `Suite=2` 套件指令通道，默认 `App`）；`Validate()` 拒绝「自建应用占用套件通道」「套件通道非第三方/代开发」两类非法组合。`ValidateReceiveId` 按三元分流：自建 App / 第三方·代开发 Suite = 静态 `ReceiveId`；第三方·代开发 App = 动态授权企业 CorpId（比对外层 `ToUserName`）。`IsEventFamilyAllowed` 开放面矩阵：授权族→Suite+第三方/代开发；上下游→App+自建；通讯录/异步→App；Unknown→不拦截。合法性闸先于拦截器 `BeforeHandleAsync`，不适用族返回 `Rejected`（→200 不重推）。
- **回调事件载荷**（`Callback/Events/Payloads/`）**不复用** DataModels 的 JSON DTO（XML vs JSON、逗号/竖线串 vs List、权限降权语义三重差异）。
- **三模式无关性（v2.2 ADR-14）**：企业自建 / 第三方 / 服务商代开发的报文**结构同一**，差异只是「值是否出现」⇒ 一份可空超集载荷覆盖三模式；**载荷与转换器层禁止出现** `WechatAppType` / `WechatCallbackChannel` 分支（守卫 CB4d）。需按模式分支时在**处理器层**读 `evt.AppType`。
- **事件键级开放面（v2.2 ADR-15；P2 后声明在载荷特性）**：官方开放面的真实粒度是**事件键**而非事件族。`IsEventFamilyAllowed` 保留为**族级默认**，事件键级声明（`SupportedAppTypes` / `RequiredChannel` / `RequiredEvent` / `RequiredFamily`）在其之上叠加；两道闸均**先于**拦截器 `BeforeHandleAsync`（CB13b）。宿主注册新 `Event` 值会落 `Unknown` 族而被族闸放行 ⇒ **必须**在载荷类 `[WechatCallbackContract]` 特性显式声明，不得依赖族默认；CB4b 双面锁定（官方清单锚点 + 特性并集一致性）。
- **上下游事件族**：`Event=change_chain` + 9 个 ChangeType（空间/分组/企业三族），信封带 `ChainId`、`IsChangeChain`；载荷按**官方报文结构族**建 **1 型**（`ChainChangedPayload`，含可空 `GroupIds`/`CorpIds`，ChangeType 经信封判别）；`batch_job_result` 官方存在**双报文布局**（通讯录 90973 顶层节点 vs 上下游 95797 `BatchJob` 包装节点）⇒ 契约声明 `ScopeFallback = "BatchJob"`，三级作用域判定由上游 `ResolveScope` 完成。**`JobType` 级差异属语义过滤、不得做成安全闸**（官方开放面约束的是事件面，非某个 `JobType` 值）⇒ 处理器按需自行判别。开放面仅自建应用（需配置到「上下游-可调用接口的应用」）；上下游系统应用自身触发的变更不回调。

### 6.6 消息推送落位决策

- msgtype/card_type **不做运行时多态**（AOT 源生成按声明类型序列化）：每个 msgtype 一个端点方法 + 请求 DTO（同路由多方法），官方各 msgtype 参数表差异（`safe` 有无、id 转译支持面、`mentioned_list` 仅群聊）在 DTO 层面精确表达；模板卡片 `TemplateCardBody` 为发送/更新两端点共用的扁平结构（`card_type` 判别 + 全可选嵌套，`replace_text`/`disable` 仅更新接口支持）。
- **响应形态陷阱**：`message/send` 的 `invaliduser` 为**竖线分隔字符串**、`update_template_card` 的为**字符串数组** ⇒ 两类响应 DTO 不共用；学校通知响应为 `invalid_parent_userid` / `invalid_student_userid` / `invalid_party` 三个数组。
- **「接收消息与事件」不是本域接口**：消息接收是企业微信回调推送（XML），由 `Mud.Wechat.Work.Callback` 承载；本域全部是发送侧 HTTP API。

## 7 契约守卫（`Tests/**/ContractGuards/`）

**规则**：改动契约面（接口/端点/路由/DTO/注册组/配置面）必须**同批**更新对应守卫。守卫是路由表、端点计数、分层形态、令牌绑定、JSON 上下文登记的**权威描述** —— 本文档不复述路由字符串，**改契约时直接打开守卫源文件核对**；各接口的官方文档 URL/ID 也已落在接口 XML 注释中（新增端点照此格式补链接）。

### 7.1 通用守卫（`WechatContractGuards.cs`，G1~G9）

| 编号 | 锁定的约束 |
|---|---|
| G1 | 全仓库 `Mud.HttpUtils*` 单一版本（防混版 `TypeLoadException`） |
| G2 | 配置 DTO 禁用 `required` |
| G3 | `WechatTokenTypes` 一律 `"Wechat."` 前缀 |
| G4 | 失效码 `{40014,42001,42007,42009,42011}` 与判定器同源 |
| G5 | Query 令牌注入白名单未放宽；**应用类型子接口仅覆盖官方实际开放的应用类型**（官方无对应 API 的类型不设子接口） |
| G6 | 授权端点路由与官方一致；`get_customized_auth_url` 不带 `[Token]` |
| G7 | Query 承载凭据的参数名 ⊆ 组件脱敏词表 **∪** 显式豁免清单（豁免须附追踪号；**豁免自过期** —— 一旦被组件词表覆盖即失败，不得静默遗留） |
| G8 | DI 桥接三接口同实例（源码顺序断言 + `WechatServiceCollectionExtensionsTests` 运行期用例） |
| G9 | `cancel_auth` 不得引入「未命中回退全部应用」的越权删除 |

### 7.2 域守卫

| 守卫 | 域 | 锁定的不变量 |
|---|---|---|
| U1~U4 | 通讯录·成员管理 | 路由表 23 条（子接口重复声明以 `DeclaredOnly` 限定）；父接口 `IsAbstract` + 三子挂 `Contact` 组并 `InheritedFrom` 父实现类 + 代开发零端点；`Wechat.AccessToken` + Query 注入；DTO 已登记 JSON 上下文 |
| D1~D4 | 通讯录·部门管理 | 与 U1~U4 同构，路由表 9 条；代开发零端点；JSON 上下文 8 型 |
| T1~T4 | 通讯录·标签管理 | 官方三类应用开放完全一致 ⇒ 全收敛父接口（7 条）；**三个子接口须零端点**（任何新增 = 能力漂移，先核官方文档） |
| CR1~CR4 | 通讯录·查看权限 | 父接口零端点 + Internal 恰 4 条（全 POST）；**不设第三方/代开发子接口** |
| B1~B4 | 通讯录·异步导入 | 父接口 4 条（3 POST + 1 GET）；**不设代开发子接口**，自建/第三方子接口零端点 |
| E1~E4 | 通讯录·异步导出 | 父接口 5 条（4 POST + 1 GET）；三子接口零端点 |
| FU1~FU4 | 客户联系·企业服务人员 | 父接口 1 条 + 第三方/代开发各 1 条差异端点；自建零端点 |
| CU1~CU4 | 客户联系·客户管理 | 父接口 10 条 + 第三方 3 条身份转换差异端点；自建/代开发零端点 |
| CT1~CT4 | 客户联系·客户标签 | 全收敛父接口（9 条，全 POST）；三子接口零端点 |
| JI1~JI4 | 客户联系·在职继承 | 全收敛父接口（3 条）；三子接口零端点 |
| RI1~RI4 | 客户联系·离职继承 | 全收敛父接口（4 条）；三子接口零端点 |
| GC1~GC4 | 客户联系·客户群 | 全收敛父接口（3 条）；三子接口零端点 |
| CG1~CG4 | 上下游基础 | 父接口 6 条（全 POST）；三子接口零端点 + **继承链上恰好只有自建/第三方/代开发三个子接口** |
| CG5~CG8 | 上下游通讯录 | 父接口 4 条 + 自建 5 条 + 代开发零端点（`DeclaredOnly` 限定声明位置） |
| CG9~CG11 | 上下游规则 | 父接口零端点 + Internal 恰 5 条、无其它子接口 |
| SEC1~SEC4 | 安全管理 | 三接口族父接口零端点 + 自建恰 9/5/2 条；**不设第三方/代开发子接口**（官方无文档） |
| MSG1~MSG4 | 消息推送 | 发送应用消息族父接口 13 条 + 第三方恰 1 条（自建/代开发零端点）；AppChat / SchoolMessage / SmartSheetGroupChat 父接口零端点 + 仅 Internal 承载端点；四族「继承链上恰好只有既定子接口」漂移断言 |
| CB1~CB13 | 回调 | 包依赖边界、事件键 48 个与 `EventTypeKey` 优先级、兜底处理器形态与文件路径、事件 DTO 字段、凭据唯一来源、echo 不消费指纹、信封上移边界、32 字节块填充（禁 `PaddingMode.PKCS7`）、指纹闸次序、通道枚举与配置面、`receiveid` 三元分流、开放面矩阵、合法性闸次序 |
| MA1~MA4 | 多应用管理（`Abstractions.Tests`） | `RemoveApp` 删除顺序、重建异常白名单、退役队列 `_disposed` 闸、`SetCorp` 参数校验。守卫为**方法体文本断言**（花括号配平），签名漂移须同步更新 |
| RD-G1~RD-G6 | Redis（`Redis.Tests`） | SCAN 模式仅经 `WechatRedisKeyBuilder.Pattern` 单一出口、配置无 `required`、重放守卫 fail-closed 上抛、凭据不进日志、全名探测防漂移、单依赖 Abstractions |

### 7.3 官方契约陷阱（新建/修改端点时必须核对）

- **拼写陷阱**：域名 IP 响应字段 `universal_domian`（官方原文如此）；`admin_oper_log` 参数表游标拼作 `cusor`（SDK 以官方 JSON 示例为准用 `cursor`）；在职群接替为 `groupchat/onjob_transfer`（"onjob" 拼写属官方契约）vs 离职群接替为 `groupchat/transfer`；`corpgroup/getresult` / `batch/getresult` / `export/get_result` 三处拼写互不相同（另 `batch/get_by_user` 亦易混）。
- **接口 XML 注释必须保留的官方警示**（不得删减或弱化）：
  - 异步导入：全量覆盖成员会**删除文件外成员**（官方对删除比例有熔断）。
  - 客户管理：规则组 create/edit **仅支持串行调用**，且仅能管理本应用创建的规则组。
  - 在职/离职继承：90 自然日内每位客户/每个客户群仅可被转接 2 次；客户每次 ≤100、客户群每次 1~100、每人每天客户群 ≤300；离职继承 `transfer_customer` 返回 errcode 0 **仅表示开始分配流程**（24 小时后自动接替），最终状态须查询接替状态。
  - 客户群管理：`groupchat/list` **必须指定 `owner_filter`**（可见范围超 1000 人报 81017，群主为离职成员时必须指定）；旧版 `offset + limit` 分页将废弃，须用 `cursor + limit`。
  - 上下游通讯录导入：**只允许串行且同时只能存在一个导入任务**。
  - 上下游规则：新增/更新规则**共用每天 1000 次额度**。
  - 安全管理：操作日志 600 次/分钟、跨度 ≤7 天；高级功能分配/取消为**异步任务**（jobid 查询）。
  - 消息推送：应用消息每应用「账号上限数 × 200」人次/天、同一成员 30 次/分 + 1000 次/时（超限丢弃）；`appchat/send` 每企业 2 万人次/分 + 规模分档小时额度、成员级 200 条/分 + 1 万条/天（超限**静默丢弃不报错** ⇒ 推送成功 ≠ 全员送达）；创建群 1000 个/天、修改群 1000 次/小时；智能表格群聊 `update` 同一群聊修改必须串行、并发上限 10。
  - 权限分层：安全管理各子域、客户标签与规则组各自独立配置「可调用接口的应用」/权限，可见范围外数据被过滤。

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

- **文件域命名空间**（`namespace X.Y;`）；公共导入集中 `GlobalUsings.cs`。
- 文件头版权块 ≠ `<auto-generated>`；生成文件（`Generated/*JsonContext.g.cs` 等）**不要手写**，改数据后重跑脚本。
- 命名：类型/方法/属性 PascalCase；接口 `I` + PascalCase；私有字段 `_camelCase`；参数 camelCase；异步方法 `...Async` + `CancellationToken cancellationToken = default`。
- 常量类用 `static class` + `public const string`（如 `WechatTokenTypes`、`WechatErrorCodes`）。
- 错误：参数用 `ArgumentNullException`，状态用 `InvalidOperationException`；统一异常类型 `WechatWorkException`（含 `ThrowIfFailed`）。
- DI：构造注入、字段 `readonly`；多应用基座相关 Singleton 必须经 `IServiceScopeFactory` 建 scope（防 Captive Dependency），退役上下文入退役队列（默认 300s 宽限）后再 `Dispose`。
- 公共 API 必须写 XML 文档（`<summary>`/`<param>`/`<returns>`/`<exception>`；门禁开启 `GenerateDocumentationFile`）。cref 必须可解析（`<paramref name="x"/>` 要求参数存在，否则 `CS1734`；跨包 cref 写全限定或用 `<c>` 兜底）。

## 9 测试规范

- xUnit + FluentAssertions + Moq；测试镜像源结构；类名 `{ClassName}Tests`，方法名 `{Method}_Should{Behavior}_When{Condition}`。
- 已知陷阱（已踩过，勿重复）：
  - **FluentAssertions `Equal` 重载误绑定**：`Equal("a","b","because 文本")` 会绑到 `params object[]` 重载 —— 集合断言须写 `Equal(new[]{ "a","b" }, "because 文本")`。
  - **Moq 的 `out` 参数**：不能直接用局部变量，须 `It.Ref<T>.IsAny` + 自定义 delegate 回调。
  - **DI 解析守卫**：新增 Singleton 服务时补「DI 可解析 + 跨 scope 同一实例」用例（`ValidateScopes = true` 变体）。
  - **可选依赖**：走 `IServiceProvider.GetService<T>()`（返回 null 降级），**不**依赖带默认值的可选构造参数。
  - **Moq 与真实 DI 装配顺序**：先 `AddWechatWorkServices(...)` 完成真实装配，再用 `AddSingleton(mock.Object)` 覆盖外部边界。

## 10 配置面与依赖版本

- 唯一公共配置 API：`WechatAppConfig`（数组节 `WechatApps`）+ `Validate()`；编排策略 `WechatAuthorizationOptions`（节 `WechatAuthorization`）；回调凭据 `WechatCallbackOptions`。
- **禁止新增「日志开关」类配置属性**（历史死配置反模式）；日志级别统一由 `Logging:LogLevel:{Category}` 控制。
- 每个公开配置属性**必须有真实消费点**（`Validate`/`ToString` 不算）。`audit-config-keys.ps1` 的口径是「消费点扫描」，**无白名单** —— 报「无消费点」时的正确处置是**补消费点或删除该属性**，不是加模式绕开（历史同款教训：`WechatAppConfig.TemplateId` 只被 `Validate()` 使用 ⇒ 删除；回调域同款：`WechatAppCallbackOptions` 不得有 `AppKey` 属性，字典键是唯一权威）。
- 消费点口径提示：`ReceiveId` 的消费点是 `receiveid` 校验（**不是**日志开关）；`AppType`/`Channel` 的消费点是 `Validate()` + `ValidateReceiveId` + `IsEventFamilyAllowed`。
- 安全默认不得削弱：`BaseUrl` 必须 HTTPS + 白名单（`AllowCustomBaseUrl=false` 为默认 SSRF 防线）。`AllowCustomBaseUrl=true` 的应用主机在注册期登记到 `WechatCustomBaseUrlRegistry`，供 errcode 判定器的同步预过滤放行（否则私有化部署静默失去令牌恢复能力）。
- **`Mud.HttpUtils` / `Mud.HttpUtils.Generator` 全仓库锁定同一版本**（当前 `3.0.0`），由 G1 守卫「版本集合大小 = 1」（不硬编码版本号，升级无需改守卫）；运行时版本由 `MudHttpUtilsUpgradeSmokeTests` 断言（≥2.0.9 且 Major ≥3）。
- **`Mud.HttpUtils` 系列 3.0.0 已在 nuget.org 上架**（含 `Mud.HttpUtils` / `.Generator` / `.Attributes` / `.Abstractions` / `.Client`，五个包均需存在）。`nuget.config` **只声明 nuget.org、无本机开发源** —— 勿再加回本机源；本地组件迭代用临时源验证后必须清掉。
- **同版本重打包不失效缓存（最易踩的坑）**：NuGet 全局包缓存按 `id + version` 计价，**版本号相同但包内容不同时，下游 restore 不会重新下载**，会用本地构建的旧位编译/测试 ⇒ 假绿。核验位一致性：比对 `<globalPackages>/{id}/{ver}/{id}.{ver}.nupkg.sha512` 与 `https://api.nuget.org/v3-flatcontainer/{id}/{ver}/{id}.{ver}.nupkg` 下载包的 SHA512（base64）。处置：移除该 `{id}/{ver}` 缓存目录 → `dotnet restore --no-cache` → `dotnet clean` → `dotnet build`（`clean` **不可省**，增量构建会把新旧程序集并置，运行时爆 `TypeLoadException`）。

## 11 安全

- 绝不记录或暴露 `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`；日志脱敏。回调域同理：`WechatCallbackEvent` 的 `DecryptedXml` / `SuiteTicket` / `AuthCode` 不得写入日志、遥测或异常消息。
- **Query 承载凭据的脱敏登记**：`corpsecret` / `suite_access_token` / `provider_access_token` 由企业微信契约强制放在 **Query**，而组件 `SensitiveUrlRedactor` 是**精确匹配**词表 —— 故 G7 要求逐参数做「补齐词表 / 登记豁免（附追踪号）」二选一，**新增任何 Query 凭据参数仍必须做该决策**。
- SDK 侧**不得抢占组件 `IExceptionRedactor`**（会丢掉 `Content`/`RequestContent` 的词表擦除 = 削弱安全默认）。
- `WechatWorkException.RequestUri` 在构造期剥离 query 与 userinfo；不得把原始 URI 直接传出。
- 授权链接参数校验：`get_customized_auth_url` 的 `state` ≤ 32 字节且仅 `[a-zA-Z0-9]`（`ValidateCustomizedState`）；安装链接 `state` ≤ 128 字节（`WechatAuthorizationUrlRequest.State`）。**两条规则不同，勿互相套用**。
- 回调入口必须保留抗重放两道闸（时效窗口 + 一次性标记），不得为兼容降级为 fail-open。
- 不要绕过契约守卫与门禁（禁 `--no-verify`、禁删除断言）。

## 12 文档

- 方案与设计文档在 `.docs/`（中文）：`MudWechatWork-授权功能方案-v1.md`、`MudWechatWork-详细设计文档-v1.md`、`MudWechatWork-产品规划方案-v1.md`、`MudWechatWork-回调域功能完善方案-v1.md`、`MudWechatWork-回调域审查缺陷修复与完善方案-v1.md`、`MudWechatWork-多应用管理域审查缺陷修复与完善方案-v1.md`、`MudWechatWork-Redis分布式存储模块方案-v1.md`、`MudWechatWork-AI代码审查提示词-多应用-令牌-回调-v1.md`、`Mud.HttpUtils-企业微信SDK需要的改动.md`。
  **`.docs/` 被 `.gitignore` 忽略**（fresh clone 无此目录），不要把它当作可外部引用的路径。
- **代码变更若触及契约面，须同批同步对应文档章节。**

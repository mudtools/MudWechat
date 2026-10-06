# AGENTS.md

企业微信（WeCom）.NET SDK 的 AI 协作指南。架构对齐 `D:/Repos/MudFeishu/FeishuV3`。

**工作方式**：能力增量 = 在既有域内加端点 + **同批**更新契约守卫。不引入新范式。

**事实来源**（本文只写「不可违反的约束」，明细一律去源头查，本文不复述）：

| 要查什么 | 去哪查 |
|---|---|
| 某域端点 / 路由 / 端点计数 / 官方开放面（自建·代开发·第三方） | `Tests/Mud.Wechat.Work.Tests/ContractGuards/Wechat{域}ContractGuards.cs`（小域为内联单端点断言） |
| 官方文档 URL/ID、频率上限、串行、覆盖删除、权限可见范围 | 接口 XML 文档注释（`Interfaces/{域}/`） |
| 模块枚举值、`Add{域}Api()` | `Extensions/WechatModule.cs`、`Extensions/WechatWorkServiceBuilder.cs` |
| 域 → 目录 / 命名空间实际映射 | `Interfaces/`、`DataModels/` 目录树 |

## 1 门禁（提交前必跑，全绿才算完成）

```bash
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # 构建 + AOT strict + 测试
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1

# 局部快速迭代（CI 用 pwsh）
dotnet build Mud.Wechat.slnx -c Release
dotnet test Tests/Mud.Wechat.Work.Tests -c Release -f net8.0 --filter "FullyQualifiedName~ContractGuards"
```

- `verify-build.ps1` 三步：Release 全量构建 → 逐源项目 AOT strict（排除 `Tests`/`Demos`/`.codeartsdoer`/`*.Generator.csproj`/`*.Analyzers.csproj`——`.codeartsdoer` 为工具临时目录，Roslyn 扩展工程为 netstandard2.0 单 TFM、无运行时 AOT 语义，其正确性由步骤 1 全量构建 + 步骤 3 守卫闭环承担）→ 逐测试工程测试（单 TFM `net8.0`，产物落 gitignored `test-reports/`）。
- **改 `verify-build.ps1` 时三处设置删掉即假绿，不许优化掉**：① strict 步骤须**同时**断言「编译错误」与「IL 诊断」计数（构建本身失败时诊断计数仍为 0）；② `Get-ChildItem -Recurse`（否则找不到嵌套 `.csproj`，AOT 步骤静默空跑）；③ `--no-incremental`（`CoreCompile` 只比对时间戳、不比对 csc 命令行，紧跟步骤 1 的 strict 构建会被整体跳过）。
- **门禁只统计编译错误与 AOT IL 诊断，不因 CS 警告失败** —— 不要为消警告大范围重构。
- `scripts/*.ps1` 为 UTF-8 **含 BOM**（无 BOM 在 PowerShell 5.1 下按 ANSI 解码 ⇒ 语法解析失败）。
- CI（`.github/workflows/dotnet-publish.yml`）的日志桶白名单、恰 5 个 nupkg 断言等只在该文件内维护，改动看文件即可。注意 `AOT006`、`MUD005` 是只打印计数的 INFO 桶（前者漂移守卫、后者企微官方 Query 传令牌契约），**断言为 0 即假红**。

## 2 改动配方（必须同批完成，缺一项即半成品）

| 场景 | 同批完成什么 |
|---|---|
| 加端点 | 接口 XML 文档（官方文档 URL/ID + 频率上限/串行/覆盖删除/权限可见范围等业务限制，格式照既有端点，**不得弱化**）+ 更新对应域守卫 |
| 加 DTO | 标 `[HttpJsonSerializable]` → 跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`（Abstractions 域手写登记进 `AuthenticationJsonContext`）→ 更新守卫中的上下文登记断言 |
| 加配置属性 | 补**真实消费点**；无消费点则**删属性**（`audit-config-keys.ps1` 无白名单、无 `-Strict`，`Validate`/`ToString` 不算消费点，正确处置是补消费点或删属性，不是加模式绕开） |
| 加/改回调事件 | `[WechatCallbackContract]`（事件键 + 族前置 + 开放面）+ 载荷 `[PayloadContract]` + 回调守卫 |
| 加/改回调处理器 | `WechatCallbackHandlerAnalyzer`（MUDCB002~005）自动生效（`SupportedEventType` ↔ 载荷契约一致性编译期校验）；若钥匙集/开放面变化则同批更新 `WechatCallbackContractGuards`（CB 系列） |
| 改契约面（接口/路由/DTO/注册组/配置面） | 对应 `Tests/**/ContractGuards/` 守卫 —— **守卫是权威描述，不是「改完再补」的收尾项** |

## 3 红线：多 TFM、语言与 AOT

`Directory.Build.props`：`TargetFrameworks = netstandard2.0;net6.0;net8.0;net10.0`、`LangVersion = 13.0`、`Nullable = enable`、`ImplicitUsings = enable`、`Version` 为版本唯一来源。

**`netstandard2.0` 无 `IsExternalInit` polyfill**，该 TFM 下：禁 `init`、禁 `record`/`with`；无 `ArgumentNullException.ThrowIfNull`；`string.IsNullOrEmpty`/`IsNullOrWhiteSpace` 无 `[NotNullWhen]`、流分析不收窄（须显式 `x == null`）；禁 `Math.Clamp`。既存大量 CS86xx 警告属正常形态（见 §1）。

**`Tests/Directory.Build.props` 遮蔽根 props**：新增任何治理属性必须同时写入该文件，否则测试工程成为门禁盲区。

AOT / Trim（`net8.0`/`net10.0` 默认开启；`AotStrictMode=true` 把 `IL2026;IL2046;IL2050;IL2057;IL2067;IL2070;IL2072;IL2075;IL2080;IL3050` 升为错误）：

- **禁反射版 `JsonSerializer.Serialize<T>(T, JsonSerializerOptions)` / `Deserialize<T>`** —— 一律走 `JsonTypeInfo`（域 `JsonContext`）或 `WechatJsonResolverExtensions` 合并解析器。
- `Generated/*JsonContext.g.cs` 是**生成物**：提交进版本控制、**勿手改**，改 DTO 后重跑脚本。
- DTO 标 `[HttpJsonSerializable]`，其 `SerializerClassName` = 命名空间域段（根命名空间直属文件归 `Common`）；上下文 `internal`，经 `InternalsVisibleTo` 供主包与测试直读。
- 配置绑定为源生成（`EnableConfigurationBindingGenerator=true`）：配置 DTO **禁 `required`**（生成器以 `new T()` 构造 ⇒ `CS9035`），校验写进 `Validate()`；绑定必须走 `Configure<T>(o => section.Bind(o))`，**不要**用 `Configure<T>(IConfiguration)` 重载（反射绑定无法被源生成器拦截，破坏 `IL2026`/`IL3050` 净零）。
- `UnconditionalSuppressMessageAttribute` 在 `net10.0` 为 `internal`，用户代码不可引用；必要时 `#pragma warning disable` 并附理由注释。
- 已知边界：开放泛型 `WechatChatbotResponse<>` 不登记（`SYSLIB1030`）；工具运行期 `AOT003` 为已知误报。

## 4 结构与落位

```
Mud.Wechat.Work/                      # 主包：Interfaces/{域}/ 接口声明 + 服务 + DI + 模块注册
Mud.Wechat.Work.Abstractions/         # 令牌基座、多应用、配置、存储端口、枚举、异常、回调信封与载荷转换器
Mud.Wechat.Work.DataModels/           # 官方 DTO（[HttpJsonSerializable]）+ Generated/ 域 JsonContext（生成物）
Mud.Wechat.Work.Callback/             # 回调接收（AES 解密、事件解析、分发）+ HTTP 中间件；Events/Payloads/ 载荷
Mud.Wechat.Work.Callback.Generator/   # 回调契约登记生成器（IsPackable=false；发射 RegisterAll）
Mud.Wechat.Work.Callback.Analyzers/   # 回调处理器契约分析器（诊断型、不发射；IsPackable=false，随 Callback nupkg 内嵌 analyzers/dotnet/cs）
Mud.Wechat.Redis/                     # 四个存储端口的 Redis 实现 + 连接基座 + DI 编排
Tests/                                # 5 个工程（Work / Abstractions / Callback / DataModels / Redis），镜像源结构，单 TFM net8.0
scripts/                              # verify-build / audit-config-keys / GenerateJsonContext / AddHttpJsonSerializable / ApplyTokenOwnerKeys
.docs/                                # 方案与设计文档（中文；已 gitignore，fresh clone 无此目录）
```

依赖单向：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`、`Redis → Abstractions`。硬边界：**`Callback` 不引用 `Work`**；**`Redis` 不引用 `Work`/`Callback`**。

**新文件落位三处一致（目录 / 命名空间 / 注册入口）**：

- 接口命名空间恒为 `Mud.Wechat.Work`（**不含** `Interfaces` 段）；DTO 命名空间 `Mud.Wechat.Work.DataModels.{域}[.{子域}]`。
- `RequestModel/`、`ResponseModel/` 与各功能族子目录**仅作组织，不入命名空间**（如 `Checkin/Schedule/` 仍是 `...DataModels.Checkin`）。
- 模块注册三段式：`WechatModule.{域}` 枚举值 + `Add{域}Api()`（在 `Extensions/WechatWorkServiceBuilder.cs`）+ 源生成器产出的 `Add{域}WebApiHttpClient()`（**无签入源文件**）；注册组名与域同名。

目录 / 命名空间 / 模块名**不一致的既存点**（新增沿用，勿「顺手修正」）：

| 域 | 接口目录 | DTO 目录 | 模块 / 注册入口 |
|---|---|---|---|
| 通讯录 | `Interfaces/Contacts/`（**无子目录**，Export 服务也在根） | `Contacts/{Users,Batch,ContactRules,Department,Export,Tags}/` | `Contact` / `AddContactApi()`（单数） |
| 微信客服 | `Interfaces/KF/`（大写 F） | `Kf/`（小写 f） | `Kf` / `AddKfApi()` |
| 令牌签发 | 无接口目录 | `{CorpToken,InternalApp,Provider}Authentication/` | 无对应模块枚举 |
| 通讯录 Users | — | `Contacts/Users/` → 命名空间 `...DataModels.Contracts.Users`（全仓唯一插入 `Contracts` 段） | — |

## 5 领域契约（不可违反）

| 应用类型 | `WechatAppType` | 令牌链 |
|---|---|---|
| 自建应用 | `Internal` | `access_token`（`corpid` + `corpsecret`） |
| 第三方（Suite） | `ThirdParty` | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发 | `Provider` | 同上；企业令牌走 `gettoken(corpsecret = permanent_code)` |

### 5.1 令牌与凭据

- **应用类型子接口必须声明「凭据归属域」**：`[Token(TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken | CorpAccessToken)]`（键值 `Wechat.AccessToken@Internal` / `@Corp`）；公共父接口**不声明**。`TokenType` 恒为 `Wechat.AccessToken`，**不得按应用类型拆分**（会改变注入参数名）。归属域在唯一咽喉点 `WechatAppContext.GetTokenManager` 校验，错配抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）；注册表键集单一来源 = `WechatTokenRouting.OwnedKeys`。批量落地用 `scripts/ApplyTokenOwnerKeys.ps1`，一致性由守卫 `WechatTokenOwnerContractGuards`（TO1~TO3）与 `WechatTokenOwnerEndToEndTests` 锁定。
- **令牌注入统一走 Query**（企微契约，非 Header）。白名单由守卫 G5 锁定，**新增须先评估、再显式扩展 G5**；例外 `get_customized_auth_url` 以显式 Query 参数传令牌且**不带 `[Token]`**。
- **分流在 `CorpTokenManager` 内按 `AppType`**：代开发 `permanent_code` 是「应用 secret」→ `gettoken`；第三方是「授权码」→ `get_corp_token`。**不新建管理器。**
- **模板 id**：代开发 `template_id` 即 `suite_id` ⇒ `WechatAppConfig` **不提供独立 `TemplateId`**（非法状态不可表达）。接口级 `templateid_list` 由宿主显式传入。
- `TokenKey` 三段式 `{tokenType}:{appKey}:{scopeKey}`；`WechatTokenTypes` 一律 `"Wechat."` 前缀。
- **`AppKey` 形状受约束**（`WechatAppKeyValidator` 经 `Validate()` 单点收敛）：`[A-Za-z0-9]` 开头 + 仅 `[A-Za-z0-9._-]` + ≤128。含 `:` 会造成键别名 ⇒ 跨应用令牌串号。
- **企业级令牌一企一份**（`scopeKey = authCorpId`），**不经声明式 `[Token]`**：显式 `GetTokenAsync(new[]{ authCorpId })`。errcode 恢复**必须显式传 scope**，否则对已缓存企业令牌是空转。
- **企业作用域一律用 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`**（第三方/代开发推荐入口）：一次性 `using`，进入时解析应用 + 快照并写入「应用 + 企业」两级上下文（归属应用取解析后 `AppKey`），释放时逆序还原（企业 → 应用）且幂等；企业参数校验失败回滚已进入的应用作用域。裸写入原语 `SetCorp`（**必填** `authCorpId`，空白即抛，与 `appKey=null`「未声明归属」区分）已标 `[Obsolete]` 引导；`ClearCorp` 为**无条件清空**，嵌套场景会误伤外层企业上下文，**勿与作用域还原混用**。读上下文须过两级校验（appKey 归属一致 + authCorpId 与 scope 一致）；`InvalidateTokenAsync` 的 `scopes` 非空但全为空白串 ⇒ 编程错误 fail-fast（`[]`/null 保持「全部」）。

### 5.2 存储端口与多实例

- `IWechatCorpAuthStore` 复合键 `(AppKey, AuthCorpId)`；`IWechatSuiteTicketStore` 按 `suiteId` 分槽（多套件互不覆盖）。
- 默认实现仅进程内 ⇒ 多实例部署引用 `Mud.Wechat.Redis`（`AddWechatRedis` **必须先于** `AddWechatApp`/`AddWechatCallback`，颠倒即注册期 fail-fast），或由宿主前置 `TryAdd` 分布式实现覆盖（宿主预注册者按契约胜出）。
- 四个存储端口（含 `IWechatCallbackReplayGuard`：接口落 `Abstractions.TokenManager`，InMemory 实现留 `Callback` 包）由 Redis 包单依赖 Abstractions 实现、共享连接基座。
- 退役清库批量能力为**可选**接口 `IWechatTokenStoreBatchRemove`（管理器先 `is` 探测，命中批删、未实现回退逐键）。
- **SE.Redis 3.3.0 陷阱**：① `RedisTimeoutException` 继承 `TimeoutException` 而非 `RedisException`，捕获须走 `WechatRedisErrors.ShouldWrap`，裸 `catch (RedisException)` 会漏超时；② `StringSetAsync` 有四套重载，**生产调用一律用命名参数钉住形态**（`keepTtl: false` / `when: ...`）。

### 5.3 多应用基座与 DI 装配

- **DI 桥接不变量**：`IAppContextHolder` / `IAppContextSwitcher` / `IWechatAppContextSwitcher` **必须是同一实例**，且 SDK 注册**必须先于** `AddMudHttpClient`（组件内部会 `TryAdd IAppContextHolder`）。破坏 ⇒ 声明式 `[Token]` 客户端上下文恒 `null`，多套件静默回退默认应用令牌。
- **`WechatAppManager` 直接实现 `IAppManager<IWechatAppContext>`**，**不继承**组件 `DefaultAppManager<T>`（基类另有影子注册表与非 virtual 写入口，会让注册静默写进读不到的表）。注册表单一来源 = `_configs` + `_lazyContexts`；`RegisterApp`/`UpdateApp`/`RegisterSwitcherFactory` 显式 `NotSupportedException`。
- 配置读取走 `ConfiguredConfigs`/`TryGetConfig`（非物化）；`TryGetApp` 会构造 HttpClient/DI scope/Timer，**回调的 `SuiteId → appKey` 匹配不得用它**。
- 细则（均已被测试锁定，改动前先读对应测试）：`RemoveApp` 顺序恒为 `TryRemove` 先于配置删除、全部移除后 `DefaultAppKey` 置 `null` 且 `GetDefaultApp` 抛明确错误；重建白名单仅瞬时 IO 型异常组，`InvalidOperationException` 直抛；退役队列 fire-and-forget + 停机限时（默认 5s）排空；`CreateAppContext` 经 `WechatOwnedResourceTracker` 登记 scope 外产物，装配中途失败逆序回收后原样重抛。

### 5.4 授权编排与回调自动化

- `IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举）：换码**单飞门 + 结果记忆**以 `(appKey, authCode)` 复合键为粒度；共享任务用 `CancellationToken.None` 承载、各调用者经 `AwaitSharedAsync` 独立取消；飞行条目仅创建者移除。`RevokeAuthorizationAsync` 顺序**先失效令牌、后删库**且仅失效本 `appKey` 令牌。策略统一落 `WechatAuthorizationOptions`（节 `WechatAuthorization`），**不得**塞进 `WechatAppConfig`。
- `IWechatAuthorizationCoordinator`：Abstractions 定义、**主包实现**，`Callback` 经 `IServiceProvider.GetService<>()` 惰性可选解析（缺主包授权模块时首次 Warning 后降级不抛）。`change_auth` 本地无记录仅告警、**不发 `get_auth_info`**；`cancel_auth` 清理范围**恒为 `SuiteId` 命中集**，未命中只告警不删库（授权记录每套件独立，回退「全部应用清理」会删掉其它套件的有效授权）。三个逐 appKey 循环**单应用失败记 Error 后继续**；`OperationCanceledException` **不在捕获面**。

### 5.5 回调接收面

| 面 | 约束 |
|---|---|
| 包形态 | `netstandard2.0` 用 `Microsoft.AspNetCore.Http 2.3.9`，`net6+` 用 `FrameworkReference`；中间件为经典约定式 `RequestDelegate`（不进 DI），宿主 `UseWechatWebhook()` 接入 |
| 凭据来源 | `WechatCallbackOptions.Apps` 字典是**唯一来源**（`WechatAppConfig` 无 Push 属性、应用级配置无 `AppKey` 属性）；路由 `/{GlobalRoutePrefix}/{AppKey}` |
| 注册 | `AddWechatCallback` 返回建造者（链式注册处理器/拦截器）；`IConfiguration` 重载为**惰性** `Configure(o => section.Bind(o))`；`IOptionsMonitor` 支持路由前缀与凭据**请求期热更** —— 勿改回注册期急切绑定。**不得引入「接收方 ID 统一注册表 / `IWechatCallbackUrlVerifier`」** |
| 校验 | 请求期单应用（`ResolveApp` 命中后 `app.Validate(appKey)`，单应用配置错误不拖垮全部路由）+ `WechatCallbackOptions.Validate()` 启动期全量入口；失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 映射类别） |
| 加解密 | 按官方 **32 字节块 PKCS7** 手工补位/剥离；**禁用 .NET 内置 16 块 `PaddingMode.PKCS7`**（会误拒官方 pad∈[17..32] 报文） |
| 抗重放 | 两道 fail-closed 闸：① 时间戳窗口 ±300s（缺失/非数字即拒）② 一次性 SHA1 指纹（**不得落盘密文本身**）。**指纹闸在「解密 + receiveid 校验成功」之后、事件返回之前**（解密失败不消耗指纹，官方重试可重入）。**GET echo 只过时效闸、不消费指纹**（同 echostr 二次保存配置必须成功）。分布式守卫异常**必须上抛**（→ 5xx → 官方 96238 重试），**禁止**吞异常 `true` 放行或 `false` 静默丢事件；空键返回 `false` 且不触达存储 |
| 事件信封 | `WechatCallbackEvent` / `IWechatCallbackEventHandler` / `IWechatCallbackEventInterceptor` / `WechatCallbackEventTypes` 落 `Abstractions.Callback`（该目录含子目录、不得出现 XML 类型）；XML→信封解析留 `Callback.WechatCallbackReceiver`。`EventTypeKey`：客户联系/获客族与邮箱族以**外层事件值**为键（`Event` 优先、套件信封回退 `InfoType`，两信封同键；邮箱族因 `receive_email` 在应用邮箱/公共邮箱两族同名），其余 = `InfoType` → `ChangeType` → `Event`；`SupportedEventType` 空串 = 兜底（内置处理器即此形态）。`AuthCorpId ← FromUserName` 兜底**仅限授权族**。信封的 `AppKey`/`AppType`/`Channel` 是只读快照，**配置权威仍是** `WechatAppManager`/`WechatCallbackOptions` |
| 分发 | 组合根期急切注册、**无 Freeze**；通配键 `"*"` 双语义（通讯录同步助手路由 + 全局处理器/拦截器桶）；匹配序 appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底。同步分发 + 软超时（默认 4500ms，**必须 < 5s 契约**）；超时/中断 → 503 触发重推；**指纹在分发前消费** ⇒ 重推同指纹被 403（fail-closed 优先于 at-least-once，**处理器须幂等**）；单处理器异常隔离（LogError 后继续）；`MaxConcurrentEvents` 为构造期快照（热更不改容量） |
| 配置面 | `WechatAppCallbackOptions`：`PushToken`/`PushEncodingAESKey`/`ReceiveId`/`AppType`/`Channel`，必须与主配置同类文件才纳入 audit 扫描。`ReceiveId` 是接收方 ID（自建填 `CorpId`、**套件填 `SuiteId`**）；非空时校验解密明文 `receiveid`，留空或明文未携带时跳过并一次性告警 |
| AppType × Channel | `Channel`（`App=1` 应用数据通道 / `Suite=2` 套件指令通道，默认 `App`）；`Validate()` 拒绝「自建应用占用套件通道」「套件通道非第三方/代开发」。`ValidateReceiveId` 三元分流，`IsEventFamilyAllowed` 为**族级默认**合法性闸、**先于**拦截器 `BeforeHandleAsync`，不适用族返回 `Rejected`（→200 不重推） |

**事件载荷体系**：**不得**再新增「逐事件 DTO + 手写 `ParseXxx`」，**不得**手写多级嵌套解析或手改 `OfficialPayloadContracts.RegisterAll` 方法体。

- 载荷按官方**报文结构族**建于 `Events/Payloads/`（子目录仅作组织，命名空间恒为 `Mud.Wechat.Work.Callback.Events.Payloads`），字段映射由上游 `Mud.HttpUtils.PayloadFieldMapGenerator` 依 `[PayloadContract]`/`[PayloadField]` **编译期生成** ⇒ 元素名↔属性名配对受编译器校验。
- 转换器落 `Abstractions/Callback/Payloads/`（纯转换语义、零 Callback 依赖）；标量方法须 `static`、非泛型、恰 1 参。官方「根下重复同名兄弟元素」列表形态（wedoc `DocId`/`FormId`/`FieldId`/`RecordId`）由投影器**根层同名叶兄弟合并**（`XElementPayloadSource`，仅根层、仅全叶组，容器 Value 取末位以保 `Values` 袋 last-wins）+ 转换器 `RepeatSiblings`（`Method` 通道）承载；根下「容器+叶项」包装形态仍走 `Items`/`ItemsObject`，不得合并。嵌套走声明化通道：单对象 `Object<TSingle>`（内层 DTO 标 `[PayloadContract]`、属性可空、禁 `ItemName`）/ 对象列表 `ItemsObject<TItem>`（须 `ItemName`、元素实参非可空）。
- 映射表须在**具体类型**处取 `XxxPayload.PayloadFieldMap`（泛型上下文取类型参数静态成员报 `CS0712`；`static abstract` 需 net7+）。类型化处理器继承**抽象基类** `WechatCallbackPayloadHandler<TPayload>`（「泛型接口 + 显式默认实现」在 ns2.0 报 `CS8701`）。
- **契约登记单一来源**：`[WechatCallbackContract]`（`AllowMultiple`，同键多声明由生成器合并去重；`requiredEvent` 缺省 = 逐键自指），生成器发射 `RegisterAll`；声明不完整由 `MUDCB001` 打红。
- **开放面粒度是事件键**（ADR-15）：`OpenSurfaces` 为多组「模式集合 × 通道」组合对，叠加在族级默认之上，两闸均先于拦截器。宿主注册新 `Event` 值会落 `Unknown` 族被族闸放行 ⇒ **必须**显式声明，不得依赖族默认。
- **三模式无关性**（ADR-14）：三种 `AppType` 报文结构同一，差异只是「值是否出现」⇒ 一份可空超集载荷覆盖三模式；**载荷与转换器层禁止出现 `WechatAppType`/`WechatCallbackChannel` 分支**，需分支时在处理器层读 `evt.AppType`。载荷**不复用** DataModels 的 JSON DTO。
- `Callback.csproj` 必须显式引用 `Mud.HttpUtils.Generator`（Abstractions 侧带 `PrivateAssets="all"`）与本仓生成器工程（`OutputItemType="Analyzer"`、`IsPackable=false`，不进「恰 5 nupkg」）。上游生成器引用与 `Mud.HttpUtils` 包版本须全仓单一（守卫 G1；`NU1605` 视为错误）。
- 族事件键不可用逐 `ChangeType` 消歧（官方裸值跨族同名）：客户联系/获客族以 `change_external_contact`/`change_external_chat`/`change_external_tag`/`customer_acquisition`/`customer_acquisition_permit_change`、邮箱族以 `app_email_change`/`public_email_change` 为键，类别由信封 `ChangeType` 判别。`JobType` 级差异属语义过滤、**不得做成安全闸**。
- **回调处理器接线（编译期）**：`WechatCallbackHandlerAnalyzer`（`Mud.Wechat.Work.Callback.Analyzers`，随 Callback nupkg 的 `analyzers/dotnet/cs` 下发）以 `TPayload` 的 `[WechatCallbackContract]` 为唯一权威，校验 `SupportedEventType` ↔ 载荷契约一致性：**MUDCB002**（Error，键 ∉ 载荷键集，消灭运行期 `ContractMismatch` 静默丢事件）、**MUDCB003**（Info，键非常量）、**MUDCB004**（Warning，键写字符串字面量而非 `WechatCallbackEventTypes` 常量）、**MUDCB005**（Warning，处理器未 `AddHandler<T>` 注册）。豁免：空键 / `null`（兜底）/ `GenericCallbackPayload` / 无契约特性；`.editorconfig` 可调 `severity`；**MUDCB005 为启发式规则、非安全闸**（跨程序集注册编排不覆盖）。分析器**只诊断、不发射**，运行期行为零变化；**全部注册入口必须异常兜底**（分析器抛异常 ⇒ `AD0001` ⇒ 宿主整次编译失败）。
- **分析器识别类型只靠硬编码 metadata name**（红线：不得引用被分析程序集）⇒ Abstractions 改命名空间/改名会让规则**静默空跑**（`GetTypeByMetadataName` 返回 `null`，0 诊断无报错）。守卫 `CB24` 的**反射断言**是唯一能发现该漂移的形态，改动那4 个 `*MetadataName` 常量时必须同批核对。

### 5.6 消息推送落位

- **不做运行时多态**（AOT 源生成按声明类型序列化）：每个 msgtype 一个端点方法 + 请求 DTO（同路由多方法），官方参数表差异（`safe` 有无、id 转译支持面、`mentioned_list` 仅群聊）在 DTO 层面精确表达；模板卡片 `TemplateCardBody` 为发送/更新两端点共用的扁平结构。
- **响应形态陷阱**：`message/send` 的 `invaliduser` 是**竖线分隔字符串**、`update_template_card` 的是**字符串数组** ⇒ 两类响应 DTO 不共用。
- **「接收消息与事件」不属于本域**：消息接收是回调推送（XML），由 `Callback` 包承载；本域全部是发送侧 HTTP API。

## 6 契约守卫（`Tests/**/ContractGuards/`）

守卫是契约的**权威描述**，改动契约面必须**同批**更新（见 §2）。各域端点计数、路由表、字段名断言、官方反直觉点（多数统计/查询接口官方即 POST、字段名照抄官方原文拼写等）**以守卫源文件与接口 XML 注释为准**，本文不复述。

- 通用守卫 `WechatContractGuards.cs`（G1~G9）：`Mud.HttpUtils*` 全仓单一版本（防混版 `TypeLoadException`）；配置 DTO 禁 `required`；`WechatTokenTypes` 前缀；失效码集合与判定器同源；**G5 = Query 令牌白名单**（未放宽且子接口仅覆盖官方实际开放的应用类型）；授权端点路由与官方一致；Query 凭据参数名 ⊆ 脱敏词表 ∪ 显式豁免（**豁免自过期**，须附追踪号）；DI 桥接三接口同实例；`cancel_auth` 不得「未命中回退全部应用」。
- 归属域守卫 `WechatTokenOwnerContractGuards.cs`（TO1~TO3）：按程序集反射枚举全部应用类型子接口（数量下限防枚举空跑），锁定族别 ↔ `TokenManagerKey` 归属域一一对应、`TokenType` → 官方 Query 参数名不漂移、声明的键被 `WechatTokenRouting.OwnedKeys` 覆盖且自建/非自建不交叉。切换器契约由 `Abstractions.Tests/WechatAppContextSwitcherTests` 锁定（`UseCorpScope` 必在契约上且返回 `IDisposable`；`SetCorp` 必带 `[Obsolete]` 指向它；实现类**不得**标 `[Obsolete]`——废弃标注只放抽象层）。
- 多应用守卫（`Abstractions.Tests`，MA1~MA4）是**方法体文本断言**（花括号配平），签名漂移须同步更新。
- 域守卫通用形态：令牌绑定（`Wechat.AccessToken` + Query 注入）、新增 DTO 的上下文登记、**官方未开放 ⇒ 零端点**、继承链子接口集合不漂移、父接口 `IsAbstract` + 子接口经 `InheritedFrom` 父实现类。

## 7 编码风格与测试

所有源文件必须以版权块开头（生成文件用 `<auto-generated>`）：

```csharp
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
```

- 文件域命名空间（`namespace X.Y;`）；公共导入集中 `GlobalUsings.cs`；生成文件勿手写。
- 命名：类型/方法/属性 PascalCase；接口 `I` + PascalCase；私有字段 `_camelCase`；异步方法 `...Async` + `CancellationToken cancellationToken = default`。常量类用 `static class` + `public const string`。
- 错误：参数用 `ArgumentNullException`，状态用 `InvalidOperationException`；统一异常类型 `WechatWorkException`（含 `ThrowIfFailed`）。
- DI：构造注入、字段 `readonly`；多应用基座相关 Singleton 必须经 `IServiceScopeFactory` 建 scope（防 Captive Dependency），退役上下文入退役队列（默认 300s 宽限）后再 `Dispose`。
- 公共 API 必须写 XML 文档；cref 必须可解析（跨包 cref 写全限定或用 `<c>` 兜底）。
- 测试：xUnit + FluentAssertions + Moq，镜像源结构；类名 `{ClassName}Tests`，方法名 `{Method}_Should{Behavior}_When{Condition}`。已踩陷阱：FluentAssertions 集合断言须写 `Equal(new[]{...}, "because…")`（否则误绑 `params object[]`）；Moq 的 `out` 参数用 `It.Ref<T>.IsAny` + 自定义 delegate；新增 Singleton 必补「DI 可解析 + 跨 scope 同一实例」用例（`ValidateScopes = true`）；可选依赖走 `GetService<T>()` 降级；Moq 覆盖顺序是**先**真实装配 `AddWechatWorkServices(...)`、**再** `AddSingleton(mock.Object)`。

## 8 配置与安全

- 配置 API 仅三处：`WechatAppConfig`（节 `WechatApps`）、`WechatAuthorizationOptions`（节 `WechatAuthorization`）、`WechatCallbackOptions`。**禁止新增「日志开关」类配置属性**；日志级别统一由 `Logging:LogLevel:{Category}` 控制。
- **每个公开配置属性必须有真实消费点**（`Validate`/`ToString` 不算）。
- **安全默认不得削弱**：`BaseUrl` 必须 HTTPS + 白名单（`AllowCustomBaseUrl=false` 是 SSRF 防线）；登记到 `WechatCustomBaseUrlRegistry` 的自定义主机才能被 errcode 判定器预过滤放行（否则私有化部署静默失去令牌恢复能力）。
- 绝不记录或暴露 `AgentSecret`/`SuiteSecret`/`ProviderSecret`/`permanent_code`/`auth_code`/`suite_ticket`；`WechatCallbackEvent` 的 `DecryptedXml`/`SuiteTicket`/`AuthCode` 不得进日志、遥测或异常消息。`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo。
- `corpsecret`/`suite_access_token`/`provider_access_token` 被官方强制放 Query，而 `SensitiveUrlRedactor` 是精确匹配词表 ⇒ **新增任何 Query 凭据参数必须做「补齐词表 / 登记豁免（附追踪号）」二选一**。SDK 侧**不得抢占组件 `IExceptionRedactor`**（会削弱词表擦除）。
- 两条 state 规则**不同，勿互相套用**：`get_customized_auth_url` 的 `state` ≤32 字节且仅 `[a-zA-Z0-9]`；安装链接 `state` ≤128 字节。
- 回调抗重放两道闸**不得为兼容降级为 fail-open**；**不得绕过门禁**（禁 `--no-verify`、禁删断言）。
- 依赖版本：`Mud.HttpUtils` / `.Generator` 全仓单一版本（守卫 G1 断言版本集合大小 = 1，不硬编码版本号）；`nuget.config` 只声明 nuget.org，勿加回本机开发源。
- **同版本重打包不失效缓存（最易假绿）**：NuGet 全局包缓存按 `id + version` 计价，版本同内容不同 ⇒ 下游沿用旧位。处置：删 `<globalPackages>/{id}/{ver}` → `dotnet restore --no-cache` → `dotnet clean`（**不可省**，否则新旧程序集并置、运行时 `TypeLoadException`）→ `dotnet build`。

## 9 提交前自检

1. 契约面改动是否**同批**更新了对应守卫？新增端点是否补齐官方文档链接与业务警示？
2. 新增 DTO 是否跑了 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`（Abstractions 域手写登记）？
3. 新增公开配置属性是否有真实消费点（否则删除）？
4. 新文件是否落在 §4 的目录 / 命名空间 / 注册入口三处一致？
5. `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1` 是否全绿？
6. 契约面变更是否同批同步了 `.docs/` 对应章节？

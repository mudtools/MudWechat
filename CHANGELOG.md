# Changelog

本文件记录 MudWechat 对外可见的重要变更。

- 格式遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。
- **口径来源**：端点 / 接口 / 事件键等计数取自 `Tests/ContractBaseline.cs`（跨域汇总基线）与各线 `Tests/**/ContractGuards/`，
  而非 README 自述。新增能力必须同批更新守卫，故守卫即权威描述。

## 1.0.0 - 2026-10-18

首个公开发布版本：**七条微信产品线 + AI 能力域 + Demo 体系，共用一个底座**。
32 个 NuGet 包（`Src/` 下 34 个源工程，其中 2 个为构建期工具、`IsPackable=false`）、19 个测试工程、273 个源生成 JSON 上下文。

> **版本口径说明**：`Directory.Build.props` 在内部演进期依次用过 1.0.3 / 1.1.0 / 1.2.0 / 1.3.0 / 1.3.1，
> 但 nuget.org 上从未出现过任何 `Mud.Wechat.*` 包（无 tag、无推送记录）⇒ 对外首发把这些积累**归一为 1.0.0**。
> 本条即「全部历史能力」的总账，后续版本只做增量。

### Added

#### 产品线与业务面

- **企业微信（`Mud.Wechat.Work*`，4 包）**：35 个业务域、461 支 `[HttpClientApi]` 契约接口（154 公共父接口 + 307 可注入接口），
  自建应用 / 第三方套件 / 服务商代开发三模式同仓共面；入口 `AddWechatApp` + `AddWechatWorkServices(b => b.AddContactApi().AddExternalContactApi())`。
- **微信公众号（`Mud.Wechat.OfficialAccount*`，4 包）**：27 个业务域、266 支去重路由（259 主接口 + 3 授权族 + 4 二进制下载通道），
  含网页授权、`snsapi_*` 用户令牌链、JS-SDK 票据、模板消息、素材与非税账单下载；入口 `AddMpApp` + `AddMpServices` + `UseMpCallback()`。
- **微信小程序（`Mud.Wechat.MiniProgram*`，3 包，无 Callback）**：30 个业务域、398 支端点，
  凭据复用公众号令牌域（同一 `/cgi-bin/token`，**不新增令牌类型**）；入口 `AddMiniProgramServices`。
- **微信支付 APIv3（`Mud.Wechat.Pay*`，4 包）**：91 支接口、596 支端点，覆盖 JSAPI/Native/付款码下单、查询、撤销、退款、
  商户进件与风控、品牌营销、分账、支付分、服务商族与回调；凭据为商户 RSA 私钥签名（`WECHATPAY2-SHA256-RSA2048`），**无 `access_token`**。
- **微信开放平台（`Mud.Wechat.OpenPlatform*`，3 包）**：29 支接口、189 支端点（含 2026-10 并入的流量主代运营 AMS 域），
  `component_access_token` + 每授权方令牌走显式提供者；入口 `AddOpenPlatform` + `AddOpenPlatformApis`。
- **微信小店 / 视频号（`Mud.Wechat.Channels*`，4 包）**：29 个业务域、394 支端点，独立令牌类型 `Wechat.Channels.AccessToken`
  （`token` + `stable_token` 双通道、独立 AppID），覆盖交易管理与内容运营两侧；入口 `AddChannelsApp` + `AddWechatChannelsApi`。
- **腾讯广告 Marketing API v3.0（`Mud.Wechat.Ads*`，3 包，预览）**：13 个业务域、48 支端点（45 声明式 + 3 支 multipart 手写通道），
  OAuth2 双令牌模型，`access_token`/`timestamp`/`nonce` 每请求现取、注入在传输层而非声明式 `[Token]`；入口 `AddAdsApp` + `AddWechatAdsApi`。

#### 底座：令牌、多应用与授权

- 令牌基座：三段式键 `{tokenType}:{appKey}:{scopeKey}`、Query 注入（企微契约）、提前刷新、errcode 失效恢复，全生命周期托管。
- 多应用基座 `WechatAppManager`（直接实现 `IAppManager<IWechatAppContext>`，注册表单一来源）与 DI 桥接同实例不变量
  （`IAppContextHolder` / `IAppContextSwitcher` / `IWechatAppContextSwitcher` 必须同一实例）。
- 多租户作用域 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`：一次性 `using`、
  进入时写「应用 + 企业」两级上下文、释放时逆序幂等还原。
- 凭据归属域 fail-fast：错配在唯一咽喉点 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`，
  杜绝「静默取到另一个企业的令牌」；键集单一来源 `WechatTokenRouting.OwnedKeys`。
- 企业级令牌一企一份（`scopeKey = authCorpId`），显式 scope 与环境上下文两条路径收敛到同一缓存/持久层键。
- 授权编排 `IWechatWorkAuthorizationService`（换码单飞门 + 结果记忆、`appKey` 解析「显式即有意、隐式即失败」、
  撤销时先失效令牌后删库）与回调驱动的 `IWechatAuthorizationCoordinator`。
- 配置基座：源生成绑定（禁 `required`，校验写进 `Validate()`）、按 `AppType` 的互斥必填校验在 **DI 注册阶段**即抛错。
- SSRF 防线：`BaseUrl` 强制 HTTPS + 主机白名单（`WechatApiHosts` 单一来源），私有化部署须显式登记 `WechatCustomBaseUrlRegistry`。
- 全线 DTO 标 `[HttpJsonSerializable]`，序列化走源生成 `JsonContext`（273 个），零运行期反射。

#### 回调接收面

- 四个回调宿主包（企业微信 / 公众号 / 支付 / 小店）+ 智能机器人 JSON 通道；中间件按经典 `RequestDelegate` 形态提供，宿主 `UseWechatWebhook()` 接入。
- 官方 **32 字节块 PKCS7** 手工补位/剥离（不使用 .NET 内置 16 块填充——那会误拒官方 `pad∈[17..32]` 的报文）。
- 两道 **fail-closed** 抗重放闸：时间戳 ±300s 窗口 + 一次性 SHA1 指纹（指纹在「解密 + `receiveid` 校验成功」之后、分发之前消费）。
- 事件键体系：企业微信 122 个事件键 / 47 个报文结构族载荷，小店 42 个事件键；键与载荷由 `[WechatCallbackContract]` + `[PayloadContract]`
  声明，字段映射编译期生成，重复同名兄弟元素由投影器全树同名兄弟合并。
- 分发面：类型化处理器 + 拦截器 + 通配键 `"*"`、匹配序「专属精确 → 全局精确 → 专属兜底 → 全局兜底」、软超时 4500ms 回 503 触发官方重推。
- `WechatCallbackHandlerAnalyzer`（`MUDCB002~005`）：`SupportedEventType` ↔ 载荷契约一致性在编译期校验，键写裸字面量给 Warning。
- 智能机器人两条通道：JSON 回调（返回式处理器 `Task<AibotMessage?>`、应答外壳 `{encrypt,msgsignature,timestamp,nonce}`、软超时回加密空包 + 200）
  与长连接（`wss://openws.work.weixin.qq.com`，`aibot_subscribe`、流式状态机、素材三步上传、Redis 租约主备），二者均**仅 net8.0+**。

#### AI 能力域（4 个工程）

- `Mud.Wechat.AI.Abstractions` / `Mud.Wechat.AI.Agent` / `Mud.Wechat.Work.AI` / `Mud.Wechat.Work.AI.Tools`，双档 `net8.0;net10.0`。
- 33 支企业微信工具（成员、通讯录标签、客户联系、微信客服、打卡、审批、汇报、会议、直播、微文档、应用消息），
  工具 Schema 由声明式接口在编译期派生、golden 基线 `WechatToolSchemas.golden.txt` 钉死形状。
- 高危写工具的授权与白名单管控：`WechatToolPolicy` + `WechatToolProfile`（33 槽）+ `IWechatToolApprovalGate` / `WechatToolApprovalDecision`，
  由 `WechatToolDispatcher` 在派发咽喉点执行；注册入口 `AddWechatWorkTools(binding)`。
- 上游引擎 `Mud.AI.* 1.0.2`（钉版，禁浮动）：`AddAgentRuntimeCore` 之上的四个中立扩展（会话门面 / 回合闸门 / 工具重试 /
  会话事实）按需接入，能力采纳口径见 `.docs/MudWechatWork-AI-Mud.AI-1.1.0-升级与能力采纳方案-v1.md`。
- **Agent 运行期遥测**：`WechatAgentTelemetry` 实现中立 `IAgentTelemetry` ⇒ span `wechat.ai.agent.turn` / `run` / `summarize`
  （租户与会话键只作 Span 属性）、直方图 `wechat.ai.agent.llm.duration`（维度 `agent`）、计数器 `wechat.ai.agent.degraded`
  （维度 `reason`）；Meter/Source 名恒 `Mud.Wechat`，随既有通配订阅自动被采。
- **工具面遥测收编**：`WechatToolTelemetry` 实现 `IAgentToolTelemetry`，执行链只经端口计数、端口内部转发到既有
  `wechat.ai.tool.*` 命名面（命名面零变化）；新增 `wechat.ai.tool.retries` / `approval_requests` / `approval_wait`。
- **同会话互斥 + 进程内削峰**：`WechatTurnGate`（`KeyedAsyncGate` + `ConcurrencyLimiter`，**先键后配额**）接入回合管线；
  配置 `WechatAi:MaxConcurrentTurns`（默认 32）与 `WechatAi:TurnQueueTimeoutSeconds`（默认 30，排队超时显式失败）。
- **HITL 机制侧（挂起 → 批准 → 续跑）**：`IWechatToolApprovalNotifier`（宿主投递端口）+ `WechatToolApprovalChannel`
  （`IToolApprovalChannel` 实现；未注册投递端口即退化为「只记日志 + 快照可查」，写工具仍 fail-closed）+
  `WechatToolApprovalResume`（待确认项查询 + 批准回灌续跑，含环境量恢复与 `ApprovalResolved` 埋点）。
- **回合事实（Facts）打通企业维度**：`WechatAgent` 经 `BuildScopeFacts` 产出 `wechat.app_key` / `wechat.auth_corp_id` /
  `wechat.open_id`；作用域工厂优先读 `TurnScopeContext.Facts`，缺席时逐字段回退环境量（软缺席）。
- **AI 域门禁**：`WechatAmbientScopeGuards`（AI 面禁止裸 `AsyncLocal`）、`WechatTelemetryContractGuards`（实现存在 /
  执行链不双记 / 指标无高基数维度）、`WechatAiContractGuards` 的「上游版本与采纳面同批」；
  `WechatWorkAiDiContractGuards` 增遥测未退化、HITL 与闸门装配、闸门旋钮投影三条。

#### 分布式存储与可观测性

- `Mud.Wechat.Redis`：令牌、企业授权、套件票据、回调抗重放、机器人长连接租约五个存储端口的 Redis 实现 + 连接基座
  （`AddWechatRedis` **必须先于** `AddWechatApp`/`AddWechatCallback`，顺序颠倒即注册期 fail-fast）。
- 后台刷新登记与优雅停机（`Dispose` 取消在途刷新，不产生未处理异常 Error 日志）。
- `Mud.Wechat.OpenTelemetry`：Tracing + Metrics 默认开、单根 `ActivitySource` 名恒 `Mud.Wechat`、产品线 Meter 通配、
  OTLP 端点与采样率可配（`AddWechatOpenTelemetry`）。

#### Demo 体系（13 个工程，全在解决方案 `/Demos/` 组内）

- L0 基座 `Mud.Wechat.Demo.Shared`：`DemoEnv.RequireAll`（一次列全缺失项、把 `<...>` / `your-...` 占位符判为未配置）、
  `DemoWriteGate`（写端点默认**不映射** ⇒ 404）、`MapDemoHealth`（只回显非敏感配置摘要）、`UseWechatApiErrorGate`
  （SDK 异常 → 502 + 错误码，且拆开组件负缓存窗口的包装异常，不回堆栈）。
- L0 `Mud.Wechat.Demo.SelfTest`：`--channel work|mp|component|channels|bot` 五通道回调离线自测，
  断言面覆盖 GET echo、POST 事件、重放 403、篡改签名 403，并锁定各通道「拒绝形态」的差异（403 空体 / `success`/`fail` / `echostr` 原文）。
- L1 六个宿主（企业微信三件套 + 公众号 + 小程序 + 支付）、L2 四个（开放平台 + 小店 + AI Agent + 广告）、
  L3 一个 AllInOne（多线同宿主 + 路由共存证明，零凭据即可起进程、缺凭据束整线跳过）。
- `Demos/README.md` 提供索引、逐工程凭据矩阵与安全须知；每个 Demo 的 README 含前置条件、环境变量、运行、验证与实测状态码表。

#### 质量门禁

- `scripts/verify-build.ps1` 三步：Release 全量构建 → 逐源工程 AOT strict（10 类反射诊断升为错误，保持净零）→ 逐测试工程测试。
- `scripts/audit-config-keys.ps1`：每个公开配置属性必须有真实消费点，无白名单、无 `-Strict` 绕开模式，无消费点的正确处置是删属性。
- 契约守卫族：企业微信 G1~G12 / TO1~TO3 / N1~N3 / CB 系列 / MA1~MA6，公众号 MP 与 MD 系列，小程序 MP-X 系列，支付 PAY-B 系列，
  小店 CH / CH-T / CH-R 系列，广告 ADS-S / ADS-B 系列，跨线 AB-G6（解决方案工程清单、配置审计搜索根、包推导口径三处一致）。
- 包清单单一来源：可打包集由 `Src/**/*.csproj` 现场推导，CI / `pack.bat` / `publish.bat` 各自推导、不持有名单。

### Changed

- 上游依赖统一：`Mud.HttpUtils` 与 `Mud.HttpUtils.Generator` 全仓单一版本 **3.0.8**（守卫 G1 断言版本集合大小 = 1，`NU1605` 视为错误）；
  `Microsoft.Extensions.*` 按档对齐（net8/10 → 10.0.12，net6/netstandard2.0 → 8.0.x）。
- SE.Redis 按 TFM 分档锚定（net8+ 3.4.5、低档 2.13.17），并锁定 `StringSetAsync` 一律用命名参数钉住重载形态。
- 令牌值编解码迁移到官方 `MigrationTokenStoreCodec`（桥接层双读旧值，旧数据无缝读、写入走新格式）。
- v2 令牌信封读取面无条件兼容（零代码改动即得）；`WechatRedisOptions` 镜像官方两阶段开关
  `UseVersionedEnvelope` / `AcknowledgeVersionedEnvelopeReadersUpgraded`，**默认关闭**且 `Validate()` 两阶段确认 fail-fast。
- Redis 接入通用 `ITokenStore` 面并合并免令牌拆分接口；跨进程刷新租约经 `AddWechatRedis` 注册即启用。
- 回调守卫存储故障的映射从「与其它验证失败同流」改为**专用 503**（故障期回 403 等于静默丢事件），与官方 96238 重试语义对齐。
- 测试工程继承根 `Directory.Build.props` 治理（`LangVersion` / `Nullable` / `ImplicitUsings` / `Version` / TFM 集一套来源），
  仅额外把 `TargetFrameworks` 收为单档 `net8.0` 并显式关闭 trim/AOT 分析器（xunit/Moq 反射会造约 900 条 IL 噪声）。
- 语言与框架面：全仓 `LangVersion 13.0`、`Nullable enable`；根属性给 `netstandard2.0;net6.0;net8.0;net10.0` 四档
  （支付与开放平台**两条线**为三档、**无 netstandard2.0**，因 `AesGcm` 在 ns2.0 不存在；AI 四工程为 `net8.0;net10.0` 双档）。
- AI 域依赖升版：`Mud.Wechat.AI.Abstractions` / `Mud.Wechat.AI.Agent` / `Mud.Wechat.Work.AI.Tools`（含 `Mud.AI.Generator` 分析器）/
  `Mud.Wechat.Work.AI.Mcp` 统一钉 `Mud.AI.* 1.0.2`（传递依赖与 1.0.1 相同，`Directory.Build.props` 传递要求段无需改动）。
- 产品环境量改走中立 `AmbientScope<T>`：`IWechatAiCorpContextAccessor` 新增 `Begin(principal)` 作用域入口，
  释放为**还原**而非置空（嵌套回合不再抹掉外层身份）；事件层在排入后台回合前 `using var _ = Begin(...)`，后台回合不再手工复位。

### Deprecated

- `IWechatAppContextSwitcher` 上四个无作用域语义的旧成员已标 `[Obsolete]`，将在下一个大版本移除：
  `UseApp(appKey)` 与 `UseDefaultApp()` → 改用 `IAppScopeSwitcher.UseAppScope` / `UseDefaultAppScope`；
  `BeginScope(appKey)` → `UseAppScope`（逐行等价别名）；裸写入 `SetCorp(...)` → `UseCorpScope(appKey, authCorpId, permanentCode)`。
- 智能机器人「长连接与回调凭据同 BotKey 并存」不再受支持：启动期 fail-fast，须二选一。

### Removed

- 私有 `WechatTokenBridgeCodec` 退役（由上游官方 `MigrationTokenStoreCodec` 取代，本仓不再持有令牌值编解码实现）。
- `WechatAppConfig` **不提供独立 `TemplateId` 属性**：代开发场景 `template_id` 即 `suite_id`，同时提供两者会造成非法状态；
  接口级 `templateid_list` 由宿主显式传入（与「广告线 `account_id` 不进配置面」同一思路：非法状态不可表达）。
- 不引入「接收方 ID 统一注册表 / `IWechatCallbackUrlVerifier`」这类旁路面：回调凭据来源恒为 `WechatCallbackOptions.Apps` 字典。
- 上游 `Mud.HttpUtils` 3.0.8 随 **BC-30** 物理移除 `IAppManager<T>` 的三个离线接缝成员（`GetWebApi` / `GetDefaultWebApi` /
  `RegisterSwitcherFactory`，替代路径 `AppKeySwitchExtensions.SwitchToApp` / `SwitchToDefaultApp`）：本仓四条线（企微 / 公众号 /
  小店 / 开放平台）原「实现体恒抛 `NotSupportedException`」的悬空实现一并删除，`OpenPlatformAppManager` 的私有异常工厂
  `NotSupportedSwitcher()` 连带删除；作用域切换恒走 DI 解析的 `I*AppContextSwitcher.UseAppScope` / `UseDefaultAppScope`
  （「无孤儿公开接缝」由源文本守卫 G12 锁定）。

### Fixed

- 六条**手工二进制通道**（小程序码、反馈图片、客服临时素材、物流对账、公众号素材下载、非税账单）在
  「只配默认应用、从不进入作用域」这一最常见宿主形态下抛「找不到应用上下文」：现按 `环境上下文 ?? 默认应用` 解析，
  与生成客户端口径一致（守卫 MP-X10 / MD11，行为回归 `MpMediaDownloadServiceTests.D9b`）。
- 公众号 `AddMpApp` + `AddMpJsApiSignature` 在 `ValidateOnBuild=true` 下建容器即失败：
  `IMpJsApiTicketManager` 只装在 `MpAppContext` 内、未命名 `IOptions<MpAppConfig>` 从未登记，现以默认应用为桥补齐（回归 A17）。
- 微信支付退款域此前一笔都调不通（官方在**验签之前**要求 `Accept` 与 `User-Agent` 两个头都在）：
  命名客户端补默认头，常量落 `WechatPayHttpClientNames`（红线 PAY-B12，正/反向两条断言）。
- 组件在令牌负缓存窗口内会把 `WechatApiException` 包进 `InvalidOperationException`，导致异常穿透、开发者页把 SDK 堆栈写进响应体：
  Demo 错误闸按「内层必须是 `WechatApiException`」拆开映射 502，普通 `InvalidOperationException` 仍原样上抛。
- 平台服务商保险合约域误用电商主域注册，改用专属保险合约扩展注册组。
- 回调四线补 `Seal` 封闭守卫与分派收尾归因：处理器异常/超时/中断的分工不再靠运行期巧合。
- 授权编排的 `appKey` 解析：空白 `appKey` 曾静默落到默认应用，现改为「显式配置的 `DefaultAppKey` → 仅一个应用时该应用 →
  ≥2 应用且未配置则 fail-fast」，消除换码/撤销的误路由。
- **同会话并发回合丢历史/串台**：AI 回合此前 `acquireGate: null` ⇒ 同一会话的两条消息并发跑两个回合，两边都读到「上一轮的历史」，
  后写覆盖先写；现由 `WechatTurnGate` 同键串行。
- **AI 回合过载无上限**：新增在途回合上限与排队超时（超时显式失败），不再无限排队至内存耗尽。

### Security

- 回调两道抗重放闸保持 **fail-closed**，不得为兼容性降级放行；分布式守卫异常必须上抛（→ 5xx → 官方重试），禁止吞异常。
- 凭据脱敏：官方强制放 Query 的凭据参数一律「进 `SensitiveUrlRedactor` 词表」或「登记带追踪号的自过期豁免」二选一；
  异常基类在**构造期**剥离 `RequestUri` 的 query 与 userinfo。
- `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`、回调 `DecryptedXml`
  与支付回调 `resource` 明文一律不进日志、遥测或异常消息。
- 支付私钥与 APIv3 密钥只以「**名称**」进配置，运行期经 `ISecretProvider` 取用；启动期拒绝把 PEM 直接贴进配置文件。
- AI 高危写工具默认不可达：须经白名单与 `IWechatToolApprovalGate` 放行，派发咽喉点单点执行。
- Demo 写操作默认关闭（端点根本不映射 ⇒ 404），真实凭据只走环境变量或本机 User Secrets，仓库内只允许变量名与占位符。

### 已知限制

- **腾讯广告线为预览**：官方无推送面 ⇒ 无回调包；错误信封 `{code, message, message_cn, data}` 与微信 `errcode` 语义**不得跨线套用**；
  经守卫抛出的 `WechatAdsException.MessageCn` 恒为 `null`（`ThrowIfFailed` 只消费 `code`/`message`，属已文档化契约）。
- 智能机器人 JSON 通道与长连接、以及全部源生成 JSON 上下文的能力，**仅 net8.0+ 可用**（低档回 415 + 告警，不静默降级）。
- `netstandard2.0` 无 `IsExternalInit` polyfill：该档禁 `init` / `record` / `with`，流分析不收窄（须显式 `x == null`）。
- 令牌刷新负缓存窗口内的抑制行为由上游组件承担（≥ 3.0.5），本仓只做异常形态的分流与脱敏。
- 真账号才能闭合的验证面：真实推送（需公网回调 URL）、OAuth/JS-SDK 官方签名校验、支付 1 分钱闭环（需平台证书）、
  AI 真实会话（需机器人 + 长连接租约）、广告授权链（需开发者应用 + 已登记回调域，官方 `redirect_uri` 不支持端口号）。
- 根 `README.md` 的部分统计（源工程数、测试工程数、各线端点数、JSON 上下文数）滞后于守卫基线，以本文件与 `Tests/ContractBaseline.cs` 为准。

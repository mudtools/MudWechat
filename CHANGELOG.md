# 更新日志

Mud.Wechat 的对外可见变更全部记录在本文件。

**编写约定**

- 格式遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，版本号遵循[语义化版本](https://semver.org/lang/zh-CN/)。
- 未发布的改动写在 `[Unreleased]` 下；发布时整段移入新版本小节并补发布日期，不留空壳小节。
- 小节顺序固定为 **新增 → 变更 → 弃用 → 移除 → 修复 → 安全**；没有内容的小节直接省略，不写「无」。
- 每个版本只记录**增量**；完整能力面见根 `README.md` 与各包目录下的 `README.md`。
- 「已知限制」记录能力边界与验证前提，不是变更条目，随版本小节末尾一并给出。
- 接口 / 路由 / 端点 / 事件键的数量为发布时的实测口径；与 `README.md` 不一致时以本文件为准。

## [1.0.0] - 2026-10-29

首个公开版本：**七条微信生态产品线 + AI 能力域 + Demo 体系，共用一个底座**。

四个贯穿全仓的设计选择：**声明式接口契约 → 源生成客户端 → DI 注入**；**源生成 JSON → Native AOT 净零**；
**多应用 / 多租户作用域 → 凭据不串号**；**官方路由与端点计数在编译期锁定 → 不会在接入之后才发现路由已改名**。

**制品面**

- **18 个 NuGet 包**：七条产品线各 1 个入口包 + 4 个 `.Callback`（有推送面的四条线）+
  3 个共用基建（`Mud.Wechat.Abstractions` / `Redis` / `OpenTelemetry`）+ 4 个 AI 包。
- **一条产品线一个入口包**：基座 + 官方 DTO + 声明式客户端 + DI 与模块注册同处一个程序集；只有 `Callback`
  （把 ASP.NET Core 依赖挡在宿主侧）与 `Redis` / `OpenTelemetry`（可选基建）独立成包。
  安装决策只有一句：**按平台装 1 个主包；要收推送再加 `.Callback`；多实例加 `Mud.Wechat.Redis`；要看链路加 `Mud.Wechat.OpenTelemetry`**。
- **目标框架分档**：`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`；支付线与开放平台线为三档（无
  `netstandard2.0`，`AesGcm` 在该档不存在），AI 域为 `net8.0;net10.0` 双档。
- **Native AOT**：`net8.0` / `net10.0` 下可裁剪发布，全部 DTO 走源生成 JSON，AOT 分析告警净零。
- **示例**：13 个 Demo 工程，按 L0 基座 / L1 单线宿主 / L2 进阶线 / L3 多线合一分层。

### 新增

#### 产品线

| 产品线            | NuGet 包                                       | 规模                                                                             | 凭据模型                                                                                                                             |
| ----------------- | ---------------------------------------------- | -------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| 企业微信（WeCom） | `Mud.Wechat.Work` / `Mud.Wechat.Work.Callback` | 35 个业务域、470 支契约接口（158 公共父接口 + 312 可注入接口）、122 个回调事件键 | `access_token` / `provider_access_token` / `suite_access_token`，Query 注入                                                          |
| 微信公众号        | `Mud.Wechat.OfficialAccount` / `.Callback`     | 27 个业务域、266 支去重路由（259 主接口路由 + 3 授权族 + 4 二进制下载通道）      | `Wechat.Mp.AccessToken`（普通 + 稳定双通道）+ `jsapi` / `wx_card` 票据                                                               |
| 微信小程序        | `Mud.Wechat.MiniProgram`                       | 588 支端点（含 6 支手工二进制通道）                                              | 复用公众号令牌域（同一 `/cgi-bin/token`，不新增令牌类型）                                                                            |
| 微信支付          | `Mud.Wechat.Pay` / `.Callback`                 | APIv3：91 支接口 / 625 支端点；APIv2：25 支接口 / 71 支端点                      | APIv3 商户 RSA 私钥签名（`WECHATPAY2-SHA256-RSA2048`），**无 `access_token`**；APIv2 走 MD5 / HMAC-SHA256 签名密钥与商户证书双向通道 |
| 微信开放平台      | `Mud.Wechat.OpenPlatform`                      | 34 支接口 / 208 支端点                                                           | `component_access_token` + 每授权方令牌（显式提供者）                                                                                |
| 微信小店 / 视频号 | `Mud.Wechat.Channels` / `.Callback`            | 29 个业务域、394 支端点、42 个回调事件键                                         | 独立令牌类型 `Wechat.Channels.AccessToken`（`token` + `stable_token` 双通道，独立 AppID）                                            |
| 腾讯广告（预览）  | `Mud.Wechat.Ads`                               | 13 个业务域、48 支端点（45 支声明式 + 3 支 multipart 手写通道）                  | OAuth2 `access_token` + `refresh_token`，每请求现取，注入在传输层                                                                    |

- **企业微信**：自建应用 / 第三方套件 / 服务商代开发三种模式同仓同面；入口 `AddWechatApp` +
  `AddWechatWorkServices(b => b.AddContactApi().AddExternalContactApi())`。
- **微信公众号**：网页授权与 `snsapi_*` 用户令牌链、JS-SDK 票据、模板消息、素材与非税账单下载；
  入口 `AddMpApp` + `AddMpServices` + `UseMpCallback()`。
- **微信小程序**：登录、码与链接、安全、数据分析、订阅消息、客服、硬件、物流、直播、虚拟支付、云开发、
  交易组件与联盟等域；入口 `AddMiniProgramServices`。
- **微信支付**：JSAPI / Native / 付款码下单与查询撤销、退款、商户进件与风控、品牌营销、分账、支付分、
  服务商族与回调；APIv2 与 APIv3 同包发布。
- **微信开放平台**：第三方平台基座、代小程序管理与代码模板库、批量代云开发、流量主代运营、半屏小程序、微信认证；
  入口 `AddOpenPlatform` + `AddOpenPlatformApis`。
- **微信小店 / 视频号**：小店交易管理面（商品 / 订单 / 售后 / 物流 / 资金 / 营销 / 联盟 / 供货）与视频号内容运营面
  （直播 / 留资 / 橱窗 / 罗盘 / 带货助手）；入口 `AddChannelsApp` + `AddWechatChannelsApi`。
- **腾讯广告**：营销单元 / 广告主 / 素材 / 报表 / 资金 / 人群包 / 定向标签等域，`access_token` / `timestamp` / `nonce`
  每请求现取、注入在传输层而非声明式 `[Token]`；入口 `AddAdsApp` + `AddWechatAdsApi`。

#### 底座：令牌、多应用与多租户

- **令牌基座**：三段式键 `{tokenType}:{appKey}:{scopeKey}`、Query 注入（企微契约）、提前刷新、errcode 失效恢复，
  全生命周期托管。
- **多应用**：`WechatAppManager` 作注册表单一来源，与 DI 桥接共享同一实例；`IWechatAppContextSwitcher`
  与 `IAppContextSwitcher` / `IAppContextHolder` 解析到的必须是同一对象。
- **多租户作用域**：`UseCorpScope(appKey, authCorpId, permanentCode)` 一次性 `using`，进入时写「应用 + 企业」
  两级上下文、释放时逆序幂等还原；企业级令牌一企一份（`scopeKey = authCorpId`）。作用域切换恒走
  `IAppScopeSwitcher.UseAppScope` / `UseDefaultAppScope` / `UseCorpScope`；`IWechatAppContextSwitcher` 上四个
  无作用域语义的成员（`UseApp` / `UseDefaultApp` / `BeginScope` / `SetCorp`）已标注 `[Obsolete]`，
  将在下一个大版本移除。
- **凭据归属 fail-fast**：应用类型与凭据归属域不匹配时，在令牌解析入口即抛
  `WechatTokenOwnerMismatchException`，不会静默取到另一个企业的令牌。
- **授权编排**：`IWechatWorkAuthorizationService` 处理换码（并发只打一次官方接口、结果记忆）与撤销
  （先失效令牌后删库）；`IWechatAuthorizationCoordinator` 承接回调驱动的授权变更。
- **配置**：源生成绑定；`AppType` 的互斥必填校验在 **DI 注册阶段**即抛错；错误配置不会拖到首次调用才暴露。
- **SSRF 防线**：`BaseUrl` 强制 HTTPS + 主机白名单；私有化部署须显式登记 `WechatCustomBaseUrlRegistry`。
- **序列化**：官方 DTO 标 `[HttpJsonSerializable]`，全部走源生成 `JsonContext`，零运行期反射。
- **存量兼容**：令牌值的旧编码格式可继续读取（写入即新格式）；版本化令牌信封的写入开关默认关闭，
  须两阶段确认后才启用，未确认即开启会在启动时失败。
- **依赖基线**：`Mud.HttpUtils` 3.0.9、`Mud.AI.*` 1.0.2（钉版）；`Microsoft.Extensions.*` 与 SE.Redis 按目标框架
  分档（`net8.0` / `net10.0` → 10.0.12 与 3.4.5，低档 → 8.0.x 与 2.13.17）。

#### 回调接收面

- 四个回调宿主包（企业微信 / 公众号 / 支付 / 小店）+ 智能机器人 JSON 通道；中间件按经典 `RequestDelegate`
  形态提供，宿主 `UseWechatWebhook()` 接入。
- 官方 **32 字节块 PKCS7** 补位 / 剥离（不使用 .NET 内置 16 块填充——那会误拒官方 `pad ∈ [17..32]` 的报文）。
- 两道 **fail-closed** 抗重放闸：时间戳 ±300s 窗口 + 一次性指纹，任一不过即拒绝，不因兼容性放行。
- 事件键体系：企业微信 122 个事件键 / 47 个报文结构族载荷，小店 42 个事件键；键与载荷由
  `[WechatCallbackContract]` + `[PayloadContract]` 声明，字段映射编译期生成。
- 分发面：类型化处理器 + 拦截器 + 通配键 `"*"`，匹配序「专属精确 → 全局精确 → 专属兜底 → 全局兜底」；
  软超时 4500ms 回 503，触发官方重推。
- 编译期校验：`SupportedEventType` 与载荷契约不一致时直接编译失败（分析器 `MUDCB002~005`）。
- 智能机器人两条通道：JSON 回调（返回式处理器 `Task<AibotMessage?>`、应答外壳
  `{encrypt, msgsignature, timestamp, nonce}`、软超时回加密空包 + 200）与长连接
  （`wss://openws.work.weixin.qq.com`、`aibot_subscribe`、流式状态机、素材三步上传、Redis 租约主备），
  二者均仅 `net8.0+`。

#### AI 能力域（4 个包）

- `Mud.Wechat.AI`（中立层）+ `Mud.Wechat.Work.AI` / `.Tools` / `.Mcp`，双档 `net8.0;net10.0`。
- 33 支企业微信工具（成员、通讯录标签、客户联系、微信客服、打卡、审批、汇报、会议、直播、微文档、应用消息），
  工具 Schema 由声明式接口在编译期派生，与 HTTP 客户端共用同一套契约。
- 高危写工具的授权与白名单管控：`WechatToolPolicy` + `WechatToolProfile`（33 个工具槽）+
  `IWechatToolApprovalGate` / `WechatToolApprovalDecision`，由 `WechatToolDispatcher` 在派发点统一执行；
  注册入口 `AddWechatWorkTools(binding)`。
- 上游引擎 `Mud.AI.* 1.0.2`（钉版，禁浮动）：`AddAgentRuntimeCore` 之上的四个中立扩展（会话门面 / 回合闸门 /
  工具重试 / 会话事实）按需接入。
- **遥测**：`WechatAgentTelemetry` 实现中立 `IAgentTelemetry` ⇒ span `wechat.ai.agent.turn` / `run` / `summarize`
  （租户与会话键只作 Span 属性）、直方图 `wechat.ai.agent.llm.duration`（维度 `agent`）、计数器
  `wechat.ai.agent.degraded`（维度 `reason`）；Meter / Source 名恒 `Mud.Wechat`，随通配订阅自动被采。
  工具面经 `WechatToolTelemetry` 汇总到 `wechat.ai.tool.*`，另有 `retries` / `approval_requests` / `approval_wait`。
- **同会话互斥 + 削峰**：`WechatTurnGate`（先按会话键串行、再按并发配额排队）接入回合管线；
  配置 `WechatAi:MaxConcurrentTurns`（默认 32）与 `WechatAi:TurnQueueTimeoutSeconds`（默认 30，排队超时显式失败）。
- **人工审批（挂起 → 批准 → 续跑）**：`IWechatToolApprovalNotifier`（宿主投递端口）+ `WechatToolApprovalChannel`
  （未注册投递端口时退化为「只记日志 + 快照可查」，写工具仍 fail-closed）+ `WechatToolApprovalResume`
  （待确认项查询 + 批准回灌续跑，含环境量恢复与 `ApprovalResolved` 埋点）。
- **回合事实**：`WechatAgent` 经 `BuildScopeFacts` 产出 `wechat.app_key` / `wechat.auth_corp_id` / `wechat.open_id`；
  作用域工厂优先读 `TurnScopeContext.Facts`，缺席时逐字段回退环境量。

#### 分布式存储与可观测性

- `Mud.Wechat.Redis`：令牌、企业授权、套件票据、回调抗重放、机器人长连接租约五类存储的 Redis 实现 + 连接基座；
  `AddWechatRedis` **必须先于** `AddWechatApp` / `AddWechatCallback` 注册，顺序颠倒即注册期 fail-fast。
- 跨进程令牌刷新租约随 `AddWechatRedis` 即启用；后台刷新与优雅停机（`Dispose` 取消在途刷新，
  不产生未处理异常日志）。
- `Mud.Wechat.OpenTelemetry`：Tracing + Metrics 默认开启、单根 `ActivitySource` 名恒 `Mud.Wechat`、
  产品线 Meter 通配、OTLP 端点与采样率可配（`AddWechatOpenTelemetry`）。

#### Demo 与示例体系

- L0 基座 `Mud.Wechat.Demo.Shared`：`DemoEnv.RequireAll`（一次列全缺失项、把 `<...>` / `your-...` 占位符判为未配置）、
  `DemoWriteGate`（写端点默认不映射 ⇒ 404）、`MapDemoHealth`（只回显非敏感配置摘要）、`UseWechatApiErrorGate`
  （SDK 异常 → 502 + 错误码，不回堆栈）。
- L0 `Mud.Wechat.Demo.SelfTest`：`--channel work|mp|component|channels|bot` 五通道回调离线自测，
  覆盖 GET echo、POST 事件、重放 403、篡改签名 403，并锁定各通道「拒绝形态」的差异
  （403 空体 / `success` / `fail` / `echostr` 原文）。
- L1 六个宿主（企业微信三件套 + 公众号 + 小程序 + 支付）、L2 四个（开放平台 + 小店 + AI Agent + 广告）、
  L3 一个 AllInOne（多线同宿主 + 路由共存，零凭据即可起进程、缺凭据束整线跳过）。
- `Demos/README.md` 提供索引、逐工程凭据矩阵与安全须知；每个 Demo 的 README 含前置条件、环境变量、运行、
  验证与状态码表。

### 安全

- 回调两道抗重放闸保持 **fail-closed**；存储侧校验故障时返回 5xx 让官方重试，不返回 4xx 静默丢事件。
- 凭据脱敏：官方要求放在 Query 的凭据参数一律脱敏；异常消息在构造期剥离 `RequestUri` 的 query 与 userinfo。
- `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`、回调
  `DecryptedXml` 与支付回调 `resource` 明文一律不进日志、遥测或异常消息。
- 支付私钥与 APIv3 密钥只以「**名称**」进配置，运行期经 `ISecretProvider` 取用；启动期拒绝把 PEM 直接贴进配置文件。
- AI 高危写工具默认不可达：须经白名单与 `IWechatToolApprovalGate` 放行。
- Demo 写操作默认关闭（端点根本不映射 ⇒ 404），真实凭据只走环境变量或本机 User Secrets，
  仓库内只允许变量名与占位符。

### 已知限制

- **腾讯广告线为预览**：官方无推送面 ⇒ 无回调包；错误信封 `{code, message, message_cn, data}` 与微信 `errcode`
  语义**不得跨线套用**；`WechatAdsException.MessageCn` 恒为 `null`（只消费 `code` / `message`，属已文档化契约）。
- 智能机器人 JSON 通道与长连接、以及依赖源生成 JSON 上下文的通道**仅 `net8.0+` 可用**（低档返回 415 + 告警，
  不静默降级）。
- 智能机器人的「长连接」与「回调凭据」不可同 BotKey 并存，启动期 fail-fast，须二选一。
- `netstandard2.0` 无 `IsExternalInit`：该档禁 `init` / `record` / `with`，流分析不因
  `string.IsNullOrEmpty` 收窄（须显式 `x == null`），且无 `ArgumentNullException.ThrowIfNull` / `Math.Clamp`。
- 令牌刷新负缓存窗口内的抑制行为由上游 `Mud.HttpUtils` 承担，本仓只做异常形态的分流与脱敏。
- **有意不提供的能力**：`WechatAppConfig` 没有独立 `TemplateId` 属性（代开发 `template_id` 即 `suite_id`，
  同时提供两者会造成非法状态，接口级 `templateid_list` 由宿主显式传入）；回调凭据来源恒为
  `WechatCallbackOptions.Apps` 字典，不提供「接收方 ID 统一注册表 / 回调 URL 校验器」这类旁路。
- **仅真实账号能闭合的验证面**：真实推送（需公网回调 URL）、公众号 OAuth / JS-SDK 官方签名校验、
  支付 1 分钱闭环（需平台证书）、AI 真实会话（需机器人 + 长连接租约）、广告授权链
  （需开发者应用 + 已登记回调域，官方 `redirect_uri` 不支持端口号）。

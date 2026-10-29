# Mud.Wechat.AI.AgentDemo（DM-11 智能机器人 + AI Agent + 工具面）

一条工程跑通 AI 原生链路：**机器人事件接入 → Agent 回合 → 工具调用 → 主动回复**，
并且**两种接入形态都能装配**（JSON 回调地址 / wss 长连接，官方 101463 强制二选一）。
全链路可在**零真实凭据、零公网、零模型密钥**下离线验收：模型用工程内的 `EchoChatClient` 替身，
回调密码学用 SDK 自身内核（`Demos/Mud.Wechat.Demo.SelfTest --channel bot`，21 项断言）。

与其它 Demo 的根本差别：这一条**不是「HTTP 端点的演示」**。它的主战场是两个后台形态 ——
分发器（`WechatBotEventDispatcher`）与长连接运行器（`WechatBotLongConnectionRunner`）——
所以 `/healthz` 之外只有三个只读观察端点，其余事实都落在**日志**与**台账**里。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 令牌与多应用底座 | `AddWechatApp(Action<WechatAppConfig>)`（`AppType = Internal`；机器人**复用自建应用令牌** `access_token = corpid + corpsecret`） |
| 模块装配 | `AddWechatWorkServices(b => b.AddAibotApi().AddContactApi())` |
| 主动回复端点 | `AddAibotApi()` 提供 `IWechatWorkInternalAibotService`（SDK 内置 `WechatBotAgentHandler` 的构造依赖，缺它即激活期 fail-fast） |
| 回调装配 | `AddWechatCallback(Action<WechatCallbackOptions>)` → `AddWechatBotCallback().AddHandler<T>(botKey)`（建造者链式；未先 `AddWechatCallback` 即 fail-fast） |
| 回调中间件 | `UseWechatWebhook()`（路由 `/{GlobalRoutePrefix}/{AppKey}`，Demo 不写回调路由） |
| 通道消歧 | `WechatCallbackChannel.Bot` + `WechatAppCallbackOptions`（`PushToken` / `PushEncodingAESKey`，**`ReceiveId` 必须留空** —— 官方 101033） |
| 处理器契约 | `IWechatBotCallbackEventHandler`（**返回式** `Task<AibotMessage?>`，`null` = 加密空包 `{}`；`SupportedEventType` 空串 = 兜底） |
| 事件键 | `WechatBotEventTypes.Text` / `.EnterChat` / `.FeedbackEvent` / `.DisconnectedEvent` …（**独立键集**，不与 XML 侧 `WechatCallbackEventTypes` 合并） |
| 应答构造 | `WechatBotReplies.Text(...)`（官方仅 `enter_chat` 允许被动回 `text`） |
| 事件信封 | `WechatBotCallbackEvent`（`AppKey` / `MsgId` / `ChatType` / `DecryptedJson` 只读快照） |
| 长连接装配 | `AddWechatBotLongConnection(IConfiguration)`（配置节 `WechatBots`，`WechatBotOptions` / `WechatBotAppOptions`） |
| 长连接租约端口 | `IWechatBotConnectionLease`（`TryAcquireAsync` / `RenewAsync` / `ReleaseAsync`；SDK 侧唯一实现来自可选包 `Mud.Wechat.Redis`） |
| Agent 装配 | `AddWechatWorkAgent(Action<WechatAgentOptions>)` + `WechatAgent`（`AgentName = "wechat-agent"`） |
| 回合入口 | `WechatAgent.RunTurnAsync(WechatConversationRequestFactory.Create(WechatConversationFacts), IConversationReplySink, ct)` → `TurnOutcome`（`ResponseText` / `Streamed` / `Replayed`） |
| 工具身份上下文 | `IWechatAiCorpContextAccessor` + `WechatToolPrincipal(appKey, authCorpId)`（由「进入回合的那一层」建立并在 `finally` 复位） |
| 工具面装配 | `AddWechatWorkTools(new WechatToolBinding(AppKey))` → `IReadOnlyList<AIFunction>`（模型可见投影） |
| 工具两道闸 | `WechatToolPolicy`（白名单 `EnabledTools` + `EnforceAuthorization`，装配期冻结）/ `IWechatToolAuthorizer`（未注册即 fail-closed 全拒） |
| 工具注册表 | `WechatToolRegistry.ToolNames`（已落地执行器，与「白名单」「可见面」是三个不同口径） |
| 替身模型 | `Microsoft.Extensions.AI.IChatClient`（工程内 `EchoChatClient`；接真实模型只换注册一行） |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非敏感摘要（`channel`、机器人与应用键、回调路由、`modelVisibleTools`、`sessionTtlSeconds`、`maxToolCallsPerTurn`、处理器名单、写闸状态） |
| GET | `/demo/tools` | 工具三段口径：`enabledTools`（白名单）/ `registryTools`（注册表）/ `modelVisible`（模型可见 + 完整 JSON Schema） |
| GET | `/demo/turn?question=&sessionId=` | 离线回合：不经企微、不打回调，直接把问题灌进 Agent 管线（含「部门」时替身模型真的发起一次 `contacts.department.get`） |
| GET | `/api/bot-events?take=` | 机器人事件台账（事件键计数 + 最近列表），只含非敏感字段 |
| GET | `/wechat/{BotKey}` | **URL 验证**（中间件承载，仅 callback 模式）：四参 `msg_signature` 验签 + 时效 → 回显**解密后的明文** |
| POST | `/wechat/{BotKey}` | **JSON 安全模式事件推送**（中间件承载，仅 callback 模式）：验签 → 时效 → 解密 → `receiveid` → 指纹 → 分发 → 加密应答 |
| GET | `/wechat` | 仅前缀 ⇒ 通配键 `"*"`（Demo 里指向 `Channel = App` 的条目）：把 JSON 打到这里会被以 `UnknownReceiver` **403** 拒收 |
| —— | `wss://openws.work.weixin.qq.com` | long 模式**无 HTTP 路由**：由 `WechatBotLongConnectionRunner` 出站建连并 `aibot_subscribe` |

本工程**不映射任何写端点**：主动回复由 SDK 处理器按 `response_url` 下发，Demo 侧没有「创建 / 发送」类路由，
故 `MUDWECHAT_DEMO_ALLOW_WRITE` 在此只影响启动横幅，不改变路由集。

## 2 前置条件

1. **已创建智能机器人**（企微后台「应用管理 → 智能机器人」），并确定它用哪种 API 形态：
   回调地址（需公网 HTTPS URL）或长连接（免公网）。官方 101463：**同一时间只允许一种，切换即失效**。
2. **callback 形态**还需 Token + EncodingAESKey（43 位）；**long 形态**还需 `BotId` + `BotSecret`。
   两套凭据**不同源**，同键同时配两套 ⇒ 长连接启动期 fail-fast（这是官方约束的程序内表达，不是 SDK 挑剔）。
3. 机器人所属企业的应用凭据（`corpid` + 该应用的 `corpsecret`）仍需配置 —— 工具调用与主动回复走的是
   **应用级 `access_token`**，机器人自身没有独立令牌。
4. 真实推送需内网穿透或公网部署；本机开发请先跑 §5 的 SelfTest 通道，它把整条回调链（含两道 fail-closed 闸）
   离线验完。

## 3 环境变量

| 变量（均带 `MUDWECHAT_DEMO_` 前缀） | 必填 | 用途 |
|---|---|---|
| `WORK_CORP_ID` | 是 | 应用令牌签发（corpid） |
| `WORK_AGENT_SECRET` | 是 | 应用令牌签发（corpsecret），同时是工具执行器的凭据底座 |
| `WORK_BOT_PUSH_TOKEN` | callback 模式是 | 机器人回调验签 Token（`WechatCallbackOptions.Apps[default]`，`Channel = Bot`） |
| `WORK_BOT_PUSH_AES_KEY` | callback 模式是 | 机器人回调 AESKey，**长度必须 43** |
| `WORK_BOT_ID` | long 模式是 | 长连接 `aibot_subscribe` 的 `bot_id`（`WechatBots:Bots:default:BotId`） |
| `WORK_BOT_SECRET` | long 模式是 | 长连接订阅凭据（`WechatBots:Bots:default:BotSecret`） |
| `AI_CHANNEL` | 否 | `callback`（缺省）/ `long`；其它值启动即抛，防「拼错开关 ⇒ 演示错形态」 |
| `AI_CONNECT` | 否 | long 模式下 `=1` 才把 `EnableLongConnection` 置真（缺省 `false`：装配与启动期三闸照验，但不产生无限重连日志） |
| `ALLOW_WRITE` | 否 | 本工程无写路由，仅影响基座横幅 |

缺失时启动即失败并**一次性列出全部缺失项**；值含 `<` / `>` / `your-` 视为占位符，等同未配置。

## 4 运行

```bash
# 1) 构建（已在 Mud.Wechat.slnx 的 /Demos/ 组内，全量构建覆盖）
dotnet build Demos/Mud.Wechat.AI.AgentDemo -c Release

# 2) 无凭据：应立刻失败并列出缺失变量（预期行为，验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.AI.AgentDemo -c Release --no-build

# 3) callback 形态（默认 https://localhost:56563 / http://localhost:56564）
export MUDWECHAT_DEMO_AI_CHANNEL=callback
export MUDWECHAT_DEMO_WORK_CORP_ID=ww……
export MUDWECHAT_DEMO_WORK_AGENT_SECRET=……
export MUDWECHAT_DEMO_WORK_BOT_PUSH_TOKEN=……
export MUDWECHAT_DEMO_WORK_BOT_PUSH_AES_KEY=……          # 43 位
dotnet run --project Demos/Mud.Wechat.AI.AgentDemo -c Release --no-build

# 4) long 形态：先只装配置、不建连（观察启动期三闸）
export MUDWECHAT_DEMO_AI_CHANNEL=long
export MUDWECHAT_DEMO_WORK_BOT_ID=aibot……
export MUDWECHAT_DEMO_WORK_BOT_SECRET=……
dotnet run --project Demos/Mud.Wechat.AI.AgentDemo -c Release --no-build
#    → 日志：智能机器人 default 未启用长连接（EnableLongConnection=false），跳过建连。
#      真机联调再加 MUDWECHAT_DEMO_AI_CONNECT=1（假凭据下会看到 1s→2s→4s 指数退避的建连失败循环，
#      这正是「先退避再夺租约」的可见证据；真凭据请确认同机器人只有一条有效连接）。
```

```bash
curl http://localhost:56564/healthz
curl http://localhost:56564/demo/tools            # 三段口径应各 3 项（父接口桥接生效的证据）
curl "http://localhost:56564/demo/turn?question=查一下部门%201%20的名称"
curl http://localhost:56564/api/bot-events         # callback 模式：推送消费后 here 才有痕迹
```

## 5 离线自测（无需公网、无需真实机器人）

```bash
# 终端 A（callback 形态启动，端口 56564）
# 终端 B
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- --channel bot
```

`--channel bot` 用 SDK 自身的密码学内核（`WechatCallbackCrypto`）构造 JSON 安全模式报文，21 项断言：

| 断言组 | 期望 | 锁定的契约 |
|---|---|---|
| GET URL 验证 | 200，响应体是**解密后的明文** | Bot 的 echo 走 XML 侧同一接收器，四参签名（含 `echostr`）验签通过；**不消费指纹** |
| GET 篡改 `msg_signature` | 403 + **空体** | 拒绝形态不给「验签是否通过」的探测面 |
| GET 时间戳超窗 | 403 + 空体 | echo 只过时效闸（±300s），同 `echostr` 二次保存配置必须成功 |
| POST `text` 消息 | 200 + 应答解密为 `{}` | 内置 `WechatBotAgentHandler` 立即回空包、回合排后台 ⇒ **HTTP 面看不出回合成败** |
| 台账含本次 `msgid` | 200 | 证明报文真被解码并分发（不是只过了闸门） |
| POST `enter_chat` | 200 + 应答含 `"msgtype":"text"` | 官方**唯一**允许被动回复 `text` 的事件 |
| 台账含 `enter_chat` | 200 | 欢迎语处理器真接住 |
| POST `feedback_event` | 200 + `{}` | 官方 101027：该事件只支持回空包 |
| 台账含兜底记录 | 200 | 未明确接线的事件由空串键兜底记账，**不被静默丢弃** |
| 同一报文重放 | 403 | 一次性指纹闸 fail-closed（签名合法，只有这道闸能拒它） |
| 篡改 `msg_signature`（POST） | 403 | 验签失败绝不解密、绝不写台账 |
| 时间戳超窗（POST） | 403 | 验签通过仍被时效闸拒绝 ⇒ 两道闸相互独立 |
| 缺 `msg_signature` 参数 | 403 | 参数缺失 fail-closed，不回落「只验 timestamp」 |
| 体缺 `encrypt` 字段 | 403 | JSON 侧「报文非法」与 XML 侧「信封无 Encrypt」同判定 |
| 明文 `receiveid` 非空 | 403 | 官方 101033：自建智能机器人接收方 ID 恒为空串 |
| 明文缺 `msgid` | 403 | 幂等与排障都靠它，非空识别的官方必填 |
| Content-Type 非 xml/json | 415 | 形态闸在分发之前，不「凑巧解开再说」 |
| PUT 方法 | 405 | 方法闸先于验签，故无需合法报文 |
| 空体 + JSON | 400 | 接收侧目标清单的三态之一 |
| POST JSON 到通配路由 `/wechat` | 403 | 通配键条目是 `Channel = App`（加密 XML），JSON 打过来按 `UnknownReceiver` 拒 |
| `GET /healthz` 标识 `ai-agent` | 200 | 端口撞车时不会把别家 Demo 的结果当成本线通过 |

真实凭据 + 公网 URL（或长连接）才有意义的部分：`/demo/turn` 打到官方通讯录、机器人会话里的实际应答。

## 6 实现要点（改 Demo 前先读）

1. **形态开关只切「凭据面」，不切分发内核**：`AI_CHANNEL` 决定填 `WechatCallbackOptions.Apps[default]`
   （`Channel = Bot`）还是 `WechatBots:Bots:default:*`，而 `AddWechatCallback` **两种形态都必须调用** ——
   长连接与 HTTP 回共用同一个 `WechatBotEventDispatcher` 实例（ADR-8），处理器注册面因此完全一致。
2. **装配顺序有硬依赖**：`AddWechatCallback` → `AddWechatBotCallback()` →（long 模式）`AddWechatBotLongConnection`
   → `AddWechatWorkAgent`。最后一步的前置是 `IChatClient` + `AddWechatApp` + `AddAibotApi`；
   `AddWechatBotLongConnection` 的前置是分发器已在容器里，否则**注册期**抛。
3. **`Channel = Bot` 的条目必须留空 `ReceiveId`**：官方把自建智能机器人的接收方 ID 定为空串，
   填了会被 `WechatCallbackOptions.Validate()` 拒。配置委托里**显式跑一次 `o.Validate()`** ——
   中间件只在请求期逐应用校验，AESKey 位数、软超时越界这类问题否则要等第一个推送进来才暴露。
4. **工具面的三段口径不是一回事**，`/demo/tools` 把它们分开列：
   `enabledTools` = 白名单（策略，装配期冻结，热更不改）；`registryTools` = 已落地的执行器（装配）；
   `modelVisible` = 白名单投影后的模型可见面。`registryTools` 空而 `modelVisible` 非空 ⇒ 执行器依赖缺席；
   反过来 ⇒ 白名单把工具关掉了。
5. **策展执行器绑的是公共父接口，而父接口按契约不进 DI**（AGENTS §4：`IsAbstract = true` 不注册）⇒
   只 `AddContactApi()` 时三个部门工具是「软缺席」：白名单与可见面各有 3 个名字，注册表却是空的。
   Demo 的处置是**宿主侧显式桥接**（见 `Program.cs` ③.5），桥接的是同一实例，不新建客户端、不复制令牌链。
6. **授权器注册必须先于 `AddWechatWorkTools`**：`EnforceAuthorization` 默认开，容器里没有
   `IWechatToolAuthorizer` 时分发器**全拒**（fail-closed）。症状是「工具在名单里，调用却说未授权」，
   不是「工具全开」—— 忘了注册的表现比直觉更安全，但也更难想到。
7. **身份不进工具 Schema**：`appKey` / `authCorpId` 来自 `WechatToolBinding` 与 `IWechatAiCorpContextAccessor`，
   所以 `/demo/tools` 里 `modelVisible[].schema` 看不到任何凭据参数 —— 模型无法越权不是靠提示词约束的。
8. **成功路径的应答恒为 200 + 加密外壳**（软超时、无人接线也都是 200 + `{}`，与 XML 侧的 503 **不同**：
   官方对 Bot 只推一次且指纹已消费，回 503 等于丢事件）。⇒「谁接住了这条事件」只能看
   `/api/bot-events`，这就是台账存在的理由。代价与 §5 一致：注册空串键兜底后，
   分发结局恒为 `Handled`，「未找到处理器」的 Warning 不再出现 ⇒ 要看 `counts` 的键分布。
9. **同一事件键的处理器是「全量执行 + 首个非 null 应答」**：`text` 上并存的
   `WechatBotAgentHandler`（回空包、排后台回合）与 `TextEventObserver`（只记账）都会跑。
   观察器恒回 `null`，因此不参与应答竞争 —— 内置处理器不认识 Demo 台账，没有它就只剩日志可查。
10. **台账 ≠ 回合完成**：`TextEventObserver` 记的是「处理器被调用」。回合在后台调度器里跑，
    结果经 `response_url` 主动回复下发，只出现在日志与企微会话里。两个事实别混为一谈。
11. **正文与解密明文一律不落账、不进日志**（AGENTS §8）：`WechatBotCallbackEvent.DecryptedJson`
    是敏感载体；台账只有事件键、`msgid`、会话类型与应答形态。
12. **离线替身有「三重防自旋」**（`EchoChatClient`）：问题含「部门」∧ 本轮确有 `contacts.department.get`
    ∧ 还没执行过工具。缺第三重，替身会对同一条消息反复发起调用，直到 `MaxToolCallsPerTurn` 才收口。
13. **`/demo/turn` 是只读链路的离线入口**：假凭据下工具真会打到官方接口并失败（`40013`），
    失败被归一成错误文本回给模型 ⇒ 回合照样完成。看到「应答里说查不到」不是 Demo 坏了，
    正是**错误归一**在工作。真凭据时它是一次真实的通讯录读取。
14. **长连接默认不建连**（`EnableLongConnection=false`）：装配、`WechatBotOptions.Validate()`、
    跨面互斥、租约缺省告警这四件事都能离线观察，而不因假凭据产生重连日志洪水。
    `AI_CONNECT=1` 才进真连接循环 —— 注意官方「每机器人仅一条有效连接」，重复起进程会互相踢。
15. **多实例部署必须换掉 Demo 的进程内租约**：`InProcessBotConnectionLease` 只保证同进程互斥，
    **不**满足官方跨实例唯一性。生产形态是 `AddWechatRedis`（其端口面含 `RedisWechatBotConnectionLease`），
    且注册顺序必须在 `AddWechatBotLongConnection` 之前。
16. **本 Demo 刻意不内嵌回调处理器分析器**：`MUDCB002~005` 校验的是 XML 侧
    `SupportedEventType ↔ [WechatCallbackContract]` 载荷一致性，而 Bot 事件键不进 `OfficialPayloadContracts`
    ⇒ 内嵌即静默空跑，只制造「编译期已校验」的错觉（同 DM-10 的理由）。

## 7 本 Demo 暴露的 SDK 面（本轮实测，均未通过改 `Src/` 绕过）

三处都是**装配期**缺口：SDK 的降级语义写在文档注释里，但 ASP.NET Core 在 Development 下默认
`ValidateOnBuild=true`，容器在建容器阶段就要求每个构造依赖可解析 —— 「留空 + 运行期告警」这条路够不着。

1. **公共父接口 + 策展执行器 = 工具面恒空**：`ContactsDepartmentToolHandlers` 构造依赖
   `IWechatWorkDepartmentsService`（父接口，`IsAbstract = true`），而契约面规定父接口不注册 DI
   ⇒ 按 SDK 文档清单装配（`AddWechatApp` + `AddContactApi` + `AddWechatWorkTools`）得到的是
   「白名单 3 项、注册表 0 项」的空工具面，且**只有真调用**才报「在白名单内但未注册」。
   处置：Demo 侧桥接（`Program.cs` ③.5）。SDK 侧的可选收敛是在 `AddWechatWorkTools` 内按已注册子接口自动桥接。
2. **长连接 Runner 依赖裸类型 `WechatCallbackOptions`**：`AddWechatCallback` 各重载只注册
   `IOptions` / `IOptionsMonitor`，裸类型全仓无人注册 ⇒ Development 下 `builder.Build()` 直接失败
   （连 `EnableLongConnection=false` 也拦，校验只看构造签名）。同包的 `WechatBotOptions` 却自带裸类型快照注册
   （见 `AddWechatBotLongConnection`），两者形态不一致。处置：Demo 侧按同形补
   `AddSingleton(sp => sp.GetRequiredService<IOptions<WechatCallbackOptions>>().Value)`。
3. **`IWechatBotConnectionLease` 无进程内默认实现**：Runner 声明可选（`null` = 进程内唯一 + 告警一次，
   见其类注释），唯一实现来自**可选包** `Mud.Wechat.Redis` ⇒ 「不引 Redis 的单实例宿主」在 Development 下
   根本装不起来，被文档承诺的告警路径不可达。处置：Demo 侧 `InProcessBotConnectionLease`（§6.15 说明其边界）。
4. **未追的一项（不属 Demo 范围，留此备案）**：启动日志里
   `TokenRefreshHostedService` 先告警「未注册任何令牌管理器」，随后 `WechatTokenRegistrationService` 记
   「默认应用 default 的令牌管理器已登记到后台刷新服务」，但其启动横幅仍是「已注册 0 个令牌管理器」。
   登记动作未抛异常，计数却为 0 —— 疑在组件（`Mud.HttpUtils`）的登记时序 / 计数口径，
   与本轮三处缺口无关，未继续深挖（Demo 不改 `Src/`，更不改组件）。

## 8 官方文档（ID 以代码注释为准，本 README 不复述逐页细节）

- 智能机器人 API 模式（回调 / 长连接二选一、每机器人一条有效连接）：**101463**
- 机器人回调接收方 ID（自建机器人恒空串）与 `msgid` 必填：**101033**
- `feedback_event` 只支持回空包：**101027**
- 逐事件 / 逐帧的官方页与业务限制（频率上限、流式刷新窗口、素材分片上限、退避与夺租约顺序）：
  `Src/Work/Mud.Wechat.Work/Callback/Bots/*.cs`、
  `Src/Work/Mud.Wechat.Work.Callback/LongConnection/*.cs`、
  `Src/Work/Mud.Wechat.Work/Interfaces/Aibot/*.cs` 的 XML 注释（每个端点/事件都有出处，两处漂移以源码为准）
- 本仓契约面：`Src/AI/*/README.md`、`Src/Work/Mud.Wechat.Work.Callback/README.md`（长连接 P3 章节）、
  `Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBotContractGuards.cs`、
  `Tests/Mud.Wechat.Work.Callback.Tests/WechatBotLongConnectionTests.cs`

## 9 安全须知

密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；不要接生产机器人
（AI 生成的应答会直接进成员会话，且长连接与回调互斥意味着演示动作可能**改变线上机器人的接入形态**）。
`WORK_AGENT_SECRET` / `WORK_BOT_SECRET` / `PushToken` / `EncodingAESKey`、
`WechatBotCallbackEvent.DecryptedJson`、消息正文、`response_url` 一律不出日志、不出响应体；
`/healthz` 只回非敏感摘要，`/api/bot-events` 只回事件键与应答形态。
两道 fail-closed 闸（±300s 时效 + 一次性指纹）与空 `ReceiveId` 校验不得为「跑通」而放宽；
工具面的白名单与授权器是**两道独立闸**，不得为演示方便把 `EnforceAuthorization` 关掉。

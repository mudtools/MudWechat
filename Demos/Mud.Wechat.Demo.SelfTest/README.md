# Mud.Wechat.Demo.SelfTest — 回调链路离线自测工具

## 目的

在没有企业微信 / 公众号 / 开放平台 / 微信小店服务器（也没有可信公网域名）的前提下，验证回调 Demo 的整条接收链路：
**验签 → 时效闸 → AES 解密 → receiveid 校验 → 抗重放指纹闸 → 事件分发**。

工具用 SDK 自身的密码学内核 `WechatCallbackCrypto`（落叶层 `Mud.Wechat.Abstractions`）构造**合法**报文，
再人为破坏其中一环，观察被探测宿主返回的状态码是否符合契约。

因此它只引用 `Mud.Wechat.Demo.Shared` 与 Core 叶层工程，**不引用任何产品线主包**，
也不会被误当成某条线的客户端示例。

`--channel` 决定验签形态：`work`（企微 XML）、`mp`（公众号 XML）、`component`（开放平台第三方平台推送）、
`channels`（微信小店 JSON 安全模式）与 `bot`（企微智能机器人 JSON 安全模式）已实现；
差异本身就是断言面（GET 三参/四参、回显密文原文/解密明文/`echostr` 原文、receiveid = AppId/CorpId/平台 AppId/小店 AppId/**空串**，
以及**拒绝时的表达形态**：XML 两线与小店回 403（小店还额外要求**空体**），
开放平台只区分响应体 `success` / `fail`，机器人通道回 **403 + 空体** 且**成功路径恒 200**）。

**无探针通道的三条线各自原因不同**：小程序与广告线**官方无推送面**（无 Callback 包 ⇒ 没有可构造的报文），
支付线的验签依赖**平台证书**、本机离线伪造不了（规划方案 R-3）。
它们的离线可验证面见各自 Demo 的 README（广告线为「假凭据起进程 + 真实出网一次拿到官方 `code`」，见 `Mud.Wechat.Ads.Demo/README.md` §5）。

## 前置条件

1. 目标回调 Demo 已在运行（`work` → `Mud.Wechat.Work.ContactCallbackDemo`，`mp` → `Mud.Wechat.OfficialAccount.Demo`，
   `component` → `Mud.Wechat.OpenPlatform.Demo`，`channels` → `Mud.Wechat.Channels.Demo`，
   `bot` → `Mud.Wechat.AI.AgentDemo`（须以 `MUDWECHAT_DEMO_AI_CHANNEL=callback` 启动 —— 长连接形态不暴露 HTTP 回调路由））。
2. 本进程能读到与被测宿主**完全相同**的回调凭据（同一 `PushToken` / `PushEncodingAESKey` / 接收方 ID），
   否则验签与解密必然失败——那是被测配置的问题，不是本工具的缺陷。

| 环境变量 | 通道 | 说明 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN` | work | 被测宿主的回调 Token |
| `MUDWECHAT_DEMO_WORK_CALLBACK_AES_KEY` | work | 被测宿主的 EncodingAESKey，**必须 43 字符** |
| `MUDWECHAT_DEMO_WORK_CORP_ID` | work | 解密明文的 `receiveid`，与被测宿主配置一致 |
| `MUDWECHAT_DEMO_MP_CALLBACK_TOKEN` | mp | 同上（公众号后台「服务器配置」的 Token） |
| `MUDWECHAT_DEMO_MP_CALLBACK_AES_KEY` | mp | 同上，43 字符 |
| `MUDWECHAT_DEMO_MP_APP_ID` | mp | 公众号的 `receiveid` 就是 AppId |
| `MUDWECHAT_DEMO_OPENPLATFORM_TOKEN` | component | 「授权事件接收 URL」的 Token |
| `MUDWECHAT_DEMO_OPENPLATFORM_AES_KEY` | component | 同上，43 字符 |
| `MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_ID` | component | 第三方平台 AppId —— 本线的 `receiveid` **不是**授权方 appid |
| `MUDWECHAT_DEMO_CHANNELS_CALLBACK_TOKEN` | channels | 小店「消息与事件接收配置」的 Token |
| `MUDWECHAT_DEMO_CHANNELS_CALLBACK_AES_KEY` | channels | 同上，43 字符（小店**强制安全模式**，探针先本地校验长度再发请求） |
| `MUDWECHAT_DEMO_CHANNELS_APP_ID` | channels | 解密明文尾部 `appid` 的校验基准（`ChannelsAppCallbackOptions.AppId`） |
| `MUDWECHAT_DEMO_WORK_BOT_PUSH_TOKEN` | bot | 智能机器人回调 Token（与 `work` 通道的 Token **不是同一份**，两套凭据分属两个配置面） |
| `MUDWECHAT_DEMO_WORK_BOT_PUSH_AES_KEY` | bot | 同上，43 字符（探针先本地校验长度再发请求） |
| —— | bot | **无 `receiveid` 变量**：官方 101033 把自建智能机器人的接收方 ID 定为空串，探针因此**用空串加密**，并把「明文 `receiveid` 非空」做成一条 403 断言 |

## 用法

```bash
# 1) 起被测宿主（凭据按被测 Demo 的 README 配置）
dotnet run --project Demos/Mud.Wechat.Work.ContactCallbackDemo

# 2) 另开终端跑自测（退出码 0 = 全部通过，1 = 有断言失败，2 = 参数错误）
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel work
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel mp
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel component
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel channels
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel bot
```

| 参数 | 默认 | 说明 |
|---|---|---|
| `--channel` | `work` | 探测通道；当前 `work` / `mp` / `component` / `channels` / `bot` |
| `--base-url` | `work` → `http://localhost:56534`，`mp` → `http://localhost:56554`，`component` → `http://localhost:56560`，`channels` → `http://localhost:56562`，`bot` → `http://localhost:56564` | 被测宿主根地址（取各自 `launchSettings.json` 的 http 档） |
| `--route` | `work` → `/wechat/default`，`mp` → `/mp/default`，`component` → `/wechat/component`，`channels` → `/channels/default`，`bot` → `/wechat/default` | 回调路由，需与宿主 `GlobalRoutePrefix` + `AppKey` 一致 |
| `--allow-remote` | 关 | 允许对非本机地址发起探测（默认拒绝，防误指向生产域名） |
| `--health-fragment` | 各通道用本线专属 Demo 的标识 | `/healthz` 响应体须包含的宿主名。**指向多线共存宿主（DM-12）时必须显式给 `--health-fragment all-in-one`** —— 否则这条断言会把「端口撞车打到别家 Demo」误报成「共存宿主不是本线 Demo」；只有 `bot` / `channels` 两通道消费它 |

**打 DM-12 时的两条环境差异**（否则会得到与 SDK 无关的失败）：

```bash
# ① 机器人通道的默认路由是 /wechat/default，而 DM-12 把机器人挂在 /wechat/bot
#    （Apps 字典按 appKey 唯一，同一键不能同时挂 Channel=App 与 Channel=Bot）
# ② Git Bash 会把 --route /wechat/bot 做成 path conversion ⇒ file:// scheme 报错，须关掉
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- \
  --channel bot --base-url http://localhost:56566 --route /wechat/bot --health-fragment all-in-one
```

DM-12 上的实测结果与两处**设计边界**（`bot` 的通配路由断言、`component` 的授权码编排断言）
记在 `Demos/Mud.Wechat.AllInOne.Demo/README.md` §5。

## 断言清单

### `--channel work`（企业微信，四项）

| # | 断言 | 构造 | 期望 |
|---|---|---|---|
| 1 | URL 验证（GET echo） | 加密一段 `echostr`，`msg_signature = SHA1(token,timestamp,nonce,echostr)` | `200` 且响应体 = 解密回明文（与官方原样回显一致） |
| 2 | 业务事件投递（POST） | `change_contact` / `create_user` 官方 90970 报文，`Encrypt` 参与四元签名 | `200` 且响应体为 `success`，宿主日志出现处理器命中行 |
| 3 | 同一报文重放 | 复用断言 2 的 `timestamp`/`nonce`/报文体再发一次 | `403`（指纹闸在解密+receiveid 成功后消费，fail-closed） |
| 4 | 签名篡改 | 在 `msg_signature` 前插一个字符 | `403`（`msg_signature 不匹配`） |

### `--channel mp`（公众号，五项）

| # | 断言 | 构造 | 期望 |
|---|---|---|---|
| 1 | URL 验证（GET echo） | **三参** `signature = SHA1(token,timestamp,nonce)`（**不含** `echostr`） | `200` 且响应体 = `echostr` **密文原文**（公众号契约与企微相反） |
| 2 | 文本消息触发被动回复 | 含「帮助」的文本消息（官方 17888 结构），四参 `msg_signature` | `200` 且回包为加密 XML；解密后 `receiveid = AppId`、明文含回复文案 |
| 3 | 同一报文重放 | 复用断言 2 的三元组与包体 | `403` |
| 4 | 签名篡改 | 在 `msg_signature` 前插一个字符 | `403` |
| 5 | 安全模式拒收明文 | 合法三参 `signature` + 明文包体、无 `msg_signature` | `403`（POST 密文分支不得回落 `signature`，即守卫 CB-MP-2 的运行期反证） |

断言 1 特意**不消耗**指纹：官方「二次保存配置」会用同一 `echostr` 再来一次，GET 只过时效闸。
断言 2 同时覆盖「密文进、密文出」：安全模式下只要回包非 `success` 就必须加密（CB-MP-3）。

### `--channel component`（开放平台第三方平台推送，十五项）

| # | 断言 | 构造 | 期望 |
|---|---|---|---|
| 1 | 合法票据推送 | `component_verify_ticket` 明文，四元 `msg_signature`（`Encrypt` 参与） | 响应体逐字 `success` |
| 2 | 票据真的落库 | 紧接断言 1 读宿主 `GET /api/ticket` | `hasTicket=true`（只回长度与指纹，不回票据原文） |
| 3 | 签名篡改 | 把 `msg_signature` 首字符改掉 | `fail`（验签失败即返回，不进入解密与写库） |
| 4 | 时间戳越界 | 签名按 `now-600s` 计算（**验签是过的**），只有时效闸能拒 | `fail` |
| 5 | 跨平台重放 | 用别的 appid 作 `receiveid` 加密 | `fail`（appid 比对拦住「拿别家报文灌进来」） |
| 6 | `authorized` 事件 | 带 `AuthorizationCode` 的授权事件 | `success` |
| 7 | 台账出现授权码 | 读宿主 `GET /api/push-events` | `pendingAuthCodeAvailable=true` |
| 8 | 消费授权码 | `POST /api/auth/exchange-latest` | 走到换取路径即算通过（假凭据下必然 502，官方结果不属本探针断言面） |
| 9 | 同一报文重放 | 复用断言 6 的完整查询串与包体 | `success`（本线无指纹闸，官方只认 `success`） |
| 10 | 重放不得重装已消费的码 | 紧接断言 9 再读台账 | `pendingAuthCodeAvailable=false` |
| 11 | 新的 `authorized`（新码） | 换一个 `AuthorizationCode` 再推 | `success` |
| 12 | 台账重新装满 | 读台账 | `pendingAuthCodeAvailable=true`（证明断言 10 测的是「清」而不是撞上空台账） |
| 13 | `unauthorized` 事件 | 同一 `AuthorizerAppid`、官方此时不带授权码 | `success` |
| 14 | 取消授权即清码 | 读台账 | `pendingAuthCodeAvailable=false`（旧码不得再被消费成 40029） |
| 15 | 未知 `InfoType` | `selftest_unknown_info_type` | `fail`（未建模类型刻意不回 success，让丢失在监控上可见） |

**本通道的期望值一律写在响应体上**（`success` / `fail`），不是状态码 ——
官方对「授权事件接收 URL」的唯一判定就是响应体是否逐字 `success`，
把 `work` / `mp` 那套「403 表拒绝」照搬过来即断言失效。

### `--channel channels`（微信小店，JSON 安全模式，十七项）

| # | 断言 | 构造 | 期望 |
|---|---|---|---|
| 1 | GET URL 验证回显 `echostr` **原文** | **三参** `signature = SHA1(token,timestamp,nonce)`（不含 `echostr`），`echostr` 为 AES 密文 | `200` 且响应体逐字等于密文原文（不解密、不消费指纹） |
| 2 | GET 篡改 `signature` | 首字符翻转（保持长度与十六进制形态） | `403` + **空体** |
| 3 | GET 时间戳越界 | 签名按 `now-600s` 计算 | `403` + 空体 |
| 4 | POST `product_spu_update` | JSON 信封 `{"ToUserName","Encrypt"}`，四元 `msg_signature`（`Encrypt` 参与） | `200` + 明文 `success` |
| 5 | 台账出现本次 `product_id` | 读宿主 `GET /api/callback-events?take=200` | 含该 ID ⇒ **类型化载荷真被读出**，不是只回了 success |
| 6 | 同一报文重放 | 复用断言 4 的 `timestamp`/`nonce`/包体 | `403`（指纹闸 fail-closed） |
| 7 | 篡改 `msg_signature` | 首字符翻转 | `403`（不解密、不写台账） |
| 8 | 跨属主推送 | 用另一 AppId 加密并签名（签名与密文都合法） | `403`（明文尾部 `appid` ≠ 配置 `AppId`） |
| 9 | POST 时间戳越界 | 签名按 `now-600s` 计算（**验签是过的**） | `403`（证明时效闸与验签闸相互独立） |
| 10 | 缺 `msg_signature` 参数 | 只带 `timestamp`/`nonce` | `403`（缺失即拒，不回落「只验 timestamp」） |
| 11 | 体缺 `Encrypt` 字段 | `{"ToUserName":"gh_x"}` | `403`（JSON 侧的「报文非法」） |
| 12 | POST 未接线事件 `set_shop_nickname` | 合法信封与签名 | `200` + `success`（官方「已收到」≠「已处理」） |
| 13 | 台账含「兜底记录」 | 读台账 | 未命中专属处理器时由空串键兜底记账，未被静默丢弃 |
| 14 | Content-Type 非 JSON | 合法查询串与包体，`application/xml` | `415`（不进接收链） |
| 15 | PUT 方法 | 任意 JSON 体 | `405`（方法闸在验签之前，故无需合法报文） |
| 16 | 路由 AppKey 未配凭据 | `POST /channels/not-configured` + 合法报文 | `403`（`UnknownReceiver`，**不回落默认应用**） |
| 17 | `/healthz` 身份确认 | 读 `GET /healthz` | 响应体含 `"channels"`（端口撞车时不会把别家 Demo 当成本线结果） |

**本通道与 `mp` 同为 XML/JSON 之外的第三种契约形态**，四处不可互抄：信封是 JSON（非 XML）、
官方**强制安全模式**（没有「明文分支」⇒ 不存在「安全模式拒收明文」这条断言，其位置由断言 11 顶上）、
成功应答恒为**明文** `success`（无被动回复语义 ⇒ 不需要加密回包）、`receiveid` 基准是小店 AppID 且属**必填**配置。
拒绝态一律 `403 + 空体` —— 断言 2/3 特意把「空体」写进期望值，因为「403 带一句拒绝理由」
等于把「验签是否通过」这个信息交给外部探测者。

### `--channel bot`（企业微信智能机器人，JSON 安全模式，二十一项）

| # | 断言 | 构造 | 期望 |
|---|---|---|---|
| 1 | GET URL 验证回显 **解密后的明文** | **四参** `msg_signature = SHA1(token,timestamp,nonce,echostr)`，`echostr` 为 AES 密文 | `200` 且响应体是明文（不是密文原文、也不是 `echostr` 逐字回显）；**不消费指纹** |
| 2 | GET 篡改 `msg_signature` | 首字符翻转 | `403` + **空体** |
| 3 | GET 时间戳越界 | 签名按超窗值计算 | `403` + 空体（echo 只过时效闸 ⇒ 同 `echostr` 二次保存配置必须成功） |
| 4 | POST `text` 消息 | JSON 信封 `{"encrypt":...}`，三参 `msg_signature` | `200` + 应答解密为 `{}`（内置处理器立即回空包、回合排后台） |
| 5 | 台账出现本次 `msgid` | 读宿主 `GET /api/bot-events` | 含该 `msgid` ⇒ 报文真被解码并分发，而不是只过了闸门 |
| 6 | POST `enter_chat` | 合法信封与签名 | `200` 且应答明文含 `"msgtype":"text"`（官方唯一允许被动回复 text 的事件） |
| 7 | 台账含 `enter_chat` | 读台账 | 欢迎语处理器接住了事件 |
| 8 | POST `feedback_event` | 合法信封与签名 | `200` + `{}`（官方 101027：该事件仅支持回复空包） |
| 9 | 台账含「兜底记录」 | 读台账 | 未精确接线的事件由空串键兜底记账，未被静默丢弃 |
| 10 | 同一报文重放 | 复用断言 4 的 `timestamp`/`nonce`/包体 | `403`（指纹闸 fail-closed） |
| 11 | 篡改 `msg_signature` | 首字符翻转 | `403`（不解密、不写台账） |
| 12 | POST 时间戳越界 | 签名按 `now-600s` 计算（**验签是过的**） | `403`（时效闸与验签闸相互独立） |
| 13 | 缺 `msg_signature` 参数 | 剥掉该参数、保留其余查询串 | `403`（缺失即拒，不回落「只验 timestamp」） |
| 14 | 体缺 `encrypt` 字段 | `{"msgtype":"text"}` | `403`（JSON 侧的「报文非法」） |
| 15 | 明文 `receiveid` 非空 | 用非空接收方 ID 加密并签名 | `403`（官方 101033：自建智能机器人恒为空串） |
| 16 | 明文缺 `msgid` | 去掉 `msgid` 后重新加密签名 | `403`（非可识别的官方报文） |
| 17 | Content-Type 非 xml/json | 合法查询串与包体，`text/plain` | `415`（不进接收链） |
| 18 | PUT 方法 | 任意 JSON 体 | `405`（方法闸在验签之前） |
| 19 | 空请求体 + JSON | `Content-Type: application/json`、零字节体 | `400`（机器人侧空体**单独成态**，与 XML 侧不同） |
| 20 | POST JSON 到通配路由 `/wechat` | 合法机器人报文，打到仅前缀的路由 | `403`（通配键条目是 `Channel = App` ⇒ 两条通道报文形态互斥） |
| 21 | `/healthz` 身份确认 | 读 `GET /healthz` | 响应体含 `"ai-agent"` |

**本通道是第四种契约形态**，三处不可与其它通道互抄：GET 用**四参**（`mp` / `channels` 是三参）却回显**解密明文**；
应答是**加密外壳** `{encrypt, msgsignature, timestamp, nonce}`（`msgsignature` **无下划线**、`nonce` 复用回调 URL 的值）；
**成功路径恒 200** —— 软超时与「无人接线」都是 200 + 加密空包（XML 侧此时是 503，
因为官方对 Bot 只推一次且指纹已消费，回 503 等于丢事件）。

## 为什么这些结论值得自动化

- 断言 3 是**最容易在重构中被悄悄削弱**的一条：把指纹闸从 fail-closed 改成 fail-open，
  业务链路仍然「全绿」，但重放攻击面就打开了。跑一次自测即可证伪。
- 断言 4 覆盖「四元签名里 `Encrypt` 字段参与、而非密文外层 `AgentID`」这一官方反直觉点。
- 断言 1 覆盖 PKCS7 手工补位：若改用 .NET 内置 16 字节块填充，`EncodingAESKey` 长度校验或解密会先炸。
- `mp` 的断言 1 与 5 成对看守「GET 三参 / POST 四参」这条**两线不同形**的边界：
  任一侧被「顺手对齐」成另一侧的形态，两项断言里必有一项变红。
- `component` 的断言 4 与 5 专门把「验签通过」做成前置条件：时效越界与跨平台 `receiveid` 都发生在
  **签名合法**之后 ⇒ 一旦有人把判定顺序改成「先解密再验签」或「时效闸挪到写库之后」，这两项立刻变红。
  断言 8~12 则看守授权码的一次性语义：官方「没收到 success 就重试」会把同一条报文再推一次，
  若台账把已消费的码重新装满，下一次换取会凭空撞 `40029`，而 Demo 表面上一切正常。
- `channels` 的断言 5 与 13 是本通道独有的两条：**状态码不足以证明「事件被消费」**。
  小店中间件对「正常」和「无人接线」都回明文 `success`（分发器对未命中只记 Warning），
  故两条断言打在宿主的 `/api/callback-events` 台账上 —— 前者证明类型化载荷真被反序列化出来，
  后者证明未接线事件被兜底记录接住。若有人把类型化通道门控（`CHANNELS_CALLBACK_TYPED_PAYLOAD` /
  `net8+` / 事件键契约登记）改坏，断言 4 仍绿、断言 5 立刻变红。
- `channels` 的断言 8/16 看守两条「不得回落」的边界：跨属主 `appid` 必须拒（否则能把别家小店的报文灌进来），
  未配置的 AppKey 必须拒（否则多小店部署会静默用默认应用的凭据去解别人的报文）。
- `bot` 的断言 1 与 4/6/8 成对看守「应答形态」：GET 回**解密明文**、POST 回**加密外壳**，
  两处形态一被「顺手对齐」（例如把 echo 改成逐字回显、或把应答外壳的 `msgsignature` 写成 `msg_signature`）
  就立刻变红 —— 后者是官方字段名，带下划线即被企微判为验签失败，而本机看不出问题。
- `bot` 的断言 15 是本通道独有的一条：明文 `receiveid` **非空**必须拒。它是官方 101033 的正向表达
  （自建智能机器人接收方 ID 恒为空串）⇒ 若有人把 `Channel = Bot` 的 `ReceiveId` 校验放宽成「留空跳过」，
  攻击面就是「别人把机器人报文配上你的 CorpId 也能收」。
- `bot` 的断言 19/20 看守「同一路由族里的形态互斥」：空体单独成 400（不是 403），
  JSON 打到 `Channel = App` 的通配键条目必须 403 —— 后者保证「两条通道靠 `Channel` 消歧」不是靠运气分流。

## 安全注意

- 凭据只走环境变量或本机用户机密，**不得**写进 `appsettings*.json`、命令行参数或提交仓库。
- 本工具把 `PushToken` / `EncodingAESKey` 只用于本地签名与加密计算，不打印、不入日志。
- 默认拒绝远端地址；若确需探测隧道域名，显式加 `--allow-remote` 并确认对端是测试环境。
- 断言 2 构造的用户标识带 `selftest_` / `oSelfTest` 前缀，与真实成员或关注者账号天然隔离，不会污染业务数据。
  `channels` 通道同理：`FromUserName` 恒为 `gh_selftest`，商品 ID 用 `98000000` + 秒级时间戳的保留前缀，
  且每次运行换新值（避免命中上一次运行的台账残留）—— 本工具只往**本机宿主**推报文，不触达真实小店。
- `bot` 通道的 `msgid` 恒为 `selftest-{事件键}-{时间戳}-{GUID}`，会话类型取 `single`、成员 ID 用 `selftest` 前缀，
  且探针**刻意不带 `response_url`** ⇒ 即便被测 Demo 的 Agent 回合被排进后台，也不会向任何真实会话外发应答。

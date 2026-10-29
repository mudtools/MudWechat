# 智能机器人主动回复消息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Aibot` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Src/Work/Mud.Wechat.Work/Interfaces/Aibot/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Aibot/)（`IWechatWorkAibotService.cs` · `IWechatWorkInternalAibotService.cs` · 目录级 `.editorconfig`） |
| DTO 源文件 | [AibotMessage.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Aibot/AibotMessage.cs)（含 `AibotTextBody` / `AibotMarkdownBody` / `AibotStreamBody` / `AibotStreamItem` / `AibotStreamImageItem`）· [AibotReplyResponse.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Aibot/AibotReplyResponse.cs) |
| 契约守卫 | [WechatAibotContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAibotContractGuards.cs)（AI1 路由与 `response_code` Query / AI2 仅自建层级 / AI3 无 `[Token]` / AI4 35 个 DTO 登记 / AI5 字段陷阱） |
| 官方文档 | 101138（企业自建应用「主动回复消息」）· 页面最后更新未核验（本次未取官方页）；101031 / 101027 / 100719 / 101463 属回调与长连接面，本次未重取 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 1 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AI1~AI5 + 两个接口文件与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Aibot/README.md) + `WechatWorkResponse` / `WechatApiResponseGuard` 源码（2026-10-29 读取） · 官方：本次未取官方页，101138 相关约束继承接口 XML 注释与域 README 的既有转写 |

## 1. 这个接口族做什么

本族只有 1 个 HTTP 端点：用机器人回调帧里给出的 `response_url`，在其携带的一次性凭据 `response_code` 有效期内主动回一条消息给用户。它解决的是「回调应答窗口装不下业务耗时」的问题——典型场景是先让回调处理器回空包，等 LLM 推理、工单查询或人工确认完成后再写回结果。

调用凭据不是任何令牌，而是 URL 上的 `response_code`：每个 `response_url` 只能调用一次、有效期 1 小时（接口 XML，官方 101138）。因此本端点没有令牌换取、刷新、归属域校验，也没有可重试语义。

本族只消费 `AibotMessage`（及其分支体）与 `AibotReplyResponse`。同目录共 35 个类型（AI4），其余分属回调接收（`AibotMessageCallback` / `AibotEventCallback` / `AibotEncryptedEnvelope`）、长连接帧（`AibotFrames.cs`）与长连接媒体回复体（`AibotMediaReplyBody` / `AibotVideoReplyBody`，本端点不支持媒体）。这些类型不改变本 HTTP 族的支持面；回调接收与长连接由 `Mud.Wechat.Work.Callback` 包承载。

**不该用本族的场景**：要在回调 HTTP 应答里同步回消息，走 Callback 包的被动回复；要流式刷新，走被动回复（回调地址模式）或长连接；要发图片/文件/语音/视频，走长连接；要把企业主体加密 userid 换算成服务商主体 `open_userid`，走 `AccountId` 域的 `IWechatWorkAccountIdBotService`。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` | `AddWechatApp` 注册期调 `WechatAppConfig.Validate()` |
| 必填配置属性 | `CorpId` · `AgentSecret` | 同上；`Internal` 缺任一项抛 `InvalidOperationException` |
| 令牌类型 | 无（不使用 `WechatTokenTypes` 任一常量）→ 官方凭据为 URL 查询参数 `response_code` | — |
| 令牌归属键 | 无（不进入 `WechatTokenRouting.OwnedKeys`，也不会触发 `WechatTokenOwnerMismatchException`） | — |

本端点不需要令牌，但仍必须先调 `AddWechatApp`：`AddWechatWorkServices(...).Build()` 会检查容器里存在 `IWechatAppManager`，缺失即抛 `InvalidOperationException`（`WechatWorkServiceCollectionExtensions.cs` 与 `WechatWorkServiceBuilder.Build()`）。另外 `Internal` 的 `Validate()` 强制要求 `CorpId` + `AgentSecret`，即使 `AgentSecret` 对本端点没有任何消费点，也不能省。

`response_code` 由宿主从回调帧的 `response_url` 提取后作为方法参数显式传入（`[Query("response_code")]`），SDK 不注入、不缓存、不刷新。父子接口都不声明 `[Token]`：补上它会让生成器按默认令牌类型注入本不该存在的令牌参数（AI3）；组件分析器 HTTPCLIENT018 只能通过目录级 `.editorconfig` 的 `dotnet_diagnostic.HTTPCLIENT018.severity = none` 豁免，不能改用 `[Token(Justification)]`。

`TokenManage` 仍然恒为 `nameof(IWechatAppManager)`，它与令牌注入无关，只配置应用切换管理器；父子两级必须一致，否则生成器以 `new` 隐藏基类切换成员（HTTPCLIENT028），基类调用会静默走默认模式。本端点的报文不含任何应用身份，不要因为 `TokenManage` 存在就推断它会注入令牌。

### 2.2 权限与业务前置

- 智能机器人须先在企业管理后台创建并完成 API 模式配置（回调 URL / Token / EncodingAESKey），或配置长连接（BotID / Secret）；凭证都在企业侧后台，不在本 SDK 配置里（接口 XML）。
- `response_url` 只出现在带交互的消息族与 `template_card_event` 中：消息族的 `text` / `image` / `mixed` / `voice` / `file` / `video` 携带，流式消息刷新（`msgtype=stream`）不携带；事件族里 `template_card_event` 携带，`feedback_event` 无此字段语义（`AibotMessageCallback.ResponseUrl` / `AibotEventCallback.ResponseUrl` 注释）。因此流式更新不能靠本端点，只能走被动回复或长连接。
- 每个 `response_url` 仅可调用一次、有效期 1 小时，超期不可用（接口 XML，官方 101138）。
- 同一会话（回复 + 主动推送合计）官方频控为 30 条/分钟、1000 条/小时（接口 XML `<remarks>`）。
- 群聊中的主动回复会引用触发回调的用户消息；`template_card` 消息不支持引用，官方默认生成一条空消息引用；`template_card.feedback.id` 非空时可启用用户反馈事件（接口 XML `<remarks>`）。
- 上游回调侧应按 `msgid` 排重（官方可能因网络原因重复回调，见 `AibotMessageCallback` 注释）；重复触发业务会白白消耗一次性凭据。

官方页对可见范围、机器人状态等权限条件的具体表述本次未取，见 §7。

### 2.3 官方开放的应用类型

只有企业自建应用。官方 9 篇智能机器人文档正文零提及第三方应用 / 服务商代开发，机器人及其凭证都在企业管理后台配置，因此本域是「零端点父接口 + 唯一自建子接口」，没有 `_ThirdParty` / `_Provider` 子接口（AI2 锁定继承链上恰为 `IWechatWorkInternalAibotService`）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAibotService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，零端点，仅作契约基座） | 0 | 否 |
| `IWechatWorkInternalAibotService` | `Mud.Wechat.Work` | 企业自建应用子接口（唯一子接口，承载全部端点） | 1（E1） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Aibot` / `AddAibotApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Aibot`
- 端点索引：**E1** `ReplyAsync`（经 `response_url` 的一次性凭据主动回复消息）
- 生成实现：父接口实现类 `WechatWorkAibotService` 落 `Mud.Wechat.Work.Interfaces.Internal`，子接口实现类落 `Mud.Wechat.Work.Internal`；子接口以 `InheritedFrom = nameof(WechatWorkAibotService)` 串联。

最终用户注入**子接口**。父接口不注册 DI、`RegistryGroupName` 为空（AI2），直接消费它拿不到实现。

---

## 4. 端点

### E1. `ReplyAsync` — 用 `response_url` 的一次性凭据主动回复消息

**签名**

```csharp
Task<AibotReplyResponse?> ReplyAsync(
    [Query("response_code")] string responseCode,
    [Body] AibotMessage message,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/aibot/response` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=RESPONSE_CODE` |
| 凭据参数 | `response_code`（Query，方法显式参数，SDK 不注入；参数名照抄官方原文，不是 `responseCode`） |
| 适用应用类型 | 企业自建应用（第三方 / 代开发官方未开放） |
| 请求体编码 | `application/json`，源生成序列化（`AibotJsonContext`，AI4 锁定登记） |
| 重试语义 | 无。凭据一次性，SDK 不做 errcode 驱动的重试；外层也不要套通用重试策略 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `responseCode`（方法参数） | `response_code` | `string` | 是 | 一次性应答凭据，取自回调帧 `response_url` 的查询参数；每个 url 仅一次、有效期 1 小时；不在 `SensitiveUrlRedactor` 精确词表内，宿主日志不要打完整 URL |
| `message` | 请求体根 | `AibotMessage` | 是 | 可空超集，覆盖被动回复 / 主动回复 / 长连接三处报文；本端点只接受 `markdown` / `template_card` 两个分支 |
| `message.MsgType` | `msgtype` | `string?` | 是（选择分支） | 本端点取值 `markdown` 或 `template_card`；其他值见下方超集行 |
| `message.Markdown` | `markdown` | `AibotMarkdownBody?` | 是（`msgtype=markdown` 时） | markdown 消息体 |
| `message.Markdown.Content` | `content` | `string?` | 是（`msgtype=markdown` 时） | 消息内容，最长 20480 个字节，必须 UTF-8，支持常见 markdown 语法（DTO 注释） |
| `message.Markdown.Feedback` | `feedback` | `MessageFeedbackBody?` | 否 | 非空时该回复被用户反馈会触发 `feedback_event`；`id` 有效长度 ≤256 字节、UTF-8（`MessageFeedbackBody` 注释） |
| `message.TemplateCard` | `template_card` | `TemplateCardBody?` | 是（`msgtype=template_card` 时） | 复用 `Mud.Wechat.Work.DataModels.Message.TemplateCardBody`；可带 `feedback.id` 启用反馈；群聊引用行为见 §2.2 |
| `message.Text` / `Stream` / `File` / `Image` / `Voice` / `Video` | `text` / `stream` / `file` / `image` / `voice` / `video` | 各自分支体 | 否（本端点不支持） | 超集字段。`stream` 与媒体消息属回调被动回复 / 长连接能力，发到本端点不被支持 |
| `message.ResponseType` / `UserIds` | `response_type` / `userids` | `string?` / `List<string>?` | 否（本端点不支持） | `update_template_card` 属被动回复与长连接面；本端点支持面只到 `markdown` / `template_card` |

SDK 不做参数校验：`responseCode` 为空串、`message` 为 `null`、`MsgType` 写了 `stream`，SDK 都不会在本地拒绝，错误以官方 `errcode` 回来。`AibotMessage` 是可空超集，编译器也不会拦下本端点不支持的组合。

```json
{
  "msgtype": "markdown",
  "markdown": {
    "content": "**已收到**，正在处理。",
    "feedback": { "id": "feedback-demo" }
  }
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功；继承自 `WechatWorkResponse` |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| —（计算属性，不序列化） | `IsSuccess` | `bool` | `ErrorCode == 0`；标了 `[JsonIgnore]`，没有对应 JSON 字段 |

官方 101138 未列出响应字段（接口 XML），因此 `AibotReplyResponse` 只按统一响应基底承载 `errcode` / `errmsg`。返回类型是可空的 `Task<AibotReplyResponse?>`：HTTP 层可能没有响应体，此时结果为 `null`，既拿不到 `errcode`，也不能据此断言成功。

```json
{
  "errcode": 0,
  "errmsg": "ok"
}
```

上例是按统一响应基底构造的示意，不是官方页给出的响应示例。

**调用示例**

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work;                            // AddWechatWorkServices / IWechatWorkInternalAibotService
using Mud.Wechat.Work.Abstractions;               // AddWechatApp（扩展方法落此命名空间）
using Mud.Wechat.Work.Abstractions.Callback.Bots; // WechatBotReplyTypes
using Mud.Wechat.Work.Abstractions.Enums;         // WechatAppType
using Mud.Wechat.Work.DataModels.Aibot;           // AibotMessage / AibotMarkdownBody
using Mud.Wechat.Work.DataModels.Message;         // MessageFeedbackBody

// builder / config / provider / logger / cancellationToken 由宿主提供：
// builder 为 WebApplicationBuilder，config 为 IConfiguration，provider 为 IServiceProvider。
// 宿主为 ASP.NET Core 时，Microsoft.Extensions.Logging（LogWarning / LogError 扩展方法）由 Web SDK 隐式 using 提供。

// ① 本端点不用令牌，但 AddWechatWorkServices.Build() 要求 IWechatAppManager 已注册；
//    Internal 的 Validate() 仍强制 CorpId + AgentSecret，缺一即启动期抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "bot-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

builder.Services.AddWechatWorkServices(b => b.AddAibotApi());

var aibot = provider.GetRequiredService<IWechatWorkInternalAibotService>();

// ② response_url 来自机器人回调帧（回调接收在 Mud.Wechat.Work.Callback 包），本篇只消费、不签发。
//    下面用占位 URL 演示从查询串提取 response_code；不要在日志里打印 response_url 或 code。
var responseUrl = "https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=PLACEHOLDER";
var query = new Uri(responseUrl).Query;

string? responseCode = null;
foreach (var pair in query.TrimStart('?').Split('&'))
{
    var separator = pair.IndexOf('=');
    if (separator <= 0)
    {
        continue;
    }

    if (string.Equals(pair.Substring(0, separator), "response_code", StringComparison.Ordinal))
    {
        responseCode = Uri.UnescapeDataString(pair.Substring(separator + 1));
        break;
    }
}

if (string.IsNullOrEmpty(responseCode))
{
    logger.LogError("回调未携带可用的 response_code，无法主动回复");
    return;
}

// ③ 本端点支持面只有 markdown / template_card；stream 与媒体消息属长连接面。
var message = new AibotMessage
{
    MsgType = WechatBotReplyTypes.Markdown,
    Markdown = new AibotMarkdownBody
    {
        // 官方上限 20480 字节（UTF-8），超限须按字节自行截断，而不是按字符数。
        Content = "**已收到**，正在处理。",
        // 可选：非空时用户反馈该消息会触发 feedback_event。
        Feedback = new MessageFeedbackBody { Id = "feedback-demo" },
    },
};

var response = await aibot.ReplyAsync(responseCode, message, cancellationToken);

// ④ 官方未列响应字段，HTTP 层可能没有响应体 ⇒ 先判 null，再判 errcode。
//    一次性凭据已消耗，失败或 null 都不要重试。
if (response is null)
{
    logger.LogWarning("主动回复无响应体，无法判定 errcode；凭据已消耗，不要重试");
    return;
}

if (!response.IsSuccess)
{
    logger.LogError("主动回复失败（不可重试）：errcode={ErrorCode}, errmsg={ErrorMessage}",
        response.ErrorCode, response.ErrorMessage);
    return;
}
```

**本端点注意事项**

- `response_code` 是一次性 URL 凭据：每个 `response_url` 只可调用一次、有效期 1 小时。失败后没有补发通道，重试只会拿到失败；需要补偿时改走被动回复或长连接。不要在外层套 Polly 之类的重试策略。
- 不要把 `response_url` 当成参数值传入。本端点只接收查询串里的 `response_code`；把完整 URL 塞进去既不合契约，也会浪费唯一一次调用机会。
- 返回值可能为 `null`（空响应体）。`WechatWorkException.ThrowIfFailed(response)` 把 `null` 也判为失败（errcode `-1`），对本端点会把「无响应体」误报成业务失败，故示例采用显式判空。
- 支持面不会被类型系统保护：`AibotMessage` 是可空超集，`stream` / 媒体字段都能编译通过。发送前自行把 `MsgType` 限制在 `markdown` / `template_card`。接口注释与域 README 提到的 `WechatBotReplySupport` 目前只有被动回复与长连接两个校验入口，没有主动回复面方法（见 §7）。
- `response_code` 不在组件 `SensitiveUrlRedactor` 的精确词表内（接口 XML 已登记为已知接受风险）。SDK 的异常消息会剥离 query，但宿主自己打日志时仍不要把完整 URL 或 code 打出去。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `IWechatWorkAccountIdBotService`（`AccountId` 域） | `provider_access_token` 鉴权、`POST /cgi-bin/service/batch/userid_to_openuserid`、企业主体加密 userid → 服务商主体 `open_userid` | 把身份换算当回复通道：凭据族别与路由都不对，拿不到任何回复能力 |
| HTTP 被动回复（回调应答，官方 101031） | 处理器返回 `AibotMessage` 作为加密应答明文，支持 `text`（仅 `enter_chat`）/ `template_card` / `stream` / `stream_with_template_card`，不支持 `markdown`；受回调软超时（默认 4500ms）约束 | markdown 放被动回复会被 `WechatBotReplySupport.ValidateHttpPassiveReply` 拒绝；长耗时生成必然超时 |
| `Message` 域的 `response_code`（发送模板卡片返回） | 用于 `UpdateTemplateCardAsync` 更新卡片，走 `access_token`、路由在 `/cgi-bin/message/`，72 小时内有效且只能使用一次 | 两个同名凭据混用：本端点的 code 是 1 小时、用于回复；消息域的是 72 小时、用于更新卡片 |
| 长连接 `msgtype`（`stream` / `file` / `image` / `voice` / `video`） | 属官方 101463 长连接回复；媒体走 `media_id`，须先经 `aibot_upload_media_*` 上传（3 天有效） | `AibotMessage` 是可空超集，编译器不拦；发到本端点不被支持，一次性凭据已消耗 |
| `[Token]` / `access_token` | 本域父子接口都不声明 `[Token]`；G5 的 Query 令牌白名单与 TO1 的归属域枚举都不覆盖本域，子接口只出现在 TO1 的「无 `[Token]` 例外清单」里 | 补 `[Token]` 会让生成器注入本不该存在的令牌参数；HTTPCLIENT018 只能走目录级 `.editorconfig` 豁免，不能用 `[Token(Justification)]` 绕 |
| `response_url` 与 `response_code` | 前者是回调给出的完整 URL，后者只是其查询串里的一个值 | 把整个 URL 传进 `responseCode`：被当作非法查询值，且浪费唯一一次调用机会 |
| `WechatWorkException.ThrowIfFailed(response)` | 该出口把 `null` 响应也判为失败（errcode `-1`），而本端点官方未列响应字段、可能空响应体 | 把「无响应体」误判成失败并触发外层重试，重试必然失败（凭据已消耗） |
| `TokenManage` 与令牌链 | `TokenManage = nameof(IWechatAppManager)` 只配置应用切换管理器，与令牌注入无关；父子必须一致 | 父/子不一致会让生成器以 `new` 隐藏基类切换成员（HTTPCLIENT028），基类调用静默走默认模式 |

另外三条 SDK 侧行为约束，写进这里是因为它们会改变代码写法：

- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判。本端点没有令牌，也就没有令牌失效自愈链路可用。
- **没有令牌也要注册应用**：`AddWechatWorkServices(...).Build()` 要求 `IWechatAppManager` 已存在，否则启动期 `InvalidOperationException`；`Internal` 的 `Validate()` 还强制 `CorpId` + `AgentSecret`。
- **一次性凭据不可重放**：`response_code` 一次有效、1 小时过期。SDK 不做 errcode 驱动的重试，调用方也不应在外层加通用重试。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径；空响应体（`null`）不在此列，见 §4 注意事项 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少凭据参数 | 本端点不消费 `access_token`；出现即说明请求落到了别的能力线，回查调用点 |
| 40014 / 42001 / 42007 / 42009 / 42011 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` / `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 各令牌失效或过期 | 令牌失效判定器集合与本端点无关：本端点没有令牌链路，这组码不应由它产生；出现即按「凭据族别错配」排查，不要加令牌 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍为 200，响应照样反序列化成功。令牌失效自愈码集合（与 `WechatTokenInvalidationDetector` 同源）为 `{40014, 42001, 42007, 42009, 42011}`，但本端点不消费任何令牌，不要指望它触发刷新重试。

官方 101138 页是否列出本端点专属错误码、`response_code` 失效/过期/重复使用时返回什么，仓内没有证据，见 §7。其余码指向官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方 101138 页面字段表、权限说明、页面更新日期 | 本次未取官方页；正文必填性来自接口 XML 与 DTO 注释 ⇒ **未逐项核验** | 官方 101138（浏览器渲染后取文） |
| `response_type=update_template_card` / `userids` 是否可用于本端点 | 接口注释与域 README 只锁定 `markdown` / `template_card`，守卫未对这两个超集字段作断言；其在本端点的语义无仓内证据 ⇒ **未逐项核验**，本文不作支持断言 | 官方 101138 参数表 |
| 端点级频率上限 | 接口注释只给会话级 30 条/分钟、1000 条/小时，没有端点级 QPS 数字 ⇒ **未逐项核验** | 官方 101138、访问频率限制（90454） |
| `response_code` 失效 / 过期 / 重复使用的 errcode 与 HTTP 形态 | 仓内无断言，本端点专属错误码未登记 ⇒ **未逐项核验** | 官方 101138、全局错误码（90455）、实测 |
| 空响应体（`null`）的官方语义 | XML 明示「可能为 null（空响应体）」，但官方未列响应字段；`null` 是否代表成功无法判定 ⇒ **未逐项核验** | 官方 101138 响应示例、实测抓包 |
| 全局令牌失效恢复链路对无 `[Token]` 请求的行为 | 若 101138 返回 42001 类码，组件判定器是否拦截并重试（可能二次消耗一次性凭据）无仓内证据 ⇒ **未逐项核验** | `WechatTokenInvalidationDetector` 与组件恢复链路 |
| `WechatBotReplySupport` 的主动回复校验入口 | 接口注释与域 README 称「调用前请经 `WechatBotReplySupport` 校验形态」，但该类只有 `ValidateHttpPassiveReply` / `ValidateLongConnectionReply`，没有主动回复面方法（源码不一致）⇒ **未逐项核验 / 待补** | [WechatBotReplySupport.cs](../../../../../Src/Work/Mud.Wechat.Work/Callback/Bots/WechatBotReplySupport.cs)、接口 XML |
| `response_code` 的脱敏豁免追踪号 | 不在 `SensitiveUrlRedactor` 精确词表内，接口注释称已在方案文档登记为已知接受风险；`.docs/` 已 gitignore，fresh clone 无该文档，追踪号无法引用 ⇒ **未逐项核验** | `.docs/` 智能机器人方案 |
| `AibotReplyResponse` 是否遗漏官方返回字段 | 官方页未列响应字段，本类型只承载 `errcode` / `errmsg`；若官方另返字段会被静默忽略 ⇒ **未逐项核验** | 官方 101138、实测抓包 |
| 长连接里程碑注释 | `WechatWorkServiceBuilder.cs:268` 与 `WechatModule.cs:110` 仍写「长连接为可选后续里程碑」，与 Callback 包已存在 `LongConnection/`（AI4 注明 2026-10-11 P3）不一致（源码不一致） | 上述两文件与 `Mud.Wechat.Work.Callback/LongConnection/` |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（端点速览 + 契约陷阱汇总）：[Src/Work/Mud.Wechat.Work/Interfaces/Aibot/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Aibot/README.md)
- 邻域·身份换算（与本族正交）：[AccountId/Bot.md](../AccountId/Bot.md)
- 回调接收与被动回复（`response_url` 的签发方）：[Mud.Wechat.Work.Callback/README.md](../../../../../Src/Work/Mud.Wechat.Work.Callback/README.md) · [长连接目录](../../../../../Src/Work/Mud.Wechat.Work.Callback/LongConnection/)
- 仓内已有主动回复消费实现（AI 层，含按字节截断）：[WechatAibotReplySink.cs](../../../../../Src/AI/Mud.Wechat.Work.AI/Channels/WechatAibotReplySink.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（未成文）
- 官方页：[主动回复消息（101138）](https://developer.work.weixin.qq.com/document/path/101138) · [被动回复消息（101031）](https://developer.work.weixin.qq.com/document/path/101031) · [回调事件（101027）](https://developer.work.weixin.qq.com/document/path/101027) · [接收消息（100719）](https://developer.work.weixin.qq.com/document/path/100719) · [长连接（101463）](https://developer.work.weixin.qq.com/document/path/101463)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md) · 仓库协作约束：[AGENTS.md](../../../../../AGENTS.md)

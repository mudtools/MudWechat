# 发送应用消息 · 11 种 msgtype、卡片更新与撤回

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Message` 域 · 发送应用消息族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMessageService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkMessageService.cs)（父，承载 13 支公共端点，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkMessageService_Internal.cs)（**零端点空标记**）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkMessageService_ThirdParty.cs)（+1：`template_msg`）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkMessageService_Provider.cs)（**零端点空标记**） |
| DTO 源文件 | [DataModels/Message/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Message/)（命名空间全域 87 型平铺（MSG4 断言四族合计 86 型，差额 `MessageFeedbackBody` 属机器人应答面、见 §7）、`SerializerClassName = Message` → `MessageJsonContext`；本族相关：`MessageSendRequest` + 11 个 msgtype 子类 + `MessageSendTemplateMsgRequest`、`MessageTextBody`、`MessageMediaBody`、`MessageVideoBody`、`MessageTextCardBody`、`MessageNewsBody`/`MessageNewsArticle`、`MessageMpNewsBody`/`MessageMpNewsArticle`、`MessageMarkdownBody`、`MessageMiniProgramNoticeBody`/`MessageContentItem`、`TemplateCardBody` 及 18 个 `TemplateCard*` 嵌套型、`MessageSendResponse`、`UpdateTemplateCardRequest/Response`、`UpdateTemplateCardButton`、`RecallMessageRequest/Response`） |
| 契约守卫 | [WechatMessageContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)（MSG1：全域 37 声明 / 11 去重路由，本族 14 声明 / 3 路由 · MSG2 `MessageInterfaceHierarchy_ShouldConvergeOnMessageRegistry`：父 13 支 + 自建/代开发空标记 + **第三方恰持 1 支 `SendTemplateMsgAsync`** · MSG3 令牌绑定（12 接口）· MSG4 上下文登记：全域 86 型，本族计 51）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 发送 自建 [90236](https://developer.work.weixin.qq.com/document/path/90236) / 第三方 [90372](https://developer.work.weixin.qq.com/document/path/90372) / 代开发 [96458](https://developer.work.weixin.qq.com/document/path/96458) · 更新卡片 94888 / 94945 / 96459 · 撤回 94867 / 94947 / 96460 · 模板消息（第三方）[94515](https://developer.work.weixin.qq.com/document/path/94515) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（应用消息发送族：文本 → 模板卡片更新 → 撤回 → 第三方模板消息，只落 `invaliduser` 计数））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MSG1~MSG4 + G5 + TO1~TO3 断言 + 本族 4 份接口文件 XML 注释 + 51 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（第 1/2/3/4 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微信里最常用的**应用主动推送通道**——3 条路由 / 14 个方法（12 个去重路由里本族占 3 条；`message/send` 一条路由承载 11 个公共 msgtype + 1 个第三方专属 msgtype）：

| # | 方法 | 路由 | 开放面 |
|---|---|---|---|
| E1 | `SendTextMessageAsync` | `POST /cgi-bin/message/send` | 自建 · 第三方 · 代开发 |
| E2 | `SendImageMessageAsync` | 同上 | 同上 |
| E3 | `SendVoiceMessageAsync` | 同上 | 同上 |
| E4 | `SendVideoMessageAsync` | 同上 | 同上 |
| E5 | `SendFileMessageAsync` | 同上 | 同上 |
| E6 | `SendTextCardMessageAsync` | 同上 | 同上 |
| E7 | `SendNewsMessageAsync` | 同上 | 同上 |
| E8 | `SendMpNewsMessageAsync` | 同上 | 同上 |
| E9 | `SendMarkdownMessageAsync` | 同上 | 同上 |
| E10 | `SendMiniProgramNoticeMessageAsync` | 同上 | 同上 |
| E11 | `SendTemplateCardMessageAsync` | 同上 | 同上 |
| E12 | `UpdateTemplateCardAsync` | `POST /cgi-bin/message/update_template_card` | 同上 |
| E13 | `RecallMessageAsync` | `POST /cgi-bin/message/recall` | 同上 |
| E14 | `SendTemplateMsgAsync` | `POST /cgi-bin/message/send`（`template_msg`） | **仅第三方**（94515） |

**三类应用等宽**：官方对自建（90236/94888/94867）、第三方（90372/94945/94947）、代开发（96458/96459/96460）开放完全一致的 13 支，故收敛在父接口；自建与代开发子接口是**零端点空标记**，只有第三方子接口多出 `template_msg`（MSG2 锁定）。

与[群聊会话族](./AppChat.md)（`/cgi-bin/appchat/send`）的区别：本族面向**应用可见范围内的成员/部门/标签**，按接收人投递并返回 `msgid`（可撤回）；群聊族只往指定群推、无 `msgid`。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（E1~E13 三类等宽；E14 仅第三方） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到授权企业作用域——本族消费的是**授权企业级** `access_token`（scope = authCorpId），不是套件/服务商令牌。

### 2.2 权限与业务前置

- **接收人至少填一个**：`touser` / `toparty` / `totag` 不能同时为空；`touser` 最多 1000 个、`toparty` 与 `totag` 各最多 100 个，多个用 `|` 分隔；`touser="@all"` 时忽略 `toparty` / `totag`。
- **部分失败不阻断**：有接收人无权限或不存在时，发送仍执行并回传无效部分（`invaliduser` / `invalidparty` / `invalidtag` / `unlicenseduser`）；**全部接收人无权限或不存在则本次调用失败（errcode 81013）**。常见原因是接收人不在应用可见范围内。
- **限频两套**：每应用不可超过「账号上限数 × 200」人次/天（一次 API 发给 1000 人算 1000 人次）；对同一成员不可超过 30 次/分钟、1000 次/小时，**超过部分被丢弃不下发**。官方建议避开每小时 0 分 / 30 分的集中高峰。
- **`agentid` 必填**：公共信封的 `agentid` 为官方必填（小程序通知消息参数表未单列该字段，实际调用仍需填写）。
- **重复消息检查**：`enable_duplicate_check=1` + `duplicate_check_interval`（默认 1800 秒，最大 4 小时）可抑制同内容重复投递。
- **id 转译面有限**：`enable_id_trans=1` 支持 text / textcard / news / mpnews / miniprogram_notice / template_card 的部分字段；**image / voice / video / file / markdown 不支持**。
- **卡片版门槛**：文本通知型 / 图文展示型 / 按钮交互型需企业微信 3.1.6+（附件下载 3.1.12+）；投票选择型 / 多项选择型需 3.1.12+；`desc_color` / `horizontal_content_list` 的 type 3 / `action_menu` / `quote_area` / `image_text_area` / `button_selection` 需 3.1.18+；微工作台不支持展示模板卡片。
- **`response_code` 一次性**：仅「支持回调」的卡片返回（按钮交互型 / 投票选择型 / 多项选择型，以及带 `action_menu` 的文本通知型 / 图文展示型），**72 小时内有效且只能使用一次**；没有配置回调接口的应用不能发支持回调的卡片。
- **撤回窗口 24 小时**：只能撤发送应用消息产生的 `msgid`，且**只能撤企业微信端数据**，微信插件端不支持撤回。
- 接收人 userid、部门 ID、标签 ID 属**个人/组织数据**：示例一律假值，接收名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E13 | ✅ | ✅ | ✅ |
| E14 `template_msg` | ❌ | ✅（94515） | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册消息模块（四族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMessageApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Message` |
| 注册入口 | `WechatWorkServiceBuilder.AddMessageApi()`（→ 源生成 `AddMessageWebApiHttpClient()`） |
| 注册组名 | `"Message"`（四族共用；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMessageService` / `IWechatWorkThirdPartyMessageService` / `IWechatWorkProviderMessageService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMessageService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **公共信封**（`MessageSendRequest`，E1~E11 与 E14 共用；E12/E13 另持自己的请求体）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `touser` | `ToUser` | `string?` | 成员 ID 列表（竖线分隔），≤1000 个；`@all` 表示全部成员 |
| `toparty` | `ToParty` | `string?` | 部门 ID 列表（竖线分隔），≤100 个；`touser=@all` 时忽略 |
| `totag` | `ToTag` | `string?` | 标签 ID 列表（竖线分隔），≤100 个；`touser=@all` 时忽略 |
| `agentid` | `AgentId` | `int?` | **官方必填**，应用 id |
| `msgtype` | `MsgType` | `string?` | **官方必填**，由各子类对应固定取值——但**子类不自动赋值**，调用方须显式填（见 §5） |
| `enable_id_trans` | `EnableIdTrans` | `int?` | 0 否（默认）/ 1 是；image/voice/video/file/markdown 不支持 |
| `enable_duplicate_check` | `EnableDuplicateCheck` | `int?` | 0 否（默认）/ 1 是 |
| `duplicate_check_interval` | `DuplicateCheckInterval` | `int?` | 重复检查间隔，默认 1800 秒，最大 4 小时 |

> **`safe` 语义**（仅 text / image / video / file / mpnews 有）：`0` 可对外分享（默认）/ `1` 不能分享且内容显示水印 / **`2` 仅在企业内分享（只有 mpnews 支持）**。

### E1. `SendTextMessageAsync` — 发送文本消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendTextMessageAsync(
    [Body] MessageSendTextRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/message/send` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体 | `MessageSendTextRequest`：公共信封 + `text` + `safe` |

**请求体** `MessageTextBody`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `content` | `Content` | `string?` | **官方必填**，≤2048 字节（超出截断）；换行用转义 `\n`；支持 `<a href="...">` 打开自定义网页；支持 id 转译 |

**返回** `MessageSendResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `invaliduser` | `InvalidUser` | `string?` | 无效 userid，**竖线分隔字符串**（形如 `userid1` + 竖线 + `userid2`，统一转小写） |
| `invalidparty` | `InvalidParty` | `string?` | 无效 partyid，竖线分隔 |
| `invalidtag` | `InvalidTag` | `string?` | 无效 tagid，竖线分隔 |
| `unlicenseduser` | `UnlicensedUser` | `string?` | 无基础接口许可（含已过期）的 userid，竖线分隔 |
| `msgid` | `MsgId` | `string?` | **消息 id，撤回时用** |
| `response_code` | `ResponseCode` | `string?` | 仅支持回调的模板卡片返回；72 小时一次性 |

**调用示例**（发文本 → 发模板卡片 → 用 `response_code` 更新卡片 → 撤回文本）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddMessageApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Message;      // 请求 / 响应 DTO（全域平铺同一命名空间）

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册消息模块（四族同组）
builder.Services.AddWechatWorkServices(b => b.AddMessageApi());

// ③ 注入自建子接口（第三方 / 代开发用各自子接口，并先 UseCorpScope）
var messages = provider.GetRequiredService<IWechatWorkInternalMessageService>();

// ④ E1 发文本：接收人三选一不可全空；MsgType 必须显式赋值
var sent = await messages.SendTextMessageAsync(new MessageSendTextRequest
{
    ToUser = "demo_user_0001|demo_user_0002",   // 假值，勿用真人 userid
    AgentId = 1000002,
    MsgType = "text",
    Text = new MessageTextBody
    {
        Content = "构建 #42 失败。\n详情见 <a href=\"https://example.com/build/42\">流水线</a>",
    },
    Safe = 0,                                   // 0 可对外分享 / 1 水印
    EnableIdTrans = 0,
    EnableDuplicateCheck = 1,                   // 同内容 30 分钟内不重复下发
    DuplicateCheckInterval = 1800,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/message/send");
bool hasInvalidRecipient = !string.IsNullOrEmpty(sent.InvalidUser);
logger.LogInformation("消息已提交（有无效接收人：{HasInvalid}）", hasInvalidRecipient);

// ⑤ E11 发模板卡片：text_notice + action_menu ⇒ 支持回调、会返回 response_code
var cardSent = await messages.SendTemplateCardMessageAsync(new MessageSendTemplateCardRequest
{
    ToUser = "demo_user_0001",
    AgentId = 1000002,
    MsgType = "template_card",
    TemplateCard = new TemplateCardBody
    {
        CardType = "text_notice",
        TaskId = "demo_task_build_42",           // 同应用不可重复；≤128 字节
        MainTitle = new TemplateCardTitle { Title = "构建失败待处理", Desc = "集成测试阶段" },
        SubTitleText = "构建 #42 在 integration-test 阶段失败",
        HorizontalContentList = new List<TemplateCardHorizontalContent>
        {
            new() { KeyName = "环境", Value = "预发布" },
            new() { KeyName = "失败用例", Value = "3 个" },
        },
        ActionMenu = new TemplateCardActionMenu
        {
            Desc = "更多操作",
            ActionList = new List<TemplateCardActionItem>
            {
                new() { Text = "查看日志", Key = "view_log" },
            },
        },
        CardAction = new TemplateCardAction { Type = 1, Url = "https://example.com/build/42" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(cardSent, "/cgi-bin/message/send");
string? responseCode = cardSent.ResponseCode;    // 仅支持回调的卡片才返回

// ⑥ E12 更新卡片：一个 response_code 只能消费一次、72 小时内有效
if (!string.IsNullOrEmpty(responseCode))
{
    var updated = await messages.UpdateTemplateCardAsync(new UpdateTemplateCardRequest
    {
        UserIds = new List<string> { "demo_user_0001" },
        AgentId = 1000002,
        ResponseCode = responseCode,
        TemplateCard = new TemplateCardBody
        {
            CardType = "text_notice",
            TaskId = "demo_task_build_42",
            MainTitle = new TemplateCardTitle { Title = "已由值班同学接手" },
            SubTitleText = "该构建问题正在处理中",
            ReplaceText = "处理中",                // 按钮替换为灰色不可点击
            CardAction = new TemplateCardAction { Type = 1, Url = "https://example.com/build/42" },
        },
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/message/update_template_card");
}

// ⑦ E13 撤回：只认 24 小时内、本接口产生的 msgid，且仅企业微信端生效
if (sent.MsgId is { Length: > 0 } msgid)
{
    var recalled = await messages.RecallMessageAsync(new RecallMessageRequest { MsgId = msgid }, cancellationToken);
    WechatWorkException.ThrowIfFailed(recalled, "/cgi-bin/message/recall");
}
```

### E2. `SendImageMessageAsync` — 发送图片消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendImageMessageAsync(
    [Body] MessageSendImageRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="image"` + `image`（`MessageMediaBody{media_id}`，经上传临时素材获取，支持 JPG/PNG）+ `safe`。

### E3. `SendVoiceMessageAsync` — 发送语音消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendVoiceMessageAsync(
    [Body] MessageSendVoiceRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="voice"` + `voice`（`MessageMediaBody{media_id}`）。**无 `safe`**。

### E4. `SendVideoMessageAsync` — 发送视频消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendVideoMessageAsync(
    [Body] MessageSendVideoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="video"` + `video`（`MessageVideoBody{media_id, title, description}`）+ `safe`。`title` ≤128 字节、`description` ≤512 字节，超长截断。

### E5. `SendFileMessageAsync` — 发送文件消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendFileMessageAsync(
    [Body] MessageSendFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="file"` + `file`（`MessageMediaBody{media_id}`）+ `safe`。`safe=1` 时格式仅限 `txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif`。

### E6. `SendTextCardMessageAsync` — 发送文本卡片消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendTextCardMessageAsync(
    [Body] MessageSendTextCardRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="textcard"` + `textcard`（**无 `safe`**）。`MessageTextCardBody`：`title` **≤128 字符**（支持 id 转译）、`description` **≤512 字符**（支持 `<br>`/空格换行、`<div class="gray/highlight/normal">`）、`url` ≤2048 字节且须带协议头、`btntxt` 默认「详情」。

### E7. `SendNewsMessageAsync` — 发送图文消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendNewsMessageAsync(
    [Body] MessageSendNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="news"` + `news.articles`（**1~8 条**）。`MessageNewsArticle` 里 **`url` 与 `appid`+`pagepath` 必须填一个**（都填时按官方口径走小程序）；`title` ≤128 字节、`description` ≤512 字节、`picurl` ≤2048 字节。

### E8. `SendMpNewsMessageAsync` — 发送图文消息（mpnews）

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendMpNewsMessageAsync(
    [Body] MessageSendMpNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="mpnews"` + `mpnews.articles`（1~8 条）+ `safe`（**唯一支持取值 2** 的类型）。与 `news` 差异是图文内容存储在企业微信；`MessageMpNewsArticle.content` ≤666K 字节、`thumb_media_id` 必填。

### E9. `SendMarkdownMessageAsync` — 发送 markdown 消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendMarkdownMessageAsync(
    [Body] MessageSendMarkdownRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="markdown"` + `markdown.content`（≤2048 字节、utf8）。仅 markdown 子集（标题 / 加粗 / 链接 / 行内代码段 / 引用）；字体颜色仅 `info`（绿）/ `comment`（灰）/ `warning`（橙红）；微工作台不支持展示。

### E10. `SendMiniProgramNoticeMessageAsync` — 发送小程序通知消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendMiniProgramNoticeMessageAsync(
    [Body] MessageSendMiniProgramNoticeRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="miniprogram_notice"` + `miniprogram_notice`。`MessageMiniProgramNoticeBody`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `appid` | `AppId` | `string?` | **必填**，必须是与当前应用关联的小程序 |
| `page` | `Page` | `string?` | 点击后的小程序页面，≤1024 字节；不填则不跳转 |
| `title` | `Title` | `string?` | 消息标题，**4-12 个汉字**（支持 id 转译） |
| `description` | `Description` | `string?` | 消息描述，**4-12 个汉字**（支持 id 转译） |
| `emphasis_first_item` | `EmphasisFirstItem` | `bool?` | 是否放大第一个 `content_item` |
| `content_item` | `ContentItem` | `List<MessageContentItem>?` | 键值对，**最多 10 个**；`key` ≤10 汉字、`value` ≤30 汉字 |

**限制**：只允许绑定了小程序的应用发送；**不支持 `@all` 全员发送**；微工作台不支持展示。

### E11. `SendTemplateCardMessageAsync` — 发送模板卡片消息

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendTemplateCardMessageAsync(
    [Body] MessageSendTemplateCardRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `msgtype="template_card"` + `template_card`（`TemplateCardBody`）。

`card_type` 五种取值与各自必填面：

| `card_type` | 含义 | 必填要点 |
|---|---|---|
| `text_notice` | 文本通知型 | `main_title.title` 与 `sub_title_text` **不可都不填**；`card_action` 必填 |
| `news_notice` | 图文展示型 | `card_action` 必填；`image_text_area` 与 `card_image` **必填一个** |
| `button_interaction` | 按钮交互型 | `task_id` 必填；`button_list` ≤6 |
| `vote_interaction` | 投票选择型 | `task_id` 必填；`checkbox` + `submit_button` |
| `multiple_interaction` | 多项选择型 | `task_id` 必填；`select_list` **不可为空**、最多 3 个 |

`TemplateCardBody` 关键字段：`task_id`（同应用不可重复、≤128 字节、仅 `0-9a-zA-Z_-@`，按钮/投票/多项选择型及带 `action_menu` 的两种通知型必填）、`source`、`action_menu`（≤3 项）、`main_title`、`quote_area`、`emphasis_content`（仅文本通知型）、`sub_title_text`、`horizontal_content_list`（≤6）、`jump_list`（≤3）、`card_action`、`image_text_area` / `card_image`、`vertical_content_list`（≤4，仅图文展示型）、`button_selection` / `button_list`（仅按钮交互型）、`checkbox` / `submit_button`、`select_list`、`replace_text`（**仅更新接口支持**）、`feedback`（智能机器人应答上下文专用）。

**返回**：`MessageSendResponse`，支持回调的卡片额外带 `response_code`。

### E12. `UpdateTemplateCardAsync` — 更新模版卡片消息

**签名**

```csharp
[Post("/cgi-bin/message/update_template_card")]
Task<UpdateTemplateCardResponse> UpdateTemplateCardAsync(
    [Body] UpdateTemplateCardRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UpdateTemplateCardRequest`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userids` | `UserIds` | `List<string>?` | 成员 ID，≤1000 |
| `partyids` | `PartyIds` | `List<int>?` | 部门 ID，≤100 |
| `tagids` | `TagIds` | `List<int>?` | 标签 ID，≤100 |
| `atall` | `AtAll` | `int?` | 1 表示更新整个任务接收人员 |
| `agentid` | `AgentId` | `int?` | **官方必填** |
| `response_code` | `ResponseCode` | `string?` | **官方必填**；一个 code 只能调用一次、72 小时内有效 |
| `enable_id_trans` | `EnableIdTrans` | `int?` | 0 否（默认）/ 1 是 |
| `button` | `Button` | `UpdateTemplateCardButton?` | 模式一：按钮改为不可点击（`replace_name`）——**仅按钮交互型 / 投票选择型 / 多项选择型可更新按钮** |
| `template_card` | `TemplateCard` | `TemplateCardBody?` | 模式二：更新为一张新卡片（可回调卡片可更新成任一种模板卡片） |

**返回** `UpdateTemplateCardResponse.invaliduser` 是 **`List<string>`**——与 E1~E11 响应的**竖线分隔字符串**形态不同。

### E13. `RecallMessageAsync` — 撤回应用消息

**签名**

```csharp
[Post("/cgi-bin/message/recall")]
Task<RecallMessageResponse> RecallMessageAsync(
    [Body] RecallMessageRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`msgid`（**官方必填**，来自 E1~E11/E14 的 `MessageSendResponse.msgid`）。撤回窗口 **24 小时**，仅企业微信端数据；**群聊族没有 `msgid`，无法撤回**。返回 `RecallMessageResponse` 仅 `errcode` / `errmsg`。

### E14. `SendTemplateMsgAsync` — 发送模板消息（仅第三方应用）

**签名**

```csharp
[Post("/cgi-bin/message/send")]
Task<MessageSendResponse> SendTemplateMsgAsync(
    [Body] MessageSendTemplateMsgRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `MessageSendTemplateMsgRequest`：公共信封 + `selected_ticket_list` / `only_unauth` + `template_msg`。

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `selected_ticket_list` | `SelectedTicketList` | `List<string>?` | 选人 sdk / 选人 jsapi 返回的 ticket，**≤10 个**；接收者不含操作者本身 |
| `only_unauth` | `OnlyUnauth` | `bool?` | 仅向 ticket 中**未授权**用户发送；为 `true` 时自动忽略 `touser` / `toparty` / `totag` |
| `template_msg` | `TemplateMsg` | `MessageTemplateMsgBody?` | **官方必填** |

`MessageTemplateMsgBody`：`template_id`（**≤64 字节**，服务商管理端创建、正式授权需审批通过）、`url`（≤2048 字节、须带协议头）、`miniprogram`（`appid` + `pagepath`）、`content_item`（**1~5 个**，`key` 1~20 utf8 字符、`value` 1~40 utf8 字符，须与模板匹配）。**`url` 与 `miniprogram` 至少填一个，都填时优先 `miniprogram`**。

**官方页面未单独标注路由**：94515 正文说明模板消息「经发送应用消息端点」发送，故本方法路由取 `/cgi-bin/message/send`（接口注释与 MSG1 均已留档该口径）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`msgtype` 无默认值** | 11 个 `MessageSend*Request` 子类**不在构造期填 `MsgType`** | 忘记赋值 → 官方按缺参报错 |
| **14 声明 3 路由** | `message/send` 承载 E1~E11 + E14；E12/E13 各一条（MSG1 全域 37 声明 / 11 去重路由） | 按路由统计端点 → 误判缺口 |
| `invaliduser` 两种形态 | 发送/更新响应：**竖线分隔字符串**；更新卡片响应：**字符串数组**（MSG4 类型断言与 DTO 注释双处留档） | 统一按数组解析 → 线上取不到无效名单 |
| `safe` 不是全局字段 | 只有 text / image / video / file / mpnews 有；voice / textcard / news / markdown / miniprogram_notice / template_card / template_msg 无 | 在无该字段的类型上赋值 → 编译期报错 |
| `safe=2` 只属 mpnews | `2` = 仅在企业内分享 | 给 text 传 `safe=2` → 语义越界 |
| `enable_id_trans` 支持面有限 | image / voice / video / file / markdown **不支持** | 以为图片也转译 → 无效果 |
| **部分失败仍成功** | 无效接收人只回传、不抛错；**全部**无效才 81013 | 只看 `errcode=0` 就认为全员收到 |
| `agentid` 处处必填 | 小程序通知参数表没单列，但实际仍需 | 漏填 → 失败 |
| `response_code` 一次性 72 小时 | 仅支持回调的卡片返回；一个 code 只能更新一次 | 复用 code → 第二次失败；超时 → 失效 |
| 更新卡片有类型门槛 | `button` 模式仅按钮交互型 / 投票选择型 / 多项选择型；`template_card` 模式对可回调卡片开放 | 对纯文本卡片更新按钮 → 失败 |
| 撤回只认本族 `msgid` | 24 小时窗口、仅企业微信端；群聊族无 `msgid` | 用群消息/过期消息撤回 → 失败 |
| `template_msg` 仅第三方 | 自建与代开发子接口是零端点空标记；官方页未单列路由 | 给自建补该方法 → 打坏 MSG2 |
| 第三方/代开发的令牌是**企业级** | 必须先 `UseCorpScope(appKey, authCorpId, permanentCode)` | 直接调用 → 取错令牌/无权限 |
| 重复检查是服务端行为 | `duplicate_check_interval` 默认 1800 秒、最大 4 小时；超限部分**丢弃不下发** | 当作本地去重 |
| 多个消息类型共用一条路由 | `msgtype` 决定服务端解析分支，字段错配会报参数错误 | 把 `text` 体传给 `news` 调用 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不补 `msgtype`**、**不校验接收人规模与可见范围**、**不管理 `response_code` 生命周期**、**不代传素材**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 仍要检查 `invaliduser` / `invalidparty` / `invalidtag` / `unlicenseduser` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |
| 81013 | 接口注释原文 | **全部**接收人无权限或不存在 | 检查接收人是否在应用可见范围 / 是否分批 |

其余业务码（模板不匹配、卡片版本不满足、`response_code` 失效、撤回超窗等）**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 除 81013 外，本族接口注释**未逐条摘录**业务码 ⇒ **未逐项核验** | 官方 90236 / 90372 / 96458 / 94888 / 94867 / 94515 |
| 接口级 QPS | 注释只有「账号上限 × 200 人次/天」「单成员 30 次/分 / 1000 次/小时」与会话级建议，无接口 QPS 数值 | 官方频率附录 90312 |
| `msgtype` 是否服务端兜底 | 仓内事实是「不填即不输出」；官方是否按其它线索推断未核验 | 官方 90236 参数表 |
| `template_msg` 的路由出处 | 94515 页面未单独标注路由，本仓按正文表述取 `message/send` | 官方 94515 正文 |
| `response_code` 的卡片类型全集 | 注释列出「按钮/投票/多项选择 + 带 action_menu 的两种通知型」，是否还有其它场景未核验 | 官方 94888 |
| `enable_id_trans` 的字段级支持面 | 只知整类型「不支持」，具体哪些字段可转译未逐项整理 | 官方各 msgtype 参数表 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |
| DTO 计数 87 vs 守卫 86 | 命名空间 `...DataModels.Message` 实测 **87 型 / 87 文件**，而 MSG4 以固定名单断言 **86 型**（发送应用消息 51 + 群聊会话 18 + 学校通知 11 + 智能表格 6）。差额为 `MessageFeedbackBody`（`{"id":…}`，用于机器人应答的 `feedback` 节点，已登记进 `MessageJsonContext`，但不属本域四族端点面）⇒ 守卫是「四族端点面」计数，不是命名空间全量计数；**勿**按本文的 87 去改 MSG4 | 守卫 [WechatMessageContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs) MSG4 与本域 DTO 目录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（37 支全表、四族层级、令牌归属表）
- 同域近邻：[AppChat.md](./AppChat.md)（群聊会话，仅自建、无 `msgid`）· [SchoolMessage.md](./SchoolMessage.md)（学校通知，`recv_scope` 语义）· [SmartSheetGroupChat.md](./SmartSheetGroupChat.md)（智能表格自动化群）
- 跨域边界：`media_id` 上传见 [Media.md](../Media/Media.md)；卡片回调事件与智能机器人 `feedback_event` 见 [Aibot](../Aibot/) 与 `Work.Callback` 包
- 官方页：[发送应用消息（自建 90236）](https://developer.work.weixin.qq.com/document/path/90236) · [更新模版卡片（94888）](https://developer.work.weixin.qq.com/document/path/94888) · [撤回（94867）](https://developer.work.weixin.qq.com/document/path/94867) · [模板消息（94515）](https://developer.work.weixin.qq.com/document/path/94515)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)

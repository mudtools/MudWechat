# 群聊会话 · 建群、改群与应用推送 9 种消息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Message` 域 · 群聊会话族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkAppChatService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkAppChatService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkAppChatService_Internal.cs)（承载全部 12 支） |
| DTO 源文件 | [DataModels/Message/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Message/)（命名空间全域 87 型平铺（MSG4 断言四族合计 86 型，差额 `MessageFeedbackBody` 属机器人应答面、见 [Message.md](./Message.md) §7）、`SerializerClassName = Message` → `MessageJsonContext`；本族相关：`CreateAppChatRequest/Response`、`UpdateAppChatRequest/Response`、`GetAppChatResponse`、`AppChatInfo`、`AppChatSendRequest` + 9 个 msgtype 子类、`AppChatTextBody`、`MessageMediaBody`、`MessageVideoBody`、`MessageTextCardBody`、`MessageNewsBody`/`MessageNewsArticle`、`MessageMpNewsBody`/`MessageMpNewsArticle`、`MessageMarkdownBody`、`AppChatSendResponse`） |
| 契约守卫 | [WechatMessageContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)（MSG1 `MessageEndpoints_ShouldMatchOfficialRoutes`：全域 37 个方法声明 / **11 条去重路由**，本族 12 声明 / 4 路由 · MSG2 `AssertInternalOnlyFamily`：父零端点 + 仅自建子接口，继承链不得出现第三方/代开发 · MSG3 令牌绑定：12 接口均 `AccessToken` + Query + `access_token` · MSG4 上下文登记：全域 86 型，本族计 18）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 [90245](https://developer.work.weixin.qq.com/document/path/90245) · 修改 [98913](https://developer.work.weixin.qq.com/document/path/98913) · 获取 [98914](https://developer.work.weixin.qq.com/document/path/98914) · 推送 [90248](https://developer.work.weixin.qq.com/document/path/90248) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（建群 → 改群 → 查群 → 群内文本推送；其余消息类型同形仅 `msgtype` 与 payload 不同））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MSG1~MSG4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 18 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（第 1/2/3/4 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

应用**自己建群、自己改群、往自己的群里推消息**——4 条路由 / 12 个方法（`appchat/send` 一条路由承载 9 种 `msgtype`，一型一个方法）：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `CreateAppChatAsync` | `POST /cgi-bin/appchat/create` | 创建群聊会话（可指定 chatid，不填则系统随机生成） |
| E2 | `UpdateAppChatAsync` | `POST /cgi-bin/appchat/update` | 改名 / 换群主 / 加人 / 踢人 |
| E3 | `GetAppChatAsync` | `GET /cgi-bin/appchat/get` | 查群名、群主、成员列表与群聊类型（**本族唯一 GET**） |
| E4 | `SendAppChatTextMessageAsync` | `POST /cgi-bin/appchat/send` | `msgtype=text` |
| E5 | `SendAppChatImageMessageAsync` | 同上 | `msgtype=image` |
| E6 | `SendAppChatVoiceMessageAsync` | 同上 | `msgtype=voice` |
| E7 | `SendAppChatVideoMessageAsync` | 同上 | `msgtype=video` |
| E8 | `SendAppChatFileMessageAsync` | 同上 | `msgtype=file` |
| E9 | `SendAppChatTextCardMessageAsync` | 同上 | `msgtype=textcard` |
| E10 | `SendAppChatNewsMessageAsync` | 同上 | `msgtype=news` |
| E11 | `SendAppChatMpNewsMessageAsync` | 同上 | `msgtype=mpnews` |
| E12 | `SendAppChatMarkdownMessageAsync` | 同上 | `msgtype=markdown` |

**只对自建应用开放**：第三方应用官方明示不可调用、服务商代开发无对应文档 ⇒ 父接口零端点，12 支全在 `IWechatWorkInternalAppChatService`（MSG2 的 `AssertInternalOnlyFamily`）。与[发送应用消息族](./Message.md)（`/cgi-bin/message/send`，三类应用等宽）是**两套通道**：本族只往群推、受众是群成员，不认 `touser`/`toparty`/`totag`。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **可见范围必须是根部门**：调用本族端点的自建应用，其可见范围必须为根部门（官方权限口径原文，见接口注释）。
- **群归属**：E2 改群、E3 查群、E4~E12 推消息都要求 `chatid` 所代表的群**由该应用通过 `appchat/create` 创建**；建群后没下发过消息的群，在旧版本终端上可能不出现。
- **建群参数闸**：`userlist` 至少 2 人、至多 2000 人（含应用），且不可超过管理端配置的「群成员人数上限」；`chatid` 最长 32 字符、仅 `0-9` 与 `a-zA-Z`、不得与已有群重复；`name` 最多 50 个 utf8 字符（超长截断）。
- **限频三套**：建群 ≤1000 个/天；改群 ≤1000 次/小时；推送每企业 ≤2 万人次/分（群 100 人时每发一次算 100 人次）＋ 按企业规模分档的小时额度（未认证或小型 ≤15 万、中型 ≤35 万、大型 ≤70 万人次/小时）。
- **投递不保证**：每个成员在群里收到的同一应用消息不可超过 200 条/分、1 万条/天，**超过部分被丢弃且接口不报错**——推送成功 ≠ 全员送达。
- **媒体消息先上传**：`image` / `voice` / `video` / `file` / `mpnews.articles[].thumb_media_id` 都要 `media_id`，素材上传走 Media 域（上传临时素材接口），本族不代传。
- **文本与视频上限**：`text.content` ≤2048 字节（支持转义 `\n`、`<@userid>` 扩展语法需企业微信 5.0.6+）；`video.title` ≤128 字节、`video.description` ≤512 字节（超长自动截断）。
- **保密消息格式白名单**：`safe=1` 时文件仅支持 `txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif`。
- **群成员 userid 属个人数据**：示例一律假值，群成员名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E12 | ✅ | ❌（官方明示不可调用） | ❌（无文档） |

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
| 注册组名 | `"Message"`（四族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalAppChatService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkAppChatService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`chatid` / `userlist` / 消息体都标「官方必填」但类型仍是 `string?` / `List<string>?` ⇒ 缺字段不会编译报错。SDK 也不做本地必填校验，官方返回参数错误。
>
> **`msgtype` 没有默认值**：`AppChatSendRequest.MsgType` 是普通可写属性，9 个 msgtype 子类**未在构造期固定取值** ⇒ 调用方必须显式赋 `MsgType`（见 §5 第一条）。

### E1. `CreateAppChatAsync` — 创建群聊会话

**签名**

```csharp
[Post("/cgi-bin/appchat/create")]
Task<CreateAppChatResponse> CreateAppChatAsync(
    [Body] CreateAppChatRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/appchat/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/appchat/create?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 仅自建 |
| 请求体 | `CreateAppChatRequest` |

**请求体** `CreateAppChatRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `name` | `Name` | `string?` | 否 | 群名，最多 50 个 utf8 字符，超出截断 |
| `owner` | `Owner` | `string?` | 否 | 群主 userid；不指定则由系统从 `userlist` 随机选一人 |
| `userlist` | `UserList` | `List<string>?` | **是** | 群成员 userid，**至少 2 人、至多 2000 人**（含应用），且不超过管理端群成员上限 |
| `chatid` | `ChatId` | `string?` | 否 | 群唯一标志；不得与已有群重复；**≤32 字符且仅 `0-9`/`a-zA-Z`**；不填则随机生成 |

**返回** `CreateAppChatResponse`：`chatid`（`ChatId`，`string?`）——后续 E2/E3 与 E4~E12 都用它定位。

**调用示例**（建群 → 查群 → 改名 → 推文本 → 推图文）

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

// ③ 注入自建子接口（本族只有自建一种应用形态）
var appChat = provider.GetRequiredService<IWechatWorkInternalAppChatService>();

// ④ E1 建群：userlist 至少 2 人；chatid 只允许 0-9 / a-zA-Z 且 ≤32 字符
var created = await appChat.CreateAppChatAsync(new CreateAppChatRequest
{
    Name = "构建告警群",                       // ≤50 utf8 字符
    Owner = "demo_user_0001",                  // 假值，勿用真人 userid
    UserList = new List<string> { "demo_user_0001", "demo_user_0002" },
    ChatId = "demo_build_alert_202610",        // 不填则系统随机生成
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/appchat/create");
string chatid = created.ChatId!;

// ⑤ E3 查群：GET + Query chatid；返回 chat_info（chatid/name/owner/userlist/chat_type）
var info = await appChat.GetAppChatAsync(chatid, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/appchat/get");
logger.LogInformation("群类型 {ChatType}，成员 {MemberCount} 人",
    info.ChatInfo?.ChatType, info.ChatInfo?.UserList?.Count ?? 0);

// ⑥ E2 改群：只改名，不动成员（add_user_list / del_user_list 不填即不改）
var renamed = await appChat.UpdateAppChatAsync(new UpdateAppChatRequest
{
    ChatId = chatid,
    Name = "构建告警群（生产）",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(renamed, "/cgi-bin/appchat/update");

// ⑦ E4 推文本：MsgType 必须显式赋值；@all 走 mentioned_list
var textSent = await appChat.SendAppChatTextMessageAsync(new AppChatSendTextRequest
{
    ChatId = chatid,
    MsgType = "text",
    Text = new AppChatTextBody
    {
        Content = "构建 #42 失败，请值班同学跟进。\n失败阶段：integration-test",
        MentionedList = new List<string> { "@all" },
    },
    Safe = 0,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(textSent, "/cgi-bin/appchat/send");

// ⑧ E10 推图文：1~8 条；mpnews 变体同样受 1~8 限制且多支持 safe=2
var newsSent = await appChat.SendAppChatNewsMessageAsync(new AppChatSendNewsRequest
{
    ChatId = chatid,
    MsgType = "news",
    News = new MessageNewsBody
    {
        Articles = new List<MessageNewsArticle>
        {
            new()
            {
                Title = "构建 #42 失败",
                Description = "integration-test 阶段 3 个用例失败",
                Url = "https://example.com/build/42",
                PicUrl = "https://example.com/assets/build-failed.png",
            },
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(newsSent, "/cgi-bin/appchat/send");
```

### E2. `UpdateAppChatAsync` — 修改群聊会话

**签名**

```csharp
[Post("/cgi-bin/appchat/update")]
Task<UpdateAppChatResponse> UpdateAppChatAsync(
    [Body] UpdateAppChatRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UpdateAppChatRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chatid` | `ChatId` | `string?` | **是** | 目标群；必须是本应用创建的群 |
| `name` | `Name` | `string?` | 否 | 新群名；≤50 utf8 字符 |
| `owner` | `Owner` | `string?` | 条件 | 新群主；**`del_user_list` 含现任群主时必填**；课程群聊群主须有课程群创建权限 |
| `add_user_list` | `AddUserList` | `List<string>?` | 否 | 新增成员 userid |
| `del_user_list` | `DelUserList` | `List<string>?` | 否 | 踢出成员 userid |

**返回** `UpdateAppChatResponse`：仅 `errcode` / `errmsg`（继承 `WechatWorkResponse`，无业务字段）。

> 未赋值的字段不会输出；官方口径是「不传即不改」。每企业改群 ≤1000 次/小时。

### E3. `GetAppChatAsync` — 获取群聊会话（本族唯一 GET）

**签名**

```csharp
[Get("/cgi-bin/appchat/get")]
Task<GetAppChatResponse> GetAppChatAsync(
    [Query("chatid")] string chatId,
    CancellationToken cancellationToken = default);
```

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `chatId` | Query `chatid` | `string` | 群聊 id（建群时指定或系统随机生成的 `chatid`） |

**返回** `GetAppChatResponse` → `chat_info`（`AppChatInfo`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `chatid` | `ChatId` | `string?` | 群唯一标志 |
| `name` | `Name` | `string?` | 群名 |
| `owner` | `Owner` | `string?` | 群主 userid |
| `userlist` | `UserList` | `List<string>?` | 群成员 userid 列表 |
| `chat_type` | `ChatType` | `int?` | **0 普通群聊 / 1 课程群聊** |

### E4. `SendAppChatTextMessageAsync` — 应用推送文本消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatTextMessageAsync(
    [Body] AppChatSendTextRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `AppChatSendTextRequest`：`chatid` + `msgtype="text"` + `text` + `safe`。

`AppChatTextBody`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `content` | `Content` | `string?` | **官方必填**，≤2048 字节；转义 `\n` 换行；`<@userid>` 扩展语法需企业微信 5.0.6+ |
| `mentioned_list` | `MentionedList` | `List<string>?` | 提醒成员 userid；特殊值 `@all` 提醒所有人 |

`safe`：`0` 否（默认）/ `1` 是（保密消息，内容显示水印）。

**返回** `AppChatSendResponse`：仅 `errcode` / `errmsg`。**没有 `msgid`** ⇒ 群消息不能撤回（撤回接口只认发送应用消息的 `msgid`）。

### E5. `SendAppChatImageMessageAsync` — 应用推送图片消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatImageMessageAsync(
    [Body] AppChatSendImageRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="image"` + `image`（`MessageMediaBody{media_id}`，经上传临时素材获取，支持 JPG/PNG）+ `safe`。**返回**同 E4。

### E6. `SendAppChatVoiceMessageAsync` — 应用推送语音消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatVoiceMessageAsync(
    [Body] AppChatSendVoiceRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="voice"` + `voice`（`MessageMediaBody{media_id}`）。语音请求体**没有 `safe`**。

### E7. `SendAppChatVideoMessageAsync` — 应用推送视频消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatVideoMessageAsync(
    [Body] AppChatSendVideoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="video"` + `video`（`MessageVideoBody{media_id, title, description}`）+ `safe`。`title` ≤128 字节、`description` ≤512 字节，超长自动截断。

### E8. `SendAppChatFileMessageAsync` — 应用推送文件消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatFileMessageAsync(
    [Body] AppChatSendFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="file"` + `file`（`MessageMediaBody{media_id}`）+ `safe`。`safe=1` 时格式仅限 `txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif`。

### E9. `SendAppChatTextCardMessageAsync` — 应用推送文本卡片消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatTextCardMessageAsync(
    [Body] AppChatSendTextCardRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="textcard"` + `textcard`（**无 `safe`**）。`MessageTextCardBody`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `title` | `Title` | `string?` | **必填**，≤128 字符 |
| `description` | `Description` | `string?` | **必填**，≤512 字符；支持 `<br>` 或空格换行；`<div class="gray/highlight/normal">` 三种文字颜色 |
| `url` | `Url` | `string?` | **必填**跳转链接，≤2048 字节，须带协议头 |
| `btntxt` | `BtnTxt` | `string?` | 按钮文字，默认「详情」，≤4 字 |

### E10. `SendAppChatNewsMessageAsync` — 应用推送图文消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatNewsMessageAsync(
    [Body] AppChatSendNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="news"` + `news.articles`（**1~8 条**，无 `safe`）。`MessageNewsArticle`：`title` ≤128 字节、`description` ≤512 字节、`url` ≤2048 字节（`appid`/`pagepath` 仅发送应用消息族支持，本族不使用）、`picurl` ≤2048 字节。

### E11. `SendAppChatMpNewsMessageAsync` — 应用推送 mpnews 图文消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatMpNewsMessageAsync(
    [Body] AppChatSendMpNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="mpnews"` + `mpnews.articles`（1~8 条）+ `safe`。与 `news` 唯一差异是图文内容**存储在企业微信**；多次发送被视为不同图文，阅读/点赞分开统计。

### E12. `SendAppChatMarkdownMessageAsync` — 应用推送 markdown 消息

**签名**

```csharp
[Post("/cgi-bin/appchat/send")]
Task<AppChatSendResponse> SendAppChatMarkdownMessageAsync(
    [Body] AppChatSendMarkdownRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`chatid` + `msgtype="markdown"` + `markdown.content`（≤2048 字节、utf8；仅 markdown 子集；支持 `<@userid>` 需 5.0.6+）。**微信插件（原企业号）不支持展示 markdown 消息**（本族注释措辞；发送应用消息族对应页写作「微工作台」，见 §7）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`msgtype` 无默认值** | 9 个 `AppChatSend*Request` 子类**不在构造期填 `MsgType`**，JSON 里 `msgtype` 直接取属性值 | 忘记赋值 → 官方按缺参报错，消息发不出 |
| **12 方法 4 路由** | `appchat/send` 一条路由 9 个方法，其余各 1 条（MSG1 全域 37 声明 / 11 去重路由） | 按方法数统计路由 → 误判契约漂移 |
| **E3 是 GET** | 本族唯一 GET，`chatid` 走 Query，**没有请求体** | 按 POST + Body 调用 → 参数落到包体，官方取不到 |
| `chatid` 是本族主键 | 建群可自带（≤32 字符 `[0-9a-zA-Z]`），否则拿返回值 | 自造带 `-`/`_` 的 chatid → 建群失败 |
| 群必须本应用创建 | E2/E3/E4~E12 都只认 `appchat/create` 建的群 | 拿别处得来的 chatid 推消息 → 无权限 |
| `safe` 不是全 msgtype 都有 | 只有 text / image / video / file / mpnews 有；voice / textcard / news / markdown 无 | 给 textcard 设 `safe` → 字段被忽略（本仓 DTO 也没有该属性，编译期即报错） |
| **两套 `safe` 语义不同** | 本族：`0` 否 / `1` 是；发送应用消息族：`0` 可对外分享 / `1` 不能分享且水印 / **`2` 仅企业内（仅 mpnews）** | 跨族抄取值 → 语义错位 |
| 保密消息有格式白名单 | `safe=1` 仅 `txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif` | 传 docm/zip → 发送失败 |
| **群消息没有 `msgid`** | `AppChatSendResponse` 只有 `errcode`/`errmsg`，**不能撤回** | 想用 `RecallMessageAsync` 撤群消息 → 无 ID 可用 |
| 建群与改群是两套额度 | 建群 1000 个/天；改群 1000 次/小时；推送 2 万人次/分 + 企业分档小时额度 | 拿建群额度当推送额度评估容量 |
| 推送成功 ≠ 送达 | 单成员 200 条/分、1 万条/天，超出**丢弃且不报错** | 以 HTTP 200 判定全员收到 |
| 媒体要先上传 | `media_id` 来自上传临时素材（Media 域），本族不代传 | 传本地路径 / base64 → 官方不认 |
| `mentioned_list` 只在 text | 只有 `AppChatTextBody` 有该字段；markdown 用 `<@userid>` 内联语法 | 在 news 里找 `mentioned_list` → 不存在 |
| 图文条数固定区间 | `news` / `mpnews` 的 `articles` **1~8 条** | 传 0 条或 9 条 → 失败 |
| 群类型是数值 | `chat_type`：0 普通 / 1 课程；课程群改群主另有权限要求 | 按字符串 `"course"` 比较 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不补 `msgtype`**、**不校验群归属与人数上限**、**不代传素材**、**不做送达确认**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 推送场景仍需自行确认业务侧是否收到（丢弃不报错） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

建群人数越界、chatid 重复、非本应用创建的群、可见范围非根部门、频控超限等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族接口注释**未逐条摘录**业务码 ⇒ **未逐项核验** | 官方 90245 / 98913 / 98914 / 90248 + 全局错误码页 |
| 推送的 QPS / 日配额完整口径 | 注释给了 2 万人次/分与企业分档小时额度，但无接口级 QPS 数值 ⇒ 未逐项核验 | 官方 90248 与频率附录 90312 |
| 「微信插件 / 微工作台」措辞 | 本族 markdown 注释写「微信插件（原企业号）」，发送应用消息族写「微工作台（原企业号）」；同一能力两页措辞不一致，是否官方新旧叫法未核验 | 官方 90248 与 90236 |
| `msgtype` 的官方容错 | 本仓 DTO 事实是「不填即不输出」；官方是否按方法名/默认值兜底**未核验** | 官方 90248 参数表 |
| 群成员上限的客户端配置面 | 注释只提「不可超过管理端配置的群成员人数上限」，具体读取接口不在本域 | 官方管理端/群聊配置文档 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（37 支全表、四族层级、令牌归属表）
- 同域近邻：[Message.md](./Message.md)（发送应用消息族，三类应用等宽 + `template_msg`）· [SchoolMessage.md](./SchoolMessage.md)（学校通知 8 种 msgtype）· [SmartSheetGroupChat.md](./SmartSheetGroupChat.md)（智能表格自动化创建的群）
- 跨域边界：`media_id` 上传见 [Media.md](../Media/Media.md)；撤回只作用于发送应用消息的 `msgid`，见 [Message.md](./Message.md#e13-recallmessageasync--撤回应用消息)
- 官方页：[创建群聊会话（90245）](https://developer.work.weixin.qq.com/document/path/90245) · [修改（98913）](https://developer.work.weixin.qq.com/document/path/98913) · [获取（98914）](https://developer.work.weixin.qq.com/document/path/98914) · [推送（90248）](https://developer.work.weixin.qq.com/document/path/90248)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)

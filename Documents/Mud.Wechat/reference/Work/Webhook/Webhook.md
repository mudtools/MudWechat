# 群机器人 Webhook 推送 · 8 种消息类型 + 媒体上传

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Webhook` 域 · 群机器人族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWebhookService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Webhook/IWechatWorkWebhookService.cs)（父，零端点、`IsAbstract = true` 不可注入）＋ [IWechatWorkInternalWebhookService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Webhook/IWechatWorkInternalWebhookService.cs)（唯一子接口，承载全部 9 支） |
| DTO 源文件 | [DataModels/Webhook/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Webhook/)（本域 19 型：9 个 `RequestModel/` 请求 + 10 个消息体/响应，`SerializerClassName = Webhook` → `WebhookJsonContext`） |
| 契约守卫 | [WechatWebhookContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWebhookContractGuards.cs)（WEB1 9 路由 / WEB2 父零端点 + 唯一自建子接口 / WEB3 无 `[Token]` + `key` 脱敏闭环 / WEB4 19 型登记）＋ `WechatTokenOwnerContractGuards` TO1（无令牌例外清单）＋ N1~N3 |
| 官方文档 | [群机器人·消息推送配置说明 91770](https://developer.work.weixin.qq.com/document/path/91770) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录的逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WEB1~WEB4 + TO1 断言 + 9 支端点 XML 注释 + 19 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Webhook/README.md) · 官方：本批未重新取文 |

## 1. 这个接口族做什么

群里的**机器人推送通道**：群管理员在群聊设置里添加「群机器人」后拿到一条 Webhook 地址，地址里的 `key` 就是这台机器人的唯一凭据。本域 9 支端点分两类：

| # | 方法 | 路由 | `msgtype` / 用途 |
|---|---|---|---|
| E1 | `SendTextAsync` | `POST /cgi-bin/webhook/send` | `text` 文本（≤2048 字节，可 `@` 成员） |
| E2 | `SendMarkdownAsync` | 同上 | `markdown`（官方子集语法，≤4096 字节） |
| E3 | `SendMarkdownV2Async` | 同上 | `markdown_v2` 增强版 markdown（≤4096 字节） |
| E4 | `SendImageAsync` | 同上 | `image`（base64 + md5，≤2MB） |
| E5 | `SendNewsAsync` | 同上 | `news` 图文（1~8 条） |
| E6 | `SendFileAsync` | 同上 | `file` 文件（需先取 `media_id`） |
| E7 | `SendVoiceAsync` | 同上 | `voice` 语音（AMR，≤2MB / ≤60s） |
| E8 | `SendTemplateCardAsync` | 同上 | `template_card` 模板卡片（复用 `Message` 域的扁平卡片结构） |
| E9 | `UploadMediaAsync` | `POST /cgi-bin/webhook/upload_media` | 文件 / 语音素材上传，换 3 天有效的 `media_id` |

**一条路由 8 个方法**：`send` 路由按 `msgtype` 逐类型拆成 8 个强类型方法（对齐 `/cgi-bin/message/send` 的既有形态，**不做运行时多态、不引入泛型 msgtype**，AOT 源生成按声明类型序列化）；请求 DTO 各自继承信封基类 `WechatWebhookSendRequest`（只含 `msgtype`）。

**与「发送应用消息」的区别**：本域接收方由 `key`（即机器人所在群）决定，没有 `touser` / `toparty` / `totag`，也**不使用任何令牌**（`access_token` / `suite_access_token` / `provider_access_token` 三条链路都与它无关）。所以同一个群机器人，任何应用形态的宿主都能推，但推不到群以外的人。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **不适用**（凭据 `key` 由群机器人配置页直接颁发，与三类应用令牌体系无关） | — |
| 必填配置属性 | **无** —— 本域不读取 `WechatAppConfig` 的任何字段 | — |
| 令牌类型 | **无令牌**：父 / 子接口均不声明 `[Token]`（WEB3 锁定；TO1 的无令牌例外清单） | — |
| 令牌归属键 | 无 | — |
| 装配前置 | 仍需先 `AddWechatApp(...)` —— `AddWechatWorkServices(...).Build()` 会校验 `IWechatAppManager` 已注册；本域只是不消费其凭据 | 注册期 |
| 机器人凭据 | Query 参数 `key`（长期有效；泄露后到机器人配置页**重置**） | 每次请求由调用方传入 |

> `key` 不是 `WechatAppConfig` 的属性，也没有 SDK 侧配置项 —— 它是每个方法的第一个参数，由宿主自行保管（配置中心 / 环境变量 / 密钥管理）。**不要**把它写进代码、示例或日志。

### 2.2 权限与业务前置

- **机器人必须先由群管理员添加**：SDK 不提供创建 / 管理群机器人的 API，`key` 只能来自企业微信客户端的群机器人配置页。
- **只能在机器人所在的群发消息**：接收范围由 `key` 绑定，没有成员级参数；要发给另一个群就得再加一台机器人。
- **频率上限 20 条/分钟/机器人**：超限后官方报错或直接丢弃。**不要用重试对冲** —— 重试继续占用同一频控窗口。
- **`@` 提醒仅群聊生效**：`mentioned_list`（userid）与 `mentioned_mobile_list`（手机号，支持 `@all`）合计最多 20 人，且只对群成员有效。
- **`media_id` 三天有效且不能跨机器人**：E6 / E7 的 `media_id` 只能来自 E9 上传、且只能被同一台机器人消费。
- **E9 当前不可用**：`[MultipartForm] IFormContent` 在现行生成器下不上传文件字节（见 §5、§7）⇒ 现阶段 E6 / E7 拿不到可用的 `media_id`。
- **日志纪律**：`key` 属长期凭据，`@` 名单里的 userid / 手机号属个人数据 ⇒ 两者都不得进日志、遥测或异常消息。SDK 已在模块注册期把 `key` 登记为进程级强制掩码键（随异常与遥测 URL 输出时被掩码），但宿主**自行拼接打印 URL** 仍会泄露。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E9 | 不区分应用类型 | 不区分应用类型 | 不区分应用类型 |

官方以**群机器人的 `key`** 为凭证主体，接口约定与 `WechatAppType` 无关；SDK 侧因此没有任何 `*ThirdParty*` / `*Provider*` 子接口（WEB2 断言派生接口集合恰为 `{ IWechatWorkInternalWebhookService }`）。三列在此仅表示**本域不按该维度开放**，而非「三类都支持」。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行（Build 期校验 IWechatAppManager），再注册群机器人模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddWebhookApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Webhook` |
| 注册入口 | `WechatWorkServiceBuilder.AddWebhookApi()`（→ 源生成 `AddWebhookWebApiHttpClient()`） |
| 注册组名 | `"Webhook"` |

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWebhookService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 0 | 否 |
| `IWechatWorkInternalWebhookService` | `Mud.Wechat.Work` | 唯一子接口（承载全部端点） | 9 | 是 |

- 端点索引：E1 `SendTextAsync` / E2 `SendMarkdownAsync` / E3 `SendMarkdownV2Async` / E4 `SendImageAsync` / E5 `SendNewsAsync` / E6 `SendFileAsync` / E7 `SendVoiceAsync` / E8 `SendTemplateCardAsync` / E9 `UploadMediaAsync`。
- **`key` 脱敏登记**：`AddWebhookApi()` 的注册组 lambda 会调用组件公开门面 `Mud.HttpUtils.SensitiveUrlKeys.Register("key")`，把 `key` 登记为**进程级强制掩码键**（幂等、线程安全，`AddAllApis` 复入安全；登记后不受 `MudHttpObservabilityOptions.RedactUrlInTelemetry` 开关约束）。宿主若绕过 builder 直调 `AddWebhookWebApiHttpClient()`（非常规用法），须自行登记。

---

## 4. 端点

### E1. `SendTextAsync` — 发送文本消息（`msgtype=text`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendTextAsync(
    [Query("key")] string key,
    [Body] WechatWebhookTextRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/webhook/send` |
| 凭据 | Query `key`（机器人凭据，**非令牌**） |
| 适用应用类型 | 不区分（凭据由机器人颁发） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `key` | `key`（Query） | `string` | 是 | 机器人 Webhook 地址上的 key |
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `text` |
| `Text.Content` | `text.content` | `string?` | 是 | 最长 **2048 字节**（不是字符）；换行须为转义的 `\n` |
| `Text.MentionedList` | `text.mentioned_list` | `List<string>?` | 否 | userid 列表，支持 `@all`；**仅群聊生效**，与手机号列表合计 ≤20 人 |
| `Text.MentionedMobileList` | `text.mentioned_mobile_list` | `List<string>?` | 否 | 手机号列表，支持 `@all`；**仅群聊生效**，合计 ≤20 人 |

**响应参数**（`WechatWebhookSendResponse`，继承 `WechatWorkResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 成功 |
| `errmsg` | `ErrorMessage` | `string?` | 错误描述 |

**本端点注意事项**

- `content` 的上限是**字节**：中文按 UTF-8 三字节计，2048 字节约 680 个汉字。
- `@` 必须在消息正文里也出现对应文本（官方显示规则），且成员须在该群内。
- 超频（20 条/分钟）后官方可能直接丢弃，调用方不应盲目重试。

**调用示例**（E1 / E2 / E5 / E8 串成一次推送，见本节末尾统一示例）

---

### E2. `SendMarkdownAsync` — 发送 markdown 消息（`msgtype=markdown`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendMarkdownAsync(
    [Query("key")] string key,
    [Body] WechatWebhookMarkdownRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `markdown` |
| `Markdown.Content` | `markdown.content` | `string?` | 是 | 最长 **4096 字节**；仅支持官方子集（标题、加粗、斜体、链接、行内代码、引用、字体颜色 `green/info/comment/warning`、换行等） |

响应同 E1。

**本端点注意事项**

- 官方子集**不是**完整 CommonMark：表格、任务列表、代码块等需要 `markdown_v2`（E3）。
- 字体颜色只认 `green` / `info`（灰）/ `comment`（灰）/ `warning`（橙红）四个值，其它值不生效。

---

### E3. `SendMarkdownV2Async` — 发送 markdown_v2 消息（`msgtype=markdown_v2`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendMarkdownV2Async(
    [Query("key")] string key,
    [Body] WechatWebhookMarkdownV2Request request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `markdown_v2` |
| `MarkdownV2.Content` | `markdown_v2.content` | `string?` | 是 | 最长 **4096 字节**；增强语法：无序 / 有序列表、引用、代码块、任务列表、表格 |

响应同 E1。

**本端点注意事项**

- `markdown` 与 `markdown_v2` 的**语法集不同、不可互相套用**：v2 的表格 / 任务列表在老语法里不渲染，老语法的颜色标记在 v2 里也不保证一致。

---

### E4. `SendImageAsync` — 发送图片消息（`msgtype=image`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendImageAsync(
    [Query("key")] string key,
    [Body] WechatWebhookImageRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `image` |
| `Image.Base64` | `image.base64` | `string?` | 是 | 图片内容的 base64 编码 |
| `Image.Md5` | `image.md5` | `string?` | 是 | 图片内容的 MD5 **十六进制摘要**（不是文件名 / 不是 base64 的摘要） |

响应同 E1。

**本端点注意事项**

- `base64` 与 `md5` **两个都必填**，缺一官方报错。
- 图片 ≤2MB，仅 JPG / PNG。
- base64 会让请求体膨胀约 4/3，靠近上限时注意宿主侧请求体限制。

---

### E5. `SendNewsAsync` — 发送图文消息（`msgtype=news`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendNewsAsync(
    [Query("key")] string key,
    [Body] WechatWebhookNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `news` |
| `News.Articles` | `news.articles` | `List<WechatWebhookNewsArticle>?` | 是 | **1~8 条**，超出报错 |
| `Articles[].Title` | `title` | `string` | 是 | 图文标题（SDK 默认空串） |
| `Articles[].Description` | `description` | `string?` | 否 | 描述 |
| `Articles[].Url` | `url` | `string?` | 否 | 点击整条图文的跳转地址 |
| `Articles[].Picurl` | `picurl` | `string?` | 否 | **公网可访问**的完整 http(s) 图片链接，JPG / PNG；建议大图 1068×455、小图 150×150 |

响应同 E1。

**本端点注意事项**

- `picurl` 不可访问（内网、需鉴权、http 被拦）时官方展示为**空白图**，不会报错 —— 排障时先验证图片链接。
- 条数上限 8，超出即整条消息失败。

---

### E6. `SendFileAsync` — 发送文件消息（`msgtype=file`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendFileAsync(
    [Query("key")] string key,
    [Body] WechatWebhookFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `file` |
| `File.MediaId` | `file.media_id` | `string?` | 是 | 须先经 E9（`type=file`）上传获得；**3 天内有效**，不能跨机器人 |

响应同 E1。

**本端点注意事项**

- 文件大小 5B ~ 20M（E9 侧限制）。
- 当前 E9 不可用（§7），故本端点实际拿不到新 `media_id` —— 先用其它通道获取素材的来源也不行，`media_id` 只能由同一机器人上传产生。

---

### E7. `SendVoiceAsync` — 发送语音消息（`msgtype=voice`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendVoiceAsync(
    [Query("key")] string key,
    [Body] WechatWebhookVoiceRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `voice` |
| `Voice.MediaId` | `voice.media_id` | `string?` | 是 | 须先经 E9（`type=voice`）上传获得；**3 天内有效**，不能跨机器人 |

响应同 E1。

**本端点注意事项**

- 语音仅 **AMR** 格式、≤2MB、播放时长 ≤60s。
- 与 E6 同样受 E9 不可用影响（§7）。

---

### E8. `SendTemplateCardAsync` — 发送模板卡片消息（`msgtype=template_card`）

```csharp
[Post("/cgi-bin/webhook/send")]
Task<WechatWebhookSendResponse> SendTemplateCardAsync(
    [Query("key")] string key,
    [Body] WechatWebhookTemplateCardRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `MsgType` | `msgtype` | `string?` | 是 | 固定填 `template_card` |
| `TemplateCard.CardType` | `template_card.card_type` | `string?` | 是 | `text_notice` / `news_notice` / `button_interaction` / `vote_interaction` / `multiple_interaction` |
| `TemplateCard.TaskId` | `template_card.task_id` | `string?` | 视类型 | 按钮交互 / 投票 / 多项选择型必填；仅数字字母与 `_-@`，≤128 字节；同应用内不可重复 |
| `TemplateCard.Source` | `template_card.source` | `TemplateCardSource?` | 否 | 卡片来源样式 |
| `TemplateCard.MainTitle` | `template_card.main_title` | `TemplateCardTitle?` | 视类型 | 一级标题（`title` / `desc`） |
| `TemplateCard.SubTitleText` | `template_card.sub_title_text` | `string?` | 否 | 二级文本，建议 ≤160 字 |
| `TemplateCard.HorizontalContentList` | `template_card.horizontal_content_list` | `List<TemplateCardHorizontalContent>?` | 否 | ≤6 条 |
| `TemplateCard.JumpList` | `template_card.jump_list` | `List<TemplateCardJump>?` | 否 | ≤3 条 |
| `TemplateCard.CardAction` | `template_card.card_action` | `TemplateCardAction?` | 视类型 | 整卡跳转；文本通知 / 图文展示型必填 |
| `TemplateCard.ImageTextArea` / `CardImage` | `image_text_area` / `card_image` | 各自对象 | 视类型 | 仅图文展示型，二者必填其一 |
| `TemplateCard.ButtonList` | `template_card.button_list` | `List<TemplateCardButton>?` | 视类型 | ≤6 条，仅按钮交互型 |
| `TemplateCard.Checkbox` / `SubmitButton` | `checkbox` / `submit_button` | 各自对象 | 视类型 | 投票 / 多项选择型 |
| `TemplateCard.SelectList` | `template_card.select_list` | `List<TemplateCardSelector>?` | 视类型 | ≤3 个，仅多项选择型 |

> 完整字段集以 [`TemplateCardBody`](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Message/TemplateCardBody.cs) 为准（`Message` 域已有专文逐字段说明，本族只列常用项）。

响应同 E1。

**本端点注意事项**

- 卡片结构**与应用消息的 `template_card` 完全同构**（共用 `TemplateCardBody`），差别只在发送端点与凭据面：本端点是「机器人 `key`」，`/cgi-bin/message/send` 是「应用 `access_token`」。
- 版本门槛与回调能力跟应用消息一致：支持回调的卡片需要应用配置回调接口并返回一次性 `response_code` —— 群机器人没有应用上下文，**可交互卡片的回调链路不适用于本域**（详见 §7）。
- 微工作台不支持展示模板卡片。

---

### E9. `UploadMediaAsync` — 上传媒体文件（文件 / 语音）

```csharp
[Post("/cgi-bin/webhook/upload_media")]
Task<WechatWebhookUploadMediaResponse> UploadMediaAsync(
    [Query("key")] string key,
    [Query("type")] string mediaType,
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/webhook/upload_media`（`multipart/form-data`） |
| 凭据 | Query `key` + Query `type` |
| 适用应用类型 | 不区分（凭据由机器人颁发） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `key` | `key`（Query） | `string` | 是 | 机器人凭据 |
| `mediaType` | `type`（Query） | `string` | 是 | `file` 普通文件 / `voice` 语音 |
| `formData` | —（Form） | `IFormContent` | 是 | multipart 表单，**文件字段名必须为 `media`**，须含 `filename` / `filelength` / `content-type`；由宿主实现 `Mud.HttpUtils.IFormContent` |

**响应参数**（`WechatWebhookUploadMediaResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `string?` | 回显媒体类型 |
| `media_id` | `MediaId` | `string?` | 素材 id，**3 天内有效**，仅可被同一机器人消费 |
| `created_at` | `CreatedAt` | `long?` | 上传时间戳（Unix 秒） |

> ⚠️ **本端点在当前依赖版本下不可用**（文件字节不上 wire），原因与复现见 §5「上传腿」与 §7。签名、参数与文档口径均有效，实际上传须等上游 `Mud.HttpUtils.Generator` 修复。

**调用示例**（E1 文本 → E2 markdown → E5 图文 → E8 模板卡片）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.DataModels.Webhook;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    // 本域不读取任何应用凭据；此处应用只为满足 AddWechatWorkServices 的 IWechatAppManager 装配前置
    o.AppKey = "notify-bot";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = "1000001";
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWebhookApi());

var webhook = provider.GetRequiredService<IWechatWorkInternalWebhookService>();

// key 取自群机器人配置页的 Webhook 地址，由宿主配置中心保管；不得写入代码或日志
string key = config["Wechat:Webhook:Key"] ?? throw new InvalidOperationException("缺少群机器人 key");

// E1 文本：@ 提醒仅群聊生效，userid 与手机号两列表合计不超过 20 人
var text = await webhook.SendTextAsync(key, new WechatWebhookTextRequest
{
    MsgType = "text",
    Text = new WechatWebhookTextBody
    {
        Content = "构建 #1042 已完成，耗时 3 分 12 秒",
        MentionedList = ["staff_0001", "staff_0002"],   // 假值
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(text, "/cgi-bin/webhook/send");

// E2 markdown：官方子集语法，正文按字节限长 4096
var markdown = await webhook.SendMarkdownAsync(key, new WechatWebhookMarkdownRequest
{
    MsgType = "markdown",
    Markdown = new WechatWebhookMarkdownBody
    {
        Content = "> 构建结果\n> 状态：<font color=\"info\">成功</font>\n> 详情：https://ci.example.invalid/builds/1042",
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(markdown, "/cgi-bin/webhook/send");

// E5 图文：1~8 条，picurl 必须是公网可访问的完整 http(s) 链接
var news = await webhook.SendNewsAsync(key, new WechatWebhookNewsRequest
{
    MsgType = "news",
    News = new WechatWebhookNewsBody
    {
        Articles =
        [
            new WechatWebhookNewsArticle
            {
                Title = "构建报告 #1042",
                Description = "3 个服务已更新",
                Url = "https://ci.example.invalid/builds/1042",
                Picurl = "https://cdn.example.invalid/builds/1042.png",
            },
        ],
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(news, "/cgi-bin/webhook/send");

// E8 模板卡片：结构复用 Message 域的 TemplateCardBody（与应用消息同一扁平结构）
var card = await webhook.SendTemplateCardAsync(key, new WechatWebhookTemplateCardRequest
{
    MsgType = "template_card",
    TemplateCard = new TemplateCardBody
    {
        CardType = "text_notice",
        MainTitle = new TemplateCardTitle { Title = "构建结果", Desc = "成功" },
        SubTitleText = "3 个服务已更新",
        CardAction = new TemplateCardAction { Type = 1, Url = "https://ci.example.invalid/builds/1042" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(card, "/cgi-bin/webhook/send");

// 只记结果码：key 与 @ 名单不得进日志
logger.LogInformation("群机器人推送完成：text={Text} markdown={Markdown} news={News} card={Card}",
    text.ErrorCode, markdown.ErrorCode, news.ErrorCode, card.ErrorCode);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 凭据面 | 本域**没有令牌**，`key` 就是凭据；`[Token]` 被 WEB3 明令禁止 | 给接口补 `[Token]` → 生成器注入 `access_token` 参数，违反域契约且打红守卫 |
| `key` 的生命周期 | **长期有效**（与智能机器人 `response_code` 的一次性窗口不同） | 当一次性凭据轮转 / 每次重建 → 反复重置凭据，影响线上机器人 |
| `send` 的路由数 | **1 条路由 8 个方法**，靠 `msgtype` 区分 | 以为有 8 条路由去拼 URL → 404 |
| `msgtype` 一致性 | 每个方法的 DTO 固定对应一个 `msgtype`，基类字段仍可写错 | 用 `SendTextAsync` 填 `msgtype=markdown` → 官方按 markdown 解析，报字段缺失 |
| `markdown` vs `markdown_v2` | 语法集不同（v2 才有表格 / 任务列表 / 代码块） | 在老语法里写表格 → 不渲染；在 v2 里套老颜色标记 → 不一致 |
| 限长单位 | `text` 2048 **字节** / `markdown(_v2)` 4096 **字节** | 按字符数截断 → 中文超限报错 |
| `image.base64` + `image.md5` | 两者**都必填**，md5 是**图片内容**的十六进制摘要 | 只传 base64 → 缺参报错；把文件名当 md5 → 校验失败 |
| `news` 条数 | 1~8 条，超出整条失败 | 拼 9 条 → 整体被拒 |
| `picurl` | 必须**公网可访问**的完整 http(s) 链接 | 内网图 / 需签名链接 → 展示空白图且不报错 |
| `media_id` 时效与归属 | 3 天有效，**只能被同一机器人**消费 | 跨机器人复用 / 第 4 天再用 → 官方拒绝 |
| 语音格式 | 仅 **AMR**、≤2MB、≤60s | 传 mp3 / 超 60s → 拒绝 |
| 模板卡片结构 | 与 `/cgi-bin/message/send` 的 `template_card` **同构共用** `TemplateCardBody` | 另写一套卡片 DTO → 字段漂移、AOT 上下文未登记 |
| **上传腿（现行缺陷）** | 生成器 **3.0.9** 对 `[MultipartForm] IFormContent` 发射 `new StringContent(formData.ToString())` + 字段名 `formData` ⇒ `ToHttpContent()` / `ToHttpContentAsync()` 从不被调用，**文件字节不上 wire** | 宿主不管怎么正确实现 `IFormContent` 都传不上去；官方侧表现为「缺少参数 / 文件」类错误 |
| 接口注释口径 vs 发射形态 | 注释称「`[MultipartForm]` 才走 `ToHttpContentAsync` 通路」——那是 3.0.1 期绕 `CS1061` 的**编译期**结论，与本批实测的**运行期**发射不符 | 按注释信任通路、不复测生成体 → 上传静默失败 |
| 频控 | 20 条/分钟/机器人，超限报错或**丢弃** | 失败重试 → 继续占窗口，形成雪崩 |
| `@` 提醒 | 仅**群聊**生效，合计 ≤20 人 | 在单聊 / 应用消息场景套用 → 不生效 |
| `key` 脱敏边界 | SDK 只在异常与遥测 URL 上掩码 | 宿主自行 `logger.LogInformation(url)` → 明文泄露长期凭据 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200，须 `WechatWorkException.ThrowIfFailed`）、**不做本地必填校验**、**不做频控与重试**、**不校验文件大小 / 格式 / 条数**（越界由官方 `errcode` 表达）、**不跟踪 `media_id` 有效期**（三天窗口须宿主自行落库与续期）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 机器人已受理并推送，无业务负载字段 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 应用令牌类错误 | **不应出现在本域**（无 `[Token]`）；出现即说明宿主改写了 URL 混入 `access_token` |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 同上，本域不适用 |

本族注释未留档**专属**业务错误码：`key` 无效 / 机器人已被移除 / 参数超限 / 频控超限等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **上传腿不可用** | 本批沿用的 3.0.9 生成体实测结论：`[MultipartForm] IFormContent` 不发射文件部分（同 [Media.md](../Media/Media.md) §5 / §7 的取证与复现命令）。影响 E9，进而使 E6 / E7 拿不到新 `media_id` | 上游 `Mud.HttpUtils.Generator` 修复后重跑生成体取证 + 补 wire 级测试；届时**同批**修正 E9 的注释口径与本节 |
| 本仓零 `IFormContent` 实现类、零 wire 级测试 | 上传腿缺陷不被 WEB1~WEB4 覆盖（守卫只锁签名形态、路由、令牌与 JSON 登记）⇒ 属**守卫盲区** | 需要运行期保证时补宿主侧集成测试 |
| 群机器人专属错误码 | `key` 无效 / 机器人不存在 / 频控超限的码值 ⇒ 未核验 | 官方 91770 + 90455 |
| 可交互卡片的回调链路 | 模板卡片若带按钮 / 投票，回调走「应用」的卡片回调；群机器人无应用上下文 ⇒ 本域是否完全支持 ⇒ 未核验（接口注释只保留「不支持 task_id 之外的应用级配置」的约束） | 官方 91770 + 模板卡片回调页 |
| `markdown` / `markdown_v2` 语法白名单 | 注释只给类别（标题 / 列表 / 表格……），未逐条列语法 ⇒ 未逐项核验 | 官方 91770 |
| `image.md5` 的摘要口径 | 注释写「图片内容的十六进制摘要」，未说明大小写与是否对原始字节 ⇒ 未核验 | 官方 91770 |
| 机器人管理面 | 创建 / 删除 / 改名 / 换 key 均无服务端 API（只能客户端配置页操作） | 官方 91770 |
| 真账号运行验证 | 本批仅编译级 + 生成体静态复测，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 同构卡片：[Message/Message.md](../Message/Message.md)（`template_card` 逐字段说明 + 更新卡片 / 撤回）
- 素材上传对照：[Media/Media.md](../Media/Media.md)（应用侧上传端点，同受上传腿缺陷影响）
- 令牌面：[Authentication/ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)（本域**不涉及**任何令牌，用于对照）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[群机器人·消息推送配置说明 91770](https://developer.work.weixin.qq.com/document/path/91770)

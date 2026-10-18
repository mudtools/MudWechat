# Webhook 域 · 群机器人

企业微信「群机器人」模块 Webhook 推送 SDK：官方全部 9 个端点声明于本接口
（`msgtype` 逐类型各一方法 + 媒体上传，同路由多方法范式对齐 `/cgi-bin/message/send`，
不做运行时多态、不引入泛型 msgtype）。 接口：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)、[IWechatWorkWebhookService](./IWechatWorkWebhookService.cs)；注册组 `Webhook`；模块枚举 `WechatModule.Webhook`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatWebhookContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWebhookContractGuards.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/Webhook`](../../../Mud.Wechat.Work.DataModels/Webhook/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalWebhookService` | `SendTextAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送文本消息（`msgtype=text`） |
| 2 | `IWechatWorkInternalWebhookService` | `SendMarkdownAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送 markdown 消息（`msgtype=markdown`） |
| 3 | `IWechatWorkInternalWebhookService` | `SendMarkdownV2Async` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送 markdown_v2 消息（`msgtype=markdown_v2`） |
| 4 | `IWechatWorkInternalWebhookService` | `SendImageAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送图片消息（`msgtype=image`） |
| 5 | `IWechatWorkInternalWebhookService` | `SendNewsAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送图文消息（`msgtype=news`） |
| 6 | `IWechatWorkInternalWebhookService` | `SendFileAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送文件消息（`msgtype=file`） |
| 7 | `IWechatWorkInternalWebhookService` | `SendVoiceAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送语音消息（`msgtype=voice`） |
| 8 | `IWechatWorkInternalWebhookService` | `SendTemplateCardAsync` | POST | `/cgi-bin/webhook/send` | 无 `[Token]`（见域级说明） | 群机器人发送模板卡片消息（`msgtype=template_card`） |
| 9 | `IWechatWorkInternalWebhookService` | `UploadMediaAsync` | POST | `/cgi-bin/webhook/upload_media` | 无 `[Token]`（见域级说明） | 群机器人上传媒体文件（`/cgi-bin/webhook/upload_media`） |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkInternalWebhookService`

**无令牌**：本接口**不声明 `[Token]`** —— 端点以 URL Query 上的 `key` 为机器人凭据
（官方 91770），不属于 `access_token` / `suite_access_token` / `provider_access_token`
任一令牌链路；故也不进入通用守卫 G5（Query 令牌注入白名单）。组件分析器据此场景发出的
HTTPCLIENT018（建议补 `[Token]`）为不适用告警，已在声明处局部 `#pragma` 豁免；
`TokenManage` 与父接口保持一致（全仓父接口恒为 `nameof(IWechatAppManager)`），
防止生成器以 `new` 隐藏基类切换成员（HTTPCLIENT028）。

**开放面**：群机器人由用户在群聊设置中添加、凭据（Webhook URL 及其 `key`）由官方直接颁发，
与应用的令牌体系无关，官方文档不区分自建 / 第三方 / 代开发 ⇒ 继承链上仅本接口一层承载端点
（无更深层应用类型子接口）。

**官方凭证脱敏**：Query 参数 `key` 为长期有效凭据、不在组件静态词表内，
已在 `Webhook` 模块注册期经组件公开登记门面
`SensitiveUrlKeys.Register("key")` 登记为进程级强制掩码键（见父接口 remarks 与守卫 WEB3）。
宿主若绕过 builder 直调生成器产出的 `AddWebhookWebApiHttpClient()`（非常规用法、
不经模块注册组），须自行调用 `SensitiveUrlKeys.Register("key")`，否则 `key`
将随异常与遥测 URL 明文输出。

### `IWechatWorkWebhookService`

群机器人（Webhook 机器人）由用户在群聊设置中添加，官方以 Webhook URL 上的 `key` 为机器人唯一凭据；
Webhook 推送**不属于任何应用**、与 access_token / suite_access_token / provider_access_token
三条令牌链路均无关，官方文档亦不区分自建 / 第三方 / 代开发开放面
⇒ 本域形态对齐 `IWechatWorkAibotService`：零端点父接口 + 唯一自建子接口承载端点，
继承链上不得出现代开发 / 第三方子接口。
端点全部声明于 `IWechatWorkInternalWebhookService`；
父接口不进入令牌归属域守卫 TO1 的枚举面，子接口的无 `[Token]` 例外登记见 `WechatTokenOwnerContractGuards` TO1。

**令牌面：本域端点不使用任何令牌链路**。官方 `/cgi-bin/webhook/send` 与
`/cgi-bin/webhook/upload_media` 均以 URL Query 上的 `key` 为机器人凭据，
故本域父 / 子接口**均不声明 `[Token]`** —— 这是「无令牌端点」的第三个既存例外
（前两个为 `IWechatWorkAibotService`：response_code 一次性凭据；
`IWechatWorkProviderAuthenticationUrl`：令牌经显式 Query 参数直传）。
组件分析器的 HTTPCLIENT018（建议补 `[Token]`）对本域为不适用告警，已在声明处局部 `#pragma` 豁免。

**无令牌 ≠ 不参与多应用切换**：`TokenManage` 只配置应用切换管理器（生成器据此发射
`UseApp` / `BeginScope`），与令牌注入无关；与全仓各域父接口形态一致（恒为
`nameof(IWechatAppManager)`），且父子两级必须统一——否则生成器以 `new` 隐藏基类切换成员
（组件分析器 HTTPCLIENT028）。

**官方凭证脱敏（模块注册期强制掩码）**：
`key` 是**长期有效**的 URL 凭据（与 response_code 的一次性窗口不同），随
`ApiException.RequestUri` 与请求/响应遥测 URL 输出。其参数名不在组件
`SensitiveUrlRedactor` 静态词表内（该词表为精确匹配、组件侧刻意不收全局通用名
—— `key` 一词过于通用，直接进全局词表会过度脱敏）。
因此 `Webhook` 模块注册期经组件公开登记门面
`Mud.HttpUtils.SensitiveUrlKeys.Register("key")` 将其登记为**进程级强制掩码键**
（幂等、线程安全、`AddAllApis` 复入亦安全；登记后不受
`MudHttpObservabilityOptions.RedactUrlInTelemetry` 开关约束）。
需 Mud.HttpUtils ≥ 3.0.2；静态词表是否仍不含 `key` 的反向自过期断言与登记后的
运行期功能断言见守卫 WEB3。宿主若绕过 builder 直调 `AddWebhookWebApiHttpClient()`
（非常规用法），需自行登记，见接口 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 9 次）**群机器人·消息推送配置说明**SDK文档：https://developer.work.weixin.qq.com/document/path/91770

### 1. `SendTextAsync` — 群机器人发送文本消息（`msgtype=text`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookTextRequest` — 请求体（`WechatWebhookTextRequest`：msgtype / text.content ≤ 2048 字节 /
text.mentioned_list 仅群聊可用、最多 20 人 / text.mentioned_mobile_list 仅群聊可用、最多 20 人）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- 支持的 markdown/模板卡片等其余消息类型见本接口各 `Send*Async` 方法（同路由逐 msgtype 一方法）。
- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：`text.content` 最长 2048 个字节（超出报错）；`@text.mentioned_list` 为 userid 列表、 / `@text.mentioned_mobile_list` 为手机号列表（支持 `@all`），二者**仅在群聊中生效**、且最多 20 人。

### 2. `SendMarkdownAsync` — 群机器人发送 markdown 消息（`msgtype=markdown`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookMarkdownRequest` — 请求体（`WechatWebhookMarkdownRequest`：msgtype / markdown.content ≤ 4096 字节）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：`markdown.content` 最长 4096 个字节（超出报错）；支持的 markdown 语法为官方子集 / （标题、加粗、斜体、链接、行内代码、引用、字体颜色 green/info/comment/warning、换行等）。

### 3. `SendMarkdownV2Async` — 群机器人发送 markdown_v2 消息（`msgtype=markdown_v2`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookMarkdownV2Request` — 请求体（`WechatWebhookMarkdownV2Request`：msgtype / markdown_v2.content ≤ 4096 字节）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：`markdown_v2.content` 最长 4096 个字节（超出报错）；markdown_v2 为增强版 markdown， / 支持无序列表、有序列表、引用、代码块、任务列表、表格等扩展语法（与 markdown 消息的语法集不同， / 二者不可互相套用）。

### 4. `SendImageAsync` — 群机器人发送图片消息（`msgtype=image`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookImageRequest` — 请求体（`WechatWebhookImageRequest`：msgtype / image.base64 与 image.md5 官方均必填）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：图片（`base64` 编码内容 + `md5` 值，二者均必填）大小不超过 2MB， / 仅支持 JPG/PNG 格式（md5 为图片内容的十六进制摘要，用于官方校验）。

### 5. `SendNewsAsync` — 群机器人发送图文消息（`msgtype=news`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookNewsRequest` — 请求体（`WechatWebhookNewsRequest`：msgtype / news.articles 1~8 条）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：图文条数（`news.articles`）1~8 条；`picurl` 必须是**公网可访问**的完整图片链接 / （http/https，否则官方展示为空白图）；点击整条图文跳转 `url`。

### 6. `SendFileAsync` — 群机器人发送文件消息（`msgtype=file`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookFileRequest` — 请求体（`WechatWebhookFileRequest`：msgtype / file.media_id）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：`file.media_id` 须先经 `UploadMediaAsync`（type=file）上传获得， / media_id 仅 **3 天内**有效。

### 7. `SendVoiceAsync` — 群机器人发送语音消息（`msgtype=voice`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookVoiceRequest` — 请求体（`WechatWebhookVoiceRequest`：msgtype / voice.media_id）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：`voice.media_id` 须先经 `UploadMediaAsync`（type=voice）上传获得， / 语音文件大小不超过 2MB、播放长度不超过 60s、仅支持 AMR 格式；media_id 仅 **3 天内**有效。

### 8. `SendTemplateCardAsync` — 群机器人发送模板卡片消息（`msgtype=template_card`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/send`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |

- **请求体**：`WechatWebhookTemplateCardRequest` — 请求体（`WechatWebhookTemplateCardRequest`：msgtype / template_card 扁平卡片结构，与发送应用消息的 `TemplateCardBody` 共用）。
- **返回**：`WechatWebhookSendResponse` — 应答结果（errcode / errmsg）。

**补充说明**

- **频率限制**：每个机器人发送的消息不能超过 20 条/分钟（超过上限后官方报错/丢弃）。

**备注（原文）**

- 官方约束：模板卡片字段与 `/cgi-bin/message/send` 的 template_card 一致（共用 / `TemplateCardBody` 扁平结构）； / 群机器人模板卡片不支持 `task_id` 驱动的按钮回调更新链路之外的应用级配置。

### 9. `UploadMediaAsync` — 群机器人上传媒体文件（`/cgi-bin/webhook/upload_media`）

- **声明接口**：[IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)
- **官方契约**：`POST /cgi-bin/webhook/upload_media`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `key` | Query `key` | `string` | 机器人 Webhook 地址上的 key 参数（官方必填；由群机器人配置页颁发的唯一凭据，泄露后可在配置页重置）。 |
| `mediaType` | Query `type` | `string` | 媒体文件类型（官方必填）：file - 普通文件，voice - 语音。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（文件字段名须为 `media`，由调用方构建<br>`IFormContent` 实现，含 filename / filelength / content-type 信息）。<br>须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的<br>`GetFormDataContentAsync` 调用在运行时接口上不存在（CS1061），<br>`[MultipartForm]` 才走 `ToHttpContentAsync` 通路。 |
- **返回**：`WechatWebhookUploadMediaResponse` — 媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。

**补充说明**

- 用于发送文件 / 语音消息前先上传素材，获取 media_id（multipart/form-data，文件标识名必须为 `media`， 须包含 filename、filelength、content-type 等信息）。

**备注（原文）**

- 官方约束：文件（file）大小要求 5B ~ 20M；语音（voice）大小不超过 2MB、播放长度不超过 60s、仅支持 AMR 格式； / 返回的 media_id 仅 **3 天内**有效。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/Webhook`](../../../Mud.Wechat.Work.DataModels/Webhook/) 内共 **19** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **19** 个，`SerializerClassName` = `Webhook`。

AOT 源生成上下文：[`WebhookJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/WebhookJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WechatWebhookSendResponse` | 应答 | [Webhook/WechatWebhookSendResponse.cs](../../../Mud.Wechat.Work.DataModels/Webhook/WechatWebhookSendResponse.cs) | — |
| `WechatWebhookTextRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookTextRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookTextRequest.cs) | `text` |
| `WechatWebhookMarkdownRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookMarkdownRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookMarkdownRequest.cs) | `markdown` |
| `WechatWebhookMarkdownV2Request` | 请求体 | [Webhook/RequestModel/WechatWebhookMarkdownV2Request.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookMarkdownV2Request.cs) | `markdown_v2` |
| `WechatWebhookImageRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookImageRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookImageRequest.cs) | `image` |
| `WechatWebhookNewsRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookNewsRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookNewsRequest.cs) | `news` |
| `WechatWebhookFileRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookFileRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookFileRequest.cs) | `file` |
| `WechatWebhookVoiceRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookVoiceRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookVoiceRequest.cs) | `voice` |
| `WechatWebhookTemplateCardRequest` | 请求体 | [Webhook/RequestModel/WechatWebhookTemplateCardRequest.cs](../../../Mud.Wechat.Work.DataModels/Webhook/RequestModel/WechatWebhookTemplateCardRequest.cs) | `template_card` |
| `WechatWebhookUploadMediaResponse` | 应答 | [Webhook/WechatWebhookUploadMediaResponse.cs](../../../Mud.Wechat.Work.DataModels/Webhook/WechatWebhookUploadMediaResponse.cs) | `type`, `media_id`, `created_at` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Webhook;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddWebhookApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalWebhookService>();
var ct = CancellationToken.None;
var response = await service.SendFileAsync("", new WechatWebhookFileRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/webhook/send");
// 占位值仅用于编译示意；业务值须按 IWechatWorkInternalWebhookService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SendFileAsync`）；请求体 `WechatWebhookFileRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkInternalWebhookService](./IWechatWorkInternalWebhookService.cs)、[IWechatWorkWebhookService](./IWechatWorkWebhookService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatWebhookContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWebhookContractGuards.cs)、[`WechatTokenOwnerContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 守卫中的代表性测试方法 | `WebhookEndpoints_ShouldMatchOfficialRoutesAndKeyQuery`、`WebhookInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild`、`WebhookInterfaces_ShouldNotDeclareTokenAttribute_AndKeyRedactionShouldStayEnforced`、`WebhookDataModels_ShouldBeRegisteredInJsonContext`、`FacadeInterfaces_ShouldDeclareOwningTokenManagerKey`、`DeclaredOwningKeys_ShouldBeCoveredByRecoveryRegistryAndDisjoint`、`OwningKeyRouting_ShouldEnforceAppTypeMatrix` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/Webhook`](../../../Mud.Wechat.Work.DataModels/Webhook/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatWebhookContractGuards.cs`、`WechatTokenOwnerContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

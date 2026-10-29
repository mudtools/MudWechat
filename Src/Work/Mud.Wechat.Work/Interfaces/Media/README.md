# Media 域 · 素材管理

企业微信「素材管理」域公共 SDK（上传临时素材 / 获取临时素材 / 上传图片 / 获取高清语音素材 / 异步上传临时素材，
三类应用均可调用的端点收敛面）。 接口：[IWechatWorkInternalMediaService](./IWechatWorkInternalMediaService.cs)、[IWechatWorkMediaService](./IWechatWorkMediaService.cs)、[IWechatWorkProviderMediaService](./IWechatWorkProviderMediaService.cs)、[IWechatWorkServiceMediaService](./IWechatWorkServiceMediaService.cs)、[IWechatWorkThirdPartyMediaService](./IWechatWorkThirdPartyMediaService.cs)、[IWechatWorkThirdPartyServiceMediaService](./IWechatWorkThirdPartyServiceMediaService.cs)；注册组 `Media`；模块枚举 `WechatModule.Media`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatMediaContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMediaContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Media`](../../../Mud.Wechat.Work/DataModels/Media/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**7** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkMediaService` | `UploadTemporaryMediaAsync` | POST | `/cgi-bin/media/upload` | `AccessToken`（Query），Query `access_token` | 上传临时素材 |
| 2 | `IWechatWorkMediaService` | `GetTemporaryMediaAsync` | GET | `/cgi-bin/media/get` | `AccessToken`（Query），Query `access_token` | 获取临时素材 |
| 3 | `IWechatWorkMediaService` | `UploadImageAsync` | POST | `/cgi-bin/media/uploadimg` | `AccessToken`（Query），Query `access_token` | 上传图片 |
| 4 | `IWechatWorkMediaService` | `GetHdVoiceMediaAsync` | GET | `/cgi-bin/media/get/jssdk` | `AccessToken`（Query），Query `access_token` | 获取高清语音素材 |
| 5 | `IWechatWorkMediaService` | `UploadMediaByUrlAsync` | POST | `/cgi-bin/media/upload_by_url` | `AccessToken`（Query），Query `access_token` | 生成异步上传任务 |
| 6 | `IWechatWorkMediaService` | `GetUploadMediaByUrlResultAsync` | POST | `/cgi-bin/media/get_upload_by_url_result` | `AccessToken`（Query），Query `access_token` | 查询异步上传任务结果 |
| 7 | `IWechatWorkThirdPartyServiceMediaService` | `UploadServiceMediaAsync` | POST | `/cgi-bin/service/media/upload` | `ProviderAccessToken`（Query），Query `provider_access_token` | 服务商上传临时素材 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkInternalMediaService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkMediaService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkProviderMediaService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkServiceMediaService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyMediaService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyServiceMediaService` | `ProviderAccessToken` | — | Query / `provider_access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkInternalMediaService`

官方向自建应用开放的素材管理端点集（上传临时素材、获取临时素材、上传图片、获取高清语音素材、
异步上传临时素材两个端点）与三类应用的公共面完全重合，全部继承自 `IWechatWorkMediaService`；
因此本接口不新增端点，仅作为自建应用的类型化契约入口存在（形态对齐
`IWechatWorkProviderDepartmentsService` 空标记子接口）。
第三方应用见 `IWechatWorkThirdPartyMediaService`；服务商代开发见 `IWechatWorkProviderMediaService`。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入）。
官方约束：异步上传临时素材须具有「客户联系」权限。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkMediaService`

服务商通道「上传临时素材」（`/cgi-bin/service/media/upload`，走 provider_access_token）仅第三方应用开放，
声明于 `IWechatWorkThirdPartyServiceMediaService`（零端点父接口
`IWechatWorkServiceMediaService`，形态对齐 `IWechatWorkIdentitySuiteService`）。

落位与形态对齐 `IWechatWorkDepartmentsService`：三类应用消费的令牌路由键均为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderMediaService`

官方向代开发应用开放的素材管理端点集（上传临时素材、获取临时素材、上传图片、获取高清语音素材、
异步上传临时素材两个端点）与三类应用的公共面完全重合，全部继承自 `IWechatWorkMediaService`；
因此本接口不新增端点，仅作为代开发应用的类型化契约入口存在（形态对齐
`IWechatWorkProviderDepartmentsService` 空标记子接口）。
自建应用见 `IWechatWorkInternalMediaService`；第三方应用见 `IWechatWorkThirdPartyMediaService`。

消费代开发应用的企业级 access_token（`gettoken` 以 `corpsecret = permanent_code` 换取——
代开发 K1 契约：permanent_code 语义是「应用 secret」，路由键 `AccessToken`，
scope = authCorpId）——调用前须经 `IWechatAppContextSwitcher` 切换到目标授权企业作用域。
官方约束：异步上传临时素材须具有「客户联系」权限。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkServiceMediaService`

官方仅向**第三方应用**开放本族端点（走服务商凭证 provider_access_token，
与三类应用公共面 `IWechatWorkMediaService` 的 access_token 路由键不同，故独立成族），
因此本父接口没有公共端点；全部 1 个端点声明于
`IWechatWorkThirdPartyServiceMediaService`（唯一的应用类型子接口，落位形态对齐
`IWechatWorkIdentitySuiteService` 零端点父接口 + 唯一子接口承载）。

与三类应用公共面（`IWechatWorkMediaService`）路由、令牌均不同：
令牌路由键为 `ProviderAccessToken`（Query 注入 `provider_access_token`，
服务商级凭证）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`provider_access_token`），无法改用 Header。

### `IWechatWorkThirdPartyMediaService`

官方向第三方应用开放的素材管理端点集（上传临时素材、获取临时素材、上传图片、获取高清语音素材、
异步上传临时素材两个端点）与三类应用的公共面完全重合，全部继承自 `IWechatWorkMediaService`；
因此本接口不新增端点，仅作为第三方应用的类型化契约入口存在。
服务商通道「上传临时素材」（`/cgi-bin/service/media/upload`）仅第三方应用开放，
走 provider_access_token、令牌不同，独立成族声明于
`IWechatWorkThirdPartyServiceMediaService`（零端点父接口 `IWechatWorkServiceMediaService`）。
自建应用见 `IWechatWorkInternalMediaService`；服务商代开发见 `IWechatWorkProviderMediaService`。

消费授权企业级 access_token（经 `get_corp_token` 以 `permanent_code` 换取，路由键
`AccessToken`，scope = authCorpId）——调用前须经
`IWechatAppContextSwitcher` 切换到目标授权企业作用域。
官方约束：异步上传临时素材须具有「客户联系」权限。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyServiceMediaService`

自建 / 代开发应用官方不开放本族端点，不设对应子接口；三类应用公共面见 `IWechatWorkMediaService`。

消费服务商级 provider_access_token（路由键 `ProviderAccessToken`）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`provider_access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `UploadTemporaryMediaAsync` — 上传临时素材

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/media/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `type` | Query `type` | `string` | 媒体文件类型（官方必填）：image - 图片，voice - 语音，video - 视频，file - 普通文件。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（文件字段名须为 `media`，由调用方构建<br>`IFormContent` 实现，含 filename / filelength / content-type 信息）。<br>须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的<br>`GetFormDataContentAsync` 调用在运行时接口上不存在（CS1061），<br>`[MultipartForm]` 才走 `ToHttpContentAsync` 通路。 |
- **返回**：`UploadMediaResponse` — 媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。

**补充说明**

- 用于发送消息的图片、语音、视频、普通文件上传（multipart/form-data，文件标识名必须为 `media`， 须包含 filename、filelength、content-type 等信息；filename 决定发消息时展示的文件名）。
- 官方限制：所有文件必须大于 5 个字节；图片（image）10MB、仅支持 JPG/PNG； 语音（voice）2MB、播放长度不超过 60s、仅支持 AMR 格式；视频（video）10MB、仅支持 MP4 格式；普通文件（file）20MB。
- 返回的 media_id 仅三天内有效，同一企业内应用之间可以共享。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90253
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90389
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96484

### 2. `GetTemporaryMediaAsync` — 获取临时素材

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/media/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mediaId` | Query `media_id` | `string` | 媒体文件 id（官方必填），来源为「上传临时素材」及「异步上传临时素材」接口。 |
| `range` | Header | `string?` | HTTP `Range` 请求头（可选，如 `bytes=0-1023`）；<br>分块下载成功返回状态码 206 Partial Content，响应含 Accept-Ranges 与 Content-Range 头。 |
- **返回**：`byte[]?` — 素材文件二进制内容（media_id 三天有效；错误时由 errcode 判定器抛 `WechatWorkException`）。

**补充说明**

- 凭 media_id 获取临时素材文件内容。成功时与普通 HTTP 下载相同（Content-Type / Content-disposition 等头部 + 二进制文件内容），失败时返回 JSON（如 errcode 40007 invalid media_id）。
- 官方限制：异步上传临时素材获得的 media_id 超过 20M 时必须使用 Range 分块下载且分块不超过 20M， 否则返回错误码 830002（其余 media_id 文件过大同样返回 830002，建议分块不超过 20M）； 加密存储场景下指定下载分片长度只能为 16 字节的倍数，否则无法进行分片解密 （以 Content-Range 结束位置 + 1 是否等于 size 判断是否为最后一个分片）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90256
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90390
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96486

### 3. `UploadImageAsync` — 上传图片

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/media/uploadimg`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（由调用方构建 `IFormContent` 实现，<br>含 filename / content-type 信息）。<br>须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的<br>`GetFormDataContentAsync` 调用在运行时接口上不存在（CS1061），<br>`[MultipartForm]` 才走 `ToHttpContentAsync` 通路。 |
- **返回**：`UploadImageResponse` — 上传后得到的图片 URL（永久有效，仅限企业微信环境使用）。

**补充说明**

- 上传图片得到图片 URL（multipart/form-data，文件标识名须包含 filename、content-type 等信息），该 URL 永久有效。
- 官方限制：图片文件大小应在 5B ~ 2MB 之间；每个企业每月最多可上传 3000 张图片，每天最多可上传 1000 张图片； 返回的图片 URL 仅能用于图文消息正文中的图片展示，或者给客户发送欢迎语等—— 若用于非企业微信环境下的页面，图片将被屏蔽。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90254
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90392
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96485

### 4. `GetHdVoiceMediaAsync` — 获取高清语音素材

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/media/get/jssdk`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mediaId` | Query `media_id` | `string` | 通过 JSSDK 的 uploadVoice 接口上传的语音文件 id（官方必填）。 |
| `range` | Header | `string?` | HTTP `Range` 请求头（可选，如 `bytes=0-1023`）。 |
- **返回**：`byte[]?` — 语音文件二进制内容（错误时由 errcode 判定器抛 `WechatWorkException`）。

**补充说明**

- 获取 JSSDK uploadVoice 接口上传的语音文件内容。成功时与普通 HTTP 下载相同 （Content-Type: voice/speex 等头部 + 二进制音频内容），失败时返回 JSON（如 errcode 40007 invalid media_id）。
- 官方限制：该素材格式为 speex、16K 采样率，比普通临时素材接口（amr、8K）更清晰， 适合用作语音识别等对音质要求较高的业务，转码需使用 Speex 官方解码库并结合微信提供的解码库； 仅企业微信 2.4 及以上版本支持，暂时不支持鸿蒙系统上传的语音。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90255
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90391
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96487

### 5. `UploadMediaByUrlAsync` — 生成异步上传任务

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/media/upload_by_url`

- **请求体**：`UploadMediaByUrlRequest` — 异步上传任务请求体（`UploadMediaByUrlRequest`：scene / type / filename / url / md5）。
- **返回**：`UploadMediaByUrlResponse` — 异步上传任务 id（jobid，60 分钟内有效）。

**补充说明**

- 提交一个从 CDN URL 拉取文件并转存为临时素材的异步任务（适合超过 20M 的大文件场景）。 任务完成时官方会向应用回调地址推送 `upload_media_job_finish` 事件（仅携带 JobId，不含具体结果）， 结果须凭 `UploadMediaByUrlAsync` 返回的 jobid 调用 `GetUploadMediaByUrlResultAsync` 查询。
- 官方限制：须配置「客户联系」可调用接口的应用并具有客户联系权限； 所有文件必须大于 5 个字节，视频与普通文件上限 200MB，视频仅支持 MP4 格式（图片与语音暂不支持）； url 要求支持 Range 分块下载（腾讯云 COS 链接需设为「公有读」权限）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96219
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97126
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96488

### 6. `GetUploadMediaByUrlResultAsync` — 查询异步上传任务结果

- **声明接口**：[IWechatWorkMediaService](./IWechatWorkMediaService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/media/get_upload_by_url_result`

- **请求体**：`GetUploadMediaByUrlResultRequest` — 查询请求体（`GetUploadMediaByUrlResultRequest`：jobid）。
- **返回**：`GetUploadMediaByUrlResultResponse` — 任务状态（status：1 - 处理中，2 - 完成，3 - 异常失败）与执行明细（`MediaUploadJobDetail`）。

**补充说明**

- 凭 `UploadMediaByUrlAsync` 返回的 jobid 查询异步上传任务的执行结果。
- 官方限制：jobid 最长 128 字节、60 分钟内有效；任务失败（status = 3）时 detail.errcode 返回非 0， 常见错误码：830001 - url 非法、830003 - url 下载数据失败、45001 - 文件大小超过限制（5 字节 ~ 200M）、 301019 - 文件 MD5 不匹配。
- 注意：本接口返回的 media_id 与普通「上传临时素材」的使用场景不通用， 目前适用于「获取临时素材」（超 20M 须 Range 分块下载）与入群欢迎语素材管理（scene = 1）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96219
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/97126
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96488

### 7. `UploadServiceMediaAsync` — 服务商上传临时素材

- **声明接口**：[IWechatWorkThirdPartyServiceMediaService](./IWechatWorkThirdPartyServiceMediaService.cs)
- **官方契约**：`POST /cgi-bin/service/media/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `type` | Query `type` | `string` | 媒体文件类型（官方必填）：image - 图片，voice - 语音，video - 视频，file - 普通文件。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（文件字段名须为 `media`，由调用方构建<br>`IFormContent` 实现，含 filename / filelength / content-type 信息）。<br>须标 `[MultipartForm]` —— 生成器 3.0.1 对 `[FormContent]` 发射的<br>`GetFormDataContentAsync` 调用在运行时接口上不存在（CS1061），<br>`[MultipartForm]` 才走 `ToHttpContentAsync` 通路。 |
| `attachmentType` | Query `attachment_type` | `int?` | 附件类型（官方选填）：用于特定场景（如 3 - 收银台），普通场景不指定。 |
- **返回**：`UploadMediaResponse` — 媒体文件类型（type）、素材 id（media_id，三天有效）与上传时间戳（created_at）。

**补充说明**

- 服务商以自身 provider_access_token 代上传临时素材（multipart/form-data，文件标识名必须为 `media`， 须包含 filename、filelength、content-type 等信息；filename 不能以正斜线开始，不能包含反斜线与冒号）。
- 官方限制：所有文件必须大于 5 个字节；图片（image）2MB、仅支持 JPG/PNG； 语音（voice）2MB、播放长度不超过 60s、仅支持 AMR 格式；视频（video）10MB、仅支持 MP4 格式；普通文件（file）20MB。
- 返回的 media_id 仅三天内有效，同一企业内应用之间可以共享。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99310

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Media`](../../../Mud.Wechat.Work/DataModels/Media/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Media`。

AOT 源生成上下文：[`MediaJsonContext.g.cs`](./../../DataModels/Generated/MediaJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `UploadMediaResponse` | 应答 | [Media/UploadMediaResponse.cs](../../DataModels/Media/UploadMediaResponse.cs) | `type`, `media_id`, `created_at` |
| `UploadImageResponse` | 应答 | [Media/UploadImageResponse.cs](../../DataModels/Media/UploadImageResponse.cs) | `url` |
| `UploadMediaByUrlResponse` | 应答 | [Media/UploadMediaByUrlResponse.cs](../../DataModels/Media/UploadMediaByUrlResponse.cs) | `jobid` |
| `UploadMediaByUrlRequest` | 请求体 | [Media/RequestModel/UploadMediaByUrlRequest.cs](../../DataModels/Media/RequestModel/UploadMediaByUrlRequest.cs) | `scene`, `type`, `filename`, `url`, `md5` |
| `GetUploadMediaByUrlResultResponse` | 应答 | [Media/GetUploadMediaByUrlResultResponse.cs](../../DataModels/Media/GetUploadMediaByUrlResultResponse.cs) | `status`, `detail` |
| `GetUploadMediaByUrlResultRequest` | 请求体 | [Media/RequestModel/GetUploadMediaByUrlResultRequest.cs](../../DataModels/Media/RequestModel/GetUploadMediaByUrlResultRequest.cs) | `jobid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Media;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMediaApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalMediaService>();
var ct = CancellationToken.None;
var response = await service.GetUploadMediaByUrlResultAsync(new GetUploadMediaByUrlResultRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/media/get_upload_by_url_result");
// 占位值仅用于编译示意；业务值须按 IWechatWorkInternalMediaService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetUploadMediaByUrlResultAsync`）；请求体 `GetUploadMediaByUrlResultRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkInternalMediaService](./IWechatWorkInternalMediaService.cs)、[IWechatWorkMediaService](./IWechatWorkMediaService.cs)、[IWechatWorkProviderMediaService](./IWechatWorkProviderMediaService.cs)、[IWechatWorkServiceMediaService](./IWechatWorkServiceMediaService.cs)、[IWechatWorkThirdPartyMediaService](./IWechatWorkThirdPartyMediaService.cs)、[IWechatWorkThirdPartyServiceMediaService](./IWechatWorkThirdPartyServiceMediaService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatMediaContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMediaContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `MediaEndpoints_ShouldMatchOfficialRoutes`、`ServiceMediaEndpoints_ShouldMatchOfficialRoutes`、`MediaInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMediaRegistry`、`ServiceMediaFamily_ShouldHaveExactlyThirdPartyChildAndAbstractParent`、`MediaTokenBinding_ShouldBeAccessAndProviderTokenInjectedViaQuery`、`MediaDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Media`](../../../Mud.Wechat.Work/DataModels/Media/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatMediaContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

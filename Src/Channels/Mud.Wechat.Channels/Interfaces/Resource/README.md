# Resource 域 · 资源管理

微信小店 / 视频号（channels 生态）「资源管理」域 SDK（8 端点：图片上传（URL / 二进制双传输形态） +
图片上传 V2（同双传输形态） + media_id 取数 + 资质图片上传 + 视频分块上传四步流程）。 接口：[IChannelsResourceService](./IChannelsResourceService.cs)；注册组 `Resource`；模块枚举 `ChannelsModule.Resource`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsResourceContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsResourceContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/Resource`](../../../Mud.Wechat.Channels/Resource/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsResourceService = 8；按 HTTP 特性实计 **10** 支。

> **计数漂移（留档）**：`IChannelsResourceService` 注释自述 8 支、实计 10 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsResourceService` | `UploadImageAsync` | POST | `/shop/ec/basics/img/upload` | `AccessToken`（Query），Query `access_token` | 上传图片——图片 URL 模式（`img_upload`，`upload_type=1`，POST JSON 包 `img_url`）。 |
| 2 | `IChannelsResourceService` | `UploadImageByBinaryAsync` | POST | `/shop/ec/basics/img/upload` | `AccessToken`（Query），Query `access_token` | 上传图片——二进制流模式（`img_upload`，`upload_type=0`，multipart/form-data 文件字段 `media`）。 |
| 3 | `IChannelsResourceService` | `UploadImageV2Async` | POST | `/shop/ec/basics/img/uploadv2` | `AccessToken`（Query），Query `access_token` | 上传图片 V2——图片 URL 模式（`img_uploadv2`，`upload_type=1`，POST JSON 包 `img_url`）。 |
| 4 | `IChannelsResourceService` | `UploadImageV2ByBinaryAsync` | POST | `/shop/ec/basics/img/uploadv2` | `AccessToken`（Query），Query `access_token` | 上传图片 V2——二进制流模式（`img_uploadv2`，`upload_type=0`，multipart/form-data 文件字段 `media`）。 |
| 5 | `IChannelsResourceService` | `GetDataByMediaIdAsync` | GET | `/channels/ec/basics/media/get` | `AccessToken`（Query），Query `access_token` | 通过 media_id 获取数据（`getdatabymediaid`；成功时响应为二进制文件内容，报错时返回 JSON）。 |
| 6 | `IChannelsResourceService` | `UploadQualificationAsync` | POST | `/shop/ec/basics/qualification/upload` | `AccessToken`（Query），Query `access_token` | 上传资质图片（`qualificationupload`，multipart/form-data 文件字段 `media`）。 |
| 7 | `IChannelsResourceService` | `InitVideoUploadAsync` | POST | `/shop/ec/basics/video/initupload` | `AccessToken`（Query），Query `access_token` | 申请上传视频（`video_initupload`，视频分块上传第一步：分配 `video_upload_key`）。 |
| 8 | `IChannelsResourceService` | `UploadVideoPartAsync` | POST | `/shop/ec/basics/video/uploadpart` | `AccessToken`（Query），Query `access_token` | 上传视频数据分块（`video_uploadpart`，视频分块上传第二步；分块大小 1M ~ 2MB）。 |
| 9 | `IChannelsResourceService` | `FinishVideoUploadAsync` | POST | `/shop/ec/basics/video/finishupload` | `AccessToken`（Query），Query `access_token` | 完成视频上传（`video_finishupload`，视频分块上传第三步：校验分片连续且 SHA 匹配）。 |
| 10 | `IChannelsResourceService` | `GetVideoPlayInfoAsync` | GET | `/shop/ec/basics/video/getplayinfo` | `AccessToken`（Query），Query `access_token` | 获取上传后的视频临时 URL（`video_getplayinfo`，视频分块上传第四步：轮询获取）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**同路由双传输形态（官方契约，勿合并为单方法）**：图片上传官方页给出两种按 `upload_type`
互斥的请求体传输形态——`upload_type=1` 图片 URL 模式（POST JSON 包 `img_url`）与
`upload_type=0` 二进制流模式（multipart/form-data 文件字段 `media`）。SDK 以
`UploadImageAsync`（JSON 通道）与
`UploadImageByBinaryAsync`（multipart 通道）两个方法承载
（对齐 Work 线 message/send「同路由多方法」与公众号线素材域 `[MultipartForm]` 先例），
共用同一响应 DTO（裁决理由见守卫 `ChannelsResourceContractGuards` CR5）。

**图片上传 V2（`img_uploadv2`，官方 2026-10-10 新增）与 V1 的契约差异**：V2 以 URL 参数
`scene_type`（场景类型，必填）替代 V1 的 `resp_type`；响应只有 `img_info.img_url`
一种形态（不返回 `media_id`/`pay_media_id`）；二进制流模式 V2 **不要求** V1 的
`height`/`width` URL 参数。图片大小与格式限制随 `scene_type` 变化（170 售后相关
≤ 10MB，支持 JPEG/PNG/BMP/GIF、不支持 WEBP/SVG）。SDK 同样以「同路由双方法」承载两种传输形态。

**media_id 与图片链接双返回形态（官方 `resp_type`）**：`resp_type=0` 返回
`media_id`/`pay_media_id`（供其他接口引用）；`resp_type=1` 返回形如
`mmecimage.cn/p/{appid}/{imgKey}` 的永久有效图片链接（官方原文「商品信息相关图片请务必使用此参数得到链接」）。

**media/get 二进制响应**：成功时返回二进制数据、报错时返回 JSON 错误信息（官方原文
「请根据回包数据格式分别处理」）——SDK 以 `byte[]?` 原始响应直读（对齐 Work 线 media/get 形态），
失败经组件判错器抛 `WechatChannelsException`。

**视频分块上传四步流程**：申请上传（分配 `video_upload_key`）→ 分块上传（1M~2MB，
可乱序并发）→ 完成上传（分片连续性 + SHA 校验）→ 轮询临时 URL（失效后仍不影响提交）。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）第三方平台代调用所属权限集 id 为 `85`、`129`、`131`、`192`。
- （出现 10 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `129`、`131`、`192`。

### 1. `UploadImageAsync` — 上传图片——图片 URL 模式（`img_upload`，`upload_type=1`，POST JSON 包 `img_url`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/img/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `uploadType` | Query `upload_type` | `int` | 上传类型（官方 `upload_type`，URL 参数，必填）：本方法为图片 URL 模式，传 `Url`（1）。 |
| `respType` | Query `resp_type` | `int` | 返回数据类型（官方 `resp_type`，URL 参数，必填），见 `ChannelsResourceImgRespTypes`；商品信息相关图片官方要求务必用 `Url`（1）取链接。 |

- **请求体**：`ChannelsResourceImgUploadRequest` — 上传请求（`img_url` 图片 URL，官方 `upload_type=1` 时必填；不支持 301/302 跳转）。
- **返回**：`ChannelsResourceImgUploadResponse` — 图片信息（`pic_file`：`media_id` / `pay_media_id` / `img_url`，按 `resp_type` 返回对应形态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_upload
- 官方业务限制：①接口频率限制 **5000/min**；②返回的图片链接有访问频率限制（appid 维度 10000/min、 / imgKey 维度 100/min，超出返回 404）——**请勿将该图片链接用于用户端（访问量较大）展示**； / ③仅支持 bmp、jpg(jpeg)、png、webp 格式（`upload_type=1` 时按 content-type 判断，不支持 301/302 跳转）； / ④大小限制：resp_type=0 为 2MB、resp_type=1 为 10MB。
- 官方错误码：`10020055`（参数有误）/ `10020056`（图片格式不合法）/ `10020060`（文件大小超出限制）/ / `10020061`（图片 URL 非法，如传入 mmecimage.cn/p/ 前缀的图片）/ `10020193`（不支持拉取原图 301/302 跳转）等。

### 2. `UploadImageByBinaryAsync` — 上传图片——二进制流模式（`img_upload`，`upload_type=0`，multipart/form-data 文件字段 `media`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/img/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `uploadType` | Query `upload_type` | `int` | 上传类型（官方 `upload_type`，URL 参数，必填）：本方法为二进制流模式，传 `BinaryStream`（0）。 |
| `respType` | Query `resp_type` | `int` | 返回数据类型（官方 `resp_type`，URL 参数，必填），见 `ChannelsResourceImgRespTypes`。 |
| `height` | Query `height` | `int` | 图片的高（官方 `height`，URL 参数，单位：像素；官方 `upload_type=0` 时必填——即本方法的必传参数）。 |
| `width` | Query `width` | `int` | 图片的宽（官方 `width`，URL 参数，单位：像素；官方 `upload_type=0` 时必填——即本方法的必传参数）。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（官方表单字段名必须为 `media`，图片文件 buffer；<br>由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`——生成器对该标注走<br>`ToHttpContentAsync` 通路；大小限制按 `resp_type`：0 为 2MB、1 为 10MB）。 |
- **返回**：`ChannelsResourceImgUploadResponse` — 图片信息（`pic_file`：`media_id` / `pay_media_id` / `img_url`，按 `resp_type` 返回对应形态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_upload
- 官方业务限制：①接口频率限制 **5000/min**；②返回的图片链接有访问频率限制（appid 维度 10000/min、 / imgKey 维度 100/min，超出返回 404）——**请勿将该图片链接用于用户端（访问量较大）展示**； / ③仅支持 bmp、jpg(jpeg)、png、webp 格式。
- 官方错误码：`10020056`（图片格式不合法）/ `10020059`（图片为空）/ `10020060`（文件大小超出限制）等。

### 3. `UploadImageV2Async` — 上传图片 V2——图片 URL 模式（`img_uploadv2`，`upload_type=1`，POST JSON 包 `img_url`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/img/uploadv2`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `sceneType` | Query `scene_type` | `int` | 场景类型（官方 `scene_type`，URL 参数，必填），取值见 `ChannelsResourceImgUploadV2SceneTypes`；不同场景的图片大小与格式限制不同。 |
| `uploadType` | Query `upload_type` | `int` | 上传类型（官方 `upload_type`，URL 参数，必填）：本方法为图片 URL 模式，传 `Url`（1）。 |

- **请求体**：`ChannelsResourceImgUploadV2Request` — 上传请求（`img_url` 图片 URL，官方 `upload_type=1` 时必填；不支持 301/302 跳转）。
- **返回**：`ChannelsResourceImgUploadV2Response` — 图片信息（`img_info.img_url` 图片链接，支持图片处理参数拼接）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_uploadv2
- 官方业务限制：①接口频率限制 **5000/min**；②**本接口上传的图片不适用于所有场景**——仅当其他接口 / 明确要求使用本接口上传图片时方可使用（官方原文）；③接口返回的图片链接支持图片处理 / （缩放 / 裁剪 / 压缩 / 水印等），「图片处理」文档内的 `download_url` 即为该链接； / ④大小与格式限制按 `scene_type`：170（售后相关）≤ 10MB，支持 JPEG/PNG/BMP/GIF、不支持 WEBP/SVG / （官方 2026-10-12 变更日志：170 的 size 限制由 5MB 调整为 10MB）。
- 与 V1（`UploadImageAsync`）的差异：V2 无 `resp_type` / （响应只有图片链接、不返回 `media_id`），须显式传 `scene_type`。
- 官方错误码：`10020055`（参数有误）/ `10020056`（图片格式不合法）/ `10020057`（获取原图超过 2s 超时）/ / `10020058`（上传图片失败，请重试）/ `10020060`（文件大小超出限制）/ `10020193`（不支持拉取原图 301/302 跳转）/ / `10020253`（原图 url 打开返回 404）/ `10020254`（原图 url 带了 `//`，无法支持该格式 url）/ / `10020331`（获取原图时返回的 header 没有 Content-Length）等。
- 官方适用范围：微信小店 ✔、小店供货商 ✔；其他账号类型不可调用。本接口不支持云调用； / 第三方平台代调用所属权限集 id 为 `129`、`131`、`192`。

### 4. `UploadImageV2ByBinaryAsync` — 上传图片 V2——二进制流模式（`img_uploadv2`，`upload_type=0`，multipart/form-data 文件字段 `media`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/img/uploadv2`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `sceneType` | Query `scene_type` | `int` | 场景类型（官方 `scene_type`，URL 参数，必填），取值见 `ChannelsResourceImgUploadV2SceneTypes`。 |
| `uploadType` | Query `upload_type` | `int` | 上传类型（官方 `upload_type`，URL 参数，必填）：本方法为二进制流模式，传 `BinaryStream`（0）。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（官方表单字段名必须为 `media`，图片文件 buffer；<br>由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`；大小限制按 `scene_type`：170 为 10MB）。 |
- **返回**：`ChannelsResourceImgUploadV2Response` — 图片信息（`img_info.img_url` 图片链接，支持图片处理参数拼接）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_img_uploadv2
- 官方业务限制：①接口频率限制 **5000/min**；②**本接口上传的图片不适用于所有场景**——仅当其他接口 / 明确要求使用本接口上传图片时方可使用（官方原文）；③须带 HTTP 头 `Content-Type: multipart/form-data`。
- **与 V1 二进制模式的差异（照录官方，勿「对齐」补参）**：V1 `img_upload` 在 `upload_type=0` / 时要求 URL 参数 `height`/`width`，而 V2 参数表**无**该两项——本方法签名因此不声明高度与宽度。 / 官方示例把 `scene_type` 写成 `0`，与枚举表（仅 170）及参数表「必填」矛盾，属示例笔误， / SDK 按参数表与枚举表建模（裁决见守卫 CR5）。
- 官方错误码：`10020056`（图片格式不合法）/ `10020058`（上传图片失败，请重试）/ `10020059`（图片为空）/ `10020060`（文件大小超出限制）。
- 官方适用范围：微信小店 ✔、小店供货商 ✔；其他账号类型不可调用。本接口不支持云调用； / 第三方平台代调用所属权限集 id 为 `129`、`131`、`192`。

### 5. `GetDataByMediaIdAsync` — 通过 media_id 获取数据（`getdatabymediaid`；成功时响应为二进制文件内容，报错时返回 JSON）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`GET /channels/ec/basics/media/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `mediaId` | Query `media_id` | `string` | 媒体 id（官方 `media_id`，URL 参数，必填；来源为售后单、纠纷单等接口返回的媒体 id）。 |
- **返回**：`byte[]?` — 媒体文件二进制内容（失败时由 errcode 判定器抛 `WechatChannelsException`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_getdatabymediaid
- 官方契约：**GET** + URL 参数 `media_id`；官方原文「成功时返回二进制数据，报错情况下返回 / json 格式的错误信息，请根据回包数据格式分别处理」——SDK 以 `byte[]?` 直读原始响应， / 失败经组件判错器统一抛错。
- 官方业务限制：本接口无特殊注意事项；无特殊错误码（走通用错误码）。

### 6. `UploadQualificationAsync` — 上传资质图片（`qualificationupload`，multipart/form-data 文件字段 `media`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/qualification/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（官方表单字段名必须为 `media`，资质图片文件 file；<br>由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`）。 |
- **返回**：`ChannelsResourceQualificationUploadResponse` — 文件信息（`data.file_id` 资质图片文件 id）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_qualificationupload
- 官方业务限制：①图片上传大小限制 **2MB**；②须带 HTTP 头 `Content-Type: multipart/form-data`， / POST 包请求参数需为图片二进制流、以键值对形式传参。
- 官方错误码：`10020059`（图片为空）/ `10020060`（文件大小超出限制）。

### 7. `InitVideoUploadAsync` — 申请上传视频（`video_initupload`，视频分块上传第一步：分配 `video_upload_key`）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/video/initupload`

- **请求体**：`ChannelsResourceVideoInitUploadRequest` — 申请请求（`scene_type`/`file_type`/`file_size` 均官方必填）。
- **返回**：`ChannelsResourceVideoInitUploadResponse` — 本次上传流程关联键（`data.video_upload_key`，用于后续分块上传 / 完成上传 / 获取临时 URL）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_initupload
- 官方业务限制：①**本接口上传的视频不适用于所有场景**——仅当其他接口明确要求使用本接口上传视频时方可使用 / （官方原文）；②场景限制：售后相关视频（scene_type=173）≤ 20MB、无时长限制；商品视频（scene_type=162） / ≤ 500MB 且 ≤ 180 秒。
- 官方业务限制：本接口无特殊错误码（走通用错误码）；场景取值见 `ChannelsResourceVideoSceneTypes`。

### 8. `UploadVideoPartAsync` — 上传视频数据分块（`video_uploadpart`，视频分块上传第二步；分块大小 1M ~ 2MB）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/video/uploadpart`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `videoUploadKey` | Query `video_upload_key` | `string` | 上传流程关联键（官方 `video_upload_key`，URL 参数，必填；通过申请上传视频接口获取）。 |
| `partNum` | Query `partnum` | `int` | 分块编号（官方 `partnum`，URL 参数，必填；最大 10000，从 1 开始）。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（官方表单字段名必须为 `media`，视频文件分块 buffer；<br>由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`）。 |
- **返回**：`ChannelsResourceVideoUploadPartResponse` — 服务端计算的分片 SHA1（`data.part_sha`，完成视频上传时须传回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_uploadpart
- 官方业务限制：①分块大小目前支持 **1M ~ 2MB**，除最后一个分块外其他分块不得小于 1M（否则调用 / finishupload 返回 `EntityTooSmall`）；②单个分块必须以长连接上传、**不支持断点续传**； / ③分块之间**可乱序并发上传**（重入覆盖），可并发上传多个分块以提高上传速度；④分块编号从 1 开始、最大 10000。
- 官方错误码：`10020319`（文件为空）。

### 9. `FinishVideoUploadAsync` — 完成视频上传（`video_finishupload`，视频分块上传第三步：校验分片连续且 SHA 匹配）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`POST /shop/ec/basics/video/finishupload`

- **请求体**：`ChannelsResourceVideoFinishUploadRequest` — 完成请求（`video_upload_key` 与 `finish_parts` 均官方必填；分片列表按分块偏移从小到大排列且编号连续）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（官方响应无业务字段，故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_finishupload
- 官方业务限制：本接口无特殊注意事项；官方错误码 `10020342`（上传分块有误——除最后一个分块， / 其他分块大小不得小于 1M）。

### 10. `GetVideoPlayInfoAsync` — 获取上传后的视频临时 URL（`video_getplayinfo`，视频分块上传第四步：轮询获取）。

- **声明接口**：[IChannelsResourceService](./IChannelsResourceService.cs)
- **官方契约**：`GET /shop/ec/basics/video/getplayinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `videoUploadKey` | Query `video_upload_key` | `string` | 上传流程关联键（官方 `video_upload_key`，URL 参数，必填；通过申请上传视频接口获取）。 |
- **返回**：`ChannelsResourceVideoPlayInfoResponse` — 视频临时 URL（`data.url`；临时性、会失效，但官方说明 URL 失效后仍不影响将其提交到相关接口）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/resource/api_video_getplayinfo
- 官方契约：**GET** + URL 参数 `video_upload_key`；完成上传后**轮询**本接口获取视频临时 URL / （视频转码期间查询会返回 10020347，请稍后再试）。
- 官方错误码：`10020324`（视频不合法——可能超出文件大小或时长限制）/ `10020347`（获取视频失败——视频还在转码中）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/Resource`](../../../Mud.Wechat.Channels/Resource/) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `Resource`。

AOT 源生成上下文：[`ResourceJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/ResourceJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsResourceImgUploadResponse` | 应答 | [Resource/ChannelsResourceImgUploadResponse.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceImgUploadResponse.cs) | `pic_file` |
| `ChannelsResourceImgUploadRequest` | 请求体 | [Resource/ChannelsResourceImgUploadRequest.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceImgUploadRequest.cs) | `img_url` |
| `ChannelsResourceImgUploadV2Response` | 应答 | [Resource/ChannelsResourceImgUploadV2Response.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceImgUploadV2Response.cs) | `img_info` |
| `ChannelsResourceImgUploadV2Request` | 请求体 | [Resource/ChannelsResourceImgUploadV2Request.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceImgUploadV2Request.cs) | `img_url` |
| `ChannelsResourceQualificationUploadResponse` | 应答 | [Resource/ChannelsResourceQualificationUploadResponse.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceQualificationUploadResponse.cs) | `data` |
| `ChannelsResourceVideoInitUploadResponse` | 应答 | [Resource/ChannelsResourceVideoInitUploadResponse.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceVideoInitUploadResponse.cs) | `data` |
| `ChannelsResourceVideoInitUploadRequest` | 请求体 | [Resource/ChannelsResourceVideoInitUploadRequest.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceVideoInitUploadRequest.cs) | `scene_type`, `file_type`, `file_size` |
| `ChannelsResourceVideoUploadPartResponse` | 应答 | [Resource/ChannelsResourceVideoUploadPartResponse.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceVideoUploadPartResponse.cs) | `data` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsResourceVideoFinishUploadRequest` | 请求体 | [Resource/ChannelsResourceVideoFinishUploadRequest.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceVideoFinishUploadRequest.cs) | `video_upload_key`, `finish_parts` |
| `ChannelsResourceVideoPlayInfoResponse` | 应答 | [Resource/ChannelsResourceVideoPlayInfoResponse.cs](../../../Mud.Wechat.Channels/Resource/ChannelsResourceVideoPlayInfoResponse.cs) | `data` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Resource;
using Mud.Wechat.Channels.DataModels;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddResourceApi());

var service = app.Services.GetRequiredService<IChannelsResourceService>();
var ct = CancellationToken.None;
var response = await service.FinishVideoUploadAsync(new ChannelsResourceVideoFinishUploadRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/shop/ec/basics/video/finishupload");
// 占位值仅用于编译示意；业务值须按 IChannelsResourceService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`FinishVideoUploadAsync`）；请求体 `ChannelsResourceVideoFinishUploadRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IChannelsResourceService`：注释 8 / 实计 10 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsResourceService](./IChannelsResourceService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsResourceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsResourceContractGuards.cs) |
| 守卫中的代表性测试方法 | `ResourceEndpoints_ShouldMatchOfficialRoutes`、`ResourceEndpoints_ShouldHonorOfficialBodyRules`、`ResourceInterface_ShouldBindChannelsToken_AndRegisterUnderResourceGroup`、`ResourceDataModels_ShouldBeRegisteredInJsonContext`、`ResourceDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`ResourceEnumConstants_ShouldLockOfficialLiteralValues`、`StaleChannelsBasicsPrefix_ShouldHaveZeroDeclaration` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/Resource`](../../../Mud.Wechat.Channels/Resource/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsResourceContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

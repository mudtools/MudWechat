# SmartApi 域 · 智能接口

微信公众号 / 服务号「智能接口」域 SDK（12 端点：AI 开放接口 3 + OCR 识别 7 + 图像处理 2）。 接口：[IMpSmartApiService](./IMpSmartApiService.cs)；注册组 `SmartApi`；模块枚举 `MpModule.SmartApi`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpSmartApiContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSmartApiContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/DataModels/SmartApi`](../../DataModels/SmartApi)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpSmartApiService = 12；按 HTTP 特性实计 **23** 支。

> **计数漂移（留档）**：`IMpSmartApiService` 注释自述 12 支、实计 23 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpSmartApiService` | `UploadVoiceForRecognitionAsync` | POST | `/cgi-bin/media/voice/addvoicetorecofortext` | `AccessToken`（Query），Query `access_token` | 上传语音文件（进行转文字识别）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_addvo… |
| 2 | `IMpSmartApiService` | `QueryVoiceRecognitionResultAsync` | POST | `/cgi-bin/media/voice/queryrecoresultfortext` | `AccessToken`（Query），Query `access_token` | 获取语音识别结果。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_queryrecores… |
| 3 | `IMpSmartApiService` | `TranslateContentAsync` | POST | `/cgi-bin/media/voice/translatecontent` | `AccessToken`（Query），Query `access_token` | 微信翻译（文本内容翻译）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_translat… |
| 4 | `IMpSmartApiService` | `IdCardOcrByUploadAsync` | POST | `/cv/ocr/idcard` | `AccessToken`（Query），Query `access_token` | 身份证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idca… |
| 5 | `IMpSmartApiService` | `IdCardOcrByUrlAsync` | POST | `/cv/ocr/idcard` | `AccessToken`（Query），Query `access_token` | 身份证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idc… |
| 6 | `IMpSmartApiService` | `BankCardOcrByUploadAsync` | POST | `/cv/ocr/bankcard` | `AccessToken`（Query），Query `access_token` | 银行卡识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bank… |
| 7 | `IMpSmartApiService` | `BankCardOcrByUrlAsync` | POST | `/cv/ocr/bankcard` | `AccessToken`（Query），Query `access_token` | 银行卡识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_ban… |
| 8 | `IMpSmartApiService` | `DrivingOcrByUploadAsync` | POST | `/cv/ocr/driving` | `AccessToken`（Query），Query `access_token` | 行驶证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_driv… |
| 9 | `IMpSmartApiService` | `DrivingOcrByUrlAsync` | POST | `/cv/ocr/driving` | `AccessToken`（Query），Query `access_token` | 行驶证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_dri… |
| 10 | `IMpSmartApiService` | `DrivingLicenseOcrByUploadAsync` | POST | `/cv/ocr/drivinglicense` | `AccessToken`（Query），Query `access_token` | 驾驶证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_driv… |
| 11 | `IMpSmartApiService` | `DrivingLicenseOcrByUrlAsync` | POST | `/cv/ocr/drivinglicense` | `AccessToken`（Query），Query `access_token` | 驾驶证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_dri… |
| 12 | `IMpSmartApiService` | `BizLicenseOcrByUploadAsync` | POST | `/cv/ocr/bizlicense` | `AccessToken`（Query），Query `access_token` | 营业执照识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_biz… |
| 13 | `IMpSmartApiService` | `BizLicenseOcrByUrlAsync` | POST | `/cv/ocr/bizlicense` | `AccessToken`（Query），Query `access_token` | 营业执照识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bi… |
| 14 | `IMpSmartApiService` | `CommOcrByUploadAsync` | POST | `/cv/ocr/comm` | `AccessToken`（Query），Query `access_token` | 通用印刷体识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_co… |
| 15 | `IMpSmartApiService` | `CommOcrByUrlAsync` | POST | `/cv/ocr/comm` | `AccessToken`（Query），Query `access_token` | 通用印刷体识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_c… |
| 16 | `IMpSmartApiService` | `MenuOcrByUploadAsync` | POST | `/cv/ocr/menu` | `AccessToken`（Query），Query `access_token` | 菜单识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menuo… |
| 17 | `IMpSmartApiService` | `MenuOcrByUrlAsync` | POST | `/cv/ocr/menu` | `AccessToken`（Query），Query `access_token` | 菜单识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menu… |
| 18 | `IMpSmartApiService` | `AiCropByUploadAsync` | POST | `/cv/img/aicrop` | `AccessToken`（Query），Query `access_token` | 图片智能裁剪（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_i… |
| 19 | `IMpSmartApiService` | `AiCropByUrlAsync` | POST | `/cv/img/aicrop` | `AccessToken`（Query），Query `access_token` | 图片智能裁剪（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_… |
| 20 | `IMpSmartApiService` | `QrcodeRecognitionByUploadAsync` | POST | `/cv/img/qrcode` | `AccessToken`（Query），Query `access_token` | 二维码/条码识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api… |
| 21 | `IMpSmartApiService` | `QrcodeRecognitionByUrlAsync` | POST | `/cv/img/qrcode` | `AccessToken`（Query），Query `access_token` | 二维码/条码识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/ap… |
| 22 | `IMpSmartApiService` | `PlateNumberOcrByUploadAsync` | POST | `/cv/ocr/platenum` | `AccessToken`（Query），Query `access_token` | 图片高清化（超分辨率）——form-data 实时上传形态。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intell… |
| 23 | `IMpSmartApiService` | `PlateNumberOcrByUrlAsync` | POST | `/cv/ocr/platenum` | `AccessToken`（Query），Query `access_token` | 车牌识别（OCR）——URL 传送形态。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Inte… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/service/api/
→ 智能接口（AI 开放接口 / OCR 识别 / 图像处理）。深链均为**逐页核验过的真实页面**（2026-10-07）。

**三种路径前缀照实建模于同一域（前缀不入命名空间）**：AI 开放接口在
`/cgi-bin/media/voice/*`、OCR 在 `/cv/ocr/*`、图像处理在 `/cv/img/*`
——官方同属「智能接口」分组且请求形态同质（multipart 上传 + Query 传参 + 同一错误码面），
沿用订阅通知域「双前缀单域」的既存先例，不按前缀拆域。

**双调用形态 = 双方法（官方契约，勿合并）**：OCR 7 + 图像处理 2 共 9 个端点的官方请求为
**二选一互斥形态**——form-data 实时上传 `img`，**或** Query 传 `img_url`
（官方原文「传这个则不用传另一个」，由微信后台下载图片识别）。SDK 为每个端点建
`*ByUploadAsync`（multipart）与 `*ByUrlAsync`（Query）两个方法：
单方法「可选 `IFormContent?`」形态被否决——生成管线对 `[MultipartForm]` 参数无条件生成
`HttpRequest.Content = formData`，传 `null` 将运行时 NRE。

**域级业务约束（逐页核验，勿弱化）**：

- **账号适用范围**：AI 开放接口三端点官方表为「小程序 / 公众号 / 服务号 / 小游戏 / 移动应用 —— 全部 ✔」；
OCR 与图像处理九端点为「小程序 ✔ / 公众号 **仅认证** / 服务号 **仅认证**」
（SDK 不做账号类型本地闸，由官方错误码表达）。
- **频率上限（不编造数值）**：OCR 七端点官方原文「次数限制为 **100 次/天**」，
其中**菜单识别页无频率章节且注明「尚未接入服务平台，暂时不支持付费购买」**；
图像处理两端点与 AI 开放接口三端点官方全页**无频率数值**。
- **图片限制**：官方原文「文件大小限制：**小于 2M**」；OCR 页错误码 `101002` 另引
「`resp_type = 0: 2MB`，`resp_type = 1: 10MB`」但全文未定义 `resp_type`
——两处矛盾照录，SDK 不做本地大小拦截（由官方错误码表达）。
- **第三方平台**：OCR 与图像处理九端点官方明示支持代商家调用（**权限集 id 117**）；
AI 开放接口三端点官方明示**不支持**云调用与第三方平台调用 —— 按 M0-R3 裁决只在 XML 记录，不扩实现面。
- **不做业务编排**：AI 语音「上传 → 10s 内轮询」的两步语义只写进 XML，SDK 不封装轮询策略。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。`img_url` / `format` /
`voice_id` / `lang` / `lfrom` / `lto` / `ratios` 均为**非凭据** Query 参数。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 23 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 7 次）官方契约：**POST multipart/form-data**，Query 携带 `access_token`；表单文件字段名 `img`。
- （出现 9 次）官方契约：**POST**（无请求体），Query 携带 `img_url` 与 `access_token`。

### 1. `UploadVoiceForRecognitionAsync` — 上传语音文件（进行转文字识别）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_addvoicetorecofortext.html
（官方接口英文名 `addVoiceTorecoForText`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cgi-bin/media/voice/addvoicetorecofortext`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `format` | Query `format` | `string` | 语音文件格式（**必填**；官方原文「只支持 mp3、16k、单声道、最大 1M」，见 `MpVoiceFileFormats`）。 |
| `voiceId` | Query `voice_id` | `string` | 语音唯一标识（**必填**；后续 `queryrecoresultfortext` 以同一 id 查询）。 |
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `media`**；<br>由调用方构建 `IFormContent` 实现，须标 `[MultipartForm]`）。 |
| `lang` | Query `lang` | `string?` | 语言（官方 `lang`，可选，`zh_CN` 或 `en_US`，默认中文；见 `MpVoiceLanguages`）。 |
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST multipart/form-data**，**Query** 携带 `format`（必填）/ `voice_id`（必填）/ / `lang`（可选）/ `access_token`；表单文件字段名为 `media`。 / 官方「注意事项」原文为「本接口无特殊注意事项」（但字段说明含 16k/单声道/最大 1M 硬限制，矛盾照录）。
- **两步语义（官方注意事项，SDK 不编排）**：本接口上传后须在 **10 秒内**调用 / `QueryVoiceRecognitionResultAsync` 取回识别结果。
- 官方错误码：`-1` / `40001` / `40010`（invalid voice size，不合法的语音文件大小）。

### 2. `QueryVoiceRecognitionResultAsync` — 获取语音识别结果。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_queryrecoresultfortext.html
（官方接口英文名 `queryRecoResultForText`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cgi-bin/media/voice/queryrecoresultfortext`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `voiceId` | Query `voice_id` | `string` | 语音唯一标识（**必填**，与上传时一致）。 |
| `lang` | Query `lang` | `string?` | 语言（官方 `lang`，可选，`zh_CN` 或 `en_US`，默认中文）。 |
- **返回**：`MpVoiceRecoResultResponse` — 识别结果（`result`）。

**备注（原文）**

- 官方契约：**POST**，**无请求体**，Query 携带 `voice_id`（必填）/ `lang`（可选）/ `access_token`。
- 官方「注意事项」原文：**「添加完文件之后 10s 内调用这个接口」** / ⇒ 轮询与退避策略归宿主（「SDK 不做业务编排」红线）。
- 官方错误码：`-1` / `40001`（本页错误码表仅此两行，照录）。

### 3. `TranslateContentAsync` — 微信翻译（文本内容翻译）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ai/api_translatecontent.html
（官方接口英文名 `translateContent`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cgi-bin/media/voice/translatecontent`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fromLanguage` | Query `lfrom` | `string` | 源语言（**必填**，官方枚举仅 `zh_CN`/`en_US`）。 |
| `toLanguage` | Query `lto` | `string` | 目标语言（**必填**，官方枚举仅 `zh_CN`/`en_US`）。 |

- **请求体**：`MpVoiceTranslateRequest` — 翻译请求体（`content` 必填，utf8，最大 600Byte）。
- **返回**：`MpVoiceTranslateResponse` — 原文与译文（`from_content`/`to_content`）。

**备注（原文）**

- 官方契约：**POST**，**Query** 携带 `lfrom`/`lto`（均必填）与 `access_token`； / 请求体 `{"content": …}`（**最大 600Byte**，SDK 不本地拦截）。
- **路径语义异常（官方现状，照录）**：本接口为文本翻译，但路径位于 `/cgi-bin/media/voice/` 下。 / 官方「注意事项」原文「本接口无特殊注意事项」（与 600Byte / 语言枚举两处硬限制矛盾，照录）。
- 官方错误码：`40001` / `40035`（invalid args size，不合法的参数）。

### 4. `IdCardOcrByUploadAsync` — 身份证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idcardocr.html
（官方接口英文名 `idcardOcr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/idcard`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrIdCardResponse` — 身份证正反面识别结果。

**备注（原文）**

- 官方原文：「图片支持使用 `img` 参数实时上传，也支持使用 `img_url` 参数传送图片地址，由微信后台下载图片进行识别」 / —— 两形态互斥，URL 形态见 `IdCardOcrByUrlAsync`。
- 官方错误码：`-1` / `40001` / `101000` / `101001` / `101002` / `101003`。

### 5. `IdCardOcrByUrlAsync` — 身份证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_idcardocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/idcard`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`；由微信后台下载识别）。 |
- **返回**：`MpOcrIdCardResponse` — 身份证正反面识别结果。

**备注（原文）**

- 与 `IdCardOcrByUploadAsync` 互斥（官方原文「传这个则不用传 img 参数」）。

### 6. `BankCardOcrByUploadAsync` — 银行卡识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bankcardocr.html
（官方接口英文名 `bankcardOcr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/bankcard`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrBankCardResponse` — 银行卡卡面识别结果（`number` / `id` 双键照录，见 `MpOcrBankCardResponse`）。

**备注（原文）**

- 官方错误码：`-1` / `40001` / `101000` / `101001` / `101003`。

### 7. `BankCardOcrByUrlAsync` — 银行卡识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bankcardocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/bankcard`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrBankCardResponse` — 银行卡卡面识别结果。

### 8. `DrivingOcrByUploadAsync` — 行驶证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivingocr.html
（官方接口英文名 `drivingocr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/driving`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrDrivingResponse` — 行驶证识别结果。

**备注（原文）**

- 官方错误码：`-1` / `40001` / `101000` / `101003`。

### 9. `DrivingOcrByUrlAsync` — 行驶证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivingocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/driving`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrDrivingResponse` — 行驶证识别结果。

### 10. `DrivingLicenseOcrByUploadAsync` — 驾驶证识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivinglicenseocr.html
（官方接口英文名 `drivinglicenseocr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/drivinglicense`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrDrivingLicenseResponse` — 驾驶证识别结果。

**备注（原文）**

- 官方错误码：`-1` / `40001` / `101000` / `101003`。

### 11. `DrivingLicenseOcrByUrlAsync` — 驾驶证识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_drivinglicenseocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/drivinglicense`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrDrivingLicenseResponse` — 驾驶证识别结果。

### 12. `BizLicenseOcrByUploadAsync` — 营业执照识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bizlicenseocr.html
（官方接口英文名 `bizlicenseOcr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/bizlicense`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrBizLicenseResponse` — 营业执照识别结果（仅返回图片中存在的字段）。

**备注（原文）**

- 官方「注意事项」原文：「返回字段仅包含当前营业执照图片中存在的字段，若对应字段不存在则不返回」。
- 官方错误码：`-1` / `40001` / `101000` / `101001` / `101002` / `101003`。

### 13. `BizLicenseOcrByUrlAsync` — 营业执照识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_bizlicenseocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/bizlicense`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrBizLicenseResponse` — 营业执照识别结果（仅返回图片中存在的字段）。

### 14. `CommOcrByUploadAsync` — 通用印刷体识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_commocr.html
（官方接口英文名 `commocr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/comm`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
- **返回**：`MpOcrCommResponse` — 印刷体识别条目列表（`items[]`）。

**备注（原文）**

- 官方契约：**POST multipart/form-data**，Query 携带 `access_token`；表单文件字段名 `img` / （本页官方把 `img` 标为非必填，但两形态实为至少择一，照录）。
- 官方错误码：`-1` / `40001` / `101000` / `101002` / `101003`。

### 15. `CommOcrByUrlAsync` — 通用印刷体识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_commocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/comm`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrCommResponse` — 印刷体识别条目列表（`items[]`）。

### 16. `MenuOcrByUploadAsync` — 菜单识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menuocr.html
（官方接口英文名 `menuOcr`；官方标注**不支持云调用**）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/menu`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，官方原文「文件大小限制：小于 2M」）。 |
- **返回**：`MpOcrMenuResponse` — 菜单内容（`content.menu_items[]`）。

**备注（原文）**

- **本页无频率上限**（官方全页无频次章节，且原文「该接口尚未接入服务平台，暂时不支持付费购买」） / ——不得套用其它 OCR 页的 100 次/天。
- 官方错误码：`-1` / `101000` / `101001` / `101002`（本页错误码表未列 `40001`，照录）。

### 17. `MenuOcrByUrlAsync` — 菜单识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/ocr/api_menuocr.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/menu`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`）。 |
- **返回**：`MpOcrMenuResponse` — 菜单内容（`content.menu_items[]`）。

### 18. `AiCropByUploadAsync` — 图片智能裁剪（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgaicrop.html
（官方接口英文名 `imgAiCrop`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/img/aicrop`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，大小 < 2M）。 |
| `ratios` | Form | `string?` | 宽高比（官方 form 字段 `ratios`，可选；多个以英文逗号「,」分隔，**最多 5 个**；<br>为空则算法自动裁剪最佳宽高比）。 |
- **返回**：`MpImageAiCropResponse` — 智能裁剪结果（`results[]`：裁剪区域四边界 + `img_size`）。

**备注（原文）**

- 官方契约：**POST multipart/form-data**，Query 携带 `access_token`； / 表单字段 `img`（文件）与 `ratios`（可选）。
- 官方「注意事项」原文：「如果提供多个宽高比，请以英文逗号「,」分隔，最多支持 5 个宽高比」。
- 官方错误码：`-1` / `40001` / `101000`（invalid image url）/ `101002`（invalid image data）。

### 19. `AiCropByUrlAsync` — 图片智能裁剪（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgaicrop.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/img/aicrop`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 要检测的图片 URL（官方 Query `img_url`；由微信后台下载识别）。 |
| `ratios` | Form | `string?` | 宽高比（官方 form 字段 `ratios`，可选；多个以英文逗号「,」分隔，最多 5 个）。 |
- **返回**：`MpImageAiCropResponse` — 智能裁剪结果（`results[]` + `img_size`）。

**备注（原文）**

- 官方契约：**POST**，Query 携带 `img_url` 与 `access_token`；`ratios` 为表单字段。

### 20. `QrcodeRecognitionByUploadAsync` — 二维码/条码识别（图片实时上传形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgqrcode.html
（官方接口英文名 `imgQrcode`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/img/qrcode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**，官方原文「文件需小于 2MB」）。 |
- **返回**：`MpImageQrcodeResponse` — 码识别结果（`code_results[]`：类型 / 信息 / 坐标 + `img_size`）。

**备注（原文）**

- 官方原文：「支持条码、二维码、DataMatrix 和 PDF417 的识别」；「**二维码、DataMatrix 会返回位置坐标， / 条码和 PDF417 暂不返回位置坐标**」⇒ 条目坐标可空。
- 官方错误码：`-1` / `40001` / `101000` / `101002`。

### 21. `QrcodeRecognitionByUrlAsync` — 二维码/条码识别（图片 URL 形态）。官方文档：https://developers.weixin.qq.com/doc/service/api/openpoc/image/api_imgqrcode.html。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/img/qrcode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 图片 URL（官方 Query `img_url`；由微信后台下载识别）。 |
- **返回**：`MpImageQrcodeResponse` — 码识别结果（`code_results[]` + `img_size`）。

### 22. `PlateNumberOcrByUploadAsync` — 图片高清化（超分辨率）——form-data 实时上传形态。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Interface/Img_Proc.html
（官方接口英文名 `imgSuperresolution`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/platenum`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（**文件字段名必须为 `img`**；<br>由调用方构建 `IFormContent` 实现，须标 `[MultipartForm]`；图片 ≤ 1MB）。 |
- **返回**：`MpOcrPlateNumberResponse` — 高清化后图片的临时素材 `media_id`（经下载通道取回）。

**备注（原文）**

- 官方契约：**POST multipart/form-data**（form-data 实时上传 `img`），Query 携带 `access_token`； / 与 `PlateNumberOcrByUrlAsync` 互斥（URL 形态走 Query `img_url`）。

### 23. `PlateNumberOcrByUrlAsync` — 车牌识别（OCR）——URL 传送形态。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Interface/OCR.html
（官方接口英文名 `platenumOcr`）。

- **声明接口**：[IMpSmartApiService](./IMpSmartApiService.cs)
- **官方契约**：`POST /cv/ocr/platenum`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `imgUrl` | Query `img_url` | `string` | 图片 URL（官方 Query `img_url`；由微信后台下载识别）。 |
- **返回**：`MpOcrPlateNumberResponse` — 识别结果（`number` 车牌号）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/DataModels/SmartApi`](../../DataModels/SmartApi) 内共 **22** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **22** 个，`SerializerClassName` = `SmartApi`。

AOT 源生成上下文：[`SmartApiJsonContext.g.cs`](./../../DataModels/Generated/SmartApiJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpResponse` | 应答 | [MpResponse.cs](../../DataModels/MpResponse.cs) | `errcode`, `errmsg` |
| `MpVoiceRecoResultResponse` | 应答 | [SmartApi/Voice/MpVoiceRecoResultResponse.cs](../../DataModels/SmartApi/Voice/MpVoiceRecoResultResponse.cs) | `result` |
| `MpVoiceTranslateResponse` | 应答 | [SmartApi/Voice/MpVoiceTranslateResponse.cs](../../DataModels/SmartApi/Voice/MpVoiceTranslateResponse.cs) | `from_content`, `to_content` |
| `MpVoiceTranslateRequest` | 请求体 | [SmartApi/Voice/MpVoiceTranslateRequest.cs](../../DataModels/SmartApi/Voice/MpVoiceTranslateRequest.cs) | `content` |
| `MpOcrIdCardResponse` | 应答 | [SmartApi/Ocr/MpOcrIdCardResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrIdCardResponse.cs) | `type`, `name`, `id`, `valid_date`, `addr`, `gender`, `nationality` |
| `MpOcrBankCardResponse` | 应答 | [SmartApi/Ocr/MpOcrBankCardResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrBankCardResponse.cs) | `number`, `id` |
| `MpOcrDrivingResponse` | 应答 | [SmartApi/Ocr/MpOcrDrivingResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrDrivingResponse.cs) | `plate_num`, `vehicle_type`, `owner`, `addr`, `use_character`, `model`, `vin`, `engine_num`, `register_date`, `issue_date`, `plate_num_b`, `record` … |
| `MpOcrDrivingLicenseResponse` | 应答 | [SmartApi/Ocr/MpOcrDrivingLicenseResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrDrivingLicenseResponse.cs) | `id_num`, `name`, `sex`, `nationality`, `address`, `birth_date`, `issue_date`, `car_class`, `valid_from`, `valid_to`, `official_seal` |
| `MpOcrBizLicenseResponse` | 应答 | [SmartApi/Ocr/MpOcrBizLicenseResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrBizLicenseResponse.cs) | `reg_num`, `serial`, `legal_representative`, `enterprise_name`, `type_of_organization`, `address`, `type_of_enterprise`, `business_scope`, `registered_capital`, `paid_in_capital`, `valid_period`, `registered_date` … |
| `MpOcrCommResponse` | 应答 | [SmartApi/Ocr/MpOcrCommResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrCommResponse.cs) | `items`, `img_size` |
| `MpOcrMenuResponse` | 应答 | [SmartApi/Ocr/MpOcrMenuResponse.cs](../../DataModels/SmartApi/Ocr/MpOcrMenuResponse.cs) | `content` |
| `MpImageAiCropResponse` | 应答 | [SmartApi/Image/MpImageAiCropResponse.cs](../../DataModels/SmartApi/Image/MpImageAiCropResponse.cs) | `results`, `img_size` |
| `MpImageQrcodeResponse` | 应答 | [SmartApi/Image/MpImageQrcodeResponse.cs](../../DataModels/SmartApi/Image/MpImageQrcodeResponse.cs) | `code_results`, `img_size` |
| `MpOcrPlateNumberResponse` | 应答 | [SmartApi/MpOcrPlateNumberResponse.cs](../../DataModels/SmartApi/MpOcrPlateNumberResponse.cs) | `number` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.SmartApi;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddSmartApiApi());

var service = app.Services.GetRequiredService<IMpSmartApiService>();
var ct = CancellationToken.None;
var response = await service.BankCardOcrByUploadAsync(default!, ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cv/ocr/bankcard");
// 占位值仅用于编译示意；业务值须按 IMpSmartApiService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`BankCardOcrByUploadAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IMpSmartApiService`：注释 12 / 实计 23 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpSmartApiService](./IMpSmartApiService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpSmartApiContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSmartApiContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `SmartApiEndpoints_ShouldMatchOfficialRoutes`、`VoiceEndpoints_ShouldKeepOfficialParameterPlacement`、`SmartApiShapes_ShouldLockOfficialFieldNames`、`SmartApiDataModels_ShouldBeRegisteredInJsonContext`、`SmartApiModule_ShouldBeRegisteredAndWhitelisted`、`SmartApiErrorCodes_ShouldMatchVerifiedOfficialValues`、`SuperResolutionEndpoint_ShouldStayUnmodeled`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/DataModels/SmartApi`](../../DataModels/SmartApi) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpSmartApiContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

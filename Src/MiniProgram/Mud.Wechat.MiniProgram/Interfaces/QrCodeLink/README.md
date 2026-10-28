# QrCodeLink 域 · 二维码 / 链接

微信小程序「二维码 / 链接」域 SDK —— **JSON 通道**（6 端点：URL Link / URL Scheme / ShortLink / NFC Scheme）。 接口：[IWxaCodeService](./IWxaCodeService.cs)、[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)；注册组 `QrCodeLink`；模块枚举 `MiniProgramModule.QrCodeLink`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/QrCodeLink`](../../../Mud.Wechat.MiniProgram/QrCodeLink/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaQrCodeLinkService = 6；按 HTTP 特性实计 **6** 支，另有手工通道 **3** 支，本域合计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaQrCodeLinkService` | `GenerateUrlLinkAsync` | POST | `/wxa/generate_urllink` | `AccessToken`（Query），Query `access_token` | 生成 URL Link（小程序加密链接）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrco… |
| 2 | `IWxaQrCodeLinkService` | `QueryUrlLinkAsync` | POST | `/wxa/query_urllink` | `AccessToken`（Query），Query `access_token` | 查询 URL Link 信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-lin… |
| 3 | `IWxaQrCodeLinkService` | `GenerateSchemeAsync` | POST | `/wxa/generatescheme` | `AccessToken`（Query），Query `access_token` | 生成 URL Scheme（小程序加密 scheme 码）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server… |
| 4 | `IWxaQrCodeLinkService` | `QuerySchemeAsync` | POST | `/wxa/queryscheme` | `AccessToken`（Query），Query `access_token` | 查询 URL Scheme 信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-l… |
| 5 | `IWxaQrCodeLinkService` | `GenerateShortLinkAsync` | POST | `/wxa/genwxashortlink` | `AccessToken`（Query），Query `access_token` | 生成 ShortLink（小程序短链）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcod… |
| 6 | `IWxaQrCodeLinkService` | `GenerateNfcSchemeAsync` | POST | `/wxa/generatenfcscheme` | `AccessToken`（Query），Query `access_token` | 获取 NFC 的小程序 scheme。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode… |
| M1 | `IWxaCodeService` | `GetUnlimitedCodeAsync` | POST | `/wxa/getwxacodeunlimit` | 手工注入 `access_token`（非 `[Token]`） | 获取**不限量**小程序码（`POST /wxa/getwxacodeunlimit`）。<br>官方文档：https://developers.weixin.qq.com/minipr… |
| M2 | `IWxaCodeService` | `GetCodeAsync` | POST | `/wxa/getwxacode` | 手工注入 `access_token`（非 `[Token]`） | 获取**限量**小程序码（`POST /wxa/getwxacode`）。<br>官方文档：https://developers.weixin.qq.com/miniprogram/de… |
| M3 | `IWxaCodeService` | `CreateQrCodeAsync` | POST | `/cgi-bin/wxaapp/createwxaqrcode` | 手工注入 `access_token`（非 `[Token]`） | 创建**二维码 C**（`POST /cgi-bin/wxaapp/createwxaqrcode`）。<br>官方文档：https://developers.weixin.qq.com… |

> **手工通道**：`IWxaCodeService`（3 支） 不是 `[HttpClientApi]` 接口 —— 成功响应为二进制流 / 文本等原始内容，须走原始响应直读 + Content-Type 分支判错，故没有 HTTP 特性。其方法仍是官方端点，已计入本域合计，明细见下方「手工通道端点」。

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaCodeService`

**为何不是 `[HttpClientApi]` 接口**：三端点**成功时返回图片二进制流**
（`Content-Type: image/*`），**失败时才返回 JSON**（`{"errcode":…}`）。
声明式管线按 JSON 反序列化，会把图片字节交给 JSON 解析器、或把错误体当图片交出 ——
两种都是静默错误。故本通道走 `IBaseHttpClient.SendRawAsync` 原始响应直读 +
**Content-Type 分支判错**（与公众号线 `IMpMediaDownloadService` 同款先例）。

**令牌**：非 `[Token]` 声明式注入，而是**手工取当前应用令牌**并拼到 Query
（`access_token`，官方契约）——因此本接口**不进** Query 令牌白名单（白名单只收
`[Token]` 声明式接口，与公众号线 `IMpMediaDownloadService` 同款处置）。

**令牌自愈**：命中失效码（`40001` / `40014` / `42001`）时失效本应用令牌并
**重试一次**，重试仍失败按业务异常上抛（`WxaException`）。

### `IWxaQrCodeLinkService`

**官方文档**（小程序服务端 API，2026-10-09 逐页核验）：
生成 URL Link https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-link/api_generateurllink、
查询 URL Link https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-link/api_queryurllink、
生成 URL Scheme https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_generatescheme、
查询 URL Scheme https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_queryscheme、
生成 ShortLink https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/short-link/api_generateshortlink、
获取 NFC scheme https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_generatenfcscheme。

**双通道分工（对齐公众号线素材域先例，勿合并）**：本域 9 端点中，
**URL Link / URL Scheme / ShortLink / NFC Scheme 六端点响应为 JSON**（本接口，走生成管线）；
**小程序码三端点响应为图片二进制流**（失败时才是 JSON）⇒ 落独立请求形态
`IWxaCodeService`（Content-Type 分支判错，不进 JSON 反序列化管线）。两者同挂
`QrCodeLink` 注册组。

**勿与公众号端点混淆（MP-X6）**：`/wxa/genwxashortlink`（`page_url` → `link`）与
公众号 `/cgi-bin/shorten/gen`（`long_data` ≤ 4KB）是**不同端点、不同语义**，并存不构成重复。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GenerateUrlLinkAsync` — 生成 URL Link（小程序加密链接）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-link/api_generateurllink。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/generate_urllink`

- **请求体**：`WxaUrlLinkRequest` — 生成请求（`path` 必填），见 `WxaUrlLinkRequest`。
- **返回**：`WxaUrlLinkResponse` — 生成的链接（`url_link`）。

**备注（原文）**

- 官方契约：**POST** `/wxa/generate_urllink` ＋ 请求体 JSON；Query 携带 `access_token`。
- **数量与配额（官方原文要点）**：加密 URL Link 与明文 URL Scheme 的**单天访问次数合计**上限 / （官方以 `quota_info.remain_visit_quota` 经查询接口暴露剩余额度）； / 生成有效链接总数亦有上限，超限报错。均**不做本地拦截**（由官方错误码表达）。
- **有效期**：`expire_type` / `expire_interval` / `expire_time` 三选一表达， / 取值语义与上限以官方页面为准（SDK 不编造数值，也不做本地校验）。

### 2. `QueryUrlLinkAsync` — 查询 URL Link 信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-link/api_queryurllink（2026-10-09 逐字段核验）。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/query_urllink`

- **请求体**：`WxaQueryUrlLinkRequest` — 查询请求（`url_link` 与 `query_type`），见 `WxaQueryUrlLinkRequest`。
- **返回**：`WxaQueryUrlLinkResponse` — 链接配置（`url_link_info`）或剩余额度（`quota_info`），见 `WxaQueryUrlLinkResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/query_urllink` ＋ 请求体 JSON；Query 携带 `access_token`。
- **`query_type` 双义**：`0`（默认）= 查询 `url_link` 配置； / `1` = 查询**当天剩余访问次数**（此时 `url_link` 无需传，响应填 `quota_info`）。

### 3. `GenerateSchemeAsync` — 生成 URL Scheme（小程序加密 scheme 码）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_generatescheme。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/generatescheme`

- **请求体**：`WxaSchemeRequest` — 生成请求（`jump_wxa`），见 `WxaSchemeRequest`。
- **返回**：`WxaSchemeResponse` — 生成的 scheme（`openlink`）。

**备注（原文）**

- 官方契约：**POST** `/wxa/generatescheme` ＋ 请求体 JSON；Query 携带 `access_token`。
- **配额**：与加密 URL Link 共用「单天访问次数」限额（见 `QuerySchemeAsync` 的 `quota_info`）。

### 4. `QuerySchemeAsync` — 查询 URL Scheme 信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_queryscheme（2026-10-09 逐字段核验）。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/queryscheme`

- **请求体**：`WxaQuerySchemeRequest` — 查询请求（`scheme` / `query_type`），见 `WxaQuerySchemeRequest`。
- **返回**：`WxaQuerySchemeResponse` — scheme 配置（`scheme_info`）或剩余额度（`quota_info`），见 `WxaQuerySchemeResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/queryscheme` ＋ 请求体 JSON；Query 携带 `access_token`。
- `query_type` 双义同 `QueryUrlLinkAsync`（`1` 时填 `quota_info.remain_visit_quota`）。

### 5. `GenerateShortLinkAsync` — 生成 ShortLink（小程序短链）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/short-link/api_generateshortlink。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/genwxashortlink`

- **请求体**：`WxaShortLinkRequest` — 生成请求（`page_url` 必填），见 `WxaShortLinkRequest`。
- **返回**：`WxaShortLinkResponse` — 生成的短链（`link`）。

**备注（原文）**

- 官方契约：**POST** `/wxa/genwxashortlink` ＋ 请求体 JSON；Query 携带 `access_token`。
- **`page_url` 必须是配置了 `sitemap.json` 的页面**（官方原文），否则生成失败； / 短链**不支持 `query` 参数**（与 URL Link / Scheme 的本质差异）。
- **时效**：`is_permanent` 为长期有效开关；短期有效时由 / `expire_type` / `expire_interval` / `expire_time` 表达（上限以官方页面为准）。

### 6. `GenerateNfcSchemeAsync` — 获取 NFC 的小程序 scheme。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/url-scheme/api_generatenfcscheme。

- **声明接口**：[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs)
- **官方契约**：`POST /wxa/generatenfcscheme`

- **请求体**：`WxaNfcSchemeRequest` — NFC 场景生成请求（`sn` / `model_id` 必填），见 `WxaNfcSchemeRequest`。
- **返回**：`WxaSchemeResponse` — 生成的 scheme（`openlink`，与 URL Scheme 同形）。

**备注（原文）**

- 官方契约：**POST** `/wxa/generatenfcscheme` ＋ 请求体 JSON；Query 携带 `access_token`。
- **适用场景（官方原文）**：用于 NFC 拉起小程序的业务场景（如智能硬件碰一碰直达页面）； / 生成参数（`sn` 设备序列号 / `model_id` 设备型号 ID）为** NFC 专属**， / 其余字段与 URL Scheme **同形但不等价**（勿与 `GenerateSchemeAsync` 混杂使用）。
- **有效期**：`expire_type` = `0`（失效间隔，`expire_interval` 官方范围 7~30 天）/ / `1`（失效日期 `expire_time` 绝对时间戳）；`is_expire` 为是否过期开关（官方原文： / 使用失效字段前先将此值设为 `true`）。取值语义以官方页面为准，SDK 不做本地校验。

### 手工通道端点（非 `[HttpClientApi]`）

以下接口不带 HTTP 特性，路由与参数取自方法 XML 注释，调用经手工原始响应通道（非生成管线）。

#### M1. `GetUnlimitedCodeAsync` — 获取**不限量**小程序码（`POST /wxa/getwxacodeunlimit`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/qr-code/api_getunlimitedqrcode。

- **声明接口**：[IWxaCodeService](./IWxaCodeService.cs)
- **官方契约**：`POST /wxa/getwxacodeunlimit`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `request` | Body | `DataModels.QrCodeLink.WxaCodeUnlimitRequest` | 生成请求（`scene` 必填，≤32 可见字符），见 `WxaCodeUnlimitRequest`。 |
- **返回**：`WxaCodeResult` — 图片内容流，见 `WxaCodeResult`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；成功响应为图片二进制。
- **总量不限**（与限量码的本质差异），但 `scene` **最长 32 个可见字符**。
- 官方错误码：`40001`（令牌，走自愈）/ `41030`（`page` 不存在或未发布）/ `45009`（调用量超限）。

#### M2. `GetCodeAsync` — 获取**限量**小程序码（`POST /wxa/getwxacode`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/qr-code/api_getqrcode。

- **声明接口**：[IWxaCodeService](./IWxaCodeService.cs)
- **官方契约**：`POST /wxa/getwxacode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `request` | Body | `DataModels.QrCodeLink.WxaCodeRequest` | 生成请求（`path` 必填），见 `WxaCodeRequest`。 |
- **返回**：`WxaCodeResult` — 图片内容流，见 `WxaCodeResult`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；成功响应为图片二进制。
- **总量上限 10 万个**（官方原文）；`path` 不得带参数（带参数请用不限量码的 `scene`）。

#### M3. `CreateQrCodeAsync` — 创建**二维码 C**（`POST /cgi-bin/wxaapp/createwxaqrcode`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/qr-code/api_createqrcode。

- **声明接口**：[IWxaCodeService](./IWxaCodeService.cs)
- **官方契约**：`POST /cgi-bin/wxaapp/createwxaqrcode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `request` | Body | `DataModels.QrCodeLink.WxaQrCodeRequest` | 生成请求（`path` 必填），见 `WxaQrCodeRequest`。 |
- **返回**：`WxaCodeResult` — 图片内容流，见 `WxaCodeResult`。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；成功响应为图片二进制。
- 生成的是**二维码**而非小程序码，样式不可定制；同样受**总量 10 万个**限制。
- **路由前缀跨包**：`/cgi-bin/wxaapp/` 是本线唯一的 `/cgi-bin/` 前缀端点 / （其余在 `/wxa/`）；与公众号线 `/cgi-bin/*` 端点**无重叠**（MP-X1 断言）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/QrCodeLink`](../../../Mud.Wechat.MiniProgram/QrCodeLink/) 内共 **20** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **20** 个，`SerializerClassName` = `QrCodeLink`。

AOT 源生成上下文：[`QrCodeLinkJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/QrCodeLinkJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaUrlLinkResponse` | 应答 | [QrCodeLink/WxaUrlLinkResponse.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaUrlLinkResponse.cs) | `url_link` |
| `WxaUrlLinkRequest` | 请求体 | [QrCodeLink/WxaUrlLinkRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaUrlLinkRequest.cs) | `path`, `query`, `expire_type`, `expire_interval`, `expire_time`, `cloud_base` |
| `WxaQueryUrlLinkResponse` | 应答 | [QrCodeLink/WxaQueryUrlLinkResponse.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaQueryUrlLinkResponse.cs) | `url_link_info`, `quota_info` |
| `WxaQueryUrlLinkRequest` | 请求体 | [QrCodeLink/WxaQueryUrlLinkRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaQueryUrlLinkRequest.cs) | `url_link`, `query_type` |
| `WxaSchemeResponse` | 应答 | [QrCodeLink/WxaSchemeResponse.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaSchemeResponse.cs) | `openlink` |
| `WxaSchemeRequest` | 请求体 | [QrCodeLink/WxaSchemeRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaSchemeRequest.cs) | `jump_wxa`, `is_expire`, `expire_type`, `expire_interval`, `expire_time` |
| `WxaQuerySchemeResponse` | 应答 | [QrCodeLink/WxaQuerySchemeResponse.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaQuerySchemeResponse.cs) | `scheme_info`, `quota_info` |
| `WxaQuerySchemeRequest` | 请求体 | [QrCodeLink/WxaQuerySchemeRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaQuerySchemeRequest.cs) | `scheme`, `query_type` |
| `WxaShortLinkResponse` | 应答 | [QrCodeLink/WxaShortLinkResponse.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaShortLinkResponse.cs) | `link` |
| `WxaShortLinkRequest` | 请求体 | [QrCodeLink/WxaShortLinkRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaShortLinkRequest.cs) | `page_url`, `page_title`, `is_permanent`, `expire_type`, `expire_interval`, `expire_time` |
| `WxaNfcSchemeRequest` | 请求体 | [QrCodeLink/WxaNfcSchemeRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaNfcSchemeRequest.cs) | `sn`, `model_id`, `jump_wxa`, `expire_type`, `expire_interval`, `expire_time`, `is_expire` |
| `WxaCodeUnlimitRequest` | 请求体 | [QrCodeLink/WxaCodeUnlimitRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaCodeUnlimitRequest.cs) | `scene`, `page`, `check_path`, `env_version`, `width`, `auto_color`, `line_color`, `is_hyaline` |
| `WxaCodeRequest` | 请求体 | [QrCodeLink/WxaCodeRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaCodeRequest.cs) | `path`, `width`, `auto_color`, `line_color`, `is_hyaline` |
| `WxaQrCodeRequest` | 请求体 | [QrCodeLink/WxaQrCodeRequest.cs](../../../Mud.Wechat.MiniProgram/QrCodeLink/WxaQrCodeRequest.cs) | `path`, `width` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.QrCodeLink;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddQrCodeLinkApi());

var service = app.Services.GetRequiredService<IWxaQrCodeLinkService>();
var ct = CancellationToken.None;
var response = await service.GenerateNfcSchemeAsync(new WxaNfcSchemeRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/generatenfcscheme");
// 占位值仅用于编译示意；业务值须按 IWxaQrCodeLinkService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GenerateNfcSchemeAsync`）；请求体 `WxaNfcSchemeRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaCodeService](./IWxaCodeService.cs)、[IWxaQrCodeLinkService](./IWxaQrCodeLinkService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/QrCodeLink`](../../../Mud.Wechat.MiniProgram/QrCodeLink/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

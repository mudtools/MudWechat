# ShortLink 域 · 长信息与短链

微信公众号 / 服务号「长信息与短链」域 SDK（2 端点）。 接口：[IMpShortLinkService](./IMpShortLinkService.cs)；注册组 `ShortLink`；模块枚举 `MpModule.ShortLink`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpShortLinkContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpShortLinkContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount.DataModels/ShortLink`](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpShortLinkService = 2；按 HTTP 特性实计 **3** 支。

> **计数漂移（留档）**：`IMpShortLinkService` 注释自述 2 支、实计 3 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpShortLinkService` | `GenerateShortKeyAsync` | POST | `/cgi-bin/shorten/gen` | `AccessToken`（Query），Query `access_token` | 长信息转短链。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_genshortke… |
| 2 | `IMpShortLinkService` | `FetchShortKeyAsync` | POST | `/cgi-bin/shorten/fetch` | `AccessToken`（Query），Query `access_token` | 短链转长信息。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_fetchshort… |
| 3 | `IMpShortLinkService` | `GetShortUrlAsync` | POST | `/cgi-bin/shorturl` | `AccessToken`（Query），Query `access_token` | 长链接转短链接（**旧版，官方已停维**）。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Account_Manage… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/service/api/
→ 服务号二维码 → 长信息与短链。深链均为**逐页核验过的真实页面**（2026-10-07）。

**域级业务约束（逐页核验，勿弱化）**：

- **适用范围**：官方表为「小程序 ✔ / 服务号 仅认证 / 小游戏 ✔」（SDK 不做账号类型本地闸）。
- **容量与时效上限**：`long_data` **不超过 4KB**；`expire_seconds`
**最大值 2592000 秒（30 天）**，默认 2592000 —— 两处越界由官方
`9410010`/`9410011` 表达，SDK **不做本地拦截**（沿用「越界由官方 errcode 表达」的既存口径，
规避 `long_data` 按字节还是字符计长的歧义）。
- **频率限制**：官方两页均**无频率章节、无频次数值**（SDK 不编造）。
- **第三方平台**：两页均支持代商家调用（**权限集 id 3、17**）——按 M0-R3 裁决只在 XML 记录，不扩实现面。
- **官方文档缺陷（照录）**：两页「注意事项」原文均为「本接口无特殊注意事项」，
但参数层存在 4KB / 30 天硬上限与两条专属错误码；`fetchShorten` 页 `47003` 的描述原文为
「模板参数不准确」（本接口无模板概念，疑为通用文案残留）。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GenerateShortKeyAsync` — 长信息转短链。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_genshortkey.html
（官方接口英文名 `genShortKey`；云调用方法 `officialAccount.shorten.gen`）。

- **声明接口**：[IMpShortLinkService](./IMpShortLinkService.cs)
- **官方契约**：`POST /cgi-bin/shorten/gen`

- **请求体**：`MpShortenGenRequest` — 转换请求（`long_data` 必填 ≤4KB；`expire_seconds` 最大 2592000 秒）。
- **返回**：`MpShortenGenResponse` — 短 key（`short_key`，15 字节 base62）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"long_data": …, "expire_seconds": …}`。
- 官方错误码：`-1` / `40001` / `44002`（empty post data）/ `47001`（data format error）/ / `47003`（没有传入 `long_data`）/ `9410010`（long_data 长度超过限制 4KB）/ / `9410011`（expire_seconds 超过限制 30 天）。

### 2. `FetchShortKeyAsync` — 短链转长信息。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/shorten/api_fetchshorten.html
（官方接口英文名 `fetchShorten`；云调用方法 `officialAccount.shorten.fetch`）。

- **声明接口**：[IMpShortLinkService](./IMpShortLinkService.cs)
- **官方契约**：`POST /cgi-bin/shorten/fetch`

- **请求体**：`MpShortenFetchRequest` — 还原请求（`short_key` 必填）。
- **返回**：`MpShortenFetchResponse` — 长信息（`long_data`）/ 创建时间戳（`create_time`）/ 剩余过期秒数（`expire_seconds`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"short_key": …}`。
- 官方错误码：`-1` / `44002` / `47001` / `47003` / / `9410012`（short_key 不存在 / 已过期 / 不属于本账号）。

### 3. `GetShortUrlAsync` — 长链接转短链接（**旧版，官方已停维**）。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Account_Management/URL_Shortener.html
（官方接口英文名 `shortUrl`）。

- **声明接口**：[IMpShortLinkService](./IMpShortLinkService.cs)
- **官方契约**：`POST /cgi-bin/shorturl`

- **请求体**：`MpShortUrlRequest` — 转换请求（`action` 固定 `long2short`；`long_url` 必填）。
- **返回**：`MpShortUrlResponse` — 短链接（`short_url`）。

**备注（原文）**

- **与 `GenerateShortKeyAsync` 的关系（官方两代接口，并存勿合并）**：本端点是旧版 / `/cgi-bin/shorturl`——官方已公告停止维护，仅存量链接兼容场景使用；新接入一律用新版 / `/cgi-bin/shorten/gen`。旧版转换的长链须已通过 ICP 备案。
- 官方错误码：`40002` / `40005`（本页错误码表照录）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount.DataModels/ShortLink`](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `ShortLink`。

AOT 源生成上下文：[`ShortLinkJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount.DataModels/Generated/ShortLinkJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpShortenGenResponse` | 应答 | [ShortLink/MpShortenGenResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortenGenResponse.cs) | `short_key` |
| `MpShortenGenRequest` | 请求体 | [ShortLink/MpShortenGenRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortenGenRequest.cs) | `long_data`, `expire_seconds` |
| `MpShortenFetchResponse` | 应答 | [ShortLink/MpShortenFetchResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortenFetchResponse.cs) | `long_data`, `create_time`, `expire_seconds` |
| `MpShortenFetchRequest` | 请求体 | [ShortLink/MpShortenFetchRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortenFetchRequest.cs) | `short_key` |
| `MpShortUrlResponse` | 应答 | [ShortLink/MpShortUrlResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortUrlResponse.cs) | `short_url` |
| `MpShortUrlRequest` | 请求体 | [ShortLink/MpShortUrlRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/MpShortUrlRequest.cs) | `action`, `long_url` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.ShortLink;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddShortLinkApi());

var service = app.Services.GetRequiredService<IMpShortLinkService>();
var ct = CancellationToken.None;
var response = await service.FetchShortKeyAsync(new MpShortenFetchRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/shorten/fetch");
// 占位值仅用于编译示意；业务值须按 IMpShortLinkService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`FetchShortKeyAsync`）；请求体 `MpShortenFetchRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IMpShortLinkService`：注释 2 / 实计 3 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpShortLinkService](./IMpShortLinkService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpShortLinkContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpShortLinkContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `ShortLinkEndpoints_ShouldMatchOfficialRoutes`、`ShortLinkShapes_ShouldLockOfficialFieldNames`、`ShortLinkModuleAndDataModels_ShouldBeRegistered`、`ShortLinkErrorCodes_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount.DataModels/ShortLink`](../../../Mud.Wechat.OfficialAccount.DataModels/ShortLink/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpShortLinkContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

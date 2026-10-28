# OpenApi 域 · openApi 管理

微信公众号 / 服务号「openApi 管理」域 SDK（5 端点：令牌形态 4 + 免令牌形态 1，单接口承载）。 接口：[IMpOpenApiService](./IMpOpenApiService.cs)、[IMpOpenApiTokenFreeService](./IMpOpenApiService.cs)；注册组 `OpenApi`；模块枚举 `MpModule.OpenApi`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpOpenApiContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpOpenApiContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)、[MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/OpenApi`](../../../Mud.Wechat.OfficialAccount/OpenApi/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpOpenApiService = 5；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpOpenApiService` | `ClearQuotaAsync` | POST | `/cgi-bin/clear_quota` | `AccessToken`（Query），Query `access_token` | 重置 API 调用次数。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clear… |
| 2 | `IMpOpenApiService` | `ClearQuotaV2Async` | POST | `/cgi-bin/clear_quota/v2` | `AccessToken`（Query），Query `access_token` |  |
| 3 | `IMpOpenApiService` | `GetApiQuotaAsync` | POST | `/cgi-bin/openapi/quota/get` | `AccessToken`（Query），Query `access_token` | 查询 API 调用额度。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_getap… |
| 4 | `IMpOpenApiService` | `ClearApiQuotaAsync` | POST | `/cgi-bin/openapi/quota/clear` | `AccessToken`（Query），Query `access_token` | 重置指定 API 调用次数。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_cle… |
| 5 | `IMpOpenApiService` | `GetRidInfoAsync` | POST | `/cgi-bin/openapi/rid/get` | `AccessToken`（Query），Query `access_token` | 查询 rid 信息（排障利器）。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_g… |
| 6 | `IMpOpenApiTokenFreeService` | `ClearQuotaV2Async` | POST | `/cgi-bin/clear_quota/v2` | 无 `[Token]`（见域级说明） | 使用 AppSecret 重置 API 调用次数（免 access_token）。官方文档：https://developers.weixin.qq.com/doc/subscri… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IMpOpenApiService`

**官方文档**：服务端 API 索引 → openApi 管理（apimanage/ 前缀）。深链均为逐页核验过的真实页面
（2026-10-07，subscription 订阅号域命中）。

**免令牌逃生端点（A-7 合并，2026-10-14 裁决）**：`clear_quota/v2` 是官方为
「access_token 耗尽无法调用重置接口」设计的**应急逃生端点**（免 access_token，appid + appsecret
入请求体），以**方法级** `[Token(Disabled = true)]` 显式免令牌（上游 3.0.7 / F-1 起支持
方法级覆盖接口级），与同接口 4 个带令牌端点共存；原「拆独立接口」workaround
（`IMpOpenApiTokenFreeService`）已 `[Obsolete]`，一版后移除。

**域级业务约束（逐页核验，勿弱化）**：

- **清零额度**：clear_quota 与 clear_quota/v2 **合计**每账号每月 **10 次**
（官方原文「清零生效一次即用掉一次机会」）；openapi/quota/clear 每账号每月 **50 次**。
- **同账号语义**：清空哪类账号的 quota 就需用该账号的 token；rid 查询仅支持同账号
（账号 A 产生的 rid 用账号 B 查询报 76003）。
- **sns 不支持**：官方原文「/xxx/sns/xxx 这类接口不支持该查询接口」（76022）。
- **第三方平台**：4 端点均支持 component_access_token 自调 / authorizer_access_token 代调用
（v2 不支持第三方平台调用）——按 M0-R3 裁决只在 XML 记录，不扩实现面。

### `IMpOpenApiTokenFreeService`

**已废弃（A-7 合并，2026-10-14 裁决）**：上游 Mud.HttpUtils 3.0.7（F-1）起
`[Token(Disabled = true)]` 支持**方法级覆盖接口级**，免令牌逃生端点无需再拆独立接口 ——
`ClearQuotaV2Async` 已合并回 `IMpOpenApiService`（同注册组、同客户端）。
本接口保留一个版本以维持二进制兼容（既有宿主按类型注入的注册/解析不变），
**Mud.Wechat 4.0 移除**；新代码请改用 `ClearQuotaV2Async`。

官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquotabyappsecret.html
（官方接口英文名 `clearQuotaByAppSecret`；路径 `/cgi-bin/clear_quota/v2`）。

**为何曾经独立接口（I4 裁决，已被 A-7 取代）**：官方注意事项原文「该接口通过 appsecret 调用，解决了
access_token 耗尽无法调用『重置 API 调用次数』的问题」——它是令牌耗尽时的**应急逃生端点**。
若声明 [Token]，SDK 会在调用前强制取令牌，恰好复刻它要解救的故障场景 ⇒ 本接口**不带 [Token]**、
不进令牌注入管线；appid/appsecret 走**请求体**（官方参数表权威；官方代码示例把两参数放
Query String 系文档矛盾——appsecret 进 URL 会扩大泄露面，SDK 不采用该形态）。

**仅支持 POST**；不支持第三方平台调用；每月与 clear_quota **合计 10 次**清零机会。
本端点调用结果不走令牌恢复链路（无令牌可恢复；48006 = 清零次数达到上限，为业务终态）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `ClearQuotaAsync` — 重置 API 调用次数。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquota.html
（官方接口英文名 `clearQuota`）。

- **声明接口**：[IMpOpenApiService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/clear_quota`

- **请求体**：`MpClearQuotaRequest` — 清零请求（`appid` 为要被清空的账号的 appid）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"appid":…}`（access_token 在 Query——官方明确「不在 body 中」）。
- **每月 10 次**（与 clear_quota/v2 合计；官方原文「每个账号每月共 10 次清零操作机会， / 清零生效一次即用掉一次机会」）。清空哪类账号的 quota 就需用该账号对应的 token。
- 官方错误码：`0` / `40013`（invalid appid）/ `48006` / （forbid to clear quota because of reaching the limit，清零次数达到上限）。

### 2. `ClearQuotaV2Async` — ClearQuotaV2Async

- **声明接口**：[IMpOpenApiService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/clear_quota/v2`

- **请求体**：`MpClearQuotaV2Request`
- **返回**：`MpResponse`

### 3. `GetApiQuotaAsync` — 查询 API 调用额度。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_getapiquota.html
（官方接口英文名 `getApiQuota`）。

- **声明接口**：[IMpOpenApiService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/openapi/quota/get`

- **请求体**：`MpCgiPathRequest` — 查询请求（`cgi_path` 以 / 开头、不带 https://api.weixin.qq.com 前缀）。
- **返回**：`MpGetApiQuotaResponse` — 额度详情（`quota` 当日额度 + `rate_limit` 频率限制 + `component_rate_limit` 代调用限制）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"cgi_path":…}`；官方原文「不要前缀 / https://api.weixin.qq.com，也不要漏了 /，否则都会 76003 的报错」。
- 官方错误码：`0` / `76021`（cgi_path not found，cgi_path 填错了）/ / `76022`（could not use this cgi_path, no permission——token 与 api 所属账号不符； / 「/xxx/sns/xxx」类接口亦报此码）。

### 4. `ClearApiQuotaAsync` — 重置指定 API 调用次数。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearapiquota.html
（官方接口英文名 `clearApiQuota`）。

- **声明接口**：[IMpOpenApiService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/openapi/quota/clear`

- **请求体**：`MpCgiPathRequest` — 清零请求（`cgi_path` 为目标 api 的请求地址）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"cgi_path":…}`（官方示例以 /channels/ec/ 开头）。
- **每月 50 次**（官方原文「每个账号每月共 50 次清零操作机会」——与 clear_quota 的 10 次独立）。
- 官方错误码：`40001` / `41001` / `42001` / `44002`（POST 数据包为空）/ / `45009` / `50002` / `76021` / `76022`。

### 5. `GetRidInfoAsync` — 查询 rid 信息（排障利器）。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_getridinfo.html
（官方接口英文名 `getRidInfo`）。

- **声明接口**：[IMpOpenApiService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/openapi/rid/get`

- **请求体**：`MpGetRidInfoRequest` — 查询请求（`rid` 为调用接口报错返回的 rid）。
- **返回**：`MpGetRidInfoResponse` — 请求详情（`request`：invoke_time / cost_in_ms / request_url / request_body / response_body / client_ip）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"rid":…}`。
- **有效期与同账号语义（官方「注意事项」原文）**：①「rid 有效期仅 7 天，超过 7 天的 rid / 查询会报错（76001）」；②「查询 rid 属于开发者私密行为，仅支持同账号查询——例如 rid 由账号 A / 调用产生，需用账号 A 的 access_token 查询，用账号 B 查询会报错（76003）」； / ③「/xxx/sns/xxx 这类接口不支持本接口查询（76022）」。
- 官方错误码：`0` / `76001`（rid 不存在）/ `76002`（rid 为空或格式错误）/ / `76003`（无权查询，rid 属其他账号）/ `76004`（rid 过期，仅支持 7 天内）。

### 6. `ClearQuotaV2Async` — 使用 AppSecret 重置 API 调用次数（免 access_token）。官方文档：https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquotabyappsecret.html
（官方接口英文名 `clearQuotaByAppSecret`）。

- **声明接口**：[IMpOpenApiTokenFreeService](./IMpOpenApiService.cs)
- **官方契约**：`POST /cgi-bin/clear_quota/v2`

- **请求体**：`MpClearQuotaV2Request` — 清零请求（`appid` + `appsecret`，均必填；走请求体，不进 URL）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST**；请求体 `{"appid":…,"appsecret":…}` / （参数位置裁决见类型 remarks——官方文档矛盾照录）。
- 官方错误码：`-1` / `40013` / `41002`（appid missing）/ / `41004`（appsecret missing）/ `48006`（清零次数达到上限）。
- **安全边界**：`appsecret` 经请求体提交、绝不进 URL/日志（AGENTS §8； / 宿主侧亦不得将其写入自己的日志/遥测）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/OpenApi`](../../../Mud.Wechat.OfficialAccount/OpenApi/) 内共 **9** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **9** 个，`SerializerClassName` = `OpenApi`。

AOT 源生成上下文：[`OpenApiJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/OpenApiJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount/MpResponse.cs) | `errcode`, `errmsg` |
| `MpClearQuotaRequest` | 请求体 | [OpenApi/MpClearQuotaRequest.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpClearQuotaRequest.cs) | `appid` |
| `MpClearQuotaV2Request` | 请求体 | [OpenApi/MpClearQuotaV2Request.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpClearQuotaV2Request.cs) | `appid`, `appsecret` |
| `MpGetApiQuotaResponse` | 应答 | [OpenApi/MpGetApiQuotaResponse.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpGetApiQuotaResponse.cs) | `quota`, `rate_limit`, `component_rate_limit` |
| `MpCgiPathRequest` | 请求体 | [OpenApi/MpCgiPathRequest.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpCgiPathRequest.cs) | `cgi_path` |
| `MpGetRidInfoResponse` | 应答 | [OpenApi/MpGetRidInfoResponse.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpGetRidInfoResponse.cs) | `request` |
| `MpGetRidInfoRequest` | 请求体 | [OpenApi/MpGetRidInfoRequest.cs](../../../Mud.Wechat.OfficialAccount/OpenApi/MpGetRidInfoRequest.cs) | `rid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.OpenApi;
using Mud.Wechat.OfficialAccount.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddOpenApiApi());

var service = app.Services.GetRequiredService<IMpOpenApiService>();
var ct = CancellationToken.None;
var response = await service.ClearApiQuotaAsync(new MpCgiPathRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/openapi/quota/clear");
// 占位值仅用于编译示意；业务值须按 IMpOpenApiService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ClearApiQuotaAsync`）；请求体 `MpCgiPathRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 部分端点无官方文档备注 | 有端点的方法 XML 注释没有 `<remarks>` 段 ⇒ 该端点的官方页 / 限制未在本仓留档 | 逐页补注释；本文不对其作额外断言 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpOpenApiService](./IMpOpenApiService.cs)、[IMpOpenApiTokenFreeService](./IMpOpenApiService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpOpenApiContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpOpenApiContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)、[`MpRouteCountGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs) |
| 守卫中的代表性测试方法 | `OpenApiEndpoints_ShouldMatchOfficialRoutes`、`TokenFreeEndpoint_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`OpenApiInterfaces_ShouldShareRegistryGroup`、`OpenApiDataModels_ShouldBeRegisteredInJsonContext`、`OpenApiDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`OpenApiErrorCodes_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName`、`MainAssemblyRoutes_ShouldMatchLedger`、`TotalRoutes_ShouldMatchLedger`、`Routes_ShouldNotBeSharedAcrossInterfaces` … |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/OpenApi`](../../../Mud.Wechat.OfficialAccount/OpenApi/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpOpenApiContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`、`MpRouteCountGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

# Qrcode 域 · 服务号二维码 → 带参二维码

微信公众号 / 服务号「服务号二维码 → 带参二维码」域 SDK（1 端点，**服务号专属**）。 接口：[IMpQrcodeService](./IMpQrcodeService.cs)；注册组 `Qrcode`；模块枚举 `MpModule.Qrcode`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpQrcodeJumpContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQrcodeJumpContractGuards.cs)、[MpMassContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount.DataModels/Qrcode`](../../../Mud.Wechat.OfficialAccount.DataModels/Qrcode/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpQrcodeService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpQrcodeService` | `CreateQrcodeAsync` | POST | `/cgi-bin/qrcode/create` | `AccessToken`（Query），Query `access_token` | 生成带参数的二维码（创建二维码 ticket）。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcod… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务号域 https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodes/api_createqrcode.html
（2026-10-07 逐页核验：subscription 订阅号域 404，服务号域命中；官方适用范围为
「服务号（仅认证）」散文形态——与其他域的双列表格写法不同，照录）。

**扫码事件闭环**：官方原文「如果用户还未关注公众号，则用户可以关注公众号，关注后微信会将
带场景值关注事件（`Subscribe`，携带
EventKey/Ticket）推送给开发者。如果用户已经关注公众号……微信会将带场景值扫描事件
（`Scan`）推送给开发者」。

**令牌路由**：消费 `AccessToken` 并以 Query 注入 `access_token`。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateQrcodeAsync` — 生成带参数的二维码（创建二维码 ticket）。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodes/api_createqrcode.html
（官方接口英文名 `createQRCode`）。

- **声明接口**：[IMpQrcodeService](./IMpQrcodeService.cs)
- **官方契约**：`POST /cgi-bin/qrcode/create`

- **请求体**：`MpQrcodeCreateRequest` — 创建请求（action_name 四形态 + scene_id/scene_str 按形态携带其一）。
- **返回**：`MpQrcodeCreateResponse` — 二维码 ticket（`ticket`/`expire_seconds`/`url`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpQrcodeCreateRequest`； / 临时二维码（QR_SCENE/QR_STR_SCENE）须设置 expire_seconds（≤2592000 秒）； / 永久二维码（QR_LIMIT_SCENE/QR_LIMIT_STR_SCENE）无过期时间、数量上限 10 万个。
- **换图（官方原文，SDK 不代下载）**：ticket 换图走 / `GET https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=TICKET`（TICKET 须 UrlEncode； / 无须登录态；正确时返回图片，错误时 HTTP 404）——详见 `MpQrcodeCreateResponse` remarks。
- 官方错误码：`-1` / `40052`（invalid action name，action 值有误）/ / `40053`（invalid action info，官方解决方案列留空，照录）。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount.DataModels/Qrcode`](../../../Mud.Wechat.OfficialAccount.DataModels/Qrcode/) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `Qrcode`。

AOT 源生成上下文：[`QrcodeJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount.DataModels/Generated/QrcodeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpQrcodeCreateResponse` | 应答 | [Qrcode/MpQrcodeCreateResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Qrcode/MpQrcodeCreateResponse.cs) | `ticket`, `expire_seconds`, `url` |
| `MpQrcodeCreateRequest` | 请求体 | [Qrcode/MpQrcodeCreateRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Qrcode/MpQrcodeCreateRequest.cs) | `expire_seconds`, `action_name`, `action_info` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Qrcode;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddQrcodeApi());

var service = app.Services.GetRequiredService<IMpQrcodeService>();
var ct = CancellationToken.None;
var response = await service.CreateQrcodeAsync(new MpQrcodeCreateRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/qrcode/create");
// 占位值仅用于编译示意；业务值须按 IMpQrcodeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateQrcodeAsync`）；请求体 `MpQrcodeCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpQrcodeService](./IMpQrcodeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpQrcodeJumpContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQrcodeJumpContractGuards.cs)、[`MpMassContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `QrcodeJumpEndpoints_ShouldMatchOfficialRoutes`、`QrcodeJumpShapes_ShouldLockOfficialFieldNames`、`QrcodeJumpVoidResponses_ShouldReuseSharedMpResponse`、`QrcodeJumpModuleAndDataModels_ShouldBeRegistered`、`QrcodeJumpConstants_ShouldMatchVerifiedOfficialValues`、`MassEndpoints_ShouldMatchOfficialRoutes`、`MassRequestShapes_ShouldLockBranchDtos`、`OneTimeSubscribe_ShouldLiveInTemplateDomainWithColorField`、`MassDataModels_ShouldBeRegisteredInJsonContext`、`QrcodeEndpoint_ShouldMatchOfficialContract`、`AutoReplyEndpoint_ShouldMatchOfficialContract`、`MassQrcodeAutoReplyModules_ShouldBeRegisteredAndWhitelisted` … |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount.DataModels/Qrcode`](../../../Mud.Wechat.OfficialAccount.DataModels/Qrcode/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpQrcodeJumpContractGuards.cs`、`MpMassContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

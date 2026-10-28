# Security 域 · 内容安全

微信小程序「内容安全」域 SDK（3 端点：文本同步审核 + 音视频异步审核 + 安全风控）。 接口：[IWxaSecurityService](./IWxaSecurityService.cs)；注册组 `Security`；模块枚举 `MiniProgramModule.Security`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/Security`](../../../Mud.Wechat.MiniProgram/Security/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaSecurityService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaSecurityService` | `MsgSecCheckAsync` | POST | `/wxa/msg_sec_check` | `AccessToken`（Query），Query `access_token` | 文本内容安全识别（同步）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/s… |
| 2 | `IWxaSecurityService` | `MediaCheckAsyncAsync` | POST | `/wxa/media_check_async` | `AccessToken`（Query），Query `access_token` | 音视频内容安全识别（**异步受理**）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-c… |
| 3 | `IWxaSecurityService` | `GetUserRiskRankAsync` | POST | `/wxa/getuserriskrank` | `AccessToken`（Query），Query `access_token` | 获取用户安全等级（`risk_rank`）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 内容安全，2026-10-09 核验）：
文本 https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/sec-check/api_msgseccheck、
音视频 https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/sec-check/api_mediacheckasync、
安全风控 https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/safety-control-capability/api_getuserriskrank。

**图片内容安全 `/wxa/img_sec_check` 刻意不实现**：官方文档页
`sec-center/sec-check/api_imgseccheck.html` 经实测为**硬 404**（已下架）；
图片 / 音频 / 视频统一走 `media_check_async`（同步图片审核已无公开入口，不凭旧记忆建模）。

**同步 vs 异步（勿混为一谈）**：`msg_sec_check` 为同步（应答即结论）；
`media_check_async` 仅**受理**并返回 `trace_id`，结论须经
`media_check_async` 的结果**回调**（`wxa_media_check` 事件）或另途查询获取 ——
SDK 只提供端点，**不做「提交 → 轮询 / 等回调」编排**（与公众号线智能接口同一取舍）。

**建议策略（官方原文）**：内容安全接口为**异步/准异步**能力，官方建议
「先审后发」时对**疑似**内容做人工复核；`40001` 走令牌自愈。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `MsgSecCheckAsync` — 文本内容安全识别（同步）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/sec-check/api_msgseccheck。

- **声明接口**：[IWxaSecurityService](./IWxaSecurityService.cs)
- **官方契约**：`POST /wxa/msg_sec_check`

- **请求体**：`WxaMsgSecCheckRequest` — 待检文本与场景，见 `WxaMsgSecCheckRequest`。
- **返回**：`WxaMsgSecCheckResponse` — 审核结论（`result.suggest` / `detail[]` / `trace_id`），见 `WxaMsgSecCheckResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/msg_sec_check` ＋ 请求体 JSON；Query 携带 `access_token`。
- **结论判定（官方原文）**：以 `result.suggest` 为准（`risky` 违规 / `pass` 通过 / / `review` 需人工复核）；`detail[].strategy` 标明命中的策略（内容 / 色情 / 广告等）。 / **注意 `errcode` 为 0 仅表示「调用成功」，不代表内容合规** —— / 违规与否在 `result` 与 `detail` 里，勿据 `errcode` 放行。
- **版本与场景**：`version` = `2` 为当前版本；`scene` 标明调用场景 / （资料 / 评论 / 论坛 / 社交日志），`openid` 为可选的用户标识（同用户累积违规会触发封禁策略）。
- 官方错误码：`40001`（令牌无效，走自愈）/ `43104`（未开通内容安全能力）/ `87014`（内容含违规信息）。

### 2. `MediaCheckAsyncAsync` — 音视频内容安全识别（**异步受理**）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/sec-check/api_mediacheckasync。

- **声明接口**：[IWxaSecurityService](./IWxaSecurityService.cs)
- **官方契约**：`POST /wxa/media_check_async`

- **请求体**：`WxaMediaCheckAsyncRequest` — 媒体地址与类型，见 `WxaMediaCheckAsyncRequest`。
- **返回**：`WxaMediaCheckAsyncResponse` — 仅 `errcode`/`errmsg` 与**查询用** `trace_id`，见 `WxaMediaCheckAsyncResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/media_check_async` ＋ 请求体 JSON；Query 携带 `access_token`。
- **本端点只受理，不给结论**（官方语义）：应答的 `trace_id` 是唯一凭据， / 结论经官方**回调**（`wxa_media_check`）送达，**或**由宿主另途查询。 / SDK **不建轮询**，也**不把 `errcode = 0` 当审核通过**。
- **媒体地址要求（官方原文）**：`media_url` 须为**公网可访问**的链接（微信服务器需回源拉取）， / 内网 / 需鉴权的地址会被官方拒绝。
- 官方错误码：`40001`（令牌无效，走自愈）/ `43104`（未开通）/ `44002`（空请求体）/ `47001`（数据格式错误）。

### 3. `GetUserRiskRankAsync` — 获取用户安全等级（`risk_rank`）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/sec-center/safety-control-capability/api_getuserriskrank。

- **声明接口**：[IWxaSecurityService](./IWxaSecurityService.cs)
- **官方契约**：`POST /wxa/getuserriskrank`

- **请求体**：`WxaGetUserRiskRankRequest` — 用户信息与场景，见 `WxaGetUserRiskRankRequest`。
- **返回**：`WxaGetUserRiskRankResponse` — 风险等级（`risk_rank`，0~4），见 `WxaGetUserRiskRankResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/getuserriskrank` ＋ 请求体 JSON；Query 携带 `access_token`。
- **能力口径（官方原文）**：根据提交的用户信息获取安全等级，**无需用户授权**； / `appid` / `openid` 为用户侧标识，`scene` 默认 `2`（营销活动）。
- **等级语义（勿把数值当布尔）**：`0` 通过 / `1` 未知 / `2` 风险待确认 / / `3` 风险命中 / `4` 高风险命中。
- 官方错误码：`40001`（令牌无效，走自愈）/ `43104`（未开通安全风控能力）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/Security`](../../../Mud.Wechat.MiniProgram/Security/) 内共 **8** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **8** 个，`SerializerClassName` = `Security`。

AOT 源生成上下文：[`SecurityJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/SecurityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaMsgSecCheckResponse` | 应答 | [Security/WxaMsgSecCheckResponse.cs](../../../Mud.Wechat.MiniProgram/Security/WxaMsgSecCheckResponse.cs) | `detail`, `trace_id`, `result` |
| `WxaMsgSecCheckRequest` | 请求体 | [Security/WxaMsgSecCheckRequest.cs](../../../Mud.Wechat.MiniProgram/Security/WxaMsgSecCheckRequest.cs) | `content`, `version`, `scene`, `openid` |
| `WxaMediaCheckAsyncResponse` | 应答 | [Security/WxaMediaCheckAsyncResponse.cs](../../../Mud.Wechat.MiniProgram/Security/WxaMediaCheckAsyncResponse.cs) | `trace_id` |
| `WxaMediaCheckAsyncRequest` | 请求体 | [Security/WxaMediaCheckAsyncRequest.cs](../../../Mud.Wechat.MiniProgram/Security/WxaMediaCheckAsyncRequest.cs) | `media_url`, `media_type`, `version`, `scene`, `openid` |
| `WxaGetUserRiskRankResponse` | 应答 | [Security/WxaGetUserRiskRankResponse.cs](../../../Mud.Wechat.MiniProgram/Security/WxaGetUserRiskRankResponse.cs) | `risk_rank` |
| `WxaGetUserRiskRankRequest` | 请求体 | [Security/WxaGetUserRiskRankRequest.cs](../../../Mud.Wechat.MiniProgram/Security/WxaGetUserRiskRankRequest.cs) | `appid`, `openid`, `scene`, `mobile_no`, `client_ip`, `email_address`, `extended_info` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Security;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddSecurityApi());

var service = app.Services.GetRequiredService<IWxaSecurityService>();
var ct = CancellationToken.None;
var response = await service.GetUserRiskRankAsync(new WxaGetUserRiskRankRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/getuserriskrank");
// 占位值仅用于编译示意；业务值须按 IWxaSecurityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetUserRiskRankAsync`）；请求体 `WxaGetUserRiskRankRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaSecurityService](./IWxaSecurityService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/Security`](../../../Mud.Wechat.MiniProgram/Security/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

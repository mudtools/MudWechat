# Semantic 域 · 语义理解（智能对话旧接口）

微信公众号 / 服务号「语义理解（智能对话旧接口）」域 SDK
（`/semantic/semproxy/search` 单端点——按服务类型对自然语言做结构化理解）。 接口：[IMpSemanticService](./IMpSemanticService.cs)；注册组 `Semantic`；模块枚举 `MpModule.Semantic`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpSemanticContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSemanticContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/Semantic`](../../../Mud.Wechat.OfficialAccount/Semantic/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpSemanticService` | `SearchAsync` | POST | `/semantic/semproxy/search` | `AccessToken`（Query），Query `access_token` | 语义理解检索。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Interface/Natural… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**停维警示**：本域是微信「智能对话」旧接口，官方长期未迭代；新项目应改用微信智能对话平台。
SDK 按官方原样承载——`Details` 以原始 JSON 透出
（官方 details 结构随服务类型有二十余种形态，逐形态建模会引入开放多态 DTO，
与本仓「不做运行时多态」红线冲突，裁决见 `MpSemanticResult`）。

MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；URL 遥测已由组件 `SensitiveUrlRedactor` 与 `MpException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SearchAsync` — 语义理解检索。官方文档：https://developers.weixin.qq.com/doc/offiaccount/Intelligent_Interface/Natural_Language_Processing.html
（官方接口英文名 `semproxySearch`）。

- **声明接口**：[IMpSemanticService](./IMpSemanticService.cs)
- **官方契约**：`POST /semantic/semproxy/search`

- **请求体**：`MpSemanticSearchRequest` — 检索请求（`query` + `type` 必填；位置上下文可选）。
- **返回**：`MpSemanticSearchResponse` — 语义结果（intent / query / type + 原样 details，解析见 `MpSemanticResult`）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；`type` 取值见官方服务分类表（weather / travel / music 等）。
- 官方错误码：`60010` / `60011` / `60015`（本页错误码表照录）。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/Semantic`](../../../Mud.Wechat.OfficialAccount/Semantic/) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `Semantic`。

AOT 源生成上下文：[`SemanticJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/SemanticJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpSemanticSearchResponse` | 应答 | [Semantic/MpSemanticSearchResponse.cs](../../../Mud.Wechat.OfficialAccount/Semantic/MpSemanticSearchResponse.cs) | `semantic` |
| `MpSemanticSearchRequest` | 请求体 | [Semantic/MpSemanticSearchRequest.cs](../../../Mud.Wechat.OfficialAccount/Semantic/MpSemanticSearchRequest.cs) | `query`, `type`, `latitude`, `longitude`, `city`, `region`, `appid`, `uid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Semantic;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddSemanticApi());

var service = app.Services.GetRequiredService<IMpSemanticService>();
var ct = CancellationToken.None;
var response = await service.SearchAsync(new MpSemanticSearchRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/semantic/semproxy/search");
// 占位值仅用于编译示意；业务值须按 IMpSemanticService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SearchAsync`）；请求体 `MpSemanticSearchRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpSemanticService](./IMpSemanticService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpSemanticContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSemanticContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `SemanticEndpoints_ShouldMatchOfficialRoutes`、`SemanticDataModels_ShouldBeRegisteredInJsonContext_AndKeepRawDetails`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/Semantic`](../../../Mud.Wechat.OfficialAccount/Semantic/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpSemanticContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

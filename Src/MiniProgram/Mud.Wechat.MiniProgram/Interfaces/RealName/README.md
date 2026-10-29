# RealName 域 · 实名认证

小程序线「实名认证」域接口（1 端点）。 接口：[IWxaRealNameService](./IWxaRealNameService.cs)；注册组 `RealName`；模块枚举 `MiniProgramModule.RealName`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/RealName`](../../DataModels/RealName)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaRealNameService` | `CheckRealNameInfoAsync` | POST | `/intp/realname/checkrealnameinfo` | `AccessToken`（Query），Query `access_token` | 校验实名信息（业务方页面获得 code 之后，通过该接口进行实名信息的校验）。 |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/basic/api_checkrealnameinfo.html。

权限集未明确；不支持第三方平台代调用；官方未给出端点级频率数字。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CheckRealNameInfoAsync` — 校验实名信息（业务方页面获得 code 之后，通过该接口进行实名信息的校验）。

- **声明接口**：[IWxaRealNameService](./IWxaRealNameService.cs)
- **官方契约**：`POST /intp/realname/checkrealnameinfo`

- **请求体**：`WxaRealNameCheckRequest` — 请求体。
- **返回**：`WxaRealNameCheckResponse` — 校验结果（含校验后的 openid 与真实姓名）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/RealName`](../../DataModels/RealName) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `RealName`。

AOT 源生成上下文：[`RealNameJsonContext.g.cs`](./../../DataModels/Generated/RealNameJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaRealNameCheckResponse` | 应答 | [RealName/WxaRealNameCheck.cs](../../DataModels/RealName/WxaRealNameCheck.cs) | `openid`, `real_name`, `cred_id`, `cred_type`, `code`, `verify_openid`, `verify_real_name` |
| `WxaRealNameCheckRequest` | 请求体 | [RealName/WxaRealNameCheck.cs](../../DataModels/RealName/WxaRealNameCheck.cs) | `openid`, `real_name`, `cred_id`, `cred_type`, `code`, `verify_openid`, `verify_real_name` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.RealName;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddRealNameApi());

var service = app.Services.GetRequiredService<IWxaRealNameService>();
var ct = CancellationToken.None;
var response = await service.CheckRealNameInfoAsync(new WxaRealNameCheckRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/intp/realname/checkrealnameinfo");
// 占位值仅用于编译示意；业务值须按 IWxaRealNameService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CheckRealNameInfoAsync`）；请求体 `WxaRealNameCheckRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 部分端点无官方文档备注 | 有端点的方法 XML 注释没有 `<remarks>` 段 ⇒ 该端点的官方页 / 限制未在本仓留档 | 逐页补注释；本文不对其作额外断言 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaRealNameService](./IWxaRealNameService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/RealName`](../../DataModels/RealName) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

# LaborUse 域 · 用工关系

微信小程序「用工关系」域 SDK（2 端点：推送用工消息 + 解绑用工关系）。 接口：[IWxaLaborUseService](./IWxaLaborUseService.cs)；注册组 `LaborUse`；模块枚举 `MiniProgramModule.LaborUse`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/LaborUse`](../../../Mud.Wechat.MiniProgram/LaborUse/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaLaborUseService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaLaborUseService` | `SendEmployeeRelationMessageAsync` | POST | `/cgi-bin/message/wxopen/employeerelationmsg/send` | `AccessToken`（Query），Query `access_token` | 推送用工消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/laboruse/api_sende… |
| 2 | `IWxaLaborUseService` | `UnbindUserB2CAuthInfoAsync` | POST | `/wxa/business/unbinduserb2cauthinfo` | `AccessToken`（Query），Query `access_token` | 解绑用工关系（批量解绑用户的 B2C 授权信息）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 用工关系，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/laboruse/api_sendemployeerelationmsg、https://developers.weixin.qq.com/miniprogram/dev/server/API/laboruse/api_unbinduserb2cauthinfo。

**业务形态（官方原文）**：用工关系面向「用工单位 + 劳动者」的小程序用工协作场景；
企业可向已建立用工关联的用户推送消息，或按需解绑用户的 B2C 授权信息。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。两端点官方均为 **POST**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SendEmployeeRelationMessageAsync` — 推送用工消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/laboruse/api_sendemployeerelationmsg。

- **声明接口**：[IWxaLaborUseService](./IWxaLaborUseService.cs)
- **官方契约**：`POST /cgi-bin/message/wxopen/employeerelationmsg/send`

- **请求体**：`DataModels.LaborUse.WxaSendEmployeeRelationMessageRequest` — 推送请求（`template_id` / `touser` 必填），见 `WxaSendEmployeeRelationMessageRequest`。
- **返回**：`DataModels.WxaResponse` — 标准应答（`errcode` / `errmsg`），见 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/message/wxopen/employeerelationmsg/send` ＋ 请求体 JSON；Query 携带 `access_token`。
- **推送语义（官方原文）**：仅可向**已建立用工关联**的用户推送； / `data` 为键值对 JSON 字符串（模板填充数据）。

### 2. `UnbindUserB2CAuthInfoAsync` — 解绑用工关系（批量解绑用户的 B2C 授权信息）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/laboruse/api_unbinduserb2cauthinfo。

- **声明接口**：[IWxaLaborUseService](./IWxaLaborUseService.cs)
- **官方契约**：`POST /wxa/business/unbinduserb2cauthinfo`

- **请求体**：`DataModels.LaborUse.WxaUnbindUserB2CAuthInfoRequest` — 解绑请求（`openid_list` 必填），见 `WxaUnbindUserB2CAuthInfoRequest`。
- **返回**：`DataModels.WxaResponse` — 标准应答（`errcode` / `errmsg`），见 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/unbinduserb2cauthinfo` ＋ 请求体 JSON；Query 携带 `access_token`。
- **批量语义（官方原文）**：`openid_list` 为待解绑用户列表（非空，单次上限以官方页面为准）；解绑后无法撤回。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/LaborUse`](../../../Mud.Wechat.MiniProgram/LaborUse/) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `LaborUse`。

AOT 源生成上下文：[`LaborUseJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/LaborUseJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaSendEmployeeRelationMessageRequest` | 请求体 | [LaborUse/WxaSendEmployeeRelationMessageRequest.cs](../../../Mud.Wechat.MiniProgram/LaborUse/WxaSendEmployeeRelationMessageRequest.cs) | `template_id`, `touser`, `page`, `data` |
| `WxaUnbindUserB2CAuthInfoRequest` | 请求体 | [LaborUse/WxaUnbindUserB2CAuthInfoRequest.cs](../../../Mud.Wechat.MiniProgram/LaborUse/WxaUnbindUserB2CAuthInfoRequest.cs) | `openid_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.LaborUse;
using Mud.Wechat.MiniProgram.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddLaborUseApi());

var service = app.Services.GetRequiredService<IWxaLaborUseService>();
var ct = CancellationToken.None;
var response = await service.SendEmployeeRelationMessageAsync(new WxaSendEmployeeRelationMessageRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/cgi-bin/message/wxopen/employeerelationmsg/send");
// 占位值仅用于编译示意；业务值须按 IWxaLaborUseService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SendEmployeeRelationMessageAsync`）；请求体 `WxaSendEmployeeRelationMessageRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaLaborUseService](./IWxaLaborUseService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/LaborUse`](../../../Mud.Wechat.MiniProgram/LaborUse/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

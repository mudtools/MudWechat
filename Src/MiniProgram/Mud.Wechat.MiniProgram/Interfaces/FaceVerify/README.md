# FaceVerify 域 · 微信人脸核身

微信小程序「微信人脸核身」域 SDK（2 端点：获取人脸核身会话唯一标识 + 查询真实验证结果）。 接口：[IWxaFaceVerifyService](./IWxaFaceVerifyService.cs)；注册组 `FaceVerify`；模块枚举 `MiniProgramModule.FaceVerify`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/FaceVerify`](../../../Mud.Wechat.MiniProgram/FaceVerify/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaFaceVerifyService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaFaceVerifyService` | `GetVerifyIdAsync` | POST | `/cityservice/face/identify/getverifyid` | `AccessToken`（Query），Query `access_token` | 获取用户人脸核身会话唯一标识。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_g… |
| 2 | `IWxaFaceVerifyService` | `QueryVerifyInfoAsync` | POST | `/cityservice/face/identify/queryverifyinfo` | `AccessToken`（Query），Query `access_token` | 查询用户人脸核身真实验证结果。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_q… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 微信人脸核身，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_getverifyid、https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_queryverifyinfo。

**业务形态（官方原文）**：业务方后台根据「用户实名信息（姓名 + 证件号）」调用
`GetVerifyIdAsync` 获取人脸核身会话唯一标识 `verify_id`，交给小程序前端拉起
人脸核身；核身完成后经 `QueryVerifyInfoAsync` 查询**真实**验证结果。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。两端点官方均为 **POST**；
官方标注本域**不支持第三方平台代调用**。

**敏感信息警示（MP-X7）**：`cert_info` 含**证件姓名与证件号码**等个人敏感信息，
禁止写入日志、遥测或异常消息；调用完成即应丢弃。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetVerifyIdAsync` — 获取用户人脸核身会话唯一标识。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_getverifyid。

- **声明接口**：[IWxaFaceVerifyService](./IWxaFaceVerifyService.cs)
- **官方契约**：`POST /cityservice/face/identify/getverifyid`

- **请求体**：`DataModels.FaceVerify.WxaFaceGetVerifyIdRequest` — 获取请求（`out_seq_no` / `openid` / `cert_info` 必填），见 `WxaFaceGetVerifyIdRequest`。
- **返回**：`DataModels.FaceVerify.WxaFaceGetVerifyIdResponse` — 会话唯一标识（`verify_id`）与其有效期（`expires_in`），见 `WxaFaceGetVerifyIdResponse`。

**备注（原文）**

- 官方契约：**POST** `/cityservice/face/identify/getverifyid` ＋ 请求体 JSON；Query 携带 `access_token`。
- **流程（官方原文）**：后台凭用户实名信息（姓名 + 证件号）调用本接口取得 `verify_id`， / 再交给小程序前端调用人脸核身组件完成核身；`verify_id` 带有效期（`expires_in`，秒）。
- **敏感信息警示**：`cert_info`（证件姓名/号码）禁止写入日志、遥测或异常消息。

### 2. `QueryVerifyInfoAsync` — 查询用户人脸核身真实验证结果。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/face/api_queryverifyinfo。

- **声明接口**：[IWxaFaceVerifyService](./IWxaFaceVerifyService.cs)
- **官方契约**：`POST /cityservice/face/identify/queryverifyinfo`

- **请求体**：`DataModels.FaceVerify.WxaFaceQueryVerifyInfoRequest` — 查询请求（`out_seq_no` / `verify_id` / `openid` / `cert_hash` 必填），见 `WxaFaceQueryVerifyInfoRequest`。
- **返回**：`DataModels.FaceVerify.WxaFaceQueryVerifyInfoResponse` — 验证结果码（`verify_ret`），见 `WxaFaceQueryVerifyInfoResponse`。

**备注（原文）**

- 官方契约：**POST** `/cityservice/face/identify/queryverifyinfo` ＋ 请求体 JSON；Query 携带 `access_token`。
- **核身通过判据（官方原文）**：须**同时**满足 `errcode == 0` 与 `verify_ret == 10000`。
- **证件摘要**：`cert_hash` 由 `SetCertHash` 计算（内存内哈希，不记录明文）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/FaceVerify`](../../../Mud.Wechat.MiniProgram/FaceVerify/) 内共 **5** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **5** 个，`SerializerClassName` = `FaceVerify`。

AOT 源生成上下文：[`FaceVerifyJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/FaceVerifyJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaFaceGetVerifyIdResponse` | 应答 | [FaceVerify/WxaFaceGetVerifyIdResponse.cs](../../../Mud.Wechat.MiniProgram/FaceVerify/WxaFaceGetVerifyIdResponse.cs) | `verify_id`, `expires_in` |
| `WxaFaceGetVerifyIdRequest` | 请求体 | [FaceVerify/WxaFaceGetVerifyIdRequest.cs](../../../Mud.Wechat.MiniProgram/FaceVerify/WxaFaceGetVerifyIdRequest.cs) | `out_seq_no`, `openid`, `cert_info` |
| `WxaFaceQueryVerifyInfoResponse` | 应答 | [FaceVerify/WxaFaceQueryVerifyInfoResponse.cs](../../../Mud.Wechat.MiniProgram/FaceVerify/WxaFaceQueryVerifyInfoResponse.cs) | `verify_ret` |
| `WxaFaceQueryVerifyInfoRequest` | 请求体 | [FaceVerify/WxaFaceQueryVerifyInfoRequest.cs](../../../Mud.Wechat.MiniProgram/FaceVerify/WxaFaceQueryVerifyInfoRequest.cs) | `out_seq_no`, `verify_id`, `openid`, `cert_hash` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.FaceVerify;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddFaceVerifyApi());

var service = app.Services.GetRequiredService<IWxaFaceVerifyService>();
var ct = CancellationToken.None;
var response = await service.GetVerifyIdAsync(new WxaFaceGetVerifyIdRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/cityservice/face/identify/getverifyid");
// 占位值仅用于编译示意；业务值须按 IWxaFaceVerifyService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetVerifyIdAsync`）；请求体 `WxaFaceGetVerifyIdRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaFaceVerifyService](./IWxaFaceVerifyService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/FaceVerify`](../../../Mud.Wechat.MiniProgram/FaceVerify/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

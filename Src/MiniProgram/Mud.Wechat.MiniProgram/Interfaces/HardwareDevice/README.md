# HardwareDevice 域 · 硬件设备

微信小程序「硬件设备」域 SDK（9 端点：设备消息 1 + 设备票据 1 + 设备组 4 + license 3）。 接口：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)；注册组 `HardwareDevice`；模块枚举 `MiniProgramModule.HardwareDevice`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/HardwareDevice`](../../../Mud.Wechat.MiniProgram/HardwareDevice/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaHardwareDeviceService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaHardwareDeviceService` | `SendDeviceMessageAsync` | POST | `/cgi-bin/message/device/subscribe/send` | `AccessToken`（Query），Query `access_token` | 发送设备消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/ap… |
| 2 | `IWxaHardwareDeviceService` | `GetSnTicketAsync` | POST | `/wxa/getsnticket` | `AccessToken`（Query），Query `access_token` | 获取设备票据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/ap… |
| 3 | `IWxaHardwareDeviceService` | `CreateIoTGroupIdAsync` | POST | `/wxa/business/group/createid` | `AccessToken`（Query），Query `access_token` | 创建设备组。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api… |
| 4 | `IWxaHardwareDeviceService` | `GetIoTGroupInfoAsync` | POST | `/wxa/business/group/getinfo` | `AccessToken`（Query），Query `access_token` | 查询设备组信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/a… |
| 5 | `IWxaHardwareDeviceService` | `AddIoTGroupDeviceAsync` | POST | `/wxa/business/group/adddevice` | `AccessToken`（Query），Query `access_token` | 设备组添加设备。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/a… |
| 6 | `IWxaHardwareDeviceService` | `RemoveIoTGroupDeviceAsync` | POST | `/wxa/business/group/removedevice` | `AccessToken`（Query），Query `access_token` | 设备组删除设备。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/a… |
| 7 | `IWxaHardwareDeviceService` | `GetLicensePkgListAsync` | POST | `/wxa/business/license/getpkglist` | `AccessToken`（Query），Query `access_token` | 查询 license 资源包列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware… |
| 8 | `IWxaHardwareDeviceService` | `ActiveLicenseDeviceAsync` | POST | `/wxa/business/license/activedevice` | `AccessToken`（Query），Query `access_token` | 激活设备 license。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-dev… |
| 9 | `IWxaHardwareDeviceService` | `GetLicenseDeviceInfoAsync` | POST | `/wxa/business/license/getdeviceinfo` | `AccessToken`（Query），Query `access_token` | 查询设备激活详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 硬件设备，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/ 系列（发送设备消息 / 获取设备票据 / 设备组 / license 资源包）。

**能力口径**：设备消息走「设备订阅」模板（接收者为 `tousers` 数组，区别于订阅消息的
单 `touser`）；设备票据 5 分钟内有效；设备组便于用户一次性订阅多个设备；license 资源包
支持批量激活设备（消耗激活码序号）与批量查询剩余有效期。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。所有端点官方均为 **POST**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SendDeviceMessageAsync` — 发送设备消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_sendhardwaredevicemessage。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /cgi-bin/message/device/subscribe/send`

- **请求体**：`DataModels.HardwareDevice.WxaDeviceMessageSendRequest` — 接收者列表与模板内容（`tousers` 必填），见 `WxaDeviceMessageSendRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示发送成功）。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/message/device/subscribe/send` ＋ 请求体 JSON；Query 携带 `access_token`。
- **前置约束（官方原文）**：接收者须已订阅该设备消息模板；`data` 键与模板关键词不一致报 `47003`。

### 2. `GetSnTicketAsync` — 获取设备票据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_getsnticket。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/getsnticket`
- **返回**：`DataModels.HardwareDevice.WxaSnTicketResponse` — 设备票据（`sn_ticket`，**5 分钟内有效**），见 `WxaSnTicketResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/getsnticket`（请求体可为空对象）；Query 携带 `access_token`。
- **票据用途（官方原文）**：用于生成设备码；过期后需重新获取。

### 3. `CreateIoTGroupIdAsync` — 创建设备组。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_createiotgroupid。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/group/createid`
- **返回**：`DataModels.HardwareDevice.WxaIoTGroupCreateResponse` — 设备组 ID 与名称，见 `WxaIoTGroupCreateResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/group/createid`（请求体可为空对象）；Query 携带 `access_token`。
- **能力口径（官方原文）**：设备组用于**便于用户一次性订阅多个设备**。

### 4. `GetIoTGroupInfoAsync` — 查询设备组信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_getiotgroupinfo。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/group/getinfo`

- **请求体**：`DataModels.HardwareDevice.WxaIoTGroupInfoRequest` — 设备组 ID（`group_id`），见 `WxaIoTGroupInfoRequest`。
- **返回**：`DataModels.HardwareDevice.WxaIoTGroupInfoResponse` — 设备组信息与组内设备列表，见 `WxaIoTGroupInfoResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/group/getinfo` ＋ 请求体 JSON；Query 携带 `access_token`。

### 5. `AddIoTGroupDeviceAsync` — 设备组添加设备。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_addiotgroupdevice。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/group/adddevice`

- **请求体**：`DataModels.HardwareDevice.WxaIoTGroupDevicesRequest` — 设备组 ID 与待添加设备列表，见 `WxaIoTGroupDevicesRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/group/adddevice` ＋ 请求体 JSON；Query 携带 `access_token`。

### 6. `RemoveIoTGroupDeviceAsync` — 设备组删除设备。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_removeiotgroupdevice。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/group/removedevice`

- **请求体**：`DataModels.HardwareDevice.WxaIoTGroupDevicesRequest` — 设备组 ID 与待删除设备列表，见 `WxaIoTGroupDevicesRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/group/removedevice` ＋ 请求体 JSON；Query 携带 `access_token`。

### 7. `GetLicensePkgListAsync` — 查询 license 资源包列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_getlicensepkglist。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/license/getpkglist`
- **返回**：`DataModels.HardwareDevice.WxaLicensePkgListResponse` — 已购买的 license 资源包列表，见 `WxaLicensePkgListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/license/getpkglist`（请求体可为空对象）；Query 携带 `access_token`。

### 8. `ActiveLicenseDeviceAsync` — 激活设备 license。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_activelicensedevice。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/license/activedevice`

- **请求体**：`DataModels.HardwareDevice.WxaLicenseActivateRequest` — 资源包 ID 与待激活设备序列号列表，见 `WxaLicenseActivateRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/license/activedevice` ＋ 请求体 JSON；Query 携带 `access_token`。
- **能力口径（官方原文）**：批量绑定设备，并消耗相应资源包中的激活码序号 —— 设备数与剩余激活数不匹配会失败。

### 9. `GetLicenseDeviceInfoAsync` — 查询设备激活详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/hardware-device/api_getlicensedeviceinfo。

- **声明接口**：[IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs)
- **官方契约**：`POST /wxa/business/license/getdeviceinfo`

- **请求体**：`DataModels.HardwareDevice.WxaLicenseDeviceInfoRequest` — 设备序列号列表（`sn_list`），见 `WxaLicenseDeviceInfoRequest`。
- **返回**：`DataModels.HardwareDevice.WxaLicenseDeviceInfoResponse` — 批量设备剩余有效期，见 `WxaLicenseDeviceInfoResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/business/license/getdeviceinfo` ＋ 请求体 JSON；Query 携带 `access_token`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/HardwareDevice`](../../../Mud.Wechat.MiniProgram/HardwareDevice/) 内共 **14** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **14** 个，`SerializerClassName` = `HardwareDevice`。

AOT 源生成上下文：[`HardwareDeviceJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/HardwareDeviceJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaDeviceMessageSendRequest` | 请求体 | [HardwareDevice/WxaDeviceMessageSendRequest.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaDeviceMessageSendRequest.cs) | `tousers`, `template_id`, `page`, `miniprogram_state`, `lang`, `data` |
| `WxaSnTicketResponse` | 应答 | [HardwareDevice/WxaSnTicketResponse.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaSnTicketResponse.cs) | `sn_ticket`, `expires_in` |
| `WxaIoTGroupCreateResponse` | 应答 | [HardwareDevice/WxaIoTGroupCreateResponse.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaIoTGroupCreateResponse.cs) | `group_id`, `group_name` |
| `WxaIoTGroupInfoResponse` | 应答 | [HardwareDevice/WxaIoTGroupInfoResponse.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaIoTGroupInfoResponse.cs) | `group_id`, `group_name`, `device_list` |
| `WxaIoTGroupInfoRequest` | 请求体 | [HardwareDevice/WxaIoTGroupInfoRequest.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaIoTGroupInfoRequest.cs) | `group_id` |
| `WxaIoTGroupDevicesRequest` | 请求体 | [HardwareDevice/WxaIoTGroupDevicesRequest.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaIoTGroupDevicesRequest.cs) | `group_id`, `device_list` |
| `WxaLicensePkgListResponse` | 应答 | [HardwareDevice/WxaLicensePkgListResponse.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaLicensePkgListResponse.cs) | `pkg_list` |
| `WxaLicenseActivateRequest` | 请求体 | [HardwareDevice/WxaLicenseActivateRequest.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaLicenseActivateRequest.cs) | `pkg_id`, `sn_list` |
| `WxaLicenseDeviceInfoResponse` | 应答 | [HardwareDevice/WxaLicenseDeviceInfoResponse.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaLicenseDeviceInfoResponse.cs) | `device_list` |
| `WxaLicenseDeviceInfoRequest` | 请求体 | [HardwareDevice/WxaLicenseDeviceInfoRequest.cs](../../../Mud.Wechat.MiniProgram/HardwareDevice/WxaLicenseDeviceInfoRequest.cs) | `sn_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.HardwareDevice;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddHardwareDeviceApi());

var service = app.Services.GetRequiredService<IWxaHardwareDeviceService>();
var ct = CancellationToken.None;
var response = await service.CreateIoTGroupIdAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/business/group/createid");
// 占位值仅用于编译示意；业务值须按 IWxaHardwareDeviceService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateIoTGroupIdAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaHardwareDeviceService](./IWxaHardwareDeviceService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/HardwareDevice`](../../../Mud.Wechat.MiniProgram/HardwareDevice/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

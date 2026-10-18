# NearbyPoi 域 · 附近小程序

微信小程序「附近小程序」域 SDK（4 端点：添加地点 / 删除地点 / 查看地点列表 / 设置展示状态）。 接口：[IWxaNearbyPoiService](./IWxaNearbyPoiService.cs)；注册组 `NearbyPoi`；模块枚举 `MiniProgramModule.NearbyPoi`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram.DataModels/NearbyPoi`](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaNearbyPoiService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaNearbyPoiService` | `AddAsync` | POST | `/wxa/addnearbypoi` | `AccessToken`（Query），Query `access_token` | 添加地点（附近小程序，提交审核）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-p… |
| 2 | `IWxaNearbyPoiService` | `DeleteAsync` | POST | `/wxa/delnearbypoi` | `AccessToken`（Query），Query `access_token` | 删除地点（附近小程序）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/ap… |
| 3 | `IWxaNearbyPoiService` | `GetListAsync` | POST | `/wxa/getnearbypoilist` | `AccessToken`（Query），Query `access_token` | 查看地点列表（附近小程序，分页）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-p… |
| 4 | `IWxaNearbyPoiService` | `SetShowStatusAsync` | POST | `/wxa/setnearbypoishowstatus` | `AccessToken`（Query），Query `access_token` | 设置展示状态（附近小程序，展示 / 取消展示）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/n… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 附近小程序，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_addnearbypoi、https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_deletenearbypoi、
https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_getnearbypoilist、https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_setshowstatus。

**业务形态（官方原文）**：附近小程序是面向「线下门店」的场景能力——将小程序地点挂到官方
地图组件（如「附近」页），用户进入附近页即可在小程序列表看到该地点。添加后进入**审核**流程
（审核结果以官方页面为准，SDK 不做本地模拟）；`poi_id` 由添加结果返回，删除/设置展示状态
均以它为键。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。四端点官方均为 **POST**。
本域不做二进制通道（响应均为 JSON）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `AddAsync` — 添加地点（附近小程序，提交审核）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_addnearbypoi。

- **声明接口**：[IWxaNearbyPoiService](./IWxaNearbyPoiService.cs)
- **官方契约**：`POST /wxa/addnearbypoi`

- **请求体**：`DataModels.NearbyPoi.WxaAddNearbyPoiRequest` — 添加请求（`is_show` / `categories` / `lat` / `lng` 等必填），见 `WxaAddNearbyPoiRequest`。
- **返回**：`DataModels.WxaResponse` — 标准应答（`errcode` / `errmsg`），见 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/addnearbypoi` ＋ 请求体 JSON；Query 携带 `access_token`。
- **必填要点（官方原文）**：`is_show`（是否展示）、`categories`（门店类目）、 / `address`/`district`/`city`/`province`、`lat`/`lng`（十进制经纬度）、 / `media_id`（小程序**永久素材**媒体 ID，经 `material/addMaterial` 上传）、 / `poi_id`（商户平台创建的门店 ID）。`kf_info` / `store_info` 选填。
- **审核流转**：添加成功后进入审核，审核结果以官方页面为准；本接口**不返回审核结论**。

### 2. `DeleteAsync` — 删除地点（附近小程序）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_deletenearbypoi。

- **声明接口**：[IWxaNearbyPoiService](./IWxaNearbyPoiService.cs)
- **官方契约**：`POST /wxa/delnearbypoi`

- **请求体**：`DataModels.NearbyPoi.WxaDeleteNearbyPoiRequest` — 删除请求（`poi_id` 必填），见 `WxaDeleteNearbyPoiRequest`。
- **返回**：`DataModels.WxaResponse` — 标准应答（`errcode` / `errmsg`），见 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/delnearbypoi` ＋ 请求体 JSON；Query 携带 `access_token`。
- **键来源（官方原文）**：`poi_id` 取自已添加的地点（`AddAsync` 成功后返回）， / SDK 不做本地记忆；删除后不可恢复。

### 3. `GetListAsync` — 查看地点列表（附近小程序，分页）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_getnearbypoilist。

- **声明接口**：[IWxaNearbyPoiService](./IWxaNearbyPoiService.cs)
- **官方契约**：`POST /wxa/getnearbypoilist`

- **请求体**：`DataModels.NearbyPoi.WxaNearbyPoiListRequest` — 分页查询请求（`page` 从 `0` 开始），见 `WxaNearbyPoiListRequest`。
- **返回**：`DataModels.NearbyPoi.WxaNearbyPoiListResponse` — 地点分页列表，见 `WxaNearbyPoiListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/getnearbypoilist` ＋ 请求体 JSON；Query 携带 `access_token`。
- **分页（官方原文）**：`page` / `page_rows` 为必填分页参数，`page` 从 `0` 开始；每页条数上限以官方页面为准。

### 4. `SetShowStatusAsync` — 设置展示状态（附近小程序，展示 / 取消展示）。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/nearby-poi/api_setshowstatus。

- **声明接口**：[IWxaNearbyPoiService](./IWxaNearbyPoiService.cs)
- **官方契约**：`POST /wxa/setnearbypoishowstatus`

- **请求体**：`DataModels.NearbyPoi.WxaSetNearbyPoiShowStatusRequest` — 状态请求（`poi_id` + `status` 必填），见 `WxaSetNearbyPoiShowStatusRequest`。
- **返回**：`DataModels.WxaResponse` — 标准应答（`errcode` / `errmsg`），见 `WxaResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/setnearbypoishowstatus` ＋ 请求体 JSON；Query 携带 `access_token`。
- **状态语义（官方原文）**：`status` = `1` 展示 / `0` 取消展示；仅对已通过审核的地点生效。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram.DataModels/NearbyPoi`](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/) 内共 **8** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **8** 个，`SerializerClassName` = `NearbyPoi`。

AOT 源生成上下文：[`NearbyPoiJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram.DataModels/Generated/NearbyPoiJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaAddNearbyPoiRequest` | 请求体 | [NearbyPoi/WxaAddNearbyPoiRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/WxaAddNearbyPoiRequest.cs) | `is_show`, `categories`, `address`, `district`, `city`, `province`, `lat`, `lng`, `media_id`, `poi_id`, `kf_info`, `store_info` |
| `WxaDeleteNearbyPoiRequest` | 请求体 | [NearbyPoi/WxaDeleteNearbyPoiRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/WxaDeleteNearbyPoiRequest.cs) | `poi_id` |
| `WxaNearbyPoiListResponse` | 应答 | [NearbyPoi/WxaNearbyPoiListResponse.cs](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/WxaNearbyPoiListResponse.cs) | `list` |
| `WxaNearbyPoiListRequest` | 请求体 | [NearbyPoi/WxaNearbyPoiListRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/WxaNearbyPoiListRequest.cs) | `page`, `page_rows` |
| `WxaSetNearbyPoiShowStatusRequest` | 请求体 | [NearbyPoi/WxaSetNearbyPoiShowStatusRequest.cs](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/WxaSetNearbyPoiShowStatusRequest.cs) | `poi_id`, `status` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.NearbyPoi;
using Mud.Wechat.MiniProgram.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddNearbyPoiApi());

var service = app.Services.GetRequiredService<IWxaNearbyPoiService>();
var ct = CancellationToken.None;
var response = await service.AddAsync(new WxaAddNearbyPoiRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/addnearbypoi");
// 占位值仅用于编译示意；业务值须按 IWxaNearbyPoiService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddAsync`）；请求体 `WxaAddNearbyPoiRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaNearbyPoiService](./IWxaNearbyPoiService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram.DataModels/NearbyPoi`](../../../Mud.Wechat.MiniProgram.DataModels/NearbyPoi/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

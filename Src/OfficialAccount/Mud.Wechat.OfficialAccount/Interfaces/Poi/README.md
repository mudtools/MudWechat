# Poi 域 · 微信门店（旧版 POI）

微信公众号 / 服务号「微信门店（旧版 POI）」域 SDK
（查询门店 / 查询门店列表 / 删除门店——官方「微信门店」分组的查询与删除面；
门店的新建 / 更新走官方后台或小程序店铺 API，官方未开放 HTTP 新建入口）。 接口：[IMpPoiService](./IMpPoiService.cs)；注册组 `Poi`；模块枚举 `MpModule.Poi`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpPoiContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpPoiContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/DataModels/Poi`](../../DataModels/Poi)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpPoiService` | `GetPoiAsync` | POST | `/cgi-bin/poi/getpoi` | `AccessToken`（Query），Query `access_token` | 查询门店信息。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_In… |
| 2 | `IMpPoiService` | `GetPoiListAsync` | POST | `/cgi-bin/poi/getpoilist` | `AccessToken`（Query），Query `access_token` | 查询门店列表。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_In… |
| 3 | `IMpPoiService` | `DeletePoiAsync` | POST | `/cgi-bin/poi/delpoi` | `AccessToken`（Query），Query `access_token` | 删除门店。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Inte… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与 `IMpStoreService`（店铺域）的关系（官方两代接口，并存勿合并）**：
本域是**旧版微信门店接口（POI，`/cgi-bin/poi/*`）**；
`Store` 域是新版小程序店铺 API（`/wxa/*`）。两代接口的门店 ID 体系不同（`poi_id` vs 店铺 ID）。

MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；URL 遥测已由组件 `SensitiveUrlRedactor` 与 `MpException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetPoiAsync` — 查询门店信息。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#9
（官方接口英文名 `getpoi`）。

- **声明接口**：[IMpPoiService](./IMpPoiService.cs)
- **官方契约**：`POST /cgi-bin/poi/getpoi`

- **请求体**：`MpPoiGetRequest` — 查询请求（`poi_id` 必填）。
- **返回**：`MpPoiGetResponse` — 门店基础信息（类目 / 坐标 / 图片 / 状态等）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"poi_id": …}`；成功响应不含 `errcode`（缺省 0 视为成功）。
- 官方错误码：`-1` / `40007`（invalid media_id）/ `61341`（invalid poi_id，照官方现状记录）。

### 2. `GetPoiListAsync` — 查询门店列表。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#10
（官方接口英文名 `getpoilist`）。

- **声明接口**：[IMpPoiService](./IMpPoiService.cs)
- **官方契约**：`POST /cgi-bin/poi/getpoilist`

- **请求体**：`MpPoiListRequest` — 分页请求（`begin` 从 0 起；`limit` 建议 ≤ 50）。
- **返回**：`MpPoiListResponse` — 门店列表与总数（每项基础信息额外携带 poi_id / 升级状态）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"begin": …, "limit": …}`。

### 3. `DeletePoiAsync` — 删除门店。官方文档：https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#12
（官方接口英文名 `delpoi`）。

- **声明接口**：[IMpPoiService](./IMpPoiService.cs)
- **官方契约**：`POST /cgi-bin/poi/delpoi`

- **请求体**：`MpPoiDeleteRequest` — 删除请求（`poi_id` 必填）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- **覆盖删除语义**：删除后微信侧门店即时下线、不可恢复；重建同名门店会得到新的 `poi_id`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/DataModels/Poi`](../../DataModels/Poi) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Poi`。

AOT 源生成上下文：[`PoiJsonContext.g.cs`](./../../DataModels/Generated/PoiJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpPoiGetResponse` | 应答 | [Poi/MpPoiGetResponse.cs](../../DataModels/Poi/MpPoiGetResponse.cs) | `business` |
| `MpPoiGetRequest` | 请求体 | [Poi/MpPoiGetRequest.cs](../../DataModels/Poi/MpPoiGetRequest.cs) | `poi_id` |
| `MpPoiListResponse` | 应答 | [Poi/MpPoiListResponse.cs](../../DataModels/Poi/MpPoiListResponse.cs) | `business_list`, `total_count` |
| `MpPoiListRequest` | 请求体 | [Poi/MpPoiListRequest.cs](../../DataModels/Poi/MpPoiListRequest.cs) | `begin`, `limit` |
| `MpResponse` | 应答 | [MpResponse.cs](../../DataModels/MpResponse.cs) | `errcode`, `errmsg` |
| `MpPoiDeleteRequest` | 请求体 | [Poi/MpPoiDeleteRequest.cs](../../DataModels/Poi/MpPoiDeleteRequest.cs) | `poi_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Poi;
using Mud.Wechat.OfficialAccount.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddPoiApi());

var service = app.Services.GetRequiredService<IMpPoiService>();
var ct = CancellationToken.None;
var response = await service.DeletePoiAsync(new MpPoiDeleteRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/poi/delpoi");
// 占位值仅用于编译示意；业务值须按 IMpPoiService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`DeletePoiAsync`）；请求体 `MpPoiDeleteRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpPoiService](./IMpPoiService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpPoiContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpPoiContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `PoiEndpoints_ShouldMatchOfficialRoutes`、`PoiDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/DataModels/Poi`](../../DataModels/Poi) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpPoiContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

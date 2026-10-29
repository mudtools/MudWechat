# PlatformKf 域 · 平台客诉工单

微信小店 / 视频号（channels 生态）「平台客诉工单」域 SDK（4 端点：工单分类配置 /
商家工单列表 / 工单详情 / 商户处理工单）。 接口：[IChannelsPlatformKfService](./IChannelsPlatformKfService.cs)；注册组 `PlatformKf`；模块枚举 `ChannelsModule.PlatformKf`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsPlatformKfContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPlatformKfContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/PlatformKf`](../../DataModels/PlatformKf)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsPlatformKfService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsPlatformKfService` | `GetTicketClassConfigAsync` | POST | `/channels/ec/platformkf/getticketclassconfig` | `AccessToken`（Query），Query `access_token` | 查询工单分类配置（`getticketclassconfig`，商家回复选项与必填项配置）。 |
| 2 | `IChannelsPlatformKfService` | `GetMerchantTicketListAsync` | POST | `/channels/ec/platformkf/getmchticketlist` | `AccessToken`（Query），Query `access_token` | 获取商家工单列表（`getmchticketlist`，分页查询商家名下工单）。 |
| 3 | `IChannelsPlatformKfService` | `GetMerchantTicketDetailAsync` | POST | `/channels/ec/platformkf/getmchticketdetail` | `AccessToken`（Query），Query `access_token` | 查询工单详情（`getmchticketdetail`，单个工单全量信息）。 |
| 4 | `IChannelsPlatformKfService` | `OperateMerchantTicketAsync` | POST | `/channels/ec/platformkf/operatemchticket` | `AccessToken`（Query），Query `access_token` | 商户处理商家工单（`operatemchticket`，回复选项 + 补充说明 + 协商时间）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「商家客服」概念严格区分**：本域是平台侧客诉工单（`/channels/ec/platformkf/*`，
平台发起、商户响应），**不是**商家客服会话（`IChannelsKfService`，
`/channels/ec/commkf/*`）。

**工单流转契约**：工单状态为「1 待商家处理 / 2 待平台处理 / 3 已完结」（见
`ChannelsPlatformKfTicketStates`）；当前仅开放「催发货」一种分类类型（type 9，见
`ChannelsPlatformKfTicketClassTypes`）。

**第三方代调用**：本域 4 端点官方权限集 id 均为 `131`，服务商获得授权后可用
`authorizer_access_token` 代微信小店商家调用（多应用基座按当前应用上下文路由）。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）第三方平台代调用所属权限集 id 为 `131`。
- （出现 4 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetTicketClassConfigAsync` — 查询工单分类配置（`getticketclassconfig`，商家回复选项与必填项配置）。

- **声明接口**：[IChannelsPlatformKfService](./IChannelsPlatformKfService.cs)
- **官方契约**：`POST /channels/ec/platformkf/getticketclassconfig`
- **返回**：`ChannelsPlatformKfGetTicketClassConfigResponse` — 商家回复选项配置（`mch_reply_options.conf_list`：工单分类类型 / 名称 + 可选回复项
的选项 id / 名称 / 补充说明·凭证·协商时间各自是否必填）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getticketclassconfig
- 官方契约：**POST** + 空请求体。
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；当前仅开放一种工单分类类型 / （催发货，type 9）；处理工单（`operatemchticket`）前须先经本接口取得 / `option_id` 与各必填项开关。

### 2. `GetMerchantTicketListAsync` — 获取商家工单列表（`getmchticketlist`，分页查询商家名下工单）。

- **声明接口**：[IChannelsPlatformKfService](./IChannelsPlatformKfService.cs)
- **官方契约**：`POST /channels/ec/platformkf/getmchticketlist`

- **请求体**：`ChannelsPlatformKfGetTicketListRequest` — 查询请求（全字段选填；不传 `ticket_state` 返回所有状态，默认按创建时间降序）。
- **返回**：`ChannelsPlatformKfGetTicketListResponse` — 工单列表（`ticket_item_list`，元素结构见 `ChannelsPlatformKfTicketItem`）与总数（`total`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getmchticketlist
- 官方业务限制：①官方注意事项原文「本接口无特殊注意事项」；②`limit` 默认 10， / 最大建议不超过 50；③`search_ticket_id` 支持工单 id / 订单 id 任意其一； / ④时间过滤针对工单的创建时间（Unix 秒级时间戳）。
- 官方错误码：`40097`（参数错误）。

### 3. `GetMerchantTicketDetailAsync` — 查询工单详情（`getmchticketdetail`，单个工单全量信息）。

- **声明接口**：[IChannelsPlatformKfService](./IChannelsPlatformKfService.cs)
- **官方契约**：`POST /channels/ec/platformkf/getmchticketdetail`

- **请求体**：`ChannelsPlatformKfGetTicketDetailRequest` — 查询请求（`ticket_id` 必填）。
- **返回**：`ChannelsPlatformKfGetTicketDetailResponse` — 工单详细信息（`merchant_ticket`，结构同工单列表元素 `ChannelsPlatformKfTicketItem`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getmchticketdetail
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；官方错误码 `40097`（参数错误）。
- 官方契约：详情对象结构与工单列表元素一致 ⇒ SDK 共用 `ChannelsPlatformKfTicketItem` 建模。

### 4. `OperateMerchantTicketAsync` — 商户处理商家工单（`operatemchticket`，回复选项 + 补充说明 + 协商时间）。

- **声明接口**：[IChannelsPlatformKfService](./IChannelsPlatformKfService.cs)
- **官方契约**：`POST /channels/ec/platformkf/operatemchticket`

- **请求体**：`ChannelsPlatformKfOperateTicketRequest` — 处理请求（`ticket_id` / `reply_option` 必填；`description` /
`confer_time` 在对应选项的 `is_must_description` / `is_must_confer_time` 为 true 时必填，
选项配置经「查询工单分类配置」接口取得）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_operatemchticket
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；同一工单不可重复处理（1600001 该工单已处理）。
- 官方错误码：`40097`（参数错误）/ `1600001`（该工单已处理）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/PlatformKf`](../../DataModels/PlatformKf) 内共 **15** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **15** 个，`SerializerClassName` = `PlatformKf`。

AOT 源生成上下文：[`PlatformKfJsonContext.g.cs`](./../../DataModels/Generated/PlatformKfJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsPlatformKfGetTicketClassConfigResponse` | 应答 | [PlatformKf/ChannelsPlatformKfGetTicketClassConfigResponse.cs](../../DataModels/PlatformKf/ChannelsPlatformKfGetTicketClassConfigResponse.cs) | `mch_reply_options` |
| `ChannelsPlatformKfGetTicketListResponse` | 应答 | [PlatformKf/ChannelsPlatformKfGetTicketListResponse.cs](../../DataModels/PlatformKf/ChannelsPlatformKfGetTicketListResponse.cs) | `ticket_item_list`, `total` |
| `ChannelsPlatformKfGetTicketListRequest` | 请求体 | [PlatformKf/ChannelsPlatformKfGetTicketListRequest.cs](../../DataModels/PlatformKf/ChannelsPlatformKfGetTicketListRequest.cs) | `ticket_state`, `ticket_class_type`, `search_ticket_id`, `search_start_time`, `search_end_time`, `sort_order_type`, `limit`, `start` |
| `ChannelsPlatformKfGetTicketDetailResponse` | 应答 | [PlatformKf/ChannelsPlatformKfGetTicketDetailResponse.cs](../../DataModels/PlatformKf/ChannelsPlatformKfGetTicketDetailResponse.cs) | `merchant_ticket` |
| `ChannelsPlatformKfGetTicketDetailRequest` | 请求体 | [PlatformKf/ChannelsPlatformKfGetTicketDetailRequest.cs](../../DataModels/PlatformKf/ChannelsPlatformKfGetTicketDetailRequest.cs) | `ticket_id` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsPlatformKfOperateTicketRequest` | 请求体 | [PlatformKf/ChannelsPlatformKfOperateTicketRequest.cs](../../DataModels/PlatformKf/ChannelsPlatformKfOperateTicketRequest.cs) | `ticket_id`, `reply_option`, `description`, `confer_time` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.PlatformKf;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddPlatformKfApi());

var service = app.Services.GetRequiredService<IChannelsPlatformKfService>();
var ct = CancellationToken.None;
var response = await service.GetTicketClassConfigAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/platformkf/getticketclassconfig");
// 占位值仅用于编译示意；业务值须按 IChannelsPlatformKfService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetTicketClassConfigAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsPlatformKfService](./IChannelsPlatformKfService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsPlatformKfContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPlatformKfContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatformKfEndpoints_ShouldMatchOfficialRoutes`、`PlatformKfEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`PlatformKfInterface_ShouldBindChannelsToken_AndRegisterUnderPlatformKfGroup`、`PlatformKfDataModels_ShouldBeRegisteredInJsonContext`、`PlatformKfDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`PlatformKfEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/PlatformKf`](../../DataModels/PlatformKf) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsPlatformKfContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

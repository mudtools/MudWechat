# Qic 域 · 质检管理

微信小店 / 视频号（channels 生态）「质检管理」域 SDK（5 端点：质检仓配置 / 送检配置模板 /
打印质检码 / 绑定送检信息 / 自寄快递送检）。 接口：[IChannelsQicService](./IChannelsQicService.cs)；注册组 `Qic`；模块枚举 `ChannelsModule.Qic`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsQicContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsQicContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Qic`](../../DataModels/Qic)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsQicService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsQicService` | `GetInspectConfigAsync` | GET | `/channels/ec/qic/inspect/config/get` | `AccessToken`（Query），Query `access_token` | 查询质检仓配置（`getinspectconfig`，质检仓 / 收发货地址）。 |
| 2 | `IChannelsQicService` | `GetInspectSubmitConfigAsync` | GET | `/channels/ec/qic/inspect/submitconfig/get` | `AccessToken`（Query），Query `access_token` | 查询送检配置模板信息（`getinspectsubmitconfig`，绑定送检信息的前置查询）。 |
| 3 | `IChannelsQicService` | `PrintInspectCodeAsync` | POST | `/channels/ec/qic/inspect/code/print` | `AccessToken`（Query），Query `access_token` | 打印质检码（`printinspectcode`，绑定完送检信息后打印）。 |
| 4 | `IChannelsQicService` | `SubmitInspectInfoAsync` | POST | `/channels/ec/qic/inspect/submit` | `AccessToken`（Query），Query `access_token` | 绑定送检信息（`submitinspectinfo`，快递送检的送检编排）。 |
| 5 | `IChannelsQicService` | `RegisterInspectLogisticsAsync` | POST | `/channels/ec/qic/inspect/register_logistics` | `AccessToken`（Query），Query `access_token` | 自寄快递送检（`registerlogistics`，登记/修改自寄快递信息）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**域定位**：本域是微信小店质检（QIC）服务的送检编排面，全部端点路由走 `/channels/ec/qic/*` 主干前缀。

**动词混合（照抄官方）**：两个查询端点（`config/get`、`submitconfig/get`）官方为 **GET**，
三个操作端点（`code/print`、`submit`、`register_logistics`）官方为 **POST**。

**质检码响应为驼峰命名**：`inspect/code/print` 的 `data` 对象字段为 camelCase
（如 `backupDeliveryId`、`boxDTOList`），与小店其余端点的 snake_case 不同，字段名照抄官方。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）第三方平台代调用所属权限集 id 为 `131`；适用账号类型仅「微信小店」。
- （出现 5 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetInspectConfigAsync` — 查询质检仓配置（`getinspectconfig`，质检仓 / 收发货地址）。

- **声明接口**：[IChannelsQicService](./IChannelsQicService.cs)
- **官方契约**：`GET /channels/ec/qic/inspect/config/get`
- **返回**：`ChannelsGetInspectConfigResponse` — 质检仓配置（`inspect_config`：质检仓 id / 名称 / 地址 / 发货地址 / 退货地址）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/qic/api_getinspectconfig
- 官方契约：**GET** + 空请求体；**不支持云调用**。
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。
- 官方错误码：`10021600`（质检系统异常）/ `10021602`（小店尚未配置质检基础信息）。

### 2. `GetInspectSubmitConfigAsync` — 查询送检配置模板信息（`getinspectsubmitconfig`，绑定送检信息的前置查询）。

- **声明接口**：[IChannelsQicService](./IChannelsQicService.cs)
- **官方契约**：`GET /channels/ec/qic/inspect/submitconfig/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `orderId` | Query `order_id` | `string?` | 订单 id（官方 `order_id`，URL 参数，选填；官方变更日志 2025-11-13 由 order_id_list 改为 order_id）。 |
- **返回**：`ChannelsGetInspectSubmitConfigResponse` — 送检配置模板（`submit_config`：快递公司列表 `delivery_list`（含快递产品与保价类型）、
质检机构列表 `inspect_org_list`、资费标准 `charge_url`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/qic/api_getinspectsubmitconfig
- 官方契约：**GET** + 空请求体（`order_id` 走 URL 参数）；**不支持云调用**。 / 质检机构类别枚举见 `ChannelsInspectOrgCategories`、保价支持枚举见 / `ChannelsInspectInsureSupports`、保价上限类型枚举见 `ChannelsInspectUpperLimitTypes`。
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；接口应在服务器端调用。
- 官方错误码：`10021600`（质检系统异常）/ `10021602`（小店尚未配置质检基础信息）/ / `10021606`（质检服务未开通）。

### 3. `PrintInspectCodeAsync` — 打印质检码（`printinspectcode`，绑定完送检信息后打印）。

- **声明接口**：[IChannelsQicService](./IChannelsQicService.cs)
- **官方契约**：`POST /channels/ec/qic/inspect/code/print`

- **请求体**：`ChannelsPrintInspectCodeRequest` — 打印请求（`order_id` 必填，订单 id）。
- **返回**：`ChannelsPrintInspectCodeResponse` — 质检码详情（`data`：质检码 / 质检仓与机构信息 / 快递信息 / 商品信息 / 打印模板数据 `printInfo` 等）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/qic/api_printinspectcode
- 官方契约：**POST**；**不支持云调用**。响应 `data` 为 camelCase 字段命名（官方照录）。
- 官方业务限制：①绑定完送检信息后可通过本接口打印微信小店的质检码；②质检面单打印新方案—— / 只需把 `printInfo` 字段传入打印控件即可进行打印（base64 编码的模板数据与模板 html 地址）。
- 官方错误码：`10021600`（质检系统异常）/ `10021601`（质检单不存在）/ / `10021603`（查询订单详情失败）/ `10021604`（查询商品信息失败）/ `10021609`（未录入送检信息）。

### 4. `SubmitInspectInfoAsync` — 绑定送检信息（`submitinspectinfo`，快递送检的送检编排）。

- **声明接口**：[IChannelsQicService](./IChannelsQicService.cs)
- **官方契约**：`POST /channels/ec/qic/inspect/submit`

- **请求体**：`ChannelsSubmitInspectRequest` — 绑定请求（`order_id`/`inspect_info` 必填；字段约束见 `ChannelsInspectInfo`）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/qic/api_submitinspectinfo
- 官方契约：**POST**；**不支持云调用**。售后拦截策略枚举见 `ChannelsInspectRefundIntercepts`。
- 官方业务限制：①需保价时 `express_insure` 为 true 且保价金额不能高于保价产品金额上限； / ②珠宝 / 沉香质检机构（`inspect_org_id` / `agarwood_inspect_org_id`）按送检配置模板的 / `org_category` 至少填一个不为空；③备注限 32 个 utf8 字符且不可含材质信息； / ④已入库质检单不支持修改送检信息；⑤更新送检信息不可太频繁。
- 官方错误码（节选）：`10021605`（当前质检单已入库，不支持修改送检信息）/ `10021606`（质检服务未开通）/ / `10021607`（质检服务已暂停，账户余额低于阈值）/ `10021610`（当前订单 id 不属于当前商家 appid）/ / `10021611`（保价金额超出快递产品保价上限）/ `10021612`（发货快递产品不支持保价）/ / `10021615`（当前质检仓尚未开通任何质检机构）/ `10021617`（保价金额不能大于订单金额）/ / `10021619`（备注超出字数限制）/ `10021620`（不可备注材质信息）/ `10021621`（更新送检信息太频繁）。

### 5. `RegisterInspectLogisticsAsync` — 自寄快递送检（`registerlogistics`，登记/修改自寄快递信息）。

- **声明接口**：[IChannelsQicService](./IChannelsQicService.cs)
- **官方契约**：`POST /channels/ec/qic/inspect/register_logistics`

- **请求体**：`ChannelsRegisterInspectLogisticsRequest` — 登记请求（`order_id_list`/`logistics_info` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/qic/api_registerlogistics
- 官方契约：**POST**；**不支持云调用**。发货方式枚举见 `ChannelsInspectDeliveryTypes`（本接口固定 1 自寄快递发货）。
- **官方业务限制：可通过该接口登记/修改自寄快递信息，最多可修改三次**。
- 官方错误码（节选）：`10021605`（当前质检单已入库，不支持修改送检信息）/ `10021622`（物流单号非法）/ / `10021623`（超出修改次数上限）/ `10021624`（当前订单质检状态不允许修改物流）/ / `10021625`（物流信息修改部分成功，可稍后重试）/ `10021626`（快递公司跟物流单号不匹配）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Qic`](../../DataModels/Qic) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `Qic`。

AOT 源生成上下文：[`QicJsonContext.g.cs`](./../../DataModels/Generated/QicJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsGetInspectConfigResponse` | 应答 | [Qic/ChannelsGetInspectConfigResponse.cs](../../DataModels/Qic/ChannelsGetInspectConfigResponse.cs) | `inspect_config` |
| `ChannelsGetInspectSubmitConfigResponse` | 应答 | [Qic/ChannelsGetInspectSubmitConfigResponse.cs](../../DataModels/Qic/ChannelsGetInspectSubmitConfigResponse.cs) | `submit_config` |
| `ChannelsPrintInspectCodeResponse` | 应答 | [Qic/ChannelsPrintInspectCodeResponse.cs](../../DataModels/Qic/ChannelsPrintInspectCodeResponse.cs) | `data` |
| `ChannelsPrintInspectCodeRequest` | 请求体 | [Qic/ChannelsPrintInspectCodeRequest.cs](../../DataModels/Qic/ChannelsPrintInspectCodeRequest.cs) | `order_id` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsSubmitInspectRequest` | 请求体 | [Qic/ChannelsSubmitInspectRequest.cs](../../DataModels/Qic/ChannelsSubmitInspectRequest.cs) | `order_id`, `inspect_info` |
| `ChannelsRegisterInspectLogisticsRequest` | 请求体 | [Qic/ChannelsRegisterInspectLogisticsRequest.cs](../../DataModels/Qic/ChannelsRegisterInspectLogisticsRequest.cs) | `order_id_list`, `logistics_info` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Qic;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddQicApi());

var service = app.Services.GetRequiredService<IChannelsQicService>();
var ct = CancellationToken.None;
var response = await service.GetInspectConfigAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/qic/inspect/config/get");
// 占位值仅用于编译示意；业务值须按 IChannelsQicService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetInspectConfigAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsQicService](./IChannelsQicService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsQicContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsQicContractGuards.cs) |
| 守卫中的代表性测试方法 | `QicEndpoints_ShouldMatchOfficialRoutes`、`QicEndpoints_ShouldMatchOfficialVerbs_WithBodyRules`、`QicInterface_ShouldBindChannelsToken_AndRegisterUnderQicGroup`、`QicDataModels_ShouldBeRegisteredInJsonContext`、`QicDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`QicEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Qic`](../../DataModels/Qic) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsQicContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

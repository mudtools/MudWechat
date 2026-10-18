# Wecom 域 · 企业微信关联

微信小店 / 视频号（channels 生态）「企业微信关联」域 SDK（1 端点：获取关联账号企微 id）。 接口：[IChannelsWecomService](./IChannelsWecomService.cs)；注册组 `Wecom`；模块枚举 `ChannelsModule.Wecom`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsWecomContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWecomContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Wecom`](../../../Mud.Wechat.Channels.DataModels/Wecom/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsWecomService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsWecomService` | `GetWecomIdAsync` | POST | `/channels/ec/wecom/get_wecom_id` | `AccessToken`（Query），Query `access_token` | 获取关联账号企微 id（`shop_get_wecom_openid`，企微 corp_id/user_id ↔ 小店侧 id 换取）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**域职责（官方契约）**：获取企业微信在微信小店侧的 `wecom_corp_id` / `wecom_user_id`，
可用于店铺 H5 链接 / 二维码 / 口令等 API 作为查询参数（企微归因链路的换取入口）。

**适用范围（官方页与其他域不同，勿套用）**：仅「微信小店带货助手」账号类型可调用；
**第三方平台调用：不支持**（无权限集 id）；**云调用：不支持**。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetWecomIdAsync` — 获取关联账号企微 id（`shop_get_wecom_openid`，企微 corp_id/user_id ↔ 小店侧 id 换取）。

- **声明接口**：[IChannelsWecomService](./IChannelsWecomService.cs)
- **官方契约**：`POST /channels/ec/wecom/get_wecom_id`

- **请求体**：`ChannelsWecomIdRequest` — 查询请求（`corp_id` 必填；`user_id` 选填且须属于该 corp_id）。
- **返回**：`ChannelsWecomIdResponse` — 企微在小店侧的映射 id（`data.wecom_corp_id` 企业微信 id / `data.wecom_user_id` 企业微信账号 id）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/wecom/api_shop_get_wecom_openid
- 官方业务限制：①查询的企业 `corp_id` 需要是店铺的关联账号，否则查询失败； / ②查询的成员 `user_id` 需要在微信小店应用的可见范围内，否则查询失败；③本接口无特殊注意事项。
- 官方错误码：`914910001`（参数非法，参考 errmsg）/ `914911001`（未绑定企业为关联账号，无法查询）/ / `914911002`（查询不到 user_id）/ `914911003`（企业微信未授权微信小店）。
- 官方适用范围：仅「微信小店带货助手」账号类型可调用；第三方平台调用不支持（无权限集 id）。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Wecom`](../../../Mud.Wechat.Channels.DataModels/Wecom/) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `Wecom`。

AOT 源生成上下文：[`WecomJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/WecomJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsWecomIdResponse` | 应答 | [Wecom/ChannelsWecomIdResponse.cs](../../../Mud.Wechat.Channels.DataModels/Wecom/ChannelsWecomIdResponse.cs) | `data` |
| `ChannelsWecomIdRequest` | 请求体 | [Wecom/ChannelsWecomIdRequest.cs](../../../Mud.Wechat.Channels.DataModels/Wecom/ChannelsWecomIdRequest.cs) | `corp_id`, `user_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Wecom;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddWecomApi());

var service = app.Services.GetRequiredService<IChannelsWecomService>();
var ct = CancellationToken.None;
var response = await service.GetWecomIdAsync(new ChannelsWecomIdRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/wecom/get_wecom_id");
// 占位值仅用于编译示意；业务值须按 IChannelsWecomService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetWecomIdAsync`）；请求体 `ChannelsWecomIdRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsWecomService](./IChannelsWecomService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsWecomContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWecomContractGuards.cs) |
| 守卫中的代表性测试方法 | `WecomEndpoints_ShouldMatchOfficialRoutes`、`WecomEndpoints_ShouldBePost_WithJsonBody`、`WecomInterface_ShouldBindChannelsToken_AndRegisterUnderWecomGroup`、`WecomDataModels_ShouldBeRegisteredInJsonContext`、`WecomDataModels_ShouldLockOfficialFields` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Wecom`](../../../Mud.Wechat.Channels.DataModels/Wecom/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsWecomContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

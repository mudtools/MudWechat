# Kf 域 · 商家客服

微信小店 / 视频号（channels 生态）「商家客服」域 SDK（2 端点：多媒体资源上传 / 客服消息发送）。 接口：[IChannelsKfService](./IChannelsKfService.cs)；注册组 `Kf`；模块枚举 `ChannelsModule.Kf`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsKfContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsKfContractGuards.cs)、[ChannelsPlatformKfContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPlatformKfContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Kf`](../../DataModels/Kf)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsKfService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsKfService` | `CosUploadAsync` | POST | `/channels/ec/commkf/cosupload` | `AccessToken`（Query），Query `access_token` | 上传多媒体资源（`cosupload`，商家客服会话用图 / 视 / 文件，返回 cos_url）。 |
| 2 | `IChannelsKfService` | `SendMessageAsync` | POST | `/channels/ec/commkf/sendmsg` | `AccessToken`（Query），Query `access_token` | 发送消息（`sendmsg`，商家客服向指定用户发送客服消息）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「企微微信客服」概念严格区分**：本域是微信小店的商家客服（`/channels/ec/commkf/*`，
小店 `access_token`），**不是**企微线的微信客服（Work 线 `Interfaces/KF/`，
企微 `Wechat.AccessToken`）。两条线无任何路由交叠，命名空间亦分线隔离。

**客服消息时序契约**：用户发消息后，客服仅可在 48 小时内回复且最多回复 5 条（超出分别报
10190009 / 10190008）；用户结束会话后不可继续发送（10190013）。

**媒体上传形态**：官方多媒体资源上传为 `multipart/form-data`（消息类型 / openid / 文件
均为表单字段），SDK 按库内既有的声明式上传通路建模（`IFormContent`，文件字段
与 `msg_type` / `open_id` 表单字段均由调用方构建）。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CosUploadAsync` — 上传多媒体资源（`cosupload`，商家客服会话用图 / 视 / 文件，返回 cos_url）。

- **声明接口**：[IChannelsKfService](./IChannelsKfService.cs)
- **官方契约**：`POST /channels/ec/commkf/cosupload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（由调用方构建 `IFormContent` 实现）——<br>官方表单字段：`msg_type`（文件类型，见 `ChannelsKfMediaTypes`，必填）、<br>`open_id`（用户 openid，必填）、`file`（待上传的视频 / 图片 / 文件，必填）。<br>须标 `[MultipartForm]` —— 走 `ToHttpContentAsync` 通路（与<br>Work 线媒体上传同形态）。 |
- **返回**：`ChannelsKfCosUploadResponse` — 多媒体资源地址（`cos_url`，形如 channels.weixin.qq.com/shop/commkf/downloadmedia?encrypted_param=…，供发送消息接口引用）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/kf/api_cosupload
- 官方业务限制：①官方请求体为 `multipart/form-data`（msg_type / open_id / file 均为表单字段）， / 不支持云调用，不支持第三方平台调用；②图片 / 视频 / 文件大小均**不超过 30MB**； / ③上传的媒体文件在微信服务器上**保存 14 天**，过期后自动删除。
- 官方错误码：`10080000`（注销公示期）/ `10190001`（系统错误）。
- 适用范围：仅「微信小店（需申请）」账号类型可调用。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 2. `SendMessageAsync` — 发送消息（`sendmsg`，商家客服向指定用户发送客服消息）。

- **声明接口**：[IChannelsKfService](./IChannelsKfService.cs)
- **官方契约**：`POST /channels/ec/commkf/sendmsg`

- **请求体**：`ChannelsKfSendMsgRequest` — 发送请求（`msg_type` 必填且决定哪个内容对象必填，见 `ChannelsKfMsgTypes`；
媒体类内容的 `cos_url` 须经上传多媒体资源接口获取）。
- **返回**：`ChannelsKfSendMsgResponse` — 消息的单调自增 id（`msg_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/kf/api_sendmsg
- 官方业务限制：①用户发消息后，客服只可在 **48 小时内**回复用户且**最多可回复 5 条消息** / （超出分别报 10190009 / 10190008）；②用户已结束会话后不能继续发送（10190013）； / ③消息发送经过风控、数据反垃圾，须注意内容安全（10190010）；④涉及用户联系方式、地址等聊天内容 / 会按《个人信息保护法》自动加密处理；⑤ /  中 `request_id` / 填写时以该任务 id 去重。
- 官方错误码：`10080000`（注销公示期）/ `10190001`（系统错误）/ `10190002`（须 POST）/ / `10190003`（参数错误）/ `10190006`（请求体转 JSON 失败）/ `10190008`（消息条数超限）/ / `10190009`（会话超过 48h）/ `10190010`（UCG 消息命中风控）/ `10190013`（用户已结束会话）。
- 适用范围：仅「微信小店（需申请）」账号类型可调用。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Kf`](../../DataModels/Kf) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Kf`。

AOT 源生成上下文：[`KfJsonContext.g.cs`](./../../DataModels/Generated/KfJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsKfCosUploadResponse` | 应答 | [Kf/ChannelsKfCosUploadResponse.cs](../../DataModels/Kf/ChannelsKfCosUploadResponse.cs) | `cos_url` |
| `ChannelsKfSendMsgResponse` | 应答 | [Kf/ChannelsKfSendMsgResponse.cs](../../DataModels/Kf/ChannelsKfSendMsgResponse.cs) | `msg_id` |
| `ChannelsKfSendMsgRequest` | 请求体 | [Kf/ChannelsKfSendMsgRequest.cs](../../DataModels/Kf/ChannelsKfSendMsgRequest.cs) | `request_id`, `open_id`, `msg_type`, `text`, `image`, `video`, `product_share`, `order_share`, `file` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Kf;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddKfApi());

var service = app.Services.GetRequiredService<IChannelsKfService>();
var ct = CancellationToken.None;
var response = await service.CosUploadAsync(default!, ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/commkf/cosupload");
// 占位值仅用于编译示意；业务值须按 IChannelsKfService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CosUploadAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsKfService](./IChannelsKfService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsKfContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsKfContractGuards.cs)、[`ChannelsPlatformKfContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPlatformKfContractGuards.cs) |
| 守卫中的代表性测试方法 | `KfEndpoints_ShouldMatchOfficialRoutes`、`KfEndpoints_ShouldAllBePost_WithMultipartUploadRules`、`KfInterface_ShouldBindChannelsToken_AndRegisterUnderKfGroup`、`KfDataModels_ShouldBeRegisteredInJsonContext`、`KfDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`KfEnumConstants_ShouldLockOfficialLiteralValues`、`PlatformKfEndpoints_ShouldMatchOfficialRoutes`、`PlatformKfEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`PlatformKfInterface_ShouldBindChannelsToken_AndRegisterUnderPlatformKfGroup`、`PlatformKfDataModels_ShouldBeRegisteredInJsonContext`、`PlatformKfDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`PlatformKfEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Kf`](../../DataModels/Kf) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsKfContractGuards.cs`、`ChannelsPlatformKfContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

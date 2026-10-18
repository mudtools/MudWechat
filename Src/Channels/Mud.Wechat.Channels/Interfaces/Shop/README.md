# Shop 域 · 店铺管理

微信小店 / 视频号（channels 生态）「店铺管理」域 SDK（4 端点：店铺基本信息 / H5 链接 / 二维码 / 微信口令）。 接口：[IChannelsShopService](./IChannelsShopService.cs)；注册组 `Shop`；模块枚举 `ChannelsModule.Shop`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsShopContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsShopContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Shop`](../../../Mud.Wechat.Channels.DataModels/Shop/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsShopService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsShopService` | `GetShopInfoAsync` | GET | `/channels/ec/basics/info/get` | `AccessToken`（Query），Query `access_token` | 获取店铺基本信息（`mmecapi_basicinfo`）。 |
| 2 | `IChannelsShopService` | `GetShopH5UrlAsync` | POST | `/channels/ec/basics/shop/h5url/get` | `AccessToken`（Query），Query `access_token` | 获取店铺 H5 链接（`getshoph5url`，支持传入企业微信参数）。 |
| 3 | `IChannelsShopService` | `GetShopQrcodeAsync` | POST | `/channels/ec/basics/shop/qrcode/get` | `AccessToken`（Query），Query `access_token` | 获取店铺二维码（`getshopqrcode`，支持传入企业微信参数）。 |
| 4 | `IChannelsShopService` | `GetShopTagLinkAsync` | POST | `/channels/ec/basics/shop/taglink/get` | `AccessToken`（Query），Query `access_token` | 获取店铺口令（`getshoptaglink`，支持传入企业微信参数）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**企业微信参数传播面（官方契约）**：H5 链接 / 二维码 / 微信口令三端点均支持传入企业微信参数
（`wecom_corp_id` / `wecom_user_id`，经「获取关联账号企微id」接口获取）——
带企微参数的链接 / 二维码 / 口令下单后，订单会在成交来源展示该企业。

**路由前缀**：本域 4 端点全部走 `/channels/ec/basics/*` 主干前缀。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）第三方平台代调用所属权限集 id 为 `129`、`131`、`192`。
- （出现 4 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 3 次）官方错误码：`10021404`（企业微信未绑定店铺）/ `10021405`（该成员不是该企业微信成员）/ `10021406`（参数有误）。

### 1. `GetShopInfoAsync` — 获取店铺基本信息（`mmecapi_basicinfo`）。

- **声明接口**：[IChannelsShopService](./IChannelsShopService.cs)
- **官方契约**：`GET /channels/ec/basics/info/get`
- **返回**：`ChannelsShopBasicInfoResponse` — 店铺信息（`info`：店铺名称 / 头像 URL / 店铺类型 / 店铺状态 / 原始 ID / 是否本地生活小店 / 开店时间戳）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_mmecapi_basicinfo
- 官方契约：**GET** + 空请求体（官方参数表仅 `access_token` 一个 URL 参数）。
- 官方业务限制：本接口无特殊注意事项；店铺状态与类型取值见 / `ChannelsShopStatuses` / `ChannelsShopSubjectTypes`； / `open_timestamp` 仅店铺状态为 open_finished 时返回。
- 官方错误码：`43001`（需要使用 HTTP GET）。

### 2. `GetShopH5UrlAsync` — 获取店铺 H5 链接（`getshoph5url`，支持传入企业微信参数）。

- **声明接口**：[IChannelsShopService](./IChannelsShopService.cs)
- **官方契约**：`POST /channels/ec/basics/shop/h5url/get`

- **请求体**：`ChannelsShopWecomLinkRequest` — 请求体（`wecom_corp_id`/`wecom_user_id` 均选填，经「获取关联账号企微id」接口获取）。
- **返回**：`ChannelsShopH5UrlResponse` — 店铺 H5 链接（`shop_h5url`；带企微参数下单后成交来源展示该企业）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshoph5url
- 官方业务限制：本接口无特殊注意事项；与店铺口令端点的请求体字段集一致 ⇒ 共用请求 DTO / `ChannelsShopWecomLinkRequest`（裁决理由见守卫 `ChannelsShopContractGuards` CS5）。

### 3. `GetShopQrcodeAsync` — 获取店铺二维码（`getshopqrcode`，支持传入企业微信参数）。

- **声明接口**：[IChannelsShopService](./IChannelsShopService.cs)
- **官方契约**：`POST /channels/ec/basics/shop/qrcode/get`

- **请求体**：`ChannelsShopQrcodeRequest` — 请求体（`qrcode_type` 必填，见 `ChannelsShopQrcodeTypes`；`wecom_corp_id`/`wecom_user_id` 选填）。
- **返回**：`ChannelsShopQrcodeResponse` — 店铺二维码链接（`shop_qrcode`；带企微参数下单后成交来源展示该企业）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshopqrcode
- 官方业务限制：本接口无特殊注意事项；`qrcode_type` 官方必填（1 二维码 / 2 标准物料 / 3 送礼物物料）， / 与 H5 链接 / 口令请求体字段集不同 ⇒ 独立请求 DTO（不共用）。

### 4. `GetShopTagLinkAsync` — 获取店铺口令（`getshoptaglink`，支持传入企业微信参数）。

- **声明接口**：[IChannelsShopService](./IChannelsShopService.cs)
- **官方契约**：`POST /channels/ec/basics/shop/taglink/get`

- **请求体**：`ChannelsShopWecomLinkRequest` — 请求体（`wecom_corp_id`/`wecom_user_id` 均选填，经「获取关联账号企微id」接口获取）。
- **返回**：`ChannelsShopTagLinkResponse` — 店铺微信口令（`shop_taglink`，形如 `#微信小店://微信小店/xxx`；带企微参数下单后成交来源展示该企业）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_getshoptaglink
- 官方业务限制：本接口无特殊注意事项；与店铺 H5 链接端点的请求体字段集一致 ⇒ 共用请求 DTO / `ChannelsShopWecomLinkRequest`（裁决理由见守卫 `ChannelsShopContractGuards` CS5）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Shop`](../../../Mud.Wechat.Channels.DataModels/Shop/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Shop`。

AOT 源生成上下文：[`ShopJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/ShopJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsShopBasicInfoResponse` | 应答 | [Shop/ChannelsShopBasicInfoResponse.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopBasicInfoResponse.cs) | `info` |
| `ChannelsShopH5UrlResponse` | 应答 | [Shop/ChannelsShopH5UrlResponse.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopH5UrlResponse.cs) | `shop_h5url` |
| `ChannelsShopWecomLinkRequest` | 请求体 | [Shop/ChannelsShopWecomLinkRequest.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopWecomLinkRequest.cs) | `wecom_corp_id`, `wecom_user_id` |
| `ChannelsShopQrcodeResponse` | 应答 | [Shop/ChannelsShopQrcodeResponse.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopQrcodeResponse.cs) | `shop_qrcode` |
| `ChannelsShopQrcodeRequest` | 请求体 | [Shop/ChannelsShopQrcodeRequest.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopQrcodeRequest.cs) | `wecom_corp_id`, `wecom_user_id`, `qrcode_type` |
| `ChannelsShopTagLinkResponse` | 应答 | [Shop/ChannelsShopTagLinkResponse.cs](../../../Mud.Wechat.Channels.DataModels/Shop/ChannelsShopTagLinkResponse.cs) | `shop_taglink` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Shop;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddShopApi());

var service = app.Services.GetRequiredService<IChannelsShopService>();
var ct = CancellationToken.None;
var response = await service.GetShopInfoAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/basics/info/get");
// 占位值仅用于编译示意；业务值须按 IChannelsShopService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetShopInfoAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsShopService](./IChannelsShopService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsShopContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsShopContractGuards.cs) |
| 守卫中的代表性测试方法 | `ShopEndpoints_ShouldMatchOfficialRoutes`、`ShopEndpoints_ShouldHonorOfficialVerbAndBodyRules`、`ShopInterface_ShouldBindChannelsToken_AndRegisterUnderShopGroup`、`ShopDataModels_ShouldBeRegisteredInJsonContext`、`ShopDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`ShopEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Shop`](../../../Mud.Wechat.Channels.DataModels/Shop/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsShopContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

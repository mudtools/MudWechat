# ProductCard 域 · 商品卡片

微信公众号 / 服务号「商品卡片」域 SDK（1 端点；端点位于 /channels/ec/ 视频号小店域前缀）。 接口：[IMpProductCardService](./IMpProductCardService.cs)；注册组 `ProductCard`；模块枚举 `MpModule.ProductCard`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpContentContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpContentContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/DataModels/ProductCard`](../../DataModels/ProductCard)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpProductCardService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpProductCardService` | `GetProductCardInfoAsync` | POST | `/channels/ec/service/product/getcardinfo` | `AccessToken`（Query），Query `access_token` | 获取商品卡片的 DOM 结构。官方文档：https://developers.weixin.qq.com/doc/subscription/api/draftbox/shop/ap… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：https://developers.weixin.qq.com/doc/subscription/api/draftbox/shop/api_ecsgetproductcardinfo.html
（官方接口英文名 `ecsgetproductcardinfo`；2026-10-07 逐页核验，subscription 订阅号域命中；
页面标题「获取商品卡片的DOM结构」）。

**域级约束**：适用范围公众号 ✔ / 服务号 ✔（无「仅认证」）；**不支持第三方平台调用**（官方原文）；
「product_key 和 DOM 只会在需要该字段的文章类型及卡片类型的请求中返回」；不同文章类型支持的卡片类型
范围不同——图片消息（newspic）支持小卡/文字链接/条卡；图文消息（news）支持大卡/小卡/文字链接
（**不支持条卡**）。配套能力：草稿 newspic 文章的 `product_info.footer_product_info.product_key`
即来自本端点。

**令牌路由**：消费 `AccessToken`，Query 注入（MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetProductCardInfoAsync` — 获取商品卡片的 DOM 结构。官方文档：https://developers.weixin.qq.com/doc/subscription/api/draftbox/shop/api_ecsgetproductcardinfo.html
（官方接口英文名 `ecsgetproductcardinfo`）。

- **声明接口**：[IMpProductCardService](./IMpProductCardService.cs)
- **官方契约**：`POST /channels/ec/service/product/getcardinfo`

- **请求体**：`MpProductCardInfoRequest` — 获取请求（product_id / article_type / card_type 均必填；按 article_type 选型 card_type）。
- **返回**：`MpProductCardInfoResponse` — 商品卡信息（`product_key` + `DOM` 结构；按文章类型与卡片类型按需返回）。

**备注（原文）**

- 官方契约：**POST**；**路径前缀为 /channels/ec/（视频号小店域，非 /cgi-bin/）**——官方路径安排，照抄原文。
- 官方错误码：`10170001`（不合法的商品 ID）/ `10170002`（不支持的文章类型）/ / `10170003`（不支持的卡片类型）。
- MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/DataModels/ProductCard`](../../DataModels/ProductCard) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `ProductCard`。

AOT 源生成上下文：[`ProductCardJsonContext.g.cs`](./../../DataModels/Generated/ProductCardJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpProductCardInfoResponse` | 应答 | [ProductCard/MpProductCardInfoResponse.cs](../../DataModels/ProductCard/MpProductCardInfoResponse.cs) | `product_key`, `DOM` |
| `MpProductCardInfoRequest` | 请求体 | [ProductCard/MpProductCardInfoRequest.cs](../../DataModels/ProductCard/MpProductCardInfoRequest.cs) | `product_id`, `article_type`, `card_type` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.ProductCard;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddProductCardApi());

var service = app.Services.GetRequiredService<IMpProductCardService>();
var ct = CancellationToken.None;
var response = await service.GetProductCardInfoAsync(new MpProductCardInfoRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/channels/ec/service/product/getcardinfo");
// 占位值仅用于编译示意；业务值须按 IMpProductCardService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetProductCardInfoAsync`）；请求体 `MpProductCardInfoRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpProductCardService](./IMpProductCardService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpContentContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpContentContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `ContentEndpoints_ShouldMatchOfficialRoutes`、`DraftShapes_ShouldLockArticleFieldsAndDualForms`、`FreePublishShapes_ShouldLockVerifiedCorrections`、`ProductCardShapes_ShouldLockOfficialFields`、`CommentShapes_ShouldLockAddressingAndSharedDtos`、`ContentDataModels_ShouldBeRegisteredInJsonContexts`、`ContentModules_ShouldBeRegisteredAndWhitelisted`、`ContentErrorCodes_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/DataModels/ProductCard`](../../DataModels/ProductCard) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpContentContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

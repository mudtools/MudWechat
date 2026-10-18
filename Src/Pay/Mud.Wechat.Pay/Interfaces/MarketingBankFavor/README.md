# MarketingBankFavor 域 · 银行代金券

微信支付「银行代金券」域 SDK（2 端点）。 接口：[IWechatPayMarketingBankFavorService](./IWechatPayMarketingBankFavorService.cs)；注册组 `MarketingBankFavor`；模块枚举 `PayModule.MarketingBankFavor`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingBankFavorContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankFavorContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MarketingBankFavor`](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingBankFavorService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingBankFavorService` | `SendBankFavorMultiActivityAsync` | POST | `/v3/marketing/bank-favor/users/{openid}/bank-multi-activity` | 商户证书 / APIv3 签名 | 银行商家券营销活动发券。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014569793（普通商户）。 |
| 2 | `IWechatPayMarketingBankFavorService` | `QueryBankFavorCouponAsync` | GET | `/v3/marketing/bank-favor/users/{openid}/coupons/{couponId}` | 商户证书 / APIv3 签名 | 银行商家券详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014569864（普通商户）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对**完全同形**
⇒ **一面一方法 + 双页文档引用**，只有字段表存在差异的端点才按「同路由多方法」分建。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**分页口径不统一**：本域既有 `offset`/`limit`（整型页码）也有 `page_size` + `offset` 形态，
一律**照页**，不得互相套用；应答的 `total_count` 官方多为选填（`offset=0` 或结果为空时才回）。

**PATCH 与 PUT**：批次/活动的改预算与改状态走 **PATCH**，终止走 **POST .../terminate**
⇒ 「写操作动词」在本域不统一，照页不得合并。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SendBankFavorMultiActivityAsync` — 银行商家券营销活动发券。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014569793（普通商户）。

- **声明接口**：[IWechatPayMarketingBankFavorService](./IWechatPayMarketingBankFavorService.cs)
- **官方契约**：`POST /v3/marketing/bank-favor/users/{openid}/bank-multi-activity`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户标识ID】 用户标识ID，用户在公众账号ID下的唯一标识。校验规则：该用户标识ID需要与接口传入中的公众账号ID有对应关系。（官方 path `openid`，**必填**，类型 `string`）。 |

- **请求体**：`BankFavorMultiActivitySendRequest` — 银行商家券营销活动发券请求体，字段见 `BankFavorMultiActivitySendRequest`。
- **返回**：`BankFavorMultiActivitySendResponse` — 银行商家券营销活动发券，见 `BankFavorMultiActivitySendResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/bank-favor/users/{openid}/bank-multi-activity`；path `openid`。
- **支持商户**【普通商户】。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **应答**见 `BankFavorMultiActivitySendResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：403 RULE_LIMIT 用户已达最大领券次数 / 403 REQUEST_BLOCKED 参数错误，请检查批次参数 / 403 NOT_ENOUGH 批次预算耗尽 / 404 RESOURCE_NOT_EXISTS 批次不存在 / 403 NOT_ENOUGH 账户余额不足，请充值 / 403 NOT_ENOUGH 发券超过单天限额 / 403 REQUEST_BLOCKED 仅在广告场景下发放批次 / 403 REQUEST_BLOCKED 商户号收款功能受限，无法发券 / 403 USER_ACCOUNT_ABNORMAL 用户未实名 / 400 PARAM_ERROR 批次号必填 / 400 PARAM_ERROR 活动ID必填 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR 商户单据号必填 / 400 PARAM_ERROR AppID必填 / 400 PARAM_ERROR 商户号必填 / 400 INVALID_REQUEST 非法的商户号 / 403 REQUEST_BLOCKED 批次不支持跨商户发券 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 403 MCH_NOT_EXISTS 商户号不合法 / 429 FREQUENCY_LIMIT_EXCEED 接口限频。

### 2. `QueryBankFavorCouponAsync` — 银行商家券详情。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014569864（普通商户）。

- **声明接口**：[IWechatPayMarketingBankFavorService](./IWechatPayMarketingBankFavorService.cs)
- **官方契约**：`GET /v3/marketing/bank-favor/users/{openid}/coupons/{couponId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户标识ID】 用户标识ID是微信用户在公众账号ID下的唯一用户标识（公众账号ID不同，则获取到的用户标识ID就不同），可用于永久标记一个用户。 获取用户标识ID的方式（官方 path `openid`，**必填**，类型 `string(128)`）。 |
| `couponId` | Path | `string` | 【代金券ID】 优惠券的主键，唯一定义此资源的标识（官方 path `coupon_id`，**必填**，类型 `string(20)`）。 |
| `appid` | Query `appid` | `string` | 【公众账号ID】 微信为调用商户分配的公众账号ID，接口传入的appid应该为公众号的appid和小程序的appid（在 微信公众平台 申请）或APP的appid（在 微信开发平台 申请）。校验规则：1、该appid需要与接口传入中的openid有对应关系；2、该appid需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定 （ 商家商户号与AppID账号关联管理）（官方 query `appid`，**必填**，类型 `string(128)`）。 |
- **返回**：`BankFavorCouponQueryResponse` — 银行商家券详情，见 `BankFavorCouponQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/bank-favor/users/{openid}/coupons/{coupon_id}`；path `openid` / `coupon_id`，query `appid`。
- **支持商户**【普通商户】。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **`appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 券ID必填 / 400 INVALID_REQUEST 非法的券ID / 400 INVALID_REQUEST 查询的商户非创建方商户 / 400 PARAM_ERROR OpenID必填 / 400 PARAM_ERROR AppID必填 / 400 INVALID_REQUEST OpenID与AppID不匹配 / 400 APPID_MCHID_NOT_MATCH 商户号与AppID不匹配 / 429 FREQUENCY_LIMIT_EXCEED 接口限频。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MarketingBankFavor`](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `MarketingBankFavor`。

AOT 源生成上下文：[`MarketingBankFavorJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MarketingBankFavorJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BankFavorMultiActivitySendResponse` | 应答 | [MarketingBankFavor/MarketingBankFavorDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/MarketingBankFavorDtos.cs) | `stock_id`, `activity_id`, `out_request_no`, `appid`, `stock_creator_mchid`, `coupon_id`, `used_amounts`, `coupon_name`, `coupon_state`, `receive_time`, `available_begin_time`, `available_end_time` … |
| `BankFavorMultiActivitySendRequest` | 请求体 | [MarketingBankFavor/MarketingBankFavorDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/MarketingBankFavorDtos.cs) | `stock_id`, `activity_id`, `out_request_no`, `appid`, `stock_creator_mchid`, `coupon_id`, `used_amounts`, `coupon_name`, `coupon_state`, `receive_time`, `available_begin_time`, `available_end_time` … |
| `BankFavorCouponQueryResponse` | 应答 | [MarketingBankFavor/MarketingBankFavorDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/MarketingBankFavorDtos.cs) | `stock_id`, `activity_id`, `out_request_no`, `appid`, `stock_creator_mchid`, `coupon_id`, `used_amounts`, `coupon_name`, `coupon_state`, `receive_time`, `available_begin_time`, `available_end_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingBankFavor;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingBankFavor));

var service = app.Services.GetRequiredService<IWechatPayMarketingBankFavorService>();
var ct = CancellationToken.None;
var response = await service.SendBankFavorMultiActivityAsync("", new BankFavorMultiActivitySendRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/bank-favor/users/{openid}/bank-multi-activity");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingBankFavorService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SendBankFavorMultiActivityAsync`）；请求体 `BankFavorMultiActivitySendRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingBankFavorService](./IWechatPayMarketingBankFavorService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingBankFavorContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankFavorContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MarketingBankFavorTopology_ShouldBeOneInterfaceOnOneGroup`、`MarketingBankFavorEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MarketingBankFavorDtos_ShouldExposeOfficialFieldRosters`、`MarketingBankFavorDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MarketingBankFavor`](../../../Mud.Wechat.Pay.DataModels/MarketingBankFavor/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingBankFavorContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

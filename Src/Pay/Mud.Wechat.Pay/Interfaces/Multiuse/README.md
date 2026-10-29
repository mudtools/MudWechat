# Multiuse 域 · 多次扣费券/消费金（发券与账单流水）

微信支付「多次扣费券/消费金（发券与账单流水）」域 SDK（4 端点）。 接口：[IWechatPayMultiuseService](./IWechatPayMultiuseService.cs)；注册组 `Multiuse`；模块枚举 `PayModule.Multiuse`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMultiuseContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMultiuseContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/Multiuse`](../../DataModels/Multiuse)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMultiuseService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMultiuseService` | `QueryMultiuseSendFlowAsync` | GET | `/v3/multiuse/stocks/{stockId}/send-flow` | 商户证书 / APIv3 签名 | 下载批次发放明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539756（【普通商户】）。 |
| 2 | `IWechatPayMultiuseService` | `QueryMultiuseUseFlowAsync` | GET | `/v3/multiuse/stocks/{stockId}/use-flow` | 商户证书 / APIv3 签名 | 下载批次核销明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539758（【普通商户】）。 |
| 3 | `IWechatPayMultiuseService` | `QueryMultiuseRefundFlowAsync` | GET | `/v3/multiuse/stocks/{stockId}/refund-flow` | 商户证书 / APIv3 签名 | 下载批次退款明细。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539762（【普通商户】）。 |
| 4 | `IWechatPayMultiuseService` | `CreateMultiuseCouponAsync` | POST | `/v3/multiuse/users/{openid}/coupons` | 商户证书 / APIv3 签名 | 发放指定批次的消费金。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539766（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中**4 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：4 / 4 支官方页**印有**端点级数字（已原样抄入各方法 remarks），其余支**未印** ⇒ 未印者不得凭第三方 SDK 补数。

**涉密面是逐页核对结论**：本接口 4 支里**1 支**的官方请求头表把 `Wechatpay-Serial` 列为**必填**（敏感字段密文上送），其余支**未列**该头 ⇒ 不得按「同域一律涉密」补齐，也不得反向省略；已落进各方法 remarks。声明式签名一律不承载该头（同本线既有涉密域口径），密文以 `string` 承载、SDK **不代为加解密**。

**四支官方全部只印商户页**（「支持商户：【普通商户】」）⇒ 无服务商镜像；发放对象由 path 段 `openid` 承载， 请求体里没有 `sub_mchid` 相关键。

**三支流水下载应答同形 ⇒ 共用一型**：发放 / 核销 / 退款明细（`send-flow` / `use-flow` / `refund-flow`） 官方应答表均为单键 `url` `string(2048)` 必填、query 均为 `begin_date` / `end_date` ⇒ 共用 `MultiuseFlowUrlResponse`；发券支应答单键 `coupon_id` 另建一型。

**本域是行业方案里唯一逐支印限速与幂等口径的族**：四支官方正文均写「接口频率：不区分来源 **1000/min**」 +「幂等规则：接口**支持幂等重入**」⇒ 已逐支照抄；流水下载链接的有效期原文为 **30s**，拿到 `url` 须立即下载。

**发券支是本域唯一涉密支**：官方请求头表把 `Wechatpay-Serial` 列为**必填**，且 `user_name` / `id_card_number` 类型为 `string(2048)` 密文 ⇒ 以 `string` 承载、SDK **不代为加解密**； 同域其余三支**未列**该头（GET 流水无需上送密文），涉密是**逐页**结论。

**发券四条业务约束照页**：「商户**默认只允许发放本商户号**创建的消费金」「消费金**不涉及跨商户发券**」 「若发放消费金，则领取用户**必须为微信实名用户**，否则将发放失败」「该接口只能使用**创建活动方**进行调用」⇒ 幂等键是 `out_request_no`，业务码 `USER_NOT_EXIST`（实名信息错误）与 `NOT_ENOUGH`（预算/余额/单天限额）不可重试。

**官方页引言跨页抄用 ⇒ 语义以标题 + 路由为准**：发放明细（`send-flow`）页首引言印的是 「可获取到某批次的**退款**明细数据」（与退款支同文）⇒ 不得据引言把三支并成一支。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**支持商户**【普通商户】。
- （出现 4 次）**频率限制**（官方原样）：接口频率：不区分来源 1000/min。
- （出现 3 次）**业务错误码**：400 INVALID_REQUEST：非法商户号 / 400 PARAM_ERROR：指定范围内不存在账单 / 429 FREQUENCY_LIMITED：调用频率过高。

### 1. `QueryMultiuseSendFlowAsync` — 下载批次发放明细。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539756（【普通商户】）。

- **声明接口**：[IWechatPayMultiuseService](./IWechatPayMultiuseService.cs)
- **官方契约**：`GET /v3/multiuse/stocks/{stockId}/send-flow`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】微信为每个代金券批次分配的唯一ID（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
| `beginDate` | Query `begin_date` | `string?` | 【账单开始日期】 账单开始日期，最长下载31天区间的（官方 query `begin_date`，选填，类型 `string`）。 |
| `endDate` | Query `end_date` | `string?` | 【账单结束日期】 账单结束日期，最长下载31天区间的（官方 query `end_date`，选填，类型 `string`）。 |
- **返回**：`MultiuseFlowUrlResponse` — 下载批次发放明细，见 `MultiuseFlowUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/multiuse/stocks/{stock_id}/send-flow`；path `stock_id`，query `begin_date` / `end_date`。

### 2. `QueryMultiuseUseFlowAsync` — 下载批次核销明细。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539758（【普通商户】）。

- **声明接口**：[IWechatPayMultiuseService](./IWechatPayMultiuseService.cs)
- **官方契约**：`GET /v3/multiuse/stocks/{stockId}/use-flow`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】成功创建政府消费卡活动后，微信为每个批次分配的唯一ID（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
| `beginDate` | Query `begin_date` | `string?` | 【账单开始时间】 账单开始日期，最长下载31天区间的（官方 query `begin_date`，选填，类型 `string`）。 |
| `endDate` | Query `end_date` | `string?` | 【账单结束时间】 账单结束时间，最长下载31天区间的（官方 query `end_date`，选填，类型 `string`）。 |
- **返回**：`MultiuseFlowUrlResponse` — 下载批次核销明细，见 `MultiuseFlowUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/multiuse/stocks/{stock_id}/use-flow`；path `stock_id`，query `begin_date` / `end_date`。

### 3. `QueryMultiuseRefundFlowAsync` — 下载批次退款明细。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539762（【普通商户】）。

- **声明接口**：[IWechatPayMultiuseService](./IWechatPayMultiuseService.cs)
- **官方契约**：`GET /v3/multiuse/stocks/{stockId}/refund-flow`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `stockId` | Path | `string` | 【批次号】微信为每个代金券批次分配的唯一ID（官方 path `stock_id`，**必填**，类型 `string(20)`）。 |
| `beginDate` | Query `begin_date` | `string?` | 【账单开始时间】 账单开始日期，最长下载31天区间的（官方 query `begin_date`，选填，类型 `string`）。 |
| `endDate` | Query `end_date` | `string?` | 【账单结束时间】 账单结束时间，最长下载31天区间的（官方 query `end_date`，选填，类型 `string`）。 |
- **返回**：`MultiuseFlowUrlResponse` — 下载批次退款明细，见 `MultiuseFlowUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/multiuse/stocks/{stock_id}/refund-flow`；path `stock_id`，query `begin_date` / `end_date`。

### 4. `CreateMultiuseCouponAsync` — 发放指定批次的消费金。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012539766（【普通商户】）。

- **声明接口**：[IWechatPayMultiuseService](./IWechatPayMultiuseService.cs)
- **官方契约**：`POST /v3/multiuse/users/{openid}/coupons`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户OpenID】 OpenID信息，用户在AppID下的唯一标识。（官方 path `openid`，**必填**，类型 `string(128)`）。 |

- **请求体**：`MultiuseCouponCreateRequest` — 发放指定批次的消费金请求体，字段见 `MultiuseCouponCreateRequest`。
- **返回**：`MultiuseCouponCreateResponse` — 发放指定批次的消费金，见 `MultiuseCouponCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/multiuse/users/{openid}/coupons`；path `openid`。
- **必须带 `Wechatpay-Serial`**：官方请求头表把该头列为**必填**（请求参数中的敏感字段须以微信支付公钥或平台证书公钥加密上送）。声明式签名**不**承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；密文以 `string` 承载，SDK **不代为加解密**。
- **应答**见 `MultiuseCouponCreateResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH：商户号与appid不匹配 / 400 INVALID_REQUEST：非法商户号 / 400 INVALID_REQUEST：openid与appid不匹配 / 400 INVALID_REQUEST：活动已结束或未激活 / 400 INVALID_REQUEST：批次信息获取失败，请确认参数是否有误 / 400 PARAM_ERROR：非法的批次状态 / 403 MCH_NOT_EXISTS：商户号不合法 / 403 NOT_ENOUGH：批次预算不足 / 403 NOT_ENOUGH：发券超过单天限额 / 403 NOT_ENOUGH：账户余额不足，请充值 / 403 NOT_ENOUGH：批次预算耗尽 / 403 REQUEST_BLOCKED：商户无权发券 / 403 REQUEST_BLOCKED：批次不支持跨商户发券 / 403 REQUEST_BLOCKED：用户被限领拦截 / 403 REQUEST_BLOCKED：仅在广告场景下发放批次 / 403 RULE_LIMIT：用户已达最大领券次数 / 403 RULE_LIMIT：被自然人规则拦截 / 403 USER_ACCOUNT_ABNORMAL：用户非法 / 404 RESOURCE_NOT_EXISTS：批次不存在 / 404 USER_NOT_EXIST：用户实名信息错误，请检查输入 / 404 USER_NOT_EXIST：用户信息错误，请检查输入 / 404 USER_NOT_EXIST：openid不匹配，请检查输入 / 404 USER_NOT_EXIST：用户信息解密错误，请检查输入 / 429 FREQUENCY_LIMITED：调用频率过高。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/Multiuse`](../../DataModels/Multiuse) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `Multiuse`。

AOT 源生成上下文：[`MultiuseJsonContext.g.cs`](./../../DataModels/Generated/MultiuseJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MultiuseFlowUrlResponse` | 应答 | [Multiuse/MultiuseFlowDtos.cs](../../DataModels/Multiuse/MultiuseFlowDtos.cs) | `url` |
| `MultiuseCouponCreateResponse` | 应答 | [Multiuse/MultiuseCouponDtos.cs](../../DataModels/Multiuse/MultiuseCouponDtos.cs) | `stock_id`, `out_request_no`, `user_name`, `id_card_number`, `amount`, `appid`, `card_type`, `coupon_id` |
| `MultiuseCouponCreateRequest` | 请求体 | [Multiuse/MultiuseCouponDtos.cs](../../DataModels/Multiuse/MultiuseCouponDtos.cs) | `stock_id`, `out_request_no`, `user_name`, `id_card_number`, `amount`, `appid`, `card_type`, `coupon_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Multiuse;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Multiuse));

var service = app.Services.GetRequiredService<IWechatPayMultiuseService>();
var ct = CancellationToken.None;
var response = await service.CreateMultiuseCouponAsync("", new MultiuseCouponCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/multiuse/users/{openid}/coupons");
// 占位值仅用于编译示意；业务值须按 IWechatPayMultiuseService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateMultiuseCouponAsync`）；请求体 `MultiuseCouponCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMultiuseService](./IWechatPayMultiuseService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMultiuseContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMultiuseContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MultiuseTopology_ShouldMatchInterfaceAndGroupRoster`、`MultiuseEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MultiuseDtos_ShouldExposeOfficialFieldRosters`、`MultiuseDataModels_ShouldSplitRequestAndResponseBaseTypes`、`MultiuseAllDtos_ShouldBeAnnotatedAndContextRegistered`、`MultiuseRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/Multiuse`](../../DataModels/Multiuse) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMultiuseContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

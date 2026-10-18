# Refund 域 · 退款

微信支付「退款」域 SDK（APIv3 申请退款 / 查询退款 / 发起异常退款，3 端点）。 接口：[IWechatPayRefundService](./IWechatPayRefundService.cs)；注册组 `Refund`；模块枚举 `PayModule.Refund`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayCombineContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCombineContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayEcommerceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[WechatPayRateActivityContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRateActivityContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/Refund`](../../../Mud.Wechat.Pay.DataModels/Refund/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayRefundService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayRefundService` | `CreateRefundAsync` | POST | `/v3/refund/domestic/refunds` | 商户证书 / APIv3 签名 | 申请退款。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791903<br>（官方接口名 `DirectAPIv3Refund`… |
| 2 | `IWechatPayRefundService` | `QueryRefundAsync` | GET | `/v3/refund/domestic/refunds/{outRefundNo}` | 商户证书 / APIv3 签名 | 查询单笔退款（按**商户退款单号**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791904<br>（官方接口名 `Dir… |
| 3 | `IWechatPayRefundService` | `ApplyAbnormalRefundAsync` | POST | `/v3/refund/domestic/refunds/{refundId}/apply-abnormal-refund` | 商户证书 / APIv3 签名 | 发起异常退款（退款单 `status = ABNORMAL` 时使用）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/401279… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 逐页核验）：
退款申请 https://pay.weixin.qq.com/doc/v3/merchant/4012791903（同内容另有 `4013071036`）、
查询单笔退款 https://pay.weixin.qq.com/doc/v3/merchant/4012791904、
发起异常退款 https://pay.weixin.qq.com/doc/v3/merchant/4012791905。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateRefundAsync` — 申请退款。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791903
（官方接口名 `DirectAPIv3Refund`；页面更新时间 2025.01.09）。

- **声明接口**：[IWechatPayRefundService](./IWechatPayRefundService.cs)
- **官方契约**：`POST /v3/refund/domestic/refunds`

- **请求体**：`RefundApplyRequest` — 退款请求体，字段见 `RefundApplyRequest`。
- **返回**：`RefundResponse` — 退款单详情（状态多为 `PROCESSING`，最终结果另经回调 / 查询获取），见 `RefundResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/refund/domestic/refunds`；支持商户类型：**普通商户**； / 必带 `Accept: application/json` 与 `Content-Type: application/json`。
- **业务限制（官方原文要点）**：① `out_refund_no` 须 6-32 字符、同一商户号下唯一； / ② 单笔订单累计退款金额不得超过订单总额（超限报 `REFUND_FEE_INVALID` / `PARAM_ERROR`）； / ③ 退款申请为**异步受理**，返回的 `status` 多为 `PROCESSING`， / 不得据本次应答判定退款成功；④ 退款结果经退款结果回调通知或「查询单笔退款」获取。

### 2. `QueryRefundAsync` — 查询单笔退款（按**商户退款单号**）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791904
（官方接口名 `DirectAPIv3Refund/out-refund-no`；页面更新时间 2025.01.09）。

- **声明接口**：[IWechatPayRefundService](./IWechatPayRefundService.cs)
- **官方契约**：`GET /v3/refund/domestic/refunds/{outRefundNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRefundNo` | Path | `string` | 商户退款单号（路径参数）。 |
- **返回**：`RefundResponse` — 退款单详情，见 `RefundResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/refund/domestic/refunds/{out_refund_no}`； / **无 query 参数**；必带 `Accept: application/json`。
- **异步语义**：退款受理后须经本接口**轮询**至终态（`SUCCESS` / `CLOSED` / `ABNORMAL`）， / 或等待退款结果回调；`status = ABNORMAL` 时应调用 `ApplyAbnormalRefundAsync`。

### 3. `ApplyAbnormalRefundAsync` — 发起异常退款（退款单 `status = ABNORMAL` 时使用）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791905
（官方接口名 `DirectAPIv3Refund/apply-abnormal-refund`；页面更新时间 2024.12.30）。

- **声明接口**：[IWechatPayRefundService](./IWechatPayRefundService.cs)
- **官方契约**：`POST /v3/refund/domestic/refunds/{refundId}/apply-abnormal-refund`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `refundId` | Path | `string` | 微信支付退款单号（路径参数，**非**商户退款单号）。 |

- **请求体**：`AbnormalRefundRequest` — 异常退款请求体，字段见 `AbnormalRefundRequest`。
- **返回**：`RefundResponse` — 更新后的退款单详情，见 `RefundResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/refund/domestic/refunds/{refund_id}/apply-abnormal-refund`； / 支持商户类型：**普通商户**；`refund_id` 为 path 参数。
- **业务限制（官方原文要点）**：① 仅当退款单 `status = ABNORMAL`（用户账户异常 / 银行卡注销） / 时可用；② 属**变更退款去向**的不可逆操作，须谨慎调用。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/Refund`](../../../Mud.Wechat.Pay.DataModels/Refund/) 内共 **8** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **8** 个，`SerializerClassName` = `Refund`。

AOT 源生成上下文：[`RefundJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/RefundJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RefundResponse` | 应答 | [Refund/RefundResponse.cs](../../../Mud.Wechat.Pay.DataModels/Refund/RefundResponse.cs) | `refund_id`, `out_refund_no`, `transaction_id`, `out_trade_no`, `channel`, `user_received_account`, `success_time`, `create_time`, `status`, `funds_account`, `amount`, `promotion_detail` … |
| `RefundApplyRequest` | 请求体 | [Refund/RefundApplyRequest.cs](../../../Mud.Wechat.Pay.DataModels/Refund/RefundApplyRequest.cs) | `transaction_id`, `out_trade_no`, `out_refund_no`, `reason`, `notify_url`, `funds_account`, `amount`, `goods_detail`, `refund`, `from`, `total`, `currency` … |
| `AbnormalRefundRequest` | 请求体 | [Refund/AbnormalRefundRequest.cs](../../../Mud.Wechat.Pay.DataModels/Refund/AbnormalRefundRequest.cs) | `out_refund_no`, `type`, `bank_type`, `bank_account`, `real_name` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Refund;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Refund));

var service = app.Services.GetRequiredService<IWechatPayRefundService>();
var ct = CancellationToken.None;
var response = await service.CreateRefundAsync(new RefundApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/refund/domestic/refunds");
// 占位值仅用于编译示意；业务值须按 IWechatPayRefundService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateRefundAsync`）；请求体 `RefundApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayRefundService](./IWechatPayRefundService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayCombineContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCombineContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayEcommerceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayEcommerceContractGuards.cs)、[`WechatPayRateActivityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRateActivityContractGuards.cs) |
| 守卫中的代表性测试方法 | `CombineEndpoints_ShouldMatchOfficialRoutes`、`CombineDtos_ShouldExposeOfficialFieldNames`、`CombineRefundEndpoint_ShouldStayUnmodeled`、`QueryShapes_ShouldStayStructurallySeparate`、`SubOrderShapes_ShouldStayStructurallySeparate`、`CombineDataModels_ShouldBeRegisteredInJsonContext`、`AppAndH5Prepay_ShouldSplitAndShareTypesPerOfficialTables`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/Refund`](../../../Mud.Wechat.Pay.DataModels/Refund/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayCombineContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayEcommerceContractGuards.cs`、`WechatPayRateActivityContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

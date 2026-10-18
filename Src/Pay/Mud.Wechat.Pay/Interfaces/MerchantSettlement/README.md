# MerchantSettlement 域 · 商户结算规则申请

微信支付「商户结算规则申请」域 SDK（**2 端点**：创建结算规则申请单 + 查询申请单状态）。 接口：[IWechatPayMerchantSettlementService](./IWechatPayMerchantSettlementService.cs)；注册组 `MerchantSettlement`；模块枚举 `PayModule.MerchantSettlement`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantSettlementContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantSettlementContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MerchantSettlement`](../../../Mud.Wechat.Pay.DataModels/MerchantSettlement/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantSettlementService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantSettlementService` | `CreateSettleRuleApplicationAsync` | POST | `/v3/merchant-settlement/merchant-settle-rule-applications` | 商户证书 / APIv3 签名 | 「创建商户结算规则申请单」：为特约商户提交一条结算规则，返回申请单号与当前状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401… |
| 2 | `IWechatPayMerchantSettlementService` | `QuerySettleRuleApplicationAsync` | GET | `/v3/merchant-settlement/merchant-settle-rule-applications/{applicationId}` | 商户证书 / APIv3 签名 | 「查询商户结算规则申请单状态」：按创建应答的 `application_id` 轮询，应答与创建支同型。<br>官方文档：https://pay.weixin.qq.com/doc/v3… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 逐字段核验）：创建
https://pay.weixin.qq.com/doc/v3/partner/4012697924、查询状态
https://pay.weixin.qq.com/doc/v3/partner/4012698574。

**本域是从业机构面**：两支页面的「支持商户」都是【从业机构（银行）】【从业机构（支付机构）】
⇒ 报文用 `acquiring_bank_id` + `channel_id` + `sub_mchid` 三层标识，
**没有** `sp_mchid`/`sub_appid` 那套服务商前缀，也不与
`IWechatPayBankTransferService` 的六层标识互换（守卫 MSET-B3 锁字段名）。

**两支同前缀 `/v3/merchant-settlement/merchant-settle-rule-applications`**：
创建是 POST 无尾段、查询是 GET 带 `/{application_id}` ⇒ 两条不同 (方法, 路由)（守卫 MSET-B1）。

**两支应答表逐字段相同**（8 键、同名同类型同必填性）⇒ 共用
`MerchantSettlementRuleApplicationResponse`（守卫 MSET-B6）。

**创建 ≠ 生效**：`application_state` 三值里只有 `PASSED` / `FAILED` 是终态，
创建应答就可能是 `PROCESSING` ⇒ 唯一闭环路径是轮询查询支；`application_process_info`
是给人看的描述串（string(256)），**不是**结构化进度节点，不得据它判状态。

**`settle_rule_id` 官方是 integer**（请求与应答两侧同为整型，请求侧必填）
⇒ 以 `long` 承载，传字符串会落到 400 `PARAM_ERROR` 而不是编译期提示。

**`update_time` 是 rfc3339 字符串**（string(128)）⇒ 本包全域时间字段一律 `string`，
不在 DTO 里做 `DateTimeOffset` 转换（两套官方时间格式并存的口径见
`PayModule` 各域注释）。

**两支页内无端点级频率数字**（两轮复核一致）。**业务错误码**为 APIv3 公共码
（400 `PARAM_ERROR` / 400 `INVALID_REQUEST` / 401 `SIGN_ERROR` / 500 `SYSTEM_ERROR`），
原样透传至 `Code`。

**本章不含「结算规则查询」与「默认规则设置」**：官方在 `商户结算规则` 章节内只文档这两支
HTTP 面（创建 + 按申请单号查），无「直接查生效规则」的读接口 ⇒ 零端点由守卫 MSET-B2 带正对照锁定。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，
三层标识是**报文参数**而非作用域；本域**零 `[Token]`**（守卫 PAY-B1）、
两页均未把 `Wechatpay-Serial` 列入参数表 ⇒ **无加密字段**。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateSettleRuleApplicationAsync` — 「创建商户结算规则申请单」：为特约商户提交一条结算规则，返回申请单号与当前状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697924。

- **声明接口**：[IWechatPayMerchantSettlementService](./IWechatPayMerchantSettlementService.cs)
- **官方契约**：`POST /v3/merchant-settlement/merchant-settle-rule-applications`

- **请求体**：`MerchantSettlementRuleApplicationCreateRequest` — 请求体，字段见 `MerchantSettlementRuleApplicationCreateRequest`。
- **返回**：`MerchantSettlementRuleApplicationResponse` — 申请单号与状态，见 `MerchantSettlementRuleApplicationResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-settlement/merchant-settle-rule-applications`； / 支持商户【从业机构（银行）】【从业机构（支付机构）】；无 path / query 参数。
- **四个字段全必填**：`acquiring_bank_id` / `channel_id` / `sub_mchid` / / `settle_rule_id` 缺一即 400 `PARAM_ERROR` ⇒ 官方没有「按当前登录商户推断」的缺省。
- **受理不等于生效**：应答的 `application_state` 可能是 `PROCESSING`， / 须轮询 `QuerySettleRuleApplicationAsync` 到 `PASSED` / `FAILED`。

### 2. `QuerySettleRuleApplicationAsync` — 「查询商户结算规则申请单状态」：按创建应答的 `application_id` 轮询，应答与创建支同型。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698574。

- **声明接口**：[IWechatPayMerchantSettlementService](./IWechatPayMerchantSettlementService.cs)
- **官方契约**：`GET /v3/merchant-settlement/merchant-settle-rule-applications/{applicationId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applicationId` | Path | `string` | 申请单号（官方 path `application_id`，必填 string）：创建应答返回。 |
- **返回**：`MerchantSettlementRuleApplicationResponse` — 与创建支**同一应答类型**，见 `MerchantSettlementRuleApplicationResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/merchant-settlement/merchant-settle-rule-applications/{application_id}`；无 query 参数、无包体。
- **只有申请单号一条定位路径**：官方未提供「按商户号查最新申请单」的接口 ⇒ / 宿主必须持久化创建应答的 `application_id`，丢了就没有查询通道（守卫 MSET-B2）。
- **终态后应答仍带全套请求字段**：`acquiring_bank_id` / `channel_id` / / `sub_mchid` / `settle_rule_id` 在查询侧回显（官方标选填）⇒ 以回显值与本地台账比对， / 不以「查到了」代替「查对了」。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MerchantSettlement`](../../../Mud.Wechat.Pay.DataModels/MerchantSettlement/) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `MerchantSettlement`。

AOT 源生成上下文：[`MerchantSettlementJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MerchantSettlementJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantSettlementRuleApplicationResponse` | 应答 | [MerchantSettlement/MerchantSettlementDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantSettlement/MerchantSettlementDtos.cs) | `acquiring_bank_id`, `channel_id`, `sub_mchid`, `settle_rule_id`, `application_id`, `application_process_info`, `application_state`, `update_time` |
| `MerchantSettlementRuleApplicationCreateRequest` | 请求体 | [MerchantSettlement/MerchantSettlementDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantSettlement/MerchantSettlementDtos.cs) | `acquiring_bank_id`, `channel_id`, `sub_mchid`, `settle_rule_id`, `application_id`, `application_process_info`, `application_state`, `update_time` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MerchantSettlement;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantSettlement));

var service = app.Services.GetRequiredService<IWechatPayMerchantSettlementService>();
var ct = CancellationToken.None;
var response = await service.CreateSettleRuleApplicationAsync(new MerchantSettlementRuleApplicationCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/merchant-settlement/merchant-settle-rule-applications");
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantSettlementService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateSettleRuleApplicationAsync`）；请求体 `MerchantSettlementRuleApplicationCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantSettlementService](./IWechatPayMerchantSettlementService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantSettlementContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantSettlementContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantSettlementEndpoints_ShouldMatchOfficialRoutesAndShareResponseType`、`MerchantSettlementRoutes_ShouldStayOnOwnPrefixAndNotDeclareUnofficialFaces`、`MerchantSettlementDtos_ShouldExposeOfficialFieldNamesAndIntegerRuleId`、`MerchantSettlementApplicationStates_ShouldMatchOfficialThreeValuesWithTwoTerminal`、`MerchantSettlementUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted`、`MerchantSettlementResponses_ShouldDeriveFromBaseAndFollowSharedTableRuling`、`MerchantSettlementDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MerchantSettlement`](../../../Mud.Wechat.Pay.DataModels/MerchantSettlement/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantSettlementContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

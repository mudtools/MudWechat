# Repayment 域 · 平台商户统一收单·还款

微信支付「平台商户统一收单·还款」域 SDK（**3 端点**：合单预下单 + 查单 + 关单）。 接口：[IWechatPayRepaymentService](./IWechatPayRepaymentService.cs)；注册组 `Repayment`；模块枚举 `PayModule.Repayment`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayRepaymentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRepaymentContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/Repayment`](../../DataModels/Repayment)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayRepaymentService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayRepaymentService` | `PrepayAsync` | POST | `/v3/repayment/combine-transactions/partner/prepay` | 商户证书 / APIv3 签名 | 平台商户「还款合单预下单」：一次为多笔待还子单生成一个合单预支付会话。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/40151244… |
| 2 | `IWechatPayRepaymentService` | `QueryByCombineOutTradeNoAsync` | GET | `/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}` | 商户证书 / APIv3 签名 | 平台商户「查询还款合单订单」：按合单商户订单号查合单状态与各子单支付结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401512… |
| 3 | `IWechatPayRepaymentService` | `CloseAsync` | POST | `/v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}/close` | 商户证书 / APIv3 签名 | 平台商户「关闭还款单」：关闭合单下的指定子单。成功应答为 **204 No Content（无包体）**。<br>官方文档：https://pay.weixin.qq.com/doc/v… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（合作伙伴文档中心 ·「统一收单」下的还款分支，2026-10-14 逐页核验，
各页「支持商户」栏恒为【平台商户】）：还款合单预下单
https://pay.weixin.qq.com/doc/v3/partner/4015124463、按合单商户订单号查询
https://pay.weixin.qq.com/doc/v3/partner/4015124483、关闭还款单
https://pay.weixin.qq.com/doc/v3/partner/4015124497。

**「还款」是合单支付的一个专用面，不是 `CombineTransactions` 的子集**：
路由前缀 `/v3/repayment/combine-transactions/partner/`（合单面是
`/v3/combine-transactions/`），且**必填 `combine_mchid` 语义反转** ——
官方在子单 `mchid` 一栏原文写明「此处请填写服务商(还款发起方)的商户号 `combine_mchid`」
⇒ 照抄合单支付的子单填法会把钱收错账户（守卫 RP-B3 锁该字段说明）。

**签名主体是平台商户号**：宿主应以平台商户配置进入调用作用域；
`combine_mchid` 与 `sub_mchid` 均为**报文参数**而非作用域。
**本域只借「商户号 + 私钥」这一既有承载面**：平台收付通的账本 / 二级商户体系
**不在本仓建模**（守卫 `WechatPayCapitalRulingContractGuards` C1/C2 的裁决边界）。

**本域 `h5_info` 是「渠道信息」聚合体，不是直连面的扁平 `scene_info`**：
内含 `combine_appid` / `payer_client_ip` / `type`（Wap / iOS / Android / Harmony）
/ `app_url` 等，字段名带 `combine_` 前缀。四个端型信息块
（`miniprogram_info` / `app_info` / `h5_info` / `wechat_web_info`）
官方要求**至少传一个** ⇒ SDK 侧不做「四选一」编译期约束（声明式无联合类型），
由宿主按 `h5_url` 是否返回判别调起方式。

**应答是「预支付」产物，不是支付结果**：`prepay_id` 供端上调起、
`h5_url` 供网页跳转，**两者皆选填**（按传入的信息块出现）⇒
拿到 200 ≠ 用户已付款，支付结果只经**回调通知**或查单得出。

**调起支付与回调通知不在本域**：端上调起是 JSAPI /小程序 / APP / H5 四支
**参数签名**（前端形态、无 HTTP 端点），支付成功回调是**接收**面（由
`Mud.Wechat.Pay.Callback` 承载）⇒ 本域**零端点**（守卫 RP-B2 带正对照）。

**关单是 204 无包体** ⇒ 声明式返回裸 `Task`（守卫 RP-B1 锁定），
且定位键在 path（`combine_out_trade_no`）、包体只带待关的**子单**列表。

**`time_expire` 是「支付结束时间」而非「订单关闭时间」**：仅首次下单可设、
**后续修改必报错**；超时的订单只能**换新 `combine_out_trade_no` 重新下单**，
未超时则可用原参数重取 `prepay_id`。格式为 rfc3339 字符串（支付线时间字段一律
`string`，不做 `DateTimeOffset` 解析）。

**`combine_out_trade_no` 须在同一商户号下唯一**；官方两页对合法字符集的描述
**不一致**（下单页 `_-|*`、查询页另含 `@`）⇒ 宿主生成单号须按**最窄集**，
不要依赖查询页的宽集（守卫 RP-B3 只锁字段名，不锁字符集）。

**子单最多 10 条**；`settle_info.subsidy_amount` 官方限**单笔 ≤ 5000 元**；
金额单位一律**分**。

**实名信息为密文**：`combine_payer_info.identity` 的 `number` / `name`
传 RSA 密文、请求头 `Wechatpay-Serial` 随之**必填**；类型 `type` 有官方 7 值表
（见 `RepaymentIdentityTypes`）。查单应答的 `combine_payer_info` 形态
**与请求侧不同**：那里只有选填 `openid` ⇒ 两型分建（守卫 RP-B4）。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse` 的
`Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `PrepayAsync` — 平台商户「还款合单预下单」：一次为多笔待还子单生成一个合单预支付会话。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015124463。

- **声明接口**：[IWechatPayRepaymentService](./IWechatPayRepaymentService.cs)
- **官方契约**：`POST /v3/repayment/combine-transactions/partner/prepay`

- **请求体**：`RepaymentPrepayRequest` — 请求体，字段见 `RepaymentPrepayRequest`。
- **返回**：`RepaymentPrepayResponse` — `prepay_id` 与 / 或 `h5_url`，见 `RepaymentPrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** / `/v3/repayment/combine-transactions/partner/prepay`；无 path / query 参数； / 请求头 `Wechatpay-Serial` **必填**（官方注明填**微信支付公钥 ID**（推荐）或 / 平台证书序列号，敏感字段须先用其加密）。
- **四块端型信息至少传一个**（`miniprogram_info` / `app_info` / / `h5_info` / `wechat_web_info`）：官方以文案约束而非字段必填 ⇒ 本方法不代校验， / 全不传时的报错由微信给出。
- **子单 `mchid` 填平台商户号自身**（官方原文「此处请填写服务商(还款发起方)的商户号 / `combine_mchid`」）：这是本域相对合单支付最容易踩反的一处。
- **`settle_info.subsidy_amount` 仅在 `profit_sharing = true` 时生效** / （官方明写），且单笔最高补差 5000 元 ⇒ 不指定分账时传补差是无效字段。
- **受理 ≠ 支付成功**：应答仅给出调起凭据，结果须经回调或 `QueryByCombineOutTradeNoAsync`。

### 2. `QueryByCombineOutTradeNoAsync` — 平台商户「查询还款合单订单」：按合单商户订单号查合单状态与各子单支付结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015124483。

- **声明接口**：[IWechatPayRepaymentService](./IWechatPayRepaymentService.cs)
- **官方契约**：`GET /v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 合单商户订单号（官方 path `combine_out_trade_no`，必填 string(32)）。 |
- **返回**：`RepaymentQueryResponse` — 合单概况与子单列表，见 `RepaymentQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}`；无 query 参数。
- **定位键只有商家单号**：本域**没有**「按微信单号查单」的第二支 / （直连 / 服务商基础支付各有双路）⇒ 单号即唯一凭据，宿主必须持久化。
- **子单 `trade_state` 只有三值**（SUCCESS / NOTPAY / CLOSED，见 / `RepaymentTradeStates`）：比基础支付的七值集合窄得多， / **不得**复用基础支付的状态判定器。
- **`bank_type` 非银行卡时官方回 `OTHERS`**（不是空值）⇒ 判空逻辑须按此写。

### 3. `CloseAsync` — 平台商户「关闭还款单」：关闭合单下的指定子单。成功应答为 **204 No Content（无包体）**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015124497。

- **声明接口**：[IWechatPayRepaymentService](./IWechatPayRepaymentService.cs)
- **官方契约**：`POST /v3/repayment/combine-transactions/partner/out-trade-no/{combineOutTradeNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 合单商户订单号（官方 path `combine_out_trade_no`，必填 string(32)）。 |

- **请求体**：`RepaymentCloseRequest` — 请求体（待关子单列表），字段见 `RepaymentCloseRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** / `/v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}/close`。
- **204 无包体 ⇒ 返回裸 `Task`**：本域唯一无 JSON 应答的端点 / （守卫 RP-B1 断言返回类型），业务失败仍以 4xx + `{"code","message"}` 呈现， / 由 `[AllowAnyStatusCode]` 透传后判错。
- **还款单号只在 path**：包体只有 `sub_orders`（`mchid` + `out_trade_no` 必填、 / `sub_mchid` 选填）⇒ 不要往包体里再塞 `combine_out_trade_no`。
- **只能整单关闭，且五要素须与下单完全一致**（官方原文）：「还款单支付订单只能使用此 / 还款单关单 api 完成关单。还款单支付关单接口不支持关闭部分还款单，关单的还款单订单号、 / 还款单发起商户号、还款明细单个数、还款明细单订单号、子商户号必须与下单时完全一致」 / ⇒ 传**子集**（少一笔明细）会被拒，普通支付面的关单接口对本域无效。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/Repayment`](../../DataModels/Repayment) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `Repayment`。

AOT 源生成上下文：[`RepaymentJsonContext.g.cs`](./../../DataModels/Generated/RepaymentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RepaymentPrepayResponse` | 应答 | [Repayment/RepaymentPrepayDtos.cs](../../DataModels/Repayment/RepaymentPrepayDtos.cs) | `combine_out_trade_no`, `combine_mchid`, `sub_orders`, `time_expire`, `notify_url`, `combine_payer_info`, `h5_info`, `app_info`, `miniprogram_info`, `wechat_web_info`, `mchid`, `sub_mchid` … |
| `RepaymentPrepayRequest` | 请求体 | [Repayment/RepaymentPrepayDtos.cs](../../DataModels/Repayment/RepaymentPrepayDtos.cs) | `combine_out_trade_no`, `combine_mchid`, `sub_orders`, `time_expire`, `notify_url`, `combine_payer_info`, `h5_info`, `app_info`, `miniprogram_info`, `wechat_web_info`, `mchid`, `sub_mchid` … |
| `RepaymentQueryResponse` | 应答 | [Repayment/RepaymentQueryDtos.cs](../../DataModels/Repayment/RepaymentQueryDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `combine_transaction_id`, `combine_payer_info`, `sub_orders`, `openid`, `mchid`, `sub_mchid`, `out_trade_no`, `transaction_id`, `trade_type` … |
| `RepaymentCloseRequest` | 请求体 | [Repayment/RepaymentQueryDtos.cs](../../DataModels/Repayment/RepaymentQueryDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `combine_transaction_id`, `combine_payer_info`, `sub_orders`, `openid`, `mchid`, `sub_mchid`, `out_trade_no`, `transaction_id`, `trade_type` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Repayment;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Repayment));

var service = app.Services.GetRequiredService<IWechatPayRepaymentService>();
var ct = CancellationToken.None;
var response = await service.PrepayAsync(new RepaymentPrepayRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/repayment/combine-transactions/partner/prepay");
// 占位值仅用于编译示意；业务值须按 IWechatPayRepaymentService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`PrepayAsync`）；请求体 `RepaymentPrepayRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayRepaymentService](./IWechatPayRepaymentService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayRepaymentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRepaymentContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `RepaymentEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForClose`、`RepaymentRoutes_ShouldStayOnRepaymentPrefixAndNotDeclareClientSideOrCallbackOrRefundFaces`、`RepaymentDtos_ShouldExposeOfficialFieldNamesIncludingDigitBearingOnes`、`RepaymentConstantsAndTypeSplits_ShouldMatchOfficialTables`、`RepaymentResponses_ShouldDeriveFromWechatPayResponseAndNotFakeNoContentTypes`、`RepaymentDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/Repayment`](../../DataModels/Repayment) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayRepaymentContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

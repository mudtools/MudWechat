# Combine 域 · 合单支付

微信支付「合单支付」域 SDK（APIv3 合单，**6 端点**：JSAPI 合单下单 / Native 合单下单 /
**APP 合单下单** / **H5 合单下单** / 合单关闭订单 / 合单查询订单）。 接口：[IWechatPayCombineService](./IWechatPayCombineService.cs)；注册组 `CombineTransactions`；模块枚举 `PayModule.CombineTransactions`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayCombineContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCombineContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPayInstallmentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayInstallmentContractGuards.cs)、[WechatPayRepaymentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRepaymentContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/Combine`](../../../Mud.Wechat.Pay.DataModels/Combine/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayCombineService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayCombineService` | `CreateJsapiOrderAsync` | POST | `/v3/combine-transactions/jsapi` | 商户证书 / APIv3 签名 | JSAPI / 小程序合单下单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.sh… |
| 2 | `IWechatPayCombineService` | `CloseOrderAsync` | POST | `/v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close` | 商户证书 / APIv3 签名 | 合单关闭订单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012709095。 |
| 3 | `IWechatPayCombineService` | `QueryOrderAsync` | GET | `/v3/combine-transactions/out-trade-no/{combineOutTradeNo}` | 商户证书 / APIv3 签名 | 合单查询订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml。 |
| 4 | `IWechatPayCombineService` | `CreateNativeOrderAsync` | POST | `/v3/combine-transactions/native` | 商户证书 / APIv3 签名 | Native 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012758240。 |
| 5 | `IWechatPayCombineService` | `CreateAppOrderAsync` | POST | `/v3/combine-transactions/app` | 商户证书 / APIv3 签名 | APP 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015973099。 |
| 6 | `IWechatPayCombineService` | `CreateH5OrderAsync` | POST | `/v3/combine-transactions/h5` | 商户证书 / APIv3 签名 | H5 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012758208。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（**合作伙伴**文档中心，2026-10-09 逐页核验；本域按**服务商面**建模）：
JSAPI/小程序合单下单 https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml
（更新时间 2025.01.16）、Native 合单下单 https://pay.weixin.qq.com/doc/v3/partner/4012758240
（更新时间 2025.01.16）、合单关闭订单 https://pay.weixin.qq.com/doc/v3/partner/4012709095
（更新时间 2024.10.24）、合单查询订单
https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml。

**⚠️ 合单支付在官方文档中同时存在「普通商户」与「普通服务商」两个面**，本域按**服务商面**建模
—— 各页均标注**支持商户：【普通服务商】**，服务商模式支持 **1–50 笔**商品单。
普通商户面亦有对应页（Native 合单下单 `…/wiki/doc/apiv3/apis/chapter5_1_5.shtml`、
App 合单下单 `…/wiki/doc/apiv3/open/pay/chapter2_9_3.shtml`，均标注【普通商户】），
差异是**只支持 2–10 笔**、且子单字段表**无** `sub_mchid` / `sub_appid`。
**路由两面共用**，故本域 DTO 以服务商面为准；**普通商户接入时勿填 `sub_mchid` / `sub_appid`**。
本仓商户基座已支持服务商（`{sp_mchid}:{sub_mchid}` 分槽），故两面均可使用。

**✅ 四个下单场景现已齐全**（JSAPI / Native / APP / H5）—— 原「APP / H5 尚未覆盖」留档已随增量关闭。
四个接口**顶层字段表各不相同**，故各自独立 DTO；**下单应答也有两种**
（JSAPI / APP 给 `prepay_id`，Native 给 `code_url`，H5 给 `h5_url`）。

**本域仅剩「非 HTTP 能力」未覆盖**：调起支付（客户端 SDK）。
**✅ 通知面已完成**：合单支付成功通知载荷见回调包 `GetCombineTransaction`
（⚠️ 其信封 `event_type` / `original_type` 与普通支付成功通知**完全同值**，
判别只能靠**解密后的载荷形态**）；合单退款通知复用标准 `REFUND.*` 事件
（退款逐子单走退款域，**无**专属事件类型）。
**不要凭推断补路由**。

**⚠️「合单退款」不是一个接口**：官方《订单退款》开发指引
（https://pay.weixin.qq.com/doc/v3/partner/4013080623，更新 2026.06.10）明示
「对于合单支付的订单，**无法通过合单支付总单号 `combine_out_trade_no` 退款，
只能根据单个子单进行退款**」，且 `transaction_id` 须填 `sub_orders.transaction_id`、
`out_trade_no` 须填 `sub_orders.out_trade_no` ⇒ 退款**逐子单**调用退款域的
`IWechatPayRefundService` 即可，本域**不**另设端点（守卫 CB6 固化该裁决，
防后来者照第三方博客臆造的 `/v3/combine-transactions/refunds` 去补一个不存在的路由）。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateJsapiOrderAsync` — JSAPI / 小程序合单下单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`POST /v3/combine-transactions/jsapi`

- **请求体**：`CombinePrepayRequest` — 合单下单请求体，见 `CombinePrepayRequest`。
- **返回**：`CombinePrepayResponse` — 预支付交易会话标识（2 小时有效），见 `CombinePrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/combine-transactions/jsapi`；无 path / query 参数。
- **子单约束**：`sub_orders` **1–50 笔**；`sub_appid` **仅允许一笔**商品单填写。
- **支付者约束**：`combine_payer_info` 的 `openid` 与 `sub_openid` **二选一必填**， / 且传 `sub_openid` 时 `sub_appid` 必填。

### 2. `CloseOrderAsync` — 合单关闭订单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012709095。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`POST /v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 合单商户订单号（官方 path `combine_out_trade_no`，必填 string(32)）。 |

- **请求体**：`CombineCloseOrderRequest` — 关单请求体（`combine_appid` + `sub_orders`），见 `CombineCloseOrderRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/combine-transactions/out-trade-no/{combine_out_trade_no}/close`； / path 必带 `combine_out_trade_no`。
- **三条硬约束（官方原文）**：① 合单支付订单**只能**用本接口关单； / ② **不支持关闭部分子单**；③ 主单商户号 / 单号 / 子单个数 / 子单商户号 / 子单单号 / **必须与下单时完全一致**。
- **返回类型为何是 `Task`**：官方明确「无应答包体」，成功状态码 **204 No Content** / （与交易域关单、支付分解除授权同款处置）。

### 3. `QueryOrderAsync` — 合单查询订单。官方文档：https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`GET /v3/combine-transactions/out-trade-no/{combineOutTradeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `combineOutTradeNo` | Path | `string` | 合单商户订单号（官方 path `combine_out_trade_no`，必填 string(32)）。 |
- **返回**：`CombineQueryResponse` — 合单订单（含各商品单的交易状态与实付金额），见 `CombineQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/combine-transactions/out-trade-no/{combine_out_trade_no}`； / **无 query 参数**。
- **查询方式只有一种**：官方本页仅提供按**合单商户订单号**查询， / **没有**「按微信支付订单号」的等价入口（与单笔交易域的两种入口不同）—— 勿照搬交易域的直觉。
- **状态判定**：`sub_orders[].trade_state`（必填）是**每个商品单各自**的交易状态， / 须逐单显式判定；`amount.payer_amount` 才是实付金额（`total_amount` 是标价）。

### 4. `CreateNativeOrderAsync` — Native 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012758240。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`POST /v3/combine-transactions/native`

- **请求体**：`CombineNativePrepayRequest` — Native 合单下单请求体，见 `CombineNativePrepayRequest`。
- **返回**：`CombineNativePrepayResponse` — 支付二维码链接（`code_url`，**有效期 2 小时**），见 `CombineNativePrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/combine-transactions/native`；无 path / query 参数。
- **与 JSAPI 下单的三点差异**（勿混用 DTO）：① 请求**无支付者信息** / （无 `combine_payer_info`）；② 应答**只有 `code_url`**（无 `prepay_id`）； / ③ 支付由用户**扫码**发起，服务端须自行把 `code_url` 生成二维码展示。
- **`code_url` 有效期 2 小时**（官方原文）：失效后须**重新调用本接口**取新链接， / 不得对旧链接做拼接或改写。

### 5. `CreateAppOrderAsync` — APP 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015973099。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`POST /v3/combine-transactions/app`

- **请求体**：`CombineAppPrepayRequest` — APP 合单下单请求体，见 `CombineAppPrepayRequest`。
- **返回**：`CombinePrepayResponse` — 预支付交易会话标识（`prepay_id`，**有效期 2 小时**），见 `CombinePrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/combine-transactions/app`；无 path / query 参数。 / 支持商户**【普通服务商】【平台商户】**（更新时间 2025.09.02）。
- **⚠️ 请求里的 `prepay_id` 是「追加订单」专用**：官方中文说明为 / 「当**发起追加订单**，需传入目标追加的合单单号对应的预支付交易会话标识」 / ⇒ 首次下单**不传**；**勿与应答里的 `prepay_id` 混淆** / （同名、同意义，但一个是入参、一个是回参）。
- **应答类型与 JSAPI 共用**：APP 应答**同样只有 `prepay_id`**（两页应答表一致）⇒ / 复用 `CombinePrepayResponse`，由「调起支付」的客户端 SDK 场景决定怎么用。
- **子单表与 JSAPI 不**共用：APP 页的子单**多** `time_start` / `time_expire`， / 且 `sub_mchid` 被标为**选填**（JSAPI / H5 页为必填）⇒ 见 `CombineAppSubOrder` 的 remarks。

### 6. `CreateH5OrderAsync` — H5 合单下单。官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012758208。

- **声明接口**：[IWechatPayCombineService](./IWechatPayCombineService.cs)
- **官方契约**：`POST /v3/combine-transactions/h5`

- **请求体**：`CombineH5PrepayRequest` — H5 合单下单请求体，见 `CombineH5PrepayRequest`。
- **返回**：`CombineH5PrepayResponse` — H5 支付跳转链接（`h5_url`），见 `CombineH5PrepayResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/combine-transactions/h5`；无 path / query 参数。 / 服务商模式支持 **1–50 笔**（更新时间 2025.01.16）。
- **⚠️ 服务商面与普通商户面字段不同**：官方**普通商户**面同路由页 / （`…/doc/v3/merchant/4012545879`，产品线标注「指定身份支付」）**有** `combine_payer_info` / （含实名 `identity`）与 `time_start`；而**服务商面本页无**这两处 / ⇒ 本接口按**服务商面**建模（普通商户接入须以自身页面字段为准）。
- **⚠️ H5 场景下最容易漏的一处**：`CombineH5SceneInfo` 的 `device_id` / 在**本页标注必填**（其它下单页均选填）⇒ 从别的下单接口复制请求体会漏掉它。
- **拿到 `h5_url` 后**须按官方《H5 调起支付》指引跳转（含 `Referer` 等要求）， / **严禁**自行改写或拼装该链接。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/Combine`](../../../Mud.Wechat.Pay.DataModels/Combine/) 内共 **22** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **22** 个，`SerializerClassName` = `CombineTransactions`。

AOT 源生成上下文：[`CombineTransactionsJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/CombineTransactionsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CombinePrepayResponse` | 应答 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombinePrepayRequest` | 请求体 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineCloseOrderRequest` | 请求体 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineQueryResponse` | 应答 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineNativePrepayResponse` | 应答 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineNativePrepayRequest` | 请求体 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineAppPrepayRequest` | 请求体 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineH5PrepayResponse` | 应答 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |
| `CombineH5PrepayRequest` | 请求体 | [Combine/CombineDtos.cs](../../../Mud.Wechat.Pay.DataModels/Combine/CombineDtos.cs) | `combine_appid`, `combine_mchid`, `combine_out_trade_no`, `scene_info`, `sub_orders`, `combine_payer_info`, `time_expire`, `notify_url`, `device_id`, `payer_client_ip`, `mchid`, `attach` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Combine;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.CombineTransactions));

var service = app.Services.GetRequiredService<IWechatPayCombineService>();
var ct = CancellationToken.None;
var response = await service.CreateAppOrderAsync(new CombineAppPrepayRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/combine-transactions/app");
// 占位值仅用于编译示意；业务值须按 IWechatPayCombineService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateAppOrderAsync`）；请求体 `CombineAppPrepayRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayCombineService](./IWechatPayCombineService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayCombineContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCombineContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPayInstallmentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayInstallmentContractGuards.cs)、[`WechatPayRepaymentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRepaymentContractGuards.cs) |
| 守卫中的代表性测试方法 | `CombineEndpoints_ShouldMatchOfficialRoutes`、`CombineDtos_ShouldExposeOfficialFieldNames`、`CombineRefundEndpoint_ShouldStayUnmodeled`、`QueryShapes_ShouldStayStructurallySeparate`、`SubOrderShapes_ShouldStayStructurallySeparate`、`CombineDataModels_ShouldBeRegisteredInJsonContext`、`AppAndH5Prepay_ShouldSplitAndShareTypesPerOfficialTables`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/Combine`](../../../Mud.Wechat.Pay.DataModels/Combine/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayCombineContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPayInstallmentContractGuards.cs`、`WechatPayRepaymentContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

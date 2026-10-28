# WebizpayEmployee 域 · 微企付企业支付 · 额度卡与员工授权

微信支付「微企付企业支付 · 额度卡与员工授权」域 SDK（6 端点）。 接口：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)；注册组 `WebizpayEmployee`；模块枚举 `PayModule.WebizpayEmployee`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayWebizpayEmployeeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayEmployeeContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/WebizpayEmployee`](../../../Mud.Wechat.Pay/WebizpayEmployee/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayWebizpayEmployeeService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayWebizpayEmployeeService` | `ApplyQuotaCardAsync` | POST | `/v3/webizpay/employees/{employeeId}/quota-cards` | 商户证书 / APIv3 签名 | 企业商户为员工下发企业支付额度卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507148（【普通服务商】，官方页更新于 … |
| 2 | `IWechatPayWebizpayEmployeeService` | `QueryQuotaCardByCardNoAsync` | GET | `/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}` | 商户证书 / APIv3 签名 | 企业商户查询企业支付额度卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507119（【普通服务商】，官方页更新于 202… |
| 3 | `IWechatPayWebizpayEmployeeService` | `QueryQuotaCardByOutCardNoAsync` | GET | `/v3/webizpay/employees/{employeeId}/out-quota-cards/{outCardNo}` | 商户证书 / APIv3 签名 | 企业商户使用商户卡号查询企业支付额度卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015001625（【普通服务商】，官方页更… |
| 4 | `IWechatPayWebizpayEmployeeService` | `CancelQuotaCardAsync` | POST | `/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/cancel` | 商户证书 / APIv3 签名 | 企业商户作废员工企业支付额度卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507127（【普通服务商】，官方页更新于 2… |
| 5 | `IWechatPayWebizpayEmployeeService` | `GetQuotaCardPaymentUrlAsync` | GET | `/v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/payment-url` | 商户证书 / APIv3 签名 | 企业商户获取指定额度卡支付链接。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507160（【普通服务商】，官方页更新于 2… |
| 6 | `IWechatPayWebizpayEmployeeService` | `RevokeEmployeeAsync` | POST | `/v3/webizpay/employees/{employeeId}/revoke` | 商户证书 / APIv3 签名 | 企业商户发起解除员工企业支付授权。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507156（【普通服务商】，官方页更新于 … |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页** ⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**「普通服务商」身份口径**：本族 12 页「支持商户」栏**恒为【普通服务商】**（无【平台商户】/【普通商户】分身页），
请求一律以 `sp_mchid`（服务商商户号）+ `sub_mchid`（出资子商户号）双层定位 ⇒ 服务商身份由调用侧签名承载；
**唯一例外**是门店主体匹配两支只印 `sp_mchid`（守卫 WEB-ST）⇒ 不得跨域补字段。

**六支建五型**：按 `card_no` 与按 `out_card_no` 两支查询的官方应答表**逐字段同形且必填口径一致**（20 行） ⇒ 共用 `WebizpayQuotaCardQueryResponse`；下发 / 作废 / 支付链接 / 解除授权各建一型。

**定位键分工（本域最易踩）**：path 一律 `employee_id`（官方【微信授权关系 ID】，形如 `WeBizPay_a968402a-…`，由注册回调或员工查询面产出，**不是**商户企业员工 ID `user_id`）； 卡级操作只认**微信卡号 `card_no`** ⇒ 作废与支付链接两支**不接受商户卡号**， 只持有 `out_card_no` 的调用方须先走 `out-quota-cards` 查询支换取 `card_no`。

**同名 `sp_mchid` / `sub_mchid` 按动词落位不同**：GET 三支（两路查询 + 支付链接）落**必填 query**、零请求体， POST 三支落 **Body** ⇒ 请求 DTO 与查询签名不得互套；支付链接支另有必填 query `application_type` （`H5` / `MINIPROGRAM`）。

**解除授权支请求用 `employee_id`、应答回显 `user_id`**（商户企业员工 ID），且官方口径为**同步**： 应答 `authorization_state` 返回 `REVOKED` 即认定该员工在本服务商-出资子商户下授权已解除、 其在本商户下的企业支付额度卡**全部作废** ⇒ 不得把该支当异步受理。

**查询应答 20 行的 6 个选填字段**（`reason` / `card_usage_time` / `transaction_id` / `card_disabled_time` / `card_remark` / `avail_mch_range_id`）⇒ 判「卡可用」须同时看 `card_state`（仅 `ACTIVE` / `DISABLED` 两值）与 `effective_end_time`，**不得**只看 `remain_amount` > 0。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 6 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 1. `ApplyQuotaCardAsync` — 企业商户为员工下发企业支付额度卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507148（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`POST /v3/webizpay/employees/{employeeId}/quota-cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |

- **请求体**：`WebizpayQuotaCardApplyRequest` — 企业商户为员工下发企业支付额度卡请求体，字段见 `WebizpayQuotaCardApplyRequest`。
- **返回**：`WebizpayQuotaCardApplyResponse` — 企业商户为员工下发企业支付额度卡，见 `WebizpayQuotaCardApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/webizpay/employees/{employee_id}/quota-cards`；path `employee_id`。
- **应答**见 `WebizpayQuotaCardApplyResponse`（官方应答表逐字段建模）。

### 2. `QueryQuotaCardByCardNoAsync` — 企业商户查询企业支付额度卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507119（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`GET /v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |
| `cardNo` | Path | `string` | 【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号。通过创建卡和查询卡返回。 示例：1068019671702503425（官方 path `card_no`，**必填**，类型 `string(64)`）。 |
| `spMchid` | Query `sp_mchid` | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query `sp_mchid`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`WebizpayQuotaCardQueryResponse` — 企业商户查询企业支付额度卡，见 `WebizpayQuotaCardQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}`；path `employee_id` / `card_no`，query `sp_mchid` / `sub_mchid`。
- **`sp_mchid` 与 `sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `QueryQuotaCardByOutCardNoAsync` — 企业商户使用商户卡号查询企业支付额度卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015001625（【普通服务商】，官方页更新于 2025.05.29）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`GET /v3/webizpay/employees/{employeeId}/out-quota-cards/{outCardNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |
| `outCardNo` | Path | `string` | 【企业支付额度卡商户卡号】 由商户在创建额度卡时传入,由商户系统生成的唯一标识额度卡的编号（官方 path `out_card_no`，**必填**，类型 `string(64)`）。 |
| `spMchid` | Query `sp_mchid` | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query `sp_mchid`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`WebizpayQuotaCardQueryResponse` — 企业商户使用商户卡号查询企业支付额度卡，见 `WebizpayQuotaCardQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/webizpay/employees/{employee_id}/out-quota-cards/{out_card_no}`；path `employee_id` / `out_card_no`，query `sp_mchid` / `sub_mchid`。
- **`sp_mchid` 与 `sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 4. `CancelQuotaCardAsync` — 企业商户作废员工企业支付额度卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507127（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`POST /v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/cancel`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |
| `cardNo` | Path | `string` | 【企业支付额度卡卡号】 企业支付额度卡卡号，由请求微信支付发放额度卡时返回 示例：1068019671702503425（官方 path `card_no`，**必填**，类型 `string(64)`）。 |

- **请求体**：`WebizpayQuotaCardCancelRequest` — 企业商户作废员工企业支付额度卡请求体，字段见 `WebizpayQuotaCardCancelRequest`。
- **返回**：`WebizpayQuotaCardCancelResponse` — 企业商户作废员工企业支付额度卡，见 `WebizpayQuotaCardCancelResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}/cancel`；path `employee_id` / `card_no`。
- **应答**见 `WebizpayQuotaCardCancelResponse`（官方应答表逐字段建模）。

### 5. `GetQuotaCardPaymentUrlAsync` — 企业商户获取指定额度卡支付链接。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507160（【普通服务商】，官方页更新于 2025.05.29）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`GET /v3/webizpay/employees/{employeeId}/quota-cards/{cardNo}/payment-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |
| `cardNo` | Path | `string` | 【企业支付额度卡卡号】 企业支付额度卡卡号，微信支付平台生成的唯一标识额度卡的编号。通过创建卡和查询卡返回。 示例：1068019671702503425（官方 path `card_no`，**必填**，类型 `string(64)`）。 |
| `spMchid` | Query `sp_mchid` | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query `sp_mchid`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
| `applicationType` | Query `application_type` | `string` | 【拉起付款码页面的企业应用类型】 用户从企业应用跳转至微信付款码，需传入拉起付款码页面的企业应用类型。可选取值：`H5` 表示从商户的应用是外部H5，跳转微信拉起企业支付相关能力 / `MINIPROGRAM` 表示从商户的应用是微信小程序，跳转微信拉起企业支付相关能力（官方 query `application_type`，**必填**，类型 `string`）。 |
- **返回**：`WebizpayQuotaCardPaymentUrlResponse` — 企业商户获取指定额度卡支付链接，见 `WebizpayQuotaCardPaymentUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/webizpay/employees/{employee_id}/quota-cards/{card_no}/payment-url`；path `employee_id` / `card_no`，query `sp_mchid` / `sub_mchid` / `application_type`。
- **`sp_mchid` 与 `sub_mchid` 与 `application_type` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 6. `RevokeEmployeeAsync` — 企业商户发起解除员工企业支付授权。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507156（【普通服务商】，官方页更新于 2025.05.16）。

- **声明接口**：[IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs)
- **官方契约**：`POST /v3/webizpay/employees/{employeeId}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `employeeId` | Path | `string` | 【微信授权关系 ID】 微信授权关系ID，微信支付平台生成的唯一标识授权关系的ID，用户注册时会回调商户获取微信授权关系ID或者商户通过企业下唯一的员工ID查询企业支付员工接口获得。 示例：WeBizPay_a968402a-73bb-43e2-9e1a-8371b0ca3d7c（官方 path `employee_id`，**必填**，类型 `string(64)`）。 |

- **请求体**：`WebizpayEmployeeRevokeRequest` — 企业商户发起解除员工企业支付授权请求体，字段见 `WebizpayEmployeeRevokeRequest`。
- **返回**：`WebizpayEmployeeRevokeResponse` — 企业商户发起解除员工企业支付授权，见 `WebizpayEmployeeRevokeResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/webizpay/employees/{employee_id}/revoke`；path `employee_id`。
- **应答**见 `WebizpayEmployeeRevokeResponse`（官方应答表逐字段建模）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/WebizpayEmployee`](../../../Mud.Wechat.Pay/WebizpayEmployee/) 内共 **8** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **8** 个，`SerializerClassName` = `WebizpayEmployee`。

AOT 源生成上下文：[`WebizpayEmployeeJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/WebizpayEmployeeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WebizpayQuotaCardApplyResponse` | 应答 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayQuotaCardApplyRequest` | 请求体 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayQuotaCardQueryResponse` | 应答 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayQuotaCardCancelResponse` | 应答 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayQuotaCardCancelRequest` | 请求体 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayQuotaCardPaymentUrlResponse` | 应答 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayEmployeeRevokeResponse` | 应答 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |
| `WebizpayEmployeeRevokeRequest` | 请求体 | [WebizpayEmployee/WebizpayEmployeeDtos.cs](../../../Mud.Wechat.Pay/WebizpayEmployee/WebizpayEmployeeDtos.cs) | `sp_mchid`, `sub_mchid`, `card_template_id`, `amount`, `card_name`, `out_card_no`, `effective_end_time`, `card_remark`, `avail_mch_range_id`, `employee_id`, `card_no`, `effective_begin_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.WebizpayEmployee;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.WebizpayEmployee));

var service = app.Services.GetRequiredService<IWechatPayWebizpayEmployeeService>();
var ct = CancellationToken.None;
var response = await service.ApplyQuotaCardAsync("", new WebizpayQuotaCardApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/webizpay/employees/{employeeId}/quota-cards");
// 占位值仅用于编译示意；业务值须按 IWechatPayWebizpayEmployeeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyQuotaCardAsync`）；请求体 `WebizpayQuotaCardApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayWebizpayEmployeeService](./IWechatPayWebizpayEmployeeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayWebizpayEmployeeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayEmployeeContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `WebizpayEmployeeTopology_ShouldBeOneInterfaceOnOneGroup`、`WebizpayEmployeeEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`WebizpayEmployeeDtos_ShouldExposeOfficialFieldRosters`、`WebizpayEmployeeDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/WebizpayEmployee`](../../../Mud.Wechat.Pay/WebizpayEmployee/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayWebizpayEmployeeContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

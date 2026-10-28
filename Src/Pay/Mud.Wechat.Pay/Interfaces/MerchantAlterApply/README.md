# MerchantAlterApply 域 · 商户资料变更（主体 / 超级管理员 / 商家简称）

微信支付「商户资料变更（主体 / 超级管理员 / 商家简称）」域 SDK（**12 端点**：三族各「提交 + 两个查询 + 撤销」）。 接口：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)；注册组 `MerchantAlterApply`；模块枚举 `PayModule.MerchantAlterApply`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantAlterApplyContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantAlterApplyContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/MerchantAlterApply`](../../../Mud.Wechat.Pay/MerchantAlterApply/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantAlterApplyService = 12；按 HTTP 特性实计 **12** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantAlterApplyService` | `SubmitSubjectAlterApplyAsync` | POST | `/v3/mchalterapply/mchsubjectalterapplyment` | 商户证书 / APIv3 签名 | 「提交主体信息变更申请单」：一次提交营业执照 / 登记证书 / 金融机构许可证 / 法人 / 受益人 / 补充材料六棵子树，<br>受理只回 `apply_id`。<br>官方文档：https… |
| 2 | `IWechatPayMerchantAlterApplyService` | `QuerySubjectAlterApplyByApplyIdAsync` | GET | `/v3/mchalterapply/mchsubjectalterapplyment/{applyId}` | 商户证书 / APIv3 签名 | 「查询主体变更申请单状态（按申请单号）」：官方 PATH `apply_id` 是 string(32)。<br>官方文档：https://pay.weixin.qq.com/doc/v… |
| 3 | `IWechatPayMerchantAlterApplyService` | `QuerySubjectAlterApplyByOutRequestNoAsync` | GET | `/v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「查询主体变更申请单状态（按商户号 + 业务申请编号）」：与 `QuerySubjectAlterApplyByApplyIdAsync` 同一应答类型。<br>官方文档：https:/… |
| 4 | `IWechatPayMerchantAlterApplyService` | `RevokeSubjectAlterApplyAsync` | POST | `/v3/mchalterapply/mchsubjectalterapplyment/{applyId}/revoke` | 商户证书 / APIv3 签名 | 「撤销主体变更申请单」：成功应答是 **200 + `{result}`**（`SUCCESS` / `FAIL`），不是 204。<br>官方文档：https://pay.weixin… |
| 5 | `IWechatPayMerchantAlterApplyService` | `SubmitContactAlterApplyAsync` | POST | `/v3/mchalterapply/mchcontactalterapplyment` | 商户证书 / APIv3 签名 | 「提交超级管理员信息变更申请单」：只提交一个 `contact_info` 对象，姓名 / 证件号 / 手机号须密文。<br>官方文档：https://pay.weixin.qq.com… |
| 6 | `IWechatPayMerchantAlterApplyService` | `QueryContactAlterApplyByApplyIdAsync` | GET | `/v3/mchalterapply/mchcontactalterapplyment/{applyId}` | 商户证书 / APIv3 签名 | 「查询超级管理员变更申请单状态（按申请单号）」：官方 PATH `apply_id` 是 string(64)。<br>官方文档：https://pay.weixin.qq.com/do… |
| 7 | `IWechatPayMerchantAlterApplyService` | `QueryContactAlterApplyByOutRequestNoAsync` | GET | `/v3/mchalterapply/mchcontactalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「查询超级管理员变更申请单状态（按商户号 + 业务申请编号）」：与 `QueryContactAlterApplyByApplyIdAsync` 同一应答类型。<br>官方文档：http… |
| 8 | `IWechatPayMerchantAlterApplyService` | `RevokeContactAlterApplyAsync` | POST | `/v3/mchalterapply/mchcontactalterapplyment/{applyId}/revoke` | 商户证书 / APIv3 签名 | 「撤销超级管理员变更申请单」：应答是 **200 + `{apply_id, state}`**。<br>官方文档：https://pay.weixin.qq.com/doc/v3/pa… |
| 9 | `IWechatPayMerchantAlterApplyService` | `SubmitMerchantNameAlterApplyAsync` | POST | `/v3/mchalterapply/merchantnamealterapplyment` | 商户证书 / APIv3 签名 | 「提交商家简称变更申请单」：三字段全明文，本支**不要求** `Wechatpay-Serial`。<br>官方文档：https://pay.weixin.qq.com/doc/v3/p… |
| 10 | `IWechatPayMerchantAlterApplyService` | `QueryMerchantNameAlterApplyByApplyIdAsync` | GET | `/v3/mchalterapply/merchantnamealterapplyment/{applyId}` | 商户证书 / APIv3 签名 | 「查询商家简称变更申请单状态（按申请单号）」：应答**不含** `audit_reject_detail`，只有总体原因。<br>官方文档：https://pay.weixin.qq.c… |
| 11 | `IWechatPayMerchantAlterApplyService` | `QueryMerchantNameAlterApplyByOutRequestNoAsync` | GET | `/v3/mchalterapply/merchantnamealterapplyment/merchant/{subMchId}/out-request-no/{outRequestNo}` | 商户证书 / APIv3 签名 | 「查询商家简称变更申请单状态（按特约商户号 + 业务申请编号）」：路径段名同为 `merchant/`，但参数是 `sub_mchid`。<br>官方文档：https://pay.wei… |
| 12 | `IWechatPayMerchantAlterApplyService` | `RevokeMerchantNameAlterApplyAsync` | POST | `/v3/mchalterapply/merchantnamealterapplyment/{applyId}/revoke` | 商户证书 / APIv3 签名 | 「撤销商家简称变更申请单」：与 `RevokeContactAlterApplyAsync` 同一应答类型（`{apply_id, state}`）。<br>官方文档：https://p… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 十六页逐字段核验，全部落在 `/v3/mchalterapply/` 前缀下）：
主体族 https://pay.weixin.qq.com/doc/v3/partner/4014090649（提交）/
https://pay.weixin.qq.com/doc/v3/partner/4014090655（按申请单号查）/
https://pay.weixin.qq.com/doc/v3/partner/4014090667（按业务单号查）/
https://pay.weixin.qq.com/doc/v3/partner/4014090672（撤销）；
超级管理员族 https://pay.weixin.qq.com/doc/v3/partner/4015617906 /
https://pay.weixin.qq.com/doc/v3/partner/4015617911 /
https://pay.weixin.qq.com/doc/v3/partner/4015617915 /
https://pay.weixin.qq.com/doc/v3/partner/4015617919；
商家简称族 https://pay.weixin.qq.com/doc/v3/partner/4016166071 /
https://pay.weixin.qq.com/doc/v3/partner/4016166074 /
https://pay.weixin.qq.com/doc/v3/partner/4016166085 /
https://pay.weixin.qq.com/doc/v3/partner/4016166090。

**三族是三条独立路由族，不是「一套模板三次调用」**：`mchsubjectalterapplyment` /
`mchcontactalterapplyment` / `merchantnamealterapplyment`，且**定位键分族**——
主体与超管用 `merchant_code`（服务商进件的特约商户号），简称族用 `sub_mchid`
⇒ 「按业务单号查」一支的路径段名同为 `/merchant/{…}/out-request-no/{…}`，
但大括号里的参数**不同名**（守卫 MAA-B1 逐支锁路径与参数位置）。

**缺口清单的出处错配已在方案 §2.7 一-1 登记**：清单用主体族四个 docId 同时承载联系人族四行，
逐页核得两族各有独立页面 ⇒ **八行是八个页面**，联系人族现行路由是
`mchcontactalterapplyment`（不是转述的 `contactalter` 形态）。

**共用与分建的裁定（逐类型照页，不是按族一刀切）**：
三族「提交」应答表相同 ⇒ 共用 `MerchantAlterApplySubmitResponse`；
超管族与简称族「撤销」应答表相同 ⇒ 共用 `MerchantAlterApplyRevokeResponse`，
而主体族撤销回 `{result}`（`SUCCESS` / `FAIL`）⇒ 分建
`MerchantAlterApplySubjectRevokeResponse`；
三族「查询」应答**各自一型**——主体族与超管族字段名与长度逐字相同，但 `state` 值清单不同
（6 值含 `EDITTING` vs 5 值），简称族另有 `sub_mchid` 且**无** `audit_reject_detail`
⇒ 按「表不同则分建」（守卫 MAA-B6 三个数都断言）。

**提交 ≠ 生效**：三族都是**审核流**，受理只回 `apply_id`；
唯一闭环路径是轮询查询支读 `state` 到 `Finished`。
`Modifing` 档表示「系统改资料中」，此时旧资料仍生效；
`Editting`（官方三 T 误拼）是「提交发生错误」，
官方处置建议是**用同一 `out_request_no` 重提**而不是换新号（换号即第二笔申请单）。

**敏感字段两支有加密头表行**：主体族提交（`legal_person_info` / `ubo_info_list[]` 的
姓名/证件号/住址）与超管族提交（姓名/证件号/手机号）把 `Wechatpay-Serial` 列为**请求头参数表行**（必填），
简称族提交则**无该表行且全明文** ⇒ 三支请求 DTO 的加密要求不同，不得按「变更族都要加密」处理
（守卫 MAA-B5 锁定该分布）。密文仍是 string，DTO 只承载密文，加密由宿主与传输层协同。

**官方拼写与长度陷阱照页**：`APPLYMENT_STATE_EDITTING`（三 T）与
`APPLYMENT_STATE_MODIFING`（单 F）；`out_request_no` 在提交侧是 string(64)（主体）/
string(124)（超管、简称），在**查询应答**侧一律 string(64)——同域两页长度不一致，官方原文如此；
`apply_id` 在「按申请单号查」的 PATH 是 string(32)（主体族）/ string(64)（另两族），
在应答侧一律 string(64) ⇒ 本 SDK 的 path 参数按线内惯例统一声明为 `string`，守卫锁的是位置不是 CLR 宽度。

**规模与格式限制**（官方散文口径，SDK 不校验）：主体族六个资料对象**全标选填**但条件必填
（按 `alter_scope` 与主体类型分派，条件组合八种）；`ubo_info_list` 最多 4 人（法人已是受益人时 3 人）；
补充材料五组各最多 5 张；`company_address` 4-512 字符；日期 `yyyy-MM-dd` 且可填关键字「长期」；
一个商户同一时间只允许一笔流程中的申请单。

**三族十二页均无端点级频率数字**（两轮复核一致），只有 429 `FREQUENCY_LIMITED` 公共错误码
⇒ 注释不凭第三方 SDK 补数。**业务错误码**：400 `PARAM_ERROR` / 400 `INVALID_REQUEST` /
401 `SIGN_ERROR` / 500 `SYSTEM_ERROR`，原样透传至
`Code`。

**本章不含「特约商户进件」与「主体进件」**：`/v3/applyment4sub/applyment`
落 `IWechatPayApply4SubMerchantService`、`/v3/applyment4subject/applyment` 落
`IWechatPayApply4SubjectService`。三族看着同类，逐字比对后只有
`finance_institution_info` 一个嵌套对象同名（`licence` vs `license` 拼写分家、
有效期单字段 `*_valid_date` vs 双字段 `*_period_begin`/`*_period_end`、
法人证件 `identification_*` vs `card_*`、UBO 列表字段名全不同）⇒ **跨域零复用**。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，
报文里的 `merchant_code` / `sub_mchid` 是**参数**而非作用域；本域**零 `[Token]`**（守卫 PAY-B1）。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SubmitSubjectAlterApplyAsync` — 「提交主体信息变更申请单」：一次提交营业执照 / 登记证书 / 金融机构许可证 / 法人 / 受益人 / 补充材料六棵子树，
受理只回 `apply_id`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014090649。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/mchsubjectalterapplyment`

- **请求体**：`MerchantAlterApplySubjectSubmitRequest` — 请求体，字段见 `MerchantAlterApplySubjectSubmitRequest`。
- **返回**：`MerchantAlterApplySubmitResponse` — 微信支付申请单号，见 `MerchantAlterApplySubmitResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/mchsubjectalterapplyment`； / 支持商户【普通服务商】【平台商户】；无 path / query 参数。
- **敏感字段须加密**：法人 / 受益人的姓名、证件号、住址须以微信支付公钥（推荐）或平台证书加密， / 该页把 `Wechatpay-Serial` 列为**请求头表行必填**。
- **受理 ≠ 通过**：须轮询 `QuerySubjectAlterApplyByApplyIdAsync`； / 被驳回后用**同一** `out_request_no` 重提是覆盖原申请单，换新号会产生第二笔单。

### 2. `QuerySubjectAlterApplyByApplyIdAsync` — 「查询主体变更申请单状态（按申请单号）」：官方 PATH `apply_id` 是 string(32)。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014090655。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/mchsubjectalterapplyment/{applyId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(32)）。 |
- **返回**：`MerchantAlterApplySubjectQueryResponse` — 状态与驳回原因，见 `MerchantAlterApplySubjectQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/mchalterapply/mchsubjectalterapplyment/{apply_id}`；无包体。
- **与「按业务单号查」两支应答表逐字段相同** ⇒ 共用应答类型；`state` 本族 6 值。
- **驳回详情分两层**：`audit_reject_reason` 是总体原因、`audit_reject_detail` / 是按报文路径逐项列出的明细（`param_name` 形如 `business_license_info.merchant_name`）， / 官方只在 `REJECTED` 状态返回二者 ⇒ 只读总体原因会漏掉可定位的字段名。

### 3. `QuerySubjectAlterApplyByOutRequestNoAsync` — 「查询主体变更申请单状态（按商户号 + 业务申请编号）」：与 `QuerySubjectAlterApplyByApplyIdAsync` 同一应答类型。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014090667。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantCode` | Path | `string` | 商户号（官方 path `merchant_code`，必填 string(32)）：服务商负责进件的特约商户号。 |
| `outRequestNo` | Path | `string` | 业务申请编号（官方 path `out_request_no`，必填 string(64)）：商家自定义，官方要求 UrlEncode。 |
- **返回**：`MerchantAlterApplySubjectQueryResponse` — 与按申请单号查**同一应答类型**，见 `MerchantAlterApplySubjectQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/mchalterapply/mchsubjectalterapplyment/merchant/{merchant_code}/out-request-no/{out_request_no}`。
- **本支存在的意义是「申请单号丢了也能查」**：受理应答若未落库，商家侧只剩自定义的 / `out_request_no` ⇒ 两支都要接，缺一支就没有恢复路径。
- **官方要求路径参数做 UrlEncode**（同域各 GET 支一致）；本 SDK 的路径填充沿用传输层既有编码口径。

### 4. `RevokeSubjectAlterApplyAsync` — 「撤销主体变更申请单」：成功应答是 **200 + `{result}`**（`SUCCESS` / `FAIL`），不是 204。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014090672。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/mchsubjectalterapplyment/{applyId}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplySubjectRevokeResponse` — 撤销结果字符串，见 `MerchantAlterApplySubjectRevokeResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/mchsubjectalterapplyment/{apply_id}/revoke`；无包体。
- **只有编辑中 / 审核中 / 已驳回三档可撤销**（官方正文口径），已完成或已作废的单不可撤销。
- **撤销失败也是 HTTP 200**：`result = FAIL` 是业务结论而非传输错误 ⇒ / 判 `IsSuccess` 通过后再判 `result`，只看 HTTP 码会把失败当成功（守卫 MAA-B1）。
- **本族应答与另两族分建**：超管族与简称族的撤销回 `{apply_id, state}` / （见 `RevokeContactAlterApplyAsync`）。

### 5. `SubmitContactAlterApplyAsync` — 「提交超级管理员信息变更申请单」：只提交一个 `contact_info` 对象，姓名 / 证件号 / 手机号须密文。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015617906。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/mchcontactalterapplyment`

- **请求体**：`MerchantAlterApplyContactSubmitRequest` — 请求体，字段见 `MerchantAlterApplyContactSubmitRequest`。
- **返回**：`MerchantAlterApplySubmitResponse` — 微信支付申请单号，见 `MerchantAlterApplySubmitResponse`（与主体族共用）。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/mchcontactalterapplyment`； / 支持商户【普通服务商】；无 path / query 参数。
- **`Wechatpay-Serial` 是请求头表行必填**：`contact_name`、 / `contact_id_card_number`、`mobile_phone` 三个字段官方逐字段注明「需进行加密处理」。
- **`out_request_no` 官方长度 string(124)**（主体族是 64）⇒ 两族请求类型不共用。

### 6. `QueryContactAlterApplyByApplyIdAsync` — 「查询超级管理员变更申请单状态（按申请单号）」：官方 PATH `apply_id` 是 string(64)。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015617911。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/mchcontactalterapplyment/{applyId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyContactQueryResponse` — 状态与驳回原因，见 `MerchantAlterApplyContactQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/mchalterapply/mchcontactalterapplyment/{apply_id}`；无包体。
- **与主体族查询应答分建**：字段名与长度两页相同，唯一差异是 `state` 值清单 / （本族 5 值、**无** `APPLYMENT_STATE_EDITTING`）⇒ 值集属字段表的一部分，按「表不同则分建」。

### 7. `QueryContactAlterApplyByOutRequestNoAsync` — 「查询超级管理员变更申请单状态（按商户号 + 业务申请编号）」：与 `QueryContactAlterApplyByApplyIdAsync` 同一应答类型。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015617915。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/mchcontactalterapplyment/merchant/{merchantCode}/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `merchantCode` | Path | `string` | 商户号（官方 path `merchant_code`，必填 string(32)）。 |
| `outRequestNo` | Path | `string` | 业务申请编号（官方 path `out_request_no`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyContactQueryResponse` — 与按申请单号查**同一应答类型**，见 `MerchantAlterApplyContactQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/mchalterapply/mchcontactalterapplyment/merchant/{merchant_code}/out-request-no/{out_request_no}`。
- **路径形态与主体族逐字符同构**（只有大括号参数名与族前缀不同）⇒ 守卫 MAA-B1 逐族锁完整路径， / 不接受「一条模板生成三族」的写法。

### 8. `RevokeContactAlterApplyAsync` — 「撤销超级管理员变更申请单」：应答是 **200 + `{apply_id, state}`**。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015617919。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/mchcontactalterapplyment/{applyId}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyRevokeResponse` — 申请单号与撤销后状态，见 `MerchantAlterApplyRevokeResponse`（与简称族共用）。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/mchcontactalterapplyment/{apply_id}/revoke`；无包体。
- **应答两字段官方均标选填 string**（无长度、无值域表行）⇒ 本 SDK 按 string 承载， / 状态判读以查询支为准。

### 9. `SubmitMerchantNameAlterApplyAsync` — 「提交商家简称变更申请单」：三字段全明文，本支**不要求** `Wechatpay-Serial`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016166071。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/merchantnamealterapplyment`

- **请求体**：`MerchantAlterApplyMerchantNameSubmitRequest` — 请求体，字段见 `MerchantAlterApplyMerchantNameSubmitRequest`。
- **返回**：`MerchantAlterApplySubmitResponse` — 微信支付申请单号，见 `MerchantAlterApplySubmitResponse`（三族共用）。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/merchantnamealterapplyment`； / 支持商户【普通服务商】【平台商户】。
- **定位键是 `sub_mchid`（不是 `merchant_code`）**：本族全用特约商户号， / 照抄另两族的字段名会直接 400 `PARAM_ERROR`。
- **无加密头表行**：本族页面无任何「须加密」字段 ⇒ 不得给简称支配 `Wechatpay-Serial`。

### 10. `QueryMerchantNameAlterApplyByApplyIdAsync` — 「查询商家简称变更申请单状态（按申请单号）」：应答**不含** `audit_reject_detail`，只有总体原因。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016166074。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/merchantnamealterapplyment/{applyId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyMerchantNameQueryResponse` — 五字段状态应答，见 `MerchantAlterApplyMerchantNameQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/mchalterapply/merchantnamealterapplyment/{apply_id}`；无包体。
- **驳回原因只有一层**：本族应答表没有 `audit_reject_detail` 行 ⇒ 复用主体族应答类型 / 会多出一个永不为真的字段（守卫 MAA-B6 断言本域三族查询应答是三型）。

### 11. `QueryMerchantNameAlterApplyByOutRequestNoAsync` — 「查询商家简称变更申请单状态（按特约商户号 + 业务申请编号）」：路径段名同为 `merchant/`，但参数是 `sub_mchid`。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016166085。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`GET /v3/mchalterapply/merchantnamealterapplyment/merchant/{subMchId}/out-request-no/{outRequestNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 特约商户号（官方 path `sub_mchid`，必填 string(32)）。 |
| `outRequestNo` | Path | `string` | 业务申请编号（官方 path `out_request_no`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyMerchantNameQueryResponse` — 与按申请单号查**同一应答类型**，见 `MerchantAlterApplyMerchantNameQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** / `/v3/mchalterapply/merchantnamealterapplyment/merchant/{sub_mchid}/out-request-no/{out_request_no}`。
- **字面段 `merchant` 里装的是特约商户号**：官方路径模板与本族参数表不同名， / 属文档形态而非契约 ⇒ 方法参数按**参数表**命名为 `subMchId`（守卫 MAA-B1 锁路径字面量与参数顺序）。

### 12. `RevokeMerchantNameAlterApplyAsync` — 「撤销商家简称变更申请单」：与 `RevokeContactAlterApplyAsync` 同一应答类型（`{apply_id, state}`）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016166090。

- **声明接口**：[IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs)
- **官方契约**：`POST /v3/mchalterapply/merchantnamealterapplyment/{applyId}/revoke`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `applyId` | Path | `string` | 微信支付申请单号（官方 path `apply_id`，必填 string(64)）。 |
- **返回**：`MerchantAlterApplyRevokeResponse` — 申请单号与撤销后状态，见 `MerchantAlterApplyRevokeResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/mchalterapply/merchantnamealterapplyment/{apply_id}/revoke`；无包体。
- **该页 `state` 描述只印 `APPLYMENT_STATE_CANCELED` 一值**（且描述首句误写成 / 「查询申请单状态」）⇒ 视为文档特例，不据此为本族另建窄枚举。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/MerchantAlterApply`](../../../Mud.Wechat.Pay/MerchantAlterApply/) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `MerchantAlterApply`。

AOT 源生成上下文：[`MerchantAlterApplyJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/MerchantAlterApplyJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantAlterApplySubmitResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplyCommonDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyCommonDtos.cs) | `apply_id`, `state` |
| `MerchantAlterApplySubjectSubmitRequest` | 请求体 | [MerchantAlterApply/MerchantAlterApplySubjectDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplySubjectDtos.cs) | `merchant_code`, `out_request_no`, `alter_scope`, `organization_type`, `finance_institution`, `business_license_info`, `certificate_info`, `finance_institution_info`, `legal_person_info`, `ubo_info_list`, `addition`, `license_number` … |
| `MerchantAlterApplySubjectQueryResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplySubjectDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplySubjectDtos.cs) | `merchant_code`, `out_request_no`, `alter_scope`, `organization_type`, `finance_institution`, `business_license_info`, `certificate_info`, `finance_institution_info`, `legal_person_info`, `ubo_info_list`, `addition`, `license_number` … |
| `MerchantAlterApplySubjectRevokeResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplySubjectDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplySubjectDtos.cs) | `merchant_code`, `out_request_no`, `alter_scope`, `organization_type`, `finance_institution`, `business_license_info`, `certificate_info`, `finance_institution_info`, `legal_person_info`, `ubo_info_list`, `addition`, `license_number` … |
| `MerchantAlterApplyContactSubmitRequest` | 请求体 | [MerchantAlterApply/MerchantAlterApplyContactDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyContactDtos.cs) | `merchant_code`, `out_request_no`, `contact_info`, `contact_name`, `contact_id_doc_type`, `contact_id_card_number`, `contact_id_doc_copy`, `contact_id_doc_copy_back`, `contact_id_doc_period_begin`, `contact_id_doc_period_end`, `mobile_phone`, `apply_id` … |
| `MerchantAlterApplyContactQueryResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplyContactDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyContactDtos.cs) | `merchant_code`, `out_request_no`, `contact_info`, `contact_name`, `contact_id_doc_type`, `contact_id_card_number`, `contact_id_doc_copy`, `contact_id_doc_copy_back`, `contact_id_doc_period_begin`, `contact_id_doc_period_end`, `mobile_phone`, `apply_id` … |
| `MerchantAlterApplyRevokeResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplyCommonDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyCommonDtos.cs) | `apply_id`, `state` |
| `MerchantAlterApplyMerchantNameSubmitRequest` | 请求体 | [MerchantAlterApply/MerchantAlterApplyMerchantNameDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyMerchantNameDtos.cs) | `sub_mchid`, `out_request_no`, `merchant_shortname`, `apply_id`, `state`, `audit_reject_reason` |
| `MerchantAlterApplyMerchantNameQueryResponse` | 应答 | [MerchantAlterApply/MerchantAlterApplyMerchantNameDtos.cs](../../../Mud.Wechat.Pay/MerchantAlterApply/MerchantAlterApplyMerchantNameDtos.cs) | `sub_mchid`, `out_request_no`, `merchant_shortname`, `apply_id`, `state`, `audit_reject_reason` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MerchantAlterApply;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantAlterApply));

var service = app.Services.GetRequiredService<IWechatPayMerchantAlterApplyService>();
var ct = CancellationToken.None;
var response = await service.QueryContactAlterApplyByApplyIdAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/mchalterapply/mchcontactalterapplyment/{applyId}");
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantAlterApplyService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryContactAlterApplyByApplyIdAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantAlterApplyService](./IWechatPayMerchantAlterApplyService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantAlterApplyContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantAlterApplyContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantAlterApplyEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentNowhere`、`MerchantAlterApplyRoutes_ShouldStayOnOwnPrefixAndPairEachMethodWithItsOwnDocId`、`MerchantAlterApplyDtos_ShouldExposeOfficialFieldNamesAndPerFamilyKeys`、`MerchantAlterApplyValueSets_ShouldMatchOfficialBulletListsPerFamily`、`MerchantAlterApplyUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndPerFamilyEncrypted`、`MerchantAlterApplyResponses_ShouldDeriveFromBaseAndFollowPerTypeSharingRulings`、`MerchantAlterApplyDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/MerchantAlterApply`](../../../Mud.Wechat.Pay/MerchantAlterApply/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantAlterApplyContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

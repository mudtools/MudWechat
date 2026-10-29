# Fapiao 域 · 电子发票

微信支付「电子发票」域 SDK（APIv3 区块链电子发票，**18 端点**：开具 + 查询 + 冲红 +
获取发票下载信息 + 插入用户卡包（直连商户面 5 支），
以及 P1-5-B 补入的**服务商 / 数电发票面 13 支**（卡券模板、通用行业与不动产租赁开具、
能力邀请、商户开票信息与开发选项、税收分类编码、用户抬头）。 接口：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)；注册组 `NewTaxControlFapiao`；模块枚举 `PayModule.NewTaxControlFapiao`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayFapiaoContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFapiaoContractGuards.cs)、[WechatPayFapiaoExtensionContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFapiaoExtensionContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/Fapiao`](../../../Mud.Wechat.Pay/DataModels/Fapiao/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayFapiaoService = 18；按 HTTP 特性实计 **18** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayFapiaoService` | `IssueFapiaoAsync` | POST | `/v3/new-tax-control-fapiao/fapiao-applications` | 商户证书 / APIv3 签名 | 开具电子发票。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538301。 |
| 2 | `IWechatPayFapiaoService` | `QueryFapiaoApplicationsAsync` | GET | `/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}` | 商户证书 / APIv3 签名 | 查询电子发票。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fa… |
| 3 | `IWechatPayFapiaoService` | `ReverseFapiaoAsync` | POST | `/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse` | 商户证书 / APIv3 签名 | 冲红电子发票。官方文档：<br>https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/rever… |
| 4 | `IWechatPayFapiaoService` | `GetFapiaoFilesAsync` | GET | `/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files` | 商户证书 / APIv3 签名 | 获取发票下载信息。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538335。 |
| 5 | `IWechatPayFapiaoService` | `InsertFapiaoCardsAsync` | POST | `/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards` | 商户证书 / APIv3 签名 | 将电子发票插入微信用户卡包。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538365。 |
| 6 | `IWechatPayFapiaoService` | `SetFapiaoCardTemplateAsync` | POST | `/v3/new-tax-control-fapiao/card-template` | 商户证书 / APIv3 签名 | 创建电子发票卡券模板。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529382（【普通商户】）、https://pay.… |
| 7 | `IWechatPayFapiaoService` | `IssueGeneralFapiaoAsync` | POST | `/v3/new-tax-control-fapiao/fapiao-applications/issue-general` | 商户证书 / APIv3 签名 | 开具通用行业电子发票。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015792574（【普通服务商】）。 |
| 8 | `IWechatPayFapiaoService` | `IssueRealEstateLeasingFapiaoAsync` | POST | `/v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing` | 商户证书 / APIv3 签名 | 开具不动产租赁行业电子发票。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941740（【普通服务商】）。 |
| 9 | `IWechatPayFapiaoService` | `GetFapiaoSpInviteUrlAsync` | GET | `/v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl` | 商户证书 / APIv3 签名 | 获取服务商电子发票能力邀请链接。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941495（【普通服务商】）。 |
| 10 | `IWechatPayFapiaoService` | `ListFapiaoSpInviteMerchantInfoAsync` | GET | `/v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo` | 商户证书 / APIv3 签名 | 获取邀请开通的商户信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941524（【普通服务商】）。 |
| 11 | `IWechatPayFapiaoService` | `CheckFapiaoMerchantAsync` | POST | `/v3/new-tax-control-fapiao/merchant/{subMchid}/check` | 商户证书 / APIv3 签名 | 检查子商户电子发票功能状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012474022（【普通服务商】）。 |
| 12 | `IWechatPayFapiaoService` | `QueryFapiaoMerchantCheckStatusAsync` | GET | `/v3/new-tax-control-fapiao/merchant/{subMchid}/check-status` | 商户证书 / APIv3 签名 | 查询受邀商户开票能力开通状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015792561（【普通服务商】）。 |
| 13 | `IWechatPayFapiaoService` | `QueryFapiaoMerchantBaseInformationAsync` | GET | `/v3/new-tax-control-fapiao/merchant/base-information` | 商户证书 / APIv3 签名 | 查询商户开票基础信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538180（【普通商户】）、https://pay.… |
| 14 | `IWechatPayFapiaoService` | `QueryFapiaoMerchantDevelopmentConfigAsync` | GET | `/v3/new-tax-control-fapiao/merchant/development-config` | 商户证书 / APIv3 签名 | 查询商户开发选项。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529526（【普通商户】）、https://pay.we… |
| 15 | `IWechatPayFapiaoService` | `UpdateFapiaoMerchantDevelopmentConfigAsync` | PATCH | `/v3/new-tax-control-fapiao/merchant/development-config` | 商户证书 / APIv3 签名 | 配置商户开发选项。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529457（【普通商户】）、https://pay.we… |
| 16 | `IWechatPayFapiaoService` | `QueryFapiaoTaxCodesAsync` | GET | `/v3/new-tax-control-fapiao/merchant/tax-codes` | 商户证书 / APIv3 签名 | 查询商户税收分类编码对照表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538236（【普通商户】）、https://p… |
| 17 | `IWechatPayFapiaoService` | `QueryFapiaoUserTitleAsync` | GET | `/v3/new-tax-control-fapiao/user-title` | 商户证书 / APIv3 签名 | 获取用户填写抬头信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538112（【普通商户】）、https://pay.… |
| 18 | `IWechatPayFapiaoService` | `GetFapiaoUserTitleUrlAsync` | GET | `/v3/new-tax-control-fapiao/user-title/title-url` | 商户证书 / APIv3 签名 | 获取用户抬头填写链接。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538106（【普通商户】）、https://pay.… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（**普通商户**文档中心，2026-10-09 逐页核验，更新时间均为 2025.09.26）：
开具 https://pay.weixin.qq.com/doc/v3/merchant/4012538301、
查询 https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html、
冲红 https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html、
获取下载信息 https://pay.weixin.qq.com/doc/v3/merchant/4012538335、
插入卡包 https://pay.weixin.qq.com/doc/v3/merchant/4012538365。
各页均标注**支持商户：【普通商户】**。

**✅ 更正设计方案 §2.5 的一条记录（并二次更正其「不覆盖」裁定）**：该处记「电子发票 · 普通商户文档中心
**无独立入口**；服务商侧 `partner/4015792574`」—— **实测不成立**：普通商户侧有完整电子发票文档
（本域各页均在其内）。服务商侧另有一套「数电发票 / 服务商电子发票」开放面
（开具**通用行业**电子发票 `/fapiao-applications/**issue-general**`、能力邀请、受邀商户查询等，
与商户侧路由不同）—— **P1-5-B 已按官方逐页取证纳入本接口**（13 支，守卫
`WechatPayFapiaoExtensionContractGuards` FPX-B1~B6），原「属另一套面，本域不覆盖」的裁定已作废。

**服务商面与商户面的三处形态差异**（取证结论，逐支写在方法 remarks 里）：
① 同一支路由官方常印**商户页 + 服务商页**两页，字段名册取**并集**建模、必填口径不一致只在注释标注，
**一面一方法**（不因身份拆建）；② 服务商面的子商户定位键**不统一**——
多数支用 query/path `sub_mchid`，但「查询/配置开发选项」两支用 `sub_mch_code`，
照页不套，不得凭同名语义互换；③ 服务商面新支的应答节多为**「无应答包体」**（含两支开具与一支校验）
⇒ 返回裸 `Task`，业务结果由异步通知或查询支取得。

**两支开具接口只是「受理」**：`issue-general` 与 `real-estate-leasing` 的官方应答节原文均为
**「无应答包体」**（正常示例状态码 **202**）⇒ 返回裸 `Task`；
开票结果须经【查询电子发票】或开票回调取得，**不得**据 HTTP 2xx 判定开票成功。
「检查子商户电子发票功能状态」同款（原文「无应答包体」，正常示例 **204**）。

**⚠️ 两个非 JSON 端点刻意不在本生成式接口内**（它们不是「漏实现」）：

-
**上传电子发票文件**（`POST /v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file`，已核）：
官方契约为 `**multipart/form-data**`（表单字段 `file` + `meta`），
且 `meta.digest` 要求 **SM3** 摘要（官方字段名写作 `digest_alogrithm`，**少一个 r，属官方拼写**）——
**SM3 不在 .NET BCL 内**，须由调用方计算后传入 ⇒ 声明式 `[Body]` JSON 端点无法承载。
-
**下载发票文件**：官方注明该下载**不支持签名和验签**，且 URL 由「获取下载信息」动态给出
（官方《下载发票文件》页称有效期 **30s**）⇒ 与账单下载同属「动态 URL + 非 JSON」形态。

当前落点：`GetFapiaoFilesAsync` 只**返回** `download_url`，宿主可自行取用；
待后续按账单下载通道（`IWechatPayBillDownloadService`）的同款模式单独建模
（含主机白名单前置闸）。**不要凭推断补路由**。

**通知已覆盖（归回调包）**：`FAPIAO.USER_APPLIED`（抬头填写完成）、
`FAPIAO.ISSUED`（开具成功）、`FAPIAO.CARD_INSERTED`（插卡成功）、
`FAPIAO.REVERSED`（冲红成功）、`FAPIAO.CARD_DISCARDED`（卡券作废）——
见 `WechatPayCallbackContext.GetFapiao` / `GetFapiaoUserApplied`。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 7 次）**支持商户**【普通商户】【普通服务商】。
- （出现 13 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**双页并集**：本支路由官方印 3 页（商户页、服务商页、服务商页），Path/Query 与应答均按**并集**建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。
- （出现 6 次）**支持商户**【普通服务商】。
- （出现 3 次）**业务错误码**：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。
- （出现 3 次）**双页并集**：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按**并集**建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。
- （出现 3 次）**业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。

### 1. `IssueFapiaoAsync` — 开具电子发票。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538301。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/fapiao-applications`

- **请求体**：`FapiaoIssueRequest` — 开票请求体，见 `FapiaoIssueRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/fapiao-applications`；无 path / query 参数。
- **返回类型为何是 `Task`**：官方标注应答状态 **202 Accepted** 且**无任何应答字段** / ⇒ 与 204 类端点同款处置（声明为无返回值，避免调用方去解一个不存在的包体）。
- **异步语义（官方原文）**：成功返回**仅代表开票请求已被受理**； / 开票完成须经**回调通知**或 `QueryFapiaoApplicationsAsync` 获取 —— / **不得**把 202 当成开票成功。

### 2. `QueryFapiaoApplicationsAsync` — 查询电子发票。官方文档：https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Path | `string` | 发票申请单号（官方 path `fapiao_apply_id`，必填 string(32)）。 |
| `fapiaoId` | Query `fapiao_id` | `string?` | 商户发票单号（官方 query `fapiao_id`，**选填** string(32)）：用于在申请单内过滤单张发票。 |
- **返回**：`FapiaoQueryResponse` — 发票列表与开票结果，见 `FapiaoQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}?fapiao_id=…`； / path 必带 `fapiao_apply_id`，query 的 `fapiao_id` **选填**。
- **官方建议**：「【将电子发票插入微信用户卡包】接口成功后，应调用本接口查询电子发票开票结果」。
- **✅ 值表可交叉引用（原「值域未核验」留档已关闭）**：本应答的 `status` 与 / `card_information.card_status` 取值见常量 `FapiaoStatuses` / `FapiaoCardStatuses` / —— 二者由官方**四张通知页**（开具成功 / 插卡成功 / 冲红成功 / 卡券作废，字段表逐项一致）逐字取得。
- **⚠️ 保留一处边界（不夸大核验范围）**：本轮**未**在「查询电子发票」页本身找到单独的取值表 / （该页只给出字段名）⇒ 常量与该页字段的对应是「**同名字段、同域取值**」的合理映射， / 而非该页逐字确认。若联调发现查询页返回常量之外的取值，须先核验官方再补，**勿臆造**。

### 3. `ReverseFapiaoAsync` — 冲红电子发票。官方文档：
https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Path | `string` | 发票申请单号（官方 path `fapiao_apply_id`，必填 string(32)）。 |

- **请求体**：`FapiaoReverseRequest` — 冲红请求体，见 `FapiaoReverseRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/reverse`； / 更新时间 2025.09.26，**支持商户：【普通商户】**。
- **无应答包体**：官方标注 **202 Accepted** 且无任何应答字段 ⇒ 返回 `Task`。 / 官方原文：「本接口成功返回**仅代表冲红请求已被受理**，当冲红完成时，微信支付会根据商户配置的 / 回调地址进行回调通知，商户也可以通过【查询电子发票】接口获取冲红结果及红票信息」。
- **重试约束（官方原文）**：红字发票开具**失败**时，可用**相同的** / `fapiao_information.fapiao_id` 重新填写冲红原因**重试**，**每次重试间隔为 5 分钟**。
- **前置约束**：**仅**在微信支付侧开具的电子发票才允许冲红；冲红会同时把发票从用户卡包中删除。

### 4. `GetFapiaoFilesAsync` — 获取发票下载信息。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538335。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Path | `string` | 发票申请单号（官方 path `fapiao_apply_id`，必填 string(32)）。 |
- **返回**：`FapiaoFilesResponse` — 发票下载信息列表（含 `download_url`），见 `FapiaoFilesResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/fapiao-files`； / 仅 path 参数，无 body / query（更新时间 2025.09.26）。
- **⚠️ 有下载链接才算真能下载**：官方原文「开票状态为 `ISSUED` 的发票才能获取到发票文件下载链接」 / ⇒ `download_url` **仅当**该张发票 `status = ISSUED` 时存在（其它状态下为 `null`）， / 消费侧**不得**假定列表里每项都有链接。
- **本端点只返回地址，不下载**：文件须再用官方《下载发票文件》按其 `download_url` 取回 / （该下载**不支持签名和验签**，与普通 APIv3 请求不同）。本域暂未建模下载动作， / 故 `download_url` 须由宿主自行取用（**严禁**改写或拼接该 URL）。

### 5. `InsertFapiaoCardsAsync` — 将电子发票插入微信用户卡包。官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538365。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Path | `string` | 发票申请单号（官方 path `fapiao_apply_id`，必填 string(32)）。 |

- **请求体**：`FapiaoInsertCardsRequest` — 插卡请求体，见 `FapiaoInsertCardsRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/insert-cards`； / 更新时间 2025.09.26，**支持商户：【普通商户】**。
- **无应答包体**：官方标注 **202 Accepted** ⇒ 返回 `Task`；官方原文「返回成功仅代表请求被受理， / 插卡完成后由微信支付回调通知，也可通过【查询电子发票】接口获取插卡结果及卡券信息」。
- **两段式前置（官方原文）**：调用本接口前**必须**先调《上传电子发票文件》拿到 / `fapiao_media_id`（**三天内有效**）—— 该上传接口是 `multipart/form-data`， / **不是** JSON 端点，故不在本生成式接口内。
- **非微信支付场景**：须先经《获取用户授权链接》取得用户授权后才能调用本接口； / 微信支付场景无需额外授权。

### 6. `SetFapiaoCardTemplateAsync` — 创建电子发票卡券模板。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529382（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474026（【普通服务商】）、https://pay.weixin.qq.com/doc/v3/partner/4015792562（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/card-template`

- **请求体**：`FapiaoCardTemplateRequest` — 创建电子发票卡券模板请求体，字段见 `FapiaoCardTemplateRequest`。
- **返回**：`FapiaoCardTemplateResponse` — 创建电子发票卡券模板，见 `FapiaoCardTemplateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/card-template`；无 path / query 参数。
- **应答**见 `FapiaoCardTemplateResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH：AppID和mch_id不匹配 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 APPID_MCHID_NOT_MATCH：appid与mchid不匹配。

### 7. `IssueGeneralFapiaoAsync` — 开具通用行业电子发票。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015792574（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/fapiao-applications/issue-general`

- **请求体**：`FapiaoGeneralIssueRequest` — 开具通用行业电子发票请求体，字段见 `FapiaoGeneralIssueRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/fapiao-applications/issue-general`；无 path / query 参数。
- **业务错误码**：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 RESOURCE_ALREADY_EXISTS：发票申请单已存在 / 400 INVALID_REQUEST：当前发票开具中，不可申请重开发票 / 400 INVALID_REQUEST：当前发票冲红中，不可申请重开发票 / 400 INVALID_REQUEST：申请开票次数，超过最大限制 / 400 INVALID_REQUEST：开票请求已被受理，请调用【查询电子发票】接口获取开票结果 / 400 INVALID_REQUEST：交易单已申请开票，两次申请的交易金额不相同 / 400 INVALID_REQUEST：交易可剩余开票金额不足 / 403 NO_AUTH：商户乐企不动产租赁发票能力未授权 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户尚未配置电子发票卡券模板信息 / 403 RULE_LIMIT：商户不可对该货物或应税劳务、服务编码开具发票 / 403 RULE_LIMIT：商户提交的购买方信息与用户填写的发票抬头不一致 / 403 RULE_LIMIT：微信支付订单所属商户与当前商户不一致（仅微信支付场景） / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头填写（仅非微信支付场景） / 429 FREQUENCY_LIMITED：频率超限。

### 8. `IssueRealEstateLeasingFapiaoAsync` — 开具不动产租赁行业电子发票。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941740（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing`

- **请求体**：`FapiaoRealEstateLeasingIssueRequest` — 开具不动产租赁行业电子发票请求体，字段见 `FapiaoRealEstateLeasingIssueRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing`；无 path / query 参数。
- **业务错误码**：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 RESOURCE_ALREADY_EXISTS：发票申请单已存在 / 400 INVALID_REQUEST：交易单已申请开票，两次申请的交易金额不相同 / 400 INVALID_REQUEST：当前发票开具中，不可申请重开发票 / 400 INVALID_REQUEST：当前发票冲红中，不可申请重开发票 / 400 INVALID_REQUEST：申请开票次数，超过最大限制 / 400 INVALID_REQUEST：开票请求已被受理，请调用【查询电子发票】接口获取开票结果 / 400 INVALID_REQUEST：交易可剩余开票金额不足 / 403 NO_AUTH：商户乐企不动产租赁发票能力未授权 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户尚未配置电子发票卡券模板信息 / 403 RULE_LIMIT：商户不可对该货物或应税劳务、服务编码开具发票 / 403 RULE_LIMIT：商户提交的购买方信息与用户填写的发票抬头不一致 / 403 RULE_LIMIT：微信支付订单所属商户与当前商户不一致（仅微信支付场景） / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头填写（仅非微信支付场景） / 429 FREQUENCY_LIMITED：频率超限。

### 9. `GetFapiaoSpInviteUrlAsync` — 获取服务商电子发票能力邀请链接。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941495（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `operationType` | Query `operation_type` | `string` | 【操作类型】 商户接受请求后的操作类型。可选取值：`AUTH_BINDING` 授权绑定（官方 query `operation_type`，**必填**，类型 `string`）。 |
| `fapiaoMode` | Query `fapiao_mode` | `string` | 【开票模式】 邀请商户开通服务商电子发票的可选开票模式。可选取值：`THIRD_OR_SELF_FAPIAO` 第三方/自建发票，即使用自有或其他第三方发票系统生成发票 / `TENCENT_DIGITAL_TAX` 腾讯数电发票，即使用腾讯数电发票系统生成发票（官方 query `fapiao_mode`，**必填**，类型 `array[string]`，官方线格式为 URL 编码 JSON ⇒ 以 `string` 承载）。 |
| `inviteCode` | Query `invite_code` | `string` | 【服务商邀请code】 服务商邀请的唯一标识，由服务商生成，可根据邀请code查询邀请开通情况（官方 query `invite_code`，**必填**，类型 `string(64)`）。 |
| `fapiaoAbilityTypeList` | Query `fapiao_ability_type_list` | `string?` | 【发票能力类型】 邀请商户开通的腾讯数电发票能力类型，可以选择多个。当开票模式为TENCENT_DIGITAL_TAX时必填。可选取值：`BASE_ABILITY` 基础开票能力（适合餐饮、酒店等） / `REAL_ESTATE_ABILITY` 不动产租赁能力（适合停车场等） / `PASSENGER_TRANSPORT_ABILITY` 旅客运输能力（适合公共交通等行业） / `REFINED_OIL_ABILITY` 成品油能力（适合石油石化等行业）（官方 query `fapiao_ability_type_list`，选填，类型 `array[string]`，官方线格式为 URL 编码 JSON ⇒ 以 `string` 承载）。 |
| `inviteChannel` | Query `invite_channel` | `string?` | 【服务商邀请渠道】 服务商自定义的渠道信息，用于标识该邀请从哪个渠道发起（官方 query `invite_channel`，选填，类型 `string(128)`）。 |
| `operateUser` | Query `operate_user` | `string?` | 【操作人员】 服务商邀请操作人员信息，用于标识该邀请是哪个人员发起。 注意：该字段为密文字段，需要使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引（官方 query `operate_user`，选填，类型 `string`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【受邀请商户号】 受服务商邀请开通电子发票能力的商户号，受邀商户号和受邀税号不能同时填（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
| `taxpayerId` | Query `taxpayer_id` | `string?` | 【受邀请商户税号】 受服务商邀请开通电子发票能力的税号，保证受邀请商户提交的商户信息与提交邀请的税号保持一致，受邀商户号和受邀税号不能同时填（官方 query `taxpayer_id`，选填，类型 `string(20)`）。 |
- **返回**：`FapiaoSpInviteUrlResponse` — 获取服务商电子发票能力邀请链接，见 `FapiaoSpInviteUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl`；query `operation_type` / `fapiao_mode` / `invite_code` / `fapiao_ability_type_list` / `invite_channel` / `operate_user` / `sub_mchid` / `taxpayer_id`。
- **`operation_type` 与 `fapiao_mode` 与 `invite_code` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 10. `ListFapiaoSpInviteMerchantInfoAsync` — 获取邀请开通的商户信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015941524（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `queryTimeStart` | Query `query_time_start` | `string` | 【查询时间起】 查询邀请商户的起始时间，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方 query `query_time_start`，**必填**，类型 `string`）。 |
| `queryTimeEnd` | Query `query_time_end` | `string` | 【查询时间止】 查询商户邀请的结束时间，与query_time_start的间隔时间不能大于7天。遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方 query `query_time_end`，**必填**，类型 `string`）。 |
| `offset` | Query `offset` | `int` | 【本次查询的起始位置】 当前查询的分页页数。从0开始计数，如offset=10，表示从第11条记录开始返回（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【本次查询的最大数量】 每页所查询的商户信息数量（官方 query `limit`，**必填**，类型 `integer`）。 |
| `mchInviteStatus` | Query `mch_invite_status` | `string` | 【商户邀请状态】 商户邀请状态。可选取值：`MCH_INVITE_SUCC` 商户已接受邀请，通过 [【检查子商户开票功能状态】](https://pay.weixin.qq.com/doc/v3/partner/4015792561.md) 接口查询具体状态 / `MCH_INVITE_FAILED` 商户邀请开通失败，具体失败原因查看invite_failed_reason字段（官方 query `mch_invite_status`，**必填**，类型 `string`）。 |
| `inviteCode` | Query `invite_code` | `string?` | 【服务商邀请code】 邀请链接唯一标识（官方 query `invite_code`，选填，类型 `string`）。 |
- **返回**：`FapiaoSpInviteMerchantListResponse` — 获取邀请开通的商户信息，见 `FapiaoSpInviteMerchantListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo`；query `query_time_start` / `query_time_end` / `offset` / `limit` / `mch_invite_status` / `invite_code`。
- **`query_time_start` 与 `query_time_end` 与 `offset` 与 `limit` 与 `mch_invite_status` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 11. `CheckFapiaoMerchantAsync` — 检查子商户电子发票功能状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012474022（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`POST /v3/new-tax-control-fapiao/merchant/{subMchid}/check`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Path | `string` | 【子商户号】 微信支付分配的子商户号（官方 path `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/new-tax-control-fapiao/merchant/{sub_mchid}/check`；path `sub_mchid`。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：服务商与子商户之间无受理关系，请确认子商户是否对服务商进行相关授权 / 403 NO_AUTH：商户未完成服务商电子发票的开通设置 / 429 FREQUENCY_LIMITED：频率超限。

### 12. `QueryFapiaoMerchantCheckStatusAsync` — 查询受邀商户开票能力开通状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015792561（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/merchant/{subMchid}/check-status`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Path | `string` | 【子商户号】 微信支付分配的子商户号，具体请参考 服务商模式开发必要参数说明（官方 path `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`FapiaoMerchantCheckStatusResponse` — 查询受邀商户开票能力开通状态，见 `FapiaoMerchantCheckStatusResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/merchant/{sub_mchid}/check-status`；path `sub_mchid`。

### 13. `QueryFapiaoMerchantBaseInformationAsync` — 查询商户开票基础信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538180（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474053（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/merchant/base-information`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`FapiaoMerchantBaseInformationResponse` — 查询商户开票基础信息，见 `FapiaoMerchantBaseInformationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/merchant/base-information`；query `sub_mchid`。

### 14. `QueryFapiaoMerchantDevelopmentConfigAsync` — 查询商户开发选项。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529526（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474033（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/merchant/development-config`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchCode` | Query `sub_mch_code` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商为子商户设置开发配置信息时必填（官方 query `sub_mch_code`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`FapiaoMerchantDevelopmentConfigResponse` — 查询商户开发选项，见 `FapiaoMerchantDevelopmentConfigResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/merchant/development-config`；query `sub_mch_code`。

### 15. `UpdateFapiaoMerchantDevelopmentConfigAsync` — 配置商户开发选项。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012529457（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474031（【普通服务商】）、https://pay.weixin.qq.com/doc/v3/partner/4015792563（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`PATCH /v3/new-tax-control-fapiao/merchant/development-config`

- **请求体**：`FapiaoMerchantDevelopmentConfigUpdateRequest` — 配置商户开发选项请求体，字段见 `FapiaoMerchantDevelopmentConfigUpdateRequest`。
- **返回**：`FapiaoMerchantDevelopmentConfigUpdateResponse` — 配置商户开发选项，见 `FapiaoMerchantDevelopmentConfigUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/new-tax-control-fapiao/merchant/development-config`；无 path / query 参数。
- **应答**见 `FapiaoMerchantDevelopmentConfigUpdateResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。

### 16. `QueryFapiaoTaxCodesAsync` — 查询商户税收分类编码对照表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538236（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474057（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/merchant/tax-codes`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `int` | 【本次查询的起始位置】 本次查询的起始位置（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【本次查询的最大数量】 本次查询的最大数量（官方 query `limit`，**必填**，类型 `integer`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发）。 |
- **返回**：`FapiaoTaxCodeListResponse` — 查询商户税收分类编码对照表，见 `FapiaoTaxCodeListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/merchant/tax-codes`；query `offset` / `limit` / `sub_mchid`。
- **`offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 17. `QueryFapiaoUserTitleAsync` — 获取用户填写抬头信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538112（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474048（【普通服务商】）、https://pay.weixin.qq.com/doc/v3/partner/4015784260（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/user-title`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Query `fapiao_apply_id` | `string` | 【发票申请单号】 发票申请单号，唯一标识一次开票行为。当开票场景为WITHOUT_WECHATPAY时，为调用【获取抬头填写链接】接口时指定的发票申请单号；当开票场景为WITH_WECHATPAY时，为与本次开票关联的微信支付订单号，且必须是属于相应商户的订单（官方 query `fapiao_apply_id`，**必填**，类型 `string(32)`）。 |
| `scene` | Query `scene` | `string` | 【开票场景】 开票场景。可选取值：`WITH_WECHATPAY` 微信支付场景 / `WITHOUT_WECHATPAY` 非微信支付场景（官方 query `scene`，**必填**，类型 `string`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发，服务商页页标选填、服务商页页标必填）。 |
- **返回**：`FapiaoUserTitleResponse` — 获取用户填写抬头信息，见 `FapiaoUserTitleResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/user-title`；query `fapiao_apply_id` / `scene` / `sub_mchid`。
- **`fapiao_apply_id` 与 `scene` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头的填写 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。

### 18. `GetFapiaoUserTitleUrlAsync` — 获取用户抬头填写链接。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012538106（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474040（【普通服务商】）、https://pay.weixin.qq.com/doc/v3/partner/4015770776（【普通服务商】）。

- **声明接口**：[IWechatPayFapiaoService](./IWechatPayFapiaoService.cs)
- **官方契约**：`GET /v3/new-tax-control-fapiao/user-title/title-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `fapiaoApplyId` | Query `fapiao_apply_id` | `string` | 【发票申请单号】 发票申请单号，唯一标识一次开票行为。只能是字母、数字、中划线-、下划线_、竖线\|、星号*这些英文半角字符，且该单号在每个商户下必须唯一。当用户完成抬头填写后，微信支付会以该单号回调商户，商户可通过【获取用户填写的抬头】接口查询用户填写的抬头信息（官方 query `fapiao_apply_id`，**必填**，类型 `string(32)`）。 |
| `appid` | Query `appid` | `string` | 【商户AppID】 若开票来源是WEB，则为商户的公众号AppID；若开票来源是MINIPROGRAM，则为商户的小程序AppID（官方 query `appid`，**必填**，类型 `string(32)`）。 |
| `openid` | Query `openid` | `string` | 【用户OpenID】 需要填写发票抬头的用户在商户AppID下的OpenID（官方 query `openid`，**必填**，类型 `string(32)`）。 |
| `totalAmount` | Query `total_amount` | `int` | 【总金额】 展示在抬头填写页面的开票金额，对该发票申请单开票时所有发票的总价税合计之和不能超过此金额，单位：分（官方 query `total_amount`，**必填**，类型 `integer`）。 |
| `source` | Query `source` | `string` | 【开票来源】 开票来源。可选取值：`WEB` 微信H5 / `MINIPROGRAM` 微信小程序（官方 query `source`，**必填**，类型 `string`）。 |
| `sellerName` | Query `seller_name` | `string?` | 【销售方名称】 展示在抬头填写页的销售方名称，若不传则默认取商户名称（官方 query `seller_name`，选填，类型 `string(32)`）。 |
| `showPhoneCell` | Query `show_phone_cell` | `bool?` | 【是否需要展示手机号填写栏】 是否需要展示手机号填写栏（官方 query `show_phone_cell`，选填，类型 `boolean`）。 |
| `mustInputPhone` | Query `must_input_phone` | `bool?` | 【用户是否必须填写手机号】 用户是否必须填写手机号，仅当需要展示手机号填写栏时生效（官方 query `must_input_phone`，选填，类型 `boolean`）。 |
| `showEmailCell` | Query `show_email_cell` | `bool?` | 【是否需要展示邮箱地址填写栏】 是否需要展示邮箱地址填写栏（官方 query `show_email_cell`，选填，类型 `boolean`）。 |
| `mustInputEmail` | Query `must_input_email` | `bool?` | 【用户是否必须填写邮箱地址】 用户是否必须填写邮箱地址，仅当需要展示邮箱地址填写栏时生效（官方 query `must_input_email`，选填，类型 `boolean`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query `sub_mchid`，选填，类型 `string(32)`，**仅服务商页**印发，服务商页页标选填、服务商页页标必填）。 |
- **返回**：`FapiaoUserTitleUrlResponse` — 获取用户抬头填写链接，见 `FapiaoUserTitleUrlResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/new-tax-control-fapiao/user-title/title-url`；query `fapiao_apply_id` / `appid` / `openid` / `total_amount` / `source` / `seller_name` / `show_phone_cell` / `must_input_phone` / `show_email_cell` / `must_input_email` / `sub_mchid`。
- **`fapiao_apply_id` 与 `appid` 与 `openid` 与 `total_amount` 与 `source` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户获取抬头链接指定的用户与已提交抬头的用户不一致 / 429 FREQUENCY_LIMITED：频率超限 / 403 NO_AUTH：RULE_LIMIT / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/Fapiao`](../../../Mud.Wechat.Pay/DataModels/Fapiao/) 内共 **53** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **53** 个，`SerializerClassName` = `NewTaxControlFapiao`。

AOT 源生成上下文：[`NewTaxControlFapiaoJsonContext.g.cs`](./../../DataModels/Generated/NewTaxControlFapiaoJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `FapiaoIssueRequest` | 请求体 | [Fapiao/FapiaoDtos.cs](../../DataModels/Fapiao/FapiaoDtos.cs) | `scene`, `fapiao_apply_id`, `buyer_information`, `fapiao_information`, `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone` … |
| `FapiaoQueryResponse` | 应答 | [Fapiao/FapiaoDtos.cs](../../DataModels/Fapiao/FapiaoDtos.cs) | `scene`, `fapiao_apply_id`, `buyer_information`, `fapiao_information`, `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone` … |
| `FapiaoReverseRequest` | 请求体 | [Fapiao/FapiaoDtos.cs](../../DataModels/Fapiao/FapiaoDtos.cs) | `scene`, `fapiao_apply_id`, `buyer_information`, `fapiao_information`, `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone` … |
| `FapiaoFilesResponse` | 应答 | [Fapiao/FapiaoDtos.cs](../../DataModels/Fapiao/FapiaoDtos.cs) | `scene`, `fapiao_apply_id`, `buyer_information`, `fapiao_information`, `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone` … |
| `FapiaoInsertCardsRequest` | 请求体 | [Fapiao/FapiaoDtos.cs](../../DataModels/Fapiao/FapiaoDtos.cs) | `scene`, `fapiao_apply_id`, `buyer_information`, `fapiao_information`, `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone` … |
| `FapiaoCardTemplateResponse` | 应答 | [Fapiao/FapiaoCardTemplateDtos.cs](../../DataModels/Fapiao/FapiaoCardTemplateDtos.cs) | `words`, `description`, `jump_url`, `miniprogram_user_name`, `miniprogram_path`, `payee_name`, `logo_url`, `custom_cell`, `card_appid`, `card_template_information`, `sub_mchid`, `card_id` |
| `FapiaoCardTemplateRequest` | 请求体 | [Fapiao/FapiaoCardTemplateDtos.cs](../../DataModels/Fapiao/FapiaoCardTemplateDtos.cs) | `words`, `description`, `jump_url`, `miniprogram_user_name`, `miniprogram_path`, `payee_name`, `logo_url`, `custom_cell`, `card_appid`, `card_template_information`, `sub_mchid`, `card_id` |
| `FapiaoGeneralIssueRequest` | 请求体 | [Fapiao/FapiaoIssueExtensionDtos.cs](../../DataModels/Fapiao/FapiaoIssueExtensionDtos.cs) | `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone`, `email`, `amount`, `out_trade_no`, `fapiao_bill_type` … |
| `FapiaoRealEstateLeasingIssueRequest` | 请求体 | [Fapiao/FapiaoIssueExtensionDtos.cs](../../DataModels/Fapiao/FapiaoIssueExtensionDtos.cs) | `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone`, `email`, `amount`, `out_trade_no`, `fapiao_bill_type` … |
| `FapiaoSpInviteUrlResponse` | 应答 | [Fapiao/FapiaoMerchantInviteDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInviteDtos.cs) | `invite_url`, `miniprogram_appid`, `miniprogram_path`, `next`, `prev`, `self`, `sub_mchid`, `mch_invite_status`, `ep_name`, `tax_id`, `invite_code`, `operate_time` … |
| `FapiaoSpInviteMerchantListResponse` | 应答 | [Fapiao/FapiaoMerchantInviteDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInviteDtos.cs) | `invite_url`, `miniprogram_appid`, `miniprogram_path`, `next`, `prev`, `self`, `sub_mchid`, `mch_invite_status`, `ep_name`, `tax_id`, `invite_code`, `operate_time` … |
| `FapiaoMerchantCheckStatusResponse` | 应答 | [Fapiao/FapiaoMerchantCheckDtos.cs](../../DataModels/Fapiao/FapiaoMerchantCheckDtos.cs) | `status`, `id`, `name`, `type`, `authorize_fail_reason`, `billing_person_info`, `access_time`, `expired_time`, `access_fail_reason`, `ability_info`, `sub_mchid`, `third_mode` … |
| `FapiaoMerchantBaseInformationResponse` | 应答 | [Fapiao/FapiaoMerchantInfoDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInfoDtos.cs) | `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `payee`, `reviewer`, `drawer`, `seller_information`, `extra_information`, `callback_url` … |
| `FapiaoMerchantDevelopmentConfigResponse` | 应答 | [Fapiao/FapiaoMerchantInfoDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInfoDtos.cs) | `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `payee`, `reviewer`, `drawer`, `seller_information`, `extra_information`, `callback_url` … |
| `FapiaoMerchantDevelopmentConfigUpdateResponse` | 应答 | [Fapiao/FapiaoMerchantInfoDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInfoDtos.cs) | `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `payee`, `reviewer`, `drawer`, `seller_information`, `extra_information`, `callback_url` … |
| `FapiaoMerchantDevelopmentConfigUpdateRequest` | 请求体 | [Fapiao/FapiaoMerchantInfoDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInfoDtos.cs) | `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `payee`, `reviewer`, `drawer`, `seller_information`, `extra_information`, `callback_url` … |
| `FapiaoTaxCodeListResponse` | 应答 | [Fapiao/FapiaoMerchantInfoDtos.cs](../../DataModels/Fapiao/FapiaoMerchantInfoDtos.cs) | `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `payee`, `reviewer`, `drawer`, `seller_information`, `extra_information`, `callback_url` … |
| `FapiaoUserTitleResponse` | 应答 | [Fapiao/FapiaoUserTitleDtos.cs](../../DataModels/Fapiao/FapiaoUserTitleDtos.cs) | `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone`, `email`, `amount`, `out_trade_no`, `fapiao_bill_type` … |
| `FapiaoUserTitleUrlResponse` | 应答 | [Fapiao/FapiaoUserTitleDtos.cs](../../DataModels/Fapiao/FapiaoUserTitleDtos.cs) | `type`, `name`, `taxpayer_id`, `address`, `telephone`, `bank_name`, `bank_account`, `phone`, `email`, `amount`, `out_trade_no`, `fapiao_bill_type` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.NewTaxControlFapiao));

var service = app.Services.GetRequiredService<IWechatPayFapiaoService>();
var ct = CancellationToken.None;
await service.CheckFapiaoMerchantAsync("", ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayFapiaoService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CheckFapiaoMerchantAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayFapiaoService](./IWechatPayFapiaoService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayFapiaoContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFapiaoContractGuards.cs)、[`WechatPayFapiaoExtensionContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFapiaoExtensionContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `FapiaoEndpoints_ShouldMatchOfficialRoutes`、`FapiaoDtos_ShouldExposeOfficialFieldNames`、`IssueAndQueryItemShapes_ShouldStaySeparate`、`FapiaoDataModels_ShouldBeRegisteredInJsonContext`、`FapiaoExtensionTopology_ShouldMatchInterfaceAndGroupRoster`、`FapiaoExtensionEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`FapiaoExtensionDtos_ShouldExposeOfficialFieldRosters`、`FapiaoExtensionDataModels_ShouldSplitRequestAndResponseBaseTypes`、`FapiaoExtensionAllDtos_ShouldBeAnnotatedAndContextRegistered`、`FapiaoExtensionRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/Fapiao`](../../../Mud.Wechat.Pay/DataModels/Fapiao/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayFapiaoContractGuards.cs`、`WechatPayFapiaoExtensionContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

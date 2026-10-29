# MarketingPayGiftActivity 域 · 支付有礼活动

微信支付「支付有礼活动」域 SDK（8 端点）。 接口：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)；注册组 `MarketingPayGiftActivity`；模块枚举 `PayModule.MarketingPayGiftActivity`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingPayGiftActivityContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPayGiftActivityContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingPayGiftActivity`](../../DataModels/MarketingPayGiftActivity)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingPayGiftActivityService = 8；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingPayGiftActivityService` | `CreatePayGiftActivityAsync` | POST | `/v3/marketing/paygiftactivity/unique-threshold-activity` | 商户证书 / APIv3 签名 | 创建支付有礼活动。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487898（普通商户）、https://pay.weix… |
| 2 | `IWechatPayMarketingPayGiftActivityService` | `QueryPayGiftActivitiesAsync` | GET | `/v3/marketing/paygiftactivity/activities` | 商户证书 / APIv3 签名 | 查询支付有礼活动列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489126（普通商户）、https://pay.we… |
| 3 | `IWechatPayMarketingPayGiftActivityService` | `QueryPayGiftActivityAsync` | GET | `/v3/marketing/paygiftactivity/activities/{activityId}` | 商户证书 / APIv3 签名 | 查询支付有礼活动详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487971（普通商户）、https://pay.we… |
| 4 | `IWechatPayMarketingPayGiftActivityService` | `TerminatePayGiftActivityAsync` | POST | `/v3/marketing/paygiftactivity/activities/{activityId}/terminate` | 商户证书 / APIv3 签名 | 终止支付有礼活动。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466523（普通商户）、https://pay.weix… |
| 5 | `IWechatPayMarketingPayGiftActivityService` | `QueryPayGiftActivityGoodsAsync` | GET | `/v3/marketing/paygiftactivity/activities/{activityId}/goods` | 商户证书 / APIv3 签名 | 查询活动商品。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466448（普通商户）、https://pay.weixin… |
| 6 | `IWechatPayMarketingPayGiftActivityService` | `QueryPayGiftActivityMerchantsAsync` | GET | `/v3/marketing/paygiftactivity/activities/{activityId}/merchants` | 商户证书 / APIv3 签名 | 查询活动商户列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466149（普通商户）、https://pay.weix… |
| 7 | `IWechatPayMarketingPayGiftActivityService` | `AddPayGiftActivityMerchantsAsync` | POST | `/v3/marketing/paygiftactivity/activities/{activityId}/merchants/add` | 商户证书 / APIv3 签名 | 添加活动商户。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466671（普通商户）、https://pay.weixin… |
| 8 | `IWechatPayMarketingPayGiftActivityService` | `DeletePayGiftActivityMerchantsAsync` | POST | `/v3/marketing/paygiftactivity/activities/{activityId}/merchants/delete` | 商户证书 / APIv3 签名 | 删除活动商户。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466787（普通商户）、https://pay.weixin… |

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

### 跨端点重复提示（原文去重）

- （出现 8 次）**支持商户**【普通商户】【普通服务商】。
- （出现 8 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 5 次）**业务错误码**：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。

### 1. `CreatePayGiftActivityAsync` — 创建支付有礼活动。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487898（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012492900（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`POST /v3/marketing/paygiftactivity/unique-threshold-activity`

- **请求体**：`PayGiftActivityCreateRequest` — 创建支付有礼活动请求体，字段见 `PayGiftActivityCreateRequest`。
- **返回**：`PayGiftActivityCreateResponse` — 创建支付有礼活动，见 `PayGiftActivityCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/paygiftactivity/unique-threshold-activity`；无 path / query 参数。
- **应答**见 `PayGiftActivityCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误 / 400 APPID_MCHID_NOT_MATCH AppID与mchid不匹配。

### 2. `QueryPayGiftActivitiesAsync` — 查询支付有礼活动列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012489126（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012493214（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`GET /v3/marketing/paygiftactivity/activities`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `int` | 【分页页码】 分页页码（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小（官方 query `limit`，**必填**，类型 `integer`）。 |
| `activityName` | Query `activity_name` | `string?` | 【活动名称】 活动名称，支持模糊搜索（官方 query `activity_name`，选填，类型 `string(10)`）。 |
| `activityStatus` | Query `activity_status` | `string?` | 【活动状态】 活动状态。可选取值：`ACT_STATUS_UNKNOWN` 状态未知 / `CREATE_ACT_STATUS` 已创建 / `ONGOING_ACT_STATUS` 运行中 / `TERMINATE_ACT_STATUS` 已终止 / `STOP_ACT_STATUS` 已暂停 / `OVER_TIME_ACT_STATUS` 已过期 / `CREATE_ACT_FAILED` 创建活动失败（官方 query `activity_status`，选填，类型 `string`）。 |
| `awardType` | Query `award_type` | `string?` | 【奖品类型】 奖品类型，暂时只支持商家券。可选取值：`BUSIFAVOR` 商家券（官方 query `award_type`，选填，类型 `string`）。 |
- **返回**：`PayGiftActivityListResponse` — 查询支付有礼活动列表，见 `PayGiftActivityListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/paygiftactivity/activities`；query `offset` / `limit` / `activity_name` / `activity_status` / `award_type`。
- **`offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 3. `QueryPayGiftActivityAsync` — 查询支付有礼活动详情。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012487971（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012492967（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`GET /v3/marketing/paygiftactivity/activities/{activityId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string`）。 |
- **返回**：`PayGiftActivityQueryResponse` — 查询支付有礼活动详情，见 `PayGiftActivityQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/paygiftactivity/activities/{activity_id}`；path `activity_id`。

### 4. `TerminatePayGiftActivityAsync` — 终止支付有礼活动。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466523（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466633（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`POST /v3/marketing/paygiftactivity/activities/{activityId}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string(20)`）。 |
- **返回**：`PayGiftActivityTerminateResponse` — 终止支付有礼活动，见 `PayGiftActivityTerminateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/paygiftactivity/activities/{activity_id}/terminate`；path `activity_id`。

### 5. `QueryPayGiftActivityGoodsAsync` — 查询活动商品。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466448（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466492（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`GET /v3/marketing/paygiftactivity/activities/{activityId}/goods`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string(20)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 分页页码（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`PayGiftActivityGoodsResponse` — 查询活动商品，见 `PayGiftActivityGoodsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/paygiftactivity/activities/{activity_id}/goods`；path `activity_id`，query `offset` / `limit`。

### 6. `QueryPayGiftActivityMerchantsAsync` — 查询活动商户列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466149（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466191（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`GET /v3/marketing/paygiftactivity/activities/{activityId}/merchants`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string(20)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 分页页码（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 限制分页最大数据条目（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`PayGiftActivityMerchantsResponse` — 查询活动商户列表，见 `PayGiftActivityMerchantsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/paygiftactivity/activities/{activity_id}/merchants`；path `activity_id`，query `offset` / `limit`。

### 7. `AddPayGiftActivityMerchantsAsync` — 添加活动商户。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466671（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466735（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`POST /v3/marketing/paygiftactivity/activities/{activityId}/merchants/add`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string(20)`）。 |

- **请求体**：`PayGiftMerchantAddRequest` — 添加活动商户请求体，字段见 `PayGiftMerchantAddRequest`。
- **返回**：`PayGiftMerchantAddResponse` — 添加活动商户，见 `PayGiftMerchantAddResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/paygiftactivity/activities/{activity_id}/merchants/add`；path `activity_id`。
- **应答**见 `PayGiftMerchantAddResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 查询活动详情结果为空，请确认活动ID是否正确 / 400 INVALID_REQUEST 发券商户号校验失败，请核实是否满足同品牌等规则 / 400 INVALID_REQUEST 发券商户号非奖品的同品牌商户号 / 400 INVALID_REQUEST 券归属商户或发券商户非同品牌 / 400 INVALID_REQUEST 创建方商户与发券商户非同品牌 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 PARAM_ERROR 创建活动参数错误，请联系技术支持 / 400 PARAM_ERROR 活动曝光商户的同品牌商户中需要至少存在一个与创建方商户具备父子关系 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 403 RULE_LIMIT 上一次修改还未完成，请稍后重试 / 403 RULE_LIMIT 活动不在运行中，不允许再进行修改 / 403 RULE_LIMIT 所有的曝光商户号均校验失败，请再次检查 / 403 RULE_LIMIT 并发修改，请稍后再试 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。

### 8. `DeletePayGiftActivityMerchantsAsync` — 删除活动商户。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466787（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466827（普通服务商）。

- **声明接口**：[IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs)
- **官方契约**：`POST /v3/marketing/paygiftactivity/activities/{activityId}/merchants/delete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动Id】 活动Id（官方 path `activity_id`，**必填**，类型 `string(20)`）。 |

- **请求体**：`PayGiftMerchantDeleteRequest` — 删除活动商户请求体，字段见 `PayGiftMerchantDeleteRequest`。
- **返回**：`PayGiftMerchantDeleteResponse` — 删除活动商户，见 `PayGiftMerchantDeleteResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/paygiftactivity/activities/{activity_id}/merchants/delete`；path `activity_id`。
- **应答**见 `PayGiftMerchantDeleteResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH AppID与MchID不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 查询活动详情结果为空，请确认活动ID是否正确 / 400 MCH_NOT_EXISTS 商户号不存在 / 400 PARAM_ERROR 创建活动参数错误，请联系技术支持 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 403 RULE_LIMIT 上一次修改还未完成，请稍后重试 / 403 RULE_LIMIT 活动不在运行中，不允许再进行修改 / 403 RULE_LIMIT 并发修改，请稍后再试 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限 / 500 SYSTEM_ERROR 系统错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingPayGiftActivity`](../../DataModels/MarketingPayGiftActivity) 内共 **44** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **44** 个，`SerializerClassName` = `MarketingPayGiftActivity`。

AOT 源生成上下文：[`MarketingPayGiftActivityJsonContext.g.cs`](./../../DataModels/Generated/MarketingPayGiftActivityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PayGiftActivityCreateResponse` | 应答 | [MarketingPayGiftActivity/PayGiftActivityDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftActivityDtos.cs) | `begin_time`, `end_time`, `begin_day_time`, `end_day_time`, `available_time`, `available_day_time`, `activity_name`, `activity_second_title`, `merchant_logo_url`, `background_color`, `available_periods`, `out_request_no` … |
| `PayGiftActivityCreateRequest` | 请求体 | [MarketingPayGiftActivity/PayGiftActivityDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftActivityDtos.cs) | `begin_time`, `end_time`, `begin_day_time`, `end_day_time`, `available_time`, `available_day_time`, `activity_name`, `activity_second_title`, `merchant_logo_url`, `background_color`, `available_periods`, `out_request_no` … |
| `PayGiftActivityListResponse` | 应答 | [MarketingPayGiftActivity/PayGiftActivityDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftActivityDtos.cs) | `begin_time`, `end_time`, `begin_day_time`, `end_day_time`, `available_time`, `available_day_time`, `activity_name`, `activity_second_title`, `merchant_logo_url`, `background_color`, `available_periods`, `out_request_no` … |
| `PayGiftActivityQueryResponse` | 应答 | [MarketingPayGiftActivity/PayGiftActivityDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftActivityDtos.cs) | `begin_time`, `end_time`, `begin_day_time`, `end_day_time`, `available_time`, `available_day_time`, `activity_name`, `activity_second_title`, `merchant_logo_url`, `background_color`, `available_periods`, `out_request_no` … |
| `PayGiftActivityTerminateResponse` | 应答 | [MarketingPayGiftActivity/PayGiftActivityDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftActivityDtos.cs) | `begin_time`, `end_time`, `begin_day_time`, `end_day_time`, `available_time`, `available_day_time`, `activity_name`, `activity_second_title`, `merchant_logo_url`, `background_color`, `available_periods`, `out_request_no` … |
| `PayGiftActivityGoodsResponse` | 应答 | [MarketingPayGiftActivity/PayGiftGoodsDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftGoodsDtos.cs) | `goods_id`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id` |
| `PayGiftActivityMerchantsResponse` | 应答 | [MarketingPayGiftActivity/PayGiftMerchantDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftMerchantDtos.cs) | `mchid`, `merchant_name`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id`, `merchant_id_list`, `add_request_no`, `invalid_reason` … |
| `PayGiftMerchantAddResponse` | 应答 | [MarketingPayGiftActivity/PayGiftMerchantDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftMerchantDtos.cs) | `mchid`, `merchant_name`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id`, `merchant_id_list`, `add_request_no`, `invalid_reason` … |
| `PayGiftMerchantAddRequest` | 请求体 | [MarketingPayGiftActivity/PayGiftMerchantDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftMerchantDtos.cs) | `mchid`, `merchant_name`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id`, `merchant_id_list`, `add_request_no`, `invalid_reason` … |
| `PayGiftMerchantDeleteResponse` | 应答 | [MarketingPayGiftActivity/PayGiftMerchantDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftMerchantDtos.cs) | `mchid`, `merchant_name`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id`, `merchant_id_list`, `add_request_no`, `invalid_reason` … |
| `PayGiftMerchantDeleteRequest` | 请求体 | [MarketingPayGiftActivity/PayGiftMerchantDtos.cs](../../DataModels/MarketingPayGiftActivity/PayGiftMerchantDtos.cs) | `mchid`, `merchant_name`, `create_time`, `update_time`, `data`, `total_count`, `offset`, `limit`, `activity_id`, `merchant_id_list`, `add_request_no`, `invalid_reason` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingPayGiftActivity;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingPayGiftActivity));

var service = app.Services.GetRequiredService<IWechatPayMarketingPayGiftActivityService>();
var ct = CancellationToken.None;
var response = await service.CreatePayGiftActivityAsync(new PayGiftActivityCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/paygiftactivity/unique-threshold-activity");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingPayGiftActivityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreatePayGiftActivityAsync`）；请求体 `PayGiftActivityCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingPayGiftActivityService](./IWechatPayMarketingPayGiftActivityService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingPayGiftActivityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPayGiftActivityContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayGiftTopology_ShouldBeOneInterfaceOnOneGroup`、`PayGiftEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PayGiftDtos_ShouldExposeOfficialFieldRosters`、`PayGiftDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingPayGiftActivity`](../../DataModels/MarketingPayGiftActivity) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingPayGiftActivityContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

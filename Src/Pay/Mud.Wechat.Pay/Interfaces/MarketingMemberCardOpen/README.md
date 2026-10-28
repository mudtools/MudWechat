# MarketingMemberCardOpen 域 · 旧版会员卡（membercard-open）

微信支付「旧版会员卡（membercard-open）」域 SDK（25 端点）。 接口：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)；注册组 `MarketingMemberCardOpen`；模块枚举 `PayModule.MarketingMemberCardOpen`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingMemberCardOpenContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingMemberCardOpenContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/MarketingMemberCardOpen`](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingMemberCardOpenService = 25；按 HTTP 特性实计 **25** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingMemberCardOpenService` | `AddMemberCardTemplateAsync` | POST | `/v3/marketing/membercard-open/cards` | 商户证书 / APIv3 签名 | 创建会员卡模板。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545584（【普通商户】）。 |
| 2 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardTemplatesAsync` | GET | `/v3/marketing/membercard-open/cards` | 商户证书 / APIv3 签名 | 查询会员卡模板列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547108（【普通商户】）。 |
| 3 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardTemplateAsync` | GET | `/v3/marketing/membercard-open/cards/{cardId}` | 商户证书 / APIv3 签名 | 查询会员卡模板信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547650（【普通商户】）。 |
| 4 | `IWechatPayMarketingMemberCardOpenService` | `UpdateMemberCardTemplateAsync` | PATCH | `/v3/marketing/membercard-open/cards/{cardId}` | 商户证书 / APIv3 签名 | 修改会员卡模板信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549567（【普通商户】）。 |
| 5 | `IWechatPayMarketingMemberCardOpenService` | `DeleteMemberCardTemplateAsync` | DELETE | `/v3/marketing/membercard-open/cards/{cardId}` | 商户证书 / APIv3 签名 | 删除会员卡模板。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549682（【普通商户】）。 |
| 6 | `IWechatPayMarketingMemberCardOpenService` | `UpgradeMemberCardTemplateAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/upgrade` | 商户证书 / APIv3 签名 | 老会员卡模板升级。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545673（【普通商户】）。 |
| 7 | `IWechatPayMarketingMemberCardOpenService` | `DepositMemberCardCodesAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/deposit` | 商户证书 / APIv3 签名 | 会员卡卡code预存。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545728（【普通商户】）。 |
| 8 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardUserCardAsync` | GET | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}` | 商户证书 / APIv3 签名 | 查询用户会员卡信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547768（【普通商户】）。 |
| 9 | `IWechatPayMarketingMemberCardOpenService` | `UpdateMemberCardUserCardAsync` | PATCH | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}` | 商户证书 / APIv3 签名 | 修改用户会员卡信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549292（【普通商户】）。 |
| 10 | `IWechatPayMarketingMemberCardOpenService` | `InvalidateMemberCardCodeAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/unavailable` | 商户证书 / APIv3 签名 | 作废用户会员卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549677（【普通商户】）。 |
| 11 | `IWechatPayMarketingMemberCardOpenService` | `ConfirmMemberCardCodeAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/confirm` | 商户证书 / APIv3 签名 | 确认用户入会。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4024293873（【普通商户】）。 |
| 12 | `IWechatPayMarketingMemberCardOpenService` | `ImportMemberCardUsersAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/users/import` | 商户证书 / APIv3 签名 | 根据OpenID导入用户会员卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549739（【普通商户】）。 |
| 13 | `IWechatPayMarketingMemberCardOpenService` | `ImportMemberCardPhoneUsersAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/phone-membercard/import` | 商户证书 / APIv3 签名 | 根据手机号导入用户会员卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550535（【普通商户】）。 |
| 14 | `IWechatPayMarketingMemberCardOpenService` | `CreateMemberCardQrcodeAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/qrcode` | 商户证书 / APIv3 签名 | 创建会员卡二维码。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550605（【普通商户】）。 |
| 15 | `IWechatPayMarketingMemberCardOpenService` | `DecryptMemberCardCodeAsync` | GET | `/v3/marketing/membercard-open/cards/{cardId}/decrypt-code` | 商户证书 / APIv3 签名 | 解密会员卡code。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550742（【普通商户】）。 |
| 16 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardActivateInformationAsync` | GET | `/v3/marketing/membercard-open/activate-informations/{activateTicket}` | 商户证书 / APIv3 签名 | 查询会员卡激活信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550779（【普通商户】）。 |
| 17 | `IWechatPayMarketingMemberCardOpenService` | `ActivateMemberCardCodeAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/activate` | 商户证书 / APIv3 签名 | 激活会员卡（跳转型激活）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550809（【普通商户】）。 |
| 18 | `IWechatPayMarketingMemberCardOpenService` | `CreateMemberCardPermissionTokenAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/permission-tokens` | 商户证书 / APIv3 签名 | 获取用户授权令牌。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550867（【普通商户】）。 |
| 19 | `IWechatPayMarketingMemberCardOpenService` | `UpdateMemberCardUserCodeRightsAsync` | PATCH | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/rights` | 商户证书 / APIv3 签名 | 设置用户会员卡权益。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551213（【普通商户】）。 |
| 20 | `IWechatPayMarketingMemberCardOpenService` | `UpdateMemberCardTemplateRightsAsync` | PATCH | `/v3/marketing/membercard-open/cards/{cardId}/rights` | 商户证书 / APIv3 签名 | 设置会员卡模板权益项。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551219（【普通商户】）。 |
| 21 | `IWechatPayMarketingMemberCardOpenService` | `CommitMemberCardBonusAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/commit` | 商户证书 / APIv3 签名 | 提交会员卡积分。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551350（【普通商户】）。 |
| 22 | `IWechatPayMarketingMemberCardOpenService` | `PrepareMemberCardBonusAsync` | POST | `/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/prepare` | 商户证书 / APIv3 签名 | 预下单会员卡积分。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012595808（【普通商户】）。 |
| 23 | `IWechatPayMarketingMemberCardOpenService` | `UpdateMemberCardCallbackAsync` | PATCH | `/v3/marketing/membercard-open/callback` | 商户证书 / APIv3 签名 | 设置会员卡事件通知URL。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012552251（【普通商户】）。 |
| 24 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardCallbackAsync` | GET | `/v3/marketing/membercard-open/callback` | 商户证书 / APIv3 签名 | 查询会员卡事件通知URL。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012552275（【普通商户】）。 |
| 25 | `IWechatPayMarketingMemberCardOpenService` | `QueryMemberCardsByUserAsync` | GET | `/v3/marketing/membercard-open/user/cards` | 商户证书 / APIv3 签名 | 查询用户在品牌下所有会员卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549255（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径；本域 25 支官方**全部只印一页**（merchant 中心【普通商户】）⇒ 一支一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**两套并存契约**：品牌会员卡 `/v3/brand/partner/card-member/*`（已落位 15 端点）与本域是**并存非替代**关系，
不得以「已有替代品」为由并回零声明名册（守卫 MCO-B6 锁定）。

**动词不统一**：修改模板走 **PATCH**，删除模板走 **DELETE**，激活走 **POST** ⇒ 照页不得合并。

**四支 204 无应答包体**：删除模板 / 升级模板 / 作废卡code / 激活卡code 返回裸 `Task`，不建响应类型。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 25 次）**支持商户**【普通商户】。
- （出现 25 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 16 次）**业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。
- （出现 5 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

### 1. `AddMemberCardTemplateAsync` — 创建会员卡模板。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545584（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards`

- **请求体**：`MemberCardTemplateCreateRequest` — 创建会员卡模板请求体，字段见 `MemberCardTemplateCreateRequest`。
- **返回**：`MemberCardTemplateCreateResponse` — 创建会员卡模板，见 `MemberCardTemplateCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards`；body 38 键（含嵌套 brand / date_information / balance_information / user_information_form / additional_statement / favor…），resp 55 键。

### 2. `QueryMemberCardTemplatesAsync` — 查询会员卡模板列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547108（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Query `brand_id` | `string?` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取）（官方 query `brand_id`，选填，类型 `string(16)`）。 |
| `appid` | Query `appid` | `string?` | 【公众号APPID】 公众号APPID，用于筛选该公众号下创建的会员卡（官方 query `appid`，选填，类型 `string(18)`）。 |
| `offset` | Query `offset` | `int?` | 【分页起始位置】 分页起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，最大50（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`MemberCardTemplateListResponse` — 查询会员卡模板列表，见 `MemberCardTemplateListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/cards`；query `brand_id`/`appid`/`offset`/`limit`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

### 3. `QueryMemberCardTemplateAsync` — 查询会员卡模板信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547650（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/cards/{cardId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`MemberCardTemplateResponse` — 查询会员卡模板信息，见 `MemberCardTemplateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/cards/{card_id}`；path `card_id`。

### 4. `UpdateMemberCardTemplateAsync` — 修改会员卡模板信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549567（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`PATCH /v3/marketing/membercard-open/cards/{cardId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardTemplateUpdateRequest` — 修改会员卡模板信息请求体，字段见 `MemberCardTemplateUpdateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/membercard-open/cards/{card_id}`；path `card_id`，body 34 键。

### 5. `DeleteMemberCardTemplateAsync` — 删除会员卡模板。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549682（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`DELETE /v3/marketing/membercard-open/cards/{cardId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**DELETE** `/v3/marketing/membercard-open/cards/{card_id}`；path `card_id`；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 6. `UpgradeMemberCardTemplateAsync` — 老会员卡模板升级。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545673（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/upgrade`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardTemplateUpgradeRequest` — 老会员卡模板升级请求体，字段见 `MemberCardTemplateUpgradeRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/upgrade`；path `card_id`，body 2 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 7. `DepositMemberCardCodesAsync` — 会员卡卡code预存。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012545728（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/deposit`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardCodeDepositRequest` — 会员卡卡code预存请求体，字段见 `MemberCardCodeDepositRequest`。
- **返回**：`MemberCardCodeDepositResponse` — 会员卡卡code预存，见 `MemberCardCodeDepositResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/deposit`；path `card_id`，body 1 键（`code_list` 数组），resp 3 键。

### 8. `QueryMemberCardUserCardAsync` — 查询用户会员卡信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012547768（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/cards/{cardId}/codes/{code}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |
- **返回**：`MemberCardUserCardResponse` — 查询用户会员卡信息，见 `MemberCardUserCardResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}`；path `card_id`/`code`。

### 9. `UpdateMemberCardUserCardAsync` — 修改用户会员卡信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549292（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`PATCH /v3/marketing/membercard-open/cards/{cardId}/codes/{code}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardUserCardUpdateRequest` — 修改用户会员卡信息请求体，字段见 `MemberCardUserCardUpdateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}`；path `card_id`/`code`，body 11 键。

### 10. `InvalidateMemberCardCodeAsync` — 作废用户会员卡。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549677（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/unavailable`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardCodeUnavailableRequest` — 作废用户会员卡请求体，字段见 `MemberCardCodeUnavailableRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/unavailable`；path `card_id`/`code`，body 1 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 11. `ConfirmMemberCardCodeAsync` — 确认用户入会。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4024293873（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/confirm`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardCodeConfirmRequest` — 确认用户入会请求体，字段见 `MemberCardCodeConfirmRequest`。
- **返回**：`MemberCardCodeConfirmResponse` — 确认用户入会，见 `MemberCardCodeConfirmResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/confirm`；path `card_id`/`code`，body 2 键，resp 2 键。

### 12. `ImportMemberCardUsersAsync` — 根据OpenID导入用户会员卡。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549739（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/users/import`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardUserImportRequest` — 根据OpenID导入用户会员卡请求体，字段见 `MemberCardUserImportRequest`。
- **返回**：`MemberCardUserImportResponse` — 根据OpenID导入用户会员卡，见 `MemberCardUserImportResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/users/import`；path `card_id`，body 3 键，resp 1 键（`code`）。

### 13. `ImportMemberCardPhoneUsersAsync` — 根据手机号导入用户会员卡。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550535（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/phone-membercard/import`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardPhoneImportRequest` — 根据手机号导入用户会员卡请求体，字段见 `MemberCardPhoneImportRequest`。
- **返回**：`MemberCardPhoneImportResponse` — 根据手机号导入用户会员卡，见 `MemberCardPhoneImportResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/phone-membercard/import`；path `card_id`，body 3 键，resp 1 键（`code`）。

### 14. `CreateMemberCardQrcodeAsync` — 创建会员卡二维码。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550605（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/qrcode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardQrcodeRequest` — 创建会员卡二维码请求体，字段见 `MemberCardQrcodeRequest`。
- **返回**：`MemberCardQrcodeResponse` — 创建会员卡二维码，见 `MemberCardQrcodeResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/qrcode`；path `card_id`，body 5 键，resp 1 键。

### 15. `DecryptMemberCardCodeAsync` — 解密会员卡code。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550742（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/cards/{cardId}/decrypt-code`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `encryptCode` | Query `encrypt_code` | `string` | 【加密后的code】 加密后的会员卡code（官方 query `encrypt_code`，**必填**，类型 `string(128)`）。 |
- **返回**：`MemberCardDecryptCodeResponse` — 解密会员卡code，见 `MemberCardDecryptCodeResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/cards/{card_id}/decrypt-code`；path `card_id`，query `encrypt_code`。

### 16. `QueryMemberCardActivateInformationAsync` — 查询会员卡激活信息。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550779（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/activate-informations/{activateTicket}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activateTicket` | Path | `string` | 【激活票据】 用户在开卡组件中完成信息填写后生成的激活票据（官方 path `activate_ticket`，**必填**，类型 `string(128)`）。 |
- **返回**：`MemberCardActivateInformationResponse` — 查询会员卡激活信息，见 `MemberCardActivateInformationResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/activate-informations/{activate_ticket}`；path `activate_ticket`。

### 17. `ActivateMemberCardCodeAsync` — 激活会员卡（跳转型激活）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550809（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/activate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardActivateRequest` — 激活会员卡请求体，字段见 `MemberCardActivateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/activate`；path `card_id`/`code`，body 1 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 18. `CreateMemberCardPermissionTokenAsync` — 获取用户授权令牌。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012550867（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/permission-tokens`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardPermissionTokenRequest` — 获取用户授权令牌请求体，字段见 `MemberCardPermissionTokenRequest`。
- **返回**：`MemberCardPermissionTokenResponse` — 获取用户授权令牌，见 `MemberCardPermissionTokenResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/permission-tokens`；path `card_id`，body 6 键，resp 1 键。

### 19. `UpdateMemberCardUserCodeRightsAsync` — 设置用户会员卡权益。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551213（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`PATCH /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/rights`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardUserCodeRightsRequest` — 设置用户会员卡权益请求体，字段见 `MemberCardUserCodeRightsRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/rights`；path `card_id`/`code`，body 13 键。

### 20. `UpdateMemberCardTemplateRightsAsync` — 设置会员卡模板权益项。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551219（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`PATCH /v3/marketing/membercard-open/cards/{cardId}/rights`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardTemplateRightsRequest` — 设置会员卡模板权益项请求体，字段见 `MemberCardTemplateRightsRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/membercard-open/cards/{card_id}/rights`；path `card_id`，body 31 键。

### 21. `CommitMemberCardBonusAsync` — 提交会员卡积分。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551350（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/commit`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardBonusCommitRequest` — 提交会员卡积分请求体，字段见 `MemberCardBonusCommitRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/commit`；path `card_id`/`code`，body 3 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 22. `PrepareMemberCardBonusAsync` — 预下单会员卡积分。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012595808（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`POST /v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/prepare`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡ID】 创建会员卡后获得的会员卡ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `code` | Path | `string` | 【会员卡code】 会员在card_id下的唯一标识（官方 path `code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`MemberCardBonusPrepareRequest` — 预下单会员卡积分请求体，字段见 `MemberCardBonusPrepareRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-open/cards/{card_id}/codes/{code}/bonus/prepare`；path `card_id`/`code`，body 3 键；官方应答节原文「无应答包体」且正常示例 204 ⇒ 裸 `Task`。

### 23. `UpdateMemberCardCallbackAsync` — 设置会员卡事件通知URL。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012552251（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`PATCH /v3/marketing/membercard-open/callback`

- **请求体**：`MemberCardCallbackRequest` — 设置会员卡事件通知URL请求体，字段见 `MemberCardCallbackRequest`。
- **返回**：`MemberCardCallbackResponse` — 设置会员卡事件通知URL，见 `MemberCardCallbackResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/membercard-open/callback`；body 1 键（`notify_url`），resp 2 键。
- **业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

### 24. `QueryMemberCardCallbackAsync` — 查询会员卡事件通知URL。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012552275（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/callback`
- **返回**：`MemberCardCallbackResponse` — 查询会员卡事件通知URL，见 `MemberCardCallbackResponse`（复用 PATCH 回调应答类型）。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/callback`；resp 2 键（`notify_url`/`mchid`）。
- **业务错误码**：401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

### 25. `QueryMemberCardsByUserAsync` — 查询用户在品牌下所有会员卡。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012549255（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs)
- **官方契约**：`GET /v3/marketing/membercard-open/user/cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 【用户openid】 用户在公众号下的openid（官方 query `openid`，**必填**，类型 `string(128)`）。 |
| `appid` | Query `appid` | `string?` | 【公众号APPID】 公众号APPID，用于筛选该公众号下领取的会员卡（官方 query `appid`，选填，类型 `string(18)`）。 |
| `offset` | Query `offset` | `int?` | 【分页起始位置】 分页起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，最大50（官方 query `limit`，选填，类型 `integer`）。 |
- **返回**：`MemberCardBrandUserCardsResponse` — 查询用户在品牌下所有会员卡，见 `MemberCardBrandUserCardsResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-open/user/cards`；query `openid`/`appid`/`offset`/`limit`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/MarketingMemberCardOpen`](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/) 内共 **93** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **93** 个，`SerializerClassName` = `MarketingMemberCardOpen`。

AOT 源生成上下文：[`MarketingMemberCardOpenJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/MarketingMemberCardOpenJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MemberCardTemplateCreateResponse` | 应答 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardTemplateCreateRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardTemplateListResponse` | 应答 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardTemplateResponse` | 应答 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardTemplateUpdateRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardTemplateUpgradeRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardTemplateDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardTemplateDtos.cs) | `brand_id`, `display_name`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `wait_days_after_receive`, `need_balance`, `balance_appid`, `balance_path`, `balance_url`, `name` … |
| `MemberCardCodeDepositResponse` | 应答 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardCodeDepositRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardUserCardResponse` | 应答 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardUserCardUpdateRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardCodeUnavailableRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardCodeConfirmResponse` | 应答 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardCodeConfirmRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardCodeDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCodeDtos.cs) | `code`, `result`, `data`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list`, `field_id`, `field_value`, `appid`, `path` … |
| `MemberCardUserImportResponse` | 应答 | [MarketingMemberCardOpen/MemberCardImportDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardImportDtos.cs) | `openid`, `code`, `out_request_no`, `encrypted_phone_number` |
| `MemberCardUserImportRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardImportDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardImportDtos.cs) | `openid`, `code`, `out_request_no`, `encrypted_phone_number` |
| `MemberCardPhoneImportResponse` | 应答 | [MarketingMemberCardOpen/MemberCardImportDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardImportDtos.cs) | `openid`, `code`, `out_request_no`, `encrypted_phone_number` |
| `MemberCardPhoneImportRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardImportDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardImportDtos.cs) | `openid`, `code`, `out_request_no`, `encrypted_phone_number` |
| `MemberCardQrcodeResponse` | 应答 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardQrcodeRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardDecryptCodeResponse` | 应答 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardActivateInformationResponse` | 应答 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardActivateRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardPermissionTokenResponse` | 应答 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardPermissionTokenRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardDeliveryDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardDeliveryDtos.cs) | `activate_type`, `activate_url`, `activate_appid`, `activate_path`, `outer_str`, `url`, `code`, `name`, `value`, `value_list`, `common_field_list`, `custom_field_list` … |
| `MemberCardUserCodeRightsRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardRightsDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardRightsDtos.cs) | `staff_name`, `head_image_url`, `contact_information_name`, `contact_information_value`, `phone_number`, `before_bonus_value`, `bonus_value`, `add_bonus_value`, `out_request_no`, `need_inform_bonus`, `member_price_word`, `fapiao_jump_word` … |
| `MemberCardTemplateRightsRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardRightsDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardRightsDtos.cs) | `staff_name`, `head_image_url`, `contact_information_name`, `contact_information_value`, `phone_number`, `before_bonus_value`, `bonus_value`, `add_bonus_value`, `out_request_no`, `need_inform_bonus`, `member_price_word`, `fapiao_jump_word` … |
| `MemberCardBonusCommitRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardRightsDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardRightsDtos.cs) | `staff_name`, `head_image_url`, `contact_information_name`, `contact_information_value`, `phone_number`, `before_bonus_value`, `bonus_value`, `add_bonus_value`, `out_request_no`, `need_inform_bonus`, `member_price_word`, `fapiao_jump_word` … |
| `MemberCardBonusPrepareRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardRightsDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardRightsDtos.cs) | `staff_name`, `head_image_url`, `contact_information_name`, `contact_information_value`, `phone_number`, `before_bonus_value`, `bonus_value`, `add_bonus_value`, `out_request_no`, `need_inform_bonus`, `member_price_word`, `fapiao_jump_word` … |
| `MemberCardCallbackResponse` | 应答 | [MarketingMemberCardOpen/MemberCardCallbackDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCallbackDtos.cs) | `notify_url`, `mchid` |
| `MemberCardCallbackRequest` | 请求体 | [MarketingMemberCardOpen/MemberCardCallbackDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardCallbackDtos.cs) | `notify_url`, `mchid` |
| `MemberCardBrandUserCardsResponse` | 应答 | [MarketingMemberCardOpen/MemberCardUserDtos.cs](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/MemberCardUserDtos.cs) | `card_id`, `code`, `data`, `total_count`, `offset`, `limit` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingMemberCardOpen));

var service = app.Services.GetRequiredService<IWechatPayMarketingMemberCardOpenService>();
var ct = CancellationToken.None;
var response = await service.QueryMemberCardCallbackAsync(ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/membercard-open/callback");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingMemberCardOpenService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryMemberCardCallbackAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingMemberCardOpenService](./IWechatPayMarketingMemberCardOpenService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingMemberCardOpenContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingMemberCardOpenContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MemberCardOpenTopology_ShouldBeOneInterfaceOnOneGroup`、`MemberCardOpenEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MemberCardOpenDtos_ShouldExposeOfficialFieldRosters`、`MemberCardOpenDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`MemberCardOpenBusinessCodeFields_ShouldHideBaseErrorEnvelope`、`MemberCardOpenRoutes_ShouldNotBeInZeroDeclarationList`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/MarketingMemberCardOpen`](../../../Mud.Wechat.Pay/MarketingMemberCardOpen/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingMemberCardOpenContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

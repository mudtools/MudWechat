# BrandCardMember 域 · 品牌会员卡与会员

微信支付「品牌会员卡与会员」域 SDK（15 端点）。 接口：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)；注册组 `BrandCardMember`；模块枚举 `PayModule.BrandCardMember`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBrandCardMemberContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardMemberContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BrandCardMember`](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBrandCardMemberService = 15；按 HTTP 特性实计 **15** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBrandCardMemberService` | `CreateBrandMemberCardAsync` | POST | `/v3/brand/partner/card-member/cards` | 商户证书 / APIv3 签名 | 会员卡模板创建。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336970（【普通服务商】）。 |
| 2 | `IWechatPayBrandCardMemberService` | `QueryBrandMemberCardsAsync` | GET | `/v3/brand/partner/card-member/cards` | 商户证书 / APIv3 签名 | 会员卡模板列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336976（【普通服务商】）。 |
| 3 | `IWechatPayBrandCardMemberService` | `QueryBrandMemberCardAsync` | GET | `/v3/brand/partner/card-member/cards/{cardId}` | 商户证书 / APIv3 签名 | 会员卡模板信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336974（【普通服务商】）。 |
| 4 | `IWechatPayBrandCardMemberService` | `UpdateBrandMemberCardAsync` | PATCH | `/v3/brand/partner/card-member/cards/{cardId}` | 商户证书 / APIv3 签名 | 会员卡模板修改。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336977（【普通服务商】）。 |
| 5 | `IWechatPayBrandCardMemberService` | `InvalidateBrandMemberCardAsync` | POST | `/v3/brand/partner/card-member/cards/{cardId}/invalidate` | 商户证书 / APIv3 签名 | 会员卡模板作废。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336972（【普通服务商】）。 |
| 6 | `IWechatPayBrandCardMemberService` | `CreateBrandMemberPreAuthTokenAsync` | POST | `/v3/brand/partner/card-member/pre-auth-tokens` | 商户证书 / APIv3 签名 | 会员卡预授权令牌发起。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336986（【普通服务商】）。 |
| 7 | `IWechatPayBrandCardMemberService` | `QueryBrandMemberUserCardAsync` | GET | `/v3/brand/partner/card-member/user-cards/{userCardCode}` | 商户证书 / APIv3 签名 | 用户会员卡信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336980（【普通服务商】）。 |
| 8 | `IWechatPayBrandCardMemberService` | `QueryBrandMemberUserCardsAsync` | GET | `/v3/brand/partner/card-member/user-cards` | 商户证书 / APIv3 签名 | 用户会员卡列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336984（【普通服务商】）。 |
| 9 | `IWechatPayBrandCardMemberService` | `UpdateBrandMemberUserCardAsync` | PATCH | `/v3/brand/partner/card-member/user-cards/{userCardCode}` | 商户证书 / APIv3 签名 | 用户会员卡信息修改。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336985（【普通服务商】）。 |
| 10 | `IWechatPayBrandCardMemberService` | `ConfirmBrandMemberUserCardAsync` | POST | `/v3/brand/partner/card-member/user-cards/{userCardCode}/confirm` | 商户证书 / APIv3 签名 | 会员开通结果同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336979（【普通服务商】）。 |
| 11 | `IWechatPayBrandCardMemberService` | `InvalidateBrandMemberUserCardAsync` | POST | `/v3/brand/partner/card-member/user-cards/{userCardCode}/invalidate` | 商户证书 / APIv3 签名 | 用户会员卡作废。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336983（【普通服务商】）。 |
| 12 | `IWechatPayBrandCardMemberService` | `ImportBrandMemberUserCardAsync` | POST | `/v3/brand/partner/card-member/user-cards/import-by-openid` | 商户证书 / APIv3 签名 | 按 OPENID 导入用户会员卡。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336981（【普通服务商】）。 |
| 13 | `IWechatPayBrandCardMemberService` | `CreateBrandMemberUserFeedAsync` | POST | `/v3/brand/partner/card-member/user-feeds` | 商户证书 / APIv3 签名 | 用户动态信息创建。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336987（【普通服务商】）。 |
| 14 | `IWechatPayBrandCardMemberService` | `ConfirmBrandMemberPointsCouponExchangeAsync` | POST | `/v3/brand/partner/card-member/user-points/exchange-coupon/confirm` | 商户证书 / APIv3 签名 | 积分兑券结果同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015897268（【普通服务商】）。 |
| 15 | `IWechatPayBrandCardMemberService` | `SyncBrandMemberPointsAsync` | POST | `/v3/brand/partner/card-member/user-points/sync` | 商户证书 / APIv3 签名 | 积分余额同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015897280（【普通服务商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页**（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 15 支官方页**全部未印**端点级 QPS 数字（个别页只提示「频率不足请联系技术支持评估」）
⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**两支列表的分页官方必填**：模板列表与用户卡列表都是 `offset`/`limit`，官方标**必填**
（与门店列表的**选填**、连接名片列表的 `page_index`/`page_size` 三形态并存 ⇒ 照页不得互套）；
过滤项 `state` / `user_card_state` 选填。应答总数字段官方为选填。

**`brand_id` 是本域的「商户作用域参数」**：本域凡官方印 `brand_id` 的支一律**必填 query**
⇒ 它不是可选过滤条件，缺省即 400；长度上限照页不做本地归一。

**模板与用户卡是两套生命周期**：`.../cards` 族管的是**会员卡模板**（创建 / 列表 / 查询 / PATCH 修改 / 作废）， `.../user-cards` 族管**用户持有的卡**（查询 / 列表 / PATCH 修改 / 开通结果确认 / 作废 / 按 OPENID 导入） ⇒ 两族的 ID 体系（`card_id` ↔ `user_card_code`）与应答字段**不同形**，不得互相套用类型。

**写动词不统一**：修改走 **PATCH**，作废与确认走 **POST .../invalidate**、**POST .../confirm** ⇒ 照页不得合并。

**应答同形族的共用裁定**（含 req 标记的签名比对）：模板四支（创建 / 查询 / 修改 / 作废）应答**逐字段同形**（49 行） ⇒ 共用 `BrandMemberCardResponse`，而**列表支是 53 行** ⇒ 单建； 用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）同形（28 行）⇒ 共用 `BrandMemberUserCardResponse`， 而**列表支 32 行** ⇒ 单建。

**请求侧不因应答同形而共用**：模板创建（45 行）与 PATCH 修改的**必填口径不同**、 用户卡的 PATCH 修改 / 确认 / 导入三支互异 ⇒ 各建独立请求类型，只有**应答**侧才共用。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 15 次）**支持商户**【普通服务商】。
- （出现 14 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**应答**见 `BrandMemberUserCardResponse`（官方应答表逐字段建模，与 `GET /brand/partner/card-member/user-cards/{user_card_code}` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。

### 1. `CreateBrandMemberCardAsync` — 会员卡模板创建。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336970（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/cards`

- **请求体**：`BrandMemberCardCreateRequest` — 会员卡模板创建请求体，字段见 `BrandMemberCardCreateRequest`。
- **返回**：`BrandMemberCardResponse` — 会员卡模板创建，见 `BrandMemberCardResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/cards`；无 path / query 参数。
- **应答**见 `BrandMemberCardResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 是否展示会员等级为 true，必填会员初始等级信息 / 400 PARAM_ERROR 是否展示会员等级为 false，不允许填写会员初始等级信息 / 400 PARAM_ERROR 是否支持用户主动入会为 true 时，必须填写会员权益、回调地址、商家法务协议 / 400 PARAM_ERROR 会员码型为跳转商家小程序，必填会员码跳转信息 / 400 PARAM_ERROR 会员码型不是跳转商家小程序，不允许填写会员码跳转信息 / 400 PARAM_ERROR 非储值会员卡，不允许填写储值小程序信息 / 400 PARAM_ERROR 非付费会员卡，不允许填写付费小程序信息 / 400 PARAM_ERROR 付费会员价格必须大于 0 / 400 PARAM_ERROR 商家请求单号重复 / 400 PARAM_ERROR AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员中心跳转AppID 无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员码跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 积分跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 储值跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 付费跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST 会员卡模板数量超过限制，每个类型最多创建两个模板 / 400 PARAM_ERROR 卡名称太长，支持最长10个中文字、英文字符、标点 / 400 PARAM_ERROR 卡名称仅支持中英文字符、数字和标点 / 400 INVALID_REQUEST 暂不支持付费或储值的会员卡类型，请修改后重试 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 2. `QueryBrandMemberCardsAsync` — 会员卡模板列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336976（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`GET /v3/brand/partner/card-member/cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query `brand_id`，**必填**，类型 `string(32)`）。 |
| `offset` | Query `offset` | `int` | 【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，范围为1~20（官方 query `limit`，**必填**，类型 `integer`）。 |
| `state` | Query `state` | `string?` | 【状态】 会员卡状态信息。可选取值：`CARD_EFFECTIVE` 生效中 / `CARD_INVALID` 已失效（官方 query `state`，选填，类型 `string`）。 |
- **返回**：`BrandMemberCardListResponse` — 会员卡模板列表，见 `BrandMemberCardListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/partner/card-member/cards`；query `brand_id` / `offset` / `limit` / `state`。
- **`brand_id` 与 `offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 3. `QueryBrandMemberCardAsync` — 会员卡模板信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336974（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`GET /v3/brand/partner/card-member/cards/{cardId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query `brand_id`，**必填**，类型 `string(32)`）。 |
- **返回**：`BrandMemberCardResponse` — 会员卡模板信息，见 `BrandMemberCardResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/partner/card-member/cards/{card_id}`；path `card_id`，query `brand_id`。
- **`brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 4. `UpdateBrandMemberCardAsync` — 会员卡模板修改。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336977（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`PATCH /v3/brand/partner/card-member/cards/{cardId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`BrandMemberCardUpdateRequest` — 会员卡模板修改请求体，字段见 `BrandMemberCardUpdateRequest`。
- **返回**：`BrandMemberCardResponse` — 会员卡模板修改，见 `BrandMemberCardResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/brand/partner/card-member/cards/{card_id}`；path `card_id`。
- **应答**见 `BrandMemberCardResponse`（官方应答表逐字段建模，与 `POST /brand/partner/card-member/cards` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 会员中心跳转AppID 无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员码跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 积分跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 储值跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 付费跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR 会员码型不是跳转商家小程序，不支持修改跳转信息 / 400 PARAM_ERROR 非储值会员卡，不允许填写储值小程序信息 / 400 PARAM_ERROR 非付费会员卡，不允许填写付费小程序信息 / 400 PARAM_ERROR 付费会员价格必须大于 0 / 400 PARAM_ERROR 卡名称太长，支持最长10个中文字、英文字符、标点 / 400 PARAM_ERROR 卡名称仅支持中英文字符、数字和标点 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 会员中心跳转 AppID 或路径为空 / 400 PARAM_ERROR 会员码跳转 AppID 或路径为空 / 400 PARAM_ERROR 积分跳转 AppID 或路径为空 / 400 PARAM_ERROR 储值跳转 AppID 或路径为空 / 400 PARAM_ERROR 付费跳转 AppID 或路径为空 / 400 PARAM_ERROR 修改后是否支持用户主动入会为 true 时，会员权益、回调地址、商家法务协议必须已填写或在本次补齐 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 5. `InvalidateBrandMemberCardAsync` — 会员卡模板作废。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336972（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/cards/{cardId}/invalidate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `cardId` | Path | `string` | 【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path `card_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`BrandMemberCardInvalidateRequest` — 会员卡模板作废请求体，字段见 `BrandMemberCardInvalidateRequest`。
- **返回**：`BrandMemberCardResponse` — 会员卡模板作废，见 `BrandMemberCardResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/cards/{card_id}/invalidate`；path `card_id`。
- **应答**见 `BrandMemberCardResponse`（官方应答表逐字段建模，与 `POST /brand/partner/card-member/cards` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 6. `CreateBrandMemberPreAuthTokenAsync` — 会员卡预授权令牌发起。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336986（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/pre-auth-tokens`

- **请求体**：`BrandMemberPreAuthTokenRequest` — 会员卡预授权令牌发起请求体，字段见 `BrandMemberPreAuthTokenRequest`。
- **返回**：`BrandMemberPreAuthTokenResponse` — 会员卡预授权令牌发起，见 `BrandMemberPreAuthTokenResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/pre-auth-tokens`；无 path / query 参数。
- **应答**见 `BrandMemberPreAuthTokenResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 功能暂未开放 / 400 INVALID_REQUEST 会员卡模板无效 / 400 INVALID_REQUEST 会员卡模板已过期 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 用户在一个卡模板下只能有一张有效的会员卡 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 7. `QueryBrandMemberUserCardAsync` — 用户会员卡信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336980（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`GET /v3/brand/partner/card-member/user-cards/{userCardCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userCardCode` | Path | `string` | 【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path `user_card_code`，**必填**，类型 `string(32)`）。 |
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query `brand_id`，**必填**，类型 `string(32)`）。 |
| `cardId` | Query `card_id` | `string` | 【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 query `card_id`，**必填**，类型 `string(32)`）。 |
| `openid` | Query `openid` | `string` | 【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明。（官方 query `openid`，**必填**，类型 `string(128)`）。 |
- **返回**：`BrandMemberUserCardResponse` — 用户会员卡信息，见 `BrandMemberUserCardResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/partner/card-member/user-cards/{user_card_code}`；path `user_card_code`，query `brand_id` / `card_id` / `openid`。
- **`brand_id` 与 `card_id` 与 `openid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 8. `QueryBrandMemberUserCardsAsync` — 用户会员卡列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336984（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`GET /v3/brand/partner/card-member/user-cards`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query `brand_id`，**必填**，类型 `string(32)`）。 |
| `openid` | Query `openid` | `string` | 【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明。（官方 query `openid`，**必填**，类型 `string(128)`）。 |
| `offset` | Query `offset` | `int` | 【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。（官方 query `offset`，**必填**，类型 `integer`）。 |
| `limit` | Query `limit` | `int` | 【分页大小】 分页大小，范围为1~20（官方 query `limit`，**必填**，类型 `integer`）。 |
| `userCardState` | Query `user_card_state` | `string?` | 【用户会员卡状态】 用户当前的卡状态。可选取值：`UNACTIVATED` 用户已领卡，但还未激活 / `EFFECTIVE` 用户的会员卡可正常使用 / `EXPIRED` 用户的会员卡已过期 / `INVALID` 用户的会员卡已失效（官方 query `user_card_state`，选填，类型 `string`）。 |
- **返回**：`BrandMemberUserCardListResponse` — 用户会员卡列表，见 `BrandMemberUserCardListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/partner/card-member/user-cards`；query `brand_id` / `openid` / `offset` / `limit` / `user_card_state`。
- **`brand_id` 与 `openid` 与 `offset` 与 `limit` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 9. `UpdateBrandMemberUserCardAsync` — 用户会员卡信息修改。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336985（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`PATCH /v3/brand/partner/card-member/user-cards/{userCardCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userCardCode` | Path | `string` | 【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path `user_card_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`BrandMemberUserCardUpdateRequest` — 用户会员卡信息修改请求体，字段见 `BrandMemberUserCardUpdateRequest`。
- **返回**：`BrandMemberUserCardResponse` — 用户会员卡信息修改，见 `BrandMemberUserCardResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/brand/partner/card-member/user-cards/{user_card_code}`；path `user_card_code`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 400 INVALID_REQUEST 用户卡状态不正确，只能操作已激活的会员卡 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 请求的用户信息通用字段未定义在卡模版中 / 400 PARAM_ERROR 用户信息通用字段中，生日格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，邮箱格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，性别格式不正确 / 400 PARAM_ERROR 请求的用户信息自定义字段未定义在卡模版中 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 10. `ConfirmBrandMemberUserCardAsync` — 会员开通结果同步。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336979（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-cards/{userCardCode}/confirm`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userCardCode` | Path | `string` | 【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path `user_card_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`BrandMemberUserCardConfirmRequest` — 会员开通结果同步请求体，字段见 `BrandMemberUserCardConfirmRequest`。
- **返回**：`BrandMemberUserCardResponse` — 会员开通结果同步，见 `BrandMemberUserCardResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-cards/{user_card_code}/confirm`；path `user_card_code`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 用户卡code 无效，只能包含数字、英文、半角标点 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 404 NOT_FOUND 用户不存在未激活的会员卡 / 400 INVALID_REQUEST 该用户卡 code 已存在 / 400 INVALID_REQUEST 会员卡号分配方式为系统分配，不允许修改会员卡号 / 400 INVALID_REQUEST 用户卡状态不正确，只能操作已激活的会员卡 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 请求的用户信息通用字段未定义在卡模版中 / 400 PARAM_ERROR 用户信息通用字段中，生日格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，邮箱格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，性别格式不正确 / 400 PARAM_ERROR 请求的用户信息自定义字段未定义在卡模版中。

### 11. `InvalidateBrandMemberUserCardAsync` — 用户会员卡作废。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336983（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-cards/{userCardCode}/invalidate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userCardCode` | Path | `string` | 【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path `user_card_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`BrandMemberUserCardInvalidateRequest` — 用户会员卡作废请求体，字段见 `BrandMemberUserCardInvalidateRequest`。
- **返回**：`BrandMemberUserCardResponse` — 用户会员卡作废，见 `BrandMemberUserCardResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-cards/{user_card_code}/invalidate`；path `user_card_code`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 用户卡状态不正确，只能作废待激活或已生效的会员卡 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR OpenID有误 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 12. `ImportBrandMemberUserCardAsync` — 按 OPENID 导入用户会员卡。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336981（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-cards/import-by-openid`

- **请求体**：`BrandMemberUserCardImportRequest` — 按 OPENID 导入用户会员卡请求体，字段见 `BrandMemberUserCardImportRequest`。
- **返回**：`BrandMemberUserCardResponse` — 按 OPENID 导入用户会员卡，见 `BrandMemberUserCardResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-cards/import-by-openid`；无 path / query 参数。
- **频率限制**（官方原样）：（2）若当前接口的频率限制无法满足业务需要，请联系微信支付技术支持评估。。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 用户卡code 无效，只能包含数字、英文、半角标点 / 400 PARAM_ERROR 固定时间范围必填领取时间 / 400 INVALID_REQUEST 无法导入已过期的会员卡 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 用户已有待激活的会员卡 / 400 INVALID_REQUEST 用户已有已生效的会员卡 / 400 INVALID_REQUEST 用户主动删除了该会员卡，无法重新导入 / 400 INVALID_REQUEST 该用户卡 code 已存在 / 400 INVALID_REQUEST 会员卡模板无效 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。

### 13. `CreateBrandMemberUserFeedAsync` — 用户动态信息创建。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015336987（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-feeds`

- **请求体**：`BrandMemberUserFeedCreateRequest` — 用户动态信息创建请求体，字段见 `BrandMemberUserFeedCreateRequest`。
- **返回**：`BrandMemberUserFeedCreateResponse` — 用户动态信息创建，见 `BrandMemberUserFeedCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-feeds`；无 path / query 参数。
- **应答**见 `BrandMemberUserFeedCreateResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商家没有权限使用该动态信息模板 / 400 INVALID_REQUEST 当前用户非会员 / 400 INVALID_REQUEST 会员卡模板无效 / 400 PARAM_ERROR cell不合法 / 400 ALREADY_EXISTS 该用户动态已存在 / 400 INVALID_REQUEST 品牌未开通名片会员 / 400 INVALID_REQUEST openid无效。

### 14. `ConfirmBrandMemberPointsCouponExchangeAsync` — 积分兑券结果同步。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015897268（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-points/exchange-coupon/confirm`

- **请求体**：`BrandMemberPointsCouponConfirmRequest` — 积分兑券结果同步请求体，字段见 `BrandMemberPointsCouponConfirmRequest`。
- **返回**：`BrandMemberPointsCouponConfirmResponse` — 积分兑券结果同步，见 `BrandMemberPointsCouponConfirmResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-points/exchange-coupon/confirm`；无 path / query 参数。
- **应答**见 `BrandMemberPointsCouponConfirmResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 积分兑券记录不存在。

### 15. `SyncBrandMemberPointsAsync` — 积分余额同步。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015897280（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs)
- **官方契约**：`POST /v3/brand/partner/card-member/user-points/sync`

- **请求体**：`BrandMemberPointsSyncRequest` — 积分余额同步请求体，字段见 `BrandMemberPointsSyncRequest`。
- **返回**：`BrandMemberPointsSyncResponse` — 积分余额同步，见 `BrandMemberPointsSyncResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/partner/card-member/user-points/sync`；无 path / query 参数。
- **应答**见 `BrandMemberPointsSyncResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BrandCardMember`](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/) 内共 **73** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **73** 个，`SerializerClassName` = `BrandCardMember`。

AOT 源生成上下文：[`BrandCardMemberJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BrandCardMemberJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BrandMemberCardResponse` | 应答 | [BrandCardMember/BrandMemberCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberCardDtos.cs) | `jump_appid`, `jump_path`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `price`, `name`, `values`, `common_field_list`, `custom_field_list`, `out_request_no` … |
| `BrandMemberCardCreateRequest` | 请求体 | [BrandCardMember/BrandMemberCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberCardDtos.cs) | `jump_appid`, `jump_path`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `price`, `name`, `values`, `common_field_list`, `custom_field_list`, `out_request_no` … |
| `BrandMemberCardListResponse` | 应答 | [BrandCardMember/BrandMemberCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberCardDtos.cs) | `jump_appid`, `jump_path`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `price`, `name`, `values`, `common_field_list`, `custom_field_list`, `out_request_no` … |
| `BrandMemberCardUpdateRequest` | 请求体 | [BrandCardMember/BrandMemberCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberCardDtos.cs) | `jump_appid`, `jump_path`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `price`, `name`, `values`, `common_field_list`, `custom_field_list`, `out_request_no` … |
| `BrandMemberCardInvalidateRequest` | 请求体 | [BrandCardMember/BrandMemberCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberCardDtos.cs) | `jump_appid`, `jump_path`, `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `price`, `name`, `values`, `common_field_list`, `custom_field_list`, `out_request_no` … |
| `BrandMemberPreAuthTokenResponse` | 应答 | [BrandCardMember/BrandMemberPreAuthDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberPreAuthDtos.cs) | `brand_id`, `card_id`, `openid`, `token`, `expire_time` |
| `BrandMemberPreAuthTokenRequest` | 请求体 | [BrandCardMember/BrandMemberPreAuthDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberPreAuthDtos.cs) | `brand_id`, `card_id`, `openid`, `token`, `expire_time` |
| `BrandMemberUserCardResponse` | 应答 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserCardListResponse` | 应答 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserCardUpdateRequest` | 请求体 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserCardConfirmRequest` | 请求体 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserCardInvalidateRequest` | 请求体 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserCardImportRequest` | 请求体 | [BrandCardMember/BrandMemberUserCardDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserCardDtos.cs) | `type`, `available_begin_time`, `available_end_time`, `available_day_after_receive`, `name`, `value`, `user_chosen_values`, `common_field_list`, `custom_field_list`, `user_card_code`, `card_id`, `openid` … |
| `BrandMemberUserFeedCreateResponse` | 应答 | [BrandCardMember/BrandMemberUserFeedDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserFeedDtos.cs) | `brand_id`, `card_id`, `user_card_code`, `openid`, `out_request_no`, `cell` |
| `BrandMemberUserFeedCreateRequest` | 请求体 | [BrandCardMember/BrandMemberUserFeedDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserFeedDtos.cs) | `brand_id`, `card_id`, `user_card_code`, `openid`, `out_request_no`, `cell` |
| `BrandMemberPointsCouponConfirmResponse` | 应答 | [BrandCardMember/BrandMemberUserPointsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserPointsDtos.cs) | `record_id`, `exchange_coupon_template_id`, `brand_id`, `card_id`, `openid`, `user_card_code`, `result`, `reject_reason`, `state`, `product_coupon_id`, `product_coupon_stock_type`, `stock_id` … |
| `BrandMemberPointsCouponConfirmRequest` | 请求体 | [BrandCardMember/BrandMemberUserPointsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserPointsDtos.cs) | `record_id`, `exchange_coupon_template_id`, `brand_id`, `card_id`, `openid`, `user_card_code`, `result`, `reject_reason`, `state`, `product_coupon_id`, `product_coupon_stock_type`, `stock_id` … |
| `BrandMemberPointsSyncResponse` | 应答 | [BrandCardMember/BrandMemberUserPointsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserPointsDtos.cs) | `record_id`, `exchange_coupon_template_id`, `brand_id`, `card_id`, `openid`, `user_card_code`, `result`, `reject_reason`, `state`, `product_coupon_id`, `product_coupon_stock_type`, `stock_id` … |
| `BrandMemberPointsSyncRequest` | 请求体 | [BrandCardMember/BrandMemberUserPointsDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/BrandMemberUserPointsDtos.cs) | `record_id`, `exchange_coupon_template_id`, `brand_id`, `card_id`, `openid`, `user_card_code`, `result`, `reject_reason`, `state`, `product_coupon_id`, `product_coupon_stock_type`, `stock_id` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BrandCardMember;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandCardMember));

var service = app.Services.GetRequiredService<IWechatPayBrandCardMemberService>();
var ct = CancellationToken.None;
var response = await service.ConfirmBrandMemberPointsCouponExchangeAsync(new BrandMemberPointsCouponConfirmRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/brand/partner/card-member/user-points/exchange-coupon/confirm");
// 占位值仅用于编译示意；业务值须按 IWechatPayBrandCardMemberService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ConfirmBrandMemberPointsCouponExchangeAsync`）；请求体 `BrandMemberPointsCouponConfirmRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBrandCardMemberService](./IWechatPayBrandCardMemberService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBrandCardMemberContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardMemberContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs) |
| 守卫中的代表性测试方法 | `BrandCardMemberTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandCardMemberEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BrandCardMemberDtos_ShouldExposeOfficialFieldRosters`、`BrandCardMemberDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource`、`PositiveControl_AlreadyDeclaredRoutes_ShouldBeFound` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BrandCardMember`](../../../Mud.Wechat.Pay.DataModels/BrandCardMember/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBrandCardMemberContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

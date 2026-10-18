# Capital 域 · 银行组件（省市 / 银行 / 支行 / 卡号开户行查询）

微信支付「银行组件（省市 / 银行 / 支行 / 卡号开户行查询）」域 SDK（6 端点）。 接口：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)；注册组 `Capital`；模块枚举 `PayModule.Capital`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayCapitalContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)、[WechatPayCapitalRulingContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalRulingContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/Capital`](../../../Mud.Wechat.Pay.DataModels/Capital/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayCapitalService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayCapitalService` | `QueryCapitalProvincesAsync` | GET | `/v3/capital/capitallhh/areas/provinces` | 商户证书 / APIv3 签名 | 查询省份列表（不包含中国港澳台地区）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698641（【普通服务商】）。 |
| 2 | `IWechatPayCapitalService` | `QueryCapitalCitiesAsync` | GET | `/v3/capital/capitallhh/areas/provinces/{provinceCode}/cities` | 商户证书 / APIv3 签名 | 根据省份编码查询该省下的城市列表（不包含中国港澳台地区城市）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697470（【… |
| 3 | `IWechatPayCapitalService` | `QueryCapitalCorporateBanksAsync` | GET | `/v3/capital/capitallhh/banks/corporate-banking` | 商户证书 / APIv3 签名 | 查询支持对公业务的银行列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697626（【普通服务商】）。 |
| 4 | `IWechatPayCapitalService` | `QueryCapitalPersonalBanksAsync` | GET | `/v3/capital/capitallhh/banks/personal-banking` | 商户证书 / APIv3 签名 | 查询支持个人业务的银行列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697583（【普通服务商】）。 |
| 5 | `IWechatPayCapitalService` | `QueryCapitalBranchesAsync` | GET | `/v3/capital/capitallhh/banks/{bankAliasCode}/branches` | 商户证书 / APIv3 签名 | 根据银行别名编码与城市编码查询支行列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697673（【普通服务商】）。 |
| 6 | `IWechatPayCapitalService` | `SearchCapitalBanksByBankAccountAsync` | GET | `/v3/capital/capitallhh/banks/search-banks-by-bank-account` | 商户证书 / APIv3 签名 | 根据对私银行卡号查询开户银行信息（仅支持部分银行的对私银行卡）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697541（… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中**6 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**六支官方全部只印服务商页**（「支持商户：【普通服务商】」，路由属 `/v3/partner` 文档树的「银行组件」章节）⇒ 无商户面镜像，**不得**按其余行业域的双页形态补建。

**频率限制是逐支印出的**：六支官方正文均写「API按照接口-商户级别限频，单商户单接口最大请求频率不超过**300TPS**」⇒ 数字照页入各方法 remarks，本域不属于「官方未印 QPS」那一类。

**本域是纯查询字典族 ⇒ 官方要求前端缓存**：省份 / 城市 / 银行 / 支行四支官方正文均写「如需检索，请在前端缓存所有XX数据并自行完成检索功能」 「业务查询到的数据为后端缓存快照，业务方需要**定期淘汰并更新数据**」⇒ 不得把本域当实时业务接口高频调用；SDK **不做**任何本地缓存。

**翻页形态两支两支不同**：省份 / 城市两支官方**无请求参数**（返回全量 + `total_count`）；银行两支与支行支是「`limit` **必填** + `offset` 选填」⇒ `limit` 做成必传参数，不给默认值。

**共用只发生在应答根类型**：对公与对私两支的官方应答表逐字段同形 ⇒ 共用 `CapitalBankPageResponse`； 支行支与卡号开户行支各建一型（前者根层另印四个银行定位键，后者无 `count` / `offset` / `links` 骨架），见守卫 CAP-B4。

**涉密面是逐页核对结论**：本接口 6 支里**1 支**（获取对私银行卡号开户银行）的官方请求头表把 `Wechatpay-Serial` 列为**必填**且 `account_number` 官方注明**需加密上送**，其余五支只列 `Authorization` / `Accept` ⇒ 不得按「同域一律涉密」补齐。声明式签名一律不承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径），密文以 `string` 承载、SDK **不代为加解密**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**支持商户**【普通服务商】。
- （出现 6 次）**频率限制**（官方原样）：API按照接口-商户级别限频，单商户单接口最大请求频率不超过300TPS。
- （出现 3 次）**业务错误码**：404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。

### 1. `QueryCapitalProvincesAsync` — 查询省份列表（不包含中国港澳台地区）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698641（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/areas/provinces`
- **返回**：`CapitalProvinceListResponse` — 查询省份列表，见 `CapitalProvinceListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/areas/provinces`；**无路径参数、无查询参数**（官方请求头只列 `Authorization` / `Accept`）。
- **官方检索口径**：原文「如需检索，请在前端缓存所有省份数据并自行完成检索功能」⇒ 本支返回**全量**省份，无分页参数。

### 2. `QueryCapitalCitiesAsync` — 根据省份编码查询该省下的城市列表（不包含中国港澳台地区城市）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697470（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/areas/provinces/{provinceCode}/cities`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `provinceCode` | Path | `long` | 【省份编码】 省份编码，唯一标识一个省份（官方 path `province_code`，**必填**，类型 `integer`），取值来自「查询省份列表」支。 |
- **返回**：`CapitalCityListResponse` — 查询城市列表，见 `CapitalCityListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/areas/provinces/{province_code}/cities`；path `province_code`，**无查询参数**。
- **官方用途口径**：原文「通过本接口根据省份编码获取省份下的城市列表信息，不包含中国港澳台地区城市信息，可用于支行数据过滤查询」 ⇒ 本支是「查询支行列表」的前置定位支。

### 3. `QueryCapitalCorporateBanksAsync` — 查询支持对公业务的银行列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697626（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/banks/corporate-banking`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `limit` | Query `limit` | `int` | 【最大资源条数】 该次请求可返回的最大资源条数（官方 query `limit`，**必填**，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
- **返回**：`CapitalBankPageResponse` — 查询支持对公业务的银行列表，见 `CapitalBankPageResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/banks/corporate-banking`；query `offset`（选填）/ `limit`（**必填**）。
- **与「查询支持个人业务的银行列表」应答同形**：两支官方应答表逐字段一致 ⇒ 共用 `CapitalBankPageResponse`（守卫 CAP-B4），**但两支是不同业务开放面**（对公账户 vs 对私银行卡），不得并成一支方法。
- **官方数据时效口径**：原文「业务查询到的数据为后端缓存快照，业务方需要定期淘汰并更新数据」⇒ 结果不可当实时数据使用。

### 4. `QueryCapitalPersonalBanksAsync` — 查询支持个人业务的银行列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697583（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/banks/personal-banking`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `limit` | Query `limit` | `int` | 【最大资源条数】 该次请求可返回的最大资源条数（官方 query `limit`，**必填**，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
- **返回**：`CapitalBankPageResponse` — 查询支持个人业务的银行列表，见 `CapitalBankPageResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/banks/personal-banking`；query `offset`（选填）/ `limit`（**必填**）。
- **官方路由是 `personal-banking` 而非 `private-banking`**：正文标题「查询支持**个人**业务的银行列表」⇒ 照页不得按「对公 / 对私」的直觉改成 `private-banking`。
- **业务错误码**：400 INVALID_REQUEST：银行卡非储蓄卡 / 查询结果超限 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。

### 5. `QueryCapitalBranchesAsync` — 根据银行别名编码与城市编码查询支行列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697673（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/banks/{bankAliasCode}/branches`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `bankAliasCode` | Path | `string` | 【银行别名编码】 银行别名的编码，与 bank_alias 一一对应（官方 path `bank_alias_code`，**必填**，类型 `string(32)`）。 |
| `cityCode` | Query `city_code` | `int` | 【城市编码】 城市编码，唯一标识一个城市（官方 query `city_code`，**必填**，类型 `integer`），取值来自「查询城市列表」支。 |
| `limit` | Query `limit` | `int` | 【最大资源条数】 该次请求可返回的最大资源条数（官方 query `limit`，**必填**，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【请求资源起始位置】 该次请求资源的起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
- **返回**：`CapitalBranchPageResponse` — 查询支行列表，见 `CapitalBranchPageResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/banks/{bank_alias_code}/branches`；path `bank_alias_code`，query `city_code`（**必填**）/ `offset`（选填）/ `limit`（**必填**）。
- **只对「需要填写支行的银行别名」开放**：官方原文「本接口可以用于根据银行别名编码（**仅支持需要填写支行的银行别名编码**）和城市编码过滤查询支行列表数据」 ⇒ 是否需填支行由银行列表支的 `need_bank_branch` 判别，不满足的编码调用即失败。
- **应答根层另印四个银行定位键**（`account_bank` / `account_bank_code` / `bank_alias` / `bank_alias_code`）⇒ 照页建模进 `CapitalBranchPageResponse`，不得判为官方笔误删除。
- **业务错误码**：400 INVALID_REQUEST：银行别名编码非数字格式 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。

### 6. `SearchCapitalBanksByBankAccountAsync` — 根据对私银行卡号查询开户银行信息（仅支持部分银行的对私银行卡）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697541（【普通服务商】）。

- **声明接口**：[IWechatPayCapitalService](./IWechatPayCapitalService.cs)
- **官方契约**：`GET /v3/capital/capitallhh/banks/search-banks-by-bank-account`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accountNumber` | Query `account_number` | `string` | 【银行卡号】 对私银行卡号，官方类型为 `string(512)`（官方 query `account_number`，**必填**）：**须以微信支付公钥或平台证书公钥加密后上送**，本参数承载**密文**，SDK 不代为加解密。 |
- **返回**：`CapitalBankByAccountResponse` — 获取对私银行卡号开户银行，见 `CapitalBankByAccountResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/capital/capitallhh/banks/search-banks-by-bank-account`；query `account_number`（**必填**，`string(512)` 密文）。
- **必须带 `Wechatpay-Serial`**：官方请求头表把该头列为**必填**，且 `account_number` 官方注明需加密上送 ⇒ 本域唯一涉密支（其余五支只列 `Authorization` / `Accept`）。声明式签名**不**承载该头（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；密文以 `string` 承载，SDK **不代为加解密**。
- **本支 Query 凭据之外的敏感参数在 URL 上**：`account_number` 是密文仍属敏感串 ⇒ 宿主日志与错误上报须按脱敏口径处理请求 URI，不得原样落日志。
- **业务错误码**：400 PARAM_ERROR：参数格式错误 / 400 INVALID_REQUEST：无效请求 / 404 NOT_FOUND：查询结果为空 / 429 FREQUENCY_LIMITED：接口调用频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/Capital`](../../../Mud.Wechat.Pay.DataModels/Capital/) 内共 **12** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **12** 个，`SerializerClassName` = `Capital`。

AOT 源生成上下文：[`CapitalJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/CapitalJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CapitalProvinceListResponse` | 应答 | [Capital/CapitalAreaDtos.cs](../../../Mud.Wechat.Pay.DataModels/Capital/CapitalAreaDtos.cs) | `data`, `total_count`, `province_name`, `province_code`, `city_name`, `city_code` |
| `CapitalCityListResponse` | 应答 | [Capital/CapitalAreaDtos.cs](../../../Mud.Wechat.Pay.DataModels/Capital/CapitalAreaDtos.cs) | `data`, `total_count`, `province_name`, `province_code`, `city_name`, `city_code` |
| `CapitalBankPageResponse` | 应答 | [Capital/CapitalBankDtos.cs](../../../Mud.Wechat.Pay.DataModels/Capital/CapitalBankDtos.cs) | `data`, `count`, `offset`, `links`, `total_count`, `bank_alias`, `bank_alias_code`, `account_bank`, `account_bank_code`, `need_bank_branch`, `next`, `prev` … |
| `CapitalBranchPageResponse` | 应答 | [Capital/CapitalBankDtos.cs](../../../Mud.Wechat.Pay.DataModels/Capital/CapitalBankDtos.cs) | `data`, `count`, `offset`, `links`, `total_count`, `bank_alias`, `bank_alias_code`, `account_bank`, `account_bank_code`, `need_bank_branch`, `next`, `prev` … |
| `CapitalBankByAccountResponse` | 应答 | [Capital/CapitalBankDtos.cs](../../../Mud.Wechat.Pay.DataModels/Capital/CapitalBankDtos.cs) | `data`, `count`, `offset`, `links`, `total_count`, `bank_alias`, `bank_alias_code`, `account_bank`, `account_bank_code`, `need_bank_branch`, `next`, `prev` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Capital;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Capital));

var service = app.Services.GetRequiredService<IWechatPayCapitalService>();
var ct = CancellationToken.None;
var response = await service.QueryCapitalProvincesAsync(ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/capital/capitallhh/areas/provinces");
// 占位值仅用于编译示意；业务值须按 IWechatPayCapitalService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryCapitalProvincesAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayCapitalService](./IWechatPayCapitalService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayCapitalContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)、[`WechatPayCapitalRulingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCapitalRulingContractGuards.cs) |
| 守卫中的代表性测试方法 | `CapitalTopology_ShouldMatchInterfaceAndGroupRoster`、`CapitalEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`CapitalDtos_ShouldExposeOfficialFieldRosters`、`CapitalDataModels_ShouldSplitRequestAndResponseBaseTypes`、`CapitalAllDtos_ShouldBeAnnotatedAndContextRegistered`、`CapitalRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/Capital`](../../../Mud.Wechat.Pay.DataModels/Capital/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayCapitalContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`、`WechatPayCapitalRulingContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

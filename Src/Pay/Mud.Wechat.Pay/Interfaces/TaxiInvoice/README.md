# TaxiInvoice 域 · 出租车电子发票（司机/车辆/经营主体建档与开票）

微信支付 APIv3「出租车电子发票」域：**9 支声明式端点** + **1 支 multipart 手写上传通道**
（`/v3/taxi-invoice/cards/upload-file`，**不计入**声明式端点数）。
接口 [IWechatPayTaxiInvoiceService.cs](./IWechatPayTaxiInvoiceService.cs) 与手写通道
[IWechatPayTaxiInvoiceFileService.cs](../../TaxiInvoice/IWechatPayTaxiInvoiceFileService.cs)（实现
[WechatPayTaxiInvoiceFileService.cs](../../TaxiInvoice/WechatPayTaxiInvoiceFileService.cs)），注册组 `TaxiInvoice`，
模块枚举 `PayModule.TaxiInvoice`（装配入口 `AddWechatPayApi(b => b.AddModules(PayModule.TaxiInvoice))`，
内部调用源生成的 `AddTaxiInvoiceWebApiHttpClient()`，并 `TryAddSingleton<IWechatPayTaxiInvoiceFileService,
WechatPayTaxiInvoiceFileService>()` 同批装配手写通道，见
[../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayTaxiInvoiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayTaxiInvoiceContractGuards.cs)
> （`TXI-B1`~`TXI-B6`）与本域 DTO（`../../../Mud.Wechat.Pay.DataModels/TaxiInvoice/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是本域守卫 `TXI-B1`：`Methods().Should().HaveCount(9)` **且** `DistinctVerbRoutes().Should().HaveCount(9)`，
并**显式断言**路由集合 `NotContain("/v3/taxi-invoice/cards/upload-file")` —— 上传通道刻意不在声明式面里。
正对照取 `IWechatPayMerchantFundService`（动词集合 `{GetAttribute}`）。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `PunchTaxiInvoiceAttendanceAsync` | POST | `/v3/taxi-invoice/attendance/punch` | 商户证书 / APIv3 请求签名【普通服务商】 | 成功 `204 No Content` ⇒ 裸 `Task` |
| 2 | `InsertTaxiInvoiceCardAsync` | POST | `/v3/taxi-invoice/cards` | 同上 | 成功 `204 No Content` ⇒ 裸 `Task`；依赖上传通道给的 `fapiao_media_id` |
| 3 | `UpdateTaxiInvoiceDriverAsync` | POST | `/v3/taxi-invoice/driver/update-driver` | 同上；**另需 `Wechatpay-Serial`** | 本域**唯一**涉密支 |
| 4 | `QueryTaxiInvoiceDriverAsync` | GET | `/v3/taxi-invoice/drivers/{driverLicense}` | 商户证书 / APIv3 请求签名 | `region_id` **必填 query** |
| 5 | `UpdateTaxiInvoiceTaxiAsync` | POST | `/v3/taxi-invoice/taxi/update-taxi` | 同上 | 回 `TaxiInvoiceTaxiUpdateResponse` |
| 6 | `QueryTaxiInvoiceTaxiAsync` | GET | `/v3/taxi-invoice/taxies/{plateNumber}` | 同上 | `region_id` **必填 query** |
| 7 | `QueryTaxiInvoiceTaxiCompanyAsync` | GET | `/v3/taxi-invoice/taxi-companies/{companyMchid}` | 同上 | `region_id` **必填 query** |
| 8 | `CreateTaxiInvoiceTaxiCompanyAsync` | POST | `/v3/taxi-invoice/taxi-company/create-taxi-company` | 同上 | 建档经营主体 |
| 9 | `QueryTaxiInvoiceUserTaxiOrderAsync` | GET | `/v3/taxi-invoice/user-taxi-order/find-by-token` | 同上 | 四个必填 query + 选填 `region_id` |
| — | `UploadTaxiInvoiceFileAsync`（手写通道） | POST | `/v3/taxi-invoice/cards/upload-file`（`multipart/form-data`） | 同上 | **不占**声明式计数；应答落 `TaxiInvoiceUploadFileResponse` |

九支声明式端点官方**全部只印服务商页**（各页均标「支持商户：【普通服务商】」）⇒ 无商户面镜像、
无双页归并问题，一支一页一方法。

## 认证：本线没有 access_token

APIv3 商户证书 `WECHATPAY2-SHA256-RSA2048` 请求签名，由传输层 `WechatPayAuthorizationHandler` 注入；
本接口不声明 `[Token]`（`PAY-B1` fail-closed）。命名客户端 `wechat-pay`，BaseAddress 恒
`https://api.mch.weixin.qq.com`；`[AllowAnyStatusCode]` + `WechatPayException.ThrowIfFailed` 是唯一判错面。
手写通道走同一注入语义（同一条商户签名链路），不经 `[HttpClientApi]`，因此**没有路由反射面**，
路径字面量锁在通道源码里。

## 上传通道为什么不在声明式接口里（刻意，非遗漏）

官方 `cards/upload-file` 的请求体是 `multipart/form-data`（表单 `file` + `meta`），且 `meta.digest`
要求**国密 SM3** 十六进制摘要，而 SM3 不在 .NET BCL 内 ⇒ 声明式 JSON 端点承载不了。
由手写通道 `WechatPayTaxiInvoiceFileService` 承载，只把**应答**落成 DTO
（`TaxiInvoiceUploadFileResponse.FapiaoMediaId`）；`meta` 的 4 个字段
（`company_mchid` / `digest` / `digest_algorithm` / `region_id`）只出现在表单段 ⇒ **不建请求 DTO**
（同电子发票域口径），字段名常量锁在通道源码里。守卫 `TXI-B1` 断言两支手工类型可解析 +
`PayServiceBuilder.cs` 里的 `TryAddSingleton` 注册存在。

**拼写差异是本域最容易踩的坑**：本页官方字段名是 `digest_algorithm`（拼写正常），而电子发票域
`/v3/new-tax-control-fapiao/.../upload-fapiao-file` 官方印的是 `digest_alogrithm`（少一个 `r`），
且本页取值印 `DIGEST_ALGORITHM_SM3`、电子发票域印 `SM3` ⇒ **两支各照各页，不得互相「纠正」**。

## `region_id` 是本域的定位基座

十支里九支携带 `region_id`（行政区划代码），且第 4、6、7 支（查司机 / 查出租车 / 查经营主体）把它放在
**必填 query**，不是请求体 ⇒ 单靠 `driver_license` / `plate_number` / `company_mchid` **不足以定位**。
`TXI-B2` 逐支锁 required-query 名册，三支的必填集都是 `{region_id}`，第 9 支是
`{token, openid, appid, auth_mode}`（`region_id` 在该支是**选填**）。
⇒ 这三支的 `regionId` 在签名上是 `int` 无默认值；照页不得省，也不得「顺手」给第 9 支的 `region_id` 升必填。

## 第 3 支是唯一涉密支

`POST /v3/taxi-invoice/driver/update-driver` 的官方请求头表把 `Wechatpay-Serial` 列为**必填**
（司机敏感信息，如证件/照片摘要，须按 APIv3 加密上送），其余 8 支**未列**该头 ⇒
**不得**按「同域一律涉密」补齐，也**不得**反向省略。声明式签名不承载该头（同本线
`Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径，由传输层按商户配置注入）；
密文以 `string` 承载，**SDK 不代为加解密**。司机姓名/证件号等明文一律不得进日志、遥测或异常消息。

## 两支 204 无应答包体与跨支依赖

签到签退（第 1 支）与插卡（第 2 支）的官方应答节原文是「无应答包体」+ 正常示例 `204 No Content`
⇒ 返回裸 `Task`，不建响应类型，结果以 HTTP 码 + `code`/`message` 判定。

**插卡支有前置依赖链**：`TaxiInvoiceCardInsertRequest.FapiaoCardInformation.FapiaoMediaId`
是「上传的电子发票文件对应的 ID」，必须由手写通道先返回，**有效期 3 天** ⇒
先上传、再插卡，跨天复用同一 `fapiao_media_id` 属未定义行为。

## 发票卡券的金额与数量单位不是「分」

插卡请求的发票行子表（`TaxiInvoiceCardInsertRequestFapiaoCardInformationItemsItem`）有两处
**反直觉缩放**，照 DTO 原文：

| 键 | 单位口径 |
|---|---|
| `quantity` | `10^-8` 为单位，`100000000` 表示数量为 1 |
| `unit_price` | `10^-6` 分为单位，`100000000` 表示 1 元 |
| `amount` / `tax_amount` / `total_amount` | 单位**分**（`total_amount` 是单行金额 + 税额之和） |
| `tax_rate` | 万分之一，`1300` 代表 13% |

另有两支必填标签枚举：`buyer_information.type` ∈ `{CORPORATION 公司, INDIVIDUAL 个体工商户}`，
但 `taxpayer_id` 的原文条件是「购买方类型为 `ORGANIZATION` 时必须存在」—— **官方同一页出现第三种取值名**，
本仓不推断、不改写（记入文末缺口）。`tax_prefer_mark` ∈ `{NO_FAVORABLE, OUTSIDE_VAT, VAT_EXEMPT,
NORMAL_ZERO_RATED, EXPORT_ZERO_RATED}`（必填）；`discount`（必填 `boolean`）原文注明
「折扣行一定是被折扣行的下一行」⇒ **发票行顺序有业务语义**，不得重排。

## 第 9 支的授权模式决定能查到什么

`auth_mode` 必填，取值二选一：

| 值 | 官方原文口径 |
|---|---|
| `ORDER_AUTH` | 协议授权的基础上，**还需对每笔订单进行授权** |
| `PROTOCOL_AUTH` | 用户同意协议，授权查询 |

该支的码表把「授权」问题全部落在 `403 NO_AUTH`（商户无权限 / 用户未授权 / 订单未授权），
另有 `429 RATE_LIMITED 频率限制`。

**限频码的拼写在逐页之间不一致，以方法 remarks 为准**：类级 remarks 概括本域为
`429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED`，但逐支摘录里第 1 支（签到签退）、第 9 支（本支）印的都是
`429 RATE_LIMITED` ⇒ 概括语与逐页原文不一致时**以逐支原文为准**，不得反向把某支的码名「统一」掉。
应答 `TaxiInvoiceUserOrderFindResponse` 含 `plate_number` / `driver_license` / `up_time` / `region_id` /
`source` / `pay_time` / `pay_amount`（金额字段 `long?`，单位按官方原文，**未逐项核验**）。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");   // 凭据底座只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.TaxiInvoice));   // 声明式九支 + 上传通道同批装配

var taxi = scope.ServiceProvider.GetRequiredService<IWechatPayTaxiInvoiceService>();
var file = scope.ServiceProvider.GetRequiredService<IWechatPayTaxiInvoiceFileService>();

// 1) 司机签到（204 No Content ⇒ 裸 Task；event_time 精确到毫秒，rfc3339 文本）
await taxi.PunchTaxiInvoiceAttendanceAsync(new TaxiInvoiceAttendancePunchRequest
{
    PlateNumber = "苏A12345",              // 假值
    DriverLicense = "occupy-driver-license-0001",   // 营运资格证号（涉密面相关，勿写真实值）
    EventTime = "2026-10-18T08:00:00.000+08:00",
    EventType = "SIGN_IN",                 // SIGN_IN 签到 / SIGN_OUT 签退
    RegionId = 320100,                     // 行政区划代码，必填
}, cancellationToken: ct);

// 2) 查出租车（region_id 是必填 query，路径只给车牌）
var taxiInfo = await taxi.QueryTaxiInvoiceTaxiAsync("苏A12345", 320100, ct);
WechatPayException.ThrowIfFailed(taxiInfo, "/v3/taxi-invoice/taxies/{plate_number}");

// 3) 凭凭证查乘客行程单（四个必填 query + auth_mode 二选一）
var order = await taxi.QueryTaxiInvoiceUserTaxiOrderAsync(
    token: "<乘客授权后下发的行程单凭证>",
    openid: "occupy-openid-000000000000000000000",
    appid: "wx0000000000000000",
    authMode: "PROTOCOL_AUTH",             // 或 ORDER_AUTH（需逐单授权）
    regionId: 320100,
    cancellationToken: ct);

// 4) 上传电子发票文件（multipart + SM3 摘要，走手写通道；fapiao_media_id 有效期 3 天）
//    表单字段与摘要计算见 IWechatPayTaxiInvoiceFileService 的注释，本域不在声明式面承载文件流。
// var uploaded = await file.UploadTaxiInvoiceFileAsync(...);
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `buyer_information.type` 与 `taxpayer_id` 的条件必填口径 | 键值表是 `{CORPORATION, INDIVIDUAL}`，而 `taxpayer_id` 原文写「类型为 `ORGANIZATION` 时必须存在」⇒ 第三取值名**未逐项核验**，本仓不改写 | 需要判定前先核验官方页；不得本地拦截 |
| 上传通道的表单细节（`meta` 各字段长度、文件大小上限、SM3 摘要来源） | 常量在通道源码，官方上限 remarks **未摘录** ⇒ 未逐项核验 | 走通道源码，勿在本文补数值 |
| `fapiao_media_id` 3 天有效期后的错误码表现 | 只记「有效期 3 天」，越期返回什么码**未逐项核验** | 不得写「返回 404」一类推断 |
| 行程单应答的金额单位（`pay_amount`） | DTO 为 `long?`，单位口径未摘录 ⇒ **未逐项核验** | 参照发票行的 10^-6 分缩放**属猜测**，不得套用 |
| 司机/车辆/经营主体建档的必填集差异 | 三支请求体的逐字段必填只在 DTO 注释里 ⇒ 本次未逐项摘录 | 需要时读 DTO 目录，勿从本文推断 |
| 各支端点级 QPS | 九支官方页**全部未印**，只有 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` / `429 RATE_LIMITED` ⇒ 不凭第三方 SDK 补数 | 记为「未印」 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、各支错误码、涉密/204 的逐页结论 | `IWechatPayTaxiInvoiceService.cs` 的类级与方法级 XML 注释 |
| 上传通道的表单字段常量与路径字面量 | `../../TaxiInvoice/IWechatPayTaxiInvoiceFileService.cs`、`../../TaxiInvoice/WechatPayTaxiInvoiceFileService.cs` |
| 方法数 9 / 「路由集合不含 upload-file」/ 手工类型可解析 / DI 注册 | `TaxiInvoiceTopology_ShouldMatchInterfaceAndGroupRoster`（`TXI-B1`） |
| 每支 query 名册与必填集（三支 `{region_id}`、第 9 支四键） | `TaxiInvoiceEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`TXI-B2`） |
| 五支请求 + 七支应答的字段名册 | `TaxiInvoiceDtos_ShouldExposeOfficialFieldRosters`（`TXI-B3`） |
| 请求/响应基底类型分离 | `TaxiInvoiceDataModels_ShouldSplitRequestAndResponseBaseTypes`（`TXI-B4`） |
| DTO 数 24 与 AOT 登记 | `TaxiInvoiceAllDtos_ShouldBeAnnotatedAndContextRegistered`（`TXI-B5`） |
| DI / `PayModule` / JSON 解析器接线 | `TaxiInvoiceRegistrars_ShouldBeWiredThroughPayModuleAndResolver`（`TXI-B6`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay.DataModels/TaxiInvoice/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、支持商户、错误码与必填口径，**同批**改 `TXI-B1` 的 9 计数、
   `TXI-B2` 的 per-endpoint query 名册；跨域总数同步 `../../../../../Tests/ContractBaseline.cs`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `TaxiInvoice`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `TXI-B3` 名册与 `TXI-B5` 的 24 计数。
3. 改上传通道 → 路径字面量、表单字段名常量、SM3 摘要计算三处须同批，并同批改本文的
   「上传通道为什么不在声明式接口里」与 `TXI-B1` 的手工类型/`TryAddSingleton` 断言。
4. **不要**把上传通道塞进声明式接口；**不要**把本页 `digest_algorithm` / `DIGEST_ALGORITHM_SM3`
   改成电子发票域的 `digest_alogrithm` / `SM3`（反之亦然）；**不要**给非涉密的 8 支补涉密结论；
   **不要**给 204 的两支建响应类型；**不要**给任何方法加 `[Token]`。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。

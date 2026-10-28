# Vehicle 域 · 车主出行（ETC 高速扣费 + 微信支付分停车）

微信支付 APIv3「车主服务」域：**10 支声明式端点**（ETC 六支 + 停车四支）。
接口 [IWechatPayVehicleService.cs](./IWechatPayVehicleService.cs)，注册组 `Vehicle`，
模块枚举 `PayModule.Vehicle`（装配入口 `AddWechatPayApi(b => b.AddModules(PayModule.Vehicle))`，
内部调用源生成的 `AddVehicleWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayVehicleContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayVehicleContractGuards.cs)
> （`VEH-B1`~`VEH-B6`）与本域 DTO（`../../../Mud.Wechat.Pay/Vehicle/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是本域守卫 `VEH-B1`：`Methods().Should().HaveCount(10)` **且** `DistinctVerbRoutes().Should().HaveCount(10)`
（「本域一支路由一方法，无分身」），动词集合 `{Get, Post}`。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryVehicleEtcContractAsync` | GET | `/v3/vehicle/etc/contracts/{contractId}` | 商户证书 / APIv3 请求签名（服务商作用域） | `sub_mchid` 为**选填** query |
| 2 | `QueryVehicleEtcUserContractAsync` | GET | `/v3/vehicle/etc/users/{openid}/contracts` | 同上 | `plan_id` + `plate_number` **必填 query** |
| 3 | `CreateVehicleEtcPreopenAsync` | POST | `/v3/vehicle/etc/preopen` | 同上 | 只回 `preopen_id` ⇒ **受理**，非开通成功 |
| 4 | `CreateVehicleEtcHighwayTransactionAsync` | POST | `/v3/vehicle/etc/transactions/highway` | 同上 | 错误码最密的一支（`RULE_LIMIT` / `CONTRACT_ERROR` / `TRADE_ERROR` …） |
| 5 | `QueryVehicleEtcTransactionByIdAsync` | GET | `/v3/vehicle/etc/transactions/id/{transactionId}` | 同上 | 与 6 共用应答类型 |
| 6 | `QueryVehicleEtcTransactionByOutTradeNoAsync` | GET | `/v3/vehicle/etc/transactions/out-trade-no/{outTradeNo}` | 同上 | 与 5 同形 ⇒ 共用 `VehicleEtcTransactionResponse` |
| 7 | `CreateVehicleParkingEntryAsync` | POST | `/v3/vehicle/parking/parkings` | 同上 | 【普通商户】【普通服务商】双面 |
| 8 | `QueryVehicleParkingServiceAsync` | GET | `/v3/vehicle/parking/services/find` | 同上 | 5 个 query **全部必填** |
| 9 | `QueryVehicleParkingTransactionByOutTradeNoAsync` | GET | `/v3/vehicle/transactions/out-trade-no/{outTradeNo}` | 同上 | `sub_mchid` **必填 query** |
| 10 | `CreateVehicleParkingTransactionAsync` | POST | `/v3/vehicle/transactions/parking` | 同上 | 异步扣款的**受理**接口 |

## 认证与两族开放面

同本线口径：APIv3 商户证书 `WECHATPAY2-SHA256-RSA2048` 签名、无 `access_token`、不声明 `[Token]`
（`PAY-B1` fail-closed）；命名客户端 `wechat-pay`，BaseAddress 恒 `https://api.mch.weixin.qq.com`；
`[AllowAnyStatusCode]` + `WechatPayException.ThrowIfFailed` 是唯一判错面；服务商作用域经
`IWechatPayMerchantContext.UseMerchant(...)` 选择。

一路由树两族、开放面**不同**（`VEH-B1` 类 remarks）：

- `/v3/vehicle/etc/*`（1–6）官方**只印服务商页**【普通服务商】⇒ 请求以 `sp_mchid` + `sub_mchid` 双层定位，
  **不得**按停车面反向补齐「商户页镜像」。
- `/v3/vehicle/parking/*` 与 `/v3/vehicle/transactions/parking`（7–10）各印**商户页 + 服务商页**
  ⇒ 字段取**并集**、必填只取**交集**，仅某页印出的字段在 DTO 注释里标「仅服务商页印发」
  （如 `VehicleParkingEntryRequest.SubMchid` 注释即「服务商模式下必传」）。一面一方法，**不得**按身份拆两套方法或类型。

## 定位键的四种形状（照页不得统一）

| 端点 | path | query（名册 / 必填集） |
|---|---|---|
| 1 | `contract_id` | `{sub_mchid}` / **无必填** |
| 2 | `openid` | `{sub_mchid, plan_id, plate_number}` / `{plan_id, plate_number}` |
| 5 | `transaction_id` | `{sub_mchid}` / 无必填 |
| 6 | `out_trade_no` | `{sub_mchid}` / 无必填 |
| 8 | 无 | `{appid, plate_number, openid, plate_color, sub_mchid}` / **五支全必填** |
| 9 | `out_trade_no` | `{sub_mchid}` / `{sub_mchid}` |

> **守卫内部有一处措辞不一致**：`VEH-B` 类 remarks 说第 2 支是「`sub_mchid` / `plan_id` / `plate_number` 三支必填 query」，
> 而逐支断言 `RequiredQueryOf(QueryVehicleEtcUserContractAsync) == {plan_id, plate_number}`
> （`sub_mchid` 只进名册、是可选参数）。**以逐支断言为准**，接口签名里 `subMchid` 带默认值即其体现。

第 2 支尤其容易踩：path 段给了 `openid` **不足以定位**，`plan_id` 与 `plate_number` 必须一起给。
第 8 支的五个 query 之所以全部无默认值，守卫 remarks 给了理由 ——「给默认值即把 400 变成静默错参」。

## `plate_color` 是六值枚举，跨支同形

`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色。
出现在入场请求/应答（`VehicleParkingEntryRequest.PlateColor` / `...EntryResponse.PlateColor`）与第 8 支 query 上，
类型一律 `string`（本线不建枚举类型，值照原文传）。车牌号 `plate_number` 的官方口径是
「仅包括省份+车牌，不包括特殊字符」，`string(32)`。

## 入场状态是两套值，且 `block_reason` 只在不可用时出现

`VehicleParkingEntryResponse` 回 `state`：

| 值 | 含义 |
|---|---|
| `NORMAL` | 正常，可以使用微信支付分停车服务 |
| `BLOCKED` | 不可用状态，暂时不可使用 |

`state = BLOCKED` 时才回 `block_reason`，取值：`PAUSE`（已暂停服务）/ `OVERDUE`（已签约但欠费）/
`OUT_SERVICE`（车牌未开通停车服务）/ `EVALUATION_FAILED`（综合评估未通过、用户支付分不可用）。
⇒ 入场返回 2xx **不等于**可以扣费，业务闸门是 `state`。

## 两支「受理」不是「成功」

- `CreateVehicleParkingTransactionAsync`：官方原文「微信支付进行**异步扣款**，支付完成后，会将订单支付结果
  发送给商户」⇒ 不得据 HTTP 2xx 判定扣款成功，结果由异步通知 + 第 9 支查询判定。
- `CreateVehicleEtcPreopenAsync`：只回 `preopen_id` ⇒ 开通结果由签约/查询支（第 1、2 支）判定。

第 4 支（高速扣款）与第 5、6 支的码表里都有 `202 USER_PAYING 当前订单正在支付中` ⇒ 202 是**业务态**
（既有支付在途），不是网关错误；同一支还印 `400 INVALID_REQUEST：订单重复扣款（同一车牌在同一出场时间重复发起扣款）`
⇒ 出场时间与车牌共同构成天然幂等键，重试**不得**改单号绕过。

## ETC 高速扣款的错误码面最宽

第 4 支（`/v3/vehicle/etc/transactions/highway`）逐字摘录的码表：
`400 APPID_MCHID_NOT_MATCH`（商户号信息错误或与 AppID 不匹配）/ `403 RULE_LIMIT`（订单金额超过支付限额）/
`400 CONTRACT_NOT_EXIST`（当前车牌无绑定或已暂停 ETC 扣费服务）/ `403 CONTRACT_ERROR`（绑定状态失效）/
`403 TRADE_ERROR`（用户紧急冻结支付账户）/ `400 INVALID_REQUEST`（订单重复扣款）/
`403 OUT_TRADE_NO_USED`（商户订单号重复）/ `404 ORDER_NOT_EXIST` / `202 USER_PAYING` /
`403 NO_AUTH`（无 ETC 代扣或委托代扣权限）/ `429 FREQUENCY_LIMITED` / `500 SYSTEM_ERROR`。
停车两支的码表**不同**（第 7 支含 `403 NO_AUTH：商户未入驻` + `429 RATELIMIT_EXCEEDED`，
第 10 支含 `400 ALREADY_EXISTS：out_trade_no冲突` 与多种绑定关系错）⇒ **不得**跨支互挂错误码。

## 应答类型的分合裁决

守卫 `VEH-B` 类 remarks 给了三条判据（都是逐页比对结果，不是「看起来一样」）：

1. ETC 两支查单（5、6）应答表 **33 行逐字段同形且必填口径一致** ⇒ **共用** `VehicleEtcTransactionResponse`。
2. 高速扣款**下单**应答与查单**不同形**（`device_info` 下单侧必填、`trade_type` 长度 16 vs 32、
   `success_time` 32 vs 25）⇒ **单建** `VehicleEtcHighwayTransactionResponse`。
3. 签约两支（1、2）应答同为 9 行同形 ⇒ **共用** `VehicleEtcContractResponse`；
   停车「查单」与「下单」字段名集相同但 `amount.currency` 必填口径不同（查询页选填、下单页必填）
   ⇒ **分建** `VehicleParkingTransactionQueryResponse` / `VehicleParkingTransactionResponse`。
   本线口径：**共用只发生在应答侧且须含必填口径一致**。

## 敏感字段：ETC 预开通支是本域唯一的重涉密面

`VehicleEtcPreopenRequest` 的键集（`VEH-B3` 名册）：`sp_appid` / `sub_appid` / `sub_mchid` / `openid` /
`plan_id` / `etc_device_id` / `plate_number` / `identify` / `bank_card_no`。其中

- `identify` 是必填对象，内含 `encrypted_real_name`（`string(256)`）、`encrypted_credential_id`（`string(256)`）、
  `credential_type`（`string(8)`，官方注「目前只支持身份证」）——**键名即要求密文**，
  加密由宿主按 APIv3 敏感信息加密指引完成，**SDK 不代为加解密**，声明式签名也不承载 `Wechatpay-Serial` 头
  （由传输层按商户配置注入）。
- `bank_card_no`（`string(2048)`，选填）是银行卡号：明文/密文都**不得**进日志、遥测或异常消息。
- 示例中这些字段一律用占位符；真实姓名、证件号、卡号不得写入任何提交物。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");   // 凭据底座只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Vehicle));

var vehicle = scope.ServiceProvider.GetRequiredService<IWechatPayVehicleService>();

// 1) 创建停车入场信息（服务商模式下 sub_mchid 必传；时间用 rfc3339 字符串）
var entry = await vehicle.CreateVehicleParkingEntryAsync(new VehicleParkingEntryRequest
{
    OutParkingNo = "PARK202610180001",   // 同一商户号下唯一
    PlateNumber = "苏A12345",             // 假值：仅省份+车牌，不含特殊字符
    PlateColor = "BLUE",                  // BLUE/GREEN/YELLOW/BLACK/WHITE/LIMEGREEN
    ParkingName = "示例停车场",
    StartTime = "2026-10-18T09:00:00+08:00",
    FreeDuration = 1800,                   // 免费时长，单位秒
    NotifyUrl = "https://example.com/parking/notify",   // 只接受 HTTPS
    SubMchid = "1900000002",
}, cancellationToken: ct);
WechatPayException.ThrowIfFailed(entry, "/v3/vehicle/parking/parkings");
if (entry.State != "NORMAL")
{
    // BLOCKED：看 entry.BlockReason ∈ {PAUSE, OVERDUE, OUT_SERVICE, EVALUATION_FAILED}，此时不得发起扣费
}

// 2) 查停车服务开通状态：五个 query 全部必填（无默认值）
var service = await vehicle.QueryVehicleParkingServiceAsync(
    appid: "wx0000000000000000",
    plateNumber: "苏A12345",
    openid: "occupy-openid-000000000000000000000",
    plateColor: "BLUE",
    subMchid: "1900000002",
    cancellationToken: ct);

// 3) 停车扣费受理下单：**异步扣款**，2xx 只代表受理
var accepted = await vehicle.CreateVehicleParkingTransactionAsync(new VehicleParkingTransactionRequest
{
    Appid = "wx0000000000000000",
    OutTradeNo = "PARKPAY202610180001",   // 仅数字、大小写字母与 _-*，同一商户号下唯一
    Description = "停车费",
    TradeScene = "PARKING",                // 必填，官方目前只印 PARKING 一个交易场景值
    NotifyUrl = "https://example.com/pay/notify",   // 只接受 HTTPS
    Amount = new VehicleParkingTransactionRequestAmount { Total = 1200, Currency = "CNY" },  // 12 元 = 1200 分
    ParkingInfo = new VehicleParkingTransactionRequestParkingInfo
    {
        PlateNumber = "苏A12345",
        PlateColor = "BLUE",
        StartTime = "2026-10-18T09:00:00+08:00",
        EndTime = "2026-10-18T11:30:00+08:00",
        ChargingDuration = 7200,           // 计费时长，单位秒
    },
}, ct);

// 4) 扣费结果以查询支为准（sub_mchid 是必填 query）
var paid = await vehicle.QueryVehicleParkingTransactionByOutTradeNoAsync("PARKPAY202610180001", "1900000002", ct);
```

> 第 3 步的请求体字段与嵌套类型名以 `VehicleParkingTransactionDtos.cs` 声明为准
> （名册见 `VEH-B3`：`appid` / `description` / `attach` / `out_trade_no` / `trade_scene` / `goods_tag` /
> `notify_url` / `profit_sharing` / `amount` / `parking_info` / `sub_appid` / `sub_mchid`）；
> 上例只填主干，接入前按 DTO 声明逐项核对属性名与类型。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 各支端点级 QPS | 十支官方页**全部未印**，只有 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不凭第三方 SDK 补数 | 记为「未印」 |
| `goods_tag` / `profit_sharing` / `attach` 的分账与营销生效条件 | `trade_scene` 官方只印 `PARKING` 一值（已入上文）；`profit_sharing` 是 `Y`/`N` 大写串（不传即不分账），但其与服务商分账面的联动规则**未逐项核验** | 不得从基础交易域借口径 |
| 高速扣款的金额上限（`RULE_LIMIT` 的具体阈值） | 错误码只说「超过支付限额」⇒ 阈值**未逐项核验** | 不得写数值，也不做本地金额闸 |
| ETC 预开通的**后续**流程（`preopen_id` 之后走哪支确认开通） | remarks 只说「只回 `preopen_id`」⇒ 链路**未逐项核验** | 需要完整链路时先核验官方页 |
| `etc_device_id` / `plan_id` 的格式与来源面 | 名册有键、格式约束未摘录 ⇒ **未逐项核验** | 以应答与官方页为准 |
| 回调通知形态（停车入场状态变更、异步扣款结果） | 由 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md) 的资源类型分发承载，本接口文件不含回调契约 | 回调面查 Callback 包 |
| 守卫类 remarks 与逐支断言在 `sub_mchid` 必填性上的措辞差异 | 见上节，**以逐支断言为准** | 修 remarks 属代码改动，须另批授权 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、各支错误码、两族开放面与「受理≠成功」原文 | `IWechatPayVehicleService.cs` 的类级与方法级 XML 注释 |
| 方法数 10 / 动词集合 / 「一支路由一方法」 | `VehicleTopology_ShouldMatchInterfaceAndGroupRoster`（`VEH-B1`） |
| 每支 query 名册与必填集 | `VehicleEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`VEH-B2`） |
| 各请求/应答类型字段名册（含 ETC 预开通与停车两支） | `VehicleDtos_ShouldExposeOfficialFieldRosters`（`VEH-B3`） |
| 请求/响应基底类型分离 | `VehicleDataModels_ShouldSplitRequestAndResponseBaseTypes`（`VEH-B4`） |
| DTO 数 34 与 AOT 登记 | `VehicleAllDtos_ShouldBeAnnotatedAndContextRegistered`（`VEH-B5`） |
| DI / `PayModule` / JSON 解析器接线 | `VehicleRegistrars_ShouldBeWiredThroughPayModuleAndResolver`（`VEH-B6`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/Vehicle/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、支持商户、错误码与必填口径，**同批**改 `VEH-B1` 的 10 计数、
   `VEH-B2` 的 per-endpoint query 名册与必填集；跨域总数同步 `../../../../../Tests/ContractBaseline.cs`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Vehicle`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `VEH-B3` 名册与 `VEH-B5` 的 34 计数（`VEH-B6` 校验 `PayModule.Vehicle` 与注册器接线）。
3. 改应答类型的分合（共用/分建）→ 必须先有逐页字段表与必填口径比对依据，再同批改本文「分合裁决」一节。
4. **不要**给 ETC 六支补商户页镜像；**不要**按身份把停车四支拆成两套方法；**不要**在第 8 支给 query 加默认值；
   **不要**把 `identify` / `bank_card_no` 当明文字段处理或在示例里写真值；**不要**给任何方法加 `[Token]`。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。

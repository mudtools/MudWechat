# Nontax 域 · 非税支付

微信公众号 / 服务号「非税支付」域：**17 条路由分两张面孔**——JSON 通道 **16 端点**（缴费查询 / 下单 /
查单 / 退款 / 订单列表 / 被扫 / 不一致通知 / 两个模拟联调 / 实名 / 车辆三端点 / 票据卡券三端点）落接口
[IMpNontaxService.cs](./IMpNontaxService.cs)；对账单下载（`/nontax/downloadbill`，成功响应为 CSV 文本）落
[../../Nontax/MpNontaxBillDownloadTypes.cs](../../Nontax/MpNontaxBillDownloadTypes.cs) 的
`IMpNontaxBillDownloadService` 独立下载通道。注册组 `Nontax`，模块枚举 `MpModule.Nontax`
（`AddNontaxApi()` ⇒ 生成入口 `AddNontaxWebApiHttpClient()`，并 `TryAddSingleton` 下载服务）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpNontaxContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpNontaxContractGuards.cs)
> （`NT1`~`NT2`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/Nontax/](../../../Mud.Wechat.OfficialAccount/DataModels/Nontax/)）。
> **计数以守卫为准**：`NT1` 钉「16 条 POST `/nontax/*` 全部带请求体，`downloadbill` 不进管线、以路径常量单列」；
> `QT1` 注释记 `IMpNontaxService` 为 16 端点（downloadbill 走无 `[Token]` 的手工通道，不入白名单）。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryFeeAsync` | POST | `/nontax/queryfee` | 应用级 `access_token`（Query） | 通知书号 + 执收单位 + 地区必填；金额单位分 |
| 2 | `UnifiedOrderAsync` | POST | `/nontax/unifiedorder` | 应用级 `access_token`（Query） | 同一通知书重复下单须先查列表防重缴 |
| 3 | `GetOrderAsync` | POST | `/nontax/getorder` | 应用级 `access_token`（Query） | `order_id` 必填 |
| 4 | `RefundAsync` | POST | `/nontax/refund` | 应用级 `access_token`（Query） | 资金逆向、不可撤回（覆盖删除语义） |
| 5 | `GetOrderListAsync` | POST | `/nontax/getorderlist` | 应用级 `access_token`（Query） | `order_no` 与 `payment_notice_no` **二选一** |
| 6 | `MicroPayAsync` | POST | `/nontax/micropay` | 应用级 `access_token`（Query） | 被扫，`auth_code` 必填 |
| 7 | `NotifyInconsistentOrderAsync` | POST | `/nontax/notifyinconsistentorder` | 应用级 `access_token`（Query） | 返回 `MpResponse` |
| 8 | `MockNotificationAsync` | POST | `/nontax/mocknotification` | 应用级 `access_token`（Query） | 回调地址必填，**联调专用** |
| 9 | `MockQueryFeeAsync` | POST | `/nontax/mockqueryfee` | 应用级 `access_token`（Query） | 与模拟通知同形，联调专用 |
| 10 | `GetRealNameAsync` | POST | `/nontax/getrealname` | 应用级 `access_token`（Query） | `wx_realname_token`；响应**加密，解密由调用方**，SDK 不持密钥 |
| 11 | `QueryVehicleStateAsync` | POST | `/nontax/vehicle/querystate` | 应用级 `access_token`（Query） | `plate_number` 与 `openid` **二选一** |
| 12 | `VehicleEntranceNotifyAsync` | POST | `/nontax/vehicle/entrancenotify` | 应用级 `access_token`（Query） | 车辆入场，`scene_info` 必填 |
| 13 | `VehiclePayApplyAsync` | POST | `/nontax/vehicle/payapply` | 应用级 `access_token`（Query） | 车辆支付申请，`scene_info` 必填 |
| 14 | `CreateBillCardAsync` | POST | `/nontax/createbillcard` | 应用级 `access_token`（Query） | `invoice_info` → 返回 `card_id` |
| 15 | `InsertBillAsync` | POST | `/nontax/insertbill` | 应用级 `access_token`（Query） | `order_id`+`card_id`+`card_ext` → `code`+`openid` |
| 16 | `GetBillAuthUrlAsync` | POST | `/nontax/getbillauthurl` | 应用级 `access_token`（Query） | `ticket` = 官方 `api_ticket`（票据基座承载） |
| 17 | `DownloadBillAsync` | POST | `/nontax/downloadbill` | 手工注入令牌，**下载通道无 `[Token]`** | 成功 = CSV 文本、失败 = JSON errcode ⇒ 独立通道（I3 延用） |

**权限口径**：本域适用范围「公众号 / 服务号」（官方文档未标「仅认证」，与素材/菜单域不同）。

## 契约要点（代码名读不出的部分）

1. **业务准入**：非税支付需与财政局签约开通专项服务（`service_id`）；本 SDK 只提供端点、不校验准入，
   未签约由官方错误码表达。

2. **金额一律以「分」为单位**：所有金额字段（`fee`、`refund_fee`、`money` 等）为整数分，勿按元填写。

3. **退款为覆盖删除式逆向操作**：`RefundAsync` 一旦发起，原支付订单进入退款流程**不可撤回**；
   部分退款另填 `refund_fee`/`refund_out_id`。

4. **重复下单前置**：同一缴费通知书重复下单前须先调 `GetOrderListAsync`（`order_no` 与 `payment_notice_no`
   **二选一**）核对，避免重复缴费。

5. **两处「二选一」互斥参数**（SDK 不做本地拦截）：`GetOrderListAsync` 的 `order_no` / `payment_notice_no`；
   `QueryVehicleStateAsync` 的 `plate_number` / `openid`。

6. **实名信息解密归调用方**：`GetRealNameAsync` 返回的 `key`/`data` 均为加密数据（含算法标识），
   密钥由调用方与财政局约定，**SDK 不持密钥、不解密**。

7. **对账单下载双通道（I3 延用）**：`/nontax/downloadbill` 成功响应是 **CSV 文本**（官方 FAQ 原文「以 csv 格式的
   文本返回」），失败才是 JSON `errcode` ⇒ 走 `IMpNontaxBillDownloadService.DownloadBillAsync`（`SendRawAsync` +
   Content-Type 分支判错，不进 JSON 生成管线）；返回 `MpNontaxBillDownloadResult`（`IDisposable`，SDK 不落盘）。
   `MpNontaxBillDownloadService.DownloadBillPath` 常量固定为 `/nontax/downloadbill`（`NT1`）。
   `MD11` 同款规则：本下载通道与素材下载通道的应用上下文解析须回退默认应用。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddNontaxApi());

// 注入 IMpNontaxService（JSON 通道）与 IMpNontaxBillDownloadService（对账单下载通道）

// 1) 查询欠费（金额单位为分）
MpNontaxQueryFeeResponse fee = await nontax.QueryFeeAsync(new MpNontaxQueryFeeRequest
{
    ServiceId = 100001,                 // 与财政局签约的专项服务 ID
    PaymentNoticeNo = "NOTICE-0001",    // 通知书号（示例假值）
    DepartmentCode = "DEPT-001",        // 执收单位编码（示例假值）
    RegionCode = "440000",              // 地区编码（示例假值）
}, ct);
MpException.ThrowIfFailed(fee, "/nontax/queryfee");
MpNontaxBillItem[] items = fee.ItemList ?? Array.Empty<MpNontaxBillItem>();
int feeInCents = fee.Fee;

// 2) 订单列表：order_no 与 payment_notice_no 二选一（重复下单前置核对）
MpException.ThrowIfFailed(
    await nontax.GetOrderListAsync(new MpNontaxGetOrderListRequest
    {
        PaymentNoticeNo = "NOTICE-0001",
        DepartmentCode = "DEPT-001",
        RegionCode = "440000",
    }, ct),
    "/nontax/getorderlist");

// 3) 获取实名信息（响应为加密数据，解密由调用方执行，SDK 不持密钥）
MpException.ThrowIfFailed(
    await nontax.GetRealNameAsync(new MpNontaxGetRealNameRequest { WxRealNameToken = realNameToken }, ct),
    "/nontax/getrealname");

// 4) 下载缴费对账单（CSV 文本流，走独立下载通道；结果对象须释放）
using (MpNontaxBillDownloadResult bill = await nontaxBill.DownloadBillAsync(
    new MpNontaxDownloadBillRequest { BillDate = "20261018" }, ct))
{
    Stream? csv = bill.Content;   // 成功为 CSV 文本流；失败时 SDK 已在通道内抛 MpException
}
```

凭据红线：`access_token`、`api_ticket`、`appsecret`、`openid`、`wx_realname_token`、`auth_code` 只作参数名，
**不得**进日志、遥测或异常消息；`getbillauthurl` 的 `ticket`、实名 `data` 属敏感载荷；示例通知书 / 编码 / 区号
均为假值，绝不写入真实缴款人姓名或证件信息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 各端点错误码表 | 接口 XML 仅统一记 MUD005，未逐端点抄录官方错误码表 | 逐页核验后补各方法 remarks 与错误码常量（当前本域不新增错误码常量） |
| 官方文档缺陷（`getorderlist` 与 `micropay` 深链同锚点） | 两页官方锚点 `#6.1.9` 重复 | 照录官方链接，勿据此判定接口混淆 |
| `downloadbill` 的 `bill_type` 取值全集 | 建模为可选 `string?`，官方枚举**未逐项核验** | 核验后补 remarks，勿从邻域借枚举 |
| 车辆 / 卡券子族字段级含义 | DTO 已建，逐字段业务约束**未逐项核验** | 需要判定的字段先核验，再同批补 remarks |
| Nontax DTO 数 | `NT2` 钉 `expectedCount = 42`（核心 Payment 22 + Vehicle 8 + BillCard 12）登记进 `NontaxJsonContext`（`SerializerClassName = Nontax`） | 漂移先核对官方再同批改基线 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、金额单位、退款不可逆、二选一互斥、解密归调用方 | [IMpNontaxService.cs](./IMpNontaxService.cs) 的 XML 注释 |
| 16 条 POST 路由全带请求体 + `downloadbill` 不进管线 + 路径常量 | `NT1` = `NontaxEndpoints_ShouldMatchOfficialRoutes` |
| DTO 登记 + `expectedCount = 42` | `NT2` = `NontaxDataModels_ShouldBeRegisteredInJsonContext` |
| 下载通道结果形态（`IDisposable`、CSV 流） | [../../Nontax/MpNontaxBillDownloadTypes.cs](../../Nontax/MpNontaxBillDownloadTypes.cs) |
| 字段名与类型 | [DataModels/Nontax/Payment/](../../DataModels/Nontax/Payment)、[Vehicle/](../../DataModels/Nontax/Vehicle)、[BillCard/](../../DataModels/Nontax/BillCard) |
| 手工下载通道回退默认应用（与素材下载同规则） | `MD11` = `ManualDownloadChannels_ShouldFallBackToDefaultApp`（[MpMediaContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMediaContractGuards.cs)） |
| 生成上下文清单 | [Generated/NontaxJsonContext.g.cs](../../DataModels/Generated/NontaxJsonContext.g.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与逐页限制，**同批**更新 `NT1` 的 16 条 POST 路由表与
   `DownloadBillPath` 常量，以及 [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)
   （`RC1`/`RC2`）与 [../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = Nontax`）→ 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 同批更新 `NT2` 的 `expectedCount`（42）。
3. 确认 [Generated/NontaxJsonContext.g.cs](../../DataModels/Generated/NontaxJsonContext.g.cs) 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 调整注册面 / 模块 → 同批改 [MpModule.cs](../../Extensions/MpModule.cs)、[MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)
   的 `AddNontaxApi()`（含下载服务 `TryAddSingleton`），并同步 `QT1` 白名单（downloadbill 不入白名单）。
5. **不要**把 `downloadbill` 折进生成管线（破坏 `NT1`）；**不要**在 SDK 内解密实名数据或持有密钥；
   **不要**把金额按元填写。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。

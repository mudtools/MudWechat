# Aftersale 域 · 售后管理

微信小店 / 视频号（channels 生态）「售后管理」域：**27 支端点，全部 POST，单一前缀**
`/channels/ec/aftersale/*`，三个子族并存 —— 售后单族 + 售后原因 / 拒绝原因字典 + 纠纷单 4 + 保障单 6。
接口 [IChannelsAftersaleService.cs](./IChannelsAftersaleService.cs)，注册组 `Aftersale`
（`[HttpClientApi(RegistryGroupName = "Aftersale", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddAftersaleApi()`，模块枚举 `ChannelsModule.Aftersale`。逐页核验 2026-10-18。

**计数只以守卫为准**：
[ChannelsAftersaleContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsAftersaleContractGuards.cs)
的 CA1（`AftersaleEndpoints_ShouldMatchOfficialRoutes`）钉「**恰 27**、全 POST、单一前缀」，
CH-R2 `ExpectedRoutesByDomain["Aftersale"]` 逐条列出同 27 条。
本文件的子族划分按守卫 `AftersaleRoutes` 数组的实际分组（15 + 2 + 4 + 6 = 27）；
接口 remarks 与行 README 的分组合计与之不一致，已如实记入文末缺口第 1 条。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（Order / Logistics / Delivery）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| **A. 售后单（15）** | | | | | |
| 1 | `GetAftersaleOrderAsync` | POST | `/channels/ec/aftersale/getaftersaleorder` | `access_token`（Query） | `after_sale_order_id` 必填 |
| 2 | `GetAftersaleListAsync` | POST | `/channels/ec/aftersale/getaftersalelist` | `access_token`（Query） | **24 小时窗口**；`next_key` 翻页 |
| 3 | `AcceptAftersaleAsync` | POST | `/channels/ec/aftersale/acceptapply` | `access_token`（Query） | `accept_type` 1 / 2；同意退货须给 `address_id` |
| 4 | `RejectAftersaleAsync` | POST | `/channels/ec/aftersale/rejectapply` | `access_token`（Query） | `reject_reason_type` + `reject_reason` |
| 5 | `AcceptExchangeReshipAsync` | POST | `/channels/ec/aftersale/acceptexchangereship` | `access_token`（Query） | **共用** `ChannelsAftersaleReshipRequest` |
| 6 | `RejectExchangeReshipAsync` | POST | `/channels/ec/aftersale/rejectexchangereship` | `access_token`（Query） | `reject_certificates` 凭证 |
| 7 | `MerchantReshipAsync` | POST | `/channels/ec/aftersale/merchantreship` | `access_token`（Query） | 补寄发货：**共用** reship 请求 |
| 8 | `MerchantUpdateReshipExpressAsync` | POST | `/channels/ec/aftersale/merchantupdatereshipexpress` | `access_token`（Query） | 更新补寄物流：**共用** reship 请求 |
| 9 | `MerchantUpdateAftersaleAsync` | POST | `/channels/ec/aftersale/merchantupdateaftersale` | `access_token`（Query） | 商家协商，`merchant_update_type` 1/2/3 |
| 10 | `GenAftersaleOrderAsync` | POST | `/channels/ec/aftersale/genaftersaleorder` | `access_token`（Query） | 商家代发起；`request_id` 幂等 |
| 11 | `RefundPriceDiffAsync` | POST | `/channels/ec/aftersale/refundpricediff` | `access_token`（Query） | 退差价；`amount` 单位**分** |
| 12 | `GetExchangeableSkuListAsync` | POST | `/channels/ec/aftersale/getexchangeableskulist` | `access_token`（Query） | 可换 SKU 列表 |
| 13 | `ApplyVirtualTelNumAsync` | POST | `/channels/ec/aftersale/applyvirtualtelnum` | `access_token`（Query） | 回 `virtual_tel_number`（隐私号） |
| 14 | `HandleFastExchangeReceiptAsync` | POST | `/channels/ec/aftersale/handlefastexchangereceipt` | `access_token`（Query） | 极速换货收货 `act` |
| 15 | `UploadRefundCertificateAsync` | POST | `/channels/ec/aftersale/uploadrefundcertificate` | `access_token`（Query） | `refund_certificates` |
| **B. 原因字典（2，官方空请求体）** | | | | | |
| 16 | `GetAftersaleReasonAsync` | POST | `/channels/ec/aftersale/reason/get` | `access_token`（Query） | **禁 `[Body]`**（CA2）：拉全量售后原因 |
| 17 | `GetAftersaleRejectReasonAsync` | POST | `/channels/ec/aftersale/rejectreason/get` | `access_token`（Query） | **禁 `[Body]`**：拉全量拒绝原因 |
| **C. 纠纷单（4）** | | | | | |
| 18 | `GetComplaintOrderAsync` | POST | `/channels/ec/aftersale/getcomplaintorder` | `access_token`（Query） | `complaint_id`；回 `status` + `history` |
| 19 | `AddComplaintMaterialAsync` | POST | `/channels/ec/aftersale/addcomplaintmaterial` | `access_token`（Query） | 留言：≤ 500 字 + 总图 ≤ 20 张 |
| 20 | `AddComplaintProofAsync` | POST | `/channels/ec/aftersale/addcomplaintproof` | `access_token`（Query） | 举证：文字 ≤ 500 字 |
| 21 | `SyncWorkOrderAsync` | POST | `/channels/ec/aftersale/syncworkorder` | `access_token`（Query） | `complaint_id` + `work_order_info` |
| **D. 保障单（6）** | | | | | |
| 22 | `GetGuaranteeOrderAsync` | POST | `/channels/ec/aftersale/getguaranteeorder` | `access_token`（Query） | `guarantee_order_id`（**number**） |
| 23 | `SearchGuaranteeOrderAsync` | POST | `/channels/ec/aftersale/searchguaranteeorder` | `access_token`（Query） | `type` 0/1/2；`status_list`；`offset` + `limit` |
| 24 | `MerchantAcceptGuaranteeAsync` | POST | `/channels/ec/aftersale/merchantacceptguarantee` | `access_token`（Query） | 保障单同意 |
| 25 | `MerchantModifyGuaranteeAsync` | POST | `/channels/ec/aftersale/merchantmodifyguarantee` | `access_token`（Query） | 协商：`bad_level` 10/30/50/80/100 |
| 26 | `MerchantProofGuaranteeAsync` | POST | `/channels/ec/aftersale/merchantproofguarantee` | `access_token`（Query） | 保障单举证 |
| 27 | `MerchantRefuseGuaranteeAsync` | POST | `/channels/ec/aftersale/merchantrefuseguarantee` | `access_token`（Query） | 保障单拒绝 |

- **本域令牌是本线独立令牌**，键 `ChannelsTokenTypes.AccessToken` = `"Wechat.Channels.AccessToken"`，
  接口级 `[Token(... InjectionMode = Query, Name = "access_token")]`（守卫 CH-T1 / CH-T2，MUD005 已接受风险）。
  本域**没有**免令牌端点（对比 [../Basic/README.md](../Basic/README.md)）。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Aftersale`
  （`Order` / `Operation` / `Complaint` / `Guarantee` 四个子目录只作文件组织）；无 `IsAbstract` 父接口（CH-X2）。
- **状态值是字符串，不是数字**：售后单状态与保障单状态都是官方大写下划线字符串（`string` 常量类），
  只有纠纷单状态与保障类型 / 坏损比例是数值 —— 三族口径互不相通。

## 1. 金额单位：一律「分」

| 字段 | 所在 DTO | 口径 |
|---|---|---|
| `amount` | `ChannelsGenAftersaleOrderRequest` / `ChannelsRefundPriceDiffRequest` / `ChannelsMerchantUpdateAftersaleRequest` | 单位分；gen 场景「非换货必填」、refundpricediff「必填」、协商场景「换货 / 补寄时不填」 |
| `amount`（退款） | `ChannelsAftersaleRefundInfo` / `ChannelsAftersaleDetail` / `ChannelsAftersaleVoucher` / `ChannelsAftersaleVoucherTimes` / `ChannelsAftersaleMerchantUpdateDetail` | 单位分 |
| `platform_discount_return_amount` | `ChannelsAftersaleRefundInfo` | 平台优惠退款金额，单位分 |
| `pay_amount` | `ChannelsGuaranteeOrderInfo` | 退款金额，单位分 |
| `compensation_liability_amount` | `ChannelsAftersaleOrderInfo` | 商责额外赔付，单位分 |
| `wxa_vip_discounted_price` | `ChannelsGuaranteeOrderInfo` | 小程序会员已优惠金额，单位分（**只读回显**，勿当本线小程序接口用） |
| `refund_amount` | `ChannelsWorkOrderItem` | 单位分，终态且同意退款时有效 |

本域没有「元」口径字段，也不做本地换算；与 Delivery / Compass 同为分口径但类型不必相同，
跨域聚合勿假设可共用同一个转换函数。

## 2. 分页：`next_key` 游标与 `offset` + `limit` 两套并存

| 端点 | 请求 | 应答 | 判下一页 |
|---|---|---|---|
| #2 `getaftersalelist` | `begin_create_time` / `end_create_time` / `begin_update_time` / `end_update_time` / `next_key` | `after_sale_order_id_list` + `next_key` + `has_more` | 读 `has_more`，用应答 `next_key` 回填 |
| #23 `searchguaranteeorder` | `offset`（默认从 0 开始）+ `limit` + 多个过滤条件 | 保障单列表 + `total_num` | 传统偏移 |
| 其余 25 支 | 无分页 | 单对象 / 全量字典 | 不适用 |

⇒ **#2 拿到的只是售后单号列表**，逐个详情要再打 #1；本域没有「列表带全字段」的形态。

## 3. 时间窗口：#2 的 24 小时硬约束

官方业务限制（接口 remarks 原文）：**`end_create_time` 减 `begin_create_time` 不得大于 24 小时**，
更新范围（`begin_update_time` / `end_update_time`）同理。
本仓**不本地拦截**，越界由应答错误码表达。
注意这与 [../Delivery/README.md](../Delivery/README.md) 的「≤ 7 天」是两套窗口，不可互搬。

## 4. 幂等：只有 #10 有请求级幂等键

`genaftersaleorder`（商家代发起售后）的 `request_id` 是官方幂等键 ——
接口 remarks 原文：「**`request_id` 失败可用相同 ID 重试避免重复发起**」。
必填集：`request_id` / `order_id` / `product_id` / `sku_id` / `count` / `reason` / `type`；
**换货场景仅支持 `count = 1`** 且必填 `exchange_sku_info.new_sku_id`；非换货场景 `amount` 必填。
`refundpricediff` 的必填集是 `request_id` / `order_id` / `product_id` / `sku_id` / `amount` / `reason`。
其余写端点（#3~#9、#14、#15、#19~#21、#24~#27）**没有官方幂等键**，重复提交由状态机与应答码表达。

## 5. 条件必填：`accept_type` 决定 `address_id`，协商类型决定金额

- **#3 `acceptapply`**：`accept_type` **1 用于退货 / 换货、2 用于仅退款 / 收到货**；
  官方原文「同意退货时传 `address_id`，否则后续处理缺地址请求将报错」⇒ 条件必填不本地拦截。
- **#4 `rejectapply`**：`reject_reason_type`（`int`，取自 #17 字典）+ `reject_reason`（可空文案）。
- **#9 `merchantupdateaftersale`**：`merchant_update_type` **1 已协商一致 / 2 邀请核实补充凭证 / 3 修改买家售后申请**；
  换货场景必填 `new_sku_id` 与 `product_cnt`，且 **`product_cnt` 必须填 1**；金额在换货 / 补寄时不填。
- **#10 / #11**：见第 4 节。
- **#25 `merchantmodifyguarantee`**：`guarantee_order_id`（`long`）与 `bad_level`（`int`，
  **可填 10 / 30 / 50 / 80 / 100**，即商家修改坏损比例）必填。
- **#19 / #20**：留言与举证的文字**最多 500 字**；官方原文「所有留言总图片数最多 **20 张**」，
  图片走 `media_id_list`（素材 id 由上传面取得，本域不含上传端点）。

## 6. 三个状态机族（CA6 锁定，含官方历史拼写）

| 常量类 | 形态 | 关键取值 |
|---|---|---|
| [ChannelsAftersaleStatuses.cs](../../../Mud.Wechat.Channels/Aftersale/Order/ChannelsAftersaleStatuses.cs) | `string` × 23 | `USER_CANCELD`（**官方历史拼写 Canceld，照抄勿「纠正」**）、`MERCHANT_PROCESSING`、`MERCHANT_REJECT_REFUND` / `..._RETURN` / `..._EXCHANGE` / `..._RESHIP`、`USER_WAIT_RETURN` / `USER_WAIT_RECEIPT` / `USER_WAIT_CONFIRM` / `USER_WAIT_CONFIRM_UPDATE` / `USER_WAIT_HANDLE_MERCHANT_AFTER_SALE`、`RETURN_CLOSED`、`MERCHANT_WAIT_RECEIPT`、`MERCHANT_OVERDUE_REFUND`、`MERCHANT_REFUND_SUCCESS`、`MERCHANT_RETURN_SUCCESS`、`MERCHANT_EXCHANGE_SUCCESS`、`MERCHANT_FAIL`、`MERCHANT_REFUND_RETRY_FAIL`、`PLATFORM_REFUNDING`、`PLATFORM_REFUND_FAIL`、`WAIT_PACKAGE_INTERCEPT` |
| [ChannelsAftersaleTypes.cs](../../../Mud.Wechat.Channels/Aftersale/Order/ChannelsAftersaleTypes.cs) / [ChannelsAftersaleSubTypes.cs](../../../Mud.Wechat.Channels/Aftersale/Order/ChannelsAftersaleSubTypes.cs) | `string` | 类型 `REFUND` 仅退款 / `RETURN` 退货退款 / `EXCHANGE` 换货 / `RESHIP` 补寄；子类型 `DEFAULT` / `REFUND_PRICE_DIFF` |
| [ChannelsAftersaleReasons.cs](../../../Mud.Wechat.Channels/Aftersale/Order/ChannelsAftersaleReasons.cs) | `string` × 14 | `INCORRECT_SELECTION`（选错/多选）… `NO_REASON_7_DAYS`（七天无理由）、`INITIATE_BY_PLATFORM`（平台发起）、`OTHERS` |
| [ChannelsComplaintStatuses.cs](../../../Mud.Wechat.Channels/Aftersale/Complaint/ChannelsComplaintStatuses.cs) | `int` × 19 | 100 待商家处理 / 101 待客服处理 / 102 客服终止介入 / 103~109 各类待举证 / 111~116 调解与退款 / **305 用户举证超时 / 307 商家举证超时** / 400 待商家处理工单（**数值不连续**） |
| [ChannelsGuaranteeStatuses.cs](../../../Mud.Wechat.Channels/Aftersale/Guarantee/ChannelsGuaranteeStatuses.cs) | `string` × 14 | `STATUS_WAIT_MERCHANT_HANDLE` … `STATUS_WAIT_OP_COMFIRM`（**官方历史拼写 COMFIRM，照抄勿「纠正」**）、`STATUS_PAY_SUCC` / `STATUS_PAY_FAIL` / `STATUS_USER_CANCEL` |
| [ChannelsGuaranteeTypes.cs](../../../Mud.Wechat.Channels/Aftersale/Guarantee/ChannelsGuaranteeTypes.cs) | `int` | `FakeOnePayFour = 1`、`BadPay = 2` |

CA6 用 `GetRawConstantValue()` 逐个字面量断言，**改名与改值都会红**；
`USER_CANCELD` / `STATUS_WAIT_OP_COMFIRM` 两处拼写是官方原文，不得「顺手修正」。

## 7. 共用 / 分建 DTO 裁决（CA4 / CA5）

DTO 总数由 CA4 钉为**恰 70**（售后单结构族 24 + 操作请求族 16 + 纠纷单族 13 + 保障单族 17），
另有**静态常量类恰 7**（售后状态 / 售后原因 / 售后类型 / 售后子类型 / 纠纷状态 / 保障状态 / 保障类型），
常量类**不得**标 `[HttpJsonSerializable]`。

| 裁决 | 内容 |
|---|---|
| 共用（正向） | #5 / #7 / #8 三支的「快递公司 + 运单号」发货形态一致 ⇒ **必须**共用 [ChannelsAftersaleReshipRequest.cs](../../../Mud.Wechat.Channels/Aftersale/Operation/ChannelsAftersaleReshipRequest.cs)（CA5 逐支反射断言），重复建模即红 |
| 分建（负向） | #19 `addcomplaintmaterial`（留言）与 #20 `addcomplaintproof`（举证）当前字段集一致（`complaint_id` / `content` / `media_id_list`），但是**两个独立官方端点、参数表可独立演化** ⇒ 按端点分建，不合并 |
| 响应继承 | 纯 `errcode` / `errmsg` 端点直接复用基线 `ChannelsResponse`；CA5 断言 `ChannelsGetAftersaleReasonResponse` 与 `ChannelsGetGuaranteeOrderResponse` 均 `IsAssignableTo(ChannelsResponse)` |
| id 类型 | `after_sale_order_id` 是 `string`（#1 / #3 / #4 / #13…），而 `guarantee_order_id` 是 `long`（#22 / #25）、`complaint_id` 是 `string`（#18~#21）⇒ 三族 id 不得互转或共用一个「售后单号」封装 |

## 8. 保障单：两种保障 + 一个必须记住的名词冲突

- 查询面 #22 / #23，处置面 #24 同意 / #25 协商 / #26 举证 / #27 拒绝。
- #23 过滤条件：`type` **0 全部 / 1 假一赔**（常量 `FakeOnePayFour`）/ 2 坏损包退（`BadPay`）；
  `status_list` 是**字符串列表**（`List<string>?`，取 `ChannelsGuaranteeStatuses` 的值），
  `guarantee_order_id_list` 与 `order_id_list` 是 `List<long>?`。
- 应答里两种保障各自的详情结构分建：`fake_one_pay_four_info`（`ChannelsGuaranteeFakeOnePayFourInfo`）
  与 `bad_pay_info`（`ChannelsGuaranteeBadPayInfo`），CA5 锁 JSON 名。
- **名词冲突**：常量与字段名都是「赔**四**」（`FakeOnePayFour` = 1、`fake_one_pay_four_info`），
  而接口 remarks 多处写「假一赔**三**」⇒ 本文件以 CA6 / CA5 锁定的**代码事实**为准，
  该矛盾已记入文末缺口第 2 条，未核对官方前不得改任一侧。

## 9. 与既有六线 / 邻域的路由交叠红线

- CH-R1 的重叠白名单只有「`/cgi-bin/token` + `/cgi-bin/stable_token` +
  [../Basic/README.md](../Basic/README.md) 的 8 条 `/cgi-bin/*`」⇒ 本域 27 条 `/channels/ec/aftersale/*`
  不与任何其它线交叠。
- CA1 的**单一前缀**约束：本域 27 条全部以 `/channels/ec/aftersale/` 开头，任何「顺手」把
  退款 / 赔付类端点落到 `/channels/ec/order/*` 或 `/channels/ec/funds/*` 都属跨域（分属 Order / Funds 域）。
- CH-V1 禁止本线任何 `/wxa/` 路由 —— 本域 `wxa_vip_discounted_price` 只是**应答字段名**，
  不构成任何 `/wxa/` 请求路由，不得据此新增端点。
- 换货 / 补寄的**发货**（#5 / #7）在本域；运单与包裹的完整履约面在 Logistics 域，
  代发场景在 [../Delivery/README.md](../Delivery/README.md)，三处不得互相代答。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Aftersale;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddAftersaleApi());

var aftersale = app.Services.GetRequiredService<IChannelsAftersaleService>();

// ① 拉待处理售后单（#2：窗口 ≤ 24 小时；返回的只是单号列表）。
var ids = await aftersale.GetAftersaleListAsync(
    new ChannelsGetAftersaleListRequest
    {
        BeginCreateTime = 1760000000,          // 秒级时间戳，窗口不得大于 24 小时
        EndCreateTime = 1760086400,
        NextKey = null,                        // 首页传空
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(ids, "/channels/ec/aftersale/getaftersalelist");
var nextKey = ids.NextKey;                     // 下一页靠应答回填，不自己算
var hasMore = ids.HasMore ?? false;

// ② 取单个售后单详情。
var order = await aftersale.GetAftersaleOrderAsync(
    new ChannelsGetAftersaleOrderRequest { AfterSaleOrderId = ids.AfterSaleOrderIdList?[0] ?? string.Empty }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(order, "/channels/ec/aftersale/getaftersaleorder");
string status = order.AfterSaleOrder?.Status ?? string.Empty;        // 官方大写下划线字符串
long refundInFen = order.AfterSaleOrder?.RefundInfo?.Amount ?? 0L;   // 单位：分

// ③ 同意退货退款（accept_type=1 且必须给退货地址，否则后续处理报错）。
var accepted = await aftersale.AcceptAftersaleAsync(
    new ChannelsAcceptAftersaleRequest
    {
        AfterSaleOrderId = order.AfterSaleOrder!.AfterSaleOrderId!,
        AcceptType = 1,
        AddressId = "<占位 address_id>",
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(accepted, "/channels/ec/aftersale/acceptapply");

// ④ 拒绝：拒绝原因类型取自 #17 的字典（本端点官方空请求体，不传 [Body]）。
var reasons = await aftersale.GetAftersaleRejectReasonAsync(ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(reasons, "/channels/ec/aftersale/rejectreason/get");

// ⑤ 补寄发货：#5 / #7 / #8 共用同一个 reship 请求类型（CA5 裁决，不得各建一份）。
var reship = await aftersale.MerchantReshipAsync(
    new ChannelsAftersaleReshipRequest
    {
        AfterSaleOrderId = order.AfterSaleOrder!.AfterSaleOrderId!,
        DeliveryId = "<占位 快递公司 id>",
        WaybillId = "<占位 快递单号>",
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(reship, "/channels/ec/aftersale/merchantreship");

// ⑥ 保障单协商：bad_level 只能取 10/30/50/80/100。
var negotiated = await aftersale.MerchantModifyGuaranteeAsync(
    new ChannelsMerchantModifyGuaranteeRequest { GuaranteeOrderId = 20261018001, BadLevel = 30 }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(negotiated, "/channels/ec/aftersale/merchantmodifyguarantee");
```

**判错只有一层**：应答均为 `ChannelsResponse` 信封（`errcode == 0` 即 `IsSuccess`）。
#16 / #17 两支是**官方空请求体**，方法签名只有 `CancellationToken` —— 造一个空请求体 DTO 即与 CA2 冲突。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **子族计数三处不一致** | 权威计数只有 CA1 的「恰 27」；守卫数组内的分组注释写「售后单（14）」但实列 15 条，接口 remarks 写「售后单 16 / 纠纷单 4 / 保障单 6 + 原因 / 拒绝原因」（合计 28），行 README 写「15（含原因/拒绝原因）+ 4 + 6 + 拒绝原因」（合计含糊） | 以 CA1 / CH-R2 为准；**不要**按任何分组口径增删端点。统一口径需先逐页核验，再同批改 remarks 与行 README |
| **保障类型名词冲突** | 代码与 CA5/CA6 是「赔**四**」（`FakeOnePayFour` = 1、`fake_one_pay_four_info`），接口 remarks 多处写「赔**三**」 | 逐页核验官方保障单页后统一；核对前不得改动常量名或 JSON 字段名 |
| 本域官方错误码 | 27 支 remarks **未摘录**专属错误码，本域也没有错误码常量类（对比 Basic 的 CB6） | 按码分支前先逐页核验，再建常量并同批改守卫 |
| 本域频率限制 | 各页官方频率段**未在本仓摘录** ⇒ 无本地计数依据 | 需要限流时逐页核验，勿按邻域经验值建闸 |
| `media_id_list` 的素材上传面 | #19 / #20 只收素材 id，取得方式（属哪个域、是否即「上传资质图片」）**未逐项核验** | 核验后补 remarks 的交叉引用 |
| `address_id` 的来源端点 | #3 要求退货地址 id，其获取端点属 Logistics / Shop 哪一面**未在本域留档** | 核验后补跨域引用，勿在封装里硬编码地址 |
| `act`（#14 极速换货收货）取值全集 | 字段存在，可取值**未逐项核验** | 需要判定操作类型时先核验官方页，勿从邻字段推断 |
| 纠纷工单 `work_order_info` 结构约束 | `ChannelsWorkOrderInfo` / `ChannelsWorkOrderItem` 已建模，官方对状态迁移与字段必填组合**未逐项核验** | 接入工单同步前逐页核验 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、24 小时窗口、500 字 / 20 张、`request_id` 幂等原文 | [IChannelsAftersaleService.cs](./IChannelsAftersaleService.cs) 的 XML 注释 |
| 路由 / 计数（**恰 27**）/ 全 POST / 单一前缀 | [ChannelsAftersaleContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsAftersaleContractGuards.cs) CA1（`AftersaleEndpoints_ShouldMatchOfficialRoutes`） |
| 两支空请求体禁 `[Body]` + 其余 25 支必须带 `[Body]` | 同上 CA2（`AftersaleEndpoints_ShouldAllBePost_WithEmptyBodyRules`） |
| 令牌绑定与注册组 `Aftersale` | 同上 CA3 |
| DTO 计数（恰 70）+ 常量类（恰 7，不得标序列化特性） | 同上 CA4（`AftersaleDataModels_ShouldBeRegisteredInJsonContext`） |
| JSON 字段名、reship 三支共用、留言/举证分建、响应继承基线 | 同上 CA5（`AftersaleDataModels_ShouldLockOfficialFieldsAndSharedDtos`） |
| 状态 / 类型 / 原因 / 保障字面量（含 `USER_CANCELD`、`STATUS_WAIT_OP_COMFIRM`） | 同上 CA6（`AftersaleEnums_ShouldLockOfficialLiteralValues`） |
| 全线逐域路由表与重叠白名单 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CA1 的 `AftersaleRoutes` 与计数（**恰 27**）、
   CH-R2 `ExpectedRoutesByDomain["Aftersale"]`；CA1 的「单一前缀」断言同时约束新端点必须落
   `/channels/ec/aftersale/`。
2. 空请求体名单（#16 / #17）由 CA2 持有；官方若给这两支补参数表，改接口 + CA2 名单 + 本文速览必须同批。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Aftersale")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CA4 计数（70）；
   新增 / 删除静态常量类同批改 CA4 的常量类计数（7）。
   **漏登记在 AOT 首次序列化才失败**（CH-X5 的合并列表就是防这一手）；序列化上下文多为 `internal`，不得在宿主示例里引用。
4. 改状态字面量 → CA6 的 `GetRawConstantValue()` 断言逐条同批改；
   `USER_CANCELD` / `STATUS_WAIT_OP_COMFIRM` 两处官方历史拼写**不得修正**。
5. 想把 #5 / #7 / #8 的 reship 请求拆成三份，或把 #19 / #20 合并成一份 → 先推翻 CA5 的对应裁决，
   并附官方两页 / 三页的逐字段比对证据。
6. 改注册面 → `ChannelsModule.Aftersale`、`ChannelsServiceBuilder.AddAftersaleApi()`（L170）与本域同批。
7. **不要**给本域加免令牌端点或第二套令牌（CH-T1）；**不要**新增 `/wxa/` 路由（CH-V1）；
   **不要**把 24 小时窗口当 7 天用；**不要**在本文件或示例里写出令牌真值、真实单号或真实手机号 / 虚拟号。
8. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。

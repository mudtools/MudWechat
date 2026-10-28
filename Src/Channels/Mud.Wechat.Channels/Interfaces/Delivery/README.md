# Delivery 域 · 代发与供货

微信小店 / 视频号（channels 生态）「代发与供货」域：**23 支端点，全部 POST**，四个前缀族并存 ——
`/channels/ec/supplier/relation/*` 9（供货商关联 3 + 自动分配 6）+
`/channels/ec/supplier/base/*` 1（供货商账户）+
`/channels/ec/order/dropship/*` 10（代发单：商家视角 5 + 供货商视角 5）+
`/channels/ec/order/supplyorder/*` 3（供货单）。
接口 [IChannelsDeliveryService.cs](./IChannelsDeliveryService.cs)，注册组 `Delivery`
（`[HttpClientApi(RegistryGroupName = "Delivery", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddDeliveryApi()`，模块枚举 `ChannelsModule.Delivery`。逐页核验 2026-10-18。

计数、前缀配比与「`/channels/ec/order/*` 下只允许 dropship / supplyorder」由守卫
[ChannelsDeliveryContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsDeliveryContractGuards.cs)
（CD1~CD6）与 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
（CH-R1 / CH-R2）锁定：CD1 钉「恰 23 = 供货商关系 9 + 供货商基础 1 + dropship 10（其中 supplier 子族 5）+ supplyorder 3」。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（Order / Logistics / Aftersale）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| **A. 供货商关联（商家视角，3）** | | | | | |
| 1 | `GetSupplierListAsync` | POST | `/channels/ec/supplier/relation/get_supplier_list` | `access_token`（Query） | `offset` / `limit` 选填 |
| 2 | `GetEcListAsync` | POST | `/channels/ec/supplier/relation/get_ec_list` | `access_token`（Query） | `offset` / `limit` **必填**；`is_filter_type=true` 时 `type` 必填 |
| 3 | `InviteSupplierAsync` | POST | `/channels/ec/supplier/relation/invite_supplier` | `access_token`（Query） | 第三方代调用时 `service_id` 必填且校验调用方一致 |
| **B. 自动分配（供货商视角，6）** | | | | | |
| 4 | `GetDistributeAsync` | POST | `/channels/ec/supplier/relation/get_distribute` | `access_token`（Query） | **官方空请求体**（CD2 禁 `[Body]`） |
| 5 | `SetManuallyDistributeAsync` | POST | `/channels/ec/supplier/relation/set_manually_distribute` | `access_token`（Query） | **官方空请求体** |
| 6 | `SetAllDistributionAsync` | POST | `/channels/ec/supplier/relation/set_all_distribution` | `access_token`（Query） | 全店自动分配给某供货商 |
| 7 | `SetProductDistributeAsync` | POST | `/channels/ec/supplier/relation/set_product_distribute` | `access_token`（Query） | `list` **≤ 200** |
| 8 | `GetProductDefaultDistributeAsync` | POST | `/channels/ec/supplier/relation/get_product_default_distribute` | `access_token`（Query） | 商品默认分配方式 |
| 9 | `GetProductListAsync` | POST | `/channels/ec/supplier/relation/get_product_list` | `access_token`（Query） | 可分配商品列表 |
| **C. 供货商账户（1）** | | | | | |
| 10 | `GetSupplierBaseInfoAsync` | POST | `/channels/ec/supplier/base/get_supplier_base_info` | `access_token`（Query） | **官方空请求体**；回 `business_mode` |
| **D. 代发单 · 商家视角（5）** | | | | | |
| 11 | `AssignDropshipAsync` | POST | `/channels/ec/order/dropship/assign` | `access_token`（Query） | `order_id` + `supplier_appid` + 商品明细 |
| 12 | `CancelDropshipAsync` | POST | `/channels/ec/order/dropship/cancel` | `access_token`（Query） | `cancel_scene` 用**代发侧**常量 |
| 13 | `GetDropshipOrderAsync` | POST | `/channels/ec/order/dropship/get` | `access_token`（Query） | `ds_order_id` 必填 |
| 14 | `GetDropshipOrderListAsync` | POST | `/channels/ec/order/dropship/list` | `access_token`（Query） | 时间范围 ≤ 7 天；`limit` ≤ 100 |
| 15 | `SearchDropshipOrdersAsync` | POST | `/channels/ec/order/dropship/search` | `access_token`（Query） | `search_condition` 必填；**`limit` ≤ 10**；`waybill_id` 只可独立查询 |
| **E. 供货单（3）** | | | | | |
| 16 | `GetSupplyOrderListAsync` | POST | `/channels/ec/order/supplyorder/list` | `access_token`（Query） | 时间范围至少填一个且 ≤ 7 天 |
| 17 | `GetSupplyOrderAsync` | POST | `/channels/ec/order/supplyorder/get` | `access_token`（Query） | `supply_order_id` 必填 |
| 18 | `SearchSupplyOrdersAsync` | POST | `/channels/ec/order/supplyorder/search` | `access_token`（Query） | `limit` ≤ 100 |
| **F. 代发单 · 供货商视角（5）** | | | | | |
| 19 | `GetSupplierDropshipOrderAsync` | POST | `/channels/ec/order/dropship/supplier/get` | `access_token`（Query） | **复用**商家侧 `ChannelsDropshipGetRequest`；应答分建 |
| 20 | `GetSupplierDropshipOrderListAsync` | POST | `/channels/ec/order/dropship/supplier/list` | `access_token`（Query） | 应答与 #14 同形 ⇒ 共用 |
| 21 | `SearchSupplierDropshipOrdersAsync` | POST | `/channels/ec/order/dropship/supplier/search` | `access_token`（Query） | 搜索条件按视角**分建**（`shop_*` vs `supplier_*`） |
| 22 | `SendSupplierDeliveryAsync` | POST | `/channels/ec/order/dropship/supplier/delivery/send` | `access_token`（Query） | `ds_order_id` 与 `supply_order_id` 选传其一；`delivery_list` 必填 |
| 23 | `UpdateSupplierDeliveryAsync` | POST | `/channels/ec/order/dropship/supplier/delivery/update` | `access_token`（Query） | **只允许改运单号**，改次有限 |

- **令牌口径**：23 支统一由接口级 `[Token(TokenType = ChannelsTokenTypes.AccessToken, InjectionMode = Query, Name = "access_token")]`
  注入（守卫 CH-T1 / CH-T2），键为 `"Wechat.Channels.AccessToken"`；**本域没有免令牌端点**
  （对比 [../Basic/README.md](../Basic/README.md) 的 `clear_quota/v2`）。官方契约强制 Query ⇒ MUD005 已接受风险，
  令牌值不得进日志、遥测或异常消息。
- **两个视角、一套声明面**：#A/#C 与 #D 是**商家（小店）视角**（权限集 `131`），
  #B/#F 与 #E 是**供货商视角**（权限集 `192`）—— 接口 remarks 逐支标注适用账号类型；
  两视角在同一接口上并存，靠路由子族区分，不靠令牌类型区分。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Delivery`
  （`Supplier` / `Dropship` / `SupplyOrder` 三个子目录 + 三个跨视角共用文件
  `ChannelsOrderExtInfo.cs` / `ChannelsSkuAttr.cs` / `ChannelsSkuDeliverInfo.cs`）；无 `IsAbstract` 父接口（CH-X2）。

## 1. 金额单位：全域「分」

| 字段 | 所在 DTO | 官方口径 |
|---|---|---|
| `total_price`（供货单总价 = 售价 × 数量 + 运费） | `ChannelsSupplyPriceInfo` | 单位分 |
| `sale_price`（售价单价） | `ChannelsSupplyProductInfo` | 单位分 |
| `supply_price`（供货价单价） | `ChannelsSupplyOrderProductInfo` | 单位分 |
| `freight`（运费） | `ChannelsSupplyOrderDetail` | 单位分 |

本域**没有任何「元」口径字段**，也不做本地换算；与 [../Aftersale/README.md](../Aftersale/README.md)、
[../Compass/README.md](../Compass/README.md) 同为分口径，但**类型不统一**（本域是数字、
罗盘多为 string），跨域聚合时不得共用同一个金额转换器假设。

## 2. 分页：三套游标形态共存，且 `offset` 已非必填

| 端点族 | 请求 | 应答 | 官方口径 |
|---|---|---|---|
| #1 供货商列表 / #9 商品列表 / #8 | `offset` + `limit`（可空 `int?`） | `list` + `total_count` | 传统偏移 |
| #2 商家侧关联列表 | `offset` + `limit`（**`long` 必填**） | `list` + 总数 | 与 #1 类型口径不同（`long` vs `int?`） |
| #14/#15/#16/#18/#20/#21 拉单与搜单 | `page_info{offset?, limit, next_key}` | `ds_order_id_list` / `supply_order_id_list` + `next_page` + `total` + `has_more` | **推荐 `next_key` 翻页**：首页传空，后续用应答 `next_page` 回填（代替 `offset`，官方称性能更优） |

`page_info` 请求对象与应答 `next_page` 字段集一致 ⇒ CD5 裁决**共用** `ChannelsDeliveryPageInfo`
（变更日志 2026-06-23：`offset` 改为非必填、新增 `next_key`）。
`limit` 上限分档：**#15 是 ≤ 10**，其余拉单 / 搜单端点是 ≤ 100 —— 别把一个端点的风筝值搬到另一个。

## 3. 时间范围：一律秒级时间戳，且 ≤ 7 天

`ChannelsDeliveryTimeRange`（`start_time` / `end_time`）被四个拉单 / 搜单端点共用（CD5 裁决 ②）。
硬约束（接口 remarks 原文）：① 各时间范围起止距离**不可超过 7 天**；② #16 `supplyorder/list` 还要求
**时间范围至少填一个**；③ 变更日志 2025-12-25 新增 `update_time_range`，2026-06-23 起 `create_time_range`
与 `update_time_range` 并存。本仓不做本地窗口校验，越界由应答错误码表达。

## 4. 幂等与覆盖语义

- **#6 / #7 是「设置」不是「追加」**：`set_all_distribution` 按 `supplier_appid` 设定全店自动分配；
  `set_product_distribute` 的 `list[]` 每项带 `is_distribute`（0 已分配 / 1 未分配，
  `ChannelsProductDistributeStatuses`）⇒ **同一 item 重复提交是覆盖**，不是幂等增量。
- **赠品连带**：商品默认会**强制把自动分配关系同步到赠品**；要单独给赠品设代发关系，
  必须显式 `apply_to_gift = false`（接口 remarks 第 ③ 条）。默认值即改变行为，属于必踩坑。
- **#11 assign / #12 cancel**：官方未提供请求级幂等键（无 `request_id`）；
  重复指派 / 重复取消由应答错误码表达，本仓不本地拦截。
- **#23 update 只改运单号**：官方原文「只允许修改运单号，商品信息必须保持一致，且修改次数有限」，
  超限码 `606041`（修改物流次数已达上限）；且**只支持改成通过微信小店电子面单取的号**，
  外部运单号不支持（`606046` / `606059`）。

## 5. 条件必填与「官方参数表矛盾」照录

- **#22 / #23**：`ds_order_id` 与 `supply_order_id` 官方**同标「必填 = 是」，说明却写「选传其一」**
  （2026-05-18 变更新增 `supply_order_id` 支持供货单发货 / 改物流）。
  本仓**不本地拦截、也不替调用方二选一**：两个字段都是 `string?`，
  组合错配由应答（`40097` / `47001` / `48001` 等）表达。这是 CD5 明确记录的官方矛盾。
- **#2 `get_ec_list`**：`offset` / `limit` 必填；`is_filter_type = true` 时 `type` 必填。
- **#3 `invite_supplier`**：第三方代调用时 `service_id` 必填，且平台校验它与实际调用方一致。
- **#7 `set_product_distribute`**：`list` 必填、**数量 ≤ 200**；只对**新增订单**生效，
  存量订单维持原有分配方式（接口 remarks 第 ② 条）—— 这不是「改了就全量重分」。
- **#15 / #21 `waybill_id`**：官方限定「**只可独立查询**」，不得与其它条件同传；
  本仓不拦，越界由应答表达。

## 6. 两套 `cancel_scene`：同值不同义，禁止合并

CD6 点名「同值不同义防串线」：

| 常量类 | 1 | 2 |
|---|---|---|
| [ChannelsDropshipCancelScenes.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsDropshipCancelScenes.cs)（**代发侧**） | 售后 | 商家取消代发 |
| [ChannelsSupplyCancelScenes.cs](../../../Mud.Wechat.Channels/Delivery/SupplyOrder/ChannelsSupplyCancelScenes.cs)（**供货侧**） | 用户售后 | **平台砍单** |

数值完全相同、语义不同 ⇒ 两常量类不得合并、不得在跨端点封装里共用一个「取消场景」枚举。

其余 CD6 锁定的状态机：

| 常量类 | 取值 |
|---|---|
| [ChannelsDropshipOrderStatuses.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsDropshipOrderStatuses.cs) / [ChannelsSupplyOrderStatuses.cs](../../../Mud.Wechat.Channels/Delivery/SupplyOrder/ChannelsSupplyOrderStatuses.cs) | 100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消（供货单多一档 **400 已完成**） |
| [ChannelsDeliveryStatuses.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsDeliveryStatuses.cs)（**包裹级**） | 10 / 19 / 20 / 30 —— 与单据级 100/199/200/300 是两套数值，勿混用 |
| [ChannelsSupplierStatuses.cs](../../../Mud.Wechat.Channels/Delivery/Supplier/ChannelsSupplierStatuses.cs) | 0 初始 / 1 已绑定 / 2 已解绑 |
| [ChannelsSupplierBindAuditStatuses.cs](../../../Mud.Wechat.Channels/Delivery/Supplier/ChannelsSupplierBindAuditStatuses.cs) | 0 待处理 / 1 已同意 / 2 已拒绝 / 3 超时拒绝 / 4 商家取消 / 5 初始 |
| [ChannelsDistributeTypes.cs](../../../Mud.Wechat.Channels/Delivery/Supplier/ChannelsDistributeTypes.cs) | 1 手动 / 2 全店自动 / 3 按商品自动 |
| [ChannelsSupplyAftersaleStatuses.cs](../../../Mud.Wechat.Channels/Delivery/SupplyOrder/ChannelsSupplyAftersaleStatuses.cs) | 0 无售后或售后关闭 / 1 售后中 / 2 售后完成 |
| [ChannelsStockTypes.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsStockTypes.cs) / [ChannelsPresaleDeliveryTypes.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsPresaleDeliveryTypes.cs) / [ChannelsCustomTypes.cs](../../../Mud.Wechat.Channels/Delivery/Dropship/ChannelsCustomTypes.cs) | 库存 0 现货 / 1 全预售；预售发货 0 付款后天数 / 1 预售结束后天数；定制 1 文字 / 2 图片 |

`get_supplier_base_info` 的 `subject_type` 是 **2 企业 / 2500 个人**（CD5），
`business_mode.is_supply`（可用供货模式）**仅企业主体**可用。

## 7. 两个视角的字段与类型差异（分建，不许「对齐」）

CD5 同时给出正向复用与负向分建两组裁决：

| 面 | 裁决 | 依据 |
|---|---|---|
| `get` 请求体 | **共用** `ChannelsDropshipGetRequest`（#13 与 #19 同一类型） | 官方参数表同形（`ds_order_id` string 必填） |
| `list` / `search` 应答 | **共用** `ChannelsDropshipOrderListResponse`（#14/#15/#20/#21 同一返回类型） | 应答同形 |
| `page_info` / 时间范围 / 发货嵌套 9 支 / `ext_info` / 贺卡 / 定制 / `sku_deliver_info` / `sku_attrs` 元素 | **共用**同一批类型 | 子树逐字段一致 ⇒ 复制分叉即双源漂移 |
| `get` **应答** | **分建**：`ChannelsDropshipGetResponse` vs `ChannelsSupplierDropshipGetResponse` | 商家标识 `supplier_*` vs 供货商视角 `shop_appid`；单据号 number vs string |
| 商品明细 | **分建**：供货商侧含 `sku_attrs`、id 为 string | 视角参数表不同 |
| `dropship_info` | **分建**：本视角 `ds_order_id` 为 string | 类型口径不同 |
| 搜索条件 | **分建** `ChannelsDropshipSearchCondition` / `ChannelsSupplierDropshipSearchCondition` / `ChannelsSupplyOrderSearchCondition` | 当前字段集虽一致，但供货侧官方示例已出现表外字段（`tel_last4` / `user_name`），预告可独立演化 |
| 发货包裹 | **分建** `ChannelsSupplierDeliveryPackage`（无 `deliver_type` / `course_info` / `sn_info`） | 本视角字段集少于商家侧；`old` / `new` 两侧结构一致 ⇒ 同一类型复用 |

⇒ 写跨视角聚合代码时，**单据号 id 的 string / number 差异**必须显式转换；
DTO 的 JSON 名与类型由 CD5 逐个锁定，改动即断链。

## 8. 批量上限与「没有的接口」

- 本域唯一批量面是 **#7 `set_product_distribute`：`list` ≤ 200**；其余端点单值操作。
- **没有解除关联的 API**：官方原文「暂无解除关联的 API，需登录商家后台手动操作」
  （商家侧 #A 族 remarks 第 ④ 条），供货商侧对称地「官方暂不提供解除商家关联的 API」（第 ① 条）。
  ⇒ 不要给本域加 `unbind` 类端点，也不要在封装里假装能回滚绑定。
- **没有删除 / 撤回类端点**：#12 cancel 只取消代发指派；发货后的处置属于
  [../Aftersale/README.md](../Aftersale/README.md) 与 Order / Logistics 域。

## 9. 与既有六线 / 邻域的路由交叠红线

- CH-R1 的重叠白名单只有「`/cgi-bin/token` + `/cgi-bin/stable_token` +
  [../Basic/README.md](../Basic/README.md) 的 8 条 `/cgi-bin/*`」⇒ 本域 23 条路由**不得**与其余六线交叠。
- **CD1 的域间红线**（本域最重要的一条）：`/channels/ec/order/*` 下**只允许** `dropship/*` 与 `supplyorder/*`
  两个子族，其余（`order/deliveryinfo`、`order/deliverynegotiation` 等**履约端点归 Order 域**）计数必须为 0。
  ⇒ 「代发」和「发货 / 协商」不是一个域，顺手把履约搬进 Delivery 会让 CD1 直接红。
- CH-V1 禁止本线任何 `/wxa/` 路由。
- `/channels/ec/supplier/*` 前缀本域独占；`/channels/ec/order/dropship/supplier/*` 是
  「同一前缀下的视角子族」，与 `/channels/ec/supplier/*` 不同源，不可互换。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Delivery;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddDeliveryApi());

var delivery = app.Services.GetRequiredService<IChannelsDeliveryService>();

// ① 商家视角：已关联供货商（#1，offset/limit 选填）。
var suppliers = await delivery.GetSupplierListAsync(
    new ChannelsGetSupplierListRequest { Offset = 0, Limit = 50 }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(suppliers, "/channels/ec/supplier/relation/get_supplier_list");
string supplierAppid = suppliers.List?[0].Appid ?? string.Empty;

// ② 供货商视角：自己的账户与可用经营模式（#10 是官方空请求体，方法只有 cancellationToken）。
var baseInfo = await delivery.GetSupplierBaseInfoAsync(ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(baseInfo, "/channels/ec/supplier/base/get_supplier_base_info");
bool canSupply = baseInfo.BusinessMode?.IsSupply ?? false;   // 仅企业主体可用

// ③ 按商品设代发关系：list ≤ 200；只对新增订单生效；赠品默认连带，需显式关掉。
await delivery.SetProductDistributeAsync(
    new ChannelsSetProductDistributeRequest
    {
        List =
        [
            new ChannelsProductDistributeItem
            {
                SupplierAppid = supplierAppid,
                ProductId = 100001,
                IsDistribute = true,
                ApplyToGift = false,          // 不给赠品沿用同一关系
            },
        ],
    }, ct).ConfigureAwait(false);

// ④ 指派代发（#11：order_id + supplier_appid + 商品明细）。
var assigned = await delivery.AssignDropshipAsync(
    new ChannelsDropshipAssignRequest
    {
        OrderId = "<占位 order_id>",
        SupplierAppid = supplierAppid,
        DropshipProductList = [ new ChannelsDropshipProduct() ],
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(assigned, "/channels/ec/order/dropship/assign");

// ⑤ 搜单（#15：search_condition 必填、limit ≤ 10、waybill_id 只可独立查询、时间范围 ≤ 7 天）。
var searched = await delivery.SearchDropshipOrdersAsync(
    new ChannelsDropshipSearchRequest
    {
        SearchCondition = new ChannelsDropshipSearchCondition
        {
            Status = ChannelsDropshipOrderStatuses.PendingDelivery,     // 100
            CreateTimeRange = new ChannelsDeliveryTimeRange { StartTime = 1760000000, EndTime = 1760604800 },
        },
        PageInfo = new ChannelsDeliveryPageInfo { Limit = 10, NextKey = null },  // 首页 NextKey 传空
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(searched, "/channels/ec/order/dropship/search");
// 下一页：把应答 next_page 的 next_key 回填进请求，不要自己算 offset。
var nextPage = searched.NextPage;

// ⑥ 供货商发货：ds_order_id 与 supply_order_id「选传其一」是官方矛盾，本仓不拦。
var sent = await delivery.SendSupplierDeliveryAsync(
    new ChannelsSupplierDeliverySendRequest
    {
        DsOrderId = "<占位 ds_order_id>",
        DeliveryList = [ new ChannelsSupplierDeliveryPackage() ],
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(sent, "/channels/ec/order/dropship/supplier/delivery/send");
```

**判错只有一层**：应答为 `ChannelsResponse` 信封（`errcode == 0` 即 `IsSuccess`）；
本域已摘录的官方码：`40097` / `47001` / `48001` / `606005`（waybill_id 不合法）/
`606031`（product_id 或 sku_id 不正确）/ `606041`（修改物流次数已达上限）/
`606046`（须用电子面单运单号）/ `606058`（原运单不存在）/ `606059`（电子面单信息不匹配）/
`6060479`（单号与物流公司正则不匹配）/ `10020115`（用户申请修改收货地址，待商家处理）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 权限集归属口径 | 接口 remarks 记「商家视角 131 / 供货商视角 192」，但**逐支端点**的权限集未在本文逐条核对 | 按支核验官方「代调用权限集」段后补进本表，勿在跨域封装里假设同一族同权限 |
| 本域频率限制 | 23 页官方频率段**未在本仓摘录** ⇒ 无本地计数依据 | 需要限流时逐页核验，勿按经验值建闸 |
| 错误码常量 | 本域未建错误码常量类，码值只存在于接口 remarks | 按码分支前先核验，再决定是否建常量并同批改守卫 |
| #17 `get_supply_order` 的应答字段全集 | `ChannelsSupplyOrderDetail` 的字段是否与 #16 列表条目同构**未逐项核验** | 需要合并展示时逐字段比对官方两页 |
| `has_more` 与 `next_key` 的权威判据 | 应答同时有 `next_page` / `total` / `has_more`，官方首选判据**未留档** | 核验后在本文第 2 节固化一条口径，避免两种判法并存 |
| 官方表外字段（`tel_last4` / `user_name`） | CD5 只记「供货侧示例已出现表外字段」⇒ 是否已进正式参数表**未逐项核验** | 官方正式收录后再考虑加属性，勿提前建模 |
| 「修改物流次数」具体上限 | 官方只给错误码 `606041`，**未给出次数值** | 核验前不得假设「可以改 N 次」 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、7 天窗口、≤10/≤100/≤200、权限集、错误码 | [IChannelsDeliveryService.cs](./IChannelsDeliveryService.cs) 的 XML 注释 |
| 路由 / 计数（恰 23）/ 四前缀配比 / `/channels/ec/order/*` 只允许两支族 | [ChannelsDeliveryContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsDeliveryContractGuards.cs) CD1（`DeliveryEndpoints_ShouldMatchOfficialRoutes`） |
| 全 POST + 3 支空请求体（`get_distribute` / `set_manually_distribute` / `get_supplier_base_info`）禁 `[Body]` | 同上 CD2（`DeliveryEndpoints_ShouldAllBePost_WithBodyRules`） |
| 令牌绑定与注册组 `Delivery` | 同上 CD3 |
| DTO 计数（恰 68）+ 常量类（恰 13，不得标序列化特性） | 同上 CD4（`DeliveryDataModels_ShouldBeRegisteredInJsonContext`；常量类须从**未过滤 abstract** 的集合取，前序批次踩过坑） |
| JSON 字段名、视角共用 / 分建的全部裁决 | 同上 CD5（`DeliveryDataModels_ShouldLockOfficialFieldsAndSharedDtos`） |
| 枚举字面量与「同值不同义」两常量类 | 同上 CD6（`DeliveryEnumConstants_ShouldLockOfficialLiteralValues`） |
| 全线逐域路由表与重叠白名单 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CD1 的 `DeliveryRoutes` 与四段配比
   （9 / 1 / 10 / 3）、CH-R2 `ExpectedRoutesByDomain["Delivery"]`。
   若新端点落在 `/channels/ec/order/*` 但不是 `dropship` 或 `supplyorder` ⇒ 它属于 **Order 域**，
   不该进本域（CD1 计数会红）。
2. 空请求体名单（#4 / #5 / #10）由 CD2 持有；官方若补参数表，改接口 + CD2 名单 + 本文第 2、3 节必须同批。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Delivery")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CD4 计数（68）；
   新增静态常量类会同时改 CD4 的常量类计数（13），且**必须从不过滤 abstract 的集合取**（CD4 内注释口径）。
4. 想合并两视角 DTO / 两套 `cancel_scene` → 先推翻 CD5 / CD6 的分建裁决，且必须带官方两页逐字段比对证据。
5. 改注册面 → `ChannelsModule.Delivery`、`ChannelsServiceBuilder.AddDeliveryApi()`（L277）与本域同批。
6. **不要**给 `ds_order_id` / `supply_order_id` 加「二选一」本地校验（官方矛盾照录）；**不要**加解除关联端点；
   **不要**把 `limit ≤ 10` 搬到别的拉单端点；**不要**假设可以回滚绑定或无限次改运单号；
   **不要**在本文件或示例里写出令牌真值或真实订单号。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。

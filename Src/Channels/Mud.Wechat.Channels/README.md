# Mud.Wechat.Channels

微信小店 / 视频号（channels 生态）SDK **主包**：**29 个业务域、394 端点**（设计方案 v1 §2.2 原规划 27 域 318 端点；缺失 API 域完善方案在同线新增 Promoter 域与 Talent 域并对既有域逐页补端点）的声明式 HTTP 客户端、模块注册器与 AOT JsonContext 合并。本线覆盖微信小店全部交易管理面（商品 / 订单 / 售后 / 物流 / 资金 / 营销 / 联盟 / 会员 / 代发 / 供货）与视频号内容运营面（直播 / 留资 / 橱窗 / 罗盘 / 带货助手），按**分阶段交付**推进。

> **交付状态（权威口径以 `Tests/Mud.Wechat.Channels.Tests/ContractGuards/` 守卫为准）**
> - ✅ **全量落地：29 域 394 端点**（P1 150：Basic 8 / Funds 16 / Product 43 / Order 27 / Aftersale 27 / Logistics 29；P2 199：Resource 8 / Shop 4 / HomePage 14 / Favorite 1 / Category 11 / Brand 8 / Qic 5 / Marketing 7 / Kf 2 / PlatformKf 4 / Vip 5 / MiniStore 10 / Warehouse 11 / League 41 / Delivery 23 / Wecom 1 / Window 9 / Locallife 6 / Subsidy 3 / Live 12 / Compass 14；缺失 API 域完善方案新增 45：Promoter 41 + Talent 4）；
> - ✅ P3 回调收口：`Mud.Wechat.Channels.Callback` 承载官方强制 JSON 安全模式回调（见其 README）。
>
> 实际端点数与设计方案 v1 规划口径的差异（类目 11、橱窗+本地生活 9、券 6、会员 5、直播面 12 含 `/channels/ec/finderlive/*` 双前缀、League 由规划 11 收敛为逐页核验后的 41、带货助手面另立 Talent 域 4（佣金单 2 + 达人橱窗 2，与 Window 域橱窗面是两套官方契约）等）均已按官方文档逐页核验后以逐域守卫定版。

## 能力面（P1 主干 6 域 150 端点，其余 23 域计数与路由表由 CH-R2 / 逐域守卫锁定）

| 模块（`ChannelsModule`） | 注册方法 | 端点 | 接口 |
| --- | --- | --- | --- |
| `Basic` | `AddBasicApi()` | 8 | `IChannelsBasicService`（8 端点：域名 IP / callback IP / callback check / clear_quota / clear_quota/v2（方法级 `[Token(Disabled = true)]` 免令牌应急逃生端点，A-7 合并）/ openapi quota get·clear / rid get；`IChannelsBasicTokenFreeService` 已 `[Obsolete]` 过渡保留一版） |
| `Funds` | `AddFundsApi()` | 16 | `IChannelsFundsService`（余额 / 结算账户 / 提现 3 + 流水 4 + 订单流水 + 银行·省市·支行 5 + 资金二维码 2；`/shop/funds/*` 历史前缀照抄官方原文） |
| `Product` | `AddProductApi()` | 43 | `IChannelsProductService`（商品增改查 / 上下架 / 审核策略与配额 25 + 库存 4 + 赠品 6 + 买赠活动 3 + 限时抢购 5） |
| `Order` | `AddOrderApi()` | 27 | `IChannelsOrderService`（订单查询 / 改价 / 改地址 / 改备注 / 换款 / 盲盒 / 生鲜质检 / 敏感信息解密 / 虚拟号·真实号 / 发货协商 / 用户预约 24 + 商家私密号实名认证 3） |
| `Aftersale` | `AddAftersaleApi()` | 27 | `IChannelsAftersaleService`（售后单 15（含原因 / 拒绝原因字典）+ 纠纷单 4 + 保障单 6 + 拒绝原因） |
| `Logistics` | `AddLogisticsApi()` | 29 | `IChannelsLogisticsService`（地址 5 + 运费模板 4 + 电子面单 17（含打印报文商家 / 供货商双路径）+ 订单发货 3） |

三个模块级入口之外还有 `AddAllApis()`（只注册**已落地**域）、`AddModules(params ChannelsModule[])` 与 `Build()`（返回 `IServiceCollection`）。

## 用法

```csharp
// ① 令牌与多小店底座（小店 AppID 独立，与公众号 / 小程序不互通）
builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
// ② 业务模块（增量交付期只注册已落地域；未落地域 fail-fast）
       .AddWechatChannelsApi(b => b.AddAllApis());

var product = await productService.GetProductAsync(new ChannelsGetProductRequest
{
    ProductId = productId,
    DataType  = 3,        // 1 线上 / 2 草稿 / 3 双份
}, ct);
```

## 已踩陷阱与边界（改动前必读）

- **MUD005 已知接受**：官方契约强制令牌走 Query 参数 `access_token`，无法改用 Header；URL 遥测已由组件 `SensitiveUrlRedactor` 与 `WechatChannelsException` 构造期脱敏。
- **AddModules 对未落地域 fail-fast**：`ChannelsModule` 枚举成员先于实现全量声明，注册「有枚举值但无注册器」的域即抛 `InvalidOperationException`（增量交付期防误装配静默吞）。当前 29 个成员均已配注册器（`ChannelsServiceBuilderTests` T1 锁定），该分支只在未来新增枚举值而漏注册器时触发。
- **双前缀并存照抄官方原文**：`/channels/ec/funds/*` 与 `/shop/funds/*` 同为官方在线前缀，禁止「对齐规范」改写（CH-R2 照录锁定）。
- **草稿 / 线上双份数据语义**：`product/add|update` 只影响草稿，需上架 + 审核通过才覆盖线上数据（`data_type` 区分读取面）。
- **同一路由只计 1**：与视频号老清单重叠的 5 条商品路由与 `aftersale/getaftersaleorder` 在域内只声明一次；`order/get` 等与 SHIPINHAO 本地生活重叠路由同口径。
- **资金 ≠ 支付**：本线 `Funds` 是小店结算 / 提现 / 流水（小店 `access_token`），与微信支付 APIv3（`Mud.Wechat.Pay`，商户私钥签名）、企微企业支付 / 收银台（Work 线）概念严格区分。
- **`/wxa/vip/*` 6 端点归 MiniProgram 线**：2026-10 逐页核对官方文档确认全部使用**小程序 access_token**（结论 A）——由小程序线 `IWxaVipService` 承载；CH-V1 锁定小店线不得跨线声明任何 `/wxa/` 路由。

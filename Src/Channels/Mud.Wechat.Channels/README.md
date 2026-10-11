# Mud.Wechat.Channels

微信小店 / 视频号（channels 生态）SDK **主包**：**27 个业务域、规划 318 端点**（设计方案 v1 §2.2）的声明式 HTTP 客户端、模块注册器与 AOT JsonContext 合并。本线覆盖微信小店全部交易管理面（商品 / 订单 / 售后 / 物流 / 资金 / 营销 / 联盟 / 会员 / 代发 / 供货）与视频号内容运营面（直播 / 留资 / 橱窗 / 罗盘），按**分阶段交付**推进。

> **交付状态（权威口径以 `Tests/Mud.Wechat.Channels.Tests/ContractGuards/` 守卫为准）**
> - ✅ **P1 已落地 6 域 149 端点**：Basic(8) / Funds(16) / Product(43) / Order(27) / Aftersale(27) / Logistics(28)；
> - ⏳ P2 待落地 21 域：Resource / Shop / HomePage / Favorite / Category / Marketing / Kf / Qic / Warehouse / League / Brand / Delivery / Wecom / MiniStore / Compass / Vip / Live / Window / Locallife / Subsidy / PlatformKf（`ChannelsModule` 枚举已全量声明，`Add{域}Api()` 随各域接口落地同批加入）；
> - ⏳ P3 回调收口：`Mud.Wechat.Channels.Callback` 包为脚手架占位（见其 README）。

## 能力面（已落地 6 域 149 端点，计数与路由表由 CH-R2 / 逐域守卫锁定）

| 模块（`ChannelsModule`） | 注册方法 | 端点 | 接口 |
| --- | --- | --- | --- |
| `Basic` | `AddBasicApi()` | 8 | `IChannelsBasicService`（7 端点：域名 IP / callback IP / callback check / clear_quota / openapi quota get·clear / rid get）+ `IChannelsBasicTokenFreeService`（`clear_quota/v2` 免令牌应急逃生端点） |
| `Funds` | `AddFundsApi()` | 16 | `IChannelsFundsService`（余额 / 结算账户 / 提现 3 + 流水 4 + 订单流水 + 银行·省市·支行 5 + 资金二维码 2；`/shop/funds/*` 历史前缀照抄官方原文） |
| `Product` | `AddProductApi()` | 43 | `IChannelsProductService`（商品增改查 / 上下架 / 审核策略与配额 25 + 库存 4 + 赠品 6 + 买赠活动 3 + 限时抢购 5） |
| `Order` | `AddOrderApi()` | 27 | `IChannelsOrderService`（订单查询 / 改价 / 改地址 / 改备注 / 换款 / 盲盒 / 生鲜质检 / 敏感信息解密 / 虚拟号·真实号 / 发货协商 / 用户预约 24 + 商家私密号实名认证 3） |
| `Aftersale` | `AddAftersaleApi()` | 27 | `IChannelsAftersaleService`（售后单 15（含原因 / 拒绝原因字典）+ 纠纷单 4 + 保障单 6 + 拒绝原因） |
| `Logistics` | `AddLogisticsApi()` | 28 | `IChannelsLogisticsService`（地址 5 + 运费模板 4 + 电子面单 16 + 订单发货 3） |

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
- **AddModules 对未落地域 fail-fast**：27 个枚举成员先于实现全量声明，注册未落地域（如 `ChannelsModule.Vip`）即抛 `InvalidOperationException`（增量交付期防误装配静默吞）。
- **双前缀并存照抄官方原文**：`/channels/ec/funds/*` 与 `/shop/funds/*` 同为官方在线前缀，禁止「对齐规范」改写（CH-R2 照录锁定）。
- **草稿 / 线上双份数据语义**：`product/add|update` 只影响草稿，需上架 + 审核通过才覆盖线上数据（`data_type` 区分读取面）。
- **同一路由只计 1**：与视频号老清单重叠的 5 条商品路由与 `aftersale/getaftersaleorder` 在域内只声明一次；`order/get` 等与 SHIPINHAO 本地生活重叠路由同口径。
- **资金 ≠ 支付**：本线 `Funds` 是小店结算 / 提现 / 流水（小店 `access_token`），与微信支付 APIv3（`Mud.Wechat.Pay`，商户私钥签名）、企微企业支付 / 收银台（Work 线）概念严格区分。
- **`/wxa/vip/*` 6 端点未落位**：令牌归属待官方逐页核对（CH-V1 锁定「未确认归属不得声明」）。

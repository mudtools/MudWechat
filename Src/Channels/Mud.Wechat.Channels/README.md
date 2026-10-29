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

---

## 原 `Mud.Wechat.Channels.Abstractions` 包面（2026-10 包合并并入本工程，命名空间不变）


微信小店 / 视频号线**令牌与多应用基座包**：独立令牌类型 `Wechat.Channels.AccessToken`、双通道签发（`/cgi-bin/token` + `/cgi-bin/stable_token`）、多小店管理器、配置面与统一异常。复用 Core 叶层（`Mud.Wechat.Abstractions`）的令牌存储端口、errcode 恢复链与 SSRF 白名单，**零新增存储端口**。

## 公开面

| 面 | 入口 | 说明 |
| --- | --- | --- |
| 令牌类型 | `ChannelsTokenTypes.AccessToken` | 字面量 `"Wechat.Channels.AccessToken"`，与 `Wechat.AccessToken` / `Wechat.Mp.AccessToken` 在共享注册表天然隔离（CH-T1 锁定，**不得**复用 `MpTokenTypes`） |
| 签发通道 | `IChannelsAuthentication` | `GET /cgi-bin/token` + `POST /cgi-bin/stable_token` 双通道；默认**稳定通道**（`ChannelsAppConfig.UseStableToken` 切换） |
| 多小店 | `AddChannelsApp` / `IChannelsAppManager` / `IChannelsAppContextSwitcher` | 复用 Core 多应用基座按 AppKey 分槽；小店无「授权企业」scope 概念，**不做** `UseCorpScope` |
| 配置 | `ChannelsAppConfig`（节 `ChannelsApps`） | `AppId` + `AppSecret` 必填（`Validate()` 校验）；`UseStableToken` / `BaseUrl` / `AllowCustomBaseUrl` / `TimeoutSeconds` / `TokenRefreshThreshold` / `IsDefault` 均有真实消费点（audit-config-keys 登记） |
| 令牌恢复 | `ChannelsTokenInvalidationDetector` | 子判定器身份（`TryAddEnumerable`）接入 Core 判定器追加集；失效码集 `{40001, 40014, 42001}`；`WechatTokenRecovery` 配置节随 `AddChannelsApp(IConfiguration)` 绑定（对齐公众号线） |
| 异常 | `WechatChannelsException` | 继承企微 `WechatWorkException` 同一基类语义，`ThrowIfFailed` + 构造期剥离 query / userinfo |

## 用法

```csharp
// 配置文件形态（ChannelsApps:0..N 数组）
builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
       .AddWechatChannelsApi(b => b.AddAllApis());

// 代码形态（多小店一次性注册；重复 AddChannelsApp 注册期 fail-fast）
builder.Services.AddChannelsApp(configs);
```

## 已踩陷阱与边界（改动前必读）

- **令牌注入走 Query**（官方契约，MUD005 已知接受）：新增任何 Query 凭据参数必须「补齐脱敏词表 / 登记豁免」二选一。
- **配置绑定形态**：`section.Bind(o)` 动作注册（源生成配置绑定器拦截），禁用 `Configure<T>(IConfiguration)` 反射重载；配置 DTO 禁 `required`（CS9035）。
- **DI 桥接不变量**：`IChannelsAppContextSwitcher` / `IAppContextHolder` / `IAppContextSwitcher` 必须同实例且先于 `AddMudHttpClient` 注册（顺序颠倒 ⇒ 声明式 `[Token]` 客户端上下文恒 null）。
- **AppKey 形状**经 Core `WechatAppKeyValidator` 约束（禁 `:`，防跨应用令牌串号）。
- **errcode 判定面**：签发面 40001 抛业务异常、业务面进恢复链——由 `ShouldInspect` 白名单（`WechatApiHosts` + 显式登记的自定义主机）预过滤，自定义 `BaseUrl` 必须登记 `WechatCustomBaseUrlRegistry`，否则私有化部署静默失去令牌恢复能力。
- **不新建令牌管理器**：普通 / 稳定两通道即两个管理器实现，按 `UseStableToken` 注册期装配；不要为「多店」再拆管理器（多店靠 AppKey 分槽）。


---

## 原 `Mud.Wechat.Channels.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）


微信小店 / 视频号线**官方 DTO 包**：`[HttpJsonSerializable]` 标注的请求 / 响应 / 嵌套对象模型 + 生成物 `Generated/*JsonContext.g.cs`（AOT JSON 上下文）。零业务逻辑、零 HTTP 语义——接口声明在主包，序列化语义在生成上下文。

## 公开面

| 目录 | 域 | 状态 |
| --- | --- | --- |
| `Basic/` | 基础接口 | ✅ P1 |
| `Funds/` | 资金结算 | ✅ P1 |
| `Product/` | 商品管理（商品 / 库存 / 赠品 / 买赠 / 限时抢购） | ✅ P1 |
| `Order/` | 订单管理 | ✅ P1 |
| `Aftersale/` | 售后管理（售后单 / 纠纷单 / 保障单） | ✅ P1 |
| `Logistics/` | 物流发货（地址 / 运费模板 / 电子面单 / 发货） | ✅ P1 |
| `ChannelsResponse.cs` | 基线响应（`errcode`/`errmsg`，实现 `IWechatApiResponse`） | ✅ |
| `Generated/` | 7 个域 JsonContext（Common + 6 业务域，`internal`，与本线类型同程序集；`InternalsVisibleTo` 只开放给测试工程） | 随域增长 |

P2 各域 DTO 随接口落地同批进入本包（命名空间 `Mud.Wechat.Channels.DataModels.{域}`）。

- **文件布局**：一类型一文件（文件名 == 类型名）；域目录下的**功能族子目录仅作组织、不入命名空间**（命名空间恒为 `...DataModels.{域}`，`SerializerClassName` 仍取域段）。回调域按**事件族**建子目录（一份载荷族一目录，含 2 个类型也对称落位），其余域按**端点族**归组；不属于任何既有分组的类型留域根，不强行归族。

## 已踩陷阱与边界（改动前必读）

- **生成物勿手改**：`Generated/*JsonContext.g.cs` 由 `scripts/GenerateJsonContext.ps1` 生成并提交版本控制；新增 / 修改 DTO 后必须重跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`，并同批更新逐域守卫的登记计数断言。
- **官方参数名照抄**：含官方历史拼写（如售后 `USER_CANCELD`、保障 `STATUS_WAIT_OP_COMFIRM`），**勿「顺手纠正」**——逐域守卫以字面量锁定，改动即变红。
- **`SerializerClassName` = 命名空间域段**（如 `Aftersale`）；常量类（`static class` + `public const`）**不得**标 `[HttpJsonSerializable]`（无序列化语义，逐域守卫锁定）。
- **共用 DTO 裁决**：字段集一致的端点共用请求 / 响应模型（如 `ChannelsAftersaleReshipRequest` 三个发货端点共用），严禁同字段重复建模；两个独立官方端点参数表可独立演化时按端点分建（如纠纷留言 / 举证两请求）并在守卫记录裁决理由。
- **官方反直觉点**：多数查询接口官方即 POST；`product/auditstrategy/get`、`getauditquota`、`aftersale/reason/get`、`rejectreason/get` 等为「POST + 空请求体」形态，不得人为造请求体 DTO。


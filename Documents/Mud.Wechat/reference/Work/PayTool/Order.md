# 收银台 · 收款工具（服务商 provider 令牌，**强制签名**）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `PayTool` 域 · 收款工具族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [PayTool/IWechatWorkPayToolOrderService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolOrderService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [PayTool/IWechatWorkPayToolOrderService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolOrderService_ThirdParty.cs)（唯一子接口，承载 4 支端点） |
| DTO 源文件 | [DataModels/PayTool/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/PayTool/)（全域 27 型、`SerializerClassName = PayTool` → `PayToolJsonContext`；本族 16 型：4 请求 + 4 响应 + 8 嵌套） |
| 契约守卫 | [WechatPayToolContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)（PT1：全域 9 路由 · PT2：三族层级 · PT3：令牌绑定 · **PT4：本族 4 支请求体必须含签名三要素** · PT5：27 型全量登记 · PT6：字段名逐条锁定 · **PT8：签名算法逐字对齐官方示例**）＋ G5 ＋ TO1 |
| 官方文档 | 创建收款订单 [98045](https://developer.work.weixin.qq.com/document/path/98045)（代开发树同文 [99358](https://developer.work.weixin.qq.com/document/path/99358)）· 取消收款订单 [98046](https://developer.work.weixin.qq.com/document/path/98046)（[99359](https://developer.work.weixin.qq.com/document/path/99359)）· 获取收款订单列表 [98053](https://developer.work.weixin.qq.com/document/path/98053)（[99360](https://developer.work.weixin.qq.com/document/path/99360)）· 获取收款订单详情 [98054](https://developer.work.weixin.qq.com/document/path/98054)（[99361](https://developer.work.weixin.qq.com/document/path/99361)）· 签名算法 [98768](https://developer.work.weixin.qq.com/document/path/98768)（代开发树同文 [99362](https://developer.work.weixin.qq.com/document/path/99362)） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PT1/PT2/PT3/PT4/PT5/PT6/PT8 断言 + 2 份接口文件 XML 注释 + 16 型 DTO 逐字段比对 + `WechatPayToolSignature` 公开面；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（`SignNested` 摊平签名 → 建单 → 关单 → 列表翻页 → 详情；密钥调用时读取、不落日志））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PT1~PT6/PT8 断言 + 本族 2 份接口文件 XML 注释 + 16 型 DTO 注释 + [WechatPayToolSignature.cs](../../../../../Src/Work/Mud.Wechat.Work/WechatPayToolSignature.cs) + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md) · 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**服务商在收银台开单、关单、查单**——本族的 4 支端点覆盖收款订单的全生命周期（创建 → 查询 → 取消）：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `CreateOrderAsync` | `POST /cgi-bin/paytool/open_order` | 创建收款订单，返回 `order_id` 与 `order_url` |
| E2 | `CloseOrderAsync` | `POST /cgi-bin/paytool/close_order` | 取消指定收款订单 |
| E3 | `GetOrderListAsync` | `POST /cgi-bin/paytool/get_order_list` | 按时间窗 + 业务类型拉取订单列表（游标分页） |
| E4 | `GetOrderDetailAsync` | `POST /cgi-bin/paytool/get_order_detail` | 查单笔订单详情（含付款渠道与到账信息） |

**族形态是「零端点父接口 + 唯一第三方子接口」**：官方在第三方应用开发与服务商代开发两棵文档树提供本族端点、共享同一端点页，企业自建应用文档树**无对应 API** ⇒ 父接口 `IWechatWorkPayToolOrderService` 零端点、不设自建子接口；4 支全部声明在 `IWechatWorkThirdPartyPayToolOrderService` 上（PT2 断言）。

**本族是本域唯一需要签名的族**：4 支端点的请求体都带 `nonce_str` / `ts` / `sig`，算法见官方 98768，SDK 侧实现为 [WechatPayToolSignature](../../../../../Src/Work/Mud.Wechat.Work/WechatPayToolSignature.cs)。同域 [Invoice.md](./Invoice.md)（发票管理）与 [VersionSuite.md](./VersionSuite.md)（应用版本付费）**官方参数表不含**签名三要素，PT4 双向锁定——**不要把签名逻辑套到另外两族上**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（服务商 / 第三方应用；代开发共用同一端点页与同一令牌） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | **不声明 `TokenManagerKey`**（PT3 反向断言不得叠加 `@Internal` / `@Corp`，TO1 同口径） | — |
| 签名密钥 | **不在 SDK 配置面**：`WechatPayToolSignature` 只做纯计算，密钥由宿主自行持有并显式传入 | 每次调用 |

**签名密钥的获取路径**（接口注释原文）：工作台 → 企业微信服务商助手 → 工具 → 收银台 → **收银台 API 调用密钥**。它属敏感凭据：只在调用瞬间从配置中心 / 密钥管理读入，**不得**写进仓库、日志、遥测或异常消息，也**不要**为了「方便」把它塞进 `WechatAppConfig`（SDK 刻意不提供该配置属性，避免成为无消费点的配置面）。

### 2.2 权限与业务前置

- **商户号注册**：E1 / E2 要求「服务商须已在收银台完成商户号注册」；支付方式为**免支付**（`pay_type = 2`）的订单可不受此限制。E3 / E4 页面标注「无特殊权限」。
- **时间窗与时钟**（E1 / E2 / E3 / E4 全部）：`ts` 为 unix 时间戳（中国时区、精确到秒），**业务系统机器时间与腾讯时间相差不能超过 15 分钟**。
- **`nonce_str` 防重**：长度 ≤ **32 字节**，且**保证 15 分钟内不重复**（同一次业务重试必须换新 nonce）。
- **创建订单的业务分支**（E1）：`business_type` 三选一 —— `1` 普通第三方应用填 `product_list.third_app`、`2` 代开发应用填 `product_list.customized_app`、`3` 行业解决方案填 `product_list.promotion_case`；`buy_info_list` 每支可填 **1~20** 个。
- **客户确认**（E1）：`notify_custom_corp` 开启后，服务商代支付与免支付订单会向企业管理员推送确认提醒，**超时 72 小时未确认会自动确认**；客户支付订单创建后推送支付提醒；指定的免支付订单创建后默认推送订单通知、**不可取消**。
- **银行回单**（E1）：`bank_receipt_media_id` 须先经「服务商上传临时素材」上传（**须指定 `attachment_type = 3`**，专用于收银台），支持 jpg/png/jpeg/bmp 与 pdf。
- **创建人**（E1）：`creator` 须有收银台收款权限；设置后「企业取消应用订单」「应用订单确认失败提醒」推送给该成员，**未设置则默认推送给服务商所有超管**。
- **个人数据**：E3 / E4 返回的 `creator`、`custom_corpid`、`custom_corp_name` 及 `bank_receipt_media_id` 均属业务敏感信息；示例一律假值，日志只落汇总。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `paytool/open_order` | ❌ | ✅ | ✅（同一端点页） |
| E2 `paytool/close_order` | ❌ | ✅ | ✅（同一端点页） |
| E3 `paytool/get_order_list` | ❌ | ✅ | ✅（同一端点页） |
| E4 `paytool/get_order_detail` | ❌ | ✅ | ✅（同一端点页） |

> 代开发不是另设子接口：**同一批路由、同一 `provider_access_token`**。PT2 的继承链断言要求本族只能有 `IWechatWorkThirdPartyPayToolOrderService` 一个子接口。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册收银台模块（三族共用一个模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.PayTool` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayToolApi()`（→ 源生成 `AddPayToolWebApiHttpClient()`） |
| 注册组名 | `"PayTool"`（三族同组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkThirdPartyPayToolOrderService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkPayToolOrderService`） |
| 不可注入 | `IWechatWorkPayToolOrderService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **签名由调用方组装，SDK 不代签**：`WechatPayToolSignature` 只提供 `BuildSignString`（构造 `stringA`）/ `Flatten`（递归摊平嵌套节点）/ `Sign`（扁平入参 → `sig`）/ `SignNested`（自动摊平）。**签的是「序列化后的报文键值」**，所以凡进了请求体的字段都应参与签名（`sig` 自身与空值除外）。

### E1. `CreateOrderAsync` — 创建收款订单

**签名**

```csharp
[Post("/cgi-bin/paytool/open_order")]
Task<CreatePayToolOrderResponse> CreateOrderAsync(
    [Body] CreatePayToolOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/open_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/open_order?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方 / 代开发（自建不开放） |
| 请求体 | `CreatePayToolOrderRequest` |
| 签名 | **必须**：`nonce_str` / `ts` / `sig` |

**请求体** `CreatePayToolOrderRequest`（顶层）：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `business_type` | `BusinessType` | `int` | 是 | `1` 普通第三方应用 / `2` 代开发应用 / `3` 行业解决方案 |
| `custom_corpid` | `CustomCorpid` | `string?` | 条件 | **`business_type = 2` 时必填**；1 / 3 可不填。≤ 64 字节 |
| `pay_type` | `PayType` | `int` | 是 | `0` 客户支付 / `1` 服务商代支付 / `2` 免支付 |
| `bank_receipt_media_id` | `BankReceiptMediaId` | `string?` | 条件 | `pay_type = 1` 时需上传企业已付款凭证；须以 `attachment_type = 3` 上传取得 |
| `creator` | `Creator` | `string?` | 否 | 订单创建人 userid；须有收银台收款权限；未设置则通知服务商所有超管 |
| `product_list` | `ProductList` | `PayToolProductList` | 是 | 购买商品，按 `business_type` 三选一分支 |
| `nonce_str` | `NonceStr` | `string` | 是 | ≤ 32 字节，**15 分钟内不重复** |
| `ts` | `Ts` | `long` | 是 | unix 时间戳（秒），与腾讯时间偏差 **≤ 15 分钟** |
| `sig` | `Sig` | `string` | 是 | HMAC-SHA256 + Base64 签名值 |

**嵌套：`product_list` 三分支**（`PayToolProductList`，官方按 `business_type` 三选一，其余保持 `null`；PT6 逐属性锁定，**不做超集归一**）：

| 分支 | 属性 | 类型 | 内层字段 |
|---|---|---|---|
| `third_app` | `ThirdApp` | `PayToolThirdAppProduct?` | `order_type`（0 新购 / 1 扩容 / 2 续期）、`buy_info_list`（`List<PayToolBuyInfo>`，1~20）、`notify_custom_corp`（0/1，不填默认 1） |
| `customized_app` | `CustomizedApp` | `PayToolCustomizedAppProduct?` | 同上三字段（`notify_custom_corp` 在免支付订单上**不可取消**） |
| `promotion_case` | `PromotionCase` | `PayToolPromotionCaseProduct?` | `order_type`、`case_id`（≤64 字节）、`promotion_edition_name`（≤128 字节）、`duration_days`（1~1825，新购/续期必填）、`take_effect_date`（`YYYYMMDD`，仅当天之后、不超一年）、`buy_info_list`（可空）、`notify_custom_corp` |

**嵌套：`buy_info_list` 元素 `PayToolBuyInfo`**（官方按业务类型给不同字段子集，SDK 用一份**可空超集**承载）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `suiteid` | `SuiteId` | `string` | 套件 ID（≤64 字节，官方必填；注意全小写无下划线） |
| `appid` | `AppId` | `int?` | 应用 ID，**仅旧套件应用需要填** |
| `edition_id` | `EditionId` | `string?` | 版本号 ID（普通第三方应用必填，≤64 字节） |
| `user_count` | `UserCount` | `int?` | 购买人数 1~1000000；新购/扩容且非固定总价版本时必填；扩容表示**增加**人数 |
| `duration_days` | `DurationDays` | `int?` | 购买时长（天，1~1825）；新购/续期必填 |
| `take_effect_date` | `TakeEffectDate` | `string?` | 生效日期 `YYYYMMDD`；最迟不超一年；有剩余时长的「新购」不支持设置 |
| `total_price` | `TotalPrice` | `long?` | **仅代开发应用请求**；单位分，需 > 0 且 ≤ 500 万（PT6 断言 `long?`） |
| `discount_info` | `DiscountInfo` | `PayToolDiscountInfo?` | **仅普通第三方应用请求** |
| `origin_price` | `OriginPrice` | `long?` | **仅响应**，原价（分） |
| `paid_price` | `PaidPrice` | `long?` | **仅响应**，实付（分），免支付返回 0 |

**嵌套：`discount_info`（`PayToolDiscountInfo`）**：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `discount_type` | `DiscountType` | `int` | `1` 固定优惠 / `2` 价格折扣 |
| `discount_amount` | `DiscountAmount` | `long?` | 单位分，1~10000000；固定优惠必填 |
| `discount_ratio` | `DiscountRatio` | `int?` | 单位 %（75 = 7.5 折），10~99；价格折扣必填 |
| `discount_remarks` | `DiscountRemarks` | `string` | 官方必填（PT6 断言为**非可空** `string`），≤256 字节，**客户侧可见** |

官方折扣上限口径：非推荐第三方应用固定优惠不超原价 **90%**、推荐应用不超 **50%**；非推荐折扣最低 **1 折**、推荐最低 **5 折**（更低需在服务商管理端操作）。「非新购 + 固定优惠」必须指定 `custom_corpid`。

**返回** `CreatePayToolOrderResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_id` | `OrderId` | `string?` | 收款订单号 |
| `order_url` | `OrderUrl` | `string?` | 客户支付订单为**支付链接**；服务商代支付与免支付订单为**订单确认链接** |
| `origin_price` | `OriginPrice` | `long?` | 原价（分）；**仅部分可确定价格的请求下返回** |
| `paid_price` | `PaidPrice` | `long?` | 折后价（分）；同上；**代开发应用两者一致** |

**调用示例**（代开发应用 + 服务商代支付；签名按报文键值递归摊平）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddPayToolApi + WechatPayToolSignature
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.PayTool;      // 本族请求 / 响应 DTO

// ① 注册第三方应用（provider_access_token 由基座注入）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册收银台模块（三族同组）
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());

// ③ 注入收款工具族子接口（父接口零端点、不可注入）
var order = provider.GetRequiredService<IWechatWorkThirdPartyPayToolOrderService>();

// ④ 签名密钥只在调用瞬间读入，不进仓库 / 日志 / 异常消息
string payToolSecret = config["Wechat:PayToolSecret"]!;

var create = new CreatePayToolOrderRequest
{
    BusinessType = 2,                        // 代开发应用
    CustomCorpid = "ww_fake_corpid_0001",    // business_type = 2 时必填（假值）
    PayType = 1,                             // 服务商代支付
    Creator = "sp_creator_fake0001",         // 须有收银台收款权限（假 userid）
    // nonce_str ≤ 32 字节且 15 分钟内不重复；同一次业务重试必须换新
    NonceStr = Guid.NewGuid().ToString("N"),
    Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    ProductList = new PayToolProductList
    {
        CustomizedApp = new PayToolCustomizedAppProduct
        {
            OrderType = 0,                   // 新购
            BuyInfoList =
            {
                new PayToolBuyInfo
                {
                    SuiteId = "suite_fake_0001",
                    TotalPrice = 10000,      // 仅代开发请求；单位分
                    DurationDays = 365,
                },
            },
        },
    },
};

// ⑤ 按「报文键值」构造签名入参：嵌套节点不直接参与，递归用子节点（官方 98768 规则⑤）
static List<KeyValuePair<string, object?>> BuildSignSource(CreatePayToolOrderRequest request)
{
    var pairs = new List<KeyValuePair<string, object?>>
    {
        new("business_type", request.BusinessType),
        new("custom_corpid", request.CustomCorpid),
        new("pay_type", request.PayType),
        new("creator", request.Creator),
        new("nonce_str", request.NonceStr),
        new("ts", request.Ts),
    };

    if (request.ProductList.CustomizedApp is { } app)
    {
        pairs.Add(new("product_list", new Dictionary<string, object?>
        {
            ["customized_app"] = new Dictionary<string, object?>
            {
                ["order_type"] = app.OrderType,
                ["buy_info_list"] = app.BuyInfoList
                    .Select(item => (object?)new Dictionary<string, object?>
                    {
                        ["suiteid"] = item.SuiteId,
                        ["total_price"] = item.TotalPrice,
                        ["user_count"] = item.UserCount,
                        ["duration_days"] = item.DurationDays,
                        ["take_effect_date"] = item.TakeEffectDate,
                    })
                    .ToList(),
            },
        }));
    }

    return pairs;
}

create.Sig = WechatPayToolSignature.SignNested(payToolSecret, BuildSignSource(create));

// ⑥ 下单：order_url 对代支付订单是「确认链接」，不是支付链接
var created = await order.CreateOrderAsync(create, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/paytool/open_order");
logger.LogInformation("收款订单已创建：{OrderId}", created.OrderId);
```

### E2. `CloseOrderAsync` — 取消收款订单

**签名**

```csharp
[Post("/cgi-bin/paytool/close_order")]
Task<WechatWorkResponse> CloseOrderAsync(
    [Body] ClosePayToolOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/close_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/close_order?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入） |
| 请求体 | `ClosePayToolOrderRequest` |
| 返回 | `WechatWorkResponse`（官方无业务负载） |
| 签名 | **必须** |

**请求体** `ClosePayToolOrderRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `order_id` | `OrderId` | `string` | 是 | 收款订单号，≤ 64 字节 |
| `nonce_str` | `NonceStr` | `string` | 是 | ≤ 32 字节，15 分钟内不重复 |
| `ts` | `Ts` | `long` | 是 | unix 时间戳（秒），偏差 ≤ 15 分钟 |
| `sig` | `Sig` | `string` | 是 | 签名值 |

**调用示例**（扁平入参直接 `Sign`；`nonce_str` 每次重试都换）

```csharp
var close = new ClosePayToolOrderRequest
{
    OrderId = created.OrderId!,              // 来自 E1
    NonceStr = Guid.NewGuid().ToString("N"),
    Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
};

// 四字段全为标量：可直接用扁平重载逐字对齐报文
close.Sig = WechatPayToolSignature.Sign(payToolSecret, new List<KeyValuePair<string, string?>>
{
    new("order_id", close.OrderId),
    new("nonce_str", close.NonceStr),
    new("ts", close.Ts.ToString(System.Globalization.CultureInfo.InvariantCulture)),
});

var closed = await order.CloseOrderAsync(close, cancellationToken);
WechatWorkException.ThrowIfFailed(closed, "/cgi-bin/paytool/close_order");
```

### E3. `GetOrderListAsync` — 获取收款订单列表

**签名**

```csharp
[Post("/cgi-bin/paytool/get_order_list")]
Task<GetPayToolOrderListResponse> GetOrderListAsync(
    [Body] GetPayToolOrderListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/get_order_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/get_order_list?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入） |
| 请求体 | `GetPayToolOrderListRequest` |
| 签名 | **必须**（注意：**查询类端点也签名**，与「查询一般不签名」的直觉相反） |

**请求体** `GetPayToolOrderListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `business_type` | `BusinessType` | `int?` | 否 | `1` / `2` / `3`；不填 = 不限业务类型 |
| `start_time` | `StartTime` | `long?` | 否 | 查询起始时间（unix 时间戳） |
| `end_time` | `EndTime` | `long?` | 否 | 查询结束时间（unix 时间戳） |
| `cursor` | `Cursor` | `string?` | 否 | 游标，取上次响应；**首轮不填** |
| `limit` | `Limit` | `int` | **是** | 分页条数，**1 ~ 2000**（PT6 断言为非可空 `int`） |
| `nonce_str` / `ts` / `sig` | 同上 | `string` / `long` / `string` | 是 | 签名三要素 |

**返回** `GetPayToolOrderListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `has_more` | `HasMore` | `int?` | 是否还有更多（`0` / `1`） |
| `pay_order_list` | `PayOrderList` | `List<PayToolOrderSummary>?` | 订单列表 |

`PayToolOrderSummary` 字段：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_id` | `OrderId` | `string?` | 订单号 |
| `create_time` | `CreateTime` | `long?` | 创建时间（unix 时间戳） |
| `custom_corpid` | `CustomCorpid` | `string?` | 客户企业 corpid |
| `buy_content` | `BuyContent` | `string?` | 购买内容 |
| `origin_price` | `OriginPrice` | `long?` | 原价（分） |
| `paid_price` | `PaidPrice` | `long?` | 实付（分），免支付返回 0 |
| `order_status` | `OrderStatus` | `int?` | `1` 待支付 / `2` 已支付 / `3` 订单取消 / `4` 支付过期 / `5` 退款申请中 / `6` 已退款 / `7` 交易完成 / `8` 待企业确认 / `9` 已部分退款 |
| `order_from` | `OrderFrom` | `int?` | `1` 客户下单 / `2` 服务商创建 |
| `creator` | `Creator` | `string?` | 订单创建人 |
| `pay_type` | `PayType` | `int?` | `0` 客户支付 / `1` 服务商代支付 / `2` 免支付 |

**调用示例**

```csharp
var query = new GetPayToolOrderListRequest
{
    BusinessType = 2,                                   // 只看代开发应用订单
    StartTime = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds(),
    EndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    Limit = 100,                                        // 官方必填，1 ~ 2000
    NonceStr = Guid.NewGuid().ToString("N"),
    Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
};

query.Sig = WechatPayToolSignature.Sign(payToolSecret, new List<KeyValuePair<string, string?>>
{
    new("business_type", query.BusinessType?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
    new("start_time", query.StartTime?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
    new("end_time", query.EndTime?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
    new("limit", query.Limit.ToString(System.Globalization.CultureInfo.InvariantCulture)),
    new("nonce_str", query.NonceStr),
    new("ts", query.Ts.ToString(System.Globalization.CultureInfo.InvariantCulture)),
});

var orders = await order.GetOrderListAsync(query, cancellationToken);
WechatWorkException.ThrowIfFailed(orders, "/cgi-bin/paytool/get_order_list");

// 翻页：has_more == 1 时用 next_cursor 续拉；每次请求都要重新签名（nonce / ts 换新）
logger.LogInformation("近 30 天订单 {Count} 条，还有更多：{HasMore}",
    orders.PayOrderList?.Count ?? 0, orders.HasMore == 1);
```

> 分页续拉的完整写法（换新 `nonce_str` / `ts` 后重签）见 §5「签名与分页的组合陷阱」；示例为避免重复只演示首页。

### E4. `GetOrderDetailAsync` — 获取收款订单详情

**签名**

```csharp
[Post("/cgi-bin/paytool/get_order_detail")]
Task<GetPayToolOrderDetailResponse> GetOrderDetailAsync(
    [Body] GetPayToolOrderDetailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/get_order_detail` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/get_order_detail?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入） |
| 请求体 | `GetPayToolOrderDetailRequest`（`order_id` + 签名三要素） |
| 签名 | **必须** |

**返回** `GetPayToolOrderDetailResponse` → `pay_order`（`PayToolOrderDetail`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_id` | `OrderId` | `string?` | 订单号 |
| `create_time` | `CreateTime` | `long?` | 创建时间（unix 时间戳） |
| `custom_corpid` | `CustomCorpid` | `string?` | 客户企业 corpid |
| `buy_content` | `BuyContent` | `string?` | 购买内容 |
| `origin_price` / `paid_price` | `OriginPrice` / `PaidPrice` | `long?` | 原价 / 实付（分）；免支付实付返回 0 |
| `order_status` | `OrderStatus` | `int?` | 同 E3 的 `1`~`9` |
| `order_from` | `OrderFrom` | `int?` | `1` 客户下单 / `2` 服务商创建 |
| `creator` | `Creator` | `string?` | 订单创建人 |
| `pay_type` | `PayType` | `int?` | `0` / `1` / `2`（**不是 `pay_from`**，见下） |
| `custom_corp_name` | `CustomCorpName` | `string?` | 客户企业简称 |
| `pay_channel` | `PayChannel` | `int?` | `1` 微信支付 / `2` 网银支付；**未支付时为空** |
| `channel_order_id` | `ChannelOrderId` | `string?` | 付款流水号；未支付时为空 |
| `paid_time` | `PaidTime` | `long?` | 付款时间；未支付时为空 |
| `business_type` | `BusinessType` | `int?` | `1` / `2` / `3` |
| `income_type` | `IncomeType` | `int?` | `1` 微信支付商户号 / `2` 财付通商户号；**收入未到账时为空** |
| `income_time` / `income_amount` | `IncomeTime` / `IncomeAmount` | `long?` | 到账时间 / 到账金额（分）；未到账时为空 |
| `product_list` | `ProductList` | `PayToolProductList?` | 购买明细，形态同 E1 的三分支（此方向带 `origin_price` / `paid_price`） |

**调用示例**

```csharp
var detailReq = new GetPayToolOrderDetailRequest
{
    OrderId = created.OrderId!,
    NonceStr = Guid.NewGuid().ToString("N"),
    Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
};

detailReq.Sig = WechatPayToolSignature.Sign(payToolSecret, new List<KeyValuePair<string, string?>>
{
    new("order_id", detailReq.OrderId),
    new("nonce_str", detailReq.NonceStr),
    new("ts", detailReq.Ts.ToString(System.Globalization.CultureInfo.InvariantCulture)),
});

var detail = await order.GetOrderDetailAsync(detailReq, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/paytool/get_order_detail");

// 未支付时 pay_channel / channel_order_id / paid_time 全为空；收入未到账时 income_* 全为空
logger.LogInformation("订单 {OrderId} 状态={Status}，已支付：{Paid}",
    detail.PayOrder?.OrderId, detail.PayOrder?.OrderStatus, detail.PayOrder?.PaidTime is not null);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本族必须签名** | 4 支请求体都要 `nonce_str` / `ts` / `sig`（含两支查询） | 漏签 → 官方拒绝；把签名叫「写操作才要」→ E3 / E4 全挂 |
| **另两族不签名** | [Invoice.md](./Invoice.md) / [VersionSuite.md](./VersionSuite.md) 请求体无签名字段 | 统一加签名中间件 → PT4 打红 + 未知字段 |
| **`sig` 不参与签名** | `BuildSignString` 剔除 `sig`（及空值） | 把自己的 `sig` 算进去 → 签名永不匹配 |
| **嵌套节点不直接参与** | 元组（对象 / 对象数组）递归摊平，用**子节点**的键值签名 | 把 `product_list` 整个 JSON 串当值 → 签名不符 |
| **同名键不去重、稳定排序** | 官方示例 2 存在同名键；`BuildSignString` 用稳定排序 | 按 key 去重 → 键数变少，签名不符 |
| **`nonce_str` ≤ 32 字节且 15 分钟不重复** | 重试必须换新 | 幂等重试复用 nonce → 被判重放 |
| **`ts` 偏差 ≤ 15 分钟** | 中国时区、秒级 | 服务器时钟漂移 → 全部请求签名过期 |
| **签名与分页的组合陷阱** | 每页请求都是**新请求**，`nonce_str` / `ts` / `cursor` 变 ⇒ **必须重算 `sig`** | 复用首页的 `sig` 翻页 → 签名不符（`cursor` 已变） |
| **`limit` 是必填、1 ~ 2000** | E3 与 [Invoice.md](./Invoice.md) 的 `limit`（可选、≤100）不同 | 跨族照抄可选或 100 → 语义漂移 / 少拉 |
| **`has_more` 是 `0/1` 整数** | 不是布尔 | 当 `bool` → 恒 `false` |
| **`pay_type` 而非 `pay_from`** | 官方返回参数表同时列出两者，但示例只用 `pay_type`；PT6 断言 `PayToolOrderDetail` **不得**有 `PayFrom` | 「补齐」`pay_from` → 守卫打红 |
| **`order_url` 语义按 `pay_type` 分叉** | 客户支付 = 支付链接；代支付 / 免支付 = 确认链接 | 一律当支付页 → 代支付场景引导错 |
| **价格字段可能不返回** | `origin_price` / `paid_price` 仅在可确定价格时返回；代开发两者一致 | 用 `null` 当 0 计价 → 金额错误 |
| **`product_list` 三分支互斥** | 按 `business_type` 只填一个分支 | 同时填多个 → 官方拒绝 |
| **`business_type = 2` 必须带 `custom_corpid`** | 1 / 3 可不填 | 代开发漏填 → 下单失败 |
| **`bank_receipt_media_id` 需 `attachment_type = 3`** | 经服务商上传临时素材取得 | 用普通上传的 media_id → 收银台不认 |
| **`notify_custom_corp` 的 72 小时** | 超时未确认**自动确认** | 把它当「需人工确认才会生效」→ 超时后订单状态突变 |
| **官方示例 JSON 语法无效** | 代开发下单示例首个 `total_price` 后**保留了逗号** | 直接复制示例报文 → JSON 解析失败，须删除该逗号 |
| **`creator` 未设 = 通知全部超管** | 官方口径 | 漏设 → 取消 / 失败提醒发给全体超管 |
| **`discount_remarks` 客户可见** | 且 ≤ 256 字节 | 写内部原因 → 客户侧泄露 |
| **父接口零端点** | 4 支都在子接口 | 去父接口找 → 找不到；搬进父接口 → PT2 打红 |
| 金额单位是分 | `origin_price` / `paid_price` / `total_price` / `discount_amount` | 当元 → 差 100 倍 |

SDK 侧通用约束：**不代签**（须调用 `WechatPayToolSignature` 并自行注入 `nonce_str` / `ts` / `sig`）、**不自动判错**、**不自动翻页（也不自动重签）**、**不校验 15 分钟时钟窗与 nonce 唯一性**、**不换算金额单位**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复；但**重试请求必须重签**，`ts` / `nonce_str` 都要换） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

签名错误、时间戳偏差超 15 分钟、`nonce_str` 复用、商户号未注册、订单状态不允许取消等业务码**接口注释未逐条留档** ⇒ 见 §7。E1 / E3 / E4 为**单层普通信封**；E2 直接返回 `WechatWorkResponse`。

> 令牌自愈与签名重算的关系：基座对令牌失效码会**自动刷新令牌并重放请求**，但请求体（含 `sig`）由调用方构造——若宿主在外层又包了一层「失败重试」，**必须重新构造请求体**（新 `nonce_str` / 新 `ts` / 重算 `sig`），否则重放的是过期签名。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 98045~98054（代开发 99358~99361）+ 全局错误码页 |
| 频率限制 | 四处 remarks 均为「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」 | 官方对应页与频率附录 |
| 订单状态机迁移规则 | 注释只列 `order_status` 取值，未给出「哪些状态可取消 / 可退款」的迁移矩阵 | 官方 98045 / 98054 |
| `take_effect_date` 与 `duration_days` 的边界 | 只留档「最迟不超一年」「有剩余时长的『新购』不支持设置生效日期」 | 官方 98045 |
| 「部分可以确定价格」的判定口径 | 注释只说「仅部分可以确定价格的请求下会返回」，未给条件 | 官方 98045 |
| 收银台 API 调用密钥的轮换流程 | 注释只给获取路径，未说明轮换与多密钥并存 | 服务商管理后台 / 官方 98768 |
| 官方页正文 | 本批未重新渲染取文 | 官方 98045 / 98768 |
| 真账号运行验证 | 本批仅编译级验证；本族需服务商账号 + 已完成商户号注册 + 收银台签名密钥 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md)（9 支全表、令牌矩阵、DTO 27 型清单、逐端点核验 2026-10-18）
- 同域近邻：[Invoice.md](./Invoice.md)（发票管理，**无需签名**）· [VersionSuite.md](./VersionSuite.md)（应用版本付费，`suite_access_token`）
- 签名实现：[WechatPayToolSignature.cs](../../../../../Src/Work/Mud.Wechat.Work/WechatPayToolSignature.cs)（`BuildSignString` / `Flatten` / `Sign` / `SignNested`，PT8 以官方示例 1 逐字节锁定）
- 素材前置：`bank_receipt_media_id` 走服务商上传临时素材（`attachment_type = 3`），见 [../Media/ServiceMedia.md](../Media/ServiceMedia.md)
- 官方页：[创建收款订单（98045）](https://developer.work.weixin.qq.com/document/path/98045) · [签名算法（98768）](https://developer.work.weixin.qq.com/document/path/98768)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)

# IndustryCoupon 域 · 行业券（出行优惠券快捷切卡组件）

微信支付 APIv3「行业券」面：**1 支声明式端点**（出行券快捷切卡组件 Token）。接口
[IWechatPayIndustryCouponService.cs](./IWechatPayIndustryCouponService.cs)，注册组 `IndustryCoupon`，
模块枚举 `PayModule.IndustryCoupon`（Pay 线**没有** `AddIndustryCouponApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.IndustryCoupon))`，内部调用源生成的
`AddIndustryCouponWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)
与 [../../Extensions/PayModule.cs](../../Extensions/PayModule.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayIndustryCouponContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayIndustryCouponContractGuards.cs)、
> 本域 DTO（`../../../Mud.Wechat.Pay.DataModels/IndustryCoupon/`）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateTransitCouponTokenAsync` | POST | `/v3/industry-coupon/tokens` | 商户证书 / APIv3 请求签名 | 返回 `IndustryCouponTransitCouponTokenResponse`（`token` / `expires_time` 均选填） |

本支官方有【普通商户】与【普通服务商】两页，两页字段表经逐页指纹比对**完全同形** ⇒ 一面一方法 + 双页文档引用，不拆面。

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成：

- 本接口**不声明 `[Token]`**，守卫 `PAY-B1` 对整条 Pay 线 fail-closed（全包零 `[Token]`），MIC-B1 在本域再锁一次。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为 `https://api.mch.weixin.qq.com`。
- `[AllowAnyStatusCode]` 是必需项：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，错误体原样落到 `WechatPayResponse.Code` / `Message`。

## 本域不可推断的契约

1. **官方字段路径带点 ⇒ 建子类型**：`coupon_list.stock_id` / `coupon_list.coupon_id` 官方是嵌套数组项，
   C# 用子类型 `IndustryCouponTransitCouponTokenRequestCouponListItem` 承载、`JsonPropertyName` 取**叶名**
   `stock_id` / `coupon_id`，外层 `CouponList` 取 `coupon_list`。
2. **券列表三条校验（照页，非 SDK 拦截）**：多组券 ID 不得重复、多组券只能同一批次、且必须是用户**最近 5 分钟内领取**的券 ⇒
   违反由应答 `code` 表达（如「券ID存在重复」「用户近五分钟没有领取该批次的券」），SDK 不做本地预判。
3. **应答两键均选填**：`token`（`Token`，`string?`，切卡组件预下单 token）与 `expires_time`（`ExpiresTime`，`string?`，
   RFC3339 文本）官方都标选填 ⇒ 判成功看 `Code` 为空，而非某键非空。
4. **单批次约束**：官方码「单次请求只能传一个批次号」⇒ 一次只切一个批次的券。
5. **频率限制官方未印**：页内未给端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数；限频由 `429 FREQUENCY_LIMIT_EXCEED` 一类码表达。
6. **本域 HTTP 面仅此一支**：其余能力在小程序 / 公众号侧，不在此 HTTP 契约面 ⇒ 不得据第三方 SDK 在本域扩路由。

## 计数与在册由谁持有

本域方法数 / 路由 / DTO（3 个）由 [WechatPayIndustryCouponContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayIndustryCouponContractGuards.cs) 的 MIC-B1~B4 锁定；
「其余能力不在 HTTP 面」由跨域零声明名册 [WechatPayNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)（MIC-N1，带正对照）背书。跨线端点总数由 `PAY-B5`（`Baseline.Pay.Endpoints`）聚合，接口在册由 `Baseline.Pay.HttpApiInterfaces` 持有。

## 快速开始

```csharp
// 1) 凭据基座（mchid / serial / 私钥与 APIv3 key 的**参数名**，真实值只走环境变量或本机 User Secrets）
builder.Services.AddPayApp(builder.Configuration, "WechatPay");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.IndustryCoupon));

var industryCoupon = scope.ServiceProvider.GetRequiredService<IWechatPayIndustryCouponService>();

var request = new IndustryCouponTransitCouponTokenRequest
{
    OpenId = "ooccupy000000000000000000000",   // 占位假值：用户在商户 AppID 下的标识，不得进日志
    CouponList = new List<IndustryCouponTransitCouponTokenRequestCouponListItem>
    {
        new IndustryCouponTransitCouponTokenRequestCouponListItem
        {
            StockId = "stock-occupy-0001",      // 批次号
            CouponId = "coupon-occupy-0001",    // 券 ID（须为最近 5 分钟内领取）
        },
    },
};

var resp = await industryCoupon.CreateTransitCouponTokenAsync(request, ct);
WechatPayException.ThrowIfFailed(resp, "/v3/industry-coupon/tokens");
var token = resp.Token;            // 选填：随后用于拉起快捷切卡小程序
var expiresTime = resp.ExpiresTime; // 选填：RFC3339 字符串
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级 QPS | 官方页**未印** ⇒ 未逐项核验 | 记为「未印」，不得凭外部 SDK 补数 |
| `open_id` 与 AppID 绑定校验细节 | 注释仅标「用户在商户 AppID 下的唯一标识」 | 需要按码分支时先核验官方 |
| 切卡组件后续小程序侧流程 | 属小程序 / 公众号面，不在此 HTTP 契约 | 不在本域建模 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文（校验规则、错误码、支持商户双页） | `IWechatPayIndustryCouponService.cs` 与方法 XML 注释 |
| 本域 1 方法 / 路由 / 请求体与返回类型 | `WechatPayIndustryCouponContractGuards.cs`（MIC-B1、MIC-B2） |
| 请求 / 应答字段名册（`open_id`/`coupon_list`、`token`/`expires_time`） | `WechatPayIndustryCouponContractGuards.cs`（MIC-B3）；DTO `../../../Mud.Wechat.Pay.DataModels/IndustryCoupon/IndustryCouponDtos.cs` |
| DTO 数 3 与 AOT 登记 | `WechatPayIndustryCouponContractGuards.cs`（MIC-B4） |
| 「其余能力不在 HTTP 面」 | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs`（MIC-N1） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 MIC-B1~B4 的计数 / 路由名册；跨线总数变化同步改 `Baseline.Pay.Endpoints` 与 `Baseline.Pay.HttpApiInterfaces`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `IndustryCoupon`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 MIC-B4 的 3 计数；上下文登记见 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs)，
   落盘见 `../../../Mud.Wechat.Pay.DataModels/Generated/IndustryCouponJsonContext.g.cs`（`internal`，示例不得引用）。
3. 改模块枚举 → 需同批有 `PayModule.IndustryCoupon` 成员 + `AddIndustryCouponWebApiHttpClient()` + 解析器上下文登记（MIC 段接线）。
4. **不要**给本域端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把小程序 / 公众号侧能力补成 HTTP 路由（破坏 MIC-N1 零声明名册）；
   **不要**把 `open_id` 写进日志或异常消息。

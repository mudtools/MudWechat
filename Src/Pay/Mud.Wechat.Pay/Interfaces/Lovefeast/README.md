# Lovefeast 域 · 爱心餐（品牌捐赠 / 供餐信息查询）

微信支付 APIv3「爱心餐」商户面：**1 支声明式端点**（查询爱心餐品牌信息）。接口
[IWechatPayLovefeastService.cs](./IWechatPayLovefeastService.cs)，注册组 `Lovefeast`，模块枚举
`PayModule.Lovefeast`（Pay 线**没有** `AddLovefeastApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.Lovefeast))`，内部调用源生成的
`AddLovefeastWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)
与 [../../Extensions/PayModule.cs](../../Extensions/PayModule.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayLovefeastContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayLovefeastContractGuards.cs)、
> 本域 DTO（`../../../Mud.Wechat.Pay/DataModels/Lovefeast/`）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryLovefeastBrandAsync` | GET | `/v3/lovefeast/brands/{brandId}` | 商户证书 / APIv3 请求签名 | 入参是 `long brandId`（品牌 ID，非商户号）；返回 `LovefeastBrandInfoResponse` |

本域 1 支官方只印商户页（「支持商户：【普通商户】」），无服务商镜像、无双页归并 ⇒ 一支一页一方法。

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文：

- 本接口**不声明 `[Token]`**，守卫 `PAY-B1` 对整条 Pay 线 fail-closed（全包零 `[Token]`），LVF-B1 在本域再锁一次。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为 `https://api.mch.weixin.qq.com`。
- `[AllowAnyStatusCode]` 是必需项：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，不加则非 2xx 直接抛 `ApiException`、把官方业务码丢掉。

## 本域不可推断的契约

1. **入参是品牌 ID 而非商户号**：官方前置条件原文「商家进驻微信支付品牌商家后获得的品牌ID」⇒
   `[Path] long brandId` 由**品牌商家进驻流程**产出，商户号本身不构成合法入参，`brandId` 是 `long` 不是 `string`。
2. **应答两键语义不对称**：`donate_count`（`DonateCount`，`long?`）必填（用户在该品牌下捐赠的总次数），
   `consume_count`（`ConsumeCount`，`long?`）**仅供餐品牌返回**（该品牌为环卫工人供餐总量）⇒
   普通品牌该键为 `null`，**不得**据其缺失判定调用失败。
3. **无查询参数、不涉密**：官方请求头只列 `Authorization` / `Accept`，请求头表**未列** `Wechatpay-Serial` ⇒ 本域无「密文上送」契约面。
4. **「爱心餐查券」不属本域**：官方《爱心餐查券接口使用指引》把查券能力指向
   `GET /v3/marketing/favor/users/{openid}/coupons`（既存 [../MarketingFavor/IWechatPayMarketingFavorService.cs](../MarketingFavor/IWechatPayMarketingFavorService.cs)，守卫 MF 系列）⇒
   **不得**在本域重复建一支查券方法，也不得为该指引另造路由。
5. **频率限制官方未印**：页内无「接口频率」条目、错误码表也未给 429 ⇒ 不得凭第三方 SDK 补数，唯一业务错误码是 `500 SYSTEM_ERROR`。

## 计数与在册由谁持有

本域方法数 / 路由 / DTO 由 [WechatPayLovefeastContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayLovefeastContractGuards.cs) 的 LVF-B1~B6 逐条锁定；
跨线端点总数由 [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) 的 `PAY-B5`（`Baseline.Pay.Endpoints`）聚合，接口在册由其接口名册断言（`Baseline.Pay.HttpApiInterfaces`）持有。本域路由**未**逐条进 `PAY-B5` 名册（仅计入总数）。

## 快速开始

```csharp
// 1) 凭据基座（mchid / serial / 私钥与 APIv3 key 的**参数名**，真实值只走环境变量或本机 User Secrets）
builder.Services.AddPayApp(builder.Configuration, "WechatPay");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Lovefeast));

var lovefeast = scope.ServiceProvider.GetRequiredService<IWechatPayLovefeastService>();

// brand_id 由品牌商家进驻流程产出（占位假值）
var brand = await lovefeast.QueryLovefeastBrandAsync(8000000001L, ct);
WechatPayException.ThrowIfFailed(brand, "/v3/lovefeast/brands/{brand_id}");

long donateCount = brand.DonateCount ?? 0L;   // 必填：捐赠总次数
long? consumeCount = brand.ConsumeCount;       // 选填：仅供餐品牌有值，普通品牌为 null
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级 QPS | 官方页**未印**、错误码表无 429 ⇒ 未逐项核验 | 记为「未印」，不得凭外部 SDK 补数 |
| 品牌 ID 的申请 / 进驻流程 | 本域只消费品牌 ID，进驻细节属产品面 | 不在本域建模 |
| 爱心餐查券能力 | 归 MarketingFavor 域（MF 守卫） | 查券面查 MarketingFavor，勿在本域加断言 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文（品牌 ID 前置、两键语义、错误码） | `IWechatPayLovefeastService.cs` 与方法 XML 注释 |
| 本域 1 方法 / 1 路由 / 动词 GET | `WechatPayLovefeastContractGuards.cs`（LVF-B1、LVF-B2） |
| 应答 DTO 字段（`donate_count` / `consume_count`） | `../../../Mud.Wechat.Pay/DataModels/Lovefeast/LovefeastBrandDtos.cs` |
| 查券归属 | `../MarketingFavor/IWechatPayMarketingFavorService.cs`（MF 守卫） |
| 跨线端点 / 接口计数 | `../../../../../Tests/ContractBaseline.cs`（`Baseline.Pay.Endpoints` / `HttpApiInterfaces`） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 LVF-B1~B6 的计数 / 路由名册，再改本文速览表；
   跨线总数变化同步改 `Baseline.Pay.Endpoints` 与 `Baseline.Pay.HttpApiInterfaces`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Lovefeast`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 LVF-B5 的 DTO 计数；上下文登记见 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs)，
   落盘见 `../../../Mud.Wechat.Pay/DataModels/Generated/LovefeastJsonContext.g.cs`（`internal`，示例不得引用）。
3. 改模块枚举 → 需同批有 `PayModule.Lovefeast` 成员 + `AddLovefeastWebApiHttpClient()` 源生成注册 + 解析器上下文登记（LVF-B6 三段式锁定）。
4. **不要**给本域端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把查券建进本域（归 MarketingFavor）；
   **不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）。

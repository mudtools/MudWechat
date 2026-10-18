# ServiceMarket 域 · 第三方服务市场（服务商面）

微信开放平台第三方平台「第三方服务市场」域：**3 支契约端点，恒 POST**（服务用户有效期列表 /
服务用户有效期 / 用户已支付订单列表）。接口
[IWechatOpenPlatformServiceMarketService.cs](./IWechatOpenPlatformServiceMarketService.cs)，注册组 `ServiceMarket`，
模块枚举 `OpenPlatformModule.ServiceMarket`（本线无 `Add{域}Api()` 扩展方法，装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.ServiceMarket))`，生成器按注册组产出
`AddServiceMarketWebApiHttpClient()`）。逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文全部断言只来自本域接口 XML 注释、守卫
> [OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs)（SM 系列）与 DTO
> [`DataModels/ServiceMarket/`](../../../Mud.Wechat.OpenPlatform.DataModels/ServiceMarket/)。

## 端点速览

令牌面恒为 **`component_access_token`**（官方服务市场文档明写「对于普通业务，access_token 为第三方平台
component_access_token」），官方 Query 参数名仍为 `access_token`，注入方式一律 **Query**
（`TokenInjectionMode.Query`，由 `[Token]` 特性声明）。三页均要求服务端调用。

| # | 方法 | HTTP 动词 | 路由 | 请求体 | 应答 |
|---|---|---|---|---|---|
| 1 | `GetServiceBuyerListAsync` | POST | `/wxa/servicemarket/service/get_service_buyer_list` | `service_id` + 分页 + `buyer_type` | `buyer_list` |
| 2 | `GetServiceBuyerAsync` | POST | `/wxa/servicemarket/service/get_service_buyer` | `service_id` + `appid`/`openid`（按 `buyer_type` 二选一） | `buyer` |
| 3 | `GetPaidOrderListAsync` | POST | `/wxa/servicemarket/get_paid_order_list` | `service_id` + 过滤条件 + 分页 | `order_list` |

分页 `limit` 默认 **10**（官方参数表给定）；`service_id` 为 number，本仓以 `long` 承载。

## 判非契约：`login_auth` 不建接口、不声明路由

任务清单中的第 4 行 `/wxa/servicemarket/service/login_auth` 经逐页核验判为**非契约**：

- 官方「登录验证」页正文明写「**本接口不支持第三方平台调用**」；
- 其调用凭证是**服务商自有 `access_token`**，不属于本线 `component_access_token` / `authorizer_access_token`
  两键系——强行声明即为「取错令牌的死路由」。

处置：不建接口、不声明路由，由守卫 `SM2` 锁定全仓 `Src/` 零声明，恒不计入端点总数；
追踪号 **SM-LOGINAUTH-NONCONTRACT-01**。若官方后续开放第三方平台面，须先同批更新基线、守卫与本裁决留档。

## 与小程序线自建服务市场区分

小程序线持有 `/wxa/servicemarket` 与 `/wxa/servicemarketretrieve` 的小程序自调面（小程序令牌）；
本域三条官方路由是**第三方服务商面**（component 令牌），令牌归属不同。两条线交叉声明即令牌串号：
小程序线守卫 `NC2` 锁定 `Src/MiniProgram` 对本域三条路由零声明，本域 `SM1` 为本线镜像；
三条路由进入 `OpenPlatformContractGuards` 的 OP-B5 全仓独占清单。

## 字段名与敏感面

守卫 DTO2 / DTO3 锁定：列表键 `buyer_list` / `order_list`、用户对象键 `buyer`、规格键 `spec_list`、
`specification_id`、`effective_time` / `expire_time`、`phone` 均照官方原文；number 字段以整型承载。

**敏感面**：`phone` 属个人联系方式、`openid` / `appid` 属用户与小程序标识——**均不得落日志、遥测或异常消息**；
承载 DTO 不重写 `ToString`（与 CBT6/CBT7 同口径）。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c => { /* 四个凭据 */ })
    .AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.ServiceMarket));

OpenPlatformServiceMarketBuyerListResponse buyers = await serviceMarket.GetServiceBuyerListAsync(
    new OpenPlatformServiceMarketBuyerListRequest
    {
        ServiceId = 123456,
        Offset = 0,
        Limit = 20,
        BuyerType = 1,
    }, ct);

OpenPlatformServiceMarketPaidOrderListResponse orders = await serviceMarket.GetPaidOrderListAsync(
    new OpenPlatformServiceMarketPaidOrderListRequest { ServiceId = 123456, Limit = 20 }, ct);
// orders.OrderList —— 其中 Phone 不得落日志
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 逐端点频率 / 权限集数字 | 官方三页未给出端点级数字（接口注释照官方留空） | 重新逐页核验后再补 remarks |
| `buyer_type` 的取值枚举 | 官方参数表未在接口注释展开完整取值 | 以 `int` 透传，不臆造常量 |
| 分页上限 | 官方仅给 `limit` 默认 10，未给上限 | 不设本地闸，按官方返回值处理 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 业务限制 | [./IWechatOpenPlatformServiceMarketService.cs](./IWechatOpenPlatformServiceMarketService.cs) 的 XML 注释 |
| 路由表、计数（恰 3）与令牌语义 | 守卫 `SM1` |
| `login_auth` 非契约裁决 | 守卫 `SM2` + 类 remarks + `Tests/ContractBaseline.cs` 追踪号 |
| 三处一致（枚举 + 委托 + resolver） | 守卫 `REG1`；DI 见 [../../Extensions/OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 登记、字段名、数值面 | 守卫 DTO1~DTO3；[`DataModels/ServiceMarket/`](../../../Mud.Wechat.OpenPlatform.DataModels/ServiceMarket/) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `SM1` 的 `ServiceMarketEndpoints` 与计数（3）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = "ServiceMarket"`）→ 跑脚本 → 更新 DTO1 计数（9）。
3. **不要**把 `login_auth` 加回契约面；如官方开放，须附新官方页证据并同批撤销 `SM2` 裁决。
4. 最后跑 `verify-build.ps1` 与 `audit-config-keys.ps1`，全绿才算完成。

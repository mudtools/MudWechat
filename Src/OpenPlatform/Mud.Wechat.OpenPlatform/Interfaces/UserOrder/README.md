# UserOrder 域 · 小程序购物订单

微信开放平台第三方平台「小程序购物订单」域：**7 支端点，恒 POST**（上传购物详情 / 上传物流信息 /
上传合单购物详情 / 上传合单物流信息 / 验证上传结果 / 开通产品权限 / 提交接入审核）。
接口 [IWechatOpenPlatformUserOrderService.cs](./IWechatOpenPlatformUserOrderService.cs)，注册组 `UserOrder`，
模块枚举 `OpenPlatformModule.UserOrder`（本线无 `Add{域}Api()` 扩展方法，装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.UserOrder))`，生成器按注册组产出
`AddUserOrderWebApiHttpClient()`）。逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文全部断言只来自本域接口 XML 注释、守卫
> [OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs)（UD 系列）与 DTO
> [`DataModels/UserOrder/`](../../../Mud.Wechat.OpenPlatform.DataModels/UserOrder/)。官方页未给出的额度 / 频率 / 权限集数字，
> 一律入文末「已知留档缺口」，不从记忆或邻域补。

## 端点速览

令牌面恒为**授权方令牌** `authorizer_access_token`（第三方平台代授权方小程序上传订单数据），
官方 Query 参数名恒为 `access_token`（值才是 `authorizer_access_token`），注入方式一律 **Query**
（`TokenInjectionMode.Query`，由 `[Token]` 特性声明）。

| # | 方法 | HTTP 动词 | 路由 | 请求体 | 应答 |
|---|---|---|---|---|---|
| 1 | `UploadOrderAsync` | POST | `/user-order/orders` | 订单标识 + `order_list` + 支付者 | 仅 `errcode`/`errmsg` |
| 2 | `UploadShippingAsync` | POST | `/user-order/orders/shippings` | 订单标识 + `delivery_mode` + 物流列表 | 仅 `errcode`/`errmsg` |
| 3 | `UploadCombinedOrderAsync` | POST | `/user-order/combine-orders` | 合单标识 + 子单购物详情 + 支付者 | 仅 `errcode`/`errmsg` |
| 4 | `UploadCombinedShippingAsync` | POST | `/user-order/combine-orders/shippings` | 合单标识 + 子单物流 + 支付者 | 仅 `errcode`/`errmsg` |
| 5 | `VerifyShoppingInfoAsync` | POST | `/user-order/shoppinginfo/verify` | 订单标识 + 支付者 | `verify_result` |
| 6 | `OpenProductPermissionAsync` | POST | `/user-order/orders-permission/open` | **无请求体** | 仅 `errcode`/`errmsg` |
| 7 | `ConfirmProductPermissionAsync` | POST | `/user-order/orders-permission/confirm` | **无请求体** | `last_result` |

## 业务前置与顺序约束

- 上传类端点要求小程序已开通「购物订单」产品权限：先 `orders-permission/open` 开通，再
  `orders-permission/confirm` 提交接入审核（审核结果由应答 `last_result` 承载）。
- **订单信息一旦关联成功不可修改**；`upload_time` 为 **RFC3339 字符串**且须晚于历史请求，以保证更新顺序。
- 官方数量上限：`order_list` 最多 **50** 条、`item_list` 最多 **50** 条、`image_url` 最多 **3** 个。
- 物流上传须在支付成功后调用；`delivery_mode` 取字符串枚举 `UNIFIED_DELIVERY`（统一发货）/
  `SPLIT_DELIVERY`（分拆发货）；顺丰速运必须填写 `contact` 联系方式。
- `orders-permission/open` 与 `orders-permission/confirm` 官方请求示例**无 JSON 包体**，操作对象即当前授权方上下文。

## 字段名/类型照官方原文不「对齐」

守卫 DTO2 / DTO3 锁定：`order_number_type`（number，1 = 微信支付单号、2 = 商户单号）、
`delivery_mode`、跳转链接 `type`（字符串枚举 `URL` / `MINI_PROGRAM`）、`verify_result`、`last_result`
均照官方原文。`order_number_type` 与各 number 字段以整型承载，**不写成 string**。

## 令牌与作用域

`[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken, Name = "access_token")]`——
调用前须经 `IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` 进入目标授权方作用域，
否则运行期 fail-fast（不静默回退平台令牌）。把 `Name` 改成 `authorizer_access_token` 会让全部端点报 40001（守卫 UD2 断言）。

## 落位判据

本域七页只落在开放平台 `doc/oplatform/…/miniprogram-management/shopping-orders/` 文档树，令牌面是
`authorizer_access_token` ⇒ 归开放平台线。小程序线守卫 `NC2` 锁定 `Src/MiniProgram` 对该路由族零声明；
本域 `UD1` / `UD2` 为本线镜像。`/user-order/` 前缀进入 `OpenPlatformContractGuards` 的 OP-B5 全仓独占清单。

## 快速开始

```csharp
// Program.cs：先装配凭证链，再按模块注册业务客户端
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";
    c.ComponentAppSecret = "...";   // 不得入日志/遥测/异常消息
    c.Token = "...";
    c.EncodingAesKey = "...";
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.UserOrder));

// 业务侧：进入目标授权方作用域后调用（UserOrderService 由 DI 注入）
using (appContextSwitcher.UseAuthorizerScope(authorizerAppId))
{
    await userOrder.OpenProductPermissionAsync(ct);
    OpenPlatformUserOrderPermissionConfirmResponse confirm =
        await userOrder.ConfirmProductPermissionAsync(ct);

    await userOrder.UploadOrderAsync(new OpenPlatformUserOrderUploadRequest
    {
        // OrderKey / OrderList / Payer —— 字段见 DTO 与官方参数表
    }, ct);

    OpenPlatformUserOrderVerifyResponse verify =
        await userOrder.VerifyShoppingInfoAsync(new OpenPlatformUserOrderVerifyRequest { /* … */ }, ct);
    // verify.VerifyResult
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 逐端点权限集与频率数字 | 官方七页未在正文给出可搬运的端点级数字（接口注释照官方留空） | 重新逐页核验后再补 remarks，不凭记忆补 |
| `verify_result` / `last_result` 的取值枚举 | 官方未在接口注释展开语义 | 以整型承载，勿写「等于某常量才算通过」的判定 |
| `delivery_mode` 与跳转 `type` 的官方表格类型标注不一致 | 官方表头标 number、示例为字符串枚举 | 本仓按示例取 string（与 SKIT 同口径），守卫锁定 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 业务限制 | [./IWechatOpenPlatformUserOrderService.cs](./IWechatOpenPlatformUserOrderService.cs) 的 XML 注释 |
| 路由表与计数（恰 7，恒 POST） | 守卫 `UD1` |
| 令牌语义（授权方值 / `access_token` 参数名 / Query / 注册组） | 守卫 `UD2` |
| 三处一致（枚举 + 委托 + resolver） | 守卫 `REG1`；DI 见 [../../Extensions/OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 登记、字段名、数值面 | 守卫 DTO1~DTO3；[`DataModels/UserOrder/`](../../../Mud.Wechat.OpenPlatform.DataModels/UserOrder/) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `UD1` 的 `UserOrderEndpoints` 与计数（7）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = "UserOrder"`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 DTO1 计数（16）与上下文登记断言。
3. 改契约面（路由 / 令牌 / 注册组）→ 对应 UD 守卫 + `OpenPlatformModule.UserOrder` +
   `AddUserOrderWebApiHttpClient()`（生成物）+ 本 README。
4. **不要**把令牌参数名改成 `authorizer_access_token`、**不要**给官方无数字的端点凭记忆补频率。
5. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与
   [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

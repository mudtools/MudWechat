# Compass 域 · 罗盘（数据分析）

视频号（channels 生态）「罗盘」域：**14 支端点，全部 POST**，按凭据主体分两个子族 ——
罗盘**商家版** `/channels/ec/compass/shop/*` 10 支（权限集 id `175`，仅「微信小店」账号类型可调用）
+ 罗盘**达人版** `/channels/ec/compass/finder/*` 4 支（权限集 id `176`，仅「视频号助手」账号类型可调用）。
接口 [IChannelsCompassService.cs](./IChannelsCompassService.cs)，注册组 `Compass`
（`[HttpClientApi(RegistryGroupName = "Compass", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddCompassApi()`，模块枚举 `ChannelsModule.Compass`。逐页核验 2026-10-18。

计数与前缀配比以守卫
[ChannelsCompassContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsCompassContractGuards.cs)
（CCP1~CCP6）与 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
（CH-R1 / CH-R2）为准：CCP1 钉「恰 14 = shop 10 + finder 4」。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（Promoter / Product）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetShopFinderAuthorizationListAsync` | POST | `/channels/ec/compass/shop/finder/authorization/list/get` | `access_token`（Query） | **官方空请求体**（CCP2 禁 `[Body]`）；回 `authorized_finder_id_list` |
| 2 | `GetShopFinderListAsync` | POST | `/channels/ec/compass/shop/finder/list/get` | `access_token`（Query） | 仅返回**当日有带货数据**的达人；`version = v2` 才回带货者身份 id |
| 3 | `GetShopFinderOverallAsync` | POST | `/channels/ec/compass/shop/finder/overall/get` | `access_token`（Query） | 带货数据概览（共用 `ds` 请求） |
| 4 | `GetShopFinderProductListAsync` | POST | `/channels/ec/compass/shop/finder/product/list/get` | `access_token`（Query） | `ds` + 三个 id 必填其一 |
| 5 | `GetShopFinderProductOverallAsync` | POST | `/channels/ec/compass/shop/finder/product/overall/get` | `access_token`（Query） | 同上三选一 |
| 6 | `GetShopLiveListAsync` | POST | `/channels/ec/compass/shop/live/list/get` | `access_token`（Query） | `ds` + `finder_id` **两者必填** |
| 7 | `GetShopOverallAsync` | POST | `/channels/ec/compass/shop/overall/get` | `access_token`（Query） | 电商概览；应答字段官方类型为 string |
| 8 | `GetShopProductDataAsync` | POST | `/channels/ec/compass/shop/product/data/get` | `access_token`（Query） | **仅历史数据**；`ds` + `product_id` 必填 |
| 9 | `GetShopProductListAsync` | POST | `/channels/ec/compass/shop/product/list/get` | `access_token`（Query） | **仅历史数据**；`limit`/`offset` 不填**默认只回 10 条** |
| 10 | `GetShopSaleProfileDataAsync` | POST | `/channels/ec/compass/shop/sale/profile/data/get` | `access_token`（Query） | **仅历史数据**；`ds` + `type` 必填 |
| 11 | `GetFinderOverallAsync` | POST | `/channels/ec/compass/finder/overall/get` | `access_token`（Query） | 达人版电商概览（共用 `ds` 请求） |
| 12 | `GetFinderProductDataAsync` | POST | `/channels/ec/compass/finder/product/data/get` | `access_token`（Query） | 达人版商品数据（共用 `ds` 请求） |
| 13 | `GetFinderProductListAsync` | POST | `/channels/ec/compass/finder/product/list/get` | `access_token`（Query） | 官方矛盾：`product_list` 参数表 objarray、示例单对象 ⇒ 按参数表建列表 |
| 14 | `GetFinderSaleProfileDataAsync` | POST | `/channels/ec/compass/finder/sale/profile/data/get` | `access_token`（Query） | **仅历史数据**；`dim_value` 为 number（与商家版 string 不同） |

- **令牌口径**：本域 14 支在声明面上**统一**使用本线唯一令牌 `ChannelsTokenTypes.AccessToken`
  = `"Wechat.Channels.AccessToken"`，接口级 `[Token(... InjectionMode = Query, Name = "access_token")]`
  （守卫 CH-T1 / CH-T2）。达人版 4 支的官方凭据主体是「视频号助手」，本仓未在声明面区分令牌类型 —— 见文末缺口第 1 条。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Compass`
  （`Shop` / `Finder` 两个子目录只作文件组织）；无 `IsAbstract` 父接口（CH-X2）。
- 全域**不支持云调用**（14 页官方原文均如此），本域也没有免令牌端点。

## 1. `ds` 日期口径：YYYYMMDD，且 4 支只吃历史数据

- `ds` 是**字符串**、格式 `YYYYMMDD`（[ChannelsCompassDsRequest.cs](../../DataModels/Compass/Shop/ChannelsCompassDsRequest.cs)
  被 5 支端点共用，CCP5 锁定 `ds` 键名）；格式错 → 官方错误码 `10021069`（请求参数日期格式有误）。
- **#8 / #9 / #10 / #14 官方原文「仅历史数据，暂无当日数据」** ⇒ 传当天 `ds` 拿到的是空数据而不是报错，
  这是本域最容易踩的坑：判「有没有数据」不能只看 `errcode`。
- 本域没有日期区间参数（不像 Delivery 的 7 天窗口），一次调用一个 `ds`。

## 2. 达人身份的三选一（`finder_id` / `talent_id` / `mp_id`）

#4 / #5 用
[ChannelsCompassShopFinderProductRequest.cs](../../DataModels/Compass/Shop/ChannelsCompassShopFinderProductRequest.cs)：
三个字段都是 `string?`，官方约束「**三者必填其一**」。
本仓**不做本地三选一校验**（组合合法性由官方判定），`finder_id` 错 → `10023001`。
#6 是另一套口径：`ds` 与 `finder_id` **两者必填**（不是三选一）。
账号类型错配（店铺账号去调达人接口，反之亦然）→ 官方错误码 `886`（登录账号类型错误）。

## 3. `version = v2` 只在一个端点存在

只有 #2 有 `version` 选填（`ChannelsGetShopFinderList` 形态的 `Version`，CCP5 锁 `version` 键名），
官方语义是「填 `v2` 会返回**带货者身份的 id**」；其余 13 支没有该键 ⇒ 别把 `v2` 当全域开关。

## 4. 金额与计数单位：单位「分」，但官方类型常是 string

- GMV / 佣金类字段**单位：分**（`pay_gmv` / `create_gmv` / `pure_pay_gmv` / `actual_commission` /
  `predict_commission` / `window_pay_gmv` / `other_pay_gmv` 等，DTO remarks 逐个标注「单位：分」）。
- **但承载类型多为 `string?`**：商家版概览 `pay_order_cnt`、达人商品概览 `pay_gmv`、
  开播 `live_time` / `live_duration`、商品列表数据全部指标（`ChannelsCompassShopProductListData` 25 个指标里
  只有 `pay_refund_ratio` 是 `double?`）官方类型即 string ⇒ 数值运算前须自行解析，
  **不得**为了「方便」把 DTO 改成数字（CCP5 按端点逐个锁类型口径）。
- 人群维度值两侧类型不同：商家版 `ChannelsCompassShopSaleProfileDim.dim_value` 官方为 **string**、
  达人版 `ChannelsCompassFinderSaleProfileDim.dim_value` 官方为 **number**（矛盾照录：参数表 number、示例像字符串）
  ⇒ **不共用 DTO**（CCP5 显式分建裁决）。
- 商品 id 类型在本域内部也不一致：#8 请求 `product_id` 是 `long`，而 #9 应答条目
  `ChannelsCompassShopProductListItem.ProductId` 是 `string?` ⇒ 跨端点串联需显式转换，不改 DTO。

## 5. 分页：只有 #9 有分页，且默认 10 条

| 端点 | 形态 | 事实 |
|---|---|---|
| #9 `shop/product/list/get` | `limit` + `offset`（均可空 `int?`） | **不填默认只返回 10 条商品数据**（官方原文），不是「返回全部」 |
| 其余 13 支 | 无分页 | 一次返回完整列表 / 单对象 |

**没有 `next_key`**，靠 `offset` 递增翻页；应答总数在 `ChannelsCompassShopProductListResponse.TotalCount`（`long?`）。

## 6. 枚举取值：两套人群用户类型（CCP6）

| 常量类 | 取值 |
|---|---|
| [ChannelsCompassShopSaleProfileUserTypes.cs](../../DataModels/Compass/Shop/ChannelsCompassShopSaleProfileUserTypes.cs) | 1 商品曝光用户 / 2 商品点击用户 / 3 购买用户 / 4 首次购买用户 / 5 重复购买用户（**1..5**） |
| [ChannelsCompassFinderSaleProfileUserTypes.cs](../../DataModels/Compass/Finder/ChannelsCompassFinderSaleProfileUserTypes.cs) | 1..5 同上 **+ 6 直播观看用户**（**1..6**） |

⇒ 把商家版的 `type` 常量传给达人版接口在数值上「看起来合法」，但达人侧多出第 6 档；
两套常量**不得合并**（CCP4 同时钉「常量类恰 2 个、均不得标 `[HttpJsonSerializable]`」）。
人群应答形态是 `data.field_list[]`（`field_name` + 维度键值对列表），没有扁平字段。

## 7. 已核验官方错误码

| 码 | 语义（接口 remarks 摘录） | 出现在 |
|---|---|---|
| `10021069` | 请求参数日期格式有误 | 全域多支 |
| `10023001` | `finder_id` 有误 | #4 / #5 / #6 |
| `886` | 登录账号类型错误（店铺 / 达人接口错配） | #4 |
| `48001` | 接口权限未开启：需前往「视频号助手 - 开放能力 - 开放场景」打开开关 | 达人版 4 支（#11~#14） |

本域**未建错误码常量类**（对比 Basic 的 CB6），码值只以文档形式存在 ⇒ 按码分支时勿依赖编译期常量。

## 8. DTO 与共用裁决（CCP4 / CCP5）

DTO 总数由 CCP4 钉为**恰 42**（商家版 32：共用 `ds` 1 + 达人列表 4 + 授权列表 1 + 达人概览 2 +
达人商品列表/详情 6 + 开播 3 + 电商概览 2 + 商品详情 4 + 商品列表 4 + 人群 5；
达人版 10：电商概览 2 + 商品数据 3 + 商品列表 1 + 人群 4），另有**常量类恰 2**（两套人群用户类型）。
已裁决的共用：`ChannelsCompassDsRequest` 由 5 支共用；`ChannelsCompassSaleProfileRequest`（`ds` + `type`）
由商家版与达人版两支人群端点共用；`ChannelsCompassShopFinderProductRequest` 由 #4 / #5 共用；
达人版 `product_list` 元素与 `getfinderproductdata` 的 `product_info` 字段集一致 ⇒ 共用
`ChannelsCompassFinderProductInfo`。反例是人群维度 `dim_value`（见第 4 节）—— 同字段名不同官方类型 ⇒ 分建。

## 9. 与既有六线的路由交叠红线

- 本域 14 条全部以 `/channels/ec/compass/` 开头，属于小店线独占前缀；CH-R1 的重叠白名单只有
  「`/cgi-bin/token` + `/cgi-bin/stable_token` + [../Basic/README.md](../Basic/README.md) 的 8 条 `/cgi-bin/*`」，
  罗盘路由**不在任何白名单内**，出现在别的线即红。
- CH-V1 禁止本线任何 `/wxa/` 路由。
- CH-R2 `ExpectedRoutesByDomain["Compass"]` 逐条锁定 14 条，`shop` 与 `finder` 两个子族**不得互换前缀**
  （权限集主体不同）。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Compass;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddCompassApi());

var compass = app.Services.GetRequiredService<IChannelsCompassService>();

// ① 授权的视频号 id 列表（#1 官方空请求体：方法只有 cancellationToken）。
var authorized = await compass.GetShopFinderAuthorizationListAsync(ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(authorized, "/channels/ec/compass/shop/finder/authorization/list/get");

// ② 当日带货达人（version = v2 才回带货者身份 id；ds 必须 YYYYMMDD）。
var finders = await compass.GetShopFinderListAsync(
    new ChannelsCompassShopFinderListRequest { Ds = "20261018", Version = "v2" }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(finders, "/channels/ec/compass/shop/finder/list/get");
// 注意：本接口「仅返回当日有带货数据的达人」——空列表不等于出错。
var finderId = finders.FinderList?[0].FinderId;         // string

// ③ 商品列表（#9 仅历史数据；limit/offset 不填只回 10 条）。
var products = await compass.GetShopProductListAsync(
    new ChannelsCompassShopProductListRequest { Ds = "20261017", Limit = 50, Offset = 0 }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(products, "/channels/ec/compass/shop/product/list/get");
long total = products.TotalCount ?? 0L;
var gmvText = products.ProductList?[0].Data?.PayGmv;    // 官方类型 string，单位「分」

// ④ 单个商品详情（#8：product_id 请求侧是 number，与上面应答里的 string 型 id 不同）。
var one = await compass.GetShopProductDataAsync(
    new ChannelsCompassShopProductDataRequest { Ds = "20261017", ProductId = 100001 }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(one, "/channels/ec/compass/shop/product/data/get");

// ⑤ 人群画像（商家版 type 1..5；达人版多出 6 直播观看用户，两套常量不合并）。
var profile = await compass.GetShopSaleProfileDataAsync(
    new ChannelsCompassSaleProfileRequest
    {
        Ds = "20261017",
        Type = ChannelsCompassShopSaleProfileUserTypes.PurchaseUser,
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(profile, "/channels/ec/compass/shop/sale/profile/data/get");
var dims = profile.Data?.FieldList;                     // field_name + dim_value（官方 string）
```

**判错只有一层**：应答均为 `ChannelsResponse` 信封（`errcode == 0` 即 `IsSuccess`）；
但本域「`errcode == 0` + 数据为空」是合法结果（未到历史数据、账号类型不匹配的软失败等），
调用方须自行区分「无权限（`48001`）」与「无数据」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 达人版 4 支的凭据主体 | 接口 remarks 写「凭据为视频号助手 `access_token`、权限集 176」，但声明面按 CH-T1 仍统一为本线唯一令牌类型 ⇒ **官方是否要求另一套凭据未逐项核验** | 逐页核验达人版 4 页的「请求地址」说明；若确需独立凭据，须走 CH-T1 的裁决变更流程，不能私自加令牌类型 |
| 历史数据的可查最早日期 | 官方仅说「仅历史数据」，**T+几 出数、保留多久未在本仓留档** | 核验官方「数据分析开发指南」后补 remarks |
| 本域频率限制 | 14 页官方频率段**未在本仓摘录**；`45009` 类码也未在本域 remarks 出现 | 需要限流时逐页核验，勿按邻域经验值建闸 |
| 错误码常量 | 本域未建 `ChannelsErrorCodes` 子集（4 个码只以文档存在） | 按码分支前先核验，再决定是否建常量并同批改 CCP 系列守卫 |
| `ChannelsCompassShopProductDetailData` 部分字段官方类型列 null | CCP5 记「`pay_refund_after_send_ratio` 官方类型列 null，照录」⇒ 实际应答类型仍属**未逐项核验** | 以真机应答复核后再定型 |
| `pay_refund_cnt` 等 2025-11-24 变更字段全集 | remarks 只记「新增 `pay_refund_cnt`、删除废弃字段」，删了哪些**未逐项核验** | 需要历史数据对齐时逐页比对官方变更日志 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、权限集 175/176、错误码、「仅历史数据」原文 | [IChannelsCompassService.cs](./IChannelsCompassService.cs) 的 XML 注释 |
| 路由 / 计数（恰 14 = shop 10 + finder 4） | [ChannelsCompassContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsCompassContractGuards.cs) CCP1（`CompassEndpoints_ShouldMatchOfficialRoutes`） |
| 全 POST + `#1` 空请求体禁 `[Body]` | 同上 CCP2（`CompassEndpoints_ShouldAllBePost_WithEmptyBodyRules`） |
| 令牌绑定与注册组 `Compass` | 同上 CCP3 |
| DTO 计数（恰 42）+ 常量类（恰 2，不得标序列化特性） | 同上 CCP4（`CompassDataModels_ShouldBeRegisteredInJsonContext`） |
| JSON 字段名、`string` / `number` 类型口径、共用与分建裁决 | 同上 CCP5（`CompassDataModels_ShouldLockOfficialFieldsAndSharedDtos`） |
| 两套人群用户类型字面量（1..5 / 1..6） | 同上 CCP6（`CompassEnumConstants_ShouldLockOfficialLiteralValues`） |
| 全线逐域路由表与重叠白名单 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CCP1 的 `CompassRoutes` 与 shop/finder 配比（10 / 4）、
   CH-R2 `ExpectedRoutesByDomain["Compass"]`。子族前缀（`shop` / `finder`）绑定的是权限集主体，不得互换。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Compass")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CCP4 计数（42）；
   **静态常量类永远不标** `[HttpJsonSerializable]`（CCP4 另有恰 2 的计数）。
   序列化上下文多为 `internal`，不得在宿主示例里引用；漏登记在 AOT 首次序列化才失败（CH-X5）。
3. 改人群用户类型常量 → CCP6 断言同批改；两套常量若被合并，CCP4 的常量类计数也会红。
4. 想「把 GMV 改成数字」→ 那是改官方类型口径，须先逐页核验并同步 CCP5，**不允许**为调用方便而单改 DTO。
5. 改注册面 → `ChannelsModule.Compass`、`ChannelsServiceBuilder.AddCompassApi()`（L290）与本域同批。
6. **不要**给达人版私自引入第二套令牌类型（CH-T1）；**不要**给 #1 造空请求体 DTO；
   **不要**把「空数据」当失败；**不要**在本文件或示例里写出令牌真值。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。

# Marketing 域 · 营销管理-优惠券

微信小店 / 视频号（channels 生态）「营销管理-优惠券」域 SDK（7 端点：创建 / 详情 / ID 列表 /
用户券详情 / 用户券 ID 列表 / 更新内容 / 更新状态，路由前缀 `/channels/ec/coupon/*`）。 接口：[IChannelsMarketingService](./IChannelsMarketingService.cs)；注册组 `Marketing`；模块枚举 `ChannelsModule.Marketing`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsMarketingContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMarketingContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Marketing`](../../DataModels/Marketing)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsMarketingService = 7；按 HTTP 特性实计 **7** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsMarketingService` | `CreateCouponAsync` | POST | `/channels/ec/coupon/create` | `AccessToken`（Query），Query `access_token` | 创建优惠券（`createcoupon`，官方路由 `/channels/ec/coupon/create`）。 |
| 2 | `IChannelsMarketingService` | `GetCouponAsync` | POST | `/channels/ec/coupon/get` | `AccessToken`（Query），Query `access_token` | 获取优惠券详情（`getcoupon`，官方路由 `/channels/ec/coupon/get`）。 |
| 3 | `IChannelsMarketingService` | `GetCouponListAsync` | POST | `/channels/ec/coupon/get_list` | `AccessToken`（Query），Query `access_token` | 获取优惠券 ID 列表（`getcouponlist`，官方路由 `/channels/ec/coupon/get_list`）。 |
| 4 | `IChannelsMarketingService` | `GetUserCouponAsync` | POST | `/channels/ec/coupon/get_user_coupon` | `AccessToken`（Query），Query `access_token` | 获取用户优惠券详情（`getusercoupon`，官方路由 `/channels/ec/coupon/get_user_coupon`）。 |
| 5 | `IChannelsMarketingService` | `GetUserCouponListAsync` | POST | `/channels/ec/coupon/get_user_coupon_list` | `AccessToken`（Query），Query `access_token` | 获取用户优惠券 ID 列表（`getusercouponlist`，官方路由 `/channels/ec/coupon/get_user_coupon_list`）。 |
| 6 | `IChannelsMarketingService` | `UpdateCouponAsync` | POST | `/channels/ec/coupon/update` | `AccessToken`（Query），Query `access_token` | 更新优惠券内容（`updatecoupon`，官方路由 `/channels/ec/coupon/update`）。 |
| 7 | `IChannelsMarketingService` | `UpdateCouponStatusAsync` | POST | `/channels/ec/coupon/update_status` | `AccessToken`（Query），Query `access_token` | 更新优惠券状态（`updatecouponstatus`，官方路由 `/channels/ec/coupon/update_status`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「本地生活团购券」（`IChannelsLocallifeService`，`/channels/ec/voucher/*`）
概念严格区分**：本域是小店电商优惠券（coupon，满减/折扣/直减），团购券是到店核销凭证
（voucher，code 核销 + 分账结算），两域字段体系互不复用。

**官方第三方代调用**：本域全部端点所属权限集 id 为 `132`（服务商可用
`authorizer_access_token` 代小店商家调用）。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 7 次）第三方平台代调用所属权限集 id 为 `132`。
- （出现 7 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `CreateCouponAsync` — 创建优惠券（`createcoupon`，官方路由 `/channels/ec/coupon/create`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/create`

- **请求体**：`ChannelsCouponCreateRequest` — 创建请求（`type`/`name`/`promote_info`/`receive_info` 必填，其余选填；类型决定必填的优惠字段组）。
- **返回**：`ChannelsCouponDataResponse` — 新优惠券 ID（`data.coupon_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_createcoupon
- 官方业务限制：①创建成功后 `status=1`（编辑中），需经「更新优惠券状态」流转到生效； / ②名称最长 10 个中文字符；③商品券 `product_ids` 必填且最多 200 个； / ④折扣数范围 1000~10000（5000=5 折）且必须是 100 的整数倍、不可低于 2 折； / ⑤直减券减免金额（分）不可超过 200 元，商品券优惠后不低于原价 2 折、店铺满减券不低于门槛 2 折； / ⑥件数门槛与价格门槛不能同时设置；⑦所有时间字段均为秒级时间戳。
- 官方错误码（节选）：10021005 名称太长 / 10021014 推广类型不对 / 10021080 置券时传了非上架商品。

### 2. `GetCouponAsync` — 获取优惠券详情（`getcoupon`，官方路由 `/channels/ec/coupon/get`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/get`

- **请求体**：`ChannelsGetCouponRequest` — 查询请求（`coupon_id` 必填）。
- **返回**：`ChannelsGetCouponResponse` — 优惠券全量信息（`coupon`：类型 / 状态 / 内容 `coupon_info` / 库存 `stock_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getcoupon
- 官方业务限制：本接口无特殊注意事项；优惠券类型见 `ChannelsCouponTypes`、状态见 `ChannelsCouponStatuses`。
- 官方错误码：`10021037`（优惠券不存在）。

### 3. `GetCouponListAsync` — 获取优惠券 ID 列表（`getcouponlist`，官方路由 `/channels/ec/coupon/get_list`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/get_list`

- **请求体**：`ChannelsGetCouponListRequest` — 查询请求（`status`/`page`/`page_size`/`page_ctx` 必填；`page_ctx` 首次请求填空串）。
- **返回**：`ChannelsGetCouponListResponse` — 优惠券 ID 列表（`coupons`）、总数（`total_num`）与翻页上下文（`page_ctx`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getcouponlist
- 官方业务限制：①每次请求的页码间隔不能超过 10；②第一次请求的页码要小于 10，或者是最后一页； / ③一页最大 200 条；④状态筛选见 `ChannelsCouponStatuses`（含官方 `200=过期或作废` 复合值）。
- 官方错误码：`10021001`（优惠券状态不对）/ `10021036`（页码与上次请求相差过大）。

### 4. `GetUserCouponAsync` — 获取用户优惠券详情（`getusercoupon`，官方路由 `/channels/ec/coupon/get_user_coupon`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/get_user_coupon`

- **请求体**：`ChannelsGetUserCouponRequest` — 查询请求（`user_coupon_id` 与 `openid` 必填）。
- **返回**：`ChannelsGetUserCouponResponse` — 用户券详情（`user_coupon`：状态 / 生效失效时间 / 使用的订单 ID），外层含 `openid`/`unionid`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getusercoupon
- 官方业务限制：本接口无特殊注意事项；用户券状态见 `ChannelsUserCouponStatuses`（100/101/102，与商家券状态枚举不同轴）。 / 官方变更日志（2026-06-03）：响应参数 `user_coupon_list` 修正为 `user_coupon`。
- 官方错误码：`40003`（OpenID 不正确）/ `10021037`（优惠券不存在）。

### 5. `GetUserCouponListAsync` — 获取用户优惠券 ID 列表（`getusercouponlist`，官方路由 `/channels/ec/coupon/get_user_coupon_list`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/get_user_coupon_list`

- **请求体**：`ChannelsGetUserCouponListRequest` — 查询请求（`openid`/`page`/`page_size`/`page_ctx` 必填；`status` 选填，不填查全部状态）。
- **返回**：`ChannelsGetUserCouponListResponse` — 用户券 ID 列表（`user_coupon_list`）与翻页上下文（`page_ctx`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getusercouponlist
- 官方业务限制：①每次请求的页码间隔不能超过 10；②第一次请求的页码要小于 10，或者是最后一页； / ③一页最大 200 条；④用户券状态筛选见 `ChannelsUserCouponStatuses`。
- 官方错误码：`40003` / `10021001` / `10021036` / `10021037`。

### 6. `UpdateCouponAsync` — 更新优惠券内容（`updatecoupon`，官方路由 `/channels/ec/coupon/update`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/update`

- **请求体**：`ChannelsCouponUpdateRequest` — 更新请求（`coupon_id`/`type`/`name`/`promote_info`/`receive_info`/`valid_info` 必填；更新形态无 `auto_valid_info`，官方参数表原文如此）。
- **返回**：`ChannelsCouponDataResponse` — 优惠券 ID（`data.coupon_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_updatecoupon
- 官方业务限制：与创建同源的优惠规则（折扣数 1000~10000 且 100 的整数倍不可低于 2 折、 / 直减金额不可超过 200 元、product_ids 最多 200 个、件数/价格门槛互斥）； / 与创建的差异：更新形态 `valid_info` 必填且不支持 `auto_valid_info`。
- 官方错误码（节选）：10021005 / 10021006 / 10021007 / 10021014 / 10021037。

### 7. `UpdateCouponStatusAsync` — 更新优惠券状态（`updatecouponstatus`，官方路由 `/channels/ec/coupon/update_status`）。

- **声明接口**：[IChannelsMarketingService](./IChannelsMarketingService.cs)
- **官方契约**：`POST /channels/ec/coupon/update_status`

- **请求体**：`ChannelsCouponUpdateStatusRequest` — 状态更新请求（`coupon_id`/`status` 必填；目标状态 2=生效、4=作废、5=删除）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_updatecouponstatus
- 官方业务限制：优惠券创建后 `status=1`（编辑中），仅可流转到 2（生效）、4（作废）、5（删除）； / 目标状态是 `ChannelsCouponStatuses` 的子集。
- 官方错误码：`10021001`（优惠券状态不对）/ `10021003`（优惠券还没过期）/ `10021034`（优惠券重入失败）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Marketing`](../../DataModels/Marketing) 内共 **27** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **27** 个，`SerializerClassName` = `Marketing`。

AOT 源生成上下文：[`MarketingJsonContext.g.cs`](./../../DataModels/Generated/MarketingJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsCouponDataResponse` | 应答 | [Marketing/Coupon/ChannelsCouponDataResponse.cs](../../DataModels/Marketing/Coupon/ChannelsCouponDataResponse.cs) | `data` |
| `ChannelsCouponCreateRequest` | 请求体 | [Marketing/Coupon/ChannelsCouponCreateRequest.cs](../../DataModels/Marketing/Coupon/ChannelsCouponCreateRequest.cs) | `type`, `name`, `promote_info`, `discount_info`, `receive_info`, `valid_info`, `ext_info`, `auto_valid_info` |
| `ChannelsGetCouponResponse` | 应答 | [Marketing/Coupon/ChannelsGetCouponResponse.cs](../../DataModels/Marketing/Coupon/ChannelsGetCouponResponse.cs) | `coupon` |
| `ChannelsGetCouponRequest` | 请求体 | [Marketing/Coupon/ChannelsGetCouponRequest.cs](../../DataModels/Marketing/Coupon/ChannelsGetCouponRequest.cs) | `coupon_id` |
| `ChannelsGetCouponListResponse` | 应答 | [Marketing/Coupon/ChannelsGetCouponListResponse.cs](../../DataModels/Marketing/Coupon/ChannelsGetCouponListResponse.cs) | `coupons`, `total_num`, `page_ctx` |
| `ChannelsGetCouponListRequest` | 请求体 | [Marketing/Coupon/ChannelsGetCouponListRequest.cs](../../DataModels/Marketing/Coupon/ChannelsGetCouponListRequest.cs) | `status`, `page`, `page_size`, `page_ctx` |
| `ChannelsGetUserCouponResponse` | 应答 | [Marketing/UserCoupon/ChannelsGetUserCouponResponse.cs](../../DataModels/Marketing/UserCoupon/ChannelsGetUserCouponResponse.cs) | `openid`, `unionid`, `user_coupon` |
| `ChannelsGetUserCouponRequest` | 请求体 | [Marketing/UserCoupon/ChannelsGetUserCouponRequest.cs](../../DataModels/Marketing/UserCoupon/ChannelsGetUserCouponRequest.cs) | `user_coupon_id`, `openid` |
| `ChannelsGetUserCouponListResponse` | 应答 | [Marketing/UserCoupon/ChannelsGetUserCouponListResponse.cs](../../DataModels/Marketing/UserCoupon/ChannelsGetUserCouponListResponse.cs) | `user_coupon_list`, `page_ctx` |
| `ChannelsGetUserCouponListRequest` | 请求体 | [Marketing/UserCoupon/ChannelsGetUserCouponListRequest.cs](../../DataModels/Marketing/UserCoupon/ChannelsGetUserCouponListRequest.cs) | `openid`, `status`, `page`, `page_size`, `page_ctx` |
| `ChannelsCouponUpdateRequest` | 请求体 | [Marketing/Coupon/ChannelsCouponUpdateRequest.cs](../../DataModels/Marketing/Coupon/ChannelsCouponUpdateRequest.cs) | `coupon_id`, `type`, `name`, `promote_info`, `discount_info`, `receive_info`, `valid_info`, `ext_info` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsCouponUpdateStatusRequest` | 请求体 | [Marketing/Coupon/ChannelsCouponUpdateStatusRequest.cs](../../DataModels/Marketing/Coupon/ChannelsCouponUpdateStatusRequest.cs) | `coupon_id`, `status` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Marketing;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddMarketingApi());

var service = app.Services.GetRequiredService<IChannelsMarketingService>();
var ct = CancellationToken.None;
var response = await service.CreateCouponAsync(new ChannelsCouponCreateRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/coupon/create");
// 占位值仅用于编译示意；业务值须按 IChannelsMarketingService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateCouponAsync`）；请求体 `ChannelsCouponCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsMarketingService](./IChannelsMarketingService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsMarketingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMarketingContractGuards.cs) |
| 守卫中的代表性测试方法 | `MarketingEndpoints_ShouldMatchOfficialRoutes`、`MarketingEndpoints_ShouldAllBePost_WithBodyRules`、`MarketingInterface_ShouldBindChannelsToken_AndRegisterUnderMarketingGroup`、`MarketingDataModels_ShouldBeRegisteredInJsonContext`、`MarketingDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`MarketingEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Marketing`](../../DataModels/Marketing) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsMarketingContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

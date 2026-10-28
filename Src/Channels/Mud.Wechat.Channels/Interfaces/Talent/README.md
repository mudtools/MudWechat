# Talent 域 · 带货助手（达人）

微信小店 / 视频号（channels 生态）「带货助手（达人）」域 SDK（4 端点：佣金单 2 / 达人橱窗商品 2）。 接口：[IChannelsTalentService](./IChannelsTalentService.cs)；注册组 `Talent`；模块枚举 `ChannelsModule.Talent`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsTalentContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/Talent`](../../../Mud.Wechat.Channels/Talent/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsTalentService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsTalentService` | `GetOrderDetailAsync` | POST | `/channels/ec/talent/get_order_detail` | `AccessToken`（Query），Query `access_token` | 获取佣金单详情（`get_order_detail`，带货助手查自己单笔佣金单的订单 / 佣金 / 下单通道 / 推客机构 / 商品信息）。 |
| 2 | `IChannelsTalentService` | `GetOrderListAsync` | POST | `/channels/ec/talent/get_order_list` | `AccessToken`（Query），Query `access_token` | 获取佣金单列表（`get_order_list`，按创建时间或更新时间范围顺序翻页拉取自己的佣金单）。 |
| 3 | `IChannelsTalentService` | `GetWindowProductDetailAsync` | POST | `/channels/ec/talent/window/product/get` | `AccessToken`（Query），Query `access_token` | 获取达人橱窗商品详情（`get_product_detail`，带货助手视角的橱窗商品单查）。 |
| 4 | `IChannelsTalentService` | `GetWindowProductListAsync` | POST | `/channels/ec/talent/window/product/list/get` | `AccessToken`（Query），Query `access_token` | 获取达人橱窗商品列表（`get_product_list`，支持页码分页与 `last_buffer` 顺序翻页两种互斥方式）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**账号类型与令牌归属**：本域四端点的适用范围官方均写「**带货助手（需申请）**」，
即凭据为带货助手的 `access_token`，或第三方服务商代调用时的 `authorizer_access_token`
（权限集 id：佣金单 `190` / 橱窗 `189`）。令牌类型仍为本线的
`AccessToken` —— 与 League（联盟机构）、小店商家侧共用同一令牌类型，
「哪一种账号」由应用配置里的 AppID/凭据区分，**不按账号类型拆分令牌类型**（同 League 域先例裁决）。

**与 Window / League 两域的严格区分（三处近邻，均不合并）**：
① 本域橱窗面为 `/channels/ec/talent/window/product/*`（带货助手视角，2 端点），
与 `IChannelsWindowService` 的 `/channels/ec/window/product/*`（视频号助手视角）
是**两套独立官方契约**——请求体、响应体、错误码、权限集全部不同，不得合并；
② 本域佣金单面为 `/channels/ec/talent/get_order_detail` 与 `/get_order_list`
（达人/带货助手查自己的佣金单），与 `IChannelsLeagueService` 的
`/channels/ec/league/headsupplier/order/*`（机构查全部佣金单）不同前缀不同归属；
③ 本域订单状态与佣金单状态枚举与 League 侧同名字段**是两张表**，故常量类分建
（见 `ChannelsTalentOrderStatuses`、`ChannelsTalentCommissionOrderStates`）。

**官方文档矛盾照录（本域逐页核验后的建模口径写在每个端点的注释里）**：详情响应示例的
`actual_payment` 为 "99.99" 而参数表注明「单位：分」；列表条目 `product_id` 参数表标 number、
示例为字符串；官方请求示例存在畸形 JSON（`"order_id": 123""`）。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetOrderDetailAsync` — 获取佣金单详情（`get_order_detail`，带货助手查自己单笔佣金单的订单 / 佣金 / 下单通道 / 推客机构 / 商品信息）。

- **声明接口**：[IChannelsTalentService](./IChannelsTalentService.cs)
- **官方契约**：`POST /channels/ec/talent/get_order_detail`

- **请求体**：`ChannelsTalentGetOrderDetailRequest` — 查询请求（`order_id` 与 `sku_id` 必填，均可从获取佣金单列表接口获得；`special_id` 选填，回传订单列表返回的额外参数）。
- **返回**：`ChannelsTalentGetOrderDetailResponse` — 订单基础信息（`base_info`）、佣金信息（`commission_info`）、下单通道信息（`channel_info`）、内容推广推客机构信息（`promotion_head_supplier_info`）与商品信息（`product_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/talent/openapi/order/api_get_order_detail.html
- 官方业务限制：①**不支持云调用**；②支持第三方平台代带货助手调用，所属权限集 id 为 `190` / （服务商获得授权后用 `authorizer_access_token` 代调用）；③仅「带货助手（需申请）」账号类型可调用； / ④专属错误码 `10024000`（参数错误，请检查传递参数）/ `10024006`（不存在该佣金单）； / ⑤官方注明「本接口无特殊注意事项」。
- **官方文档矛盾照录**：①响应参数表 `base_info.actual_payment` 注明「单位：分」、 / 返回示例为 `"99.99"`（元形态）——SDK 照抄官方类型标注建模为字符串、不做单位换算； / ②返回示例的 `errcode` 是字符串 `"0"` 而参数表标注 number——按参数表与各线统一响应基座处置； / ③请求示例为畸形 JSON（`"order_id": 123""`，多一个引号）；④返回示例未出现 / `promotion_head_supplier_info` 与 `order_pay_time`/`settle_payment`（后两者为变更日志新增字段）； / ⑤「注意事项」小节编号漂移（正文为 `# 4. 注意事项` 而错误码起于 `# 7`）。
- 接口变更日志：2026-01-15 新增 `settle_payment`（外部对齐佣金单结算基数）；2025-11-17 新增 `order_pay_time`。

### 2. `GetOrderListAsync` — 获取佣金单列表（`get_order_list`，按创建时间或更新时间范围顺序翻页拉取自己的佣金单）。

- **声明接口**：[IChannelsTalentService](./IChannelsTalentService.cs)
- **官方契约**：`POST /channels/ec/talent/get_order_list`

- **请求体**：`ChannelsTalentGetOrderListRequest` — 查询请求（`page_size` 必填且**不超过 10**；`create_time_gt`/`create_time_lt` 与 `update_time_gt`/`update_time_lt` **两组二选一**；`order_id`/`spu_id` 选填过滤；`next_key` 顺序翻页上下文）。
- **返回**：`ChannelsTalentGetOrderListResponse` — 佣金单三元组列表（`order_list`，每项 `order_id`/`sku_id`/`special_id`）、是否还有剩余订单（`has_more`）与翻页上下文（`next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/talent/openapi/order/api_get_order_list.html
- 官方业务限制：①**不支持云调用**；②支持第三方平台代带货助手调用，所属权限集 id 为 `190`； / ③仅「带货助手（需申请）」账号类型可调用；④**page_size 上限 10**（本线其他列表面普遍为 30/100/500，本域为最小档）； / ⑤**创建时间范围与更新时间范围二选一**（官方对四个时间参数逐条标注）；⑥官方要求「使用前请阅读开发者指南」， / 本接口无特殊错误码（走通用错误码）。
- **官方文档矛盾照录**：响应参数表把 `order_list` 的类型列渲染为带模板标记的 / `" data-v-…>objarray`（文档渲染缺陷，非类型信息）——SDK 按「Res.order_list(Array) Object Payload」 / 小节建模为对象数组；返回示例未出现 `next_key`（仅 `has_more`）。

### 3. `GetWindowProductDetailAsync` — 获取达人橱窗商品详情（`get_product_detail`，带货助手视角的橱窗商品单查）。

- **声明接口**：[IChannelsTalentService](./IChannelsTalentService.cs)
- **官方契约**：`POST /channels/ec/talent/window/product/get`

- **请求体**：`ChannelsTalentGetWindowProductDetailRequest` — 查询请求（`product_id` 必填，橱窗商品 ID，从获取达人橱窗商品列表接口获得）。
- **返回**：`ChannelsTalentGetWindowProductDetailResponse` — 橱窗商品详情（`product`：商品 ID / 来源店铺 appid / 货源商品 ID / 标题 / 头图 / 叶子类目 / 状态 / 售价 / 库存 / 销量 / 是否隐藏 / 跟佣跳转串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/talent/openapi/window/api_get_product_detail.html
- 官方业务限制：①**不支持云调用**；②支持第三方平台代带货助手调用，所属权限集 id 为 `189`； / ③仅「带货助手（需申请）」账号类型可调用；④**商品状态枚举仅两值**：`1` 生效中、`2` 被禁止售卖 / （见 `ChannelsTalentWindowProductStatuses`，与视频号助手橱窗面不是同一张表）； / ⑤专属错误码 `10200006`（达人平台账号异常）/ `10200007`（商品未在达人平台橱窗）/ / `10200008`（商品状态异常），三者官方均要求「请登录达人平台后台确认」。
- **与 Window 域不合并（照录裁决）**：本端点 `/channels/ec/talent/window/product/get` 与 / `IChannelsWindowService` 的 `/channels/ec/window/product/get` 请求体同为 `product_id`， / 但响应体字段集、状态枚举、错误码与权限集（189 对 133/177）全部不同，属两套契约，SDK 各自建模。
- **官方文档矛盾照录**：`selling_price` 官方注明「价格区间最小值（单位分）（销售价）」、 / 示例为 `999`，与同域列表侧无对应字段；SDK 按参数表 number 建模为 `long`、不做单位换算。

### 4. `GetWindowProductListAsync` — 获取达人橱窗商品列表（`get_product_list`，支持页码分页与 `last_buffer` 顺序翻页两种互斥方式）。

- **声明接口**：[IChannelsTalentService](./IChannelsTalentService.cs)
- **官方契约**：`POST /channels/ec/talent/window/product/list/get`

- **请求体**：`ChannelsTalentGetWindowProductListRequest` — 查询请求（`page_size` 必填且**不超过 500**；`page_index` 从 1 开始，**与 `last_buffer` 互斥、不可同时填写**）。
- **返回**：`ChannelsTalentGetWindowProductListResponse` — 橱窗商品列表（`products`，每项含 `product_id` 与来源标识）与顺序翻页上下文（`last_buffer`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/talent/openapi/window/api_get_product_list.html
- 官方业务限制：①**不支持云调用**；②支持第三方平台代带货助手调用，所属权限集 id 为 `189`； / ③仅「带货助手（需申请）」账号类型可调用；④**两种翻页方式互斥**：`page_size`+`page_index` / 受 **page_size × page_index ≤ 10000** 约束，`last_buffer` 顺序翻页无数量限制， / 官方明确「当数据量较大时建议优先使用 last_buffer 顺序翻页」；⑤**商品来源二分**： / `product_source`=1 来源店铺自营商品（此时返回 `appid`）、=2 来源选品中心带货商品 / （此时返回 `out_product_id`），见 `ChannelsTalentWindowProductSources`； / ⑥专属错误码 `10200003`（page_size 超过上限 500）/ `10200004`（page_index 与 last_buffer 同时存在）/ / `10200005`（page_size × page_index 超过 10000，建议改用 last_buffer）。
- **官方文档矛盾照录**：列表条目 `product_id` 的「Res.products(Array) Object Payload」参数表标注 / number，而返回示例为带引号字符串（`"1000000000000"`）——SDK **按参数表建模为 number** 并在 DTO 注释照录该分歧； / 详情侧（`/channels/ec/talent/window/product/get`）参数表与示例均为 string，故两处类型不一致属官方原文，不做强统一。 / 另响应参数表把 `products` 的类型列渲染为带模板标记的 `" data-v-…>objarray`（文档渲染缺陷）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/Talent`](../../../Mud.Wechat.Channels/Talent/) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `Talent`。

AOT 源生成上下文：[`TalentJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/TalentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsTalentGetOrderDetailResponse` | 应答 | [Talent/Order/ChannelsTalentGetOrderDetailResponse.cs](../../../Mud.Wechat.Channels/Talent/Order/ChannelsTalentGetOrderDetailResponse.cs) | `base_info`, `commission_info`, `channel_info`, `promotion_head_supplier_info`, `product_info` |
| `ChannelsTalentGetOrderDetailRequest` | 请求体 | [Talent/Order/ChannelsTalentGetOrderDetailRequest.cs](../../../Mud.Wechat.Channels/Talent/Order/ChannelsTalentGetOrderDetailRequest.cs) | `order_id`, `sku_id`, `special_id` |
| `ChannelsTalentGetOrderListResponse` | 应答 | [Talent/Order/ChannelsTalentGetOrderListResponse.cs](../../../Mud.Wechat.Channels/Talent/Order/ChannelsTalentGetOrderListResponse.cs) | `order_list`, `has_more`, `next_key` |
| `ChannelsTalentGetOrderListRequest` | 请求体 | [Talent/Order/ChannelsTalentGetOrderListRequest.cs](../../../Mud.Wechat.Channels/Talent/Order/ChannelsTalentGetOrderListRequest.cs) | `create_time_gt`, `create_time_lt`, `update_time_gt`, `update_time_lt`, `order_id`, `spu_id`, `page_size`, `next_key` |
| `ChannelsTalentGetWindowProductDetailResponse` | 应答 | [Talent/Window/ChannelsTalentGetWindowProductDetailResponse.cs](../../../Mud.Wechat.Channels/Talent/Window/ChannelsTalentGetWindowProductDetailResponse.cs) | `product` |
| `ChannelsTalentGetWindowProductDetailRequest` | 请求体 | [Talent/Window/ChannelsTalentGetWindowProductDetailRequest.cs](../../../Mud.Wechat.Channels/Talent/Window/ChannelsTalentGetWindowProductDetailRequest.cs) | `product_id` |
| `ChannelsTalentGetWindowProductListResponse` | 应答 | [Talent/Window/ChannelsTalentGetWindowProductListResponse.cs](../../../Mud.Wechat.Channels/Talent/Window/ChannelsTalentGetWindowProductListResponse.cs) | `products`, `last_buffer` |
| `ChannelsTalentGetWindowProductListRequest` | 请求体 | [Talent/Window/ChannelsTalentGetWindowProductListRequest.cs](../../../Mud.Wechat.Channels/Talent/Window/ChannelsTalentGetWindowProductListRequest.cs) | `page_size`, `page_index`, `last_buffer` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Talent;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddTalentApi());

var service = app.Services.GetRequiredService<IChannelsTalentService>();
var ct = CancellationToken.None;
var response = await service.GetOrderDetailAsync(new ChannelsTalentGetOrderDetailRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/talent/get_order_detail");
// 占位值仅用于编译示意；业务值须按 IChannelsTalentService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetOrderDetailAsync`）；请求体 `ChannelsTalentGetOrderDetailRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsTalentService](./IChannelsTalentService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsTalentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs) |
| 守卫中的代表性测试方法 | `TalentEndpoints_ShouldMatchOfficialRoutes`、`TalentEndpoints_ShouldAllBePost_WithSingleBody_AndTypedResponses`、`TalentInterface_ShouldBindChannelsToken_AndRegisterUnderTalentGroup`、`TalentDataModels_ShouldBeRegisteredInJsonContext`、`TalentDataModels_ShouldLockOfficialFieldsAndSplitDecisions`、`TalentConstants_ShouldLockOfficialEnumValues_AndStaySplitFromLeague`、`TalentDataModels_ShouldRecordOfficialContradictions` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/Talent`](../../../Mud.Wechat.Channels/Talent/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsTalentContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

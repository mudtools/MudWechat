# MiniStore 域 · 连接小程序

微信小店 / 视频号（channels 生态）「连接小程序」域 SDK（10 端点：文件下载链接 / 资料上传 /
礼物活动查询 / 授权送礼礼物单全链路）。 接口：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)；注册组 `MiniStore`；模块枚举 `ChannelsModule.MiniStore`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsMiniStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMiniStoreContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/MiniStore`](../../../Mud.Wechat.Channels.DataModels/MiniStore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsMiniStoreService = 10；按 HTTP 特性实计 **10** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsMiniStoreService` | `GetDownloadUrlAsync` | POST | `/channels/ec/open/get_download_url` | `AccessToken`（Query），Query `access_token` | 获取文件下载链接（`get_download_url`，定制商品用户上传资料下载）。 |
| 2 | `IChannelsMiniStoreService` | `UploadMaterialAsync` | POST | `/channels/ec/open/upload` | `AccessToken`（Query），Query `access_token` | 上传资料（`upload`，下单后传给商家进行定制商品发货）。 |
| 3 | `IChannelsMiniStoreService` | `GetPresentActivityListAsync` | POST | `/channels/ec/b2c/activity/list/promoter/get` | `AccessToken`（Query），Query `access_token` | 查询小店礼物活动列表（`activity/list/promoter/get`，授权送礼活动）。 |
| 4 | `IChannelsMiniStoreService` | `GetPresentActivityAsync` | POST | `/channels/ec/b2c/activity/info/promoter/get` | `AccessToken`（Query），Query `access_token` | 查询礼物活动详情（`activity/info/promoter/get`）。 |
| 5 | `IChannelsMiniStoreService` | `SetPresentReceiverAsync` | POST | `/channels/ec/order/presentorder/receiver/set` | `AccessToken`（Query），Query `access_token` | 指定礼物收礼者（`presentorder/receiver/set`，按子单指定收礼者 openid）。 |
| 6 | `IChannelsMiniStoreService` | `CreatePresentOrderAsync` | POST | `/channels/ec/order/presentorder/create` | `AccessToken`（Query），Query `access_token` | 创建并发送礼物（`presentorder/create`，发送授权送礼商品给指定用户）。 |
| 7 | `IChannelsMiniStoreService` | `GetPresentListAsync` | POST | `/channels/ec/order/presentlist/get` | `AccessToken`（Query），Query `access_token` | 查询礼物订单列表（`presentlist/get`，当前小程序发放的礼物单 id 列表）。 |
| 8 | `IChannelsMiniStoreService` | `GetReceiverOrderListAsync` | POST | `/channels/ec/order/receiverorderlist/get` | `AccessToken`（Query），Query `access_token` | 查询收礼者订单列表（`receiverorderlist/get`，发放给某收礼者的订单）。 |
| 9 | `IChannelsMiniStoreService` | `GetPresentAsync` | POST | `/channels/ec/order/present/get` | `AccessToken`（Query），Query `access_token` | 查询礼物订单详情（`present/get`，礼物单全量信息）。 |
| 10 | `IChannelsMiniStoreService` | `SetPresentWishMessageAsync` | POST | `/channels/ec/order/presentorder/wishmessage/set` | `AccessToken`（Query），Query `access_token` | 指定礼物单祝福语（`presentorder/wishmessage/set`，改写礼物单祝福语）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**域定位（设计方案 v1「连接小程序」）**：本域是小程序侧消费小店能力的开放面
（`/channels/ec/open/*`、`/channels/ec/b2c/*`、`/channels/ec/order/present*`），
令牌仍按小店线统一为 `ChannelsTokenTypes.AccessToken`（官方 upload 端点原文亦接受
authorizer_access_token 代调用）。

**礼物单子单状态契约**：见 `ChannelsMiniStorePresentSubOrderStatuses`
（12 待收下 → 20/21/30 → 100 完成；180 过期 / 200 全部售后后取消）。

**祝福语契约**：创建礼物单时 `wishmessage` 最大 100 字符；改写祝福语
（`wishmessage/set`）仅可在「指定礼物收礼者」之前，且中文 ≤ 20 字 / 英文 ≤ 40 字符（6080091）。

**与 Order 域边界**：小店订单主链路（`/channels/ec/order/*` 下单 / 查单 / 发货等）落
`IChannelsOrderService`；本域仅承载「合作商家为小程序发放礼物」族的礼物单端点与
连接小程序基础端点，两域无路由交叠。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 10 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetDownloadUrlAsync` — 获取文件下载链接（`get_download_url`，定制商品用户上传资料下载）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/open/get_download_url`

- **请求体**：`ChannelsMiniStoreGetDownloadUrlRequest` — 查询请求（`openid` / `order_id` 必填）。
- **返回**：`ChannelsMiniStoreGetDownloadUrlResponse` — 文件下载链接（`url`）与到期时间（`expire_time`，秒级时间戳，有效期 10 分钟）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/basic/api_getdownloadurl
- 官方业务限制：①频率限制为 **order_id 维度 10 次/天**；②**订单已结算或超过售后期 7 天后， / 不可获取对应的文件下载链接**；③链接有效期 10 分钟（官方变更记录：原 download_url 字段已修正为 url）。
- 官方错误码：`10005`（订单已取消，禁止下载文件）/ `10006`（订单已结束，禁止下载文件）/ / `10009`（下载文件次数超过日上限）等。
- 第三方平台代调用所属权限集 id 为 `131`。

### 2. `UploadMaterialAsync` — 上传资料（`upload`，下单后传给商家进行定制商品发货）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/open/upload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `uploadType` | Query `upload_type` | `int` | 上传方式（官方 `upload_type`，Query 参数，见 `ChannelsMiniStoreUploadTypes`）——<br>SDK 建模官方 JSON 形态（upload_type=1 文件 url 上传）；`upload_type=0` 二进制上传官方为<br>multipart/form-data，不在本 JSON 方法语义内。 |

- **请求体**：`ChannelsMiniStoreUploadMaterialRequest` — 上传请求（`openid` 必填——upload_type=1 时走请求体；`file_url`
必填，支持 <50M 的文件）。
- **返回**：`ChannelsMiniStoreUploadMaterialResponse` — 文件 media_id（用于后续下发商家）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/basic/api_uploadec
- 官方业务限制：①官方提供两种形态——upload_type=0 二进制上传（multipart/form-data，文件 <10M、 / openid 走 Query）与 upload_type=1 URL 上传（JSON 请求体，文件 <50M）；SDK 按 JSON 形态建模； / ②支持格式见官方表（图片 / CAD / 3DMax / 压缩包 / 普通文件），压缩包中只能包含支持的文件类型； / ③适用范围官方标注「小程序」账号类型调用。
- 官方错误码：`10004`（上传文件存在风险）/ `10007`（上传文件格式不支持）/ / `10008`（上传文件的大小超过最大值）/ `10010`（上传 url 无效）。
- 第三方平台代调用所属权限集 id 为 `180`。

### 3. `GetPresentActivityListAsync` — 查询小店礼物活动列表（`activity/list/promoter/get`，授权送礼活动）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/b2c/activity/list/promoter/get`

- **请求体**：`ChannelsMiniStoreGetActivityListRequest` — 查询请求（`offset` / `limit` 必填，limit 最大 100）。
- **返回**：`ChannelsMiniStoreGetActivityListResponse` — 礼物活动列表（`list`：小店 appid / 昵称 / 活动 id / 状态 / 类型）与总数（`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_list_present_activity
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；活动状态枚举见 / `ChannelsMiniStoreActivityStatuses`，类型枚举见 `ChannelsMiniStoreActivityTypes`。
- 官方错误码：`40097`（参数错误）/ `10020002`（非法的 limit 参数，应该小于等于 100）/ / `10028004`（推广者账号送礼被封禁）。

### 4. `GetPresentActivityAsync` — 查询礼物活动详情（`activity/info/promoter/get`）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/b2c/activity/info/promoter/get`

- **请求体**：`ChannelsMiniStoreGetActivityRequest` — 查询请求（`shop_appid` / `activity_id` 必填）。
- **返回**：`ChannelsMiniStoreGetActivityResponse` — 活动信息（`activity`：活动 id / 状态 / 基础信息（起止时间 + 小店信息）/ 礼物商品信息列表）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_activity
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；`can_use_stock_num`（剩余库存） / 仅小店授权类型活动才有，`present_order_id`（礼物订单号）仅自购类型才有。
- 官方错误码：`40097`（参数错误）/ `10028003`（活动 id 不存在）。

### 5. `SetPresentReceiverAsync` — 指定礼物收礼者（`presentorder/receiver/set`，按子单指定收礼者 openid）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/presentorder/receiver/set`

- **请求体**：`ChannelsMiniStoreSetPresentReceiverRequest` — 指定请求（`present_order_id` / `sub_order_list` 必填，子单不可为空）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_set_present_receiver
- 官方业务限制：①指定收礼者后不允许再修改祝福语（与 `wishmessage/set` 的时序契约）； / ②多笔子单不可指定同样的收礼者（6080079）；③同一子单不可重复指定（6080076）。
- 官方错误码：`6080073`（礼物单状态错误）/ `6080074`（当前礼物单暂不支持此操作）/ / `6080075`（子单号错误）/ `6080076`（当前子单已经指定过收礼者）/ `6080077`（礼物单号错误）/ / `6080078`（子单列表为空）/ `6080079`（多笔子单指定了同样的收礼者）/ `6080095`（外部 APP 收礼场景传参错误）。

### 6. `CreatePresentOrderAsync` — 创建并发送礼物（`presentorder/create`，发送授权送礼商品给指定用户）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/presentorder/create`

- **请求体**：`ChannelsMiniStoreCreatePresentOrderRequest` — 创建请求（`activity_id` / `openid` / `shop_appid` / `wishmessage` /
`product_id` / `sku_id` 必填；祝福语最大 100 字符）。
- **返回**：`ChannelsMiniStoreCreatePresentOrderResponse` — 礼物单 id（`present_order_id`）与外部应用跳转字段（business_type / query，仅外部应用身份调用返回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_create_present_order
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；送礼商品价格不得超过 2 万元（608021）； / 传入幂等 id（idempotent_id）后相同幂等 id 的并发请求返回 6011104（请求处理中）。
- 官方错误码：`108001`（库存不足）/ `108002`（商品状态异常）/ `608010`（平台风控）/ / `608016`（该商品不在礼物池）/ `608021`（商品送礼价格过高）/ `6011100`（活动类型错误）/ / `6011102`（祝福语长度过长）/ `6011103`（活动侧全 sku 库存不足）/ `6011104`（请求处理中）/ / `6080060`（送礼份数超出预设活动库存）/ `6080061`（送礼商品非预设活动商品）。

### 7. `GetPresentListAsync` — 查询礼物订单列表（`presentlist/get`，当前小程序发放的礼物单 id 列表）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/presentlist/get`

- **请求体**：`ChannelsMiniStoreGetPresentListRequest` — 查询请求（`shop_appid` / `page_size` 必填；创建 / 更新时间范围至少填一个，
跨度均不可超过 7 天；游标翻页经 `next_key`，首次置空）。
- **返回**：`ChannelsMiniStoreGetPresentListResponse` — 礼物单列表（`present_order_list`，元素含礼物单 id）与翻页游标（`next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_present_list
- 官方业务限制：①`page_size` 不超过 100；②时间范围（创建或更新时间二选一填写）跨度不可超过 7 天； / ③官方参数表将 `present_order_list` 声明为 objarray、返回示例以对象形式（含 present_order_id） / 展示——SDK 按参数表元素形态（对象）建模。
- 官方错误码：无特殊错误码（走通用错误码）。

### 8. `GetReceiverOrderListAsync` — 查询收礼者订单列表（`receiverorderlist/get`，发放给某收礼者的订单）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/receiverorderlist/get`

- **请求体**：`ChannelsMiniStoreGetReceiverOrderListRequest` — 查询请求（`shop_appid` / `openid` / `page_size` 必填；时间范围约束同礼物单列表）。
- **返回**：`ChannelsMiniStoreGetReceiverOrderListResponse` — 订单列表（`order_list`：礼物单 id + 订单 id）与翻页游标（`next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_receiver_order_list
- 官方业务限制：①时间范围跨度不可超过 7 天；②`page_size` 不超过 100；③游标翻页（next_key）。
- 官方错误码：`40003`（openid 错误）/ `40097`（请求体参数不正确）。

### 9. `GetPresentAsync` — 查询礼物订单详情（`present/get`，礼物单全量信息）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/present/get`

- **请求体**：`ChannelsMiniStoreGetPresentRequest` — 查询请求（`present_order_id` 必填）。
- **返回**：`ChannelsMiniStoreGetPresentResponse` — 礼物单结构（`present`：礼物单 id / 创建时间 / 价格与支付信息 / 祝福语 /
子单列表（子单状态见 `ChannelsMiniStorePresentSubOrderStatuses`）/ 是否无需付款 / 关联活动 id）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_present
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；**官方文档矛盾照录**： / `is_b2c_free_present` 官方声明 boolean 类型但描述为「0：已付款 1：无需付款」——SDK 按声明 / 类型 boolean 建模，0/1 描述照录。
- 官方错误码：`6080077`（礼物单号错误）。

### 10. `SetPresentWishMessageAsync` — 指定礼物单祝福语（`presentorder/wishmessage/set`，改写礼物单祝福语）。

- **声明接口**：[IChannelsMiniStoreService](./IChannelsMiniStoreService.cs)
- **官方契约**：`POST /channels/ec/order/presentorder/wishmessage/set`

- **请求体**：`ChannelsMiniStoreSetPresentWishMessageRequest` — 改写请求（`present_order_id` / `wish_message` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_set_present_wishmessage
- 官方业务限制：①仅可在调用「指定礼物收礼者」接口**之前**修改祝福语，指定收礼者后不允许修改（6080076）； / ②长度限制：纯中文最多 20 个汉字，纯英文最多 40 个字符（6080091）；③祝福语内容安全校验未通过报 6080085。
- 官方错误码：`6080073`（礼物单状态错误）/ `6080074`（当前礼物单暂不支持此操作）/ / `6080076`（礼物子单已指定收礼人，不允许修改祝福语）/ `6080077`（礼物单号错误）/ / `6080085`（祝福语内容安全校验未通过）/ `6080091`（祝福语长度超过限制）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/MiniStore`](../../../Mud.Wechat.Channels.DataModels/MiniStore/) 内共 **34** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **34** 个，`SerializerClassName` = `MiniStore`。

AOT 源生成上下文：[`MiniStoreJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/MiniStoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsMiniStoreGetDownloadUrlResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetDownloadUrlResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetDownloadUrlResponse.cs) | `url`, `expire_time` |
| `ChannelsMiniStoreGetDownloadUrlRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetDownloadUrlRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetDownloadUrlRequest.cs) | `openid`, `order_id` |
| `ChannelsMiniStoreUploadMaterialResponse` | 应答 | [MiniStore/ChannelsMiniStoreUploadMaterialResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreUploadMaterialResponse.cs) | `media_id` |
| `ChannelsMiniStoreUploadMaterialRequest` | 请求体 | [MiniStore/ChannelsMiniStoreUploadMaterialRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreUploadMaterialRequest.cs) | `openid`, `file_url` |
| `ChannelsMiniStoreGetActivityListResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetActivityListResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetActivityListResponse.cs) | `list`, `total_num` |
| `ChannelsMiniStoreGetActivityListRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetActivityListRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetActivityListRequest.cs) | `offset`, `limit`, `shop_appid`, `status`, `type` |
| `ChannelsMiniStoreGetActivityResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetActivityResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetActivityResponse.cs) | `activity` |
| `ChannelsMiniStoreGetActivityRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetActivityRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetActivityRequest.cs) | `shop_appid`, `activity_id` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels.DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsMiniStoreSetPresentReceiverRequest` | 请求体 | [MiniStore/ChannelsMiniStoreSetPresentReceiverRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreSetPresentReceiverRequest.cs) | `present_order_id`, `sub_order_list` |
| `ChannelsMiniStoreCreatePresentOrderResponse` | 应答 | [MiniStore/ChannelsMiniStoreCreatePresentOrderResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreCreatePresentOrderResponse.cs) | `present_order_id`, `business_type`, `query` |
| `ChannelsMiniStoreCreatePresentOrderRequest` | 请求体 | [MiniStore/ChannelsMiniStoreCreatePresentOrderRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreCreatePresentOrderRequest.cs) | `activity_id`, `openid`, `shop_appid`, `wishmessage`, `product_id`, `sku_id`, `idempotent_id` |
| `ChannelsMiniStoreGetPresentListResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetPresentListResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetPresentListResponse.cs) | `present_order_list`, `next_key` |
| `ChannelsMiniStoreGetPresentListRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetPresentListRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetPresentListRequest.cs) | `create_time_range`, `update_time_range`, `shop_appid`, `page_size`, `next_key` |
| `ChannelsMiniStoreGetReceiverOrderListResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetReceiverOrderListResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetReceiverOrderListResponse.cs) | `order_list`, `next_key` |
| `ChannelsMiniStoreGetReceiverOrderListRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetReceiverOrderListRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetReceiverOrderListRequest.cs) | `create_time_range`, `update_time_range`, `shop_appid`, `openid`, `page_size`, `next_key` |
| `ChannelsMiniStoreGetPresentResponse` | 应答 | [MiniStore/ChannelsMiniStoreGetPresentResponse.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetPresentResponse.cs) | `present` |
| `ChannelsMiniStoreGetPresentRequest` | 请求体 | [MiniStore/ChannelsMiniStoreGetPresentRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreGetPresentRequest.cs) | `present_order_id` |
| `ChannelsMiniStoreSetPresentWishMessageRequest` | 请求体 | [MiniStore/ChannelsMiniStoreSetPresentWishMessageRequest.cs](../../../Mud.Wechat.Channels.DataModels/MiniStore/ChannelsMiniStoreSetPresentWishMessageRequest.cs) | `present_order_id`, `wish_message` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.MiniStore;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddMiniStoreApi());

var service = app.Services.GetRequiredService<IChannelsMiniStoreService>();
var ct = CancellationToken.None;
var response = await service.CreatePresentOrderAsync(new ChannelsMiniStoreCreatePresentOrderRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/order/presentorder/create");
// 占位值仅用于编译示意；业务值须按 IChannelsMiniStoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreatePresentOrderAsync`）；请求体 `ChannelsMiniStoreCreatePresentOrderRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsMiniStoreService](./IChannelsMiniStoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsMiniStoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsMiniStoreContractGuards.cs) |
| 守卫中的代表性测试方法 | `MiniStoreEndpoints_ShouldMatchOfficialRoutes`、`MiniStoreEndpoints_ShouldAllBePost_WithUploadQueryRules`、`MiniStoreInterface_ShouldBindChannelsToken_AndRegisterUnderMiniStoreGroup`、`MiniStoreDataModels_ShouldBeRegisteredInJsonContext`、`MiniStoreDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`MiniStoreEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/MiniStore`](../../../Mud.Wechat.Channels.DataModels/MiniStore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsMiniStoreContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

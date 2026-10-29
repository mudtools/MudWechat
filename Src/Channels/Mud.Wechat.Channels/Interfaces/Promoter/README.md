# Promoter 域 · 联盟带货机构推客（promoter）

微信小店 / 视频号（channels 生态）「联盟带货机构推客（promoter）」域 SDK（41 端点：推客账号与分佣 7 + 机构优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7）。 接口：[IChannelsPromoterService](./IChannelsPromoterService.cs)；注册组 `Promoter`；模块枚举 `ChannelsModule.Promoter`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsPromoterContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPromoterContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Promoter`](../../DataModels/Promoter)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsPromoterService = 41；按 HTTP 特性实计 **41** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsPromoterService` | `GetBindSharerListAsync` | POST | `/channels/ec/promoter/get_bind_sharer_list` | `AccessToken`（Query），Query `access_token` | 获取机构绑定的推客信息（`promoter/get_bind_sharer_list`，推客列表分页查询）。 |
| 2 | `IChannelsPromoterService` | `GetBindShopListAsync` | POST | `/channels/ec/promoter/get_bind_shop_list` | `AccessToken`（Query），Query `access_token` | 获取合作的小店列表（`promoter/get_bind_shop_list`，亦可在「微信小店·联盟带货机构 - 关系管理 - 合作商家」查看）。 |
| 3 | `IChannelsPromoterService` | `GetBindShopPromoterListAsync` | POST | `/channels/ec/promoter/get_bind_shop_promoter_list` | `AccessToken`（Query），Query `access_token` | 获取合作小店的关联账号列表（`promoter/get_bind_shop_promoter_list`）。 |
| 4 | `IChannelsPromoterService` | `GetBindTalentListAsync` | POST | `/channels/ec/promoter/get_bind_talent_list` | `AccessToken`（Query），Query `access_token` | 获取合作的带货者列表（`promoter/get_bind_talent_list`）。 |
| 5 | `IChannelsPromoterService` | `GetPromoterRegisterAndBindStatusAsync` | POST | `/channels/ec/promoter/get_promoter_register_and_bind_status` | `AccessToken`（Query），Query `access_token` | 获取推客的注册状态以及和机构的绑定状态（`promoter/get_promoter_register_and_bind_status`）。 |
| 6 | `IChannelsPromoterService` | `HeadsupplierUnbindPromoterAsync` | POST | `/channels/ec/promoter/headsupplier_unbind_promoter` | `AccessToken`（Query），Query `access_token` | 机构解绑推客（`promoter/headsupplier_unbind_promoter`）。 |
| 7 | `IChannelsPromoterService` | `SetSharerCommissionInfoAsync` | POST | `/channels/ec/promoter/set_sharer_commission_info` | `AccessToken`（Query），Query `access_token` | 设置推客的分佣类型和比例信息（`promoter/set_sharer_commission_info`）。 |
| 8 | `IChannelsPromoterService` | `GetCooperativeCouponListAsync` | POST | `/channels/ec/promoter/get_cooperative_coupon_list` | `AccessToken`（Query），Query `access_token` | 获取定向券 ID 列表（`promoter/get_cooperative_coupon_list`）。 |
| 9 | `IChannelsPromoterService` | `GetPublicCouponListAsync` | POST | `/channels/ec/promoter/get_public_coupon_list` | `AccessToken`（Query），Query `access_token` | 获取公开券 ID 列表（`promoter/get_public_coupon_list`）。 |
| 10 | `IChannelsPromoterService` | `GetCouponDetailAsync` | POST | `/channels/ec/promoter/get_coupon_detail` | `AccessToken`（Query），Query `access_token` | 获取券 ID 对应的详情内容（`promoter/get_coupon_detail`）。 |
| 11 | `IChannelsPromoterService` | `GetCouponPromoterShareLinkAsync` | POST | `/channels/ec/promoter/get_coupon_promoter_share_link` | `AccessToken`（Query），Query `access_token` | 获取某个券 ID 对应的小程序内嵌微信小店优惠券推广参数（`promoter/get_coupon_promoter_share_link`）。 |
| 12 | `IChannelsPromoterService` | `GetCouponQrCodeAsync` | POST | `/channels/ec/promoter/get_coupon_qr_code` | `AccessToken`（Query），Query `access_token` | 获取某个券 ID 对应的推广二维码（`promoter/get_coupon_qr_code`）。 |
| 13 | `IChannelsPromoterService` | `GetCouponShortLinkAsync` | POST | `/channels/ec/promoter/get_coupon_short_link` | `AccessToken`（Query），Query `access_token` | 获取某个券 ID 对应的推广短链（`promoter/get_coupon_short_link`）。 |
| 14 | `IChannelsPromoterService` | `GetCouponToHeadSupplierAsync` | POST | `/channels/ec/promoter/get_coupon_to_headsupplier` | `AccessToken`（Query），Query `access_token` | 获取分发给其他机构的优惠券（`promoter/get_coupon_to_headsupplier`，按接收机构维度分页查询分发记录）。 |
| 15 | `IChannelsPromoterService` | `SetCouponToHeadSupplierAsync` | POST | `/channels/ec/promoter/set_coupon_to_headsupplier` | `AccessToken`（Query），Query `access_token` | 分发优惠券给其他机构（`promoter/set_coupon_to_headsupplier`）。 |
| 16 | `IChannelsPromoterService` | `GetFeedListAsync` | POST | `/channels/ec/promoter/get_feed_list` | `AccessToken`（Query），Query `access_token` | 获取带货者推广的短视频信息（`promoter/get_feed_list`，带货者侧短视频列表）。 |
| 17 | `IChannelsPromoterService` | `GetFeedPromotionInfoAsync` | POST | `/channels/ec/promoter/get_feed_promotion_info` | `AccessToken`（Query），Query `access_token` | 生成某个带货者的某些短视频内嵌短视频卡片需要的 feedtoken（`promoter/get_feed_promotion_info`）。 |
| 18 | `IChannelsPromoterService` | `GetShopFeedListAsync` | POST | `/channels/ec/promoter/get_shop_feed_list` | `AccessToken`（Query），Query `access_token` | 获取小店关联账号的短视频列表（`promoter/get_shop_feed_list`，本店侧短视频列表）。 |
| 19 | `IChannelsPromoterService` | `GetShopFeedPromotionInfoAsync` | POST | `/channels/ec/promoter/get_shop_feed_promotion_info` | `AccessToken`（Query），Query `access_token` | 生成小店关联账号的短视频内嵌小程序推广信息（`promoter/get_shop_feed_promotion_info`）。 |
| 20 | `IChannelsPromoterService` | `GetLiveCommissionProductListAsync` | POST | `/channels/ec/promoter/get_live_commission_product_list` | `AccessToken`（Query），Query `access_token` | 获取某个带货者当前直播的带货商品列表（`promoter/get_live_commission_product_list`）。 |
| 21 | `IChannelsPromoterService` | `GetShopLiveCommissionProductListAsync` | POST | `/channels/ec/promoter/get_shop_live_commission_product_list` | `AccessToken`（Query），Query `access_token` | 获取小店关联账号的当前直播的自营商品列表（`promoter/get_shop_live_commission_product_list`）。 |
| 22 | `IChannelsPromoterService` | `GetLiveNoticePromoterShareLinkAsync` | POST | `/channels/ec/promoter/get_live_notice_promoter_share_link` | `AccessToken`（Query），Query `access_token` | 生成某个带货者的用于直播预约的推广参数（`promoter/get_live_notice_promoter_share_link`）。 |
| 23 | `IChannelsPromoterService` | `GetShopLiveNoticePromoterShareLinkAsync` | POST | `/channels/ec/promoter/get_shop_live_notice_promoter_share_link` | `AccessToken`（Query），Query `access_token` | 为推客获取小店关联账号直播预约的推广参数（`promoter/get_shop_live_notice_promoter_share_link`）。 |
| 24 | `IChannelsPromoterService` | `GetLiveNoticeRecordListAsync` | POST | `/channels/ec/promoter/get_live_notice_record_list` | `AccessToken`（Query），Query `access_token` | 获取带货者的直播预约列表（`promoter/get_live_notice_record_list`，官方无分页参数）。 |
| 25 | `IChannelsPromoterService` | `GetShopLiveNoticeRecordListAsync` | POST | `/channels/ec/promoter/get_shop_live_notice_record_list` | `AccessToken`（Query），Query `access_token` | 获取小店关联账号的预约直播列表（`promoter/get_shop_live_notice_record_list`，官方无分页参数）。 |
| 26 | `IChannelsPromoterService` | `GetLiveNoticeRecordQrCodeAsync` | POST | `/channels/ec/promoter/get_live_notice_record_qr_code` | `AccessToken`（Query），Query `access_token` | 为某个推客生成单个直播预约的推广二维码（`promoter/get_live_notice_record_qr_code`）。 |
| 27 | `IChannelsPromoterService` | `GetShopLiveNoticeRecordQrCodeAsync` | POST | `/channels/ec/promoter/get_shop_live_notice_record_qr_code` | `AccessToken`（Query），Query `access_token` | 为推客生成小店关联账号直播预约的推广二维码（`promoter/get_shop_live_notice_record_qr_code`）。 |
| 28 | `IChannelsPromoterService` | `GetLiveNoticeReservationInfoAsync` | POST | `/channels/ec/promoter/get_live_notice_reservation_info` | `AccessToken`（Query），Query `access_token` | 获取直播预约的单个推客的推广预约人数（`promoter/get_live_notice_reservation_info`）。 |
| 29 | `IChannelsPromoterService` | `GetLiveRecordListAsync` | POST | `/channels/ec/promoter/get_live_record_list` | `AccessToken`（Query），Query `access_token` | 获取带货者的直播列表（`promoter/get_live_record_list`）。 |
| 30 | `IChannelsPromoterService` | `GetShopLiveRecordListAsync` | POST | `/channels/ec/promoter/get_shop_live_record_list` | `AccessToken`（Query），Query `access_token` | 获取小店关联账号的直播列表（`promoter/get_shop_live_record_list`）。 |
| 31 | `IChannelsPromoterService` | `GetLiveRecordQrCodeAsync` | POST | `/channels/ec/promoter/get_live_record_qr_code` | `AccessToken`（Query），Query `access_token` | 为某个推客生成单个直播的推广二维码（`promoter/get_live_record_qr_code`）。 |
| 32 | `IChannelsPromoterService` | `GetShopLiveRecordQrCodeAsync` | POST | `/channels/ec/promoter/get_shop_live_record_qr_code` | `AccessToken`（Query），Query `access_token` | 为推客生成小店关联账号直播的推广二维码（`promoter/get_shop_live_record_qr_code`）。 |
| 33 | `IChannelsPromoterService` | `GetShopMpArticleListAsync` | POST | `/channels/ec/promoter/get_shop_mp_article_list` | `AccessToken`（Query），Query `access_token` | 获取小店关联账号的文章列表（`promoter/get_shop_mp_article_list`）。 |
| 34 | `IChannelsPromoterService` | `GetTalentBizArticleListAsync` | POST | `/channels/ec/promoter/get_talent_biz_article_list` | `AccessToken`（Query），Query `access_token` | 获取某个带货者可推广文章列表（`promoter/get_talent_biz_article_list`）。 |
| 35 | `IChannelsPromoterService` | `GetProductPromotionLinkInfoAsync` | POST | `/channels/ec/promoter/get_product_promotion_link_info` | `AccessToken`（Query），Query `access_token` | 获取推客对某个商品的推广短链（`promoter/get_product_promotion_link_info`）。 |
| 36 | `IChannelsPromoterService` | `GetProductPromotionQrCodeInfoAsync` | POST | `/channels/ec/promoter/get_product_promotion_qrcode_info` | `AccessToken`（Query），Query `access_token` | 获取推客对某个商品的推广二维码（`promoter/get_product_promotion_qrcode_info`）。 |
| 37 | `IChannelsPromoterService` | `GetPromoterSingleProductPromotionInfoAsync` | POST | `/channels/ec/promoter/get_promoter_single_product_promotion_info` | `AccessToken`（Query），Query `access_token` | 获取某个推客某个商品的内嵌商品卡片 product_promotion_link（`promoter/get_promoter_single_product_promotion_i… |
| 38 | `IChannelsPromoterService` | `GetPromoteProductDetailAsync` | POST | `/channels/ec/promoter/get_promote_product_detail` | `AccessToken`（Query），Query `access_token` | 获取可推广的商品详情（`promoter/get_promote_product_detail`）。 |
| 39 | `IChannelsPromoterService` | `GetPromoteProductListAsync` | POST | `/channels/ec/promoter/get_promote_product_list` | `AccessToken`（Query），Query `access_token` | 获取可推广的商品 id 列表（`promoter/get_promote_product_list`，支持多维筛选）。 |
| 40 | `IChannelsPromoterService` | `GetSharerProductCommissionInfoAsync` | POST | `/channels/ec/promoter/get_sharer_product_commission_info` | `AccessToken`（Query），Query `access_token` | 获取推客的某个商品的推广分佣比例（`promoter/get_sharer_product_commission_info`）。 |
| 41 | `IChannelsPromoterService` | `SetSharerProductCommissionInfoAsync` | POST | `/channels/ec/promoter/set_sharer_product_commission_info` | `AccessToken`（Query），Query `access_token` | 设置推客的单个商品的分佣比例信息（`promoter/set_sharer_product_commission_info`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**视角与令牌归属（与 League 域不得合并）**：本域全部路由为 `/channels/ec/promoter/*`，
由「联盟带货机构（headsupplier）」以**机构自身 `access_token`** 调用，官方适用范围恒为
「本接口支持『联盟带货机构』账号类型调用。其他账号类型如无特殊说明，均不可调用」，且逐页均声明
**不支持云调用** 与 **不支持第三方平台调用**——与 League 域小店商家侧（`/channels/ec/league/*`、
第三方权限集 141 可代调用）是两套公开面，跨线代调用一律不可用。

**「promoter」一词在本仓两处语义不同**：本域路径段 `promoter` 指**推客**
（`sharer_appid` / `sharer_openid`，微信电商平台的推广身份），而 League 域
`/channels/ec/league/promoter/*` 的 `promoter` 指**带货者**（小店商家侧推广对象）。
路径前缀即视角，字段与错误码各自成表，不得因同名而合并或复用 DTO。

**同域双视角（带货者侧 vs 小店关联账号侧，成对端点不得合并）**：短视频、直播、文章三族各有成对端点——
①**带货者（talent）侧**（`get_feed_list` / `get_live_*` / `get_talent_biz_article_list`，
定位键 `talent_appid`）；②**小店关联账号（shop）侧**（`get_shop_*`，定位键为
`shop_appid` + `promoter_id` + `promoter_type` 三元组，取自
`GetBindShopListAsync` 与 `GetBindShopPromoterListAsync`）。
两视角响应子结构字段集不同（shop 侧条目无 `talent_appid`、直播商品条目多出 `commission_rate`），
且 shop 侧独有错误码 `10071055`（关联账号 id 错误）——故 DTO 分建，仅官方参数表逐字段一致的类型共用。

**`promoter_type` 取值按族收敛**：短视频与直播族官方要求「需要是视频号」（`1`），
文章族要求「需要是公众号或服务号」（`4` / `5`），枚举见 `ChannelsPromoterShopPromoterTypes`；
官方 2025-12-25 变更日志仅更新该字段描述、未改值域。
`promoter_type` 在 `get_shop_feed_promotion_info` 的参数表中标为**选填**，而同族其余 shop 侧端点均必填，
属官方文档不一致，已在对应方法 remarks 照录。

**翻页形态三套并存**：①多数列表走 `next_key` + `page_size`（首屏 `next_key` 传空，
上限逐端点不同：推客 `20` / 券 `200` / 可推广商品 `20` / 其余 `10`）；
②`get_coupon_to_headsupplier` 走 `limit` + `page_context` 且响应回 `total_count`；
③直播预约 / 直播列表族官方**无分页参数**。另 `GetBindSharerListAsync` 响应
**不含** `has_more` 而 `GetCouponDetailAsync`（详情端点）反而带 `next_key` + `has_more`，
两处均属官方参数表原样，不得「顺手对齐」。

**参数表与代码示例类型矛盾（本域按官方参数表定类型，矛盾逐条照录于对应方法 remarks 并由守卫锁定）**：
例 `commission_info.status`（表 string / 枚举与示例 number）、`category_id`（表 number /
示例值超 Int64 范围）、`sku_attrs`（表 object / 示例 array）、
`set_sharer_product_commission_info.commission_ratio`（表 string / 同义兄弟端点 number）、
列表侧 `product_id`（string）与详情侧 `product_id`（number）等。
另：官方代码示例的 `errcode` 恒写作字符串 `"0"`，与参数表 `number` 冲突，本仓统一按参数表取
`ErrorCode` 的整型形态。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 41 次）适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。
- （出现 41 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 3 次）官方错误码：`10024000`（参数错误）。
- （出现 5 次）官方错误码：`10024000`（参数错误）/ `10071048`（参数异常，请检查入参）。
- （出现 6 次）接口变更日志：2025 年 11 月 24 日新增错误码 `10071048`。
- （出现 3 次）官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）。

### 1. `GetBindSharerListAsync` — 获取机构绑定的推客信息（`promoter/get_bind_sharer_list`，推客列表分页查询）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_bind_sharer_list`

- **请求体**：`ChannelsPromoterGetBindSharerListRequest` — 查询请求（`next_key` 与 `page_size` 必填；`sharer_appid` / `sharer_openid` 选填且二选一）。
- **返回**：`ChannelsPromoterGetBindSharerListResponse` — 推客列表与下一页参数（`sharer_info_list` / `next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindsharerlist.html
- 官方业务限制：①`page_size` 一页获取多少个推客，最大 `20`；②`next_key` 分页参数，第一页为空， / 后面返回前面一页返回的数据；③`sharer_openid` **必须是机构绑定小程序的用户 openid**， / 而不能是其他小程序的用户 openid。
- 官方参数表照录：响应体**只有** `sharer_info_list` 与 `next_key`，**不含** `has_more`（同域其余 next_key 分页端点均回 `has_more`）。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071003`。

### 2. `GetBindShopListAsync` — 获取合作的小店列表（`promoter/get_bind_shop_list`，亦可在「微信小店·联盟带货机构 - 关系管理 - 合作商家」查看）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_bind_shop_list`

- **请求体**：`ChannelsPromoterGetBindShopListRequest` — 查询请求（`next_key` 与 `page_size` 必填）。
- **返回**：`ChannelsPromoterGetBindShopListResponse` — 合作小店列表、下一页参数与翻页标志（`shop_list` / `next_key` / `has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindshoplist.html
- 官方业务限制：`page_size` 一页获取多少个合作小店，最大 `10`；`next_key` 第一页为空，后续传上一页返回的值。
- 返回的 `shop_appid` 是本店侧（`get_shop_*`）端点定位三元组的第一项，配合 `GetBindShopPromoterListAsync` 取 `promoter_id`。

### 3. `GetBindShopPromoterListAsync` — 获取合作小店的关联账号列表（`promoter/get_bind_shop_promoter_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_bind_shop_promoter_list`

- **请求体**：`ChannelsPromoterGetBindShopPromoterListRequest` — 查询请求（`next_key` / `page_size` / `shop_appid` 均必填）。
- **返回**：`ChannelsPromoterGetBindShopPromoterListResponse` — 关联账号列表、下一页参数与翻页标志（`promoter_list` / `next_key` / `has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindshoppromoterlist.html
- 官方业务限制：①`page_size` 一页获取多少个关联账号，最大 `10`；②`shop_appid` 必填且须与机构存在合作关系； / ③`promoter_type` 枚举见 `ChannelsPromoterShopPromoterTypes`（`1` 视频号 / `4` 公众号 / `5` 服务号）， / 官方描述「仅视频号类型有直播和短视频内容」——故文章族用 `4`/`5`，短视频与直播族用 `1`。
- 返回的 `promoter_id` + `promoter_type` 是本店侧（`get_shop_*`）端点定位三元组的后两项。
- 官方错误码：`10024000`（参数错误）/ `10071057`（shop_appid 无合作关系）。
- 接口变更日志：2025 年 12 月 25 日更新 `promoter_type` 字段描述。

### 4. `GetBindTalentListAsync` — 获取合作的带货者列表（`promoter/get_bind_talent_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_bind_talent_list`

- **请求体**：`ChannelsPromoterGetBindTalentListRequest` — 查询请求（`next_key` / `page_size` 必填；`talent_appid` 选填，用于查询单个合作带货者）。
- **返回**：`ChannelsPromoterGetBindTalentListResponse` — 合作带货者列表、下一页参数与翻页标志（`talent_list` / `next_key` / `has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindtalentlist.html
- 官方业务限制：`page_size` 一页获取多少个合作带货者，最大 `10`。 / 条目内 `related_finder_exportname_list`（关联视频号 username）与 `related_mp_biz_appid_list`（关联公众号 appid） / 是 `GetFeedListAsync` 的 `finder_exportusername` 与 `GetTalentBizArticleListAsync` 的 `biz_appid` 的官方获取方式 / （「全部内容」合作方式必填，旧「部分内容」合作方式可不填）。

### 5. `GetPromoterRegisterAndBindStatusAsync` — 获取推客的注册状态以及和机构的绑定状态（`promoter/get_promoter_register_and_bind_status`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_promoter_register_and_bind_status`

- **请求体**：`ChannelsPromoterGetRegisterAndBindStatusRequest` — 查询请求（`sharer_openid` 与 `sharer_appid` 选填；`is_simple_register` 选填）。
- **返回**：`ChannelsPromoterGetRegisterAndBindStatusResponse` — 注册与绑定状态、注册/绑定业务类型与跳转串，以及命中推客的 `sharer_appid`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getpromoterregisterandbindstatus.html
- 官方业务限制：①`register_status` 枚举见 `ChannelsPromoterRegisterStatuses` / （`0` 未注册 / `1` 注册中 / `2` 已注册 / `3` 需实名并绑定银行卡）； / ②`register_query_string` / `bind_query_string` 为官方给出的跳转业务串，由宿主拼接到小程序跳转路径； / ③以 `sharer_openid` 查询时须为机构绑定小程序的用户 openid。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071049`（推客功能已关闭）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。

### 6. `HeadsupplierUnbindPromoterAsync` — 机构解绑推客（`promoter/headsupplier_unbind_promoter`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/headsupplier_unbind_promoter`

- **请求体**：`ChannelsPromoterUnbindPromoterRequest` — 解绑请求（`sharer_appid` 必填）。
- **返回**：`ChannelsResponse` — 官方仅返回 `errcode` / `errmsg`，故回 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_mmecpromoter_headsupplier_unbind_promoter.html
- 官方业务限制：本接口无特殊注意事项；解绑为不可逆操作，须由宿主自行评估（解绑后该推客的分佣关系与推广参数全部失效）。
- 官方错误码：`10071003`（参数错误）/ `10071058`（推客没有绑定机构）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071003`。

### 7. `SetSharerCommissionInfoAsync` — 设置推客的分佣类型和比例信息（`promoter/set_sharer_commission_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/set_sharer_commission_info`

- **请求体**：`ChannelsPromoterSetSharerCommissionInfoRequest` — 设置请求（`sharer_appid` / `commission_ratio` / `commission_type` 均必填）。
- **返回**：`ChannelsResponse` — 官方仅返回 `errcode` / `errmsg`，故回 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_setsharercommissioninfo.html
- 官方业务限制：①`commission_ratio` 为「平台分佣时的分佣比例」，范围 `[100000, 1000000]`，代表 10%-100%； / ②`commission_type` 枚举见 `ChannelsPromoterCommissionTypes`（`0` 平台分佣 / `1` 机构自己分佣）。
- 官方参数表矛盾照录：本端点 `commission_ratio` 上界为 `1000000`（100%）， / 而同域 `get_sharer_product_commission_info` 的商品级比例区间写作 `[100000, 900000]`（10%-90%）——两表上界不同，各自照录、不做统一校验。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071029`（推客和机构未绑定）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071029`。

### 8. `GetCooperativeCouponListAsync` — 获取定向券 ID 列表（`promoter/get_cooperative_coupon_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_cooperative_coupon_list`

- **请求体**：`ChannelsPromoterGetCooperativeCouponListRequest` — 查询请求（`next_key` / `page_size` 必填；`shop_appid`、`is_from_head_supplier`、`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterGetCooperativeCouponListResponse` — 定向券列表（条目含 `from_head_supplier_appid`）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecooperativecouponlist.html
- 官方业务限制：①`page_size` 券数量，不能超过 `200`；②`shop_appid` 填了只返回属于该店铺的券； / ③`from_head_supplier_appid`（查询特定机构提供的券）**仅在 `is_from_head_supplier` 为 `true` 时有效**， / 来源机构的分发关系见 `SetCouponToHeadSupplierAsync`。
- 与 `GetPublicCouponListAsync` 的条目**分建**：定向券条目含 `from_head_supplier_appid`，公开券条目仅含 `coupon_id`（官方参数表逐字段不同）。
- 接口变更日志：2025 年 11 月 10 日新增 `is_from_head_supplier`、`from_head_supplier_appid` 请求字段，补充错误码。

### 9. `GetPublicCouponListAsync` — 获取公开券 ID 列表（`promoter/get_public_coupon_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_public_coupon_list`

- **请求体**：`ChannelsPromoterGetPublicCouponListRequest` — 查询请求（`next_key` / `page_size` 必填；`shop_appid` 选填）。
- **返回**：`ChannelsPromoterGetPublicCouponListResponse` — 公开券列表（条目仅 `coupon_id`）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotepublishcouponlist.html
- 官方业务限制：`page_size` 券数量，不能超过 `200`；`shop_appid` 填了只返回属于该店铺的券。
- 路由与文档名不对称照录：路由为 `get_public_coupon_list`，官方文档页名为 `api_getpromotepublishcouponlist`（「公开券」写作 publish）， / 二者指同一端点，本仓以官方请求路径为准。

### 10. `GetCouponDetailAsync` — 获取券 ID 对应的详情内容（`promoter/get_coupon_detail`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_coupon_detail`

- **请求体**：`ChannelsPromoterGetCouponDetailRequest` — 查询请求（`coupon_id` 必填；`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterGetCouponDetailResponse` — 券详情（`coupon`：类型 / 状态 / 优惠 / 领取 / 有效期 / 库存等子结构）及 `next_key` / `has_more`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecoupondetail.html
- 官方业务限制：①`from_head_supplier_appid` 为「来源的机构」，指该机构通过 `SetCouponToHeadSupplierAsync` / 给予本机构优惠券的情形；②券类型枚举见 `ChannelsPromoterCouponTypes`（`1`/`2`/`3`/`4`/`101`/`102`/`103`/`104`）、 / 状态枚举见 `ChannelsPromoterCouponStatuses`（`1`~`5`）、有效期类型见 `ChannelsPromoterCouponValidTypes`（`1` 固定时间 / `2` 领取后生效）。
- 官方参数表照录：详情端点的返回体仍带 `next_key` 与 `has_more` 两个翻页字段（本域唯一），按原样建模、不做裁剪。
- 官方参数表矛盾照录：`ext_info` 子结构的 `valid_time` 与 `invalid_time` 两行说明在官方表内互换，本仓按字段名原样映射。
- 与 Marketing 域 `ChannelsCoupon` 子树**分建**：本域券子树含 `shop_appid` 与机构分发相关字段，Marketing 域券子树含 `promote_info`，字段集不同。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071034`（获取优惠券失败）。
- 接口变更日志：2025 年 12 月 26 日新增 `is_can_up`、`can_up_times`、`discount_limit` 字段；2025 年 11 月 24 日新增错误码 `10071034`；2025 年 11 月 10 日新增 `from_head_supplier_appid` 字段、新增「参数错误」枚举值。

### 11. `GetCouponPromoterShareLinkAsync` — 获取某个券 ID 对应的小程序内嵌微信小店优惠券推广参数（`promoter/get_coupon_promoter_share_link`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_coupon_promoter_share_link`

- **请求体**：`ChannelsPromoterCouponPromotionRequest` — 查询请求（`coupon_id` 必填；`sharer_appid`、`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterCouponShareLinkResponse` — 小程序内嵌用的推广参数（`promoter_share_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponpromotersharelink.html
- 官方业务限制：`sharer_appid` 不传时生成「只带机构参数」的推广参数（即不归属具体推客，无法按推客结算佣金）。
- 请求 DTO `ChannelsPromoterCouponPromotionRequest` 与 `GetCouponQrCodeAsync`、 / `GetCouponShortLinkAsync` 共用（官方三支参数表逐字段一致）；响应各自分建，因为返回字段名不同 / （`promoter_share_link` / `qrcode_url` / `short_link`）。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071035`（优惠券非生效中）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071035`；2025 年 11 月 10 日新增 `from_head_supplier_appid` 字段。

### 12. `GetCouponQrCodeAsync` — 获取某个券 ID 对应的推广二维码（`promoter/get_coupon_qr_code`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_coupon_qr_code`

- **请求体**：`ChannelsPromoterCouponPromotionRequest` — 查询请求（`coupon_id` 必填；`sharer_appid`、`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterCouponQrCodeResponse` — 推广二维码地址（`qrcode_url`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponqrcode.html
- 官方业务限制：`sharer_appid` 不传时生成「只带机构参数」的券短链对应的二维码。
- 请求 DTO 与 `GetCouponPromoterShareLinkAsync`、`GetCouponShortLinkAsync` 共用，理由见该方法的 remarks。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071034`（获取优惠券失败）/ `10071035`（优惠券非生效中）。
- 接口变更日志：2026 年 01 月 28 日新增 `from_head_supplier_appid` 字段，补充错误码 `10071003`、`10071035`、`10071034`。

### 13. `GetCouponShortLinkAsync` — 获取某个券 ID 对应的推广短链（`promoter/get_coupon_short_link`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_coupon_short_link`

- **请求体**：`ChannelsPromoterCouponPromotionRequest` — 查询请求（`coupon_id` 必填；`sharer_appid`、`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterCouponShortLinkResponse` — 推广短链（`short_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponshortlink.html
- 官方业务限制：`sharer_appid` 不传时生成「只带机构参数」的券短链。
- 请求 DTO 与 `GetCouponPromoterShareLinkAsync`、`GetCouponQrCodeAsync` 共用，理由见该方法的 remarks。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071034`（获取优惠券失败）/ `10071035`（优惠券非生效中）。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071034`；2025 年 11 月 24 日新增错误码 `10071035`；2025 年 11 月 10 日新增 `from_head_supplier_appid` 字段。

### 14. `GetCouponToHeadSupplierAsync` — 获取分发给其他机构的优惠券（`promoter/get_coupon_to_headsupplier`，按接收机构维度分页查询分发记录）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_coupon_to_headsupplier`

- **请求体**：`ChannelsPromoterGetCouponToHeadSupplierRequest` — 查询请求（`to_head_supplier_appid` / `limit` / `page_context` 均必填）。
- **返回**：`ChannelsPromoterGetCouponToHeadSupplierResponse` — 分发记录列表、翻页上下文与符合条件的记录总数。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_get_coupon_to_headsupplier.html
- 官方业务限制：本端点是本域**唯一**使用 `limit` + `page_context` 翻页形态的列表（其余列表端点用 `next_key` + `page_size`）， / `page_context` 第一页放空、后续传上一页返回值；响应含 `total_count`。
- 与 `SetCouponToHeadSupplierAsync` 成对：前者查询分发记录，后者执行分发。
- 接口变更日志：2025 年 11 月 10 日新增「获取分发给其他机构的优惠券」接口。

### 15. `SetCouponToHeadSupplierAsync` — 分发优惠券给其他机构（`promoter/set_coupon_to_headsupplier`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/set_coupon_to_headsupplier`

- **请求体**：`ChannelsPromoterSetCouponToHeadSupplierRequest` — 分发请求（`to_head_supplier_appid` 与 `coupon_id` 均必填）。
- **返回**：`ChannelsResponse` — 官方仅返回 `errcode` / `errmsg`，故回 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_set_coupon_to_headsupplier.html
- 官方注意事项（原文照录，属不可逆业务约束）：①「只能店铺设置给当前机构的定向券才能分发给其他机构」， / 即 `coupon_id` 只能取自 `GetCooperativeCouponListAsync`（定向券列表），公开券不可分发； / ②「设置之后，目前无法取消，请自行评估」——本仓不提供撤销端点，因为官方无对应开放面。
- 接口变更日志：2025 年 11 月 10 日新增「分发优惠券给其他机构」接口。

### 16. `GetFeedListAsync` — 获取带货者推广的短视频信息（`promoter/get_feed_list`，带货者侧短视频列表）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_feed_list`

- **请求体**：`ChannelsPromoterGetFeedListRequest` — 查询请求（`talent_appid` / `next_key` / `page_size` 必填；`finder_exportusername` 选填）。
- **返回**：`ChannelsPromoterGetFeedListResponse` — 短视频列表（条目含商品信息与预计佣金）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getfeedlist.html
- 官方业务限制：①`page_size` 一页获取多少个数据，最大 `10`；②`finder_exportusername` / 「当合作方式是『全部内容合作』时，需要指定视频号 username，如果是之前的『部分内容』的合作方式，则可以不填」， / 获得方式为 `GetBindTalentListAsync` 返回的 `related_finder_exportname_list`； / ③官方注意事项原文：「已兼容新老内容合作方式——后台查询时，会先查询是否有『全部内容』的合作，如果没有才查询『部分内容』的合作」。
- 与 `GetShopFeedListAsync` 的条目 DTO **分建**：本店侧条目无 `talent_appid`；商品子结构 `product_info` 逐字段一致故共用 `ChannelsPromoterFeedProductInfo`。
- 官方错误码：`10024000`（参数错误）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071049`。

### 17. `GetFeedPromotionInfoAsync` — 生成某个带货者的某些短视频内嵌短视频卡片需要的 feedtoken（`promoter/get_feed_promotion_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_feed_promotion_info`

- **请求体**：`ChannelsPromoterGetFeedPromotionInfoRequest` — 生成请求（`feed_list` / `mini_program_appid` / `talent_appid` 必填；`sharer_appid` 选填）。
- **返回**：`ChannelsPromoterFeedPromotionInfoResponse` — 短视频 token 列表（`export_id` / `feed_token` / `promoter_share_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getfeedpromotioninfo.html
- 官方业务限制：①`mini_program_appid` 「需要挂在的小程序 appid【需要和机构绑定】」，未绑定时报 `10071046`； / ②`sharer_appid` 选填，「为了生成某个推客推广的 link」，不传则生成不含推客归属的推广参数； / ③请求条目 `feed_list[].export_id` 在本端点为必填。
- 响应 DTO `ChannelsPromoterFeedPromotionInfoResponse` 与 `GetShopFeedPromotionInfoAsync` 共用（官方返回参数表逐字段一致）。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071046`（小程序没有绑定机构）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071003`、`10071046`、`10071049`。

### 18. `GetShopFeedListAsync` — 获取小店关联账号的短视频列表（`promoter/get_shop_feed_list`，本店侧短视频列表）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_feed_list`

- **请求体**：`ChannelsPromoterGetShopFeedListRequest` — 查询请求（`next_key` / `page_size` / `shop_appid` / `promoter_id` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterGetShopFeedListResponse` — 短视频列表（条目无 `talent_appid`）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getshopfeedlist.html
- 官方业务限制：①`page_size` 最大 `10`；②定位三元组 `shop_appid` + `promoter_id` + `promoter_type` / 分别取自 `GetBindShopListAsync` 与 `GetBindShopPromoterListAsync`； / ③`promoter_type` 官方要求「需要是视频号」（值 `1`，见 `ChannelsPromoterShopPromoterTypes`）， / 传公众号/服务号会落 `10071055`（关联账号 id 错误）。
- 官方错误码：`10024000`（参数错误）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071055`（关联账号 id 错误）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071048`、`10071049`、`10071055`。

### 19. `GetShopFeedPromotionInfoAsync` — 生成小店关联账号的短视频内嵌小程序推广信息（`promoter/get_shop_feed_promotion_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_feed_promotion_info`

- **请求体**：`ChannelsPromoterGetShopFeedPromotionInfoRequest` — 生成请求（`shop_appid` / `promoter_id` / `mini_program_appid` / `feed_list` / `sharer_appid` 必填；`promoter_type` 官方标选填）。
- **返回**：`ChannelsPromoterFeedPromotionInfoResponse` — 短视频 token 列表（与 `GetFeedPromotionInfoAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getshopfeedpromotioninfo.html
- 官方业务限制：①`mini_program_appid` 需要和机构绑定，未绑定报 `10071046`； / ②`sharer_appid` 在本端点为**必填**（带货者侧 `GetFeedPromotionInfoAsync` 为选填）； / ③`promoter_type` 需要是视频号。
- 官方参数表不一致照录（两处，与本族其余端点冲突，本仓按参数表原样建模、不做运行时校验）： / ①`promoter_type` 在本端点标「否」，而同族 `get_shop_feed_list` 等均为「是」； / ②请求条目 `feed_list[].export_id` 在本端点标「否」，而 `get_feed_promotion_info` 标「是」， / 二者共用 `ChannelsPromoterFeedExportItem`，故该字段建模为可空字符串。
- 官方错误码：`10024000`（参数错误）/ `10071046`（小程序没有绑定机构）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071055`（关联账号 id 错误）。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071049`、`10071055`；2025 年 11 月 24 日新增错误码 `10071048`。

### 20. `GetLiveCommissionProductListAsync` — 获取某个带货者当前直播的带货商品列表（`promoter/get_live_commission_product_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_commission_product_list`

- **请求体**：`ChannelsPromoterGetLiveCommissionProductListRequest` — 查询请求（`talent_appid` / `next_key` / `page_size` 均必填）。
- **返回**：`ChannelsPromoterGetLiveCommissionProductListResponse` — 直播带货商品列表、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivecommissionproductlist.html
- 官方业务限制：`page_size` 一页获取多少个商品，最大 `10`；只返回「当前直播」的商品，须带货者正在开播。
- 与 `GetShopLiveCommissionProductListAsync` 的条目 DTO **分建**：本店侧条目多出 `commission_rate`（小店自营商品的分佣比例）。
- 官方错误码：`10024000`（参数错误）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071049`、`10071048`。

### 21. `GetShopLiveCommissionProductListAsync` — 获取小店关联账号的当前直播的自营商品列表（`promoter/get_shop_live_commission_product_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_commission_product_list`

- **请求体**：`ChannelsPromoterGetShopLiveCommissionProductListRequest` — 查询请求（`next_key` / `page_size` / `shop_appid` / `promoter_id` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterGetShopLiveCommissionProductListResponse` — 直播自营商品列表（条目含 `commission_rate`）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivecommissionproductlist.html
- 官方业务限制：①`page_size` 最大 `10`；②`promoter_type` 需要是视频号（`1`）；③定位三元组取自 `GetBindShopPromoterListAsync`。
- 官方错误码：`10024000`（参数错误）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071055`（关联账号 id 错误）。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071049`；2025 年 11 月 24 日新增错误码 `10071055`、`10071048` 等。

### 22. `GetLiveNoticePromoterShareLinkAsync` — 生成某个带货者的用于直播预约的推广参数（`promoter/get_live_notice_promoter_share_link`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_notice_promoter_share_link`

- **请求体**：`ChannelsPromoterGetLiveNoticePromoterShareLinkRequest` — 生成请求（`mini_program_appid` / `notice_id` / `talent_appid` / `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterLiveShareLinkResponse` — 直播预约推广参数（`promoter_share_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticepromotersharelink.html
- 官方注意事项（原文）：小程序侧配合 `jsapi` 使用—— / `wx.reserveChannelsLive({ noticeId, promoterShareLink })`，即本端点返回的 `promoter_share_link` 须与 `notice_id` 一并传给该 API 才能完成归因。
- 响应 DTO `ChannelsPromoterLiveShareLinkResponse` 与 `GetShopLiveNoticePromoterShareLinkAsync` 共用（官方返回参数表一致）。
- 官方错误码：`10024000`（参数错误）/ `10071030`（获取带货者账号错误，官方解决方案：检查 `talent_appid` 参数）/ `10071048`（参数异常，请检查入参）。

### 23. `GetShopLiveNoticePromoterShareLinkAsync` — 为推客获取小店关联账号直播预约的推广参数（`promoter/get_shop_live_notice_promoter_share_link`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_notice_promoter_share_link`

- **请求体**：`ChannelsPromoterGetShopLiveNoticePromoterShareLinkRequest` — 生成请求（`shop_appid` / `promoter_id` / `promoter_type` / `notice_id` / `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterLiveShareLinkResponse` — 直播预约推广参数（与 `GetLiveNoticePromoterShareLinkAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticepromotersharelink.html
- 官方业务限制：`promoter_type` 官方示例为 `1`（需要是视频号）；`notice_id` 取自 `GetShopLiveNoticeRecordListAsync`。

### 24. `GetLiveNoticeRecordListAsync` — 获取带货者的直播预约列表（`promoter/get_live_notice_record_list`，官方无分页参数）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_notice_record_list`

- **请求体**：`ChannelsPromoterGetLiveNoticeRecordListRequest` — 查询请求（`talent_appid` 必填）。
- **返回**：`ChannelsPromoterLiveNoticeRecordListResponse` — 直播预约数据（`notice_id` / `description` / `start_time`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticerecordlist.html
- 官方业务限制：本端点官方**只有** `talent_appid` 一个请求参数，**无分页**；返回的 `notice_id` 是 `GetLiveNoticePromoterShareLinkAsync`、`GetLiveNoticeRecordQrCodeAsync` 与 `GetLiveNoticeReservationInfoAsync` 的定位键（后者参数名为 `live_notice_id`）。
- 响应 DTO `ChannelsPromoterLiveNoticeRecordListResponse` 与 `GetShopLiveNoticeRecordListAsync` 共用（官方返回参数表逐字段一致）。
- 官方错误码：`10024000`（参数错误）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071049`。

### 25. `GetShopLiveNoticeRecordListAsync` — 获取小店关联账号的预约直播列表（`promoter/get_shop_live_notice_record_list`，官方无分页参数）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_notice_record_list`

- **请求体**：`ChannelsPromoterGetShopLiveNoticeRecordListRequest` — 查询请求（`shop_appid` / `promoter_id` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterLiveNoticeRecordListResponse` — 直播预约数据（与 `GetLiveNoticeRecordListAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticerecordlist.html
- 官方业务限制：`promoter_type` 需要是视频号（`1`）；本端点无分页参数。
- 官方错误码：`10024000`（参数错误）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071055`（关联账号 id 错误）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071055`、`10071049` 等。

### 26. `GetLiveNoticeRecordQrCodeAsync` — 为某个推客生成单个直播预约的推广二维码（`promoter/get_live_notice_record_qr_code`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_notice_record_qr_code`

- **请求体**：`ChannelsPromoterGetLiveNoticeRecordQrCodeRequest` — 生成请求（`talent_appid` / `notice_id` / `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterLiveQrCodeResponse` — 推广二维码地址（`qrcode_url`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticerecordqrcode.html
- 官方业务限制：本端点三支参数全部必填（与短视频推广参数端点的「`sharer_appid` 选填」不同）。
- 响应 DTO `ChannelsPromoterLiveQrCodeResponse` 为本域直播族四支二维码端点共用（官方返回参数表恒为 `qrcode_url` 单字段）。

### 27. `GetShopLiveNoticeRecordQrCodeAsync` — 为推客生成小店关联账号直播预约的推广二维码（`promoter/get_shop_live_notice_record_qr_code`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_notice_record_qr_code`

- **请求体**：`ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest` — 生成请求（`shop_appid` / `promoter_id` / `notice_id` / `sharer_appid` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterLiveQrCodeResponse` — 推广二维码地址（与 `GetLiveNoticeRecordQrCodeAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticerecordqrcode.html
- 官方业务限制：`promoter_type` 需要是视频号（`1`）。

### 28. `GetLiveNoticeReservationInfoAsync` — 获取直播预约的单个推客的推广预约人数（`promoter/get_live_notice_reservation_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_notice_reservation_info`

- **请求体**：`ChannelsPromoterGetLiveNoticeReservationInfoRequest` — 查询请求（`live_notice_id` 与 `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterGetLiveNoticeReservationInfoResponse` — 该推客带来的预约人数（`count`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticereservationinfo.html
- 官方参数表照录：预约 id 在本端点写作 `live_notice_id`，而兄弟端点 `get_live_notice_promoter_share_link` 与 / `get_live_notice_record_qr_code` 写作 `notice_id`，值同为 `GetLiveNoticeRecordListAsync` 返回的预约 id—— / 字段名不统一属官方现状，本仓逐端点照抄、不做别名收敛。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071030`（获取推客 talentuin 失败）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071030`、`10071003`。

### 29. `GetLiveRecordListAsync` — 获取带货者的直播列表（`promoter/get_live_record_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_record_list`

- **请求体**：`ChannelsPromoterGetLiveRecordListRequest` — 查询请求（`talent_appid` / `mini_program_appid` 必填；`sharer_appid` 选填）。
- **返回**：`ChannelsPromoterLiveRecordListResponse` — 直播数据（`export_id` / `description` / `promoter_share_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliverecordlist.html
- 官方业务限制：①`mini_program_appid` 「需要挂载的小程序 appid【要和机构绑定】」，未绑定报 `10071046`；②`sharer_appid` 选填，「为了生成某个推客推广的 link」；③本端点无分页参数。
- 响应 DTO `ChannelsPromoterLiveRecordListResponse` 与 `GetShopLiveRecordListAsync` 共用（官方返回参数表逐字段一致）。
- 官方错误码：`10024000`（参数错误）/ `10071046`（小程序没有绑定机构）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071046`、`10071047` 等。

### 30. `GetShopLiveRecordListAsync` — 获取小店关联账号的直播列表（`promoter/get_shop_live_record_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_record_list`

- **请求体**：`ChannelsPromoterGetShopLiveRecordListRequest` — 查询请求（`shop_appid` / `promoter_id` / `mini_program_appid` / `sharer_appid` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterLiveRecordListResponse` — 直播数据（与 `GetLiveRecordListAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshopliverecordlist.html
- 官方业务限制：`sharer_appid` 在本端点为**必填**（带货者侧 `GetLiveRecordListAsync` 为选填）；`promoter_type` 需要是视频号。
- 官方错误码：`10024000`（参数错误）/ `10071046`（小程序没有绑定机构）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071055`（关联账号 id 错误）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071046`、`10071047` 等。

### 31. `GetLiveRecordQrCodeAsync` — 为某个推客生成单个直播的推广二维码（`promoter/get_live_record_qr_code`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_live_record_qr_code`

- **请求体**：`ChannelsPromoterGetLiveRecordQrCodeRequest` — 生成请求（`talent_appid` / `export_id` / `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterLiveQrCodeResponse` — 推广二维码地址（与 `GetLiveNoticeRecordQrCodeAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliverecordqrcode.html
- 官方业务限制：`export_id` 为直播 id，取自 `GetLiveRecordListAsync` 的返回条目。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071047`（没有相关的合作内容）/ `10071048`（参数异常，请检查入参）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071003`、`10071047` 等。

### 32. `GetShopLiveRecordQrCodeAsync` — 为推客生成小店关联账号直播的推广二维码（`promoter/get_shop_live_record_qr_code`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_live_record_qr_code`

- **请求体**：`ChannelsPromoterGetShopLiveRecordQrCodeRequest` — 生成请求（`shop_appid` / `promoter_id` / `export_id` / `sharer_appid` / `promoter_type` 均必填）。
- **返回**：`ChannelsPromoterLiveQrCodeResponse` — 推广二维码地址（与 `GetLiveRecordQrCodeAsync` 共用响应类型）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshopliverecordqrcode.html
- 官方业务限制：`export_id` 取自 `GetShopLiveRecordListAsync`；`promoter_type` 需要是视频号。
- 官方错误码：`10024000`（参数错误）/ `10071047`（没有相关的合作内容）。
- 接口变更日志：2025 年 11 月 24 日新增错误码 `10071047`。

### 33. `GetShopMpArticleListAsync` — 获取小店关联账号的文章列表（`promoter/get_shop_mp_article_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_shop_mp_article_list`

- **请求体**：`ChannelsPromoterGetShopMpArticleListRequest` — 查询请求（`next_key` / `page_size` / `shop_appid` / `promoter_id` / `promoter_type` / `sharer_appid` 均必填）。
- **返回**：`ChannelsPromoterGetShopMpArticleListResponse` — 文章列表（含文章基础信息、商品列表与创建/更新时间）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/article/api_getshopmparticlelist.html
- 官方业务限制：①`page_size` 最大 `10`；②`promoter_type` 官方要求「需要是公众号或服务号」 / （`4` / `5`，见 `ChannelsPromoterShopPromoterTypes`）——与短视频、直播两族的「需要是视频号」相反， / 传 `1` 会落 `10071055` 类错误；③文章基础信息子结构 `article_info`（标题 / 封面 / 链接 / 推广参数）与带货者侧共用 / `ChannelsPromoterArticleInfo`。
- 与 `GetTalentBizArticleListAsync` 的条目与商品子结构**分建**：本店侧条目商品子结构用 `commission_rate`、数组名为 `product_list`；带货者侧用 `product_mini_price`、数组名为 `product_info_list`。
- 官方错误码：`10024000`（参数错误）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 12 月 23 日新增错误码 `10071049`；2025 年 11 月 24 日新增错误码 `10071048`。

### 34. `GetTalentBizArticleListAsync` — 获取某个带货者可推广文章列表（`promoter/get_talent_biz_article_list`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_talent_biz_article_list`

- **请求体**：`ChannelsPromoterGetTalentBizArticleListRequest` — 查询请求（`talent_appid` / `next_key` / `page_size` / `sharer_appid` 必填；`biz_appid` 选填）。
- **返回**：`ChannelsPromoterGetTalentBizArticleListResponse` — 文章列表（条目商品子结构数组名为 `product_info_list`）、下一页参数与翻页标志。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/article/api_gettalentbizarticlelist.html
- 官方业务限制：①`page_size` 最大 `10`；②`biz_appid`「当合作方式是『全部内容合作』时，需要指定公众号 appid。 / 如果是之前的『部分内容』的合作方式，则可以不填」，获得方式为 `GetBindTalentListAsync` 的 `related_mp_biz_appid_list`； / ③官方注意事项原文：「已兼容新老内容合作方式——后台查询时，会先查询是否有『全部内容』的合作，如果没有才查询『部分内容』的合作」。
- 官方错误码：`10024000`（参数错误）/ `10071049`（推客功能已关闭）。
- 接口变更日志：2025 年 12 月 23 日新增错误码 `10071049`。

### 35. `GetProductPromotionLinkInfoAsync` — 获取推客对某个商品的推广短链（`promoter/get_product_promotion_link_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_product_promotion_link_info`

- **请求体**：`ChannelsPromoterProductPromotionRequest` — 查询请求（`sharer_appid` 与 `sharer_openid` 二选一；`product_id` + `shop_appid` 与 `product_short_link` 二选一；`head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterProductPromotionLinkInfoResponse` — 推广短链（`short_link` / `http_short_link`）及命中商品的 `product_id` 与 `shop_appid`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getproductpromotionlinkinfo.html
- 官方业务限制：①`sharer_openid`「必须是机构绑定小程序的用户 openid，而不能是其他小程序的用户 openid」， / 否则报 `10071056`；②使用 `product_id` 时必须同时传入 `shop_appid`；③`product_short_link` 与 `product_id` 二选一。
- 请求 DTO `ChannelsPromoterProductPromotionRequest` 与 `GetProductPromotionQrCodeInfoAsync` 共用 / （官方两支参数表逐字段一致）；与 `GetPromoterSingleProductPromotionInfoAsync` **分建**——后者多一个 `custom_token` 字段， / 本端点参数表**无** `custom_token`，即推广短链/二维码不支持携带用户自定义关联参数，只有商品卡内嵌链接支持。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071049`；2025 年 12 月 22 日返回参数新增商品 id 和店铺 id；2025 年 11 月 24 日新增错误码 `10071053`、`10071030` 等。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071029`（推客和机构未绑定）/ `10071030`（获取推客 talentuin 失败）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071053`（商品不在架）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。

### 36. `GetProductPromotionQrCodeInfoAsync` — 获取推客对某个商品的推广二维码（`promoter/get_product_promotion_qrcode_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_product_promotion_qrcode_info`

- **请求体**：`ChannelsPromoterProductPromotionRequest` — 查询请求（字段与二选一约束同 `GetProductPromotionLinkInfoAsync`）。
- **返回**：`ChannelsPromoterProductPromotionQrCodeResponse` — 推广二维码地址（`qrcode_url`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getproductpromotionqrcodeinfo.html
- 官方业务限制：请求参数表与 `GetProductPromotionLinkInfoAsync` 逐字段一致，故共用 `ChannelsPromoterProductPromotionRequest`；本端点参数表同样**无** `custom_token`。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071029`（推客和机构未绑定）/ `10071030`（获取推客 talentuin 失败）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071053`（商品不在架）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。
- 接口变更日志：2025 年 12 月 22 日新增错误码 `10071048`、`10071049`；2025 年 11 月 24 日新增错误码 `10071053`、`10071048` 等。

### 37. `GetPromoterSingleProductPromotionInfoAsync` — 获取某个推客某个商品的内嵌商品卡片 product_promotion_link（`promoter/get_promoter_single_product_promotion_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_promoter_single_product_promotion_info`

- **请求体**：`ChannelsPromoterGetSingleProductPromotionInfoRequest` — 查询请求（`sharer_appid`/`sharer_openid` 二选一、`product_id`+`shop_appid` 与 `product_short_link` 二选一，另可带 `custom_token`）。
- **返回**：`ChannelsPromoterGetSingleProductPromotionInfoResponse` — 内嵌商品卡片用的推广链接（`product_promotion_link`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromotersingleproductpromotioninfo.html
- 官方业务限制（`custom_token` 原文照录）：「长度要求 <=128，只有商品卡的分享直接成交的才生效，间接锁客的不生效， / 当前不支持二维码和短链；这个关联参数直接成交时，会在订单详情中返回」——即归因参数只在商品卡场景与直接成交链路可见。
- 请求 DTO 与 `GetProductPromotionLinkInfoAsync` / `GetProductPromotionQrCodeInfoAsync` **分建**，唯一差异即 `custom_token`。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071029`（推客和机构未绑定）/ `10071030`（获取推客 talentuin 失败）/ `10071048`（参数异常，请检查入参）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。
- 接口变更日志：2026 年 03 月 12 日新增 `custom_token` 字段，用户自定义关联参数；2025 年 11 月 24 日新增错误码 `10071003`、`10071029` 等。

### 38. `GetPromoteProductDetailAsync` — 获取可推广的商品详情（`promoter/get_promote_product_detail`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_promote_product_detail`

- **请求体**：`ChannelsPromoterGetPromoteProductDetailRequest` — 查询请求（`shop_appid` / `plan_type` / `product_id` 必填；`get_available_coupon`、`head_supplier_appid`、`custom_token` 选填）。
- **返回**：`ChannelsPromoterGetPromoteProductDetailResponse` — 商品详情（`product`：标题 / 头图 / 描述 / 类目 / SKU / 推广链接 / 状态 / 佣金与供货机构信息）及可用机构券（公开券、定向券两个数组）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromoteproductdetail.html
- 官方业务限制：①`plan_type` 枚举见 `ChannelsPromoterPlanTypes` / （`1` 定向计划 / `2` 公开计划 / `3` 机构定向计划 / `4` 机构普通计划）， / 且「当 `plan_type` 为 `3` 和 `4` 时，`head_supplier_appid` 这个参数必传」； / ②`get_available_coupon` 填 `true` 才返回商品可用的机构券； / ③`custom_token` 约束同 `GetPromoterSingleProductPromotionInfoAsync`（长度 <=128，仅商品卡直接成交生效）； / ④保障标枚举见 `ChannelsPromoterProductFlags`，商品状态枚举见 `ChannelsPromoterProductStatuses`， / 推广状态枚举见 `ChannelsPromoterProductPromotionStatuses`。
- 官方文档矛盾与裁剪照录：①`commission_info.status` 参数表类型为 string、但其枚举与代码示例均为数字，本仓取 `int?`； / ②`UserInclusivePrice`（用户视角普惠价）为 2026-09-16 变更新增； / ③官方代码示例额外出现 `express_info`、`extra_service`、`statistical_data`、`normal_commission_info` / 四个子树，但官方**参数表未定义其字段**，本仓不建模（避免凭示例臆造契约面），需要时读原始 JSON。
- 官方错误码：`10024000`（参数错误，请开发人员检查传递参数是否正确）/ `10024003`（不合法的 appid）/ `10024004`（不存在该商品）/ `10071003`（参数错误）/ `10071008`（该商品不存在）/ `10071033`（shop_appid 错误）/ `10071044`（推客功能保证金不足）/ `10071048`（参数异常，请检查入参）/ `10071049`（推客功能已关闭）/ `10071053`（商品不在架）/ `10071056`（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。
- 接口变更日志：2026 年 09 月 16 日增加用户视角普惠价；2026 年 03 月 12 日新增 `custom_token` 字段；2025 年 11 月 24 日新增错误码 `10071008`、`10071033`、`10071056`、`10071049` 等。

### 39. `GetPromoteProductListAsync` — 获取可推广的商品 id 列表（`promoter/get_promote_product_list`，支持多维筛选）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_promote_product_list`

- **请求体**：`ChannelsPromoterGetPromoteProductListRequest` — 查询请求（`plan_type` / `page_size` / `next_key` 必填；`shop_appid`、`spu_item_condition`、`category`、`keyword`、`from_head_supplier_appid` 选填）。
- **返回**：`ChannelsPromoterGetPromoteProductListResponse` — 商品列表条目（`product_id` / `shop_appid` / `head_supplier_appid`）与下一页参数。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromoteproductlist.html
- 官方业务限制：①`page_size` 一页获取多少个商品，最大 `20`（本域唯一的 20 上限商品列表）； / ②`plan_type` 见 `ChannelsPromoterPlanTypes`；③`category`「商品类目查询条件（**定向计划不支持**）」； / ④`from_head_supplier_appid`「来源机构筛选，当商品是机构公开或定向时有效」； / ⑤`ChannelsPromoterSpuItemCondition` 各区间单位由官方逐字段标注：售卖价与 `selling_price_range` 为「分」， / 服务费率与佣金率区间为「10 万分之」，保障标 `flags` 见 `ChannelsPromoterProductFlags`（其中 `2` 品牌官方标注已废弃）。
- 区间子结构 `ChannelsPromoterPromotionRange`（`min`/`max`）由 `spu_item_condition` 内八个区间字段共用——官方八张子表逐字段同名同型。
- 官方参数表矛盾照录：①列表条目 `product_id` 为 string，而详情端点 `get_promote_product_detail` 的 `product_id` 为 number； / ②`category.category_id` 参数表为 number，但官方示例值 `11099070301657575487` 超出 `long` 范围，本仓取 `string?`； / ③`category_ids_1/2/3` 官方类型列写作「-」且说明与示例在「单值 number / 数组」间不一致，本仓按示例取 `List<string>?`。
- 官方错误码：`10024000`（参数错误）/ `10071003`（参数错误）/ `10071033`（shop_appid 错误）/ `10071044`（推客功能保证金不足）/ `10071048`（参数异常，请检查入参）/ `10071053`（商品不在架）。
- 接口变更日志：2026 年 03 月 09 日新增 `from_head_supplier_appid` 字段。

### 40. `GetSharerProductCommissionInfoAsync` — 获取推客的某个商品的推广分佣比例（`promoter/get_sharer_product_commission_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/get_sharer_product_commission_info`

- **请求体**：`ChannelsPromoterGetSharerProductCommissionInfoRequest` — 查询请求（`sharer_appid` 与 `product_id` 均必填）。
- **返回**：`ChannelsPromoterGetSharerProductCommissionInfoResponse` — 分佣比例（`commission_ratio`）与是否已设置该商品佣金率的标志（`is_set`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getsharerproductcommissioninfo.html
- 官方参数表矛盾照录（两处，本仓均按参数表建模、不做区间校验）： / ①`commission_ratio` 参数表说明为「范围为【100000 - 900000】，代表【10%-90%】」，而其代码示例返回 `10`，两者不可同时成立； / ②本端点 `commission_ratio` 为 number，写入端点 `SetSharerProductCommissionInfoAsync` 的同类字段为 string。
- 官方 `product_id` 说明写作「要设置的商品 id」（读取端点沿用写入端点的措辞），按原样照录。

### 41. `SetSharerProductCommissionInfoAsync` — 设置推客的单个商品的分佣比例信息（`promoter/set_sharer_product_commission_info`）。

- **声明接口**：[IChannelsPromoterService](./IChannelsPromoterService.cs)
- **官方契约**：`POST /channels/ec/promoter/set_sharer_product_commission_info`

- **请求体**：`ChannelsPromoterSetSharerProductCommissionInfoRequest` — 设置请求（`sharer_appid` / `commission_ratio` / `product_id` 均必填）。
- **返回**：`ChannelsResponse` — 官方仅返回 `errcode` / `errmsg`，故回 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_setsharerproductcommissioninfo.html
- 官方业务限制：`commission_ratio`「平台分佣时的分佣比例，范围为【100000 - 1000000】，代表【10%-100%】」， / 官方参数表类型为 **string**（读取端点 `GetSharerProductCommissionInfoAsync` 为 number，且其区间上界写作 900000）， / 本仓按各自参数表原样建模并在 DTO remarks 照录。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Promoter`](../../DataModels/Promoter) 内共 **111** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **111** 个，`SerializerClassName` = `Promoter`。

AOT 源生成上下文：[`PromoterJsonContext.g.cs`](./../../DataModels/Generated/PromoterJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsPromoterGetBindSharerListResponse` | 应答 | [Promoter/Account/ChannelsPromoterGetBindSharerListResponse.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindSharerListResponse.cs) | `sharer_info_list`, `next_key` |
| `ChannelsPromoterGetBindSharerListRequest` | 请求体 | [Promoter/Account/ChannelsPromoterGetBindSharerListRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindSharerListRequest.cs) | `next_key`, `page_size`, `sharer_appid`, `sharer_openid` |
| `ChannelsPromoterGetBindShopListResponse` | 应答 | [Promoter/Account/ChannelsPromoterGetBindShopListResponse.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindShopListResponse.cs) | `next_key`, `shop_list`, `has_more` |
| `ChannelsPromoterGetBindShopListRequest` | 请求体 | [Promoter/Account/ChannelsPromoterGetBindShopListRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindShopListRequest.cs) | `next_key`, `page_size` |
| `ChannelsPromoterGetBindShopPromoterListResponse` | 应答 | [Promoter/Account/ChannelsPromoterGetBindShopPromoterListResponse.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindShopPromoterListResponse.cs) | `next_key`, `promoter_list`, `has_more` |
| `ChannelsPromoterGetBindShopPromoterListRequest` | 请求体 | [Promoter/Account/ChannelsPromoterGetBindShopPromoterListRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindShopPromoterListRequest.cs) | `next_key`, `page_size`, `shop_appid` |
| `ChannelsPromoterGetBindTalentListResponse` | 应答 | [Promoter/Account/ChannelsPromoterGetBindTalentListResponse.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindTalentListResponse.cs) | `next_key`, `talent_list`, `has_more` |
| `ChannelsPromoterGetBindTalentListRequest` | 请求体 | [Promoter/Account/ChannelsPromoterGetBindTalentListRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetBindTalentListRequest.cs) | `next_key`, `page_size`, `talent_appid` |
| `ChannelsPromoterGetRegisterAndBindStatusResponse` | 应答 | [Promoter/Account/ChannelsPromoterGetRegisterAndBindStatusResponse.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetRegisterAndBindStatusResponse.cs) | `bind_status`, `register_status`, `register_business_type`, `register_query_string`, `bind_business_type`, `bind_query_string`, `sharer_appid` |
| `ChannelsPromoterGetRegisterAndBindStatusRequest` | 请求体 | [Promoter/Account/ChannelsPromoterGetRegisterAndBindStatusRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterGetRegisterAndBindStatusRequest.cs) | `sharer_openid`, `sharer_appid`, `is_simple_register` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsPromoterUnbindPromoterRequest` | 请求体 | [Promoter/Account/ChannelsPromoterUnbindPromoterRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterUnbindPromoterRequest.cs) | `sharer_appid` |
| `ChannelsPromoterSetSharerCommissionInfoRequest` | 请求体 | [Promoter/Account/ChannelsPromoterSetSharerCommissionInfoRequest.cs](../../DataModels/Promoter/Account/ChannelsPromoterSetSharerCommissionInfoRequest.cs) | `sharer_appid`, `commission_ratio`, `commission_type` |
| `ChannelsPromoterGetCooperativeCouponListResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterGetCooperativeCouponListResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCooperativeCouponListResponse.cs) | `next_key`, `coupons`, `has_more` |
| `ChannelsPromoterGetCooperativeCouponListRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterGetCooperativeCouponListRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCooperativeCouponListRequest.cs) | `shop_appid`, `next_key`, `page_size`, `is_from_head_supplier`, `from_head_supplier_appid` |
| `ChannelsPromoterGetPublicCouponListResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterGetPublicCouponListResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetPublicCouponListResponse.cs) | `next_key`, `coupons`, `has_more` |
| `ChannelsPromoterGetPublicCouponListRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterGetPublicCouponListRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetPublicCouponListRequest.cs) | `shop_appid`, `next_key`, `page_size` |
| `ChannelsPromoterGetCouponDetailResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterGetCouponDetailResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCouponDetailResponse.cs) | `coupon`, `next_key`, `has_more` |
| `ChannelsPromoterGetCouponDetailRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterGetCouponDetailRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCouponDetailRequest.cs) | `coupon_id`, `from_head_supplier_appid` |
| `ChannelsPromoterCouponShareLinkResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterCouponShareLinkResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterCouponShareLinkResponse.cs) | `promoter_share_link` |
| `ChannelsPromoterCouponPromotionRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterCouponPromotionRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterCouponPromotionRequest.cs) | `coupon_id`, `sharer_appid`, `from_head_supplier_appid` |
| `ChannelsPromoterCouponQrCodeResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterCouponQrCodeResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterCouponQrCodeResponse.cs) | `qrcode_url` |
| `ChannelsPromoterCouponShortLinkResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterCouponShortLinkResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterCouponShortLinkResponse.cs) | `short_link` |
| `ChannelsPromoterGetCouponToHeadSupplierResponse` | 应答 | [Promoter/Coupon/ChannelsPromoterGetCouponToHeadSupplierResponse.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCouponToHeadSupplierResponse.cs) | `list`, `page_context`, `total_count` |
| `ChannelsPromoterGetCouponToHeadSupplierRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterGetCouponToHeadSupplierRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterGetCouponToHeadSupplierRequest.cs) | `to_head_supplier_appid`, `limit`, `page_context` |
| `ChannelsPromoterSetCouponToHeadSupplierRequest` | 请求体 | [Promoter/Coupon/ChannelsPromoterSetCouponToHeadSupplierRequest.cs](../../DataModels/Promoter/Coupon/ChannelsPromoterSetCouponToHeadSupplierRequest.cs) | `to_head_supplier_appid`, `coupon_id` |
| `ChannelsPromoterGetFeedListResponse` | 应答 | [Promoter/Video/ChannelsPromoterGetFeedListResponse.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetFeedListResponse.cs) | `next_key`, `has_more`, `feed_list` |
| `ChannelsPromoterGetFeedListRequest` | 请求体 | [Promoter/Video/ChannelsPromoterGetFeedListRequest.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetFeedListRequest.cs) | `talent_appid`, `finder_exportusername`, `next_key`, `page_size` |
| `ChannelsPromoterFeedPromotionInfoResponse` | 应答 | [Promoter/Video/ChannelsPromoterFeedPromotionInfoResponse.cs](../../DataModels/Promoter/Video/ChannelsPromoterFeedPromotionInfoResponse.cs) | `feed_list` |
| `ChannelsPromoterGetFeedPromotionInfoRequest` | 请求体 | [Promoter/Video/ChannelsPromoterGetFeedPromotionInfoRequest.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetFeedPromotionInfoRequest.cs) | `feed_list`, `mini_program_appid`, `talent_appid`, `sharer_appid` |
| `ChannelsPromoterGetShopFeedListResponse` | 应答 | [Promoter/Video/ChannelsPromoterGetShopFeedListResponse.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetShopFeedListResponse.cs) | `next_key`, `has_more`, `feed_list` |
| `ChannelsPromoterGetShopFeedListRequest` | 请求体 | [Promoter/Video/ChannelsPromoterGetShopFeedListRequest.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetShopFeedListRequest.cs) | `next_key`, `page_size`, `shop_appid`, `promoter_id`, `promoter_type` |
| `ChannelsPromoterGetShopFeedPromotionInfoRequest` | 请求体 | [Promoter/Video/ChannelsPromoterGetShopFeedPromotionInfoRequest.cs](../../DataModels/Promoter/Video/ChannelsPromoterGetShopFeedPromotionInfoRequest.cs) | `shop_appid`, `promoter_id`, `promoter_type`, `mini_program_appid`, `feed_list`, `sharer_appid` |
| `ChannelsPromoterGetLiveCommissionProductListResponse` | 应答 | [Promoter/Live/ChannelsPromoterGetLiveCommissionProductListResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveCommissionProductListResponse.cs) | `next_key`, `has_more`, `product_list` |
| `ChannelsPromoterGetLiveCommissionProductListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveCommissionProductListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveCommissionProductListRequest.cs) | `talent_appid`, `next_key`, `page_size` |
| `ChannelsPromoterGetShopLiveCommissionProductListResponse` | 应答 | [Promoter/Live/ChannelsPromoterGetShopLiveCommissionProductListResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveCommissionProductListResponse.cs) | `next_key`, `product_list`, `has_more` |
| `ChannelsPromoterGetShopLiveCommissionProductListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveCommissionProductListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveCommissionProductListRequest.cs) | `next_key`, `page_size`, `shop_appid`, `promoter_id`, `promoter_type` |
| `ChannelsPromoterLiveShareLinkResponse` | 应答 | [Promoter/Live/ChannelsPromoterLiveShareLinkResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterLiveShareLinkResponse.cs) | `promoter_share_link` |
| `ChannelsPromoterGetLiveNoticePromoterShareLinkRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveNoticePromoterShareLinkRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveNoticePromoterShareLinkRequest.cs) | `mini_program_appid`, `notice_id`, `talent_appid`, `sharer_appid` |
| `ChannelsPromoterGetShopLiveNoticePromoterShareLinkRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveNoticePromoterShareLinkRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveNoticePromoterShareLinkRequest.cs) | `shop_appid`, `promoter_id`, `promoter_type`, `notice_id`, `sharer_appid` |
| `ChannelsPromoterLiveNoticeRecordListResponse` | 应答 | [Promoter/Live/ChannelsPromoterLiveNoticeRecordListResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterLiveNoticeRecordListResponse.cs) | `live_notice_record_list` |
| `ChannelsPromoterGetLiveNoticeRecordListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveNoticeRecordListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveNoticeRecordListRequest.cs) | `talent_appid` |
| `ChannelsPromoterGetShopLiveNoticeRecordListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveNoticeRecordListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveNoticeRecordListRequest.cs) | `shop_appid`, `promoter_id`, `promoter_type` |
| `ChannelsPromoterLiveQrCodeResponse` | 应答 | [Promoter/Live/ChannelsPromoterLiveQrCodeResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterLiveQrCodeResponse.cs) | `qrcode_url` |
| `ChannelsPromoterGetLiveNoticeRecordQrCodeRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveNoticeRecordQrCodeRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveNoticeRecordQrCodeRequest.cs) | `talent_appid`, `notice_id`, `sharer_appid` |
| `ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest.cs) | `shop_appid`, `promoter_id`, `notice_id`, `sharer_appid`, `promoter_type` |
| `ChannelsPromoterGetLiveNoticeReservationInfoResponse` | 应答 | [Promoter/Live/ChannelsPromoterGetLiveNoticeReservationInfoResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveNoticeReservationInfoResponse.cs) | `count` |
| `ChannelsPromoterGetLiveNoticeReservationInfoRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveNoticeReservationInfoRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveNoticeReservationInfoRequest.cs) | `live_notice_id`, `sharer_appid` |
| `ChannelsPromoterLiveRecordListResponse` | 应答 | [Promoter/Live/ChannelsPromoterLiveRecordListResponse.cs](../../DataModels/Promoter/Live/ChannelsPromoterLiveRecordListResponse.cs) | `live_record_list` |
| `ChannelsPromoterGetLiveRecordListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveRecordListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveRecordListRequest.cs) | `talent_appid`, `mini_program_appid`, `sharer_appid` |
| `ChannelsPromoterGetShopLiveRecordListRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveRecordListRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveRecordListRequest.cs) | `shop_appid`, `promoter_id`, `mini_program_appid`, `sharer_appid`, `promoter_type` |
| `ChannelsPromoterGetLiveRecordQrCodeRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetLiveRecordQrCodeRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetLiveRecordQrCodeRequest.cs) | `talent_appid`, `export_id`, `sharer_appid` |
| `ChannelsPromoterGetShopLiveRecordQrCodeRequest` | 请求体 | [Promoter/Live/ChannelsPromoterGetShopLiveRecordQrCodeRequest.cs](../../DataModels/Promoter/Live/ChannelsPromoterGetShopLiveRecordQrCodeRequest.cs) | `shop_appid`, `promoter_id`, `export_id`, `sharer_appid`, `promoter_type` |
| `ChannelsPromoterGetShopMpArticleListResponse` | 应答 | [Promoter/Article/ChannelsPromoterGetShopMpArticleListResponse.cs](../../DataModels/Promoter/Article/ChannelsPromoterGetShopMpArticleListResponse.cs) | `next_key`, `has_more`, `article_list` |
| `ChannelsPromoterGetShopMpArticleListRequest` | 请求体 | [Promoter/Article/ChannelsPromoterGetShopMpArticleListRequest.cs](../../DataModels/Promoter/Article/ChannelsPromoterGetShopMpArticleListRequest.cs) | `next_key`, `page_size`, `shop_appid`, `promoter_id`, `promoter_type`, `sharer_appid` |
| `ChannelsPromoterGetTalentBizArticleListResponse` | 应答 | [Promoter/Article/ChannelsPromoterGetTalentBizArticleListResponse.cs](../../DataModels/Promoter/Article/ChannelsPromoterGetTalentBizArticleListResponse.cs) | `next_key`, `has_more`, `article_list` |
| `ChannelsPromoterGetTalentBizArticleListRequest` | 请求体 | [Promoter/Article/ChannelsPromoterGetTalentBizArticleListRequest.cs](../../DataModels/Promoter/Article/ChannelsPromoterGetTalentBizArticleListRequest.cs) | `talent_appid`, `next_key`, `page_size`, `sharer_appid`, `biz_appid` |
| `ChannelsPromoterProductPromotionLinkInfoResponse` | 应答 | [Promoter/Product/ChannelsPromoterProductPromotionLinkInfoResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterProductPromotionLinkInfoResponse.cs) | `short_link`, `http_short_link`, `product_id`, `shop_appid` |
| `ChannelsPromoterProductPromotionRequest` | 请求体 | [Promoter/Product/ChannelsPromoterProductPromotionRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterProductPromotionRequest.cs) | `sharer_appid`, `sharer_openid`, `product_id`, `shop_appid`, `product_short_link`, `head_supplier_appid` |
| `ChannelsPromoterProductPromotionQrCodeResponse` | 应答 | [Promoter/Product/ChannelsPromoterProductPromotionQrCodeResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterProductPromotionQrCodeResponse.cs) | `qrcode_url` |
| `ChannelsPromoterGetSingleProductPromotionInfoResponse` | 应答 | [Promoter/Product/ChannelsPromoterGetSingleProductPromotionInfoResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetSingleProductPromotionInfoResponse.cs) | `product_promotion_link` |
| `ChannelsPromoterGetSingleProductPromotionInfoRequest` | 请求体 | [Promoter/Product/ChannelsPromoterGetSingleProductPromotionInfoRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetSingleProductPromotionInfoRequest.cs) | `sharer_appid`, `sharer_openid`, `product_id`, `shop_appid`, `product_short_link`, `head_supplier_appid`, `custom_token` |
| `ChannelsPromoterGetPromoteProductDetailResponse` | 应答 | [Promoter/Product/ChannelsPromoterGetPromoteProductDetailResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetPromoteProductDetailResponse.cs) | `product`, `publish_coupons`, `cooperative_coupons` |
| `ChannelsPromoterGetPromoteProductDetailRequest` | 请求体 | [Promoter/Product/ChannelsPromoterGetPromoteProductDetailRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetPromoteProductDetailRequest.cs) | `shop_appid`, `plan_type`, `product_id`, `get_available_coupon`, `head_supplier_appid`, `custom_token` |
| `ChannelsPromoterGetPromoteProductListResponse` | 应答 | [Promoter/Product/ChannelsPromoterGetPromoteProductListResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetPromoteProductListResponse.cs) | `product_list`, `next_key` |
| `ChannelsPromoterGetPromoteProductListRequest` | 请求体 | [Promoter/Product/ChannelsPromoterGetPromoteProductListRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetPromoteProductListRequest.cs) | `shop_appid`, `plan_type`, `spu_item_condition`, `category`, `keyword`, `page_size`, `next_key`, `from_head_supplier_appid` |
| `ChannelsPromoterGetSharerProductCommissionInfoResponse` | 应答 | [Promoter/Product/ChannelsPromoterGetSharerProductCommissionInfoResponse.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetSharerProductCommissionInfoResponse.cs) | `commission_ratio`, `is_set` |
| `ChannelsPromoterGetSharerProductCommissionInfoRequest` | 请求体 | [Promoter/Product/ChannelsPromoterGetSharerProductCommissionInfoRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterGetSharerProductCommissionInfoRequest.cs) | `sharer_appid`, `product_id` |
| `ChannelsPromoterSetSharerProductCommissionInfoRequest` | 请求体 | [Promoter/Product/ChannelsPromoterSetSharerProductCommissionInfoRequest.cs](../../DataModels/Promoter/Product/ChannelsPromoterSetSharerProductCommissionInfoRequest.cs) | `sharer_appid`, `commission_ratio`, `product_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Promoter;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddPromoterApi());

var service = app.Services.GetRequiredService<IChannelsPromoterService>();
var ct = CancellationToken.None;
var response = await service.GetBindSharerListAsync(new ChannelsPromoterGetBindSharerListRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/promoter/get_bind_sharer_list");
// 占位值仅用于编译示意；业务值须按 IChannelsPromoterService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetBindSharerListAsync`）；请求体 `ChannelsPromoterGetBindSharerListRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsPromoterService](./IChannelsPromoterService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsPromoterContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPromoterContractGuards.cs) |
| 守卫中的代表性测试方法 | `PromoterEndpoints_ShouldMatchOfficialRoutes`、`PromoterEndpoints_ShouldAllBePost_WithSingleBody_AndResponseReuse`、`PromoterInterface_ShouldBindChannelsToken_AndRegisterUnderPromoterGroup`、`PromoterDataModels_ShouldBeRegisteredInJsonContext`、`PromoterDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`PromoterEnumConstants_ShouldLockOfficialLiteralValues`、`PromoterDataModels_ShouldLockDocContradictionsAndNotModelExampleOnlySubtrees` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Promoter`](../../DataModels/Promoter) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsPromoterContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

# League 域 · 优选联盟

微信小店 / 视频号（channels 生态）「优选联盟」域 SDK（41 端点：商家侧带货者 4 + 商品操作 7 + 联盟带货机构资金 3 + 机构商品 6 + 机构商家合作 3 + 机构佣金单 2 + 机构橱窗与授权 6 + 机构商品订阅 3 + 机构达人佣金计划 2 + 机构帮卖供货 2 + 机构线索订单 2 + 机构直播保护 1）。 接口：[IChannelsLeagueService](./IChannelsLeagueService.cs)；注册组 `League`；模块枚举 `ChannelsModule.League`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsLeagueContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLeagueContractGuards.cs)、[ChannelsPromoterContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPromoterContractGuards.cs)、[ChannelsTalentContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/League`](../../../Mud.Wechat.Channels/League/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsLeagueService = 41；按 HTTP 特性实计 **41** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsLeagueService` | `GetPromoterAsync` | POST | `/channels/ec/league/promoter/get` | `AccessToken`（Query），Query `access_token` | 获取带货者详情信息（`promoter/get`）。 |
| 2 | `IChannelsLeagueService` | `GetPromoterListAsync` | POST | `/channels/ec/league/promoter/list/get` | `AccessToken`（Query），Query `access_token` | 获取带货者列表（`promoter/list/get`，商家带货者列表分页查询）。 |
| 3 | `IChannelsLeagueService` | `GetTalentProductDataListAsync` | POST | `/channels/ec/league/talent/getproductdatalist` | `AccessToken`（Query），Query `access_token` | 获取带货者商品数据列表（`talent/getproductdatalist`，合作管理列表）。 |
| 4 | `IChannelsLeagueService` | `UpdateTalentAsync` | POST | `/channels/ec/league/talent/update` | `AccessToken`（Query），Query `access_token` | 编辑带货者（`talent/update`，拉黑 / 取消拉黑）。 |
| 5 | `IChannelsLeagueService` | `BatchAddLeagueItemAsync` | POST | `/channels/ec/league/item/batchadd` | `AccessToken`（Query），Query `access_token` | 批量新增联盟商品（`item/batchadd`，新增推广计划）。 |
| 6 | `IChannelsLeagueService` | `GetLeagueItemAsync` | POST | `/channels/ec/league/item/get` | `AccessToken`（Query），Query `access_token` | 获取联盟推广计划详情（`item/get`）。 |
| 7 | `IChannelsLeagueService` | `BatchAddHeadSupplierItemAsync` | POST | `/channels/ec/league/item/headsupplier/batchadd` | `AccessToken`（Query），Query `access_token` | 批量新增联盟机构推广（`item/headsupplier/batchadd`，机构带货计划）。 |
| 8 | `IChannelsLeagueService` | `GetLeagueItemListAsync` | POST | `/channels/ec/league/item/list/get` | `AccessToken`（Query），Query `access_token` | 获取联盟商品列表（`item/list/get`，分页拉取本店联盟推广商品）。 |
| 9 | `IChannelsLeagueService` | `UpdateLeagueItemAsync` | POST | `/channels/ec/league/item/upd` | `AccessToken`（Query），Query `access_token` | 更新联盟推广计划信息（`item/upd`，编辑并上架 / 上架 / 下架）。 |
| 10 | `IChannelsLeagueService` | `ListSpuPromotionPlansAsync` | POST | `/channels/ec/league/promotion/plan/list` | `AccessToken`（Query），Query `access_token` | 获取商品推广计划列表（`promotion/plan/list`）。 |
| 11 | `IChannelsLeagueService` | `ListSpuPromotionTalentsAsync` | POST | `/channels/ec/league/promotion/talent/list` | `AccessToken`（Query），Query `access_token` | 获取商品推广带货者列表（`promotion/talent/list`，按推广计划拉取带货者）。 |
| 12 | `IChannelsLeagueService` | `GetHeadSupplierBalanceAsync` | POST | `/channels/ec/league/headsupplier/funds/balance/get` | `AccessToken`（Query），Query `access_token` | 获取机构账户余额（`headsupplier/funds/balance/get`，联盟带货机构视角）。 |
| 13 | `IChannelsLeagueService` | `GetHeadSupplierFundsFlowListAsync` | POST | `/channels/ec/league/headsupplier/funds/flowlist/get` | `AccessToken`（Query），Query `access_token` | 获取机构资金流水列表（`headsupplier/funds/flowlist/get`，只回流水号，详情须逐单查）。 |
| 14 | `IChannelsLeagueService` | `GetHeadSupplierFundsFlowDetailAsync` | POST | `/channels/ec/league/headsupplier/funds/flowdetail/get` | `AccessToken`（Query），Query `access_token` | 获取机构资金流水详情（`headsupplier/funds/flowdetail/get`）。 |
| 15 | `IChannelsLeagueService` | `AddHeadSupplierCooperativeItemAsync` | POST | `/channels/ec/league/headsupplier/cooperativeitem/add` | `AccessToken`（Query），Query `access_token` | 机构添加合作商品（`headsupplier/cooperativeitem/add`，把选品广场的商品加入机构合作计划）。 |
| 16 | `IChannelsLeagueService` | `GetHeadSupplierCooperativeItemListAsync` | POST | `/channels/ec/league/headsupplier/cooperativeitem/list/get` | `AccessToken`（Query），Query `access_token` | 获取合作商品列表（`headsupplier/cooperativeitem/list/get`，机构在选品广场添加过的商品）。 |
| 17 | `IChannelsLeagueService` | `GetHeadSupplierItemPromotionDetailAsync` | POST | `/channels/ec/league/headsupplier/item/promotiondetail/get` | `AccessToken`（Query），Query `access_token` | 获取商品推广参数详情（`headsupplier/item/promotiondetail/get`，佣金率、推广链接等推广参数）。 |
| 18 | `IChannelsLeagueService` | `GetHeadSupplierSelectionProductsAsync` | POST | `/channels/ec/league/headsupplier/selectionproducts/list/get` | `AccessToken`（Query），Query `access_token` | 获取机构选品广场商品列表（`headsupplier/selectionproducts/list/get`，拉取属于机构的联盟公开 / 定向商品池）。 |
| 19 | `IChannelsLeagueService` | `GetHeadSupplierProductDetailAsync` | POST | `/channels/ec/league/headsupplier/productdetail/get` | `AccessToken`（Query），Query `access_token` | 获取商品基础详情（`headsupplier/productdetail/get`，机构视角查询合作小店的完整商品信息）。 |
| 20 | `IChannelsLeagueService` | `GetHeadSupplierProductActivityAsync` | POST | `/channels/ec/league/headsupplier/product/getactivity` | `AccessToken`（Query），Query `access_token` | 获取商品活动补贴信息（`headsupplier/product/getactivity`，品牌日 / 通用货补活动的活动价与平台补贴）。 |
| 21 | `IChannelsLeagueService` | `GetHeadSupplierShopAsync` | POST | `/channels/ec/league/headsupplier/shop/get` | `AccessToken`（Query），Query `access_token` | 获取商家合作详情（`headsupplier/shop/get`，机构视角的单个小店基础信息 + 合作统计数据 + 合作状态）。 |
| 22 | `IChannelsLeagueService` | `GetHeadSupplierShopListAsync` | POST | `/channels/ec/league/headsupplier/shop/list/get` | `AccessToken`（Query），Query `access_token` | 分页获取合作商家列表（`headsupplier/shop/list/get`，机构视角的合作小店列表）。 |
| 23 | `IChannelsLeagueService` | `UpdateHeadSupplierRelationAsync` | POST | `/channels/ec/league/headsupplier/headsupplierrelation/update` | `AccessToken`（Query），Query `access_token` | 更新机构合作关系（`headsupplier/headsupplierrelation/update`：邀请 / 通过 / 拒绝 / 取消邀请 / 取消合作）。 |
| 24 | `IChannelsLeagueService` | `GetHeadSupplierOrderListAsync` | POST | `/channels/ec/league/headsupplier/order/list/get` | `AccessToken`（Query），Query `access_token` | 获取佣金单列表（`headsupplier/order/list/get`，按时间范围 / 订单号 / 视频号 / 推客翻页捞取佣金单标识）。 |
| 25 | `IChannelsLeagueService` | `GetHeadSupplierOrderAsync` | POST | `/channels/ec/league/headsupplier/order/get` | `AccessToken`（Query），Query `access_token` | 获取佣金单详情（`headsupplier/order/get`：订单、商品、买卖双方与多层分佣明细）。 |
| 26 | `IChannelsLeagueService` | `AddHeadSupplierWindowAsync` | POST | `/channels/ec/league/headsupplier/window/add` | `AccessToken`（Query），Query `access_token` | 添加机构商品到橱窗（`headsupplier/window/add`，机构把优选联盟商品挂到达人橱窗）。 |
| 27 | `IChannelsLeagueService` | `GetHeadSupplierWindowListAsync` | POST | `/channels/ec/league/headsupplier/window/getall` | `AccessToken`（Query），Query `access_token` | 查询橱窗上机构商品列表（`headsupplier/window/getall`，按 `offset` 偏移翻页）。 |
| 28 | `IChannelsLeagueService` | `GetHeadSupplierWindowProductDetailAsync` | POST | `/channels/ec/league/headsupplier/window/getdetail` | `AccessToken`（Query），Query `access_token` | 查询橱窗上机构商品详情（`headsupplier/window/getdetail`，橱窗视角的商品快照）。 |
| 29 | `IChannelsLeagueService` | `RemoveHeadSupplierWindowAsync` | POST | `/channels/ec/league/headsupplier/window/remove` | `AccessToken`（Query），Query `access_token` | 从橱窗移除机构商品（`headsupplier/window/remove`）。 |
| 30 | `IChannelsLeagueService` | `GetHeadSupplierWindowAuthAsync` | POST | `/channels/ec/league/headsupplier/windowauth/get` | `AccessToken`（Query），Query `access_token` | 获取带货达人橱窗授权链接（`headsupplier/windowauth/get`，机构引导达人完成橱窗授权）。 |
| 31 | `IChannelsLeagueService` | `GetHeadSupplierWindowAuthStatusAsync` | POST | `/channels/ec/league/headsupplier/windowauth/status/get` | `AccessToken`（Query），Query `access_token` | 获取带货达人橱窗授权状态（`headsupplier/windowauth/status/get`）。 |
| 32 | `IChannelsLeagueService` | `GetHeadSupplierSubscribeProductsAsync` | POST | `/channels/ec/league/headsupplier/subscription/getsubscribe` | `AccessToken`（Query），Query `access_token` | 获取订阅的商品列表（`headsupplier/subscription/getsubscribe`，机构视角的主动 + 被动订阅商品）。 |
| 33 | `IChannelsLeagueService` | `SubscribeHeadSupplierProductAsync` | POST | `/channels/ec/league/headsupplier/subscription/subscribe` | `AccessToken`（Query），Query `access_token` | 商品订阅（`headsupplier/subscription/subscribe`）。 |
| 34 | `IChannelsLeagueService` | `UnsubscribeHeadSupplierProductAsync` | POST | `/channels/ec/league/headsupplier/subscription/unsubscribe` | `AccessToken`（Query），Query `access_token` | 取消商品订阅（`headsupplier/subscription/unsubscribe`）。 |
| 35 | `IChannelsLeagueService` | `AddHeadSupplierSubItemAsync` | POST | `/channels/ec/league/headsupplier/subitem/add` | `AccessToken`（Query），Query `access_token` | 配置达人佣金率（`headsupplier/subitem/add`，为「机构分配达人佣金」的商品分配达人佣金）。 |
| 36 | `IChannelsLeagueService` | `GetHeadSupplierSubItemListAsync` | POST | `/channels/ec/league/headsupplier/subitem/list/get` | `AccessToken`（Query），Query `access_token` | 获取机构配置达人佣金计划列表（`headsupplier/subitem/list/get`）。 |
| 37 | `IChannelsLeagueService` | `AddHeadSupplierHelpSellItemAsync` | POST | `/channels/ec/league/headsupplier/secondheadsupplieritem/add` | `AccessToken`（Query），Query `access_token` | 机构给帮卖机构供货（`headsupplier/secondheadsupplieritem/add`，创建机构管理页面的「机构推广」）。 |
| 38 | `IChannelsLeagueService` | `UpdateHeadSupplierHelpSellItemAsync` | POST | `/channels/ec/league/headsupplier/secondheadsupplieritem/update` | `AccessToken`（Query），Query `access_token` | 机构更新或删除已添加的推广计划（`headsupplier/secondheadsupplieritem/update`，对应后台「机构推广」改佣金率或结束推广）。 |
| 39 | `IChannelsLeagueService` | `GetHeadSupplierClueAsync` | POST | `/channels/ec/league/headsupplier/clue/get` | `AccessToken`（Query），Query `access_token` | 推客机构获取内容推广的自营商品订单详情（`headsupplier/clue/get`，即「线索订单」详情）。 |
| 40 | `IChannelsLeagueService` | `GetHeadSupplierClueListAsync` | POST | `/channels/ec/league/headsupplier/clue/list/get` | `AccessToken`（Query），Query `access_token` | 推客机构获取内容推广的自营商品订单列表（`headsupplier/clue/list/get`，只回线索单键）。 |
| 41 | `IChannelsLeagueService` | `GetHeadSupplierLiveProtectionQrCodeAsync` | POST | `/channels/ec/league/headsupplier/liveprotection/getqrcode` | `AccessToken`（Query），Query `access_token` | 获取直播大场保护的提报二维码（`headsupplier/liveprotection/getqrcode`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**同域双视角（路径前缀即视角，不得合并）**：本域内混居两套官方视角——①**小店商家侧**
（`/channels/ec/league/{promoter,talent,item,promotion}/*`，店铺 `access_token`，第三方权限集 141）；
②**联盟带货机构（headsupplier）侧**（`/channels/ec/league/headsupplier/*`，机构自身 `access_token`，
**不支持第三方平台代调用**）。两视角的字段与错误码各自成表，机构侧资金端点与 Funds 域「小店资金」
分建 DTO（详见各方法 remarks）。

**与「达人合作（合作管理）」「罗盘带货数据」概念严格区分**：本域是小店商家侧的
**优选联盟推广计划管理**（`/channels/ec/league/*`，小店 `access_token`，
第三方代调用权限集 id 141，店铺须先开通优选联盟，未开通报 `10023007`）；
达人数据看板为 Compass 域，企微「渠道二维码 / 联系我」与联盟无关。

**带货者语义（官方改版照录）**：官方自 2026-06 起将推广对象统一为「带货者」
（`talent`，带货者 appid），旧「达人 finder / promoter」字段已由变更日志声明删除
（如 `finder_ids` / `promoter_ids` / `last_buffer` 等）——本域字段以官方
**参数表**为准（talent 系命名），与官方代码示例中残留的旧字段名不一致处在各方法 remarks 照录。

**「type」一字两表**：请求侧「获取商品推广类别」（`type`：`100` 全部带货者计划 /
`300` 指定带货者计划，见 `ChannelsLeaguePlanQueryTypes`）与计划/佣金侧的
「普通 / 定向 / 专属」三分类（值 1/2/3，见 `ChannelsLeaguePromotionTypes`）是
**两套不同枚举**，勿混用。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）第三方平台代调用所属权限集 id 为 `141`；账号类型须为「微信小店（需申请）」。
- （出现 41 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 3 次）官方错误码：无特殊错误码（走通用错误码）。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `141`；不支持云调用。
- （出现 7 次）官方错误码：`10024000`（参数错误）。
- （出现 17 次）官方适用范围：「联盟带货机构」账号类型；**不支持云调用**；**不支持第三方平台调用**。

### 1. `GetPromoterAsync` — 获取带货者详情信息（`promoter/get`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/promoter/get`

- **请求体**：`ChannelsGetPromoterRequest` — 查询请求（`talent_appid` 与 `talent_search_key` 均选填）。
- **返回**：`ChannelsGetPromoterResponse` — 带货者信息（`talent`：基础信息 / 数据信息 / 特征信息，含累计与近 30 日销售额、订单数、橱窗商品数、等级与拉黑状态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_getpromoter
- 官方业务限制：①带货者等级枚举见 `ChannelsLeagueTalentLevels`（0 带货新人 ~ 5 五星带货者）； / ②`TalentSearchKey` 支持带货者 appid 和视频号 finder_id 两种搜索参数。
- 官方错误码：`10023000`（参数错误）/ `10023001`（不合法的 finder_id）/ `10023007`（请先开通优选联盟）/ `10024003`（不合法的 appid）。

### 2. `GetPromoterListAsync` — 获取带货者列表（`promoter/list/get`，商家带货者列表分页查询）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/promoter/list/get`

- **请求体**：`ChannelsGetPromoterListRequest` — 查询请求（`page_size` 必填且不超过 20；`page_index` 选填，下标从 1 开始）。
- **返回**：`ChannelsGetPromoterListResponse` — 带货者总数、翻页标志与列表（`total_num`/`continue_flag`/`list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_getpromoterlist
- 官方业务限制：`page_size` 单页带货者数不超过 20；`page_index` 下标从 1 开始，默认为 1； / `continue_flag` 为 false 表示已结束。
- 官方错误码：`10023000`（参数错误）/ `10023007`（请先开通优选联盟）。

### 3. `GetTalentProductDataListAsync` — 获取带货者商品数据列表（`talent/getproductdatalist`，合作管理列表）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/talent/getproductdatalist`

- **请求体**：`ChannelsGetTalentProductDataListRequest` — 查询请求（`get_data_type`/`page_index`/`page_size`/`talent_appid` 均必填）。
- **返回**：`ChannelsGetTalentProductDataListResponse` — 商品总数、翻页标志与商品数据列表（`total_num`/`continue_flag`/`list`，元素含 `spu_id` / 商品数据 / 带货者佣金信息）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_gettalentproductdata
- 官方业务限制：①`get_data_type` 获取的商品数据类型枚举见 / `ChannelsLeagueGetDataTypes`（1 近 30 天日成交商品数据 / 2 当前橱窗商品数据）； / ②`page_size` 不超过 20；③佣金率 `ratio` 取值 `[0, 1000000]`，1000000 = 100%； / ④佣金类型枚举见 `ChannelsLeaguePromotionTypes`（1 普通计划 / 2 定向计划 / 3 专属计划）。

### 4. `UpdateTalentAsync` — 编辑带货者（`talent/update`，拉黑 / 取消拉黑）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/talent/update`

- **请求体**：`ChannelsUpdateTalentRequest` — 编辑请求（`talent_appid`/`upd_talent_type` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_updatetalent
- 官方业务限制：编辑操作类型枚举见 `ChannelsLeagueUpdTalentTypes` / （1 拉黑带货者 / 2 取消拉黑带货者）；官方变更日志 2026-03-30 新增该接口。

### 5. `BatchAddLeagueItemAsync` — 批量新增联盟商品（`item/batchadd`，新增推广计划）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/item/batchadd`

- **请求体**：`ChannelsBatchAddLeagueItemRequest` — 新增请求（`type`/`list` 必填，商品不可重复且数量不超过 20）。
- **返回**：`ChannelsBatchAddLeagueItemResponse` — 逐商品添加结果列表（`result_info_list`：单商品错误码 / 推广计划 id / 商品 id / 推广失败带货者列表）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_batchadditem
- 官方业务限制：①「获取商品推广类别」枚举见 `ChannelsLeaguePlanQueryTypes` / （100 全部带货者计划 / 300 指定带货者计划）；②商品列表 `product_id` 不可重复、数量不超过 20； / ③推广佣金 `ratio` 取值 `[0, 90]`%；④普通/定向计划与专属计划互斥（`10023105`）。
- **官方文档矛盾照录**：请求体参数表字段为 `talent_appids`（带货者 appid 列表）， / 代码示例写作 `promoter_ids`；响应参数表字段为 `promotion_plan_id`，示例写作 / `info_id`/`ratio`——SDK 按参数表建模（官方变更日志 2026-06-17：删除 / finder_ids/promoter_ids/fail_promoter_ids/fail_finder_ids/info_id，新增 / talent_appids/promotion_plan_id/fail_talent_appids）。
- 官方错误码：`10023001` / `10023007` / `10024003`（不合法的 appid）/ / `10023100`（type 类型错误）/ `10023101`（不允许相同 product_id）/ `10023102`（操作数量超过限制）/ / `10023103`（不存在该商品）/ `10023104`（该小店商品未上架）/ `10023105`（普通/定向计划与专属计划互斥）/ / `10023106` / `10023107` / `10023108` / `10023109`（佣金数值非法）/ `10023110`（新增达人数量超过计划上限）/ / `10023111`（未达到准入标准）/ `10023112`（不存在合作关系）/ `10023113`（计划持续时间非法）/ / `10023114`（运营处罚）/ `10023115`（类目禁售）/ `10023117`（预售中）/ `10023121`（店铺新手村限制）。

### 6. `GetLeagueItemAsync` — 获取联盟推广计划详情（`item/get`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/item/get`

- **请求体**：`ChannelsGetLeagueItemRequest` — 查询请求（`type`/`product_id` 必填，`promotion_plan_id` 选填）。
- **返回**：`ChannelsGetLeagueItemResponse` — 推广计划信息（`item`：商品 id / 推广类别 / 推广状态 / 佣金 / 特殊推广信息 / 扩展信息）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_getitem
- 官方业务限制：①响应 `item.type`（普通推广商品 / 定向推广商品 / 专属推广商品）为 / `ChannelsLeaguePromotionTypes`，与请求侧 `ChannelsLeaguePlanQueryTypes` / 是两套枚举；②推广状态枚举见 `ChannelsLeagueItemStatuses` / （2026-07-15 新增 7 商品下架终止 / 8 计划过期终止 / 9 商品被封禁下架终止三个终止推广选项）； / ③`IsBanned` 标识商品是否被官方封禁。
- **官方文档矛盾照录**：响应 `exclusive_info` 参数表字段为 / `promotion_plan_id`/`begin_time`/`end_time`/`is_forerver`/`talent_num`， / 代码示例写作 `info_id`/`finder_ids`/`finder_num`/`finder_info_list`； / `ext_info` 参数表为 `is_banned`、示例为 `is_sale_forbidden`——SDK 均按参数表建模 / （官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
- 官方错误码：`10023000`（参数错误）/ `10023001`（不合法的 finder_id）/ `10023007`（请先开通优选联盟）/ / `10023100`（type 类型错误）/ `10023103`（不存在该商品）。

### 7. `BatchAddHeadSupplierItemAsync` — 批量新增联盟机构推广（`item/headsupplier/batchadd`，机构带货计划）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/item/headsupplier/batchadd`

- **请求体**：`ChannelsBatchAddHeadSupplierItemRequest` — 新增请求（`headsupplier_appid`/`list` 必填；元素须带 `ratio` 与 `service_ratio`）。
- **返回**：`ChannelsBatchAddHeadSupplierResponse` — 逐商品添加结果列表（`result_info_list`：单商品错误码 / 商品 id）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_batchaddheadsupplieritem
- 官方业务限制：①`headsupplier_appid` 为机构开放唯一凭证（账号 ID），机构可在 / 「微信小店·联盟带货机构管理平台 - 设置 - 开放信息」中获得，须已完成机构开通且账号无异常状态； / ②推广佣金 `ratio` 与推广服务费 `service_ratio` 取值均为 `[0, 90]`%； / ③机构计划类型枚举见 `ChannelsLeagueHeadSupplierPlanTypes` / （0 商家指定达人佣金 / 1 机构指定达人佣金【总包计划类型】）。
- 官方错误码：`10023000` / `10023007` / `10024003`（机构 appid 不存在）/ / `10023100`~`10023121`（与批量新增联盟商品同族，含 `10023101` 不允许相同 product_id、 / `10023102` 操作数量超过限制、`10023109` 佣金数值非法等）。

### 8. `GetLeagueItemListAsync` — 获取联盟商品列表（`item/list/get`，分页拉取本店联盟推广商品）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/item/list/get`

- **请求体**：`ChannelsGetLeagueItemListRequest` — 查询请求（`page_size` 必填且不超过 100；其余选填）。
- **返回**：`ChannelsGetLeagueItemListResponse` — 商品信息列表、翻页标志与总数（`items`/`has_more`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_getitemlist
- 官方业务限制：①`page_size` 单页商品数不超过 100；②**接口限制了 `page_size × page_index ≤ 5000`**； / ③商品状态枚举见 `ChannelsLeagueItemStatuses`（与推广计划详情同值域）。
- **官方文档矛盾照录**：注意事项建议「命中限制时改用传 `last_buffer` 顺序翻页」， / 但官方变更日志 2026-06-17 已删除 `last_buffer` 等字段、当前参数表无此入参——SDK 按参数表建模（无 last_buffer）。
- 官方错误码：`10023000`（参数错误）/ `10023001`（不合法的 finder_id）/ `10023007`（请先开通优选联盟）/ / `10023100`（type 类型错误）/ `10023103`（不存在该推广商品）。

### 9. `UpdateLeagueItemAsync` — 更新联盟推广计划信息（`item/upd`，编辑并上架 / 上架 / 下架）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/item/upd`

- **请求体**：`ChannelsUpdateLeagueItemRequest` — 更新请求（`type`/`product_id`/`promotion_plan_id`/`operate_type` 必填）。
- **返回**：`ChannelsUpdateLeagueItemResponse` — 推广计划 id 与更新失败达人列表（`promotion_plan_id`/`fail_talent_appids`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_upditem
- 官方业务限制：①更新操作类别枚举见 `ChannelsLeagueOperateTypes` / （1 编辑并上架 / 2 下架 / 4 上架）；②`ratio` 操作为编辑时必填，取值 `[0, 90]`%； / ③`exclusive_info` 内新增 / 删除推广达人列表各不超过 20 个。
- 官方错误码：`10023000` / `10023001` / `10023007` / `10023100`（type 类型错误）/ / `10023102` / `10023103` / `10023104` / `10023105`~`10023115` / / `10023116`（operate_type 非法）/ `10023117` / `10023121`。

### 10. `ListSpuPromotionPlansAsync` — 获取商品推广计划列表（`promotion/plan/list`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/promotion/plan/list`

- **请求体**：`ChannelsListSpuPromotionPlansRequest` — 查询请求（`product_id`/`page_index`/`page_size` 必填，`promotion_plan_type` 选填）。
- **返回**：`ChannelsListSpuPromotionPlansResponse` — 计划总数、翻页标志与计划列表（`total_num`/`continue_flag`/`list`，元素含 `promotion_plan_id`/`promotion_plan_type`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_listspupromotionplans
- 官方业务限制：①`page_size` 不超过 20；②推广计划类型枚举见 / `ChannelsLeaguePromotionPlanTypes`（100 全部带货者 / 200 圈选带货者 / 300 指定带货者）。

### 11. `ListSpuPromotionTalentsAsync` — 获取商品推广带货者列表（`promotion/talent/list`，按推广计划拉取带货者）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/promotion/talent/list`

- **请求体**：`ChannelsListSpuPromotionTalentsRequest` — 查询请求（`product_id`/`page_index`/`page_size`/`promotion_plan_id` 必填，`talent_appid` 选填）。
- **返回**：`ChannelsListSpuPromotionTalentsResponse` — 带货者总数、翻页标志与列表（`total_num`/`continue_flag`/`list`，元素含基础信息 / 佣金信息 / 推广状态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_listspupromotiontalents
- 官方业务限制：①`page_size` 单页带货者数不超过 10；②带货者推广状态枚举见 / `ChannelsLeagueTalentRelationStatuses`（1 推广中 / 4 已移除 / 10 已被拉黑）； / ③佣金率 `ratio` 取值 `[0, 1000000]`，1000000 = 100%。
- 官方错误码：无特殊错误码（走通用错误码）；官方变更日志 2026-06-11 新增该接口、2026-07-28 更新 page_size 描述。

### 12. `GetHeadSupplierBalanceAsync` — 获取机构账户余额（`headsupplier/funds/balance/get`，联盟带货机构视角）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/funds/balance/get`
- **返回**：`ChannelsLeagueHeadSupplierGetBalanceResponse` — 可提现余额与待结算余额（`available_amount`/`pending_amount`，单位分）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getbalance.html（2026-10-17 逐页核验）
- 官方契约：**POST** + 请求体参数表为**无**（官方注意事项原文「调用接口时传空的 json 串即可」） / ⇒ SDK 不造请求体 DTO，守卫 CLG2 锁定本端点不得携带 `[Body]`。
- 官方适用范围：本接口支持「**联盟带货机构**」账号类型调用（机构自身凭据视角，与本域商家侧 / `/channels/ec/league/{promoter,item,talent,promotion}/*` 族不同）；**支持云调用** / （`channels.ec.league.headsupplier.funds.balance.get`）；**不支持第三方平台调用**。
- 官方错误码：本接口无特殊错误码（走通用错误码）。

### 13. `GetHeadSupplierFundsFlowListAsync` — 获取机构资金流水列表（`headsupplier/funds/flowlist/get`，只回流水号，详情须逐单查）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/funds/flowlist/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetFundsFlowListRequest` — 查询请求（全部选填：`page`/`page_size`/`next_key`/`start_time`/`end_time`/`funds_type`）。
- **返回**：`ChannelsGetFundsFlowListResponse` — 流水单号列表与翻页标志（`flow_ids`/`has_more`/`next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getfundsflowlist.html（2026-10-17 逐页核验）
- 官方业务限制：①深翻页约束——`page * page_size >= 5000` 时 `next_key` **必填** / （官方参数表原文），SDK 不做本地拦截，越界由 `10024000` 表达；②`next_key` 仅在 / 「`page` 为上一页加一且 `page_size` 与上一页相同」时才生效；③`page_size` 不填默认 `10`； / ④本接口列表**只返回流水单号**，金额与关联单据须经 `GetHeadSupplierFundsFlowDetailAsync` 逐单获取。
- 官方枚举：`funds_type` 1 提现 / 2 分账 / 3 联盟技术服务费（2026-03-04 变更日志新增）/ 4 分账回退（2026-06-01 变更日志新增），见 `ChannelsLeagueHeadSupplierFundsTypes`。
- 官方适用范围：「联盟带货机构」账号类型；**支持云调用**（`channels.ec.league.headsupplier.funds.flowlist.get`）；**不支持第三方平台调用**。
- 响应侧复用裁决：本接口返回参数表（`flow_ids`/`has_more`/`next_key`）与小店商家侧 / `merchant/funds/flowlist/get` 逐字段一致 ⇒ 复用 Funds 域 / `ChannelsGetFundsFlowListResponse`，不分建（守卫 CLG5 锁类型同一）；请求侧因机构侧多 / `funds_type`、无 `transaction_id` ⇒ 分建。

### 14. `GetHeadSupplierFundsFlowDetailAsync` — 获取机构资金流水详情（`headsupplier/funds/flowdetail/get`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/funds/flowdetail/get`

- **请求体**：`ChannelsGetFundsFlowDetailRequest` — 查询请求（`flow_id` 必填，经流水列表接口获取）。
- **返回**：`ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse` — 流水信息（`funds_flow`：资金类型 / 金额 / 余额 / 记账时间 / 备注 / 关联订单号 / 关联提现单号）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getfundsflowdetail.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；**支持云调用**（`channels.ec.league.headsupplier.funds.flowdetail.get`）；**不支持第三方平台调用**。
- 官方错误码：`10024000`（参数错误）/ `10024008`（不存在该流水）。
- 分建与复用裁决：请求侧 `flow_id` 单字段与商家侧一致 ⇒ 复用 `ChannelsGetFundsFlowDetailRequest`； / `funds_flow` 元素字段集机构侧含 `order_id`/`withdraw_id`、无 `flow_type`/ / `related_info_list`/`funds_type_desc` ⇒ 响应与元素类型分建（守卫 CLG5 锁两侧元素类型不同）。
- 官方变更日志：2026-03-04 与 2026-06-01 分别新增 `funds_type` 枚举值 `3`、`4`。

### 15. `AddHeadSupplierCooperativeItemAsync` — 机构添加合作商品（`headsupplier/cooperativeitem/add`，把选品广场的商品加入机构合作计划）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/cooperativeitem/add`

- **请求体**：`ChannelsLeagueHeadSupplierAddCooperativeItemRequest` — 请求体（`head_supplier_item_link` 必填，`hidden_flag` 选填）。
- **返回**：`ChannelsLeagueHeadSupplierAddCooperativeItemResponse` — 机构合作计划 id（`cooperative_item_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_addcooperativeitem.html（2026-10-17 逐页核验）
- 官方业务限制：`head_supplier_item_link` 须取自「获取机构选品广场商品列表」或「获取合作商品列表」回包，形如 `weixinstorehs/123456`；`hidden_flag` 控制是否在机构主页隐藏。
- 官方错误码：`10024000`（参数错误）/ `10024024`（该计划未在待选品状态）/ `10024025`（商品计划不在上架中）—— 后两枚为官方变更日志 2025-11-24 新增。
- 官方注意事项：本接口已升级迭代，请开发者尽快适配。

### 16. `GetHeadSupplierCooperativeItemListAsync` — 获取合作商品列表（`headsupplier/cooperativeitem/list/get`，机构在选品广场添加过的商品）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/cooperativeitem/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetCooperativeItemListRequest` — 查询请求（`commission_type`/`page_size`/`next_key` 必填，`listing_time_range` 选填）。
- **返回**：`ChannelsLeagueHeadSupplierGetCooperativeItemListResponse` — 合作商品列表（`list`：`product_id`/`cooperative_item_id`/`head_supplier_item_link`）与下一页 `next_key`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getcooperativeitemlist.html（2026-10-17 逐页核验）
- 官方业务限制：①**翻页为 `next_key` 游标式**（第一页传空串，之后写入上一页返回值），`page_size` 最大 `20`； / ②`commission_type` 语义 0 商家指定达人佣金 / 1 机构指定达人佣金，与推广详情的 `plan_type`（1 定向 / 2 公开）是**两套不同枚举**； / ③推客商品的获取不走本接口，官方要求改用「推客带货 / 商品获取 / 获取可推广的商品 id 列表」。
- 官方变更日志：2025-11-03 `Res.list` 的 `product_id` 类型由 `string` 变更为 `number`；本仓按新类型建模。
- 旧接口 `headsupplier/item/list/get`（获取合作商品列表-旧）已从官方目录树移除 ⇒ 本仓**不实现**，由守卫 CLG7 锁定零声明。

### 17. `GetHeadSupplierItemPromotionDetailAsync` — 获取商品推广参数详情（`headsupplier/item/promotiondetail/get`，佣金率、推广链接等推广参数）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/item/promotiondetail/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest` — 查询请求（`head_supplier_item_link` 必填）。
- **返回**：`ChannelsLeagueHeadSupplierGetItemPromotionDetailResponse` — 推广详情（`item`：商品与店铺 / 供货来源 / 佣金信息 / 供货机构信息 / 合作信息）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getitempromotiondetail.html（2026-10-17 逐页核验）
- 官方业务限制：①本接口只返回**推广参数**，商品基础信息（图、类目等）须走 `productdetail/get`； / ②`commission_type = 1`（机构指定达人佣金）时该计划无达人佣金率，须先创建子计划（`subitem/add`）才能上橱窗； / ③`link`（带货链接）仅在「商家指定达人佣金」且机构商品上架中时返回； / ④佣金与服务费率取值 `[0, 1000000]`，`1000000` 表示全额。
- 官方错误码：`10024000`（参数错误）/ `10024025`（商品计划不在上架中，2025-12-19 新增）。
- 官方变更日志：2025-11-03 `head_supplier_info` 新增 `head_img`；2025-12-19 新增错误码 `10024025`；2026-02-09 更新 `link` 描述。

### 18. `GetHeadSupplierSelectionProductsAsync` — 获取机构选品广场商品列表（`headsupplier/selectionproducts/list/get`，拉取属于机构的联盟公开 / 定向商品池）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/selectionproducts/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetSelectionProductsRequest` — 查询请求（`plan_type` 必填：1 定向计划 / 2 公开计划；可带属性、类目、关键词筛选与游标翻页）。
- **返回**：`ChannelsLeagueHeadSupplierGetSelectionProductsResponse` — 商品列表（`product_list`：`product_id` / `shop_appid` / `head_supplier_item_link`）与下一页 `next_key`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_listselectionproducts.html（2026-10-17 逐页核验）
- 官方业务限制：①翻页为 `next_key` 游标式（第一页传空），`page_size` 最大 `20`； / ②**类目筛选（`category`）定向计划不支持**；③**`create_time_range` 与 `keyword` 不能共用**； / ④`spu_item_condition` 内六个区间对象字段集完全同形（`min`/`max`），服务费率与佣金率区间单位为 **10 万分之**； / ⑤`plan_type`（1 定向 / 2 公开）与「获取合作商品列表」的 `commission_type`（0/1）是两套不同枚举，本域 `commission_type` 又为 1/2 ⇒ 常量类分建。
- 官方变更日志：2026-03-09 新增 `commission_type`（佣金类型查询条件）与 `from_head_supplier_appid`（来源机构筛选） / ——但同日核验证的请求参数表内**无** `from_head_supplier_appid` ⇒ 本仓照参数表建模、不回填日志提及字段。

### 19. `GetHeadSupplierProductDetailAsync` — 获取商品基础详情（`headsupplier/productdetail/get`，机构视角查询合作小店的完整商品信息）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/productdetail/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetProductDetailRequest` — 查询请求（`shop_appid` 与 `product_id` 均必填）。
- **返回**：`ChannelsLeagueHeadSupplierGetProductDetailResponse` — 商品详情（`item`：所属小店 / 商品 ID / `product_info` 完整商品信息 / `shop` 店铺信息）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_getproductdetail.html（2026-10-17 逐页核验）
- 官方业务限制：①本接口是「机构查别人的商品」，必须同时给商品**所属小店 appid**，与小店商家侧 `product/get`（本店令牌 + `product_id`） / 是两套不同契约；②返回的 `freight_info` 是完整物流模板（含条件包邮、分地区计费、不发货区域），与商家侧 `express_info`（`template_id` 三元组）字段集完全不同 ⇒ 分建 DTO； / ③`is_hot_brand` 官方已废弃，改看 `is_brand_good`。
- 官方错误码：`10024000`（参数错误）/ `10024003`（不合法的 appid）/ `10024004`（不存在该商品）/ `10024043`（商品不属于该店铺，2026-07-10 新增）。
- 官方文档自身差异（照录不改）：返回参数表根字段为 `item`、代码示例写 `product`； / 参数表有 `freight_info`、代码示例写 `express_info` + `statistical_data` ⇒ 本仓**以参数表为权威**。
- 官方变更日志：2026-02-04 新增 `cats_v2` / `extra_service` / `product_spring_festival_delivery_config`；2026-03-09 新增 `freight_info` 等一批字段与「发货时间期限」枚举；2026-05-09 `is_hot_brand` 废弃、新增 `is_brand_good`；2026-09-16 sku 级新增用户普惠价。

### 20. `GetHeadSupplierProductActivityAsync` — 获取商品活动补贴信息（`headsupplier/product/getactivity`，品牌日 / 通用货补活动的活动价与平台补贴）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/product/getactivity`

- **请求体**：`ChannelsLeagueHeadSupplierGetProductActivityRequest` — 查询请求（`product_id` 与 `shop_appid` 均必填）。
- **返回**：`ChannelsLeagueHeadSupplierGetProductActivityResponse` — 是否存在活动（`has_activity`）与活动列表（`product_activity_list`：活动价 / 平台补贴金额与件数 / 活动子类型 / 结束时间 / 库存模式与数量）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_league_get_product_activity_info.html（2026-10-17 逐页核验）
- 官方业务限制：`activity_sub_type` **目前仅支持「3 品牌日」「4 通用货补」**，其他活动类型不返回； / `stock_mode` 为 `0` 活动库存 / `1` 商品自营库存（2026-03-27 官方优化该字段描述）；金额字段单位均为分。
- 官方错误码：`-1`（system error）/ `10024000`（参数错误）/ `10024009`（不合法的商品 ID）/ `10024012`（获取店铺信息失败）。
- 官方变更日志：2026-03-09 上线；2026-03-27 优化库存描述。

### 21. `GetHeadSupplierShopAsync` — 获取商家合作详情（`headsupplier/shop/get`，机构视角的单个小店基础信息 + 合作统计数据 + 合作状态）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/shop/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetShopRequest` — 查询请求（`appid` 必填）。
- **返回**：`ChannelsLeagueHeadSupplierGetShopResponse` — 小店详情（`shop_detail`：基础信息、统计数据、合作状态、开始合作时间戳）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_getshop.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.shop.get`；**不支持第三方平台调用**。
- 官方业务限制：①返回的 `data_info` 中 `gmv` 与 `unsettle_amount` 单位为分； / ②列表接口 `shop/list/get` 的列表项只有 `base_info` + `status`（无统计数据与合作时间）⇒ 两形态分建、 / `base_info` 同形复用；③请求体参数表只有 `appid`，代码示例残留 `page_size`（属文档陈旧）⇒ 不建模。
- 官方错误码：`10024000`（参数错误）/ `10024007`（与商家无关联关系）。
- 官方变更日志：2025-11-24 返回根字段 `shop_list` 修正为 `shop_detail`。

### 22. `GetHeadSupplierShopListAsync` — 分页获取合作商家列表（`headsupplier/shop/list/get`，机构视角的合作小店列表）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/shop/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetShopListRequest` — 查询请求（`page_size` 必填且不超过 30，翻页传 `next_key`）。
- **返回**：`ChannelsLeagueHeadSupplierGetShopListResponse` — 商家列表（`shop_list`：基础信息 + 合作状态）、翻页上下文与 `has_more`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_getshoplist.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.shop.list.get`；**不支持第三方平台调用**。
- 官方业务限制：`page_size` 必填且不超过 30；`next_key` 由上次请求返回、仅用于顺序翻页。

### 23. `UpdateHeadSupplierRelationAsync` — 更新机构合作关系（`headsupplier/headsupplierrelation/update`：邀请 / 通过 / 拒绝 / 取消邀请 / 取消合作）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/headsupplierrelation/update`

- **请求体**：`ChannelsLeagueHeadSupplierUpdateRelationRequest` — 更新请求（`object_type` 必填；机构侧须传 `to_head_supplier_appid` + `hs_update_type`，店铺侧须传 `to_shop_appid` + `shop_update_type`）。
- **返回**：`ChannelsResponse` — 官方返回体仅 `errcode` / `errmsg`，复用 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_update_hs_relation.html（2026-10-17 逐页核验）
- 官方业务限制：`object_type` 须为 `1`（机构）或 `2`（店铺），两套必填参数按对象类型分流 / （`hs_update_type` 值域 1-5、`shop_update_type` 值域 1-3）；返回体无业务字段。
- 官方错误码：`-1`（系统错误）/ `10024000`（参数错误）/ `10024035`（已经合作，无需重复申请）/ `10024037`（当前关系状态不允许该操作）/ `10024040`（已经接受该团长的邀请，无需重复操作）。
- 官方变更日志：2026-03-09 上线。

### 24. `GetHeadSupplierOrderListAsync` — 获取佣金单列表（`headsupplier/order/list/get`，按时间范围 / 订单号 / 视频号 / 推客翻页捞取佣金单标识）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/order/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetOrderListRequest` — 查询请求（`page_size` 必填且不超过 30；`create_time_range` 与 `update_time_range` 必填一个且互斥）。
- **返回**：`ChannelsLeagueHeadSupplierGetOrderListResponse` — 佣金单标识列表（`list`：订单号 + skuid）、翻页上下文与 `has_more`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/API/getorderlist.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.order.list.get`；**不支持第三方平台调用**。
- 官方业务限制：①**创建时间范围与更新时间范围必填一个**（填了其一则不填另一）；②填 `order_id` 后其他过滤参数无效； / ③本接口只返回 `order_id` + `sku_id`，详情须再调 `order/get`；④时间范围为秒级时间戳。
- 官方错误码：`10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024003`（不合法的 appid）。

### 25. `GetHeadSupplierOrderAsync` — 获取佣金单详情（`headsupplier/order/get`：订单、商品、买卖双方与多层分佣明细）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/order/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetOrderRequest` — 查询请求（`order_id` 与 `sku_id` 均必填、均为 number）。
- **返回**：`ChannelsLeagueHeadSupplierGetOrderResponse` — 佣金单结构（官方根字段名为 `commssion_order`，照抄官方漏字母 i 的拼写）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/API/getorder.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.order.get`；**不支持第三方平台调用**。
- 官方业务限制：①本接口是「联盟带货机构」视角的分佣单，与小店商家侧 `order/get`（交易单）不是同一契约； / ②**两张同名 `status` 枚举表**：佣金单状态（20/100/200）与订单状态（10/12/17/20/21/30/100/200/250）⇒ 分建常量类； / ③费率类字段（`ratio` / `service_ratio`）区间 `[0, 1000000]`（十万分比）、金额类字段单位为分； / ④帮卖 / 推客 / 供货机构三处 `*_agency_info` 字段集完全同形 ⇒ 复用一支类型； / ⑤**推广信息内层的 `finder_info` / `talent_info` 与外层同名字段字段集不同**（无 ratio/amount）⇒ 各建一支类型。
- 官方文档自身差异（照录不改）：返回根字段 `commssion_order` 拼写漏字母 i；参数表定 `order_id` 为 string、`sku_id` 为 number，代码示例把两者都写成字符串；参数表 `order_info.status` 在示例里写 `order_status`；机构三处说明均复制为「帮卖机构昵称」⇒ 本仓**以参数表为权威**。
- 官方错误码：`10024000`（参数错误）/ `10024006`（不存在该佣金单）/ `10024034`（订单已换 sku，当前 sku 不存在分佣单）。
- 官方变更日志：2026-04-23 更新 `service_amount` 描述、新增 `platform_commission_status` / `service_total_amount`、更新 `status` 枚举值；2026-03-31 新增 `product_cnt`；2026-03-12 新增 `custom_token`；2026-03-06 新增 `platform_amount`；2026-02-26 新增错误码与错误信息。

### 26. `AddHeadSupplierWindowAsync` — 添加机构商品到橱窗（`headsupplier/window/add`，机构把优选联盟商品挂到达人橱窗）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/window/add`

- **请求体**：`ChannelsLeagueHeadSupplierAddWindowRequest` — 请求（`product_id` 必填；机构类商品须传 `product_link`；达人定位走 `openfinderid` 与 `opentalentid` 二选一）。
- **返回**：`ChannelsResponse` — 官方返回体仅 `errcode` / `errmsg`，复用 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_addwindow.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.window.add`；**不支持第三方平台调用**。
- 官方业务限制：①`product_link` 取「获取合作商品列表」中**商家指定达人佣金**类型的 `head_supplier_item_link`， / 「机构指定达人佣金」类型须先创建子计划再取其 `head_supplier_item_link`； / ②`openfinderid`（视频号橱窗）与 `opentalentid`（带货助手橱窗）二选一；③`finder_id` 官方标注后续废弃； / ④返回体无业务字段。
- 官方错误码：`10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024003`（不合法的 appid）/ `10024009`（不合法的 product_id）/ `10024010`（橱窗操作失败）/ `10024011`（未获得橱窗授权）/ `10024021`（橱窗上架商品失败，官方同值两条描述：含「当前视频号未绑定带货者账号」）/ `10024023`（解析商品链接失败）。
- 官方变更日志：2025-11-24 新增错误码 10024021；2026-02-25 更新 `product_link` 字段描述。

### 27. `GetHeadSupplierWindowListAsync` — 查询橱窗上机构商品列表（`headsupplier/window/getall`，按 `offset` 偏移翻页）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/window/getall`

- **请求体**：`ChannelsLeagueHeadSupplierGetWindowListRequest` — 查询请求（`offset` 必填从 0 开始，`page_size` 默认 100、最大 500，需总数时传 `need_total_num`）。
- **返回**：`ChannelsLeagueHeadSupplierGetWindowListResponse` — 商品概要列表（`list`：机构 appid + 机构商品 ID + 橱窗商品 ID + 是否可操作）、`next_offset`、`has_more` 与 `total_num`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getallwindow.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.window.getall`；**不支持第三方平台调用**。
- 官方业务限制：①本族翻页是 `offset` / `next_offset`，**与机构侧其余列表接口的 `next_key` 形态不同**； / ②`is_get_all=true` 返回达人橱窗**全部**商品（含其它机构与达人自加商品），只想看本机构商品时不得置真； / ③移除操作的权限校验按「商品归属 + 添加操作者」判定（官方注意事项表），故列表项带 `is_operation_allowed`。
- 官方错误码：`10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024003`（不合法的 appid）/ `10024009`（不合法的 product_id）/ `10024010`（橱窗操作失败）/ `10024011`（未获得橱窗授权）/ `10024012`（不合法的参数，查询不到对应账号，2025-11-24 新增）。
- 官方变更日志：2025-11-24 新增错误码 10024012；另官方正文标注 2025-02 起支持查询达人全部橱窗商品。

### 28. `GetHeadSupplierWindowProductDetailAsync` — 查询橱窗上机构商品详情（`headsupplier/window/getdetail`，橱窗视角的商品快照）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/window/getdetail`

- **请求体**：`ChannelsLeagueHeadSupplierGetWindowProductDetailRequest` — 查询请求（达人定位 `openfinderid` / `opentalentid` 二选一；商品定位 `product_id` / `product_window_id` 二选一，官方推荐后者）。
- **返回**：`ChannelsLeagueHeadSupplierGetWindowProductDetailResponse` — 商品所有信息（`product_detail`：所属小店 appid、商品 id 与标题 / 副标题 / 主图 / 详情 / 新旧类目树 / 推广链接）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowdetail.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.window.getdetail`；**不支持第三方平台调用**。
- 官方业务限制：①本接口是**橱窗快照**，字段集远小于 `headsupplier/productdetail/get`（无 sku / 物流 / 服务保障）⇒ 分建 DTO； / ②官方启用新多级类目树：`cats` 为旧三级结构、`cats_v2` 为多级结构，过渡期两者兼容返回，官方建议切换 `cats_v2`； / ③本接口 `cats` / `cats_v2` 元素**只有 `cat_id`**（机构商品详情的 `cats_v2` 另含 `cat_name`）⇒ 复用 Product 域类目类型。
- 官方错误码：`2`（该商品已移除）/ `3`（该商品已永久删除）/ `10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024003`（不合法的 appid）/ `10024009`（不合法的 product_id）/ `10024010`（橱窗操作失败）/ `10024011`（未获得橱窗授权，2025-11-21 新增）/ `10024020`（橱窗不存在该商品）。
- 官方变更日志：2025-11-21 新增错误码 10024011。

### 29. `RemoveHeadSupplierWindowAsync` — 从橱窗移除机构商品（`headsupplier/window/remove`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/window/remove`

- **请求体**：`ChannelsLeagueHeadSupplierRemoveWindowRequest` — 请求（达人定位二选一；商品定位 `product_id` / `product_window_id` 二选一）。
- **返回**：`ChannelsResponse` — 官方返回体仅 `errcode` / `errmsg`，复用 `ChannelsResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_removewindow.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.window.remove`；**不支持第三方平台调用**。
- 官方业务限制：①请求体无 `appid`（与 `window/getdetail` 不同）；②权限面见 `window/getall` 的「商品归属 + 添加操作者」判定， / 机构商品若由达人自行添加则不可移出；③返回体无业务字段。
- 官方错误码：`10024000`（参数错误）/ `10024010`（橱窗操作失败）/ `10024011`（未获得橱窗授权）/ `10024020`（橱窗不存在该商品）等，同族橱窗错误码。

### 30. `GetHeadSupplierWindowAuthAsync` — 获取带货达人橱窗授权链接（`headsupplier/windowauth/get`，机构引导达人完成橱窗授权）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/windowauth/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetWindowAuthRequest` — 请求（`finder_id` / `openid` / `talent_appid` 三选一）。
- **返回**：`ChannelsLeagueHeadSupplierGetWindowAuthResponse` — 授权链接信息（`auth_info`：授权链接、小程序 name / appid / 路径）与 `openfinderid` / `opentalentid`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowauth.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.windowauth.get`；**不支持第三方平台调用**。
- 官方业务限制：①三选一（`openid` 须为已在机构端绑定小程序的用户）； / ②官方参数表把三行都标「必填 = 是」，实为三选一 ⇒ 本仓全部建模为可空； / ③返回体的 `openfinderid` / `opentalentid` 在参数表位于根、代码示例只出现 `opentalentid` ⇒ 以参数表为权威。
- 官方错误码：`10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024002`（达人已同意授权无需重复申请授权）/ `10024003`（不合法的 appid）/ `10024014`（该用户不存在对应视频号）/ `10024018`（未绑定小程序）/ `10024019`（不合法的 openid）/ `10024031`（该达人平台未开通商户号）。
- 官方变更日志：2025-12-19 新增错误码 10024031、10024003。

### 31. `GetHeadSupplierWindowAuthStatusAsync` — 获取带货达人橱窗授权状态（`headsupplier/windowauth/status/get`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/windowauth/status/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest` — 请求（`openfinderid` / `talent_appid` / `opentalentid` 三选一，另可传官方标注即将废弃的 `finder_id`）。
- **返回**：`ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse` — 授权状态（`window_auth_status`：0 未授权 / 1 已授权）与对应的 `openfinderid` / `opentalentid` / 达人昵称与头像。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowauthstatus.html（2026-10-17 逐页核验）
- 官方适用范围：「联盟带货机构」账号类型；支持云调用 `channels.ec.league.headsupplier.windowauth.status.get`；**不支持第三方平台调用**。
- 官方业务限制：①授权状态是 `number`（0/1）而非 boolean，字段名带 `status` ⇒ 建常量类； / ②返回的达人昵称 / 头像（`nickname` / `head_img_url`）在代码示例中未出现，但参数表列出 ⇒ 以参数表为权威； / ③本接口是 `windowauth/get` 的配套轮询面。
- 官方错误码：`10024000`（参数错误）/ `10024001`（不合法的 finder_id）/ `10024012`（不合法的参数，查询不到对应账号，2025-12-22 新增）。
- 官方变更日志：2025-12-22 新增错误码 10024012。

### 32. `GetHeadSupplierSubscribeProductsAsync` — 获取订阅的商品列表（`headsupplier/subscription/getsubscribe`，机构视角的主动 + 被动订阅商品）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/subscription/getsubscribe`

- **请求体**：`ChannelsLeagueHeadSupplierGetSubscribeProductsRequest` — 请求（`page_size` 必填且不超过 `1000`；`filter` / `next_key` / `need_total_num` 选填）。
- **返回**：`ChannelsLeagueHeadSupplierGetSubscribeProductsResponse` — 订阅商品信息列表（`subscribe_info_list`：`product_id` + `shop_appid`）、`total_num` 与翻页游标。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_getsubscribeproduct.html（2026-10-17 逐页核验）
- 官方业务限制：①订阅商品分两类——**主动订阅**（调用 `SubscribeHeadSupplierProductAsync` 订阅的商品） / 与**被动订阅**（优选联盟后台合作商品中所有已合作商品，含商家定向 / 商家公开 / 机构定向 / 机构普通四种计划）； / ②`page_size` 上限 `1000`（本域其余列表接口上限为 20 / 100，勿套用）； / ③官方 `filter` 子表把 `product_id` / `product_name` / `appid` 三行都标「必填 = 是」， / 实为筛选条件 ⇒ 本仓全部建模为可空； / ④`need_total_num` 的官方说明原文为「否需要返回符合条件的记录总数」（「是」字缺字，照录）。
- 官方返回体自相矛盾（照录）：`subscribe_info_list` 参数表类型为 `objarray`（对象数组）， / 但代码示例把它写成**单个对象** ⇒ 以参数表为权威建模为列表；`has_more` 参数表为 `boolean`、 / 示例值为字符串 `"false"` ⇒ 以参数表为准。

### 33. `SubscribeHeadSupplierProductAsync` — 商品订阅（`headsupplier/subscription/subscribe`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/subscription/subscribe`

- **请求体**：`ChannelsLeagueHeadSupplierSubscribeProductRequest` — 请求（`product_id` 必填，官方示例值 `10000207528319`）。
- **返回**：`ChannelsResponse` — 仅含 `errcode` / `errmsg` 的通用响应（官方返回参数表无业务字段）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_subscribeproduct.html（2026-10-17 逐页核验）
- 官方业务限制：订阅后，商品的**基础信息或计划信息**发生变化时会通过**回调**通知机构 / ⇒ 本端点与订阅回调是配套面；取消订阅见 `UnsubscribeHeadSupplierProductAsync`， / 两端点请求体字段集完全同形（单 `product_id`）⇒ 共用一支请求 DTO。
- 官方错误码：`10024000`（其他参数错误）/ `10024009`（不合法的商品 ID 或未加入团长推广）。
- 官方变更日志：2025-11-21 修改错误码 `10024009`。

### 34. `UnsubscribeHeadSupplierProductAsync` — 取消商品订阅（`headsupplier/subscription/unsubscribe`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/subscription/unsubscribe`

- **请求体**：`ChannelsLeagueHeadSupplierSubscribeProductRequest` — 请求（`product_id` 必填，要取消订阅的商品 ID）。
- **返回**：`ChannelsResponse` — 仅含 `errcode` / `errmsg` 的通用响应（官方返回参数表无业务字段）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_unsubscribeproduct.html（2026-10-17 逐页核验）
- 官方请求示例的 JSON 结尾多一个逗号（`"product_id": 10000207528319,`），为官方笔误 ⇒ / 字段集以参数表为准；与 `SubscribeHeadSupplierProductAsync` 同形，共用请求 DTO。

### 35. `AddHeadSupplierSubItemAsync` — 配置达人佣金率（`headsupplier/subitem/add`，为「机构分配达人佣金」的商品分配达人佣金）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/subitem/add`

- **请求体**：`ChannelsLeagueHeadSupplierAddSubItemRequest` — 请求（`cooperative_item_id` 必填 + `ratio` / `ratio_ext` 二选一）。
- **返回**：`ChannelsLeagueHeadSupplierAddSubItemResponse` — 机构配置达人佣金计划 ID（`sub_item_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_addsubitem.html（2026-10-17 逐页核验）
- 官方业务限制：①**两个佣金率字段二选一**——`ratio` 为整数佣金率 `[0, 90]%`、 / `ratio_ext` 为精确佣金率 `[0, 900000]` 且 `10000 = 1%`（配置含小数点佣金率时使用）； / 官方参数表把两者都标「必填 = 是」⇒ 本仓建模为可空并在守卫 CLG5 锁定两者并存； / ②分配完成后可把链接给到达人带货（列表见 `GetHeadSupplierSubItemListAsync`）； / ③「商家指定达人佣金」的商品不走本接口，直接调获取商品推广参数详情。
- 官方错误码：`10024000`（参数错误）/ `10024025`（商品计划不在上架中）/ `10024026`（自由佣金计划比例重复）/ `10024027`（佣金比例超过限制）。
- 官方变更日志：2026-03-23 `ratio_ext` 字段描述更新；2025-11-24 新增错误码 `10024025`、`10024026` 等。

### 36. `GetHeadSupplierSubItemListAsync` — 获取机构配置达人佣金计划列表（`headsupplier/subitem/list/get`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/subitem/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetSubItemListRequest` — 请求（`cooperative_item_id` / `page_size`（最大 20）/ `next_key` / `need_qr_code`）。
- **返回**：`ChannelsLeagueHeadSupplierGetSubItemListResponse` — 达人佣金计划列表（`list`：计划 ID / 计划链接 / 状态 / 三档佣金率 / 推广二维码短链）与下一页游标 `next_key`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getsubitemlist.html（2026-10-17 逐页核验）
- 官方业务限制：①**返回体无 `has_more`**（本域多数列表接口有）⇒ 翻页只靠 `next_key`， / 守卫 CLG5 已锁该字段不得出现；②`page_size` 最大 `20`； / ③`next_key` 官方标「必填 = 是」并注明第一页传空串 ⇒ 建模为可空字符串； / ④`need_qr_code` 官方原文提示「如不确定需不需要则不填，因为会增加耗时」⇒ 建模为可空； / ⑤`qr_code_url` 是推广二维码**对应短链**，官方要求自行转成二维码后方可扫码。
- 官方变更日志：2026-03-23 `ratio_ext` 字段描述更新；2026-03-09 新增 `need_qr_code` 请求字段与 `qr_code_url` 返回字段。

### 37. `AddHeadSupplierHelpSellItemAsync` — 机构给帮卖机构供货（`headsupplier/secondheadsupplieritem/add`，创建机构管理页面的「机构推广」）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/secondheadsupplieritem/add`

- **请求体**：`ChannelsLeagueHeadSupplierAddHelpSellItemRequest` — 请求（`cooperative_item_id` 与 `head_supplier_item_link` 必填其一 + `plan_type` + `service_ratio`，定向计划另填 `to_head_supplier_appid` / 起止时间）。
- **返回**：`ChannelsResponse` — 仅含 `errcode` / `errmsg` 的通用响应（官方返回参数表无业务字段，示例 `errmsg` 为 `success` 而非 `ok`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_add_secondheadsupplier_item.html（2026-10-17 逐页核验）
- 官方业务限制：①**仅商家定向的合作商品**才支持通过本接口提供给帮卖机构，商家公开 / 机构定向 / 机构公开均不允许（官方注意事项原文）； / ②`plan_type`：`1` 定向计划 / `2` 公开计划，官方特别提示**设置定向计划之前要先设置公开计划**； / ③`service_ratio` 是帮卖服务费率，单位为**百万分之 N**（`10000` 表示 1%），且数值需**能被 1000 整除** / —— 与 `AddHeadSupplierSubItemAsync` 的 `ratio_ext`（万分之 N）**量纲不同**，勿互相套用； / ④`cooperative_item_id` 填写时官方声明会**忽略** `head_supplier_item_link`； / ⑤`start_time` / `end_time` 为秒级时间戳，仅定向计划可填。
- 官方错误码：`10024000`（参数错误，解决方案「检查参数」）/ `10024003`（不合法的 appid）。
- 官方变更日志：2025-12-30 更新 `plan_type` 字段描述；2025-12-19 新增本接口、新增错误码 `10024003`。

### 38. `UpdateHeadSupplierHelpSellItemAsync` — 机构更新或删除已添加的推广计划（`headsupplier/secondheadsupplieritem/update`，对应后台「机构推广」改佣金率或结束推广）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/secondheadsupplieritem/update`

- **请求体**：`ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest` — 请求（`plan_type` + `update_type` 必填，商品定位键 `cooperative_item_id` / `head_supplier_item_link` 必填其一，更新佣金率时填 `service_ratio`）。
- **返回**：`ChannelsResponse` — 仅含 `errcode` / `errmsg` 的通用响应（官方返回参数表无业务字段）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_update_secondheadsupplier_item.html（2026-10-17 逐页核验）
- 官方业务限制：①`update_type` 为**操作类别**而非布尔：`1` 更新佣金率 / `2` 结束推广， / 官方声明 `service_ratio` 仅在「更新」情况下必填 ⇒ 建模为可空； / ②`to_head_supplier_appid` 官方声明「定向计划的情况必填」⇒ 建模为可空； / ③本接口是 `AddHeadSupplierHelpSellItemAsync` 的改/撤面，字段集**不含** 起止时间 / （与添加接口的差异点，守卫 CLG5 已锁）。
- 官方错误码：`10024000`（参数错误，解决方案「检查参数」）。
- 官方变更日志：2025-11-28 新增本 API。

### 39. `GetHeadSupplierClueAsync` — 推客机构获取内容推广的自营商品订单详情（`headsupplier/clue/get`，即「线索订单」详情）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/clue/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetClueRequest` — 请求（`order_id` 与 `sku_id` 均为 `string` 且必填）。
- **返回**：`ChannelsLeagueHeadSupplierGetClueResponse` — 线索订单结构（官方根字段名 `commssion_clue`）：订单号 / skuid / 起止时间戳 + `clue_detail`（商家 / 商品 / 订单 / 推客 / 买家信息）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/clue/api_getclue.html（2026-10-17 逐页核验）
- 官方业务语义：直播间等内容分销场景存在挂**自营商品**的情况，该情况**不给带货机构分佣**， / 平台改为给出**销售订单线索**，达人和机构自行协商 ⇒ 本族与佣金单（`GetHeadSupplierOrderAsync`）是两套订单面。
- 官方文档自相矛盾（照录，均以参数表为权威）：①根字段名官方拼作 `commssion_clue`（少一个 `i`）， / 属性名按正确拼写命名、`JsonPropertyName` 照抄官方；②**参数表无根 `status`**，代码示例却有 / `"status": 20` ⇒ 不建模（订单状态只按参数表落 `clue_detail.order_info.order_status`）； / ③参数表买家信息键为 `buyer_info`、示例却写 `buyer` ⇒ 以参数表为准； / ④**请求侧** `sku_id` 是 `string`，**返回体** `commssion_clue.sku_id` 参数表却标 `number`（示例又写字符串）⇒ 返回侧按参数表建为 `long`； / ⑤官方「注意事项」给出订单状态枚举表（`10/20/21/30/100/200/250`），标题却写作「status 订单状态」。
- 官方错误码：`10024000`（参数错误，请开发人员检查传递参数是否正确）/ `10024006`（不存在该线索订单）。

### 40. `GetHeadSupplierClueListAsync` — 推客机构获取内容推广的自营商品订单列表（`headsupplier/clue/list/get`，只回线索单键）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/clue/list/get`

- **请求体**：`ChannelsLeagueHeadSupplierGetClueListRequest` — 请求（`create_time_range`（必填）+ `page_size`（不超过 100），`appid` / `sharer_appid` / `next_key` 选填）。
- **返回**：`ChannelsLeagueHeadSupplierGetClueListResponse` — 线索订单列表项（`list`：`order_id` + `sku_id`）与翻页上下文 `next_key` / `has_more`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/clue/api_getcluelist.html（2026-10-17 逐页核验）
- 官方业务限制：①**时间区间自 2026-05-22 起改为必填**（官方变更日志原文「调整查询时间区间为必填」） / ⇒ 本仓把 `create_time_range` 建模为可空引用类型但文档注释标明必填，且不为其造校验（保持 SDK 透传）； / ②列表项 `order_id` 参数表类型为 `number`，而详情接口请求侧为 `string`、示例又写字符串 / ⇒ 按参数表建为 `long`，与佣金单列表的 `string` `order_id` **不同形**（守卫 CLG5 锁定差异，防误共用）； / ③时间区间形态与本域其它接口共用 `ChannelsLeagueHeadSupplierTimeRange`； / ④详情须用 `GetHeadSupplierClueAsync` 逐单拉取。
- 官方错误码：`10024000`（参数错误，请开发人员检查传递参数是否正确）/ `10024003`（不合法的 appid）。
- 官方变更日志：2026-05-22 调整查询时间区间为必填。

### 41. `GetHeadSupplierLiveProtectionQrCodeAsync` — 获取直播大场保护的提报二维码（`headsupplier/liveprotection/getqrcode`）。

- **声明接口**：[IChannelsLeagueService](./IChannelsLeagueService.cs)
- **官方契约**：`POST /channels/ec/league/headsupplier/liveprotection/getqrcode`
- **返回**：`ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse` — 提报二维码 URL（`qrcode_url`，官方示例为 `https://mp.weixin.qq.com/`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliveprotectionqrcode.html（2026-10-17 逐页核验）
- 官方契约：**POST** + 请求体参数表为**无**（示例请求体为 `{}`） / ⇒ SDK 不造请求体 DTO，守卫 CLG2 锁定本端点不得携带 `[Body]`。
- 大场保护机制（官方原文口径）：针对高 GMV 的重点直播场次，平台对非安全底线的违规行为实行 / **「先直播后处置」**策略，直播期间暂缓执行处罚、结束后统一处理。
- 官方使用限制：①机构**近 30 天 GMV** 符合激励条件（统计数据 T+1 生效）—— 与错误码 `10024030` / 的「近 7 天推客带货 GMV」**表述不一致**，两处均照录；②每个自然月内单个达人仅有 **1 次**报备资格， / 已通过高星达人权益通道报备的不可重复使用本接口；③提报达人绑定的视频号需为合作机构**近 7 天 GMV 前 2 名**。
- 官方错误码：`10024000`（参数错误）/ `10024029`（二维码生成失败）/ `10024030`（近 7 天推客带货 GMV 符合激励条件的机构才能使用提报能力）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/League`](../../../Mud.Wechat.Channels/League/) 内共 **149** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **149** 个，`SerializerClassName` = `League`。

AOT 源生成上下文：[`LeagueJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/LeagueJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsGetPromoterResponse` | 应答 | [League/Promoter/ChannelsGetPromoterResponse.cs](../../../Mud.Wechat.Channels/League/Promoter/ChannelsGetPromoterResponse.cs) | `talent` |
| `ChannelsGetPromoterRequest` | 请求体 | [League/Promoter/ChannelsGetPromoterRequest.cs](../../../Mud.Wechat.Channels/League/Promoter/ChannelsGetPromoterRequest.cs) | `talent_appid`, `talent_search_key` |
| `ChannelsGetPromoterListResponse` | 应答 | [League/Promoter/ChannelsGetPromoterListResponse.cs](../../../Mud.Wechat.Channels/League/Promoter/ChannelsGetPromoterListResponse.cs) | `total_num`, `continue_flag`, `list` |
| `ChannelsGetPromoterListRequest` | 请求体 | [League/Promoter/ChannelsGetPromoterListRequest.cs](../../../Mud.Wechat.Channels/League/Promoter/ChannelsGetPromoterListRequest.cs) | `page_index`, `page_size` |
| `ChannelsGetTalentProductDataListResponse` | 应答 | [League/Talent/ChannelsGetTalentProductDataListResponse.cs](../../../Mud.Wechat.Channels/League/Talent/ChannelsGetTalentProductDataListResponse.cs) | `total_num`, `continue_flag`, `list` |
| `ChannelsGetTalentProductDataListRequest` | 请求体 | [League/Talent/ChannelsGetTalentProductDataListRequest.cs](../../../Mud.Wechat.Channels/League/Talent/ChannelsGetTalentProductDataListRequest.cs) | `get_data_type`, `page_index`, `page_size`, `talent_appid` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsUpdateTalentRequest` | 请求体 | [League/Talent/ChannelsUpdateTalentRequest.cs](../../../Mud.Wechat.Channels/League/Talent/ChannelsUpdateTalentRequest.cs) | `talent_appid`, `upd_talent_type` |
| `ChannelsBatchAddLeagueItemResponse` | 应答 | [League/Item/ChannelsBatchAddLeagueItemResponse.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsBatchAddLeagueItemResponse.cs) | `result_info_list` |
| `ChannelsBatchAddLeagueItemRequest` | 请求体 | [League/Item/ChannelsBatchAddLeagueItemRequest.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsBatchAddLeagueItemRequest.cs) | `type`, `list`, `begin_time`, `end_time`, `is_forerver`, `talent_appids` |
| `ChannelsGetLeagueItemResponse` | 应答 | [League/Item/ChannelsGetLeagueItemResponse.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsGetLeagueItemResponse.cs) | `item` |
| `ChannelsGetLeagueItemRequest` | 请求体 | [League/Item/ChannelsGetLeagueItemRequest.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsGetLeagueItemRequest.cs) | `type`, `product_id`, `promotion_plan_id` |
| `ChannelsBatchAddHeadSupplierResponse` | 应答 | [League/HeadSupplier/ChannelsBatchAddHeadSupplierResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/ChannelsBatchAddHeadSupplierResponse.cs) | `result_info_list` |
| `ChannelsBatchAddHeadSupplierItemRequest` | 请求体 | [League/HeadSupplier/ChannelsBatchAddHeadSupplierItemRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/ChannelsBatchAddHeadSupplierItemRequest.cs) | `headsupplier_appid`, `list`, `begin_time`, `end_time`, `is_forerver` |
| `ChannelsGetLeagueItemListResponse` | 应答 | [League/Item/ChannelsGetLeagueItemListResponse.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsGetLeagueItemListResponse.cs) | `items`, `has_more`, `total_num` |
| `ChannelsGetLeagueItemListRequest` | 请求体 | [League/Item/ChannelsGetLeagueItemListRequest.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsGetLeagueItemListRequest.cs) | `page_size`, `page_index`, `product_id`, `need_total_num` |
| `ChannelsUpdateLeagueItemResponse` | 应答 | [League/Item/ChannelsUpdateLeagueItemResponse.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsUpdateLeagueItemResponse.cs) | `promotion_plan_id`, `fail_talent_appids` |
| `ChannelsUpdateLeagueItemRequest` | 请求体 | [League/Item/ChannelsUpdateLeagueItemRequest.cs](../../../Mud.Wechat.Channels/League/Item/ChannelsUpdateLeagueItemRequest.cs) | `type`, `product_id`, `promotion_plan_id`, `operate_type`, `ratio`, `exclusive_info` |
| `ChannelsListSpuPromotionPlansResponse` | 应答 | [League/PromotionPlan/ChannelsListSpuPromotionPlansResponse.cs](../../../Mud.Wechat.Channels/League/PromotionPlan/ChannelsListSpuPromotionPlansResponse.cs) | `total_num`, `continue_flag`, `list` |
| `ChannelsListSpuPromotionPlansRequest` | 请求体 | [League/PromotionPlan/ChannelsListSpuPromotionPlansRequest.cs](../../../Mud.Wechat.Channels/League/PromotionPlan/ChannelsListSpuPromotionPlansRequest.cs) | `product_id`, `page_index`, `page_size`, `promotion_plan_type` |
| `ChannelsListSpuPromotionTalentsResponse` | 应答 | [League/PromotionPlan/ChannelsListSpuPromotionTalentsResponse.cs](../../../Mud.Wechat.Channels/League/PromotionPlan/ChannelsListSpuPromotionTalentsResponse.cs) | `total_num`, `continue_flag`, `list` |
| `ChannelsListSpuPromotionTalentsRequest` | 请求体 | [League/PromotionPlan/ChannelsListSpuPromotionTalentsRequest.cs](../../../Mud.Wechat.Channels/League/PromotionPlan/ChannelsListSpuPromotionTalentsRequest.cs) | `product_id`, `page_index`, `page_size`, `promotion_plan_id`, `talent_appid` |
| `ChannelsLeagueHeadSupplierGetBalanceResponse` | 应答 | [League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetBalanceResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetBalanceResponse.cs) | `available_amount`, `pending_amount` |
| `ChannelsGetFundsFlowListResponse` | 应答 | [Funds/Flow/ChannelsGetFundsFlowListResponse.cs](../../../Mud.Wechat.Channels/Funds/Flow/ChannelsGetFundsFlowListResponse.cs) | `flow_ids`, `has_more`, `next_key` |
| `ChannelsLeagueHeadSupplierGetFundsFlowListRequest` | 请求体 | [League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetFundsFlowListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetFundsFlowListRequest.cs) | `page`, `page_size`, `next_key`, `start_time`, `end_time`, `funds_type` |
| `ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse` | 应答 | [League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Funds/ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse.cs) | `funds_flow` |
| `ChannelsGetFundsFlowDetailRequest` | 请求体 | [Funds/Flow/ChannelsGetFundsFlowDetailRequest.cs](../../../Mud.Wechat.Channels/Funds/Flow/ChannelsGetFundsFlowDetailRequest.cs) | `flow_id` |
| `ChannelsLeagueHeadSupplierAddCooperativeItemResponse` | 应答 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierAddCooperativeItemResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierAddCooperativeItemResponse.cs) | `cooperative_item_id` |
| `ChannelsLeagueHeadSupplierAddCooperativeItemRequest` | 请求体 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierAddCooperativeItemRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierAddCooperativeItemRequest.cs) | `head_supplier_item_link`, `hidden_flag` |
| `ChannelsLeagueHeadSupplierGetCooperativeItemListResponse` | 应答 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetCooperativeItemListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetCooperativeItemListResponse.cs) | `list`, `next_key` |
| `ChannelsLeagueHeadSupplierGetCooperativeItemListRequest` | 请求体 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetCooperativeItemListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetCooperativeItemListRequest.cs) | `commission_type`, `listing_time_range`, `page_size`, `next_key` |
| `ChannelsLeagueHeadSupplierGetItemPromotionDetailResponse` | 应答 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetItemPromotionDetailResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetItemPromotionDetailResponse.cs) | `item` |
| `ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest` | 请求体 | [League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Item/ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest.cs) | `head_supplier_item_link` |
| `ChannelsLeagueHeadSupplierGetSelectionProductsResponse` | 应答 | [League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetSelectionProductsResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetSelectionProductsResponse.cs) | `product_list`, `next_key` |
| `ChannelsLeagueHeadSupplierGetSelectionProductsRequest` | 请求体 | [League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetSelectionProductsRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetSelectionProductsRequest.cs) | `plan_type`, `spu_item_condition`, `category`, `keyword`, `spu_source`, `commission_type`, `page_size`, `next_key` |
| `ChannelsLeagueHeadSupplierGetProductDetailResponse` | 应答 | [League/HeadSupplier/Product/ChannelsLeagueHeadSupplierGetProductDetailResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Product/ChannelsLeagueHeadSupplierGetProductDetailResponse.cs) | `item` |
| `ChannelsLeagueHeadSupplierGetProductDetailRequest` | 请求体 | [League/HeadSupplier/Product/ChannelsLeagueHeadSupplierGetProductDetailRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Product/ChannelsLeagueHeadSupplierGetProductDetailRequest.cs) | `shop_appid`, `product_id` |
| `ChannelsLeagueHeadSupplierGetProductActivityResponse` | 应答 | [League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetProductActivityResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetProductActivityResponse.cs) | `has_activity`, `product_activity_list` |
| `ChannelsLeagueHeadSupplierGetProductActivityRequest` | 请求体 | [League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetProductActivityRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Selection/ChannelsLeagueHeadSupplierGetProductActivityRequest.cs) | `product_id`, `shop_appid` |
| `ChannelsLeagueHeadSupplierGetShopResponse` | 应答 | [League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopResponse.cs) | `shop_detail` |
| `ChannelsLeagueHeadSupplierGetShopRequest` | 请求体 | [League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopRequest.cs) | `appid` |
| `ChannelsLeagueHeadSupplierGetShopListResponse` | 应答 | [League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopListResponse.cs) | `shop_list`, `next_key`, `has_more` |
| `ChannelsLeagueHeadSupplierGetShopListRequest` | 请求体 | [League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierGetShopListRequest.cs) | `page_size`, `next_key` |
| `ChannelsLeagueHeadSupplierUpdateRelationRequest` | 请求体 | [League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierUpdateRelationRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Shop/ChannelsLeagueHeadSupplierUpdateRelationRequest.cs) | `object_type`, `to_shop_appid`, `to_head_supplier_appid`, `hs_update_type`, `shop_update_type` |
| `ChannelsLeagueHeadSupplierGetOrderListResponse` | 应答 | [League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderListResponse.cs) | `list`, `next_key`, `has_more` |
| `ChannelsLeagueHeadSupplierGetOrderListRequest` | 请求体 | [League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderListRequest.cs) | `appid`, `finder_id`, `page_size`, `next_key`, `create_time_range`, `update_time_range`, `order_id`, `openfinderid`, `sharer_appid` |
| `ChannelsLeagueHeadSupplierGetOrderResponse` | 应答 | [League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderResponse.cs) | `commssion_order` |
| `ChannelsLeagueHeadSupplierGetOrderRequest` | 请求体 | [League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Order/ChannelsLeagueHeadSupplierGetOrderRequest.cs) | `order_id`, `sku_id` |
| `ChannelsLeagueHeadSupplierAddWindowRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierAddWindowRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierAddWindowRequest.cs) | `finder_id`, `product_id`, `openfinderid`, `is_hide_for_window`, `product_link`, `opentalentid` |
| `ChannelsLeagueHeadSupplierGetWindowListResponse` | 应答 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowListResponse.cs) | `list`, `next_offset`, `has_more`, `total_num` |
| `ChannelsLeagueHeadSupplierGetWindowListRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowListRequest.cs) | `finder_id`, `offset`, `page_size`, `need_total_num`, `openfinderid`, `is_get_all`, `opentalentid` |
| `ChannelsLeagueHeadSupplierGetWindowProductDetailResponse` | 应答 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowProductDetailResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowProductDetailResponse.cs) | `product_detail` |
| `ChannelsLeagueHeadSupplierGetWindowProductDetailRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowProductDetailRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowProductDetailRequest.cs) | `appid`, `finder_id`, `product_id`, `openfinderid`, `product_window_id`, `opentalentid` |
| `ChannelsLeagueHeadSupplierRemoveWindowRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierRemoveWindowRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierRemoveWindowRequest.cs) | `finder_id`, `product_id`, `openfinderid`, `product_window_id`, `opentalentid` |
| `ChannelsLeagueHeadSupplierGetWindowAuthResponse` | 应答 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthResponse.cs) | `auth_info`, `openfinderid`, `opentalentid` |
| `ChannelsLeagueHeadSupplierGetWindowAuthRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthRequest.cs) | `finder_id`, `openid`, `talent_appid` |
| `ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse` | 应答 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse.cs) | `window_auth_status`, `openfinderid`, `opentalentid`, `nickname`, `head_img_url` |
| `ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest` | 请求体 | [League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Window/ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest.cs) | `finder_id`, `openfinderid`, `talent_appid`, `opentalentid` |
| `ChannelsLeagueHeadSupplierGetSubscribeProductsResponse` | 应答 | [League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierGetSubscribeProductsResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierGetSubscribeProductsResponse.cs) | `subscribe_info_list`, `total_num`, `next_key`, `has_more` |
| `ChannelsLeagueHeadSupplierGetSubscribeProductsRequest` | 请求体 | [League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierGetSubscribeProductsRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierGetSubscribeProductsRequest.cs) | `filter`, `page_size`, `next_key`, `need_total_num` |
| `ChannelsLeagueHeadSupplierSubscribeProductRequest` | 请求体 | [League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierSubscribeProductRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Subscription/ChannelsLeagueHeadSupplierSubscribeProductRequest.cs) | `product_id` |
| `ChannelsLeagueHeadSupplierAddSubItemResponse` | 应答 | [League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierAddSubItemResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierAddSubItemResponse.cs) | `sub_item_id` |
| `ChannelsLeagueHeadSupplierAddSubItemRequest` | 请求体 | [League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierAddSubItemRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierAddSubItemRequest.cs) | `cooperative_item_id`, `ratio_ext`, `ratio` |
| `ChannelsLeagueHeadSupplierGetSubItemListResponse` | 应答 | [League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierGetSubItemListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierGetSubItemListResponse.cs) | `list`, `next_key` |
| `ChannelsLeagueHeadSupplierGetSubItemListRequest` | 请求体 | [League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierGetSubItemListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/SubItem/ChannelsLeagueHeadSupplierGetSubItemListRequest.cs) | `cooperative_item_id`, `page_size`, `next_key`, `need_qr_code` |
| `ChannelsLeagueHeadSupplierAddHelpSellItemRequest` | 请求体 | [League/HeadSupplier/HelpSell/ChannelsLeagueHeadSupplierAddHelpSellItemRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/HelpSell/ChannelsLeagueHeadSupplierAddHelpSellItemRequest.cs) | `cooperative_item_id`, `head_supplier_item_link`, `plan_type`, `service_ratio`, `to_head_supplier_appid`, `start_time`, `end_time` |
| `ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest` | 请求体 | [League/HeadSupplier/HelpSell/ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/HelpSell/ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest.cs) | `plan_type`, `update_type`, `cooperative_item_id`, `head_supplier_item_link`, `service_ratio`, `to_head_supplier_appid` |
| `ChannelsLeagueHeadSupplierGetClueResponse` | 应答 | [League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueResponse.cs) | `commssion_clue` |
| `ChannelsLeagueHeadSupplierGetClueRequest` | 请求体 | [League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueRequest.cs) | `order_id`, `sku_id` |
| `ChannelsLeagueHeadSupplierGetClueListResponse` | 应答 | [League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueListResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueListResponse.cs) | `list`, `next_key`, `has_more` |
| `ChannelsLeagueHeadSupplierGetClueListRequest` | 请求体 | [League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueListRequest.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/Clue/ChannelsLeagueHeadSupplierGetClueListRequest.cs) | `appid`, `sharer_appid`, `create_time_range`, `page_size`, `next_key` |
| `ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse` | 应答 | [League/HeadSupplier/LiveProtection/ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse.cs](../../../Mud.Wechat.Channels/League/HeadSupplier/LiveProtection/ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse.cs) | `qrcode_url` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.League;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddLeagueApi());

var service = app.Services.GetRequiredService<IChannelsLeagueService>();
var ct = CancellationToken.None;
var response = await service.GetHeadSupplierBalanceAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/league/headsupplier/funds/balance/get");
// 占位值仅用于编译示意；业务值须按 IChannelsLeagueService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetHeadSupplierBalanceAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsLeagueService](./IChannelsLeagueService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsLeagueContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLeagueContractGuards.cs)、[`ChannelsPromoterContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsPromoterContractGuards.cs)、[`ChannelsTalentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs) |
| 守卫中的代表性测试方法 | `LeagueEndpoints_ShouldMatchOfficialRoutes`、`LeagueEndpoints_ShouldAllBePost_WithBodyRules`、`LeagueInterface_ShouldBindChannelsToken_AndRegisterUnderLeagueGroup`、`LeagueDataModels_ShouldBeRegisteredInJsonContext`、`LeagueDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`LeagueEnumConstants_ShouldLockOfficialLiteralValues`、`RemovedLeagueRoutes_ShouldHaveZeroDeclaration`、`PromoterEndpoints_ShouldMatchOfficialRoutes`、`PromoterEndpoints_ShouldAllBePost_WithSingleBody_AndResponseReuse`、`PromoterInterface_ShouldBindChannelsToken_AndRegisterUnderPromoterGroup`、`PromoterDataModels_ShouldBeRegisteredInJsonContext`、`PromoterDataModels_ShouldLockOfficialFieldsAndSharedDtos` … |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/League`](../../../Mud.Wechat.Channels/League/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsLeagueContractGuards.cs`、`ChannelsPromoterContractGuards.cs`、`ChannelsTalentContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

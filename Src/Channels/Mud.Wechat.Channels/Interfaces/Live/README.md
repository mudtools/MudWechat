# Live 域 · 直播

视频号助手（channels 生态）「直播」域 SDK（12 端点：直播记录 / 预约记录 / 留资组件管理 /
留资服务的直播数据 / 直播大屏）。 接口：[IChannelsLiveService](./IChannelsLiveService.cs)；注册组 `Live`；模块枚举 `ChannelsModule.Live`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsLiveContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLiveContractGuards.cs)、[ChannelsDeliveryContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsDeliveryContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/Live`](../../../Mud.Wechat.Channels/Live/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsLiveService = 12；按 HTTP 特性实计 **12** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsLiveService` | `GetFinderLiveRecordListAsync` | POST | `/channels/ec/finderlive/getfinderliverecordlist` | `AccessToken`（Query），Query `access_token` | 视频号获取当前的直播记录（`getfinderliverecordlist`）。 |
| 2 | `IChannelsLiveService` | `GetFinderLiveNoticeRecordListAsync` | POST | `/channels/ec/finderlive/getfinderlivenoticerecordlist` | `AccessToken`（Query），Query `access_token` | 视频号获取当前的直播预约记录（`getfinderlivenoticerecordlist`）。 |
| 3 | `IChannelsLiveService` | `GetLeadsInfoByRequestIdAsync` | POST | `/channels/leads/get_leads_info_by_request_id` | `AccessToken`（Query），Query `access_token` | 按直播场次获取留资信息详情（`get_leads_info_by_request_id`）。 |
| 4 | `IChannelsLiveService` | `GetLeadsInfoByComponentIdAsync` | POST | `/channels/leads/get_leads_info_by_component_id` | `AccessToken`（Query），Query `access_token` | 按时间获取留资信息详情（`get_leads_info_by_component_id`）。 |
| 5 | `IChannelsLiveService` | `GetLeadsRequestIdListAsync` | POST | `/channels/leads/get_leads_request_id` | `AccessToken`（Query），Query `access_token` | 获取留资 request_id 列表详情（`get_leads_request_id`）。 |
| 6 | `IChannelsLiveService` | `GetLeadsComponentPromoteRecordAsync` | POST | `/channels/leads/get_leads_component_promote_record` | `AccessToken`（Query），Query `access_token` | 获取留资组件直播推广记录信息详情（`get_leads_component_promote_record`）。 |
| 7 | `IChannelsLiveService` | `GetLeadsComponentIdListAsync` | POST | `/channels/leads/get_leads_component_id` | `AccessToken`（Query），Query `access_token` | 获取留资组件 Id 列表详情（`get_leads_component_id`）。 |
| 8 | `IChannelsLiveService` | `GetFinderAttrByAppidAsync` | POST | `/channels/finderlive/get_finder_attr_by_appid` | `AccessToken`（Query），Query `access_token` | 获取视频号账号信息（`get_finder_attr_by_appid`）。 |
| 9 | `IChannelsLiveService` | `GetFinderLiveDataListAsync` | POST | `/channels/finderlive/get_finder_live_data_list` | `AccessToken`（Query），Query `access_token` | 获取留资直播数据详情（`get_finder_live_data_list`）。 |
| 10 | `IChannelsLiveService` | `GetFinderLiveLeadsDataAsync` | POST | `/channels/finderlive/get_finder_live_leads_data` | `AccessToken`（Query），Query `access_token` | 获取账号收集的留资数量（`get_finder_live_leads_data`）。 |
| 11 | `IChannelsLiveService` | `GetLiveDashboardListAsync` | POST | `/channels/livedashboard/getlivelist` | `AccessToken`（Query），Query `access_token` | 获取直播大屏直播列表（`getlivelist`，只返回电商直播间的直播列表）。 |
| 12 | `IChannelsLiveService` | `GetLiveDashboardDataAsync` | POST | `/channels/livedashboard/getlivedata` | `AccessToken`（Query），Query `access_token` | 获取直播大屏数据（`getlivedata`，按直播唯一 ID 拉取单场大屏全量数据）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「罗盘数据」域严格区分**：本域承载视频号侧直播原始数据（`/channels/ec/finderlive/*`、
`/channels/finderlive/*`）、留资组件（`/channels/leads/*`）与直播大屏
（`/channels/livedashboard/*`）；商家/达人经营分析指标走 `IChannelsCompassService`。

**多前缀并存（官方原文，照抄不得改写）**：直播记录/预约走 `/channels/ec/finderlive/*`（2 端点）、
留资服务的直播数据走 `/channels/finderlive/*`（3 端点，无 `/ec` 段）、留资组件走
`/channels/leads/*`（5 端点）、直播大屏走 `/channels/livedashboard/*`（2 端点）。

**适用范围**：全部端点仅「视频号助手」账号类型可调用（官方适用范围原文），且须在服务器端调用、
均不支持云调用；第三方平台代调用权限集 id 见各端点 remarks。

**MUD005 已知接受风险**：视频号助手官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 12 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 8 次）官方错误码：`-2`（请求的参数错误）/ `1`（请求的信息不存在）/ `48001`（接口权限未开启）/ `100000`（没有权限访问）。

### 1. `GetFinderLiveRecordListAsync` — 视频号获取当前的直播记录（`getfinderliverecordlist`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/ec/finderlive/getfinderliverecordlist`
- **返回**：`ChannelsLiveGetFinderLiveRecordListResponse` — 直播记录列表（`live_list`：直播 id / 描述 / 封面 / 开播视频号昵称与头像）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/live/api_getfinderliverecordlist
- 官方契约：**POST** + 空请求体（官方示例请求体为 `{}`）。
- 官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代调用权限集 id 为 133、160、177；仅「视频号助手」账号类型可调用。
- 官方错误码：`48001`（接口权限未开启，需前往视频号助手-开放能力-开放场景打开开关）。

### 2. `GetFinderLiveNoticeRecordListAsync` — 视频号获取当前的直播预约记录（`getfinderlivenoticerecordlist`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/ec/finderlive/getfinderlivenoticerecordlist`
- **返回**：`ChannelsLiveGetFinderLiveNoticeRecordListResponse` — 直播预约记录列表（`live_notice_list`：预约 id / 状态 / 状态说明 / 开始时间 / 描述 / 视频号昵称与头像）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/live/api_getfinderlivenoticerecordlist
- 官方契约：**POST** + 空请求体（官方示例请求体为 `{}`）。
- 官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代调用权限集 id 为 133、160、177；仅「视频号助手」账号类型可调用。
- 官方错误码：`48001`（接口权限未开启，需前往视频号助手-开放能力-开放场景打开开关）。

### 3. `GetLeadsInfoByRequestIdAsync` — 按直播场次获取留资信息详情（`get_leads_info_by_request_id`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/leads/get_leads_info_by_request_id`

- **请求体**：`ChannelsLeadsInfoByRequestIdRequest` — 查询请求（`request_id` 与 `version` 必填；`last_buffer` 顺序翻页上下文选填）。
- **返回**：`ChannelsLeadsInfoResponse` — 用户留资信息列表（`user_data`）、翻页上下文（`last_buffer`）与是否还有留资信息（`continue_flag`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsinfobyrequestid
- 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143； / ④仅「视频号助手」账号类型可调用。

### 4. `GetLeadsInfoByComponentIdAsync` — 按时间获取留资信息详情（`get_leads_info_by_component_id`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/leads/get_leads_info_by_component_id`

- **请求体**：`ChannelsLeadsInfoByComponentIdRequest` — 查询请求（`leads_component_id` 与 `version` 必填；时间范围与 `last_buffer` 选填）。
- **返回**：`ChannelsLeadsInfoResponse` — 用户留资信息列表（`user_data`）、翻页上下文（`last_buffer`）与是否还有留资信息（`continue_flag`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsinfobycomponentid
- 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143； / ④仅「视频号助手」账号类型可调用；⑤响应参数表未逐项列出 `user_data` 元素字段， / 官方示例与「按直播场次获取」端点一致 ⇒ 共用 `ChannelsLeadsInfoResponse`（含可选 `close_flag` 超集）。

### 5. `GetLeadsRequestIdListAsync` — 获取留资 request_id 列表详情（`get_leads_request_id`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/leads/get_leads_request_id`

- **请求体**：`ChannelsLeadsRequestIdRequest` — 查询请求（`leads_component_id` 必填；`last_buffer` 顺序翻页上下文选填）。
- **返回**：`ChannelsLeadsRequestIdResponse` — 直播场次 request_id 列表（`item`）、翻页上下文与是否还有留资信息（`continue_flag`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsrequestid
- 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143； / ④仅「视频号助手」账号类型可调用；⑤ /  中的 `request_id` 可用于 / 「按直播场次获取留资信息详情」端点。

### 6. `GetLeadsComponentPromoteRecordAsync` — 获取留资组件直播推广记录信息详情（`get_leads_component_promote_record`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/leads/get_leads_component_promote_record`

- **请求体**：`ChannelsLeadsPromoteRecordRequest` — 查询请求（`leads_component_id` 必填；时间范围与 `last_buffer` 选填）。
- **返回**：`ChannelsLeadsPromoteRecordResponse` — 直播推广记录列表（`record_data`：观众数 / 曝光 uv / 点击 uv / 点击转化率 / 留资条数等）、翻页上下文与 `continue_flag`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadscomponentpromoterecord
- 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143； / ④仅「视频号助手」账号类型可调用。
- **官方文档矛盾照录**：返回参数表把 `exposure_click_rate`（点击转化率）类型标注为 / `boolean`、`click_uv` 说明写为「点击率」——但官方返回示例中 `exposure_click_rate` / 为数值 `0.0334`。SDK 按参数表建模（`ExposureClickRate` 为 bool）， / 若实际报文为数值请以官方后续修订为准。

### 7. `GetLeadsComponentIdListAsync` — 获取留资组件 Id 列表详情（`get_leads_component_id`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/leads/get_leads_component_id`

- **请求体**：`ChannelsLeadsComponentIdRequest` — 查询请求（`last_buffer` 顺序翻页上下文选填）。
- **返回**：`ChannelsLeadsComponentIdResponse` — 留资组件列表（`item`：组件 id / 标题 / 状态码）、翻页上下文与是否还有留资组件 Id（`continue_flag`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadscomponentid
- 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143； / ④仅「视频号助手」账号类型可调用；⑤组件状态枚举见 `ChannelsLeadsComponentStatuses` / （1 未提交 / 2 提交等待结果 / 3 未通过审核 / 4 审核通过 / 5 已删除）。

### 8. `GetFinderAttrByAppidAsync` — 获取视频号账号信息（`get_finder_attr_by_appid`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/finderlive/get_finder_attr_by_appid`
- **返回**：`ChannelsGetFinderAttrResponse` — 视频号信息（`finder_attr`：唯一标识 / 昵称 / 粉丝数）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderattrbyappid
- 官方契约：**POST** + 空请求体（官方原文「调用接口时传空的json串即可」）。
- 官方业务限制：①不支持云调用；②第三方平台代调用权限集 id 为 160；③仅「视频号助手」账号类型可调用。
- **官方文档矛盾照录**：返回参数表把 `finder_attr` 标注为 `objarray`（数组）， / 官方返回示例为单对象。SDK 按参数表建模（`FinderAttr` 为列表）。

### 9. `GetFinderLiveDataListAsync` — 获取留资直播数据详情（`get_finder_live_data_list`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/finderlive/get_finder_live_data_list`

- **请求体**：`ChannelsGetFinderLiveDataListRequest` — 查询请求（时间范围与 `last_buffer` 全部选填；时间与关播时间对比）。
- **返回**：`ChannelsGetFinderLiveDataListResponse` — 直播数据列表（`item`：观看人数 / 喝彩 / 分享 / 评论 / 人均观看时长 / 最高在线 / 新增粉丝等）、翻页上下文与 `continue_flag`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderlivedatalist
- 官方业务限制：①**该接口只包含 2023.9.21 号起、同时直播间有 1min 及以上的留资挂载才能导出对应直播数据**（官方原文）； / ②不支持云调用；③第三方平台代调用权限集 id 为 160；④仅「视频号助手」账号类型可调用。

### 10. `GetFinderLiveLeadsDataAsync` — 获取账号收集的留资数量（`get_finder_live_leads_data`）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/finderlive/get_finder_live_leads_data`

- **请求体**：`ChannelsGetFinderLiveLeadsDataRequest` — 查询请求（`start_time`/`end_time` 必填，与用户留资时间对比；`source_type` 选填，见 `ChannelsLeadsSourceTypes`）。
- **返回**：`ChannelsGetFinderLiveLeadsDataResponse` — 留资明细列表（`item`：组件类型 / 流量来源 / 留资条数，见 `ChannelsLeadsComponentTypes` 与 `ChannelsLeadsTrafficTypes`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderliveleadsdata
- 官方业务限制：①**该接口只统计 2023.9.13 号起的数据，所以 start_time 应大于等于 1694534400**（官方原文）； / ②不支持云调用；③第三方平台代调用权限集 id 为 160；④仅「视频号助手」账号类型可调用。

### 11. `GetLiveDashboardListAsync` — 获取直播大屏直播列表（`getlivelist`，只返回电商直播间的直播列表）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/livedashboard/getlivelist`

- **请求体**：`ChannelsGetLiveDashboardListRequest` — 查询请求（`ds` 必填，日期数值如 20240612；官方约定不得早于 20240101）。
- **返回**：`ChannelsGetLiveDashboardListResponse` — 直播列表（`live_items`：直播唯一 ID / 创建时间）、追踪 ID（`trace_id`）与是否还有更多（`has_more`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/livedashboard/api_getlivelist
- 官方业务限制：①**只返回电商直播间的直播列表**（官方原文）；②`ds` 目前不得早于 20240101； / ③不支持云调用；④第三方平台代调用权限集 id 为 176；⑤仅「视频号助手」账号类型可调用。
- 官方错误码：`1`（请求的参数错误）/ `5`（查询的日期过早）/ `48001`（接口权限未开启）。

### 12. `GetLiveDashboardDataAsync` — 获取直播大屏数据（`getlivedata`，按直播唯一 ID 拉取单场大屏全量数据）。

- **声明接口**：[IChannelsLiveService](./IChannelsLiveService.cs)
- **官方契约**：`POST /channels/livedashboard/getlivedata`

- **请求体**：`ChannelsGetLiveDashboardDataRequest` — 查询请求（`export_id` 必填，直播唯一 ID，可从直播大屏直播列表获取）。
- **返回**：`ChannelsGetLiveDashboardDataResponse` — 直播大屏全量数据（基础数据 `live_dashboard_data` / 内容力 `live_comparison_index` /
电商概要 `live_ec_data_summary` / 电商转化力 `live_ec_conversion_metric` /
电商画像 `live_ec_profile` / 渠道分布 `live_distribution_channel` / 商品数据
`single_live_ec_spu_data_page_v2`）与追踪 ID（`trace_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/livedashboard/api_getlivedata
- 官方业务限制：①不支持云调用；②第三方平台代调用权限集 id 为 176；③仅「视频号助手」账号类型可调用。
- **官方未展开结构的保真建模**：①`live_dashboard_data` 下的 `source.new_watch_uv` 与 / `portrait.online_watch_uv`/`portrait.new_watch_uv` 为「维度组合」动态键对象 / （键形如 `16`、`16,1,7`，维度枚举见 `ChannelsLiveDimensionTypes`）， / SDK 以 `Dictionary<string, JsonElement>` 保真透传；②`live_ec_profile` 的 / `profiles`/`totals` 官方仅给实体名未给字段表，以 `List<JsonElement>` 承载； / ③`live_ec_conversion_metric` 的 `ConversionMetric` 字段取自官方实体示例 / （`update_time`/`shelftime`），官方示例另列 `ItemConversionMetric` 实体但未给嵌套位置。
- **官方文档矛盾照录**：`live_distribution_channel` 子树参数表将 `dist_flow_type_stats` / 标注为 object 而官方示例为数组（元素字段亦与子表不一致）——SDK 容器形态按示例建模为列表、 / 元素字段按参数表子表建模，矛盾以官方后续修订为准。
- 官方错误码：`1`（请求参数错误）/ `48001`（接口权限未开启）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/Live`](../../../Mud.Wechat.Channels/Live/) 内共 **53** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **53** 个，`SerializerClassName` = `Live`。

AOT 源生成上下文：[`LiveJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/LiveJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsLiveGetFinderLiveRecordListResponse` | 应答 | [Live/ChannelsLiveGetFinderLiveRecordListResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsLiveGetFinderLiveRecordListResponse.cs) | `live_list` |
| `ChannelsLiveGetFinderLiveNoticeRecordListResponse` | 应答 | [Live/ChannelsLiveGetFinderLiveNoticeRecordListResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsLiveGetFinderLiveNoticeRecordListResponse.cs) | `live_notice_list` |
| `ChannelsLeadsInfoResponse` | 应答 | [Live/Leads/ChannelsLeadsInfoResponse.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsInfoResponse.cs) | `user_data`, `last_buffer`, `continue_flag` |
| `ChannelsLeadsInfoByRequestIdRequest` | 请求体 | [Live/Leads/ChannelsLeadsInfoByRequestIdRequest.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsInfoByRequestIdRequest.cs) | `request_id`, `last_buffer`, `version` |
| `ChannelsLeadsInfoByComponentIdRequest` | 请求体 | [Live/Leads/ChannelsLeadsInfoByComponentIdRequest.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsInfoByComponentIdRequest.cs) | `leads_component_id`, `start_time`, `end_time`, `last_buffer`, `version` |
| `ChannelsLeadsRequestIdResponse` | 应答 | [Live/Leads/ChannelsLeadsRequestIdResponse.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsRequestIdResponse.cs) | `item`, `last_buffer`, `continue_flag` |
| `ChannelsLeadsRequestIdRequest` | 请求体 | [Live/Leads/ChannelsLeadsRequestIdRequest.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsRequestIdRequest.cs) | `leads_component_id`, `last_buffer` |
| `ChannelsLeadsPromoteRecordResponse` | 应答 | [Live/Leads/ChannelsLeadsPromoteRecordResponse.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsPromoteRecordResponse.cs) | `record_data`, `last_buffer`, `continue_flag` |
| `ChannelsLeadsPromoteRecordRequest` | 请求体 | [Live/Leads/ChannelsLeadsPromoteRecordRequest.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsPromoteRecordRequest.cs) | `leads_component_id`, `start_time`, `end_time`, `last_buffer` |
| `ChannelsLeadsComponentIdResponse` | 应答 | [Live/Leads/ChannelsLeadsComponentIdResponse.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsComponentIdResponse.cs) | `item`, `last_buffer`, `continue_flag` |
| `ChannelsLeadsComponentIdRequest` | 请求体 | [Live/Leads/ChannelsLeadsComponentIdRequest.cs](../../../Mud.Wechat.Channels/Live/Leads/ChannelsLeadsComponentIdRequest.cs) | `last_buffer` |
| `ChannelsGetFinderAttrResponse` | 应答 | [Live/ChannelsGetFinderAttrResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetFinderAttrResponse.cs) | `finder_attr` |
| `ChannelsGetFinderLiveDataListResponse` | 应答 | [Live/ChannelsGetFinderLiveDataListResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetFinderLiveDataListResponse.cs) | `item`, `last_buffer`, `continue_flag` |
| `ChannelsGetFinderLiveDataListRequest` | 请求体 | [Live/ChannelsGetFinderLiveDataListRequest.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetFinderLiveDataListRequest.cs) | `start_time`, `end_time`, `last_buffer` |
| `ChannelsGetFinderLiveLeadsDataResponse` | 应答 | [Live/ChannelsGetFinderLiveLeadsDataResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetFinderLiveLeadsDataResponse.cs) | `item` |
| `ChannelsGetFinderLiveLeadsDataRequest` | 请求体 | [Live/ChannelsGetFinderLiveLeadsDataRequest.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetFinderLiveLeadsDataRequest.cs) | `start_time`, `end_time`, `source_type` |
| `ChannelsGetLiveDashboardListResponse` | 应答 | [Live/ChannelsGetLiveDashboardListResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetLiveDashboardListResponse.cs) | `trace_id`, `live_items`, `has_more` |
| `ChannelsGetLiveDashboardListRequest` | 请求体 | [Live/ChannelsGetLiveDashboardListRequest.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetLiveDashboardListRequest.cs) | `ds` |
| `ChannelsGetLiveDashboardDataResponse` | 应答 | [Live/ChannelsGetLiveDashboardDataResponse.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetLiveDashboardDataResponse.cs) | `trace_id`, `live_dashboard_data`, `live_comparison_index`, `live_ec_data_summary`, `live_ec_conversion_metric`, `live_ec_profile`, `live_distribution_channel`, `single_live_ec_spu_data_page_v2` |
| `ChannelsGetLiveDashboardDataRequest` | 请求体 | [Live/ChannelsGetLiveDashboardDataRequest.cs](../../../Mud.Wechat.Channels/Live/ChannelsGetLiveDashboardDataRequest.cs) | `export_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Live;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddLiveApi());

var service = app.Services.GetRequiredService<IChannelsLiveService>();
var ct = CancellationToken.None;
var response = await service.GetFinderAttrByAppidAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/finderlive/get_finder_attr_by_appid");
// 占位值仅用于编译示意；业务值须按 IChannelsLiveService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetFinderAttrByAppidAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsLiveService](./IChannelsLiveService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsLiveContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLiveContractGuards.cs)、[`ChannelsDeliveryContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsDeliveryContractGuards.cs) |
| 守卫中的代表性测试方法 | `LiveEndpoints_ShouldMatchOfficialRoutes`、`LiveEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`LiveInterface_ShouldBindChannelsToken_AndRegisterUnderLiveGroup`、`LiveDataModels_ShouldBeRegisteredInJsonContext`、`LiveDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`LiveEnumConstants_ShouldLockOfficialLiteralValues`、`DeliveryEndpoints_ShouldMatchOfficialRoutes`、`DeliveryEndpoints_ShouldAllBePost_WithBodyRules`、`DeliveryInterface_ShouldBindChannelsToken_AndRegisterUnderDeliveryGroup`、`DeliveryDataModels_ShouldBeRegisteredInJsonContext`、`DeliveryDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`DeliveryEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/Live`](../../../Mud.Wechat.Channels/Live/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsLiveContractGuards.cs`、`ChannelsDeliveryContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。

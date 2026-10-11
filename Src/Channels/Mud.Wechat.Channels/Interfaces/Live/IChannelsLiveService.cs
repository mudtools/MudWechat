// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Live;

namespace Mud.Wechat.Channels;

/// <summary>
/// 视频号助手（channels 生态）「直播」域 SDK（12 端点：直播记录 / 预约记录 / 留资组件管理 /
/// 留资服务的直播数据 / 直播大屏）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「罗盘数据」域严格区分</b>：本域承载视频号侧直播原始数据（<c>/channels/ec/finderlive/*</c>、
/// <c>/channels/finderlive/*</c>）、留资组件（<c>/channels/leads/*</c>）与直播大屏
/// （<c>/channels/livedashboard/*</c>）；商家/达人经营分析指标走 <see cref="IChannelsCompassService"/>。
/// </para>
/// <para>
/// <b>多前缀并存（官方原文，照抄不得改写）</b>：直播记录/预约走 <c>/channels/ec/finderlive/*</c>（2 端点）、
/// 留资服务的直播数据走 <c>/channels/finderlive/*</c>（3 端点，无 <c>/ec</c> 段）、留资组件走
/// <c>/channels/leads/*</c>（5 端点）、直播大屏走 <c>/channels/livedashboard/*</c>（2 端点）。
/// </para>
/// <para>
/// <b>适用范围</b>：全部端点仅「视频号助手」账号类型可调用（官方适用范围原文），且须在服务器端调用、
/// 均不支持云调用；第三方平台代调用权限集 id 见各端点 remarks。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：视频号助手官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsLiveService
{
    /// <summary>
    /// 视频号获取当前的直播记录（<c>getfinderliverecordlist</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播记录列表（<c>live_list</c>：直播 id / 描述 / 封面 / 开播视频号昵称与头像）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/live/api_getfinderliverecordlist"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方示例请求体为 <c>{}</c>）。</para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代调用权限集 id 为 133、160、177；仅「视频号助手」账号类型可调用。</para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启，需前往视频号助手-开放能力-开放场景打开开关）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/finderlive/getfinderliverecordlist")]
    Task<ChannelsLiveGetFinderLiveRecordListResponse> GetFinderLiveRecordListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 视频号获取当前的直播预约记录（<c>getfinderlivenoticerecordlist</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播预约记录列表（<c>live_notice_list</c>：预约 id / 状态 / 状态说明 / 开始时间 / 描述 / 视频号昵称与头像）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/live/api_getfinderlivenoticerecordlist"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方示例请求体为 <c>{}</c>）。</para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代调用权限集 id 为 133、160、177；仅「视频号助手」账号类型可调用。</para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启，需前往视频号助手-开放能力-开放场景打开开关）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/finderlive/getfinderlivenoticerecordlist")]
    Task<ChannelsLiveGetFinderLiveNoticeRecordListResponse> GetFinderLiveNoticeRecordListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 按直播场次获取留资信息详情（<c>get_leads_info_by_request_id</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>request_id</c> 与 <c>version</c> 必填；<c>last_buffer</c> 顺序翻页上下文选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户留资信息列表（<c>user_data</c>）、翻页上下文（<c>last_buffer</c>）与是否还有留资信息（<c>continue_flag</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsinfobyrequestid"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143；
    /// ④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/leads/get_leads_info_by_request_id")]
    Task<ChannelsLeadsInfoResponse> GetLeadsInfoByRequestIdAsync(
        [Body] ChannelsLeadsInfoByRequestIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按时间获取留资信息详情（<c>get_leads_info_by_component_id</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>leads_component_id</c> 与 <c>version</c> 必填；时间范围与 <c>last_buffer</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户留资信息列表（<c>user_data</c>）、翻页上下文（<c>last_buffer</c>）与是否还有留资信息（<c>continue_flag</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsinfobycomponentid"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143；
    /// ④仅「视频号助手」账号类型可调用；⑤响应参数表未逐项列出 <c>user_data</c> 元素字段，
    /// 官方示例与「按直播场次获取」端点一致 ⇒ 共用 <see cref="ChannelsLeadsInfoResponse"/>（含可选 <c>close_flag</c> 超集）。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/leads/get_leads_info_by_component_id")]
    Task<ChannelsLeadsInfoResponse> GetLeadsInfoByComponentIdAsync(
        [Body] ChannelsLeadsInfoByComponentIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取留资 request_id 列表详情（<c>get_leads_request_id</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>leads_component_id</c> 必填；<c>last_buffer</c> 顺序翻页上下文选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播场次 request_id 列表（<c>item</c>）、翻页上下文与是否还有留资信息（<c>continue_flag</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadsrequestid"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143；
    /// ④仅「视频号助手」账号类型可调用；⑤<paramref name="request"/> 中的 <c>request_id</c> 可用于
    /// 「按直播场次获取留资信息详情」端点。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/leads/get_leads_request_id")]
    Task<ChannelsLeadsRequestIdResponse> GetLeadsRequestIdListAsync(
        [Body] ChannelsLeadsRequestIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取留资组件直播推广记录信息详情（<c>get_leads_component_promote_record</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>leads_component_id</c> 必填；时间范围与 <c>last_buffer</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播推广记录列表（<c>record_data</c>：观众数 / 曝光 uv / 点击 uv / 点击转化率 / 留资条数等）、翻页上下文与 <c>continue_flag</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadscomponentpromoterecord"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143；
    /// ④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：返回参数表把 <c>exposure_click_rate</c>（点击转化率）类型标注为
    /// <c>boolean</c>、<c>click_uv</c> 说明写为「点击率」——但官方返回示例中 <c>exposure_click_rate</c>
    /// 为数值 <c>0.0334</c>。SDK 按参数表建模（<see cref="ChannelsLeadsPromoteRecord.ExposureClickRate"/> 为 bool），
    /// 若实际报文为数值请以官方后续修订为准。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/leads/get_leads_component_promote_record")]
    Task<ChannelsLeadsPromoteRecordResponse> GetLeadsComponentPromoteRecordAsync(
        [Body] ChannelsLeadsPromoteRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取留资组件 Id 列表详情（<c>get_leads_component_id</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>last_buffer</c> 顺序翻页上下文选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>留资组件列表（<c>item</c>：组件 id / 标题 / 状态码）、翻页上下文与是否还有留资组件 Id（<c>continue_flag</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leads/api_getleadscomponentid"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 143；
    /// ④仅「视频号助手」账号类型可调用；⑤组件状态枚举见 <see cref="ChannelsLeadsComponentStatuses"/>
    /// （1 未提交 / 2 提交等待结果 / 3 未通过审核 / 4 审核通过 / 5 已删除）。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/leads/get_leads_component_id")]
    Task<ChannelsLeadsComponentIdResponse> GetLeadsComponentIdListAsync(
        [Body] ChannelsLeadsComponentIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取视频号账号信息（<c>get_finder_attr_by_appid</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>视频号信息（<c>finder_attr</c>：唯一标识 / 昵称 / 粉丝数）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderattrbyappid"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</para>
    /// <para>
    /// 官方业务限制：①不支持云调用；②第三方平台代调用权限集 id 为 160；③仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：返回参数表把 <c>finder_attr</c> 标注为 <c>objarray</c>（数组），
    /// 官方返回示例为单对象。SDK 按参数表建模（<see cref="ChannelsGetFinderAttrResponse.FinderAttr"/> 为列表）。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/finderlive/get_finder_attr_by_appid")]
    Task<ChannelsGetFinderAttrResponse> GetFinderAttrByAppidAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取留资直播数据详情（<c>get_finder_live_data_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（时间范围与 <c>last_buffer</c> 全部选填；时间与关播时间对比）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播数据列表（<c>item</c>：观看人数 / 喝彩 / 分享 / 评论 / 人均观看时长 / 最高在线 / 新增粉丝等）、翻页上下文与 <c>continue_flag</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderlivedatalist"/></para>
    /// <para>
    /// 官方业务限制：①<b>该接口只包含 2023.9.21 号起、同时直播间有 1min 及以上的留资挂载才能导出对应直播数据</b>（官方原文）；
    /// ②不支持云调用；③第三方平台代调用权限集 id 为 160；④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/finderlive/get_finder_live_data_list")]
    Task<ChannelsGetFinderLiveDataListResponse> GetFinderLiveDataListAsync(
        [Body] ChannelsGetFinderLiveDataListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取账号收集的留资数量（<c>get_finder_live_leads_data</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>start_time</c>/<c>end_time</c> 必填，与用户留资时间对比；<c>source_type</c> 选填，见 <see cref="ChannelsLeadsSourceTypes"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>留资明细列表（<c>item</c>：组件类型 / 流量来源 / 留资条数，见 <see cref="ChannelsLeadsComponentTypes"/> 与 <see cref="ChannelsLeadsTrafficTypes"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/leadslive/api_getfinderliveleadsdata"/></para>
    /// <para>
    /// 官方业务限制：①<b>该接口只统计 2023.9.13 号起的数据，所以 start_time 应大于等于 1694534400</b>（官方原文）；
    /// ②不支持云调用；③第三方平台代调用权限集 id 为 160；④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>-2</c>（请求的参数错误）/ <c>1</c>（请求的信息不存在）/ <c>48001</c>（接口权限未开启）/ <c>100000</c>（没有权限访问）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/finderlive/get_finder_live_leads_data")]
    Task<ChannelsGetFinderLiveLeadsDataResponse> GetFinderLiveLeadsDataAsync(
        [Body] ChannelsGetFinderLiveLeadsDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播大屏直播列表（<c>getlivelist</c>，只返回电商直播间的直播列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，日期数值如 20240612；官方约定不得早于 20240101）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播列表（<c>live_items</c>：直播唯一 ID / 创建时间）、追踪 ID（<c>trace_id</c>）与是否还有更多（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/livedashboard/api_getlivelist"/></para>
    /// <para>
    /// 官方业务限制：①<b>只返回电商直播间的直播列表</b>（官方原文）；②<c>ds</c> 目前不得早于 20240101；
    /// ③不支持云调用；④第三方平台代调用权限集 id 为 176；⑤仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>1</c>（请求的参数错误）/ <c>5</c>（查询的日期过早）/ <c>48001</c>（接口权限未开启）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/livedashboard/getlivelist")]
    Task<ChannelsGetLiveDashboardListResponse> GetLiveDashboardListAsync(
        [Body] ChannelsGetLiveDashboardListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播大屏数据（<c>getlivedata</c>，按直播唯一 ID 拉取单场大屏全量数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>export_id</c> 必填，直播唯一 ID，可从直播大屏直播列表获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 直播大屏全量数据（基础数据 <c>live_dashboard_data</c> / 内容力 <c>live_comparison_index</c> /
    /// 电商概要 <c>live_ec_data_summary</c> / 电商转化力 <c>live_ec_conversion_metric</c> /
    /// 电商画像 <c>live_ec_profile</c> / 渠道分布 <c>live_distribution_channel</c> / 商品数据
    /// <c>single_live_ec_spu_data_page_v2</c>）与追踪 ID（<c>trace_id</c>）。
    /// </returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/livedashboard/api_getlivedata"/></para>
    /// <para>
    /// 官方业务限制：①不支持云调用；②第三方平台代调用权限集 id 为 176；③仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方未展开结构的保真建模</b>：①<c>live_dashboard_data</c> 下的 <c>source.new_watch_uv</c> 与
    /// <c>portrait.online_watch_uv</c>/<c>portrait.new_watch_uv</c> 为「维度组合」动态键对象
    /// （键形如 <c>16</c>、<c>16,1,7</c>，维度枚举见 <see cref="ChannelsLiveDimensionTypes"/>），
    /// SDK 以 <c>Dictionary&lt;string, JsonElement&gt;</c> 保真透传；②<c>live_ec_profile</c> 的
    /// <c>profiles</c>/<c>totals</c> 官方仅给实体名未给字段表，以 <c>List&lt;JsonElement&gt;</c> 承载；
    /// ③<c>live_ec_conversion_metric</c> 的 <c>ConversionMetric</c> 字段取自官方实体示例
    /// （<c>update_time</c>/<c>shelftime</c>），官方示例另列 <c>ItemConversionMetric</c> 实体但未给嵌套位置。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：<c>live_distribution_channel</c> 子树参数表将 <c>dist_flow_type_stats</c>
    /// 标注为 object 而官方示例为数组（元素字段亦与子表不一致）——SDK 容器形态按示例建模为列表、
    /// 元素字段按参数表子表建模，矛盾以官方后续修订为准。
    /// </para>
    /// <para>官方错误码：<c>1</c>（请求参数错误）/ <c>48001</c>（接口权限未开启）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/livedashboard/getlivedata")]
    Task<ChannelsGetLiveDashboardDataResponse> GetLiveDashboardDataAsync(
        [Body] ChannelsGetLiveDashboardDataRequest request,
        CancellationToken cancellationToken = default);
}

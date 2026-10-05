// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块预约会议高级管理域企业自建应用 SDK（承载本域全部 19 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingAdvancedService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许获取/修改该应用创建的会议的数据；
/// 成员相关查询仅返回在应用可见范围内的成员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingAdvancedService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingAdvancedService : IWechatWorkMeetingAdvancedService
{
    /// <summary>
    /// 获取会议受邀成员列表
    /// <para>受邀成员即预约会议时添加到参会人名单的成员；根据会议 ID 获取受邀成员列表，支持分页获取。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingInviteesRequest"/>：meetingid / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有尚未拉取的成员列表（has_more）、分页游标（next_cursor）与受邀成员列表（invitees：userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98160"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_invitees")]
    Task<GetMeetingInviteesResponse> GetInviteesAsync(
        [Body] GetMeetingInviteesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新会议受邀成员列表
    /// <para>根据会议 ID 更新会议受邀成员列表（覆盖式更新）。</para>
    /// <para>官方限制：最多可以传入 2000 名受邀成员（与企业所购在线会议室最大方数相关）；管理员必须在受邀成员列表中。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingInviteesRequest"/>：meetingid / invitees）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98162"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许更新该应用创建的会议；成员需要在应用的可见范围内。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/set_invitees")]
    Task<WechatWorkResponse> SetInviteesAsync(
        [Body] SetMeetingInviteesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建用户专属参会链接
    /// <para>为会议生成多个专属入会链接，不同链接以 <c>customer_data</c> 进行区分；通过用户入会、用户进入等候室等事件，
    /// 或通过获取等候室成员列表的 API 可查询到该参数。</para>
    /// <para>官方限制：该接口不支持网络研讨会（Webinar）；customer_data 长度不超过 256 字节，
    /// 需以 <c>{"ver": "1.0", "userData":"自定义字段"}</c> 的结构进行 Base64 编码。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateMeetingCustomerShortUrlRequest"/>：meetingid / customer_data）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户专属参会链接对象（meeting_short_url_customer_data：customer_data / meeting_short_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98818"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/create_customer_short_url")]
    Task<CreateMeetingCustomerShortUrlResponse> CreateCustomerShortUrlAsync(
        [Body] CreateMeetingCustomerShortUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户专属参会链接
    /// <para>获取指定会议的所有专属参会链接及 <c>customer_data</c>。</para>
    /// <para>官方限制：该接口不支持个人会议号会议、网络研讨会（Webinar）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingCustomerShortUrlRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户专属参会链接对象列表（meeting_short_url_customer_data_list：customer_data / meeting_short_url）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98819"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_customer_short_url")]
    Task<GetMeetingCustomerShortUrlResponse> GetCustomerShortUrlAsync(
        [Body] GetMeetingCustomerShortUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实时会中成员列表
    /// <para>获取当前会中成员列表，仅包括会中的成员；如果成员已离会，则不返回。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// limit 最大 50 条，且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致；
    /// 周期性会议 sub_meetingid 必传。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingRealtimeAttendeeListRequest"/>：meetingid / sub_meetingid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多的成员列表（has_more）、分页游标（next_cursor）与参会人列表
    /// （attendees：userid / tmp_openid / join_time / instance_id / role / join_type / audio_state / video_state / screen_shared_state）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98157"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// <para>官方文档陷阱：官方请求示例将分页游标字段误写为 <c>cursort</c>，参数表为 <c>cursor</c>，本 SDK 以参数表为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_realtime_attendee_list")]
    Task<GetMeetingRealtimeAttendeeListResponse> GetRealtimeAttendeeListAsync(
        [Body] GetMeetingRealtimeAttendeeListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已参会成员列表
    /// <para>获取会议的已参会成员列表，支持查询预约会议及网络研讨会；会议还未开始时调用返回空列表。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// start_time / end_time 时间区间不允许超过 31 天（为空时默认前推 31 天至当前时间），
    /// 两者都没传时最大查询时间跨度 90 天；对于周期性会议查询暂时不生效，请使用分页参数查询；
    /// limit 每页最大 100 条，且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingAttendeeListRequest"/>：meetingid / sub_meetingid / start_time / end_time / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有待拉取的成员列表（has_more）、分页游标（next_cursor）与参会人列表
    /// （attendees：userid / tmp_openid / join_time / quit_time / instance_id / role / webinar_role / join_type / net / audio_state / video_state / screen_shared_state / customer_data）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98156"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_attendee_list")]
    Task<GetMeetingAttendeeListResponse> GetAttendeeListAsync(
        [Body] GetMeetingAttendeeListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实时等候室成员列表
    /// <para>获取某指定会议的等候室成员列表；需开启等候室且为「会议进行中」状态，会议非进行中时返回空列表。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// limit 默认 10、最大 50，且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingWaitingRoomCurrentUserListRequest"/>：meetingid / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有未拉取的成员列表（has_more）、分页游标（next_cursor）与等候室人员列表
    /// （user_list：userid / instance_id / customer_data / tmp_openid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98163"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/waitingroom/get_current_user_list")]
    Task<GetMeetingWaitingRoomCurrentUserListResponse> GetWaitingRoomCurrentUserListAsync(
        [Body] GetMeetingWaitingRoomCurrentUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取等候室成员记录
    /// <para>获取等候室成员列表，包括等候室内所有进出过的成员；会前、会中、会后都可以获取。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// limit 默认 20、最大 50，且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingWaitingRoomUserListRequest"/>：meetingid / limit / cursor）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多成员列表（has_more）、分页游标（next_cursor）与等候室成员记录列表
    /// （user_list：userid / tmp_openid / instance_id / join_time / quit_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98164"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/waitingroom/get_user_list")]
    Task<GetMeetingWaitingRoomUserListResponse> GetWaitingRoomUserListAsync(
        [Body] GetMeetingWaitingRoomUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员设备是否入会
    /// <para>查询企业内成员在当前时间前是否有设备进入指定的会议中。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；成员需要在应用的可见范围内；meetingid_list 须为本企业创建的会议。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CheckMeetingDeviceInMeetingRequest"/>：userid / instance_id_list / meetingid_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>已入会的设备检查结果列表（result_list：meetingid / instance_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98165"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据；成员需要在应用的可见范围内。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/check_device_in_meeting")]
    Task<CheckMeetingDeviceInMeetingResponse> CheckDeviceInMeetingAsync(
        [Body] CheckMeetingDeviceInMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议嘉宾列表
    /// <para>通过会议 ID 获取会议嘉宾列表。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingGuestsRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议 ID（meetingid）、入会码（meeting_code）、会议主题（title）与嘉宾列表（guests：area / phone_number / guest_name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99039"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_guests")]
    Task<GetMeetingGuestsResponse> GetGuestsAsync(
        [Body] GetMeetingGuestsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新会议嘉宾列表
    /// <para>通过会议 ID 更新会议嘉宾列表。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingGuestsRequest"/>：meetingid / guests）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99040"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/set_guests")]
    Task<WechatWorkResponse> SetGuestsAsync(
        [Body] SetMeetingGuestsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议健康度
    /// <para>获取已结束会议的会议及参会成员的健康度。</para>
    /// <para>官方限制：仅允许查询该应用创建的会议的数据；仅返回在应用可见范围内的成员；
    /// start_time 可查询的时间区间为过去 7 天到现在，返回离 start_time 最近的一个媒体房间数据
    /// （从第一个人入会到会中成员全部离开会议形成一个媒体房间，若同一会议号下再次有人入会则形成新的媒体房间）；
    /// limit 默认 50、最大 50，且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致；
    /// 周期性会议 sub_meetingid 必传。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingQualityRequest"/>：meetingid / sub_meetingid / start_time / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议级健康度（quality / audio_quality / video_quality / screen_share_quality / network_quality / problems）、
    /// 参会人员健康度列表（attendees）、分页游标（next_cursor）与是否还有待拉取的列表（has_more）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98821"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许查询该应用创建的会议的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_quality")]
    Task<GetMeetingQualityResponse> GetQualityAsync(
        [Body] GetMeetingQualityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会议报名配置
    /// <para>修改会议的报名配置和报名问题。</para>
    /// <para>官方限制：需要会议已开启报名；非特殊问题按传入的顺序排序，特殊问题会优先放在最前面；
    /// 报名问题最多 8 个选项，每个选项限 40 个汉字；问题标题限 40 个字符。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingEnrollConfigRequest"/>：meetingid / approve_type / is_collect_question / no_registration_needed_for_staff / question_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>报名问题数量（question_count，不收集问题时返回 0）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98797"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/set_config")]
    Task<SetMeetingEnrollConfigResponse> SetEnrollConfigAsync(
        [Body] SetMeetingEnrollConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议报名配置
    /// <para>获取会议的报名配置和报名问题。</para>
    /// <para>官方限制：会议未开启报名时会返回未开启报名错误。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingEnrollConfigRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批类型（approve_type）、是否收集问题（is_collect_question）、本企业成员是否无需报名（no_registration_needed_for_staff）与报名问题列表（question_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98800"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/get_config")]
    Task<GetMeetingEnrollConfigResponse> GetEnrollConfigAsync(
        [Body] GetMeetingEnrollConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议成员报名 ID
    /// <para>查询会议中已报名成员的报名 ID：可通过会中成员的 tmp_openid 查询到对应的报名 ID，
    /// 成员的报名 ID 每场会议是唯一的，可以通过「获取会议报名信息」接口匹配其对应的报名信息。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；tmp_openid_list 单次最多支持 500 条；
    /// 如果传入的成员没有报名，则不会返回该成员的 tmp_openid 和报名 ID。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="QueryMeetingEnrollIdsRequest"/>：meetingid / sorting_rules / tmp_openid_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员报名 ID 数组（enroll_id_list：tmp_openid / enroll_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98794"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/query_by_tmp_openid")]
    Task<QueryMeetingEnrollIdsResponse> QueryEnrollIdsAsync(
        [Body] QueryMeetingEnrollIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议报名信息
    /// <para>获取已报名成员数量和报名成员答题详情。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；limit 最大 50 条（默认 50），
    /// 且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingEnrollsRequest"/>：meetingid / status / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有待拉取的成员列表（has_more）、分页游标（next_cursor）与当前页的报名列表
    /// （enroll_list：enroll_id / enroll_time / enroll_source_type / nick_name / status / userid / tmp_openid / enroll_code / answer_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98810"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/list")]
    Task<ListMeetingEnrollsResponse> ListEnrollsAsync(
        [Body] ListMeetingEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 审批会议报名信息
    /// <para>批量审批会议的报名信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议；取消批准后状态将变成待审批。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ApproveMeetingEnrollsRequest"/>：meetingid / action / enroll_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功处理的数量（handled_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98807"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/approve")]
    Task<ApproveMeetingEnrollsResponse> ApproveEnrollsAsync(
        [Body] ApproveMeetingEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入会议报名信息
    /// <para>指定会议中导入报名信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据；会议未开启报名时会返回未开启报名错误。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ImportMeetingEnrollsRequest"/>：meetingid / enroll_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功导入的报名信息条数（total_count）与报名成员列表（enroll_list：enroll_id / userid / area / phone_number / nick_name / enroll_code）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98816"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/import")]
    Task<ImportMeetingEnrollsResponse> ImportEnrollsAsync(
        [Body] ImportMeetingEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会议报名信息
    /// <para>删除指定会议的报名信息，支持删除成员手动报名的信息和导入的报名信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的会议的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteMeetingEnrollsRequest"/>：meetingid / enroll_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功删除的报名信息数量（total_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98817"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/enroll/delete")]
    Task<DeleteMeetingEnrollsResponse> DeleteEnrollsAsync(
        [Body] DeleteMeetingEnrollsRequest request,
        CancellationToken cancellationToken = default);
}

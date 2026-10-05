// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块网络研讨会（Webinar）管理域企业自建应用 SDK（承载本域全部 14 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingWebinarService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；成员需要在应用的可见范围内；
/// 仅允许获取/修改/取消该应用创建的网络研讨会的数据。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingWebinarService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingWebinarService : IWechatWorkMeetingWebinarService
{
    /// <summary>
    /// 创建网络研讨会
    /// <para>预定一场网络研讨会（适用于沙龙、圆桌会议、行业峰会等场景，支持最高 5w 人同时参会，会中可展开丰富的互动及管理）。</para>
    /// <para>官方限制：开始时间不能少于当前时间戳半小时以上；admission_type = 2 时 password 必传且仅此时生效；
    /// playback_for_audience 为官方必填，开启时必须开启云录制（auto_record_type = cloud）；
    /// cover_url/description 需要开启活动页配置（activity_page），封面为异步上传（可订阅「素材上传结果」事件）；
    /// 敏感词最多 50 个、单个限制 10 个中文字符。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateWebinarRequest"/>：admin_userid / title / sponsor / start_time / end_time / admission_type / hosts / password / cover_url / description / enable_guest_invite_link / media_setting / enable_qa / sensitive_words / enable_manual_check / activity_page / display_number_of_attendees / playback_for_audience / preparation_mode）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>网络研讨会主题（title）、ID（meetingid）、会议号（meeting_code）、开始/结束时间（start_time / end_time）、
    /// 观众观看限制类型（admission_type）、观看密码（password）、观众入会链接（audience_join_link）、嘉宾入会链接（guest_join_link）、
    /// 人工审核链接与密码（manual_check_link / manual_check_password，enable_manual_check 开启后返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98842"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；成员需要在应用的可见范围内。</para>
    /// <para>官方文档陷阱：start_time/end_time 参数表与示例均为字符串形态的时间戳（单位秒），本模型按字符串承载。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/create")]
    Task<CreateWebinarResponse> CreateWebinarAsync(
        [Body] CreateWebinarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改网络研讨会
    /// <para>修改指定的网络研讨会信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的网络研讨会的数据；成员需要在应用的可见范围内；
    /// playback_for_audience 为官方必填，开启时必须开启云录制；修改 hosts 会覆盖原有设置。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWebinarRequest"/>：meetingid / title / sponsor / start_time / end_time / admission_type / hosts / password / cover_url / description / enable_guest_invite_link / media_setting / enable_qa / sensitive_words / enable_manual_check / activity_page / display_number_of_attendees / playback_for_audience / preparation_mode）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98843"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据；成员需要在应用的可见范围内。</para>
    /// <para>官方文档陷阱：文档页参数表将 media_setting 标注为 object[]，官方请求示例为单个对象，本 SDK 以单个对象承载。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/update")]
    Task<WechatWorkResponse> UpdateWebinarAsync(
        [Body] UpdateWebinarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消网络研讨会
    /// <para>取消指定的网络研讨会。</para>
    /// <para>官方限制：仅允许取消该应用创建的网络研讨会。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelWebinarRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98870"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许取消该应用创建的网络研讨会。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/cancel")]
    Task<WechatWorkResponse> CancelWebinarAsync(
        [Body] CancelWebinarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取网络研讨会详情
    /// <para>获取指定的网络研讨会详细信息，支持通过会议 ID 或会议 Code 方式查询。</para>
    /// <para>官方限制：仅允许获取该应用创建的网络研讨会的数据；仅返回在应用可见范围内的成员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWebinarRequest"/>：meetingid / meeting_code，二者必须送一个，都送时以 meetingid 为准）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>网络研讨会详情（title / meetingid / meeting_code / status / sponsor / start_time / end_time / admission_type / hosts /
    /// password / cover_url / description / enable_guest_invite_link / audience_join_link / guest_join_link / media_setting / enable_qa /
    /// manual_check_link / manual_check_password / activity_page / display_number_of_attendees / playback_for_audience / playback_url /
    /// preparation_mode / warm_up_picture / warm_up_video / allow_attendees_invite_others）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98860"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的网络研讨会的数据；仅返回在应用可见范围内的成员。</para>
    /// <para>官方文档陷阱：参数表将主题字段列为 <c>subject</c>、官方响应示例为 <c>title</c>（与创建响应一致），本 SDK 以示例为准；
    /// status 为字符串枚举（MEETING_STATE_*）；响应 media_setting 的入会静音字段示例作 <c>mute_enable_join</c>（参数表作 enable_enter_mute）。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/get")]
    Task<GetWebinarResponse> GetWebinarAsync(
        [Body] GetWebinarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取网络研讨会嘉宾列表
    /// <para>获取指定的网络研讨会嘉宾列表，支持通过会议 ID 或会议 Code 方式查询。</para>
    /// <para>官方限制：仅允许获取该应用创建的网络研讨会的数据；仅返回在应用可见范围内的成员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListWebinarGuestsRequest"/>：meetingid / meeting_code，二者必须送一个，都送时以 meetingid 为准）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>嘉宾列表（guests：guest_type / userid / area / phone_number / guest_name / email）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98871"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的网络研讨会的数据；仅返回在应用可见范围内的成员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/list_guest")]
    Task<ListWebinarGuestsResponse> ListWebinarGuestsAsync(
        [Body] ListWebinarGuestsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新网络研讨会嘉宾列表
    /// <para>更新指定的网络研讨会嘉宾列表。</para>
    /// <para>官方限制：仅允许修改该应用创建的网络研讨会的数据；成员需要在应用的可见范围内。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWebinarGuestsRequest"/>：meetingid / guests）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98872"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据；成员需要在应用的可见范围内。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/update_guest_list")]
    Task<WechatWorkResponse> UpdateWebinarGuestsAsync(
        [Body] UpdateWebinarGuestsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 管理网络研讨会暖场配置
    /// <para>对网络研讨会进行暖场设置；可订阅「网络研讨会暖场上传结果」事件用于获得上传结果。</para>
    /// <para>官方限制：仅允许修改该应用创建的网络研讨会的数据；暖场图片与视频只能选择一个，同时传入以图片为准。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateWebinarWarmUpRequest"/>：meetingid / warm_up_picture / warm_up_video / allow_attendees_invite_others）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98882"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/update_warm_up")]
    Task<WechatWorkResponse> UpdateWebinarWarmUpAsync(
        [Body] UpdateWebinarWarmUpRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改网络研讨会报名配置
    /// <para>修改网络研讨会的报名配置和报名问题。</para>
    /// <para>官方限制：需要会议已开启报名；仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetWebinarEnrollConfigRequest"/>：meetingid / approve_type / is_collect_question / no_registration_needed_for_staff / question_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>报名问题数量（question_count，不收集问题时返回 0）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98875"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/set_config")]
    Task<SetWebinarEnrollConfigResponse> SetWebinarEnrollConfigAsync(
        [Body] SetWebinarEnrollConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取网络研讨会报名配置
    /// <para>获取网络研讨会的报名配置和报名问题。</para>
    /// <para>官方限制：会议未开启报名时会返回未开启报名错误。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWebinarEnrollConfigRequest"/>：meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审批类型（approve_type）、是否收集问题（is_collect_question）、本企业成员是否无需报名（no_registration_needed_for_staff）与报名问题列表（question_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98874"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/get_config")]
    Task<GetWebinarEnrollConfigResponse> GetWebinarEnrollConfigAsync(
        [Body] GetWebinarEnrollConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取网络研讨会成员报名 ID
    /// <para>查询网络研讨会中已报名成员的报名 ID（每场会议唯一，可通过「获取网络研讨会报名信息」接口匹配其对应的报名信息）。</para>
    /// <para>官方限制：仅允许获取该应用创建的网络研讨会的数据；tmp_openid_list 单次最多支持 500 条；
    /// 如果传入的成员没有报名，则不会返回该成员的 ms_open_id 和报名 ID。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="QueryWebinarEnrollIdsRequest"/>：meetingid / sorting_rules / tmp_openid_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员报名 ID 数组（enroll_id_list：tmp_openid / enroll_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98873"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid")]
    Task<QueryWebinarEnrollIdsResponse> QueryWebinarEnrollIdsAsync(
        [Body] QueryWebinarEnrollIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取网络研讨会报名信息
    /// <para>获取已报名观众数量和报名观众答题详情。</para>
    /// <para>官方限制：会议未开启报名时会返回未开启报名错误；limit 最大 50 条（默认 50），
    /// 且 limit 参数必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListWebinarEnrollsRequest"/>：meetingid / status / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有待拉取的成员列表（has_more）、分页游标（next_cursor）与当前页的报名列表（enroll_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98876"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/list")]
    Task<ListWebinarEnrollsResponse> ListWebinarEnrollsAsync(
        [Body] ListWebinarEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 审批网络研讨会报名信息
    /// <para>批量审批网络研讨会的报名信息。</para>
    /// <para>官方限制：仅允许审批该应用创建的网络研讨会的数据；取消批准后状态将变成待审批。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ApproveWebinarEnrollsRequest"/>：meetingid / enroll_id_list / action）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功处理的数量（handled_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98877"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许审批该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/approve")]
    Task<ApproveWebinarEnrollsResponse> ApproveWebinarEnrollsAsync(
        [Body] ApproveWebinarEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入网络研讨会报名信息
    /// <para>在指定网络研讨会（Webinar）中导入报名信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的网络研讨会的数据；会议未开启报名时会返回未开启报名错误。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ImportWebinarEnrollsRequest"/>：meetingid / enroll_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功导入的报名信息条数（total_count）与报名成员列表（enroll_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98880"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/import")]
    Task<ImportWebinarEnrollsResponse> ImportWebinarEnrollsAsync(
        [Body] ImportWebinarEnrollsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除网络研讨会报名信息
    /// <para>删除指定网络研讨会（Webinar）的报名信息，支持删除成员手动报名的信息和导入的报名信息。</para>
    /// <para>官方限制：仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteWebinarEnrollsRequest"/>：meetingid / enroll_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功删除的报名信息数量（total_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98881"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许修改该应用创建的网络研讨会的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/webinar/enroll/delete")]
    Task<DeleteWebinarEnrollsResponse> DeleteWebinarEnrollsAsync(
        [Body] DeleteWebinarEnrollsRequest request,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块 Rooms 会议室管理域企业自建应用 SDK（承载本域全部 12 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingRoomsService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许操作/获取该应用创建的会议的数据。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingRoomsService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingRoomsService : IWechatWorkMeetingRoomsService
{
    /// <summary>
    /// 预定 Rooms 会议室
    /// <para>对成功预定的会议添加 Rooms 会议室，支持为同一个会议预定多个 Rooms 会议室。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；Rooms 会议室预定对会议时长有硬性要求，
    /// 会议时长不得大于 24 小时，且不支持周期性会议。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BookMeetingRoomRequest"/>：meetingid / meeting_room_id_list / subject_visible）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>Rooms 会议室对象列表（meeting_room_list：meeting_room_id / meeting_room_name / meeting_room_location）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98791"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/book")]
    Task<BookMeetingRoomResponse> BookMeetingRoomAsync(
        [Body] BookMeetingRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 释放 Rooms 会议室
    /// <para>通过会议 ID 释放 Rooms 会议室，支持释放多个 Rooms 会议室。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ReleaseMeetingRoomRequest"/>：meetingid / meeting_room_id_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98792"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/release")]
    Task<WechatWorkResponse> ReleaseMeetingRoomAsync(
        [Body] ReleaseMeetingRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室列表
    /// <para>获取企业下的 Rooms 会议室列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingRoomsRequest"/>：meeting_room_name / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多列表（has_more）、分页游标（next_cursor）与 Rooms 会议室对象列表（meeting_room_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98795"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/list")]
    Task<ListMeetingRoomsResponse> ListMeetingRoomsAsync(
        [Body] ListMeetingRoomsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室详情
    /// <para>根据 Rooms 会议室 ID 获取该 Rooms 会议室详细信息（基本信息 / 账号信息 / 硬件信息 / PMI 信息）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingRoomInfoRequest"/>：meeting_room_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>Rooms 会议室详情（basic_info / account_info / hardware_info / pmi_info / monitor_status / is_allow_call / scheduled_status）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98793"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/get_info")]
    Task<GetMeetingRoomInfoResponse> GetMeetingRoomInfoAsync(
        [Body] GetMeetingRoomInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室配置项
    /// <para>获取 Rooms 会议室的配置项（会议配置 + 录制配置）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingRoomConfigRequest"/>：meeting_room_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议配置对象（meeting_settings）与录制配置对象（record_settings）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98802"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/get_config")]
    Task<GetMeetingRoomConfigResponse> GetMeetingRoomConfigAsync(
        [Body] GetMeetingRoomConfigRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室下的会议列表
    /// <para>获取指定 Rooms 会议室下的会议列表。</para>
    /// <para>官方限制：仅允许获取该应用创建的会议的数据；meeting_room_id 与 rooms_id 二者填其一；
    /// 时间区间不超过 90 天；limit 默认 20 条、最大 20 条，且必须与首次调用获得 cursor 时传入的 limit 一致。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListMeetingRoomMeetingsRequest"/>：meeting_room_id / rooms_id / start_time / end_time / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多列表（has_more）、分页游标（next_cursor）与会议对象列表（meeting_info_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98796"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许获取该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/list_meetings")]
    Task<ListMeetingRoomMeetingsResponse> ListMeetingRoomMeetingsAsync(
        [Body] ListMeetingRoomMeetingsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备列表
    /// <para>获取企业下的可用设备列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListRoomDevicesRequest"/>：meeting_room_name / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多列表（has_more）、分页游标（next_cursor）与设备信息对象列表（device_info_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98798"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/list_devices")]
    Task<ListRoomDevicesResponse> ListRoomDevicesAsync(
        [Body] ListRoomDevicesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取控制器列表
    /// <para>获取企业下的控制器列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListRoomControllersRequest"/>：controller_name / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多列表（has_more）、分页游标（next_cursor）与控制器信息对象列表（controller_info_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98799"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/list_controllers")]
    Task<ListRoomControllersResponse> ListRoomControllersAsync(
        [Body] ListRoomControllersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室资源
    /// <para>获取企业购买的 Rooms 会议室资源。</para>
    /// <para>官方契约：本端点为<b>无请求体的 POST</b>，仅以 Query 注入的 access_token 鉴权。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>企业 Rooms 资源统计（normal_count / special_count / normal_used_count / special_used_count / normal_expired_count / special_expired_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98809"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/get_inventory")]
    Task<GetMeetingRoomInventoryResponse> GetMeetingRoomInventoryAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 呼叫 Rooms 会议室
    /// <para>会议可以通过 Rooms 会议室 ID 呼叫 Rooms 会议室邀请其入会（也支持通过 MRA 信令地址呼叫）。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议的数据；meeting_room_id 与 mra_address 二选一。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CallMeetingRoomRequest"/>：meetingid / meeting_room_id / mra_address）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>呼叫 ID（invite_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98804"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/call")]
    Task<CallMeetingRoomResponse> CallMeetingRoomAsync(
        [Body] CallMeetingRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消呼叫 Rooms 会议室
    /// <para>会议可以通过 Rooms 会议室 ID 进行取消呼叫操作。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议的数据；meeting_room_id 与 mra_address 二选一。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelCallMeetingRoomRequest"/>：meetingid / invite_id / meeting_room_id / mra_address）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98805"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/cancel_call")]
    Task<WechatWorkResponse> CancelCallMeetingRoomAsync(
        [Body] CancelCallMeetingRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Rooms 会议室应答状态
    /// <para>会议获取其呼叫 Rooms 会议室的应答状态。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议的数据；meeting_room_id 与 mra_address 二选一。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingRoomResponseStatusRequest"/>：meetingid / meeting_room_id / mra_address）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>应答状态（status：0 无应答 / 1 未呼叫 / 2 入会中 / 3 被拒绝 / 4 呼叫中 / 5 取消呼叫（仅 Rooms 会议室有该状态）/ 6 已离会）
    /// 与最近一次应答时间（response_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98806"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议的数据。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/rooms/get_response_status")]
    Task<GetMeetingRoomResponseStatusResponse> GetMeetingRoomResponseStatusAsync(
        [Body] GetMeetingRoomResponseStatusRequest request,
        CancellationToken cancellationToken = default);
}

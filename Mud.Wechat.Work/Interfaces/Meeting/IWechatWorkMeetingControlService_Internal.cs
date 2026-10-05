// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块会中控制管理域企业自建应用 SDK（承载本域全部 17 端点）。
/// <para>
/// 官方仅向企业自建应用开放本域端点（第三方应用开发与服务商代开发章节均无对应 API），
/// 端点全部声明于本接口；继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），
/// 父接口见 <see cref="IWechatWorkMeetingControlService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置在「可调用接口的应用」列表中；仅允许操作该应用创建的会议；
/// 会议投票端点还要求仅进行中的会议可以调用、操作者是主持人或者会议管理员。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingControlService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingControlService : IWechatWorkMeetingControlService
{
    /// <summary>
    /// 管理会中设置
    /// <para>管理进行中会议的设置项，例如全体静音、是否允许参会者聊天、锁定会议、隐藏会议号和密码、是否开启等候室等。</para>
    /// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况；
    /// <c>allow_unmute_self</c> 请求参数 <c>mute_all</c> 必传，且 <c>mute_all</c> = true 时设置才生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingRealtimeSettingsRequest"/>：meetingid / mute_all / allow_unmute_self / enable_enter_mute / meeting_locked / hide_meeting_code_password / allow_chat / allow_share_screen / allow_external_user / play_ivr_on_join / enable_waiting_room）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98175"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/set")]
    Task<WechatWorkResponse> SetRealtimeSettingsAsync(
        [Body] SetMeetingRealtimeSettingsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 管理联席主持人
    /// <para>设置或撤销会议中的参会者联席主持人身份。</para>
    /// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况；
    /// 被操作成员终端设备类型仅支持 PC/Mac/Android/iOS/Web/iPad/Android Pad/voip、sip 设备/鸿蒙设备。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingCoHostRequest"/>：meetingid / action / operated_user）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98180"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/set_cohost")]
    Task<WechatWorkResponse> SetCoHostAsync(
        [Body] SetMeetingCoHostRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 静音成员
    /// <para>会议中成员静音操作。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议的数据；被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MuteMeetingUserRequest"/>：meetingid / option / operated_user）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98184"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议的数据。</para>
    /// <para>官方文档陷阱：参数表将 <c>option</c> 标注为 string、<c>operated_user</c> 标注为 object[]，
    /// 官方请求示例分别为布尔值与单个对象，本 SDK 以示例为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/mute_user")]
    Task<WechatWorkResponse> MuteUserAsync(
        [Body] MuteMeetingUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭或开启成员视频
    /// <para>关闭指定成员视频，支持关闭或开启 MRA 设备的视频。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；<c>video</c> = true（开启视频）仅支持 MRA 设备；
    /// 被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SwitchMeetingUserVideoRequest"/>：meetingid / video / operated_user）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98189"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// <para>官方文档陷阱：文档页 User 参数表将设备类型字段误写为 <c>instanceid</c>，官方请求示例为 <c>instance_id</c>，本 SDK 以示例为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/switch_user_video")]
    Task<WechatWorkResponse> SwitchUserVideoAsync(
        [Body] SwitchMeetingUserVideoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭成员屏幕共享
    /// <para>停止成员发起的屏幕共享。</para>
    /// <para>官方限制：目前暂不支持 MRA 设备作为被操作者的情况；
    /// 被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CloseMeetingScreenShareRequest"/>：meetingid / operated_user）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98185"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/close_screen_share")]
    Task<WechatWorkResponse> CloseScreenShareAsync(
        [Body] CloseMeetingScreenShareRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改成员在会中显示的昵称
    /// <para>修改成员在会中显示的昵称（批量）。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；昵称限制 20 个字符；
    /// 被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetMeetingUserNicknamesRequest"/>：meetingid / operated_users）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98188"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// <para>官方文档陷阱：文档页参数表将本字段误写为 <c>opereated_users</c>（多一个 e），官方请求示例为 <c>operated_users</c>，本 SDK 以示例为准。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/set_nicknames")]
    Task<WechatWorkResponse> SetUserNicknamesAsync(
        [Body] SetMeetingUserNicknamesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 管理等候室成员
    /// <para>会议等候室设置，支持主持人将等候室成员移入会议、将会议成员移入等候室、将等候室成员移出等候室等操作。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；<c>allow_rejoin</c> 仅 <c>operate_type</c> = 3 时才允许设置（该字段对 MRA 设备不生效）；
    /// 被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ManageMeetingWaitingRoomUsersRequest"/>：meetingid / operate_type / allow_rejoin / operated_users）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98186"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/manage_waiting_room_users")]
    Task<WechatWorkResponse> ManageWaitingRoomUsersAsync(
        [Body] ManageMeetingWaitingRoomUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移出成员
    /// <para>将会议中成员移出会议。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；
    /// 被操作成员的设备类型须与被操作者的设备类型保持一致，否则不生效。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="KickoutMeetingUsersRequest"/>：meetingid / allow_rejoin / operated_users）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98181"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/kickout_users")]
    Task<WechatWorkResponse> KickoutUsersAsync(
        [Body] KickoutMeetingUsersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 结束会议
    /// <para>结束一个进行中的会议。</para>
    /// <para>官方限制：仅允许操作该应用创建的会议；
    /// 周期性会议如果还有子会议，<c>retrieve_code</c> 需设置为 0（不回收会议号），否则会导致后续子会议无法正常进行；
    /// <c>retrieve_code</c> 对快速会议不生效，快速会议会强制收回会议号。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DismissMeetingRequest"/>：meetingid / force_dismiss / retrieve_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98187"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅允许操作该应用创建的会议。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/realcontrol/dismiss")]
    Task<WechatWorkResponse> DismissMeetingAsync(
        [Body] DismissMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建会议投票主题
    /// <para>为指定的会议创建投票，该接口支持多问题投票。</para>
    /// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员；
    /// 每个投票支持添加 10 个问题；投票主题最多 50 个字符、描述最多 100 个字符；
    /// 每个问题最多 10 个选项、最少 2 个选项，每个选项最多 36 个字符。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateMeetingPollThemeRequest"/>：operator_userid / instance_id / meetingid / poll_topic / poll_desc / is_anony / poll_questions）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投票主题 ID（poll_theme_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98834"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/create_theme")]
    Task<CreateMeetingPollThemeResponse> CreatePollThemeAsync(
        [Body] CreateMeetingPollThemeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改会议投票主题
    /// <para>修改投票主题信息，目前仅支持全覆盖修改。</para>
    /// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员；
    /// 每个投票支持添加 10 个问题；每个问题最多 10 个选项、最少 1 个选项。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateMeetingPollThemeRequest"/>：operator_userid / instance_id / meetingid / poll_theme_id / poll_topic / poll_desc / is_anony / poll_questions）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98835"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/update_theme")]
    Task<WechatWorkResponse> UpdatePollThemeAsync(
        [Body] UpdateMeetingPollThemeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议投票列表
    /// <para>获取指定会议的投票列表（按投票主题分组）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingPollListRequest"/>：operator_userid / instance_id / meetingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投票主题信息列表（polls_theme_info：poll_theme_id / polls_info：poll_id / poll_topic / status / is_shared / is_anony）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98836"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/get_poll_list")]
    Task<GetMeetingPollListResponse> GetPollListAsync(
        [Body] GetMeetingPollListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议投票主题信息
    /// <para>获取投票主题的详细信息，包括问题、选项，但不包括投票结果。</para>
    /// <para>官方限制：本端点 <c>meetingid</c> 为非必填（本组投票端点中唯一可选）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingPollThemeInfoRequest"/>：operator_userid / instance_id / meetingid / poll_theme_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投票主题（poll_topic）、投票描述（poll_desc）、是否匿名（is_anony）与投票问题数组
    /// （poll_question_data：question_desc / question_type / option_info：option_desc）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98837"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/get_theme_info")]
    Task<GetMeetingPollThemeInfoResponse> GetPollThemeInfoAsync(
        [Body] GetMeetingPollThemeInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议投票详情
    /// <para>获取投票的详情，包括问题、选项、投票结果。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingPollDetailRequest"/>：operator_userid / instance_id / meetingid / poll_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投票主题（poll_theme_id / poll_topic / poll_desc / is_anony / status / is_shared / vote_total_num）与投票结果数组
    /// （poll_question_data：question_desc / question_type / question_id / option_info：option_id / option_desc / option_num / rate / option_user：userid / tmp_openid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98838"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/get_poll_detail")]
    Task<GetMeetingPollDetailResponse> GetPollDetailAsync(
        [Body] GetMeetingPollDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会议投票
    /// <para>删除会议投票：传入投票主题 ID 则删除投票主题（不影响投票实例），传入投票 ID 则删除投票实例。</para>
    /// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员；
    /// 投票主题 ID 和投票 ID 二选一，如果都传入，会使用投票 ID；当主题下所有投票实例被删，则投票主题也被删除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteMeetingPollRequest"/>：operator_userid / instance_id / meetingid / poll_theme_id / poll_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98839"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/delete")]
    Task<WechatWorkResponse> DeletePollAsync(
        [Body] DeleteMeetingPollRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发起会议投票
    /// <para>使用已有的投票主题发起投票。</para>
    /// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="StartMeetingPollRequest"/>：operator_userid / instance_id / meetingid / poll_theme_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投票 ID（poll_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98840"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/start")]
    Task<StartMeetingPollResponse> StartPollAsync(
        [Body] StartMeetingPollRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 结束会议投票
    /// <para>结束指定会议的投票。</para>
    /// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="FinishMeetingPollRequest"/>：operator_userid / instance_id / meetingid / poll_theme_id / poll_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98841"/></para>
    /// <para>官方权限：仅配置在「可调用接口的应用」列表中的自建应用可调用；仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/poll/finish")]
    Task<WechatWorkResponse> FinishPollAsync(
        [Body] FinishMeetingPollRequest request,
        CancellationToken cancellationToken = default);
}

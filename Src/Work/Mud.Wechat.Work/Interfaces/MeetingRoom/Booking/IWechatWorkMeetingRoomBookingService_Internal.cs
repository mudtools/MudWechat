// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.MeetingRoom;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议室预定管理」模块企业自建应用 SDK
/// （查询会议室预定信息 + 预定会议室 + 按日程预定 + 按会议预定 + 取消预定 + 获取预定详情，共 6 端点）。
/// <para>
/// 官方「会议室」文档树只在企业自建应用开发根下存在页面，本域端点全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalDialService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// </para>
/// <para>
/// 官方权限说明（93620「权限说明」一节原文）：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中，
/// 代开发应用与第三方应用均为「暂不支持」——故本族只有唯一的自建子接口。
/// 官方注：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// 官方频率：本族 6 个页面小节均无独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
/// （每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；每 IP 不可超过 2 万次/分、60 万次/小时），
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>。
/// </para>
/// <para>
/// 官方「会议室预定管理」页（2024-06-24 正文）只含上述 6 个小节：曾由第三方 SDK 转述的
/// <c>/cgi-bin/oa/meetingroom/get_booking_info_by_meeting_id</c> 已不在该页与「会议室」树任何节点内，
/// 本域不声明该端点，由 RD1 零声明守卫持有（追踪号 P2-QY-MEETINGROOM-BYMEETINGID-07）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MeetingRoom",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingRoomBookingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingRoomBookingService : IWechatWorkMeetingRoomBookingService
{
    /// <summary>
    /// 查询会议室预定信息
    /// <para>按会议室或按城市 / 楼宇 / 楼层与时间范围查询会议室的排期与预定状态。</para>
    /// <para>官方限制：<b>不支持跨天查询</b>；start_time 默认为当前时间、end_time 默认为明日 0 时；
    /// 若按位置信息查询，须保证其上一级位置信息已填写（如用楼宇过滤必须同时填写城市）；
    /// 条目 status 官方口径为 0-已预定 / 2-申请中 / 3-审批中（本端点不回传 1-已取消）；
    /// schedule_id 在会议室已取消预定（未保留日历）的情况下无法再获取到日程详情。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingRoomBookingInfoRequest"/>：meetingroom_id 与 city / building / floor 两种定位方式，start_time / end_time 限定时间窗）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预定列表（官方 booking_list，元素为「会议室 ID + schedule 排期数组」，见 <see cref="MeetingRoomBooking"/>）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：本族 6 端点均只在企业自建应用文档树内开放；
    /// 官方响应中的排期容器名为单数 <c>schedule</c> 但形态是数组，字段名照官方原文保留（见守卫 MR5）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/get_booking_info")]
    Task<GetMeetingRoomBookingInfoResponse> GetMeetingRoomBookingInfoAsync(
        [Body] GetMeetingRoomBookingInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 预定会议室
    /// <para>以时间区间直接预定会议室并创建会议与关联日程。</para>
    /// <para>官方限制：最小预定时长为 30 分钟；预定与结束时间会<b>自动按 30 分钟取整</b>（官方例：传入 15:15 与 15:45 会自动取整为 15:00 与 16:00）；
    /// 本端点<b>仅可预定无需审批的会议室</b>；当前时间已晚于预定时间时按三条规则处理——已过结束时间则不允许预定、
    /// 处于开始时间 15 分钟内则允许预定、超过开始时间 15 分钟则自动把开始时间顺延一个 30 分钟窗口。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomBookRequest"/>：meetingroom_id / start_time / end_time / booker 必填，subject / attendees 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预定 ID（booking_id）与关联日程 ID（schedule_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：booker 为预定者的成员账号（userid），attendees 为参与人的 userid 列表；
    /// 与「按日程 / 按会议预定」的差异是本端点由会议室侧生成日程并回传 schedule_id。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/book")]
    Task<MeetingRoomBookResponse> BookMeetingRoomAsync(
        [Body] MeetingRoomBookRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按日程预定会议室
    /// <para>以已存在的日程 ID 抢占预定会议室，支持重复日程预定。</para>
    /// <para>官方限制：schedule_id <b>仅可使用同应用创建的日程</b>；本端点仅可预定无需审批的会议室；
    /// 预定后该日程将<b>不能</b>再通过更新日程接口编辑，只能调用新增 / 删除日程参与者接口；
    /// 若需更新日程时间等字段，须先取消会议室预定、再更新日程、之后重新预定。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomBookByScheduleRequest"/>：meetingroom_id + schedule_id + booker）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预定 ID（booking_id）；冲突日期以官方 conflict_date 时间戳数组返回。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：官方把本端点归为「通过日程预定会议室」，schedule_id 取自建日程接口创建的日程 ID；
    /// 响应字段面与 <see cref="BookMeetingRoomAsync"/> 不同：本端点回传 booking_id 与 conflict_date（当天 0 点时间戳数组，
    /// 重复日程部分日期与既有预定冲突时返回），不回传 schedule_id。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/book_by_schedule")]
    Task<MeetingRoomBookByScheduleResponse> BookMeetingRoomByScheduleAsync(
        [Body] MeetingRoomBookByScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按会议预定会议室
    /// <para>把已创建的会议挂到会议室上（复用会议已有信息生成预定），支持重复会议预定。</para>
    /// <para>官方限制：meetingid <b>仅可使用同应用创建的会议</b>；本端点仅可预定无需审批的会议室；
    /// 冲突语义与按日程侧一致（conflict_date 为当天 0 点时间戳数组）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomBookByMeetingRequest"/>：meetingroom_id + meetingid + booker）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>预定 ID（booking_id）；冲突日期以官方 conflict_date 时间戳数组返回。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：<b>官方契约陷阱</b>：本端点的会议 ID 请求字段官方拼写为 <c>meetingid</c>（无下划线），
    /// 与同域其余端点的 <c>meeting_id</c> 不同名，字段名照官方原文保留（见守卫 MR5）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/book_by_meeting")]
    Task<MeetingRoomBookByMeetingResponse> BookMeetingRoomByMeetingAsync(
        [Body] MeetingRoomBookByMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消会议室预定
    /// <para>按预定 ID 取消会议室预定，并可选择是否保留关联日程、只取消重复预定中的某一天。</para>
    /// <para>官方限制：booking_id 必填（官方口径为「会议室的预定 id」，<b>不是</b>会议 ID）；
    /// keep_schedule 为 0-同步删除 / 1-保留，且<b>仅对非重复日程有效</b>；
    /// cancel_date 针对重复日程——不填表示取消所有重复预定，填写（当天 0 点时间戳）表示仅取消对应日期当天的预定。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomCancelBookRequest"/>：booking_id 必填，keep_schedule / cancel_date 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方成功响应仅有 errcode / errmsg，无业务字段。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：官方响应体不含业务字段，故返回 <see cref="WechatWorkResponse"/>，不另建响应 DTO；
    /// 取消后该预定的 schedule_id 若未保留日历，将无法再经日程接口取到详情。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/cancel_book")]
    Task<WechatWorkResponse> CancelMeetingRoomBookAsync(
        [Body] MeetingRoomCancelBookRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会议室预定详情
    /// <para>按预定 ID 查询该预定的会议室与排期信息。</para>
    /// <para>官方限制：meetingroom_id 与 booking_id 均必填；本端点条目的 status 官方口径比查询侧多一档——
    /// 0-已预定 / 1-已取消 / 2-申请中 / 3-审批中；附录另述审批流转（无需审批：申请即已预定；
    /// 需审批：申请中 → 提交审批后审批中 → 审批完成后已预定，再取消为已取消）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MeetingRoomBookInfoGetRequest"/>：meetingroom_id + booking_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议室 ID 与预定排期（官方 schedule 对象形态，比查询侧多回传 booking_id / master_booking_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93620"/></para>
    /// <para>官方权限：自建应用须配置到「应用管理 - 会议室 - 可调用接口的应用」中；代开发与第三方应用官方标注为「暂不支持」。</para>
    /// <para>官方业务限制：路由为 <c>bookinfo/get</c>（斜杠分层），与本域其余端点的平级路由形态不同；
    /// 官方以 master_booking_id 回传主预定 ID（该预定是某个周期性预定的一部分时返回，字段面见 <see cref="MeetingRoomSchedule"/>）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/meetingroom/bookinfo/get")]
    Task<MeetingRoomBookInfoGetResponse> GetMeetingRoomBookInfoAsync(
        [Body] MeetingRoomBookInfoGetRequest request,
        CancellationToken cancellationToken = default);
}

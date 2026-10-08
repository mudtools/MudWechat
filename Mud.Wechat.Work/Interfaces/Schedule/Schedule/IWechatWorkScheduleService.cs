// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Schedule;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「日程」模块管理日程域公共 SDK（创建日程 + 更新日程 + 新增/删除日程参与者 + 获取日历下的日程列表 + 获取日程详情 + 取消日程）。
/// <para>
/// 官方对三类应用开放一致的 7 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalScheduleService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderScheduleService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyScheduleService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 官方约束：日程管理员须在共享成员列表中且最多 3 人；参与者累计最多 1000 人；
/// 更新日程是<b>覆盖式</b>而不是增量式（增量更新参与人用新增/删除日程参与者端点）；创建者与日程所属日历 ID 不可更新；
/// 已预约会议室的日程无法经更新端点修改会议室关联字段，须先取消会议室预定再更新；
/// 被取消的日程仍可拉取详情（status = 1），调用方须自行检查 status 字段。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkScheduleService
{
    /// <summary>
    /// 创建日程
    /// <para>在日历中创建一个日程（返回日程 ID <c>schedule_id</c>）。</para>
    /// <para>官方限制：管理员须在共享成员内且最多 3 人；参与者最多 1000 人；标题 0 ~ 128 字符（不填默认「新建事件」）、
    /// 描述不多于 1000 字符、地址不多于 128 字符；提醒时间仅支持官方固定档位（详见 <see cref="ScheduleReminders"/>）；
    /// 自建应用 <c>cal_id</c> 不填时写入应用默认日历，第三方应用必须指定 <c>cal_id</c>（不多于 64 字节）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddScheduleRequest"/>：schedule 日程信息（schedule_id / cal_id / admins / attendees / summary / description / reminders / location / start_time / end_time / is_whole_day） / agentid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日程 ID（schedule_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93648"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93703"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96824"/></para>
    /// <para>官方契约提醒：<c>agentid</c> 仅旧的第三方多应用套件需填；
    /// <c>remind_time_diffs</c> 非空时优先于 <c>remind_before_event_secs</c>（两者仅一个生效，官方建议改用 remind_time_diffs）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/add")]
    Task<AddScheduleResponse> AddScheduleAsync(
        [Body] AddScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新日程
    /// <para>在日历中更新指定日程；重复日程的三种操作模式（0-默认全部修改；1-仅修改此日程；2-修改将来的所有日程）详见官方「更新重复日程」说明页。</para>
    /// <para>官方限制：<b>更新操作是覆盖式，而不是增量式</b>（增量更新参与人用 <see cref="AddScheduleAttendeesAsync"/> / <see cref="DelScheduleAttendeesAsync"/>）；
    /// 创建者和日程所属日历 ID 不可更新；已预约会议室的日程无法通过本端点更新会议室关联字段
    ///（start_time / end_time / is_whole_day / is_repeat / repeat_type / repeat_until / is_custom_repeat / repeat_interval /
    /// repeat_day_of_week / repeat_day_of_month / timezone），须先取消会议室预定再更新。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateScheduleRequest"/>：skip_attendees / op_mode / op_start_time / schedule 日程信息（schedule_id / admins / attendees / summary / description / reminders / location / start_time / end_time / is_whole_day））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日程 ID（schedule_id；修改重复日程时为新产生的日程 ID，非全部周期修改时会修剪原日程并生成新日程）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97720"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97787"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97761"/></para>
    /// <para><b>更新重复日程</b>操作模式说明（与本端点同一路由，非独立 HTTP API）——
    /// 自建：<see href="https://developer.work.weixin.qq.com/document/path/96204"/>；
    /// 第三方：<see href="https://developer.work.weixin.qq.com/document/path/96198"/>；
    /// 服务商代开发：<see href="https://developer.work.weixin.qq.com/document/path/96826"/></para>
    /// <para>官方契约提醒：非周期日程指定 op_mode 为 1 或 2 会报错 90485；op_start_time 匹配不到某次周期的开始时间会报错 90482；
    /// 「仅修改此日程」模式下不能将重复性参数指定为重复（报错 90484）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/update")]
    Task<UpdateScheduleResponse> UpdateScheduleAsync(
        [Body] UpdateScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增日程参与者
    /// <para>在日历中更新指定的日程参与者列表；本端点为<b>增量式</b>请求方式（仅新增，不覆盖已有参与者）。</para>
    /// <para>官方限制：参与者累计最多 1000 人，单个参与者 ID 不多于 64 字节。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddScheduleAttendeesRequest"/>：schedule_id / attendees 参与者列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97721"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97789"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97763"/></para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/add_attendees")]
    Task<WechatWorkResponse> AddScheduleAttendeesAsync(
        [Body] AddScheduleAttendeesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除日程参与者
    /// <para>在日历中更新指定的日程参与者列表（从日程中移除指定参与者）；本端点为<b>增量式</b>请求方式（仅移除，不覆盖其余参与者）。</para>
    /// <para>官方限制：参与者列表最多可添加 1000 人，单个参与者 ID 不多于 64 字节。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelScheduleAttendeesRequest"/>：schedule_id / attendees 参与者列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97722"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97794"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97764"/></para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/del_attendees")]
    Task<WechatWorkResponse> DelScheduleAttendeesAsync(
        [Body] DelScheduleAttendeesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取日历下的日程列表
    /// <para>获取指定日历下的日程列表；仅可获取应用自己创建的日历下的日程。</para>
    /// <para>官方限制：分页参数 <c>offset</c> 默认 0、<c>limit</c> 默认 500（取值范围 1 ~ 1000）；
    /// 当返回的 schedule_list 为空表示 offset 过大应终止获取，有新增日程时可在原基础上继续增量获取；
    /// 被取消的日程也会返回，调用者需检查 status 字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListSchedulesByCalendarRequest"/>：cal_id / offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日程列表（schedule_list：schedule_id / admins / attendees（response_status）/ summary / description / reminders / location / status / start_time / end_time / sequence / cal_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97723"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97796"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97765"/></para>
    /// <para>官方契约提醒：本域分页采用 offset + limit（区别于多数域的 cursor + limit）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/get_by_calendar")]
    Task<ListSchedulesByCalendarResponse> ListSchedulesByCalendarAsync(
        [Body] ListSchedulesByCalendarRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取日程详情
    /// <para>获取指定的日程详情。</para>
    /// <para>官方限制：<c>schedule_id_list</c> 一次最多拉取 1000 条；被取消的日程也可拉取详情，调用方须自行检查 status 字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetScheduleRequest"/>：schedule_id_list 日程 ID 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>日程列表（schedule_list：schedule_id / admins / attendees（response_status / event_time）/ summary / description / reminders（含 exclude_time_list）/ location / status / start_time / end_time / is_whole_day / cal_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97724"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97798"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97766"/></para>
    /// <para>官方契约提醒：响应 reminders 的 exclude_time_list 为重复日程排除的日期列表（对重复日程修改/删除特定一天或多天时原日程排除对应日期）。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/get")]
    Task<GetScheduleResponse> GetScheduleAsync(
        [Body] GetScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消日程
    /// <para>取消指定的日程（官方路由为 schedule/del）；重复日程可经 op_mode 指定取消范围：0-默认删除所有日程；1-仅删除此日程；2-删除本次及后续日程。</para>
    /// <para>官方限制：<c>op_start_time</c> 仅当操作模式是 1 或 2 时有效，该时间必须是重复日程的某一次开始时间；
    /// 被取消的日程仍可拉取详情（status = 1）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DelScheduleRequest"/>：schedule_id / op_mode / op_start_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97725"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97799"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97767"/></para>
    /// </remarks>
    [Post("/cgi-bin/oa/schedule/del")]
    Task<WechatWorkResponse> DelScheduleAsync(
        [Body] DelScheduleRequest request,
        CancellationToken cancellationToken = default);
}

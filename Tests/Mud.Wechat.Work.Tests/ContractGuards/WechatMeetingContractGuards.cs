// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 会议模块（Meeting 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 四功能族形态：
/// 预约会议基础管理族（创建/修改/取消/获取成员会议 ID 列表 4 端点官方对三类应用开放一致，
/// 收敛声明于公共父接口；获取会议详情为自建/第三方差异端点——同路由两分支子接口各自声明，
/// 服务商代开发章节未开放该端点、其子接口为零端点空标记）；
/// 会议统计管理族（获取会议发起记录官方仅自建应用开放——第三方/代开发章节未提供会议统计管理文档页，
/// 零端点父接口 + 唯一自建子接口承载端点）；
/// 预约会议高级管理族（19 端点官方仅自建应用开放——第三方应用开发与服务商代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；创建/修改/取消预约会议、获取会议详情、获取成员会议 ID 列表
/// 5 个高级管理文档页与基础管理族同路由，不重复声明端点、仅将请求/响应 DTO 扩展为高级文档参数超集）；
/// 会中控制管理族（17 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；会控 9 端点挂 realcontrol 段、投票 8 端点挂 poll 段）；
/// 网络研讨会管理族（14 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；全部挂 webinar 段，报名 7 端点挂 webinar/enroll 子段，
/// 报名问题/报名 ID/报名信息结构官方与普通会议报名同构、共用同一批嵌套 DTO）；
/// 电话入会（PSTN）管理族（3 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；全部挂 phone 段）；
/// Rooms 会议室管理族（12 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；全部挂 rooms 段；获取资源端点官方为无请求体 POST）；
/// 会议室连接器（MRA）管理族（4 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；全部挂 mra 段）；
/// 会议布局和背景管理族（15 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；基础布局与背景挂 layout 段、高级布局挂 advanced_layout 段；
/// 获取布局模板列表为会议域唯一 GET 端点；基础/高级布局座次结构不同构分型承载）；
/// 录制管理族（10 端点官方仅自建应用开放——第三方/代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；挂 record 段、转写挂 record/transcript 子段）；
/// 高级功能账号管理族（3 个官方文档页承载 5 端点——分配/取消文档页各含提交任务与查询结果两端点，
/// 官方仅自建应用开放，零端点父接口 + 唯一自建子接口承载端点；全部挂 vip 段）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：会议域 105 条路由中 104 条官方即 POST、仅获取布局模板列表为 GET
/// （含仅查询语义的 meeting/get_info / meeting/get_user_meetingid / meeting/statistics/get_start_list /
/// meeting/get_invitees / meeting/get_customer_short_url / meeting/get_realtime_attendee_list /
/// meeting/get_attendee_list / meeting/waitingroom/* / meeting/check_device_in_meeting / meeting/get_guests /
/// meeting/get_quality / meeting/enroll/* / meeting/realcontrol/* / meeting/poll/* / meeting/webinar/* /
/// meeting/phone/* / meeting/rooms/* / meeting/mra/* / meeting/layout/* / meeting/advanced_layout/* /
/// meeting/record/* / meeting/vip/*，其中仅 layout/list_template 为 GET，勿「顺手统一」为 POST）；
/// 会议 ID 官方字段名作 <c>meetingid</c>（无下划线）、列表作 <c>meetingid_list</c>；获取成员会议 ID 列表用
/// cursor+limit 翻页（cursor 初次调用可填 "0"）；会议统计管理路由挂 <c>/cgi-bin/meeting/statistics/</c> 段；
/// 高级管理报名配置与等候室路由挂 <c>/cgi-bin/meeting/enroll/</c> 与 <c>/cgi-bin/meeting/waitingroom/</c> 段；
/// 会控与投票路由挂 <c>/cgi-bin/meeting/realcontrol/</c> 与 <c>/cgi-bin/meeting/poll/</c> 段；
/// 网络研讨会与电话入会路由挂 <c>/cgi-bin/meeting/webinar/</c> 与 <c>/cgi-bin/meeting/phone/</c> 段；
/// Rooms 会议室与 MRA 路由挂 <c>/cgi-bin/meeting/rooms/</c> 与 <c>/cgi-bin/meeting/mra/</c> 段；
/// 布局/背景、高级布局、录制、高级功能账号路由挂 <c>/cgi-bin/meeting/layout/</c>、
/// <c>/cgi-bin/meeting/advanced_layout/</c>、<c>/cgi-bin/meeting/record/</c> 与 <c>/cgi-bin/meeting/vip/</c> 段；
/// 获取实时会中成员列表官方请求示例将分页游标误写为 <c>cursort</c>、参数表为 <c>cursor</c>，以参数表为准；
/// 会控单数命名 <c>operated_user</c>（管理联席主持人/静音成员/关闭屏幕共享/开关成员视频）承载单个对象
/// （参数表 object[] 标注为文档笔误），复数命名 <c>operated_users</c>（管理等候室成员/移出成员/修改昵称）承载数组；
/// 修改成员昵称参数表 <c>opereated_users</c>、开关成员视频参数表 <c>instanceid</c>、静音成员参数表
/// <c>option</c> 标注 string 均为文档笔误，以官方示例（operated_users / instance_id / bool）为准；
/// 网络研讨会详情响应主题字段参数表作 <c>subject</c>、示例作 <c>title</c>（与创建响应一致），以示例为准；
/// 详情响应 media_setting 的入会静音字段示例作 <c>mute_enable_join</c>（参数表作 enable_enter_mute），
/// 与请求形态分型承载；详情响应 status 为字符串枚举（MEETING_STATE_*）；
/// 网络研讨会 start_time/end_time 参数表与示例均为字符串形态时间戳（单位秒）；
/// 录制列表响应字段以参数表 <c>record_list</c>/<c>record_file_list</c> 为准（示例误写 record_meetings/record_files）；
/// 单个录制文件详情的 start_time/end_time 参数表标注 int64（示例为字符串形态）、meeting_summary 参数表标注 object[]（示例为单对象），
/// 均以参数表为准；
/// 创建预约会议响应 meetingid 可用于「进入会议」接口（小程序/JS-SDK）；
/// 创建/修改预约会议请求与获取会议详情响应的 settings/reminders/invitees
/// 三嵌套对象官方参数表高度同构，本 SDK 以共用结构承载；
/// <c>remind_before</c> 为秒数数组（仅支持 0/300/900/3600/86400），非单值；
/// 获取受邀成员列表的 <c>invitees</c> 为 <see cref="MeetingInvitee"/> 对象数组，与创建/修改请求的
/// <see cref="MeetingInvitees"/>（userid 字符串数组包裹对象）同名字段两种形态并存；
/// 删除会议报名信息请求的 <c>enroll_id_list</c> 为对象数组（<see cref="MeetingEnrollIdRef"/>），
/// 而审批会议报名信息请求的同名字段为字符串数组（普通会议与网络研讨会报名域同构同规）。
/// </para>
/// </remarks>
public class WechatMeetingContractGuards
{
    private const string MeetingRegistryGroupName = "Meeting";

    // ------------------------------------------------------------------
    // 路由表：59 条官方路由（get_info 同路由两分支；高级管理文档页与基础管理族 5 条同路由不重复建端点），
    // 全部 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingBaseRoutes =
        {
            // 创建预约会议（自建 99104 / 第三方 93706 / 代开发 97454；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.CreateMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/create"),
            // 修改预约会议（自建 99047 / 第三方 93710 / 代开发 97455；三类公共收敛父接口；
            // meeting_start 与 meeting_duration 须成对修改）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.UpdateMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/update"),
            // 取消预约会议（自建 99048 / 第三方 93709 / 代开发 97456；三类公共收敛父接口；仅预约状态可取消）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.CancelMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/cancel"),
            // 获取成员会议 ID 列表（自建 99050 / 第三方 93707 / 代开发 97457；三类公共收敛父接口）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.GetUserMeetingIdListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_user_meetingid"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingInfoRoutes =
        {
            // 获取会议详情（自建 99049 / 第三方 93708；自建/第三方差异端点——同路由两分支子接口各自声明；
            // 服务商代开发章节未开放该端点；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingService),
                nameof(IWechatWorkInternalMeetingService.GetMeetingInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_info"),
            (typeof(IWechatWorkThirdPartyMeetingService),
                nameof(IWechatWorkThirdPartyMeetingService.GetMeetingInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingStatisticsRoutes =
        {
            // 获取会议发起记录（自建 99651；官方仅自建应用开放；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingStatisticsService),
                nameof(IWechatWorkInternalMeetingStatisticsService.GetMeetingStartListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/statistics/get_start_list"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingAdvancedRoutes =
        {
            // 获取会议受邀成员列表（自建 98160；官方仅自建应用开放；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetInviteesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_invitees"),
            // 更新会议受邀成员列表（自建 98162；最多 2000 名受邀成员，管理员必须在受邀成员列表中）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetInviteesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/set_invitees"),
            // 创建用户专属参会链接（自建 98818；不支持网络研讨会；customer_data 需 Base64 编码且 ≤256 字节）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.CreateCustomerShortUrlAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/create_customer_short_url"),
            // 获取用户专属参会链接（自建 98819；不支持个人会议号会议、网络研讨会）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetCustomerShortUrlAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_customer_short_url"),
            // 获取实时会中成员列表（自建 98157；官方请求示例 cursort 为拼写陷阱，参数表为 cursor）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetRealtimeAttendeeListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_realtime_attendee_list"),
            // 获取已参会成员列表（自建 98156；时间区间 ≤31 天，时间跨度最大 90 天）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetAttendeeListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_attendee_list"),
            // 获取实时等候室成员列表（自建 98163；需开启等候室且会议进行中；路由挂 waitingroom 段）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetWaitingRoomCurrentUserListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/waitingroom/get_current_user_list"),
            // 获取等候室成员记录（自建 98164；会前/会中/会后均可获取）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetWaitingRoomUserListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/waitingroom/get_user_list"),
            // 获取成员设备是否入会（自建 98165）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.CheckDeviceInMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/check_device_in_meeting"),
            // 获取会议嘉宾列表（自建 99039）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_guests"),
            // 更新会议嘉宾列表（自建 99040）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/set_guests"),
            // 获取会议健康度（自建 98821；已结束会议；start_time 查询区间为过去 7 天到现在）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetQualityAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_quality"),
            // 修改会议报名配置（自建 98797；需会议已开启报名；路由挂 enroll 段）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/set_config"),
            // 获取会议报名配置（自建 98800；未开启报名返回错误）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/get_config"),
            // 获取会议成员报名 ID（自建 98794；tmp_openid_list 单次最多 500 条）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.QueryEnrollIdsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/query_by_tmp_openid"),
            // 获取会议报名信息（自建 98810）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ListEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/list"),
            // 审批会议报名信息（自建 98807；enroll_id_list 为字符串数组）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ApproveEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/approve"),
            // 导入会议报名信息（自建 98816）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ImportEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/import"),
            // 删除会议报名信息（自建 98817；enroll_id_list 为对象数组）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.DeleteEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/delete"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingControlRoutes =
        {
            // 管理会中设置（自建 98175；allow_unmute_self 需 mute_all=true 才生效；暂不支持 MRA 被操作）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.SetRealtimeSettingsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/set"),
            // 管理联席主持人（自建 98180；action 为布尔设置/撤销；暂不支持 MRA 被操作）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.SetCoHostAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/set_cohost"),
            // 静音成员（自建 98184；option 参数表标 string、示例为 bool，以示例为准）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.MuteUserAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/mute_user"),
            // 关闭或开启成员视频（自建 98189；video=true 仅支持 MRA 设备；参数表 instanceid 为笔误，示例为 instance_id）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.SwitchUserVideoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/switch_user_video"),
            // 关闭成员屏幕共享（自建 98185；暂不支持 MRA 被操作）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.CloseScreenShareAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/close_screen_share"),
            // 修改成员在会中显示的昵称（自建 98188；参数表 opereated_users 为笔误，示例为 operated_users）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.SetUserNicknamesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/set_nicknames"),
            // 管理等候室成员（自建 98186；allow_rejoin 仅 operate_type=3 时允许设置）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.ManageWaitingRoomUsersAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/manage_waiting_room_users"),
            // 移出成员（自建 98181）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.KickoutUsersAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/kickout_users"),
            // 结束会议（自建 98187；周期性会议还有子会议时 retrieve_code 须为 0）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.DismissMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/realcontrol/dismiss"),
            // 创建会议投票主题（自建 98834；仅进行中的会议；操作者须为主持人或会议管理员）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.CreatePollThemeAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/create_theme"),
            // 修改会议投票主题（自建 98835；目前仅支持全覆盖修改）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.UpdatePollThemeAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/update_theme"),
            // 获取会议投票列表（自建 98836）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.GetPollListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/get_poll_list"),
            // 获取会议投票主题信息（自建 98837；meetingid 为投票组端点中唯一非必填）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.GetPollThemeInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/get_theme_info"),
            // 获取会议投票详情（自建 98838；含投票结果）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.GetPollDetailAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/get_poll_detail"),
            // 删除会议投票（自建 98839；poll_theme_id 与 poll_id 二选一，都传以投票 ID 为准）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.DeletePollAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/delete"),
            // 发起会议投票（自建 98840；使用已有投票主题发起）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.StartPollAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/start"),
            // 结束会议投票（自建 98841；路由官方作 finish，勿「顺手归一」为 cancel/stop）。
            (typeof(IWechatWorkInternalMeetingControlService),
                nameof(IWechatWorkInternalMeetingControlService.FinishPollAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/poll/finish"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingWebinarRoutes =
        {
            // 创建网络研讨会（自建 98842；admission_type=2 时 password 必传；playback_for_audience 开启须开云录制）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.CreateWebinarAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/create"),
            // 修改网络研讨会（自建 98843；media_setting 参数表 object[] 为笔误，示例为单对象）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/update"),
            // 取消网络研讨会（自建 98870）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.CancelWebinarAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/cancel"),
            // 获取网络研讨会详情（自建 98860；meetingid/meeting_code 二选一；响应 subject/title、mute_enable_join 均以示例为准）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.GetWebinarAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/get"),
            // 获取网络研讨会嘉宾列表（自建 98871；guests 参数表标 object、示例为数组）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.ListWebinarGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/list_guest"),
            // 更新网络研讨会嘉宾列表（自建 98872）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/update_guest_list"),
            // 管理网络研讨会暖场配置（自建 98882；图片与视频二选一，同时传以图片为准）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarWarmUpAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/update_warm_up"),
            // 修改网络研讨会报名配置（自建 98875；特殊问题类型额外支持 6 - 组织规模）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.SetWebinarEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/set_config"),
            // 获取网络研讨会报名配置（自建 98874）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.GetWebinarEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/get_config"),
            // 获取网络研讨会成员报名 ID（自建 98873；tmp_openid_list 单次最多 500 条）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.QueryWebinarEnrollIdsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid"),
            // 获取网络研讨会报名信息（自建 98876）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.ListWebinarEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/list"),
            // 审批网络研讨会报名信息（自建 98877）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.ApproveWebinarEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/approve"),
            // 导入网络研讨会报名信息（自建 98880）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.ImportWebinarEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/import"),
            // 删除网络研讨会报名信息（自建 98881）。
            (typeof(IWechatWorkInternalMeetingWebinarService),
                nameof(IWechatWorkInternalMeetingWebinarService.DeleteWebinarEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/webinar/enroll/delete"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingPstnRoutes =
        {
            // 批量外呼（自建 98823；单次最多 50 路；支持境外电话号及分机号；Webinar 暂不支持外呼）。
            (typeof(IWechatWorkInternalMeetingPstnService),
                nameof(IWechatWorkInternalMeetingPstnService.BatchCalloutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/phone/callout"),
            // 获取会议的外呼状态（自建 98824；不针对呼叫人做数据隔离；分页最大 100）。
            (typeof(IWechatWorkInternalMeetingPstnService),
                nameof(IWechatWorkInternalMeetingPstnService.GetCalloutStatusAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/phone/get_callout_status"),
            // 获取电话入会的成员 ID（自建 98825；phone_numbers 上限 20 个；同一座机号可能对应多个 tmp_openid）。
            (typeof(IWechatWorkInternalMeetingPstnService),
                nameof(IWechatWorkInternalMeetingPstnService.GetTmpOpenidAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/phone/get_tmp_openid"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingRoomsRoutes =
        {
            // 预定 Rooms 会议室（自建 98791；会议时长不得大于 24 小时且不支持周期性会议）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.BookMeetingRoomAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/book"),
            // 释放 Rooms 会议室（自建 98792）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.ReleaseMeetingRoomAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/release"),
            // 获取 Rooms 会议室列表（自建 98795；limit 最大 50、默认 20）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.ListMeetingRoomsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/list"),
            // 获取 Rooms 会议室详情（自建 98793；basic/account/hardware/pmi 四段信息）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.GetMeetingRoomInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/get_info"),
            // 获取 Rooms 会议室配置项（自建 98802；会议配置 + 录制配置两段）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.GetMeetingRoomConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/get_config"),
            // 获取 Rooms 会议室下的会议列表（自建 98796；meeting_room_id 与 rooms_id 二者填其一；时间区间 ≤90 天）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.ListMeetingRoomMeetingsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/list_meetings"),
            // 获取设备列表（自建 98798）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.ListRoomDevicesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/list_devices"),
            // 获取控制器列表（自建 98799；status 为字符串形态 "0"/"1"）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.ListRoomControllersAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/list_controllers"),
            // 获取 Rooms 会议室资源（自建 98809；官方为无请求体 POST）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.GetMeetingRoomInventoryAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/get_inventory"),
            // 呼叫 Rooms 会议室（自建 98804；meeting_room_id 与 mra_address 二选一）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.CallMeetingRoomAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/call"),
            // 取消呼叫 Rooms 会议室（自建 98805）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.CancelCallMeetingRoomAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/cancel_call"),
            // 获取 Rooms 会议室应答状态（自建 98806；status 0~6，仅 Rooms 有「取消呼叫」状态）。
            (typeof(IWechatWorkInternalMeetingRoomsService),
                nameof(IWechatWorkInternalMeetingRoomsService.GetMeetingRoomResponseStatusAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/rooms/get_response_status"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingMraRoutes =
        {
            // 获取 MRA 状态信息（自建 98786；instance_id=9 voip/sip 设备）。
            (typeof(IWechatWorkInternalMeetingMraService),
                nameof(IWechatWorkInternalMeetingMraService.QueryMraStatusAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/mra/query_status"),
            // 切换 MRA 默认布局（自建 98787；已显示自定义布局/个性布局/焦点视频时不支持设置）。
            (typeof(IWechatWorkInternalMeetingMraService),
                nameof(IWechatWorkInternalMeetingMraService.SetMraDefaultLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/mra/set_default_layout"),
            // 设置 MRA 举手或手放下（自建 98788）。
            (typeof(IWechatWorkInternalMeetingMraService),
                nameof(IWechatWorkInternalMeetingMraService.SetMraRaiseHandAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/mra/set_raise_hand"),
            // 挂断 MRA 呼叫（自建 98789）。
            (typeof(IWechatWorkInternalMeetingMraService),
                nameof(IWechatWorkInternalMeetingMraService.HangupMraAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/mra/hangup"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingLayoutRoutes =
        {
            // 获取布局模板列表（自建 98844；会议域唯一 GET 端点，勿「顺手统一」为 POST）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.ListLayoutTemplatesAsync),
                typeof(GetAttribute), "/cgi-bin/meeting/layout/list_template"),
            // 添加会议基础布局（自建 98845；一场会议最多 10 个布局）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.AddMeetingLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/add"),
            // 添加会议高级布局（自建 98861；最多 20 个高级布局，仅支持 H.323/SIP 会议室终端）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.AddAdvancedLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/add"),
            // 修改会议基础布局（自建 98846）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.UpdateMeetingLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/update"),
            // 修改会议高级布局（自建 98868；仅支持全量更新）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.UpdateAdvancedLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/update"),
            // 设置会议默认布局（自建 98847；selected_layout_id 传空恢复默认原始布局）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.SetDefaultMeetingLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/set_default"),
            // 设置高级布局（自建 98869；user_list 单次最多 20 个用户）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.ApplyAdvancedLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/apply"),
            // 获取会议布局列表（自建 98862；返回基础和高级自定义布局）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.ListMeetingLayoutsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/list"),
            // 获取用户布局（自建 98865；布局优先级：个性布局 > 自定义布局 > 默认布局）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.GetUserLayoutAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/get_user_layout"),
            // 批量删除布局（自建 98866；最多 20 个布局 ID；正在被应用的布局无法删除）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.BatchDeleteLayoutsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/advanced_layout/batch_delete"),
            // 添加会议背景（自建 98851；最多 7 个背景，PNG ≤10MB、分辨率最小 1920x1080；异步上传）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.AddMeetingBackgroundAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/add_background"),
            // 设置会议默认背景（自建 98852；传空恢复默认黑色背景）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.SetDefaultMeetingBackgroundAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/set_default_background"),
            // 获取会议背景列表（自建 98856）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.ListMeetingBackgroundsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/list_background"),
            // 删除会议背景（自建 98853）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.DeleteMeetingBackgroundAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/delete_background"),
            // 批量删除会议背景（自建 98854）。
            (typeof(IWechatWorkInternalMeetingLayoutService),
                nameof(IWechatWorkInternalMeetingLayoutService.BatchDeleteMeetingBackgroundsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/layout/batch_delete_background"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingRecordingRoutes =
        {
            // 获取会议录制列表（自建 98192；meetingid/meeting_code/userid 三选一；时间区间 ≤31 天；响应字段以参数表为准）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.ListMeetingRecordsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/list"),
            // 获取录制文件访问统计（自建 98209；按天维度返回）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.GetRecordStatisticsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/get_statistics"),
            // 修改会议录制共享设置（自建 98208）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.UpdateRecordSharingConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/update_sharing_config"),
            // 删除会议录制（自建 98206；删除会议录制 ID 对应的所有云录制文件）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.DeleteMeetingRecordAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/delete"),
            // 删除单个录制文件（自建 98207）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.DeleteRecordFileAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/delete_file"),
            // 获取单个录制文件详情（自建 98205；含会议纪要与录制转写文件列表）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.GetRecordFileAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/get_file"),
            // 获取会议录制地址（自建 98196；播放/下载地址默认 6 小时过期）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.GetRecordFileListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/get_file_list"),
            // 获取录制转写段落信息（自建 98212）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.GetRecordTranscriptParagraphsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/transcript/get_paragraph_list"),
            // 获取录制转写详情（自建 98211；段落→句子→词条三级结构）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.GetRecordTranscriptDetailAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/transcript/get_detail"),
            // 搜索录制转写（自建 98213）。
            (typeof(IWechatWorkInternalMeetingRecordingService),
                nameof(IWechatWorkInternalMeetingRecordingService.SearchRecordTranscriptAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/record/transcript/search"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingPremiumAccountRoutes =
        {
            // 分配高级功能账号（自建 99508 上半页；userid_list 单次最多 100 个）。
            (typeof(IWechatWorkInternalMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService.AssignPremiumAccountsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/vip/submit_batch_add_job"),
            // 查询分配高级功能账号结果（自建 99508 下半页）。
            (typeof(IWechatWorkInternalMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService.GetAssignPremiumAccountsResultAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/vip/batch_add_job_result"),
            // 取消高级功能账号（自建 99509 上半页；userid_list 单次最多 100 个）。
            (typeof(IWechatWorkInternalMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService.RevokePremiumAccountsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/vip/submit_batch_del_job"),
            // 查询取消高级功能账号结果（自建 99509 下半页）。
            (typeof(IWechatWorkInternalMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService.GetRevokePremiumAccountsResultAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/vip/batch_del_job_result"),
            // 获取高级功能账号列表（自建 99510；limit 默认 100、最大 200，必须用 has_more 判断是否继续请求）。
            (typeof(IWechatWorkInternalMeetingPremiumAccountService),
                nameof(IWechatWorkInternalMeetingPremiumAccountService.ListPremiumAccountsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/vip/list"),
        };

    // ------------------------------------------------------------------
    // MT1：全部端点路由与官方契约一致（65 条路由表项去重后 105 条官方路由，
    // get_info 同路由两分支；基础管理族 4 端点与高级管理族 5 个文档页同路由不重复建端点；
    // layout/list_template 为会议域唯一 GET 端点）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingEndpoints_ShouldMatchOfficialRoutes()
    {
        // 预约会议基础管理族公共面：4 端点收敛父接口。
        MeetingBaseRoutes.Should().HaveCount(4, "预约会议基础管理族公共面 = 创建 + 修改 + 取消 + 获取成员会议 ID 列表");
        MeetingBaseRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/", StringComparison.Ordinal), "预约会议基础管理族路由位于 /cgi-bin/meeting/ 段");

        // 获取会议详情：自建/第三方差异端点同路由两分支，代开发不承载。
        MeetingInfoRoutes.Should().HaveCount(2, "获取会议详情为自建/第三方差异端点（同路由两分支）");
        MeetingInfoRoutes.Select(r => r.Interface).Should().BeEquivalentTo(new[]
        {
            typeof(IWechatWorkInternalMeetingService),
            typeof(IWechatWorkThirdPartyMeetingService),
        }, "获取会议详情仅自建应用与第三方应用开放（代开发章节未提供该端点）");

        // 会议统计管理族：路由挂 statistics 段（官方原文如此）。
        MeetingStatisticsRoutes.Should().HaveCount(1, "会议统计管理族 = 获取会议发起记录单端点");
        MeetingStatisticsRoutes.Single().Route.Should().StartWith(
            "/cgi-bin/meeting/statistics/", "会议统计管理路由挂 /cgi-bin/meeting/statistics/ 段（get_start_list）");

        // 预约会议高级管理族：19 端点全部挂于唯一自建子接口。
        MeetingAdvancedRoutes.Should().HaveCount(19, "预约会议高级管理族 = 受邀成员 2 + 专属链接 2 + 会中/已参会成员 2 + 等候室 2 + 设备入会 1 + 嘉宾 2 + 健康度 1 + 报名 7 端点");
        MeetingAdvancedRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingAdvancedService),
            "预约会议高级管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingAdvancedRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/enroll/", StringComparison.Ordinal))
            .Should().HaveCount(7, "报名配置与报名信息 7 端点路由挂 /cgi-bin/meeting/enroll/ 段");
        MeetingAdvancedRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/waitingroom/", StringComparison.Ordinal))
            .Should().HaveCount(2, "等候室 2 端点路由挂 /cgi-bin/meeting/waitingroom/ 段");

        // 会中控制管理族：17 端点全部挂于唯一自建子接口（realcontrol 9 + poll 8）。
        MeetingControlRoutes.Should().HaveCount(17, "会中控制管理族 = 会控 9 + 会议投票 8 端点");
        MeetingControlRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingControlService),
            "会中控制管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingControlRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/realcontrol/", StringComparison.Ordinal))
            .Should().HaveCount(9, "会控 9 端点路由挂 /cgi-bin/meeting/realcontrol/ 段");
        MeetingControlRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/poll/", StringComparison.Ordinal))
            .Should().HaveCount(8, "会议投票 8 端点路由挂 /cgi-bin/meeting/poll/ 段");

        // 网络研讨会管理族：14 端点全部挂于唯一自建子接口（webinar 段，报名 7 端点挂 webinar/enroll 子段）。
        MeetingWebinarRoutes.Should().HaveCount(14, "网络研讨会管理族 = 基础管理 4 + 嘉宾 2 + 暖场 1 + 报名 7 端点");
        MeetingWebinarRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingWebinarService),
            "网络研讨会管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingWebinarRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/webinar/", StringComparison.Ordinal),
            "网络研讨会管理族路由全部挂 /cgi-bin/meeting/webinar/ 段");
        MeetingWebinarRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/webinar/enroll/", StringComparison.Ordinal))
            .Should().HaveCount(7, "网络研讨会报名 7 端点路由挂 /cgi-bin/meeting/webinar/enroll/ 子段");

        // 电话入会（PSTN）管理族：3 端点全部挂于唯一自建子接口（phone 段）。
        MeetingPstnRoutes.Should().HaveCount(3, "电话入会（PSTN）管理族 = 批量外呼 + 外呼状态 + 成员 ID 端点");
        MeetingPstnRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingPstnService),
            "电话入会（PSTN）管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingPstnRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/phone/", StringComparison.Ordinal),
            "电话入会（PSTN）管理族路由全部挂 /cgi-bin/meeting/phone/ 段");

        // Rooms 会议室管理族：12 端点全部挂于唯一自建子接口（rooms 段）。
        MeetingRoomsRoutes.Should().HaveCount(12, "Rooms 会议室管理族 = 预定/释放 2 + 列表/详情/配置/资源 4 + 会议列表 1 + 设备/控制器列表 2 + 呼叫/取消呼叫/应答状态 3 端点");
        MeetingRoomsRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingRoomsService),
            "Rooms 会议室管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingRoomsRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/rooms/", StringComparison.Ordinal),
            "Rooms 会议室管理族路由全部挂 /cgi-bin/meeting/rooms/ 段");

        // 会议室连接器（MRA）管理族：4 端点全部挂于唯一自建子接口（mra 段）。
        MeetingMraRoutes.Should().HaveCount(4, "会议室连接器（MRA）管理族 = 状态查询 + 默认布局 + 举手 + 挂断端点");
        MeetingMraRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingMraService),
            "会议室连接器（MRA）管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingMraRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/mra/", StringComparison.Ordinal),
            "会议室连接器（MRA）管理族路由全部挂 /cgi-bin/meeting/mra/ 段");

        // 会议布局和背景管理族：15 端点全部挂于唯一自建子接口（layout/advanced_layout 段）。
        MeetingLayoutRoutes.Should().HaveCount(15, "会议布局和背景管理族 = 布局模板列表 1 + 基础布局 3（添加/修改/设置默认）+ 高级布局 6（添加/修改/应用/列表/用户布局/批量删除）+ 会议背景 5 端点");
        MeetingLayoutRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingLayoutService),
            "会议布局和背景管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingLayoutRoutes.Select(r => r.Route)
            .Should().OnlyContain(r => r.StartsWith("/cgi-bin/meeting/layout/", StringComparison.Ordinal)
                || r.StartsWith("/cgi-bin/meeting/advanced_layout/", StringComparison.Ordinal),
            "会议布局和背景管理族路由挂 /cgi-bin/meeting/layout/ 与 /cgi-bin/meeting/advanced_layout/ 段");
        MeetingLayoutRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/advanced_layout/", StringComparison.Ordinal))
            .Should().HaveCount(6, "高级布局 6 端点路由挂 /cgi-bin/meeting/advanced_layout/ 段");
        MeetingLayoutRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/layout/", StringComparison.Ordinal))
            .Should().HaveCount(9, "布局模板列表 1 + 基础布局 3 + 会议背景 5 端点路由挂 /cgi-bin/meeting/layout/ 段");
        MeetingLayoutRoutes.Single(r => r.Route.EndsWith("/list_template", StringComparison.Ordinal)).HttpAttribute
            .Should().Be(typeof(GetAttribute), "获取布局模板列表为会议域唯一 GET 端点（勿「顺手统一」为 POST）");
        MeetingLayoutRoutes.Count(r => r.HttpAttribute == typeof(PostAttribute)).Should().Be(14,
            "布局和背景管理族其余 14 端点官方均即 POST");

        // 录制管理族：10 端点全部挂于唯一自建子接口（record 段，转写挂 record/transcript 子段）。
        MeetingRecordingRoutes.Should().HaveCount(10, "录制管理族 = 列表/统计/共享设置 3 + 删除 2 + 详情/地址 2 + 转写 3 端点");
        MeetingRecordingRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingRecordingService),
            "录制管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingRecordingRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/record/", StringComparison.Ordinal),
            "录制管理族路由全部挂 /cgi-bin/meeting/record/ 段");
        MeetingRecordingRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/record/transcript/", StringComparison.Ordinal))
            .Should().HaveCount(3, "录制转写 3 端点路由挂 /cgi-bin/meeting/record/transcript/ 子段");

        // 高级功能账号管理族：3 个官方文档页承载 5 端点，全部挂于唯一自建子接口（vip 段）。
        MeetingPremiumAccountRoutes.Should().HaveCount(5, "高级功能账号管理族 = 分配 + 分配结果 + 取消 + 取消结果 + 账号列表端点");
        MeetingPremiumAccountRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingPremiumAccountService),
            "高级功能账号管理族官方仅自建应用开放（代开发/第三方应用暂不支持）");
        MeetingPremiumAccountRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/vip/", StringComparison.Ordinal),
            "高级功能账号管理族路由全部挂 /cgi-bin/meeting/vip/ 段");

        // 获取 Rooms 会议室资源官方为无请求体 POST（对齐 get_openid_migration 先例），不得添加请求体参数。
        typeof(IWechatWorkInternalMeetingRoomsService)
            .GetMethod(nameof(IWechatWorkInternalMeetingRoomsService.GetMeetingRoomInventoryAsync), BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters().Should().ContainSingle("获取 Rooms 会议室资源官方无请求体，方法仅承载 CancellationToken")
            .Which.ParameterType.Should().Be(typeof(CancellationToken),
                "获取 Rooms 会议室资源无请求体，唯一参数必须是 CancellationToken");
        // 获取布局模板列表官方为无请求体 GET，同理不得添加请求体参数。
        typeof(IWechatWorkInternalMeetingLayoutService)
            .GetMethod(nameof(IWechatWorkInternalMeetingLayoutService.ListLayoutTemplatesAsync), BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters().Should().ContainSingle("获取布局模板列表官方无请求体，方法仅承载 CancellationToken")
            .Which.ParameterType.Should().Be(typeof(CancellationToken),
                "获取布局模板列表无请求体，唯一参数必须是 CancellationToken");

        AssertRoutes(MeetingBaseRoutes);
        AssertRoutes(MeetingInfoRoutes);
        AssertRoutes(MeetingStatisticsRoutes);
        AssertRoutes(MeetingAdvancedRoutes);
        AssertRoutes(MeetingControlRoutes);
        AssertRoutes(MeetingWebinarRoutes);
        AssertRoutes(MeetingPstnRoutes);
        AssertRoutes(MeetingRoomsRoutes);
        AssertRoutes(MeetingMraRoutes);
        AssertRoutes(MeetingLayoutRoutes);
        AssertRoutes(MeetingRecordingRoutes);
        AssertRoutes(MeetingPremiumAccountRoutes);

        // 全部官方路由去重清单锁定（get_info 两分支去重后 105 条；基础管理族与高级管理族的
        // create/update/cancel/get_info/get_user_meetingid 5 条同路由仅计一次）。
        var allRoutes = MeetingBaseRoutes.Concat(MeetingInfoRoutes).Concat(MeetingStatisticsRoutes)
            .Concat(MeetingAdvancedRoutes).Concat(MeetingControlRoutes)
            .Concat(MeetingWebinarRoutes).Concat(MeetingPstnRoutes)
            .Concat(MeetingRoomsRoutes).Concat(MeetingMraRoutes)
            .Concat(MeetingLayoutRoutes).Concat(MeetingRecordingRoutes).Concat(MeetingPremiumAccountRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            // 预约会议基础管理族（5 条；与高级管理族 5 个文档页同路由，不重复建端点）。
            "/cgi-bin/meeting/create",
            "/cgi-bin/meeting/update",
            "/cgi-bin/meeting/cancel",
            "/cgi-bin/meeting/get_user_meetingid",
            "/cgi-bin/meeting/get_info",
            // 会议统计管理族。
            "/cgi-bin/meeting/statistics/get_start_list",
            // 预约会议高级管理族（19 条）。
            "/cgi-bin/meeting/get_invitees",
            "/cgi-bin/meeting/set_invitees",
            "/cgi-bin/meeting/create_customer_short_url",
            "/cgi-bin/meeting/get_customer_short_url",
            "/cgi-bin/meeting/get_realtime_attendee_list",
            "/cgi-bin/meeting/get_attendee_list",
            "/cgi-bin/meeting/waitingroom/get_current_user_list",
            "/cgi-bin/meeting/waitingroom/get_user_list",
            "/cgi-bin/meeting/check_device_in_meeting",
            "/cgi-bin/meeting/get_guests",
            "/cgi-bin/meeting/set_guests",
            "/cgi-bin/meeting/get_quality",
            "/cgi-bin/meeting/enroll/set_config",
            "/cgi-bin/meeting/enroll/get_config",
            "/cgi-bin/meeting/enroll/query_by_tmp_openid",
            "/cgi-bin/meeting/enroll/list",
            "/cgi-bin/meeting/enroll/approve",
            "/cgi-bin/meeting/enroll/import",
            "/cgi-bin/meeting/enroll/delete",
            // 会中控制管理族（17 条）。
            "/cgi-bin/meeting/realcontrol/set",
            "/cgi-bin/meeting/realcontrol/set_cohost",
            "/cgi-bin/meeting/realcontrol/mute_user",
            "/cgi-bin/meeting/realcontrol/switch_user_video",
            "/cgi-bin/meeting/realcontrol/close_screen_share",
            "/cgi-bin/meeting/realcontrol/set_nicknames",
            "/cgi-bin/meeting/realcontrol/manage_waiting_room_users",
            "/cgi-bin/meeting/realcontrol/kickout_users",
            "/cgi-bin/meeting/realcontrol/dismiss",
            "/cgi-bin/meeting/poll/create_theme",
            "/cgi-bin/meeting/poll/update_theme",
            "/cgi-bin/meeting/poll/get_poll_list",
            "/cgi-bin/meeting/poll/get_theme_info",
            "/cgi-bin/meeting/poll/get_poll_detail",
            "/cgi-bin/meeting/poll/delete",
            "/cgi-bin/meeting/poll/start",
            "/cgi-bin/meeting/poll/finish",
            // 网络研讨会管理族（14 条）。
            "/cgi-bin/meeting/webinar/create",
            "/cgi-bin/meeting/webinar/update",
            "/cgi-bin/meeting/webinar/cancel",
            "/cgi-bin/meeting/webinar/get",
            "/cgi-bin/meeting/webinar/list_guest",
            "/cgi-bin/meeting/webinar/update_guest_list",
            "/cgi-bin/meeting/webinar/update_warm_up",
            "/cgi-bin/meeting/webinar/enroll/set_config",
            "/cgi-bin/meeting/webinar/enroll/get_config",
            "/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid",
            "/cgi-bin/meeting/webinar/enroll/list",
            "/cgi-bin/meeting/webinar/enroll/approve",
            "/cgi-bin/meeting/webinar/enroll/import",
            "/cgi-bin/meeting/webinar/enroll/delete",
            // 电话入会（PSTN）管理族（3 条）。
            "/cgi-bin/meeting/phone/callout",
            "/cgi-bin/meeting/phone/get_callout_status",
            "/cgi-bin/meeting/phone/get_tmp_openid",
            // Rooms 会议室管理族（12 条）。
            "/cgi-bin/meeting/rooms/book",
            "/cgi-bin/meeting/rooms/release",
            "/cgi-bin/meeting/rooms/list",
            "/cgi-bin/meeting/rooms/get_info",
            "/cgi-bin/meeting/rooms/get_config",
            "/cgi-bin/meeting/rooms/list_meetings",
            "/cgi-bin/meeting/rooms/list_devices",
            "/cgi-bin/meeting/rooms/list_controllers",
            "/cgi-bin/meeting/rooms/get_inventory",
            "/cgi-bin/meeting/rooms/call",
            "/cgi-bin/meeting/rooms/cancel_call",
            "/cgi-bin/meeting/rooms/get_response_status",
            // 会议室连接器（MRA）管理族（4 条）。
            "/cgi-bin/meeting/mra/query_status",
            "/cgi-bin/meeting/mra/set_default_layout",
            "/cgi-bin/meeting/mra/set_raise_hand",
            "/cgi-bin/meeting/mra/hangup",
            // 会议布局和背景管理族（15 条）。
            "/cgi-bin/meeting/layout/list_template",
            "/cgi-bin/meeting/layout/add",
            "/cgi-bin/meeting/layout/update",
            "/cgi-bin/meeting/layout/set_default",
            "/cgi-bin/meeting/advanced_layout/add",
            "/cgi-bin/meeting/advanced_layout/update",
            "/cgi-bin/meeting/advanced_layout/apply",
            "/cgi-bin/meeting/advanced_layout/list",
            "/cgi-bin/meeting/advanced_layout/get_user_layout",
            "/cgi-bin/meeting/advanced_layout/batch_delete",
            "/cgi-bin/meeting/layout/add_background",
            "/cgi-bin/meeting/layout/set_default_background",
            "/cgi-bin/meeting/layout/list_background",
            "/cgi-bin/meeting/layout/delete_background",
            "/cgi-bin/meeting/layout/batch_delete_background",
            // 录制管理族（10 条）。
            "/cgi-bin/meeting/record/list",
            "/cgi-bin/meeting/record/get_statistics",
            "/cgi-bin/meeting/record/update_sharing_config",
            "/cgi-bin/meeting/record/delete",
            "/cgi-bin/meeting/record/delete_file",
            "/cgi-bin/meeting/record/get_file",
            "/cgi-bin/meeting/record/get_file_list",
            "/cgi-bin/meeting/record/transcript/get_paragraph_list",
            "/cgi-bin/meeting/record/transcript/get_detail",
            "/cgi-bin/meeting/record/transcript/search",
            // 高级功能账号管理族（5 条）。
            "/cgi-bin/meeting/vip/submit_batch_add_job",
            "/cgi-bin/meeting/vip/batch_add_job_result",
            "/cgi-bin/meeting/vip/submit_batch_del_job",
            "/cgi-bin/meeting/vip/batch_del_job_result",
            "/cgi-bin/meeting/vip/list",
        }, "会议域全部官方路由须与官方文档一一对应");
        allRoutes.Should().HaveCount(105, "会议域共 105 条官方路由（get_info 同路由两分支去重；高级管理文档页与基础管理族 5 条同路由不重复计入）");

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        typeof(IWechatWorkMeetingService)
            .GetMethod(nameof(IWechatWorkMeetingService.CancelMeetingAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "取消预约会议仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingAdvancedService)
            .GetMethod(nameof(IWechatWorkInternalMeetingAdvancedService.SetInviteesAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新会议受邀成员列表仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingAdvancedService)
            .GetMethod(nameof(IWechatWorkInternalMeetingAdvancedService.SetGuestsAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新会议嘉宾列表仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingControlService)
            .GetMethod(nameof(IWechatWorkInternalMeetingControlService.SetRealtimeSettingsAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "管理会中设置仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingControlService)
            .GetMethod(nameof(IWechatWorkInternalMeetingControlService.UpdatePollThemeAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "修改会议投票主题仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingWebinarService)
            .GetMethod(nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "修改网络研讨会仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingWebinarService)
            .GetMethod(nameof(IWechatWorkInternalMeetingWebinarService.CancelWebinarAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "取消网络研讨会仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingWebinarService)
            .GetMethod(nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarGuestsAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新网络研讨会嘉宾列表仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingWebinarService)
            .GetMethod(nameof(IWechatWorkInternalMeetingWebinarService.UpdateWebinarWarmUpAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "管理网络研讨会暖场配置仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
    }

    // ------------------------------------------------------------------
    // MT2：接口层级与生成器注册形态（三族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 预约会议基础管理族：公共父 4 端点 + 自建/第三方各 1 差异端点（获取会议详情）+ 代开发零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingService),
            parentImplementation: "WechatWorkMeetingService",
            parentDeclaredEndpointCount: 4,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingService), 1),
                (typeof(IWechatWorkProviderMeetingService), 0),
                (typeof(IWechatWorkThirdPartyMeetingService), 1),
            });

        // 会议统计管理族：官方仅自建开放，父接口零端点 + 唯一自建子接口承载端点
        //（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingStatisticsService),
            parentImplementation: "WechatWorkMeetingStatisticsService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingStatisticsService), 1),
            });

        // 预约会议高级管理族：官方仅自建应用开放（第三方应用开发与服务商代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 19 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingAdvancedService),
            parentImplementation: "WechatWorkMeetingAdvancedService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingAdvancedService), 19),
            });

        // 会中控制管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 17 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingControlService),
            parentImplementation: "WechatWorkMeetingControlService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingControlService), 17),
            });

        // 网络研讨会管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 14 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingWebinarService),
            parentImplementation: "WechatWorkMeetingWebinarService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingWebinarService), 14),
            });

        // 电话入会（PSTN）管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 3 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingPstnService),
            parentImplementation: "WechatWorkMeetingPstnService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingPstnService), 3),
            });

        // Rooms 会议室管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 12 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingRoomsService),
            parentImplementation: "WechatWorkMeetingRoomsService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingRoomsService), 12),
            });

        // 会议室连接器（MRA）管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 4 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingMraService),
            parentImplementation: "WechatWorkMeetingMraService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingMraService), 4),
            });

        // 会议布局和背景管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 15 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingLayoutService),
            parentImplementation: "WechatWorkMeetingLayoutService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingLayoutService), 15),
            });

        // 录制管理族：官方仅自建应用开放（第三方/代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 10 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingRecordingService),
            parentImplementation: "WechatWorkMeetingRecordingService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingRecordingService), 10),
            });

        // 高级功能账号管理族：官方仅自建应用开放（代开发/第三方应用暂不支持），
        // 父接口零端点 + 唯一自建子接口承载全部 5 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingPremiumAccountService),
            parentImplementation: "WechatWorkMeetingPremiumAccountService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingPremiumAccountService), 5),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(MeetingRegistryGroupName,
                $"{child.Name} 必须挂 {MeetingRegistryGroupName} 注册组（Meeting 模块共用 Add{MeetingRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // MT3：令牌绑定——会议域 8 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkMeetingService),
            typeof(IWechatWorkInternalMeetingService),
            typeof(IWechatWorkProviderMeetingService),
            typeof(IWechatWorkThirdPartyMeetingService),
            typeof(IWechatWorkMeetingStatisticsService),
            typeof(IWechatWorkInternalMeetingStatisticsService),
            typeof(IWechatWorkMeetingAdvancedService),
            typeof(IWechatWorkInternalMeetingAdvancedService),
            typeof(IWechatWorkMeetingControlService),
            typeof(IWechatWorkInternalMeetingControlService),
            typeof(IWechatWorkMeetingWebinarService),
            typeof(IWechatWorkInternalMeetingWebinarService),
            typeof(IWechatWorkMeetingPstnService),
            typeof(IWechatWorkInternalMeetingPstnService),
            typeof(IWechatWorkMeetingRoomsService),
            typeof(IWechatWorkInternalMeetingRoomsService),
            typeof(IWechatWorkMeetingMraService),
            typeof(IWechatWorkInternalMeetingMraService),
            typeof(IWechatWorkMeetingLayoutService),
            typeof(IWechatWorkInternalMeetingLayoutService),
            typeof(IWechatWorkMeetingRecordingService),
            typeof(IWechatWorkInternalMeetingRecordingService),
            typeof(IWechatWorkMeetingPremiumAccountService),
            typeof(IWechatWorkInternalMeetingPremiumAccountService),
        };

        accessTokenInterfaces.Should().HaveCount(24, "会议域十一族 = 预约会议基础管理族 4 接口 + 会议统计管理族 2 接口 + 预约会议高级管理族 2 接口 + 会中控制管理族 2 接口 + 网络研讨会管理族 2 接口 + 电话入会管理族 2 接口 + Rooms 会议室管理族 2 接口 + MRA 管理族 2 接口 + 会议布局和背景管理族 2 接口 + 录制管理族 2 接口 + 高级功能账号管理族 2 接口");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // MT4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Meeting）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingDataModels_ShouldBeRegisteredInJsonContext()
    {
        var meetingContext = MeetingJsonContext.Default;

        var domainTypes = typeof(CreateMeetingRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Meeting"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 MeetingJsonContext 且 SerializerClassName 统一为 Meeting
        //（生成物 MeetingJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(261,
            "会议模块契约面类型数漂移须先核对官方文档再同批调整本守卫（预约会议基础管理族 17：创建 2 + 修改 2 + 取消 1 + 获取详情 2 + 成员会议 ID 列表 2 + 共用嵌套 8；会议统计管理族 3；预约会议高级管理族 55：端点级请求/响应 36 + 嵌套对象 19；会中控制管理族 32：端点级请求/响应 22 + 嵌套对象 10；网络研讨会管理族 28：端点级请求/响应 24 + 嵌套对象 4；电话入会管理族 10：端点级请求/响应 6 + 嵌套对象 4；Rooms 会议室管理族 33：端点级请求 11 + 响应 10 + 嵌套对象 12（含 MRA 信令地址对象）；MRA 管理族 6：端点级请求 4 + 响应 1 + 嵌套对象 1；会议布局和背景管理族 35：端点级请求/响应 21 + 嵌套对象 14（基础与高级布局结构不同构分型承载）；录制管理族 31：端点级请求/响应 17 + 嵌套对象 14；高级功能账号管理族 11：端点级请求/响应 10 + 嵌套对象 1）");

        foreach (var type in domainTypes)
        {
            meetingContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于会议域命名空间，必须登记进 MeetingJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Meeting",
                $"{type.Name} 的 SerializerClassName 必须为会议域段 Meeting");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 预约会议基础管理族。
            typeof(CreateMeetingRequest), typeof(CreateMeetingResponse),
            typeof(UpdateMeetingRequest), typeof(UpdateMeetingResponse),
            typeof(CancelMeetingRequest),
            typeof(GetMeetingInfoRequest), typeof(GetMeetingInfoResponse),
            typeof(GetUserMeetingIdListRequest), typeof(GetUserMeetingIdListResponse),
            // 共用嵌套对象。
            typeof(MeetingInvitees), typeof(MeetingSettings), typeof(MeetingReminders),
            typeof(MeetingHosts), typeof(MeetingRingUsers),
            typeof(MeetingAttendees), typeof(MeetingAttendeeMember), typeof(MeetingTmpExternalUser),
            // 会议统计管理族。
            typeof(GetMeetingStartListRequest), typeof(GetMeetingStartListResponse), typeof(MeetingStartRecord),
            // 预约会议高级管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(GetMeetingInviteesRequest), typeof(GetMeetingInviteesResponse), typeof(MeetingInvitee),
            typeof(SetMeetingInviteesRequest),
            typeof(CreateMeetingCustomerShortUrlRequest), typeof(CreateMeetingCustomerShortUrlResponse),
            typeof(GetMeetingCustomerShortUrlRequest), typeof(GetMeetingCustomerShortUrlResponse),
            typeof(MeetingCustomerShortUrl),
            typeof(GetMeetingRealtimeAttendeeListRequest), typeof(GetMeetingRealtimeAttendeeListResponse),
            typeof(MeetingRealtimeAttendee),
            typeof(GetMeetingAttendeeListRequest), typeof(GetMeetingAttendeeListResponse),
            typeof(MeetingAttendedAttendee),
            typeof(GetMeetingWaitingRoomCurrentUserListRequest), typeof(GetMeetingWaitingRoomCurrentUserListResponse),
            typeof(MeetingWaitingRoomCurrentUser),
            typeof(GetMeetingWaitingRoomUserListRequest), typeof(GetMeetingWaitingRoomUserListResponse),
            typeof(MeetingWaitingRoomUserRecord),
            typeof(CheckMeetingDeviceInMeetingRequest), typeof(CheckMeetingDeviceInMeetingResponse),
            typeof(MeetingDeviceCheckResult),
            typeof(GetMeetingGuestsRequest), typeof(GetMeetingGuestsResponse), typeof(MeetingGuest),
            typeof(SetMeetingGuestsRequest),
            typeof(GetMeetingQualityRequest), typeof(GetMeetingQualityResponse), typeof(MeetingQualityAttendee),
            typeof(SetMeetingEnrollConfigRequest), typeof(SetMeetingEnrollConfigResponse),
            typeof(GetMeetingEnrollConfigRequest), typeof(GetMeetingEnrollConfigResponse),
            typeof(MeetingEnrollQuestion), typeof(MeetingEnrollQuestionOption),
            typeof(QueryMeetingEnrollIdsRequest), typeof(QueryMeetingEnrollIdsResponse), typeof(MeetingEnrollId),
            typeof(ListMeetingEnrollsRequest), typeof(ListMeetingEnrollsResponse),
            typeof(MeetingEnrollInfo), typeof(MeetingEnrollAnswer),
            typeof(ApproveMeetingEnrollsRequest), typeof(ApproveMeetingEnrollsResponse),
            typeof(ImportMeetingEnrollsRequest), typeof(ImportMeetingEnrollsResponse),
            typeof(MeetingEnrollImportItem), typeof(MeetingEnrollImportResult),
            typeof(DeleteMeetingEnrollsRequest), typeof(DeleteMeetingEnrollsResponse), typeof(MeetingEnrollIdRef),
            typeof(MeetingSubMeeting), typeof(MeetingSubRepeatInfo),
            // 会中控制管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(SetMeetingRealtimeSettingsRequest),
            typeof(SetMeetingCoHostRequest), typeof(MuteMeetingUserRequest),
            typeof(SwitchMeetingUserVideoRequest), typeof(CloseMeetingScreenShareRequest),
            typeof(SetMeetingUserNicknamesRequest), typeof(ManageMeetingWaitingRoomUsersRequest),
            typeof(KickoutMeetingUsersRequest), typeof(DismissMeetingRequest),
            typeof(MeetingOperatedUser), typeof(MeetingOperatedNicknameUser),
            typeof(CreateMeetingPollThemeRequest), typeof(CreateMeetingPollThemeResponse),
            typeof(UpdateMeetingPollThemeRequest),
            typeof(GetMeetingPollListRequest), typeof(GetMeetingPollListResponse),
            typeof(GetMeetingPollThemeInfoRequest), typeof(GetMeetingPollThemeInfoResponse),
            typeof(GetMeetingPollDetailRequest), typeof(GetMeetingPollDetailResponse),
            typeof(DeleteMeetingPollRequest),
            typeof(StartMeetingPollRequest), typeof(StartMeetingPollResponse),
            typeof(FinishMeetingPollRequest),
            typeof(MeetingPollQuestion), typeof(MeetingPollThemeInfo), typeof(MeetingPollInfo),
            typeof(MeetingPollThemeQuestion), typeof(MeetingPollThemeOption),
            typeof(MeetingPollDetailQuestion), typeof(MeetingPollDetailOption), typeof(MeetingPollOptionUser),
            // 网络研讨会管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(CreateWebinarRequest), typeof(CreateWebinarResponse),
            typeof(UpdateWebinarRequest), typeof(CancelWebinarRequest),
            typeof(GetWebinarRequest), typeof(GetWebinarResponse),
            typeof(ListWebinarGuestsRequest), typeof(ListWebinarGuestsResponse),
            typeof(UpdateWebinarGuestsRequest), typeof(UpdateWebinarWarmUpRequest),
            typeof(SetWebinarEnrollConfigRequest), typeof(SetWebinarEnrollConfigResponse),
            typeof(GetWebinarEnrollConfigRequest), typeof(GetWebinarEnrollConfigResponse),
            typeof(QueryWebinarEnrollIdsRequest), typeof(QueryWebinarEnrollIdsResponse),
            typeof(ListWebinarEnrollsRequest), typeof(ListWebinarEnrollsResponse),
            typeof(ApproveWebinarEnrollsRequest), typeof(ApproveWebinarEnrollsResponse),
            typeof(ImportWebinarEnrollsRequest), typeof(ImportWebinarEnrollsResponse),
            typeof(DeleteWebinarEnrollsRequest), typeof(DeleteWebinarEnrollsResponse),
            typeof(WebinarHostInfo), typeof(WebinarMediaSetting), typeof(WebinarMediaSettingInfo), typeof(WebinarGuest),
            // 电话入会（PSTN）管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(PstnBatchCalloutRequest), typeof(PstnBatchCalloutResponse),
            typeof(PstnGetCalloutStatusRequest), typeof(PstnGetCalloutStatusResponse),
            typeof(PstnGetTmpOpenidRequest), typeof(PstnGetTmpOpenidResponse),
            typeof(PstnPhoneNumber), typeof(PstnCalloutPhoneNumber),
            typeof(PstnCalloutStatusPhoneNumber), typeof(PstnTmpOpenidPhoneNumber),
            // Rooms 会议室管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(BookMeetingRoomRequest), typeof(BookMeetingRoomResponse),
            typeof(ReleaseMeetingRoomRequest),
            typeof(ListMeetingRoomsRequest), typeof(ListMeetingRoomsResponse),
            typeof(GetMeetingRoomInfoRequest), typeof(GetMeetingRoomInfoResponse),
            typeof(GetMeetingRoomConfigRequest), typeof(GetMeetingRoomConfigResponse),
            typeof(ListMeetingRoomMeetingsRequest), typeof(ListMeetingRoomMeetingsResponse),
            typeof(ListRoomDevicesRequest), typeof(ListRoomDevicesResponse),
            typeof(ListRoomControllersRequest), typeof(ListRoomControllersResponse),
            typeof(GetMeetingRoomInventoryResponse),
            typeof(CallMeetingRoomRequest), typeof(CallMeetingRoomResponse),
            typeof(CancelCallMeetingRoomRequest),
            typeof(GetMeetingRoomResponseStatusRequest), typeof(GetMeetingRoomResponseStatusResponse),
            typeof(RoomsMeetingRoom), typeof(RoomsBasicInfo), typeof(RoomsAccountInfo),
            typeof(RoomsHardwareInfo), typeof(RoomsPmiInfo),
            typeof(RoomsMeetingSettings), typeof(RoomsRecordSettings),
            typeof(RoomsMeetingInfo), typeof(RoomsDeviceInfo), typeof(RoomsDeviceMonitorInfo),
            typeof(RoomsControllerInfo), typeof(RoomsMraAddress),
            // 会议室连接器（MRA）管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(QueryMraStatusRequest), typeof(QueryMraStatusResponse),
            typeof(SetMraDefaultLayoutRequest), typeof(SetMraRaiseHandRequest), typeof(HangupMraRequest),
            typeof(MraDeviceRef),
            // 会议布局和背景管理族（端点级请求/响应 + 嵌套对象全清单；
            // 基础布局与高级布局结构不同构，按 LayoutBasic* / LayoutAdvanced* 分型承载）。
            typeof(ListLayoutTemplatesResponse), typeof(LayoutTemplate),
            typeof(AddMeetingLayoutRequest), typeof(AddMeetingLayoutResponse),
            typeof(LayoutBasicRequest), typeof(LayoutBasicPage), typeof(LayoutBasicSeat), typeof(LayoutBasicInfo),
            typeof(UpdateMeetingLayoutRequest),
            typeof(AddAdvancedLayoutRequest), typeof(AddAdvancedLayoutResponse),
            typeof(LayoutAdvancedRequest), typeof(LayoutAdvancedPage), typeof(LayoutAdvancedPollingSetting),
            typeof(LayoutAdvancedSeat), typeof(LayoutAdvancedGridUser), typeof(LayoutAdvancedInfo),
            typeof(UpdateAdvancedLayoutRequest),
            typeof(SetDefaultMeetingLayoutRequest),
            typeof(ApplyAdvancedLayoutRequest), typeof(LayoutApplyUser),
            typeof(ListMeetingLayoutsRequest), typeof(ListMeetingLayoutsResponse),
            typeof(GetUserLayoutRequest), typeof(GetUserLayoutResponse),
            typeof(BatchDeleteLayoutsRequest),
            typeof(AddMeetingBackgroundRequest), typeof(AddMeetingBackgroundResponse),
            typeof(LayoutBackgroundImage), typeof(LayoutBackground),
            typeof(SetDefaultMeetingBackgroundRequest),
            typeof(ListMeetingBackgroundsRequest), typeof(ListMeetingBackgroundsResponse),
            typeof(DeleteMeetingBackgroundRequest), typeof(BatchDeleteMeetingBackgroundsRequest),
            // 录制管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(ListMeetingRecordsRequest), typeof(ListMeetingRecordsResponse),
            typeof(MeetingRecord), typeof(MeetingRecordFile),
            typeof(GetRecordStatisticsRequest), typeof(GetRecordStatisticsResponse), typeof(RecordStatisticsSummary),
            typeof(UpdateRecordSharingConfigRequest), typeof(RecordSharingConfig),
            typeof(DeleteMeetingRecordRequest), typeof(DeleteRecordFileRequest),
            typeof(GetRecordFileRequest), typeof(GetRecordFileResponse), typeof(RecordFileDownload),
            typeof(GetRecordFileListRequest), typeof(GetRecordFileListResponse), typeof(RecordFileAddress),
            typeof(GetRecordTranscriptParagraphsRequest), typeof(GetRecordTranscriptParagraphsResponse),
            typeof(RecordTranscriptParagraph),
            typeof(GetRecordTranscriptDetailRequest), typeof(GetRecordTranscriptDetailResponse),
            typeof(RecordTranscriptDetail), typeof(RecordTranscriptDetailParagraph),
            typeof(RecordTranscriptSentence), typeof(RecordTranscriptWord), typeof(RecordTranscriptSpeaker),
            typeof(SearchRecordTranscriptRequest), typeof(SearchRecordTranscriptResponse),
            typeof(RecordTranscriptHit), typeof(RecordTranscriptTimeline),
            // 高级功能账号管理族（端点级请求/响应 + 嵌套对象全清单；
            // 分配/取消两个文档页各含「提交任务」与「查询结果」两端点，结构同构故共用嵌套对象）。
            typeof(AssignPremiumAccountsRequest), typeof(AssignPremiumAccountsResponse),
            typeof(GetAssignPremiumAccountsResultRequest), typeof(GetAssignPremiumAccountsResultResponse),
            typeof(RevokePremiumAccountsRequest), typeof(RevokePremiumAccountsResponse),
            typeof(GetRevokePremiumAccountsResultRequest), typeof(GetRevokePremiumAccountsResultResponse),
            typeof(ListPremiumAccountsRequest), typeof(ListPremiumAccountsResponse),
            typeof(PremiumAccountJobResult),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于会议域命名空间");
    }

    // ------------------------------------------------------------------
    // MT5：官方契约陷阱锁定——共用嵌套结构、meetingid 拼写形态与翻页/数组形态。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingDataModels_ShouldLockOfficialContractTraps()
    {
        // 创建/修改预约会议请求与获取会议详情响应的 settings/reminders/invitees 共用同一结构
        //（官方参数表高度同构；对齐 Schedule calendar 扁平结构先例）。
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "创建预约会议 settings 与修改/详情共用同一配置结构");
        typeof(UpdateMeetingRequest).GetProperty(nameof(UpdateMeetingRequest.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "修改预约会议 settings 与创建/详情共用同一配置结构");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "获取会议详情响应 settings 与创建/修改共用同一配置结构");
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Reminders))!
            .PropertyType.Should().Be(typeof(MeetingReminders), "创建预约会议 reminders 与修改/详情共用同一重复配置结构");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Reminders))!
            .PropertyType.Should().Be(typeof(MeetingReminders), "获取会议详情响应 reminders 与创建/修改共用同一重复配置结构");
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Invitees))!
            .PropertyType.Should().Be(typeof(MeetingInvitees), "创建预约会议 invitees 与修改预约会议共用同一成员结构");
        typeof(UpdateMeetingRequest).GetProperty(nameof(UpdateMeetingRequest.Invitees))!
            .PropertyType.Should().Be(typeof(MeetingInvitees), "修改预约会议 invitees 与创建预约会议共用同一成员结构");

        // 会议 ID 官方字段名作 meetingid（无下划线），列表作 meetingid_list——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(UpdateMeetingRequest), nameof(UpdateMeetingRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(CancelMeetingRequest), nameof(CancelMeetingRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(GetMeetingInfoRequest), nameof(GetMeetingInfoRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(GetUserMeetingIdListResponse), nameof(GetUserMeetingIdListResponse.MeetingidList), "meetingid_list");

        // 翻页游标字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetUserMeetingIdListResponse), nameof(GetUserMeetingIdListResponse.NextCursor), "next_cursor");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.NextCursor), "next_cursor");

        // remind_before 为秒数数组（仅支持 0/300/900/3600/86400），非单值。
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RemindBefore))!
            .PropertyType.Should().Be(typeof(List<int>), "remind_before 官方为提前提醒秒数数组");

        // 创建/修改预约会议响应均携带 excess_users（购买会议专业版企业部分参会人无有效会议账号时返回）。
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.ExcessUsers), "excess_users");
        JsonNameShouldBe(typeof(UpdateMeetingResponse), nameof(UpdateMeetingResponse.ExcessUsers), "excess_users");

        // 获取会议详情响应的成员字段名照抄官方原文。
        JsonNameShouldBe(typeof(MeetingAttendees), nameof(MeetingAttendees.TmpExternalUser), "tmp_external_user");
        JsonNameShouldBe(typeof(MeetingTmpExternalUser), nameof(MeetingTmpExternalUser.TmpExternalUserid), "tmp_external_userid");
        JsonNameShouldBe(typeof(MeetingAttendeeMember), nameof(MeetingAttendeeMember.TotalJoinCount), "total_join_count");

        // 会议统计：type 决定记录成败口径、has_more 为布尔翻页标记。
        JsonNameShouldBe(typeof(GetMeetingStartListRequest), nameof(GetMeetingStartListRequest.BeginTime), "begin_time");
        JsonNameShouldBe(typeof(GetMeetingStartListRequest), nameof(GetMeetingStartListRequest.EndTime), "end_time");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.MeetingList), "meeting_list");
        JsonNameShouldBe(typeof(MeetingStartRecord), nameof(MeetingStartRecord.StartTime), "start_time");

        // ---- 预约会议高级管理族契约陷阱 ----

        // 高级管理文档页与基础管理族 5 条同路由：请求/响应 DTO 扩展为高级文档参数超集（同结构承载，不另建 DTO）。
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Guests))!
            .PropertyType.Should().Be(typeof(List<MeetingGuest>), "创建预约会议 guests 仅高级管理文档页声明，与嘉宾列表端点共用 MeetingGuest");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Guests))!
            .PropertyType.Should().Be(typeof(List<MeetingGuest>), "获取会议详情 guests 与嘉宾列表端点共用 MeetingGuest");
        typeof(CancelMeetingRequest).GetProperty(nameof(CancelMeetingRequest.SubMeetingid))!
            .PropertyType.Should().Be(typeof(string), "取消预约会议 sub_meetingid 仅高级管理文档页声明（不传则取消整个周期系列）");
        typeof(GetMeetingInfoRequest).GetProperty(nameof(GetMeetingInfoRequest.MeetingCode))!
            .PropertyType.Should().Be(typeof(string), "获取会议详情高级文档口径为 meetingid 与 meeting_code 必须填一个");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.MeetingCode), "meeting_code");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.MeetingLink), "meeting_link");

        // MeetingSettings 超集字段（报名/主持人密钥/录制/同声传译/联席主持人等）沿用 Meeting 结构，勿拆分类型。
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.EnableEnroll))!
            .PropertyType.Should().Be(typeof(bool?), "enable_enroll 为布尔可空（官方未赋值字段不输出）");
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.HostKey))!
            .PropertyType.Should().Be(typeof(string), "host_key 为 6 位数字字符串");
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.CoHosts))!
            .PropertyType.Should().Be(typeof(MeetingHosts), "co_hosts 与 hosts 共用 userid 数组包裹结构");
        JsonNameShouldBe(typeof(MeetingSettings), nameof(MeetingSettings.AllowUnmuteSelf), "allow_unmute_self");
        JsonNameShouldBe(typeof(MeetingSettings), nameof(MeetingSettings.AutoRecordType), "auto_record_type");

        // MeetingReminders 自定义重复字段：repeat_until_type 官方创建文档页参数表作 uint32[]，示例为单值，按单值承载。
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RepeatUntilType))!
            .PropertyType.Should().Be(typeof(int?), "repeat_until_type 官方示例为单值整数（参数表 uint32[] 为文档笔误）");
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RepeatDayOfWeek))!
            .PropertyType.Should().Be(typeof(List<int>), "repeat_day_of_week 官方为周几数组（1~7）");

        // 受邀成员两种形态并存：创建/修改请求为 userid 数组包裹对象，受邀成员列表端点为对象数组。
        typeof(MeetingInvitees).GetProperty(nameof(MeetingInvitees.Userid))!
            .PropertyType.Should().Be(typeof(List<string>), "创建/修改预约会议 invitees.userid 为字符串数组");
        typeof(MeetingInvitee).GetProperty(nameof(MeetingInvitee.Userid))!
            .PropertyType.Should().Be(typeof(string), "受邀成员列表端点 invitees 元素为 {userid} 对象");

        // 受邀成员列表端点字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetMeetingInviteesResponse), nameof(GetMeetingInviteesResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(GetMeetingInviteesResponse), nameof(GetMeetingInviteesResponse.Invitees), "invitees");
        JsonNameShouldBe(typeof(SetMeetingInviteesRequest), nameof(SetMeetingInviteesRequest.Invitees), "invitees");

        // 实时会中成员列表：官方请求示例 cursort 为拼写陷阱，cursor 以参数表为准。
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListRequest), nameof(GetMeetingRealtimeAttendeeListRequest.Cursor), "cursor");
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListRequest), nameof(GetMeetingRealtimeAttendeeListRequest.SubMeetingid), "sub_meetingid");
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListResponse), nameof(GetMeetingRealtimeAttendeeListResponse.Attendees), "attendees");
        JsonNameShouldBe(typeof(MeetingRealtimeAttendee), nameof(MeetingRealtimeAttendee.TmpOpenid), "tmp_openid");
        JsonNameShouldBe(typeof(MeetingRealtimeAttendee), nameof(MeetingRealtimeAttendee.ScreenSharedState), "screen_shared_state");

        // 已参会成员列表：支持网络研讨会角色与专属链接 customer_data。
        JsonNameShouldBe(typeof(MeetingAttendedAttendee), nameof(MeetingAttendedAttendee.WebinarRole), "webinar_role");
        JsonNameShouldBe(typeof(MeetingAttendedAttendee), nameof(MeetingAttendedAttendee.CustomerData), "customer_data");

        // 等候室两列表端点 user_list 字段名一致、记录对象不同构（实时含 customer_data，记录含毫秒级进出时间）。
        JsonNameShouldBe(typeof(GetMeetingWaitingRoomCurrentUserListResponse), nameof(GetMeetingWaitingRoomCurrentUserListResponse.UserList), "user_list");
        JsonNameShouldBe(typeof(GetMeetingWaitingRoomUserListResponse), nameof(GetMeetingWaitingRoomUserListResponse.UserList), "user_list");
        JsonNameShouldBe(typeof(MeetingWaitingRoomUserRecord), nameof(MeetingWaitingRoomUserRecord.JoinTime), "join_time");
        JsonNameShouldBe(typeof(MeetingWaitingRoomUserRecord), nameof(MeetingWaitingRoomUserRecord.QuitTime), "quit_time");

        // 专属参会链接与嘉宾列表字段名照抄官方原文。
        JsonNameShouldBe(typeof(CreateMeetingCustomerShortUrlResponse), nameof(CreateMeetingCustomerShortUrlResponse.MeetingShortUrlCustomerData), "meeting_short_url_customer_data");
        JsonNameShouldBe(typeof(GetMeetingCustomerShortUrlResponse), nameof(GetMeetingCustomerShortUrlResponse.MeetingShortUrlCustomerDataList), "meeting_short_url_customer_data_list");
        JsonNameShouldBe(typeof(MeetingGuest), nameof(MeetingGuest.PhoneNumber), "phone_number");
        JsonNameShouldBe(typeof(MeetingGuest), nameof(MeetingGuest.GuestName), "guest_name");

        // 健康度：会议级与成员级同构字段（quality/audio_quality/video_quality/screen_share_quality/network_quality/problems）。
        JsonNameShouldBe(typeof(GetMeetingQualityRequest), nameof(GetMeetingQualityRequest.StartTime), "start_time");
        JsonNameShouldBe(typeof(GetMeetingQualityResponse), nameof(GetMeetingQualityResponse.ScreenShareQuality), "screen_share_quality");
        JsonNameShouldBe(typeof(GetMeetingQualityResponse), nameof(GetMeetingQualityResponse.Problems), "problems");
        JsonNameShouldBe(typeof(MeetingQualityAttendee), nameof(MeetingQualityAttendee.NetworkQuality), "network_quality");

        // 报名域：审批请求 enroll_id_list 为字符串数组、删除请求为对象数组（官方两种形态并存，勿统一）。
        typeof(ApproveMeetingEnrollsRequest).GetProperty(nameof(ApproveMeetingEnrollsRequest.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<string>), "审批会议报名信息 enroll_id_list 官方为字符串数组");
        typeof(DeleteMeetingEnrollsRequest).GetProperty(nameof(DeleteMeetingEnrollsRequest.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollIdRef>), "删除会议报名信息 enroll_id_list 官方为对象数组（{enroll_id}）");
        JsonNameShouldBe(typeof(SetMeetingEnrollConfigRequest), nameof(SetMeetingEnrollConfigRequest.ApproveType), "approve_type");
        JsonNameShouldBe(typeof(SetMeetingEnrollConfigRequest), nameof(SetMeetingEnrollConfigRequest.NoRegistrationNeededForStaff), "no_registration_needed_for_staff");
        JsonNameShouldBe(typeof(GetMeetingEnrollConfigResponse), nameof(GetMeetingEnrollConfigResponse.QuestionList), "question_list");
        JsonNameShouldBe(typeof(QueryMeetingEnrollIdsRequest), nameof(QueryMeetingEnrollIdsRequest.TmpOpenidList), "tmp_openid_list");
        JsonNameShouldBe(typeof(QueryMeetingEnrollIdsResponse), nameof(QueryMeetingEnrollIdsResponse.EnrollIdList), "enroll_id_list");
        JsonNameShouldBe(typeof(MeetingEnrollId), nameof(MeetingEnrollId.EnrollId), "enroll_id");
        JsonNameShouldBe(typeof(ListMeetingEnrollsResponse), nameof(ListMeetingEnrollsResponse.EnrollList), "enroll_list");
        JsonNameShouldBe(typeof(MeetingEnrollInfo), nameof(MeetingEnrollInfo.EnrollSourceType), "enroll_source_type");
        JsonNameShouldBe(typeof(MeetingEnrollInfo), nameof(MeetingEnrollInfo.EnrollCode), "enroll_code");
        JsonNameShouldBe(typeof(MeetingEnrollAnswer), nameof(MeetingEnrollAnswer.AnswerContent), "answer_content");
        JsonNameShouldBe(typeof(MeetingEnrollAnswer), nameof(MeetingEnrollAnswer.QuestionNum), "question_num");
        JsonNameShouldBe(typeof(ApproveMeetingEnrollsResponse), nameof(ApproveMeetingEnrollsResponse.HandledCount), "handled_count");
        JsonNameShouldBe(typeof(ImportMeetingEnrollsResponse), nameof(ImportMeetingEnrollsResponse.TotalCount), "total_count");
        JsonNameShouldBe(typeof(DeleteMeetingEnrollsResponse), nameof(DeleteMeetingEnrollsResponse.TotalCount), "total_count");

        // 获取会议详情高级文档页扩展字段：周期性子会议与分段信息字段名照抄官方原文（meetingid 无下划线）。
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.MeetingType), "meeting_type");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.HasVote), "has_vote");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.SubMeetings), "sub_meetings");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.HasMoreSubMeeting), "has_more_sub_meeting");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.RemainSubMeetings), "remain_sub_meetings");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.CurrentSubMeetingid), "current_sub_meetingid");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.SubRepeatList), "sub_repeat_list");
        JsonNameShouldBe(typeof(MeetingSubMeeting), nameof(MeetingSubMeeting.SubMeetingid), "sub_meetingid");
        JsonNameShouldBe(typeof(MeetingSubMeeting), nameof(MeetingSubMeeting.RepeatId), "repeat_id");
        JsonNameShouldBe(typeof(MeetingSubRepeatInfo), nameof(MeetingSubRepeatInfo.RepeatId), "repeat_id");
        JsonNameShouldBe(typeof(MeetingSubRepeatInfo), nameof(MeetingSubRepeatInfo.RepeatUntilCount), "repeat_until_count");

        // ---- 会中控制管理族契约陷阱 ----

        // 会控单数 operated_user 承载单个对象（参数表 object[] 为文档笔误），复数 operated_users 承载数组——勿统一。
        typeof(SetMeetingCoHostRequest).GetProperty(nameof(SetMeetingCoHostRequest.OperatedUser))!
            .PropertyType.Should().Be(typeof(MeetingOperatedUser), "管理联席主持人 operated_user 为单个对象");
        typeof(MuteMeetingUserRequest).GetProperty(nameof(MuteMeetingUserRequest.OperatedUser))!
            .PropertyType.Should().Be(typeof(MeetingOperatedUser), "静音成员 operated_user 为单个对象");
        typeof(CloseMeetingScreenShareRequest).GetProperty(nameof(CloseMeetingScreenShareRequest.OperatedUser))!
            .PropertyType.Should().Be(typeof(MeetingOperatedUser), "关闭成员屏幕共享 operated_user 为单个对象");
        typeof(SwitchMeetingUserVideoRequest).GetProperty(nameof(SwitchMeetingUserVideoRequest.OperatedUser))!
            .PropertyType.Should().Be(typeof(MeetingOperatedUser), "开关成员视频 operated_user 为单个对象");
        typeof(ManageMeetingWaitingRoomUsersRequest).GetProperty(nameof(ManageMeetingWaitingRoomUsersRequest.OperatedUsers))!
            .PropertyType.Should().Be(typeof(List<MeetingOperatedUser>), "管理等候室成员 operated_users 为对象数组");
        typeof(KickoutMeetingUsersRequest).GetProperty(nameof(KickoutMeetingUsersRequest.OperatedUsers))!
            .PropertyType.Should().Be(typeof(List<MeetingOperatedUser>), "移出成员 operated_users 为对象数组");
        typeof(SetMeetingUserNicknamesRequest).GetProperty(nameof(SetMeetingUserNicknamesRequest.OperatedUsers))!
            .PropertyType.Should().Be(typeof(List<MeetingOperatedNicknameUser>), "修改成员昵称 operated_users 为对象数组（参数表 opereated_users 为笔误，以示例 operated_users 为准）");

        // 静音成员 option 参数表标 string、示例为 bool——以示例为准；开关成员视频参数表 instanceid 为笔误、以示例 instance_id 为准。
        typeof(MuteMeetingUserRequest).GetProperty(nameof(MuteMeetingUserRequest.Option))!
            .PropertyType.Should().Be(typeof(bool?), "静音成员 option 官方示例为布尔值（参数表 string 为文档笔误）");
        JsonNameShouldBe(typeof(MeetingOperatedUser), nameof(MeetingOperatedUser.InstanceId), "instance_id");
        JsonNameShouldBe(typeof(MeetingOperatedUser), nameof(MeetingOperatedUser.TmpOpenid), "tmp_openid");
        JsonNameShouldBe(typeof(MeetingOperatedNicknameUser), nameof(MeetingOperatedNicknameUser.Nickname), "nickname");

        // 管理会中设置：allow_unmute_self 需 mute_all=true 才生效（联动约束锁定字段形态）。
        typeof(SetMeetingRealtimeSettingsRequest).GetProperty(nameof(SetMeetingRealtimeSettingsRequest.MuteAll))!
            .PropertyType.Should().Be(typeof(bool?), "管理会中设置 mute_all 为布尔可空");
        JsonNameShouldBe(typeof(SetMeetingRealtimeSettingsRequest), nameof(SetMeetingRealtimeSettingsRequest.HideMeetingCodePassword), "hide_meeting_code_password");
        JsonNameShouldBe(typeof(SetMeetingRealtimeSettingsRequest), nameof(SetMeetingRealtimeSettingsRequest.PlayIvrOnJoin), "play_ivr_on_join");

        // 结束会议：force_dismiss 默认 1、retrieve_code 默认 0（周期性会议还有子会议时须不回收会议号）。
        JsonNameShouldBe(typeof(DismissMeetingRequest), nameof(DismissMeetingRequest.ForceDismiss), "force_dismiss");
        JsonNameShouldBe(typeof(DismissMeetingRequest), nameof(DismissMeetingRequest.RetrieveCode), "retrieve_code");

        // 会议投票：操作者三元组（operator_userid/instance_id/meetingid）字段名与投票状态/共享/匿名枚举字段名照抄官方原文。
        JsonNameShouldBe(typeof(CreateMeetingPollThemeRequest), nameof(CreateMeetingPollThemeRequest.OperatorUserid), "operator_userid");
        JsonNameShouldBe(typeof(CreateMeetingPollThemeRequest), nameof(CreateMeetingPollThemeRequest.IsAnony), "is_anony");
        JsonNameShouldBe(typeof(CreateMeetingPollThemeRequest), nameof(CreateMeetingPollThemeRequest.PollQuestions), "poll_questions");
        JsonNameShouldBe(typeof(MeetingPollQuestion), nameof(MeetingPollQuestion.QuestionDesc), "question_desc");
        JsonNameShouldBe(typeof(MeetingPollQuestion), nameof(MeetingPollQuestion.PollOption), "poll_option");
        JsonNameShouldBe(typeof(CreateMeetingPollThemeResponse), nameof(CreateMeetingPollThemeResponse.PollThemeId), "poll_theme_id");
        JsonNameShouldBe(typeof(GetMeetingPollListResponse), nameof(GetMeetingPollListResponse.PollsThemeInfo), "polls_theme_info");
        JsonNameShouldBe(typeof(MeetingPollThemeInfo), nameof(MeetingPollThemeInfo.PollsInfo), "polls_info");
        JsonNameShouldBe(typeof(MeetingPollInfo), nameof(MeetingPollInfo.PollId), "poll_id");
        JsonNameShouldBe(typeof(MeetingPollInfo), nameof(MeetingPollInfo.IsShared), "is_shared");
        JsonNameShouldBe(typeof(GetMeetingPollThemeInfoResponse), nameof(GetMeetingPollThemeInfoResponse.PollQuestionData), "poll_question_data");
        JsonNameShouldBe(typeof(MeetingPollThemeOption), nameof(MeetingPollThemeOption.OptionDesc), "option_desc");
        JsonNameShouldBe(typeof(GetMeetingPollDetailResponse), nameof(GetMeetingPollDetailResponse.VoteTotalNum), "vote_total_num");
        JsonNameShouldBe(typeof(MeetingPollDetailQuestion), nameof(MeetingPollDetailQuestion.QuestionId), "question_id");
        JsonNameShouldBe(typeof(MeetingPollDetailOption), nameof(MeetingPollDetailOption.OptionNum), "option_num");
        JsonNameShouldBe(typeof(MeetingPollDetailOption), nameof(MeetingPollDetailOption.Rate), "rate");
        JsonNameShouldBe(typeof(MeetingPollDetailOption), nameof(MeetingPollDetailOption.OptionUser), "option_user");
        JsonNameShouldBe(typeof(StartMeetingPollResponse), nameof(StartMeetingPollResponse.PollId), "poll_id");

        // ---- 网络研讨会管理族契约陷阱 ----

        // 详情响应主题字段参数表作 subject、示例作 title（与创建响应一致）——以示例为准。
        JsonNameShouldBe(typeof(GetWebinarResponse), nameof(GetWebinarResponse.Title), "title");
        JsonNameShouldBe(typeof(GetWebinarResponse), nameof(GetWebinarResponse.MeetingCode), "meeting_code");
        JsonNameShouldBe(typeof(CreateWebinarResponse), nameof(CreateWebinarResponse.Meetingid), "meetingid");

        // 详情响应 status 为字符串枚举（MEETING_STATE_*），display_number_of_attendees 响应表标 string、示例为数字，按整数承载。
        typeof(GetWebinarResponse).GetProperty(nameof(GetWebinarResponse.Status))!
            .PropertyType.Should().Be(typeof(string), "网络研讨会详情 status 官方为字符串枚举（MEETING_STATE_*）");
        typeof(GetWebinarResponse).GetProperty(nameof(GetWebinarResponse.DisplayNumberOfAttendees))!
            .PropertyType.Should().Be(typeof(int?), "display_number_of_attendees 官方示例为数字（响应表 string 为笔误）");

        // start_time/end_time 参数表与示例均为字符串形态时间戳（单位秒），按字符串承载。
        typeof(CreateWebinarRequest).GetProperty(nameof(CreateWebinarRequest.StartTime))!
            .PropertyType.Should().Be(typeof(string), "网络研讨会 start_time 官方为字符串形态时间戳");
        typeof(GetWebinarResponse).GetProperty(nameof(GetWebinarResponse.StartTime))!
            .PropertyType.Should().Be(typeof(string), "网络研讨会详情 start_time 官方为字符串形态时间戳");

        // media_setting 请求/响应分型承载：请求入会静音作 enable_enter_mute，响应官方示例作 mute_enable_join。
        typeof(WebinarMediaSetting).GetProperty(nameof(WebinarMediaSetting.EnableEnterMute))!
            .PropertyType.Should().Be(typeof(bool?), "创建/修改网络研讨会请求入会静音字段为 enable_enter_mute");
        JsonNameShouldBe(typeof(WebinarMediaSetting), nameof(WebinarMediaSetting.EnableEnterMute), "enable_enter_mute");
        JsonNameShouldBe(typeof(WebinarMediaSettingInfo), nameof(WebinarMediaSettingInfo.MuteEnableJoin), "mute_enable_join");
        typeof(GetWebinarResponse).GetProperty(nameof(GetWebinarResponse.MediaSetting))!
            .PropertyType.Should().Be(typeof(WebinarMediaSettingInfo), "获取网络研讨会详情 media_setting 与请求形态分型（字段名不同构）");
        typeof(UpdateWebinarRequest).GetProperty(nameof(UpdateWebinarRequest.MediaSetting))!
            .PropertyType.Should().Be(typeof(WebinarMediaSetting), "修改网络研讨会 media_setting 参数表 object[] 为笔误，与创建共用单对象结构");

        // 主持人列表为 {userid} 单值对象数组（区别于预约会议 hosts 的 userid 数组包裹对象）。
        typeof(CreateWebinarRequest).GetProperty(nameof(CreateWebinarRequest.Hosts))!
            .PropertyType.Should().Be(typeof(List<WebinarHostInfo>), "创建网络研讨会 hosts 为主持人对象数组（ userid 单值）");
        JsonNameShouldBe(typeof(WebinarHostInfo), nameof(WebinarHostInfo.Userid), "userid");

        // 网络研讨会嘉宾为多字段对象（guest_type/userid/area/phone_number/guest_name/email），区别于普通会议嘉宾 MeetingGuest。
        typeof(ListWebinarGuestsResponse).GetProperty(nameof(ListWebinarGuestsResponse.Guests))!
            .PropertyType.Should().Be(typeof(List<WebinarGuest>), "网络研讨会嘉宾列表 guests 参数表标 object、示例为数组，按数组承载");
        JsonNameShouldBe(typeof(WebinarGuest), nameof(WebinarGuest.GuestType), "guest_type");
        JsonNameShouldBe(typeof(WebinarGuest), nameof(WebinarGuest.Email), "email");

        // 网络研讨会报名 7 端点与普通会议报名域同构，复用同一批嵌套 DTO（勿另建平行类型）。
        typeof(SetWebinarEnrollConfigRequest).GetProperty(nameof(SetWebinarEnrollConfigRequest.QuestionList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollQuestion>), "网络研讨会报名问题与普通会议报名共用同一结构");
        typeof(QueryWebinarEnrollIdsResponse).GetProperty(nameof(QueryWebinarEnrollIdsResponse.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollId>), "网络研讨会报名 ID 与普通会议报名 ID 共用同一结构");
        typeof(ListWebinarEnrollsResponse).GetProperty(nameof(ListWebinarEnrollsResponse.EnrollList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollInfo>), "网络研讨会报名信息与普通会议报名信息共用同一结构");
        typeof(ImportWebinarEnrollsRequest).GetProperty(nameof(ImportWebinarEnrollsRequest.EnrollList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollImportItem>), "网络研讨会导入报名条目与普通会议共用同一结构");
        typeof(DeleteWebinarEnrollsRequest).GetProperty(nameof(DeleteWebinarEnrollsRequest.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollIdRef>), "网络研讨会删除报名 enroll_id_list 与普通会议同为对象数组");

        // ---- 电话入会（PSTN）管理族契约陷阱 ----

        // 号码对象按形态分型：请求/不合法号码无回执字段，外呼成功带 status，外呼状态查询再带 tmp_openid——勿合并。
        typeof(PstnBatchCalloutRequest).GetProperty(nameof(PstnBatchCalloutRequest.PhoneNumbers))!
            .PropertyType.Should().Be(typeof(List<PstnPhoneNumber>), "批量外呼请求 phone_numbers 为基础号码对象数组");
        typeof(PstnBatchCalloutResponse).GetProperty(nameof(PstnBatchCalloutResponse.PhoneNumbers))!
            .PropertyType.Should().Be(typeof(List<PstnCalloutPhoneNumber>), "批量外呼成功号码带外呼状态 status");
        typeof(PstnBatchCalloutResponse).GetProperty(nameof(PstnBatchCalloutResponse.InvalidPhoneNumbers))!
            .PropertyType.Should().Be(typeof(List<PstnPhoneNumber>), "批量外呼不合法号码为基础号码对象数组");
        typeof(PstnGetCalloutStatusResponse).GetProperty(nameof(PstnGetCalloutStatusResponse.PhoneNumbers))!
            .PropertyType.Should().Be(typeof(List<PstnCalloutStatusPhoneNumber>), "外呼状态号码在 status 之上另有 tmp_openid");
        JsonNameShouldBe(typeof(PstnPhoneNumber), nameof(PstnPhoneNumber.ExtensionNumber), "extension_number");
        JsonNameShouldBe(typeof(PstnGetTmpOpenidResponse), nameof(PstnGetTmpOpenidResponse.TmpOpenidList), "tmp_openid_list");
        JsonNameShouldBe(typeof(PstnCalloutStatusPhoneNumber), nameof(PstnCalloutStatusPhoneNumber.TmpOpenid), "tmp_openid");

        // ---- Rooms 会议室管理族契约陷阱 ----

        // 会议室 ID 字段族照抄官方原文（meeting_room_id 无下划线、列表作 meeting_room_id_list）。
        JsonNameShouldBe(typeof(BookMeetingRoomRequest), nameof(BookMeetingRoomRequest.MeetingRoomIdList), "meeting_room_id_list");
        JsonNameShouldBe(typeof(RoomsMeetingRoom), nameof(RoomsMeetingRoom.MeetingRoomId), "meeting_room_id");
        JsonNameShouldBe(typeof(RoomsMeetingRoom), nameof(RoomsMeetingRoom.MeetingRoomLocation), "meeting_room_location");
        JsonNameShouldBe(typeof(GetMeetingRoomInfoRequest), nameof(GetMeetingRoomInfoRequest.MeetingRoomId), "meeting_room_id");
        JsonNameShouldBe(typeof(ListMeetingRoomMeetingsRequest), nameof(ListMeetingRoomMeetingsRequest.RoomsId), "rooms_id");

        // 预定/列表响应共用 RoomsMeetingRoom 超集结构（预定文档页 MeetingRoom 仅含基础三字段，列表页扩展账号/状态字段）。
        typeof(BookMeetingRoomResponse).GetProperty(nameof(BookMeetingRoomResponse.MeetingRoomList))!
            .PropertyType.Should().Be(typeof(List<RoomsMeetingRoom>), "预定 Rooms 会议室响应与列表响应共用 MeetingRoom 超集结构");
        typeof(ListMeetingRoomsResponse).GetProperty(nameof(ListMeetingRoomsResponse.MeetingRoomList))!
            .PropertyType.Should().Be(typeof(List<RoomsMeetingRoom>), "获取 Rooms 会议室列表与预定响应共用 MeetingRoom 超集结构");

        // 详情四段信息与配置两段信息结构锁定。
        typeof(GetMeetingRoomInfoResponse).GetProperty(nameof(GetMeetingRoomInfoResponse.BasicInfo))!
            .PropertyType.Should().Be(typeof(RoomsBasicInfo), "Rooms 详情 basic_info 结构");
        typeof(GetMeetingRoomInfoResponse).GetProperty(nameof(GetMeetingRoomInfoResponse.HardwareInfo))!
            .PropertyType.Should().Be(typeof(RoomsHardwareInfo), "Rooms 详情 hardware_info 结构");
        typeof(GetMeetingRoomConfigResponse).GetProperty(nameof(GetMeetingRoomConfigResponse.MeetingSettings))!
            .PropertyType.Should().Be(typeof(RoomsMeetingSettings), "Rooms 配置项 meeting_settings 与预约会议 MeetingSettings 为两个不同结构");
        JsonNameShouldBe(typeof(RoomsMeetingSettings), nameof(RoomsMeetingSettings.WaterMark), "water_mark");
        JsonNameShouldBe(typeof(RoomsRecordSettings), nameof(RoomsRecordSettings.ShareRecord), "share_record");

        // Rooms 会议室下的会议列表：status 为字符串枚举（MEETING_STATE_*，本页含 MEETING_STATE_NULL）。
        typeof(RoomsMeetingInfo).GetProperty(nameof(RoomsMeetingInfo.Status))!
            .PropertyType.Should().Be(typeof(string), "Rooms 会议室下的会议列表 status 官方为字符串枚举");
        JsonNameShouldBe(typeof(RoomsMeetingInfo), nameof(RoomsMeetingInfo.Subject), "subject");
        JsonNameShouldBe(typeof(RoomsMeetingInfo), nameof(RoomsMeetingInfo.MeetingType), "meeting_type");

        // 设备/控制器列表字段名照抄官方原文（设备 app_version 文档说明误写「激活码」、控制器 status 为字符串形态）。
        JsonNameShouldBe(typeof(RoomsDeviceInfo), nameof(RoomsDeviceInfo.AppVersion), "app_version");
        JsonNameShouldBe(typeof(RoomsDeviceInfo), nameof(RoomsDeviceInfo.DeviceMonitorInfo), "device_monitor_info");
        JsonNameShouldBe(typeof(RoomsDeviceMonitorInfo), nameof(RoomsDeviceMonitorInfo.MicrophoneStatus), "microphone_status");
        JsonNameShouldBe(typeof(RoomsControllerInfo), nameof(RoomsControllerInfo.ManufactureName), "manufacture_name");
        JsonNameShouldBe(typeof(RoomsControllerInfo), nameof(RoomsControllerInfo.FrameworkVersion), "framework_version");
        typeof(RoomsControllerInfo).GetProperty(nameof(RoomsControllerInfo.Status))!
            .PropertyType.Should().Be(typeof(string), "控制器设备状态官方为字符串形态（\"0\" 离线 / \"1\" 在线）");

        // 呼叫类端点：meeting_room_id 与 mra_address 二选一（两字段均可空），信令地址对象结构锁定。
        typeof(CallMeetingRoomRequest).GetProperty(nameof(CallMeetingRoomRequest.MraAddress))!
            .PropertyType.Should().Be(typeof(RoomsMraAddress), "呼叫 Rooms 会议室 mra_address 为 MRA 信令地址对象");
        JsonNameShouldBe(typeof(RoomsMraAddress), nameof(RoomsMraAddress.DialString), "dial_string");
        JsonNameShouldBe(typeof(GetMeetingRoomResponseStatusResponse), nameof(GetMeetingRoomResponseStatusResponse.ResponseTime), "response_time");

        // ---- 会议室连接器（MRA）管理族契约陷阱 ----

        // MRA 被操作设备为 {tmp_openid} 单值包裹对象（区别于会控 operated_user 的多字段结构）。
        typeof(SetMraDefaultLayoutRequest).GetProperty(nameof(SetMraDefaultLayoutRequest.Mra))!
            .PropertyType.Should().Be(typeof(MraDeviceRef), "切换 MRA 默认布局 mra 为被操作设备对象");
        JsonNameShouldBe(typeof(MraDeviceRef), nameof(MraDeviceRef.TmpOpenid), "tmp_openid");
        JsonNameShouldBe(typeof(QueryMraStatusResponse), nameof(QueryMraStatusResponse.RaiseHandsState), "raise_hands_state");
        JsonNameShouldBe(typeof(QueryMraStatusResponse), nameof(QueryMraStatusResponse.DefaultLayout), "default_layout");
        JsonNameShouldBe(typeof(QueryMraStatusResponse), nameof(QueryMraStatusResponse.WebinarMemberRole), "webinar_member_role");
        JsonNameShouldBe(typeof(SetMraDefaultLayoutRequest), nameof(SetMraDefaultLayoutRequest.DefaultNovideoUser), "default_novideo_user");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}

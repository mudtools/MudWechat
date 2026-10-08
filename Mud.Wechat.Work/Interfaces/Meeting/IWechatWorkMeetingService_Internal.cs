// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Meeting;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会议」模块预约会议基础管理域企业自建应用 SDK。
/// <para>
/// 官方对自建应用开放与三类应用公共面一致的 4 端点（继承自
/// <see cref="IWechatWorkMeetingService"/>），并额外开放 1 个差异端点：
/// 「获取会议详情」（服务商代开发章节未开放该端点，见 <see cref="IWechatWorkProviderMeetingService"/>）。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderMeetingService"/>；
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMeetingService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：发起人和企业内部参与人必须在应用可见范围内；自建应用需配置在「可调用接口的应用」列表中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Meeting",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMeetingService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMeetingService : IWechatWorkMeetingService
{
    /// <summary>
    /// 获取会议详情
    /// <para>获取该应用创建的某个预约会议的详情（基础信息 + 会议成员 + 会议嘉宾 + 会议配置 + 重复会议配置 + 周期性子会议）。</para>
    /// <para>官方限制：只能拉取该应用创建的会议；快速会议仅返回已参与成员列表。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMeetingInfoRequest"/>：meetingid / meeting_code / sub_meetingid；预约会议高级管理文档页口径为 meetingid 与 meeting_code 必须填一个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会议详情（admin_userid / title / meeting_start / meeting_duration / description / location / main_department / status / meeting_type / agentid / meeting_code / meeting_link / cal_id / attendees / settings / reminders / guests / has_vote / sub_meetings / has_more_sub_meeting / remain_sub_meetings / current_sub_meetingid / sub_repeat_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99049"/></para>
    /// <para><b>企业自建应用·预约会议高级管理</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98149"/></para>
    /// <para>官方权限：仅允许拉取当前应用创建的会议；自建应用需配置在「可调用接口的应用」列表中。</para>
    /// </remarks>
    [Post("/cgi-bin/meeting/get_info")]
    Task<GetMeetingInfoResponse> GetMeetingInfoAsync(
        [Body] GetMeetingInfoRequest request,
        CancellationToken cancellationToken = default);
}

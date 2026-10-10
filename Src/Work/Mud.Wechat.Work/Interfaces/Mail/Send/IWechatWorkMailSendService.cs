// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「邮件」模块发送邮件族公共 SDK
/// （发送普通邮件 + 发送日程邮件 + 发送会议邮件，三端点共用官方路由 <c>/cgi-bin/exmail/app/compose_send</c>）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalMailSendService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMailSendService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderMailSendService"/>。
/// </para>
/// <para>获取接收的邮件族见 <see cref="IWechatWorkMailReceiveService"/>；
/// 管理应用邮箱账号族（官方仅自建应用开放）见 <see cref="IWechatWorkMailAccountService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与代开发自建应用需具有「邮件」权限（发送日程邮件还需「日程」权限、发送会议邮件还需「日程」和「会议」权限）。
/// to.userids / cc.userids / bcc.userids 指定的成员需在应用可见范围内。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMailSendService
{
    /// <summary>
    /// 发送普通邮件
    /// <para>应用可以通过该接口发送普通邮件；成员通过收到的邮件可回复至应用邮箱。</para>
    /// <para>官方业务限制：所有附件加正文的大小不允许超过 50M，且附件个数不能超过 200 个；
    /// to.emails 和 to.userids 至少传一个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MailSendRequest"/>：to / cc / bcc / subject / content / attachment_list / content_type / enable_id_trans）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97445"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97515"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97504"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/compose_send")]
    Task<WechatWorkResponse> SendNormalMailAsync(
        [Body] MailSendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送日程邮件
    /// <para>应用可以通过该接口发送日程邮件；subject 同时是日程标题、content 同时是日程描述，
    /// 日程相关数据（时间/地点/提醒/重复/管理员）通过 schedule 承载，发日程邮件官方必填。</para>
    /// <para>官方业务限制：所有附件加正文的大小不允许超过 50M，且附件个数不能超过 200 个；
    /// schedule.schedule_admins 管理员上限 3 个，只支持传 userid，必须是同企业的用户且在参与人中；
    /// 应用还需具有「日程」权限。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MailSendScheduleRequest"/>：公共邮件字段 + schedule）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97854"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97867"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97865"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/compose_send")]
    Task<WechatWorkResponse> SendScheduleMailAsync(
        [Body] MailSendScheduleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送会议邮件
    /// <para>应用可以通过该接口发送会议邮件；subject 同时是会议标题、content 同时是会议描述，
    /// 会议相关数据通过 schedule 承载（发会议邮件必须带上），会议设置通过 meeting 承载
    /// （官方标注会议邮件必填，且必须同时带上 schedule）。</para>
    /// <para>官方业务限制：所有附件加正文的大小不允许超过 50M，且附件个数不能超过 200 个；
    /// meeting.hosts 会议主持人最多 10 个（只支持填 userid）；
    /// meeting.meeting_admins 会议管理员仅可指定 1 人，只支持传 userid，必须是同企业的用户且在参与人中；
    /// 应用还需具有「日程」和「会议」权限。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MailSendMeetingRequest"/>：公共邮件字段 + schedule + meeting）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97855"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97868"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97866"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/compose_send")]
    Task<WechatWorkResponse> SendMeetingMailAsync(
        [Body] MailSendMeetingRequest request,
        CancellationToken cancellationToken = default);
}

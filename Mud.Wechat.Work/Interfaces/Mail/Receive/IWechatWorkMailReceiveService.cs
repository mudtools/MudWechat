// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块获取接收的邮件族公共 SDK
/// （获取收件箱邮件列表 + 获取邮件内容）。
/// <para>
/// 官方对三类应用开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalMailReceiveService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyMailReceiveService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderMailReceiveService"/>。
/// </para>
/// <para>发送邮件族见 <see cref="IWechatWorkMailSendService"/>；
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
/// 第三方应用与代开发自建应用需具有「邮件」权限。
/// 用户可向应用绑定的邮箱回复邮件，故应用可通过本接口族获取应用收件箱的邮件。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkMailReceiveService
{
    /// <summary>
    /// 获取收件箱邮件列表
    /// <para>分页获取应用收件箱下，指定时间范围内的邮件 id 列表（cursor + has_more 翻页拉取）；
    /// 邮件 id 可用于「获取邮件内容」接口。</para>
    /// <para>官方业务限制：limit 期望请求的数据量默认值为 100，最大值为 1000。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetMailListRequest"/>：begin_time / end_time 官方必填，cursor / limit 官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>翻页游标（next_cursor）、是否还有更多数据（has_more）与邮件 id 列表（mail_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97369"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97516"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97505"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/get_mail_list")]
    Task<GetMailListResponse> GetMailListAsync(
        [Body] GetMailListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取邮件内容
    /// <para>指定单个邮件 id，获取该邮件的 eml 数据（即完整原始邮件格式内容）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ReadMailRequest"/>：mail_id 官方必填，可通过「获取收件箱邮件列表」获得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>邮件 eml 内容（mail_data）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97979"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97983"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97982"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/read_mail")]
    Task<ReadMailResponse> ReadMailAsync(
        [Body] ReadMailRequest request,
        CancellationToken cancellationToken = default);
}

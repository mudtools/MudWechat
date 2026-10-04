// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块成员邮箱操作族企业自建应用 SDK
/// （禁用/启用邮箱账号 + 获取邮件未读数）。
/// <para>
/// 官方仅向企业自建应用开放本域 2 个端点，全部收敛声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalSecurityVipService"/> 仅自建子接口承载）。
/// 官方未向第三方应用与服务商代开发应用开放本域，故本家族不声明对应应用类型子接口
/// （能力漂移守卫：继承链上恰好只有自建子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Mail",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailUserService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailUserService : IWechatWorkMailUserService
{
    /// <summary>
    /// 禁用/启用邮箱账号
    /// <para>禁用或启用邮箱，可以操作成员邮箱和业务邮箱（公共邮箱）。</para>
    /// <para>官方业务限制：userid 与 publicemail_id 至少应该传一项，同时传则只操作 userid；
    /// 不可禁用超管与企业创建人。</para>
    /// </summary>
    /// <param name="request">操作请求体（<see cref="SetEmailAccountStatusRequest"/>：userid / publicemail_id 二选一（同时传则只操作 userid），
    /// type 官方必填：1 - 启用，2 - 禁用）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95512"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/account/act_email")]
    Task<WechatWorkResponse> SetEmailAccountStatusAsync(
        [Body] SetEmailAccountStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取邮件未读数
    /// <para>获取指定成员邮箱当前未读邮件数量。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMailUnreadCountRequest"/>：userid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员邮箱中邮件未读数（count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95514"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/mail/get_newcount")]
    Task<GetMailUnreadCountResponse> GetMailUnreadCountAsync(
        [Body] GetMailUnreadCountRequest request,
        CancellationToken cancellationToken = default);
}

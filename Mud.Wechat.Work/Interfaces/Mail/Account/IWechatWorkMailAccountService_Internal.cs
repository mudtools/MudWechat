// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块管理应用邮箱账号族企业自建应用 SDK
/// （更新应用邮箱账号 + 查询应用邮箱账号）。
/// <para>
/// 官方仅向企业自建应用开放本域 2 个端点，全部收敛声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalGovGridListService"/> 仅自建子接口承载）。
/// 官方对第三方应用与服务商代开发应用未开放本域，故本家族不声明对应应用类型子接口
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
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailAccountService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailAccountService : IWechatWorkMailAccountService
{
    /// <summary>
    /// 更新应用邮箱账号
    /// <para>更新应用绑定的邮箱账号；调用本接口更新后，原有的应用邮箱账号会作为别名邮箱保留，且仍具备收信能力。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateEmailAliasRequest"/>：new_email 官方必填，修改后的应用邮箱账号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97373"/></para>
    /// <para>官方注明：更新后原应用邮箱账号作为别名邮箱保留，仍具备收信能力。</para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/update_email_alias")]
    Task<WechatWorkResponse> UpdateEmailAliasAsync(
        [Body] UpdateEmailAliasRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询应用邮箱账号
    /// <para>查询应用自己的应用邮箱账号及别名邮箱；本接口不需要请求包体。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>主邮箱地址（email，发邮件以此邮箱为发件人）与别名邮箱地址列表（alias_list，能作为收件人收邮件）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97991"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/app/get_email_alias")]
    Task<GetEmailAliasResponse> GetEmailAliasAsync(
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块其他邮件客户端登录设置族企业自建应用 SDK
/// （获取用户功能属性 + 更改用户功能属性）。
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
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailUserOptionService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailUserOptionService : IWechatWorkMailUserOptionService
{
    /// <summary>
    /// 获取用户功能属性
    /// <para>获取用户的功能属性（强制启用安全登录 / IMAP/SMTP 服务 / POP/SMTP 服务 / 是否启用安全登录）。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetMailUserOptionRequest"/>：userid / type 官方必填，
    /// type 为功能设置属性类型列表：1 - 强制启用安全登录，2 - IMAP/SMTP 服务，3 - POP/SMTP 服务，4 - 是否启用安全登录）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>功能属性集合（option.list：type / value，value 为 1 表示启用、0 表示关闭）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95513"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/useroption/get")]
    Task<GetMailUserOptionResponse> GetUserOptionAsync(
        [Body] GetMailUserOptionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更改用户功能属性
    /// <para>更新用户的功能属性（强制启用安全登录 / IMAP/SMTP 服务 / POP/SMTP 服务 / 是否启用安全登录）。</para>
    /// </summary>
    /// <param name="request">更新请求体（<see cref="UpdateMailUserOptionRequest"/>：userid / option 官方必填，
    /// 属性项 type 取值同获取接口，value 为 1 表示启用、0 表示关闭）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98008"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/useroption/update")]
    Task<WechatWorkResponse> UpdateUserOptionAsync(
        [Body] UpdateMailUserOptionRequest request,
        CancellationToken cancellationToken = default);
}

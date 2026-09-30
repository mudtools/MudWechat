// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 第三方/服务商套件 suite_access_token 令牌管理器（suite_id + suite_secret + suite_ticket）。
/// </summary>
/// <remarks>
/// <see cref="ISharedTokenManager"/>（Mud.HttpUtils v2.0.9）：全租户共享凭据 →
/// 租户绑定守卫默认豁免。刷新时读取共享 suite_ticket（<see cref="IWechatSuiteTicketProvider"/>，
/// 由回调包写入仓储），换取 suite_access_token。
/// </remarks>
internal sealed class SuiteTokenManager : WechatAppTokenManagerBase, IWechatSuiteTokenManager, ISharedTokenManager
{
    private readonly IWechatWorkProviderAuthentication _auth;
    private readonly IWechatSuiteTicketProvider _ticketProvider;

    /// <summary>创建套件令牌管理器。</summary>
    public SuiteTokenManager(
        IWechatWorkProviderAuthentication auth,
        IWechatSuiteTicketProvider ticketProvider,
        IOptions<WechatAppConfig> options,
        ILogger<SuiteTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, WechatTokenTypes.SuiteAccessToken)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _ticketProvider = ticketProvider ?? throw new ArgumentNullException(nameof(ticketProvider));
    }

    /// <summary>唯一模板点：读取共享 suite_ticket 后换取套件令牌。</summary>
    protected override async Task<(string? AccessToken, int ExpireSeconds)> RefreshTokenFromApiAsync(CancellationToken cancellationToken)
    {
        var ticket = await _ticketProvider.GetSuiteTicketAsync(cancellationToken).ConfigureAwait(false);
        var resp = await _auth
            .GetSuiteTokenAsync(new GetSuiteTokenRequest
            {
                SuiteId = Options.SuiteId,
                SuiteSecret = Options.SuiteSecret,
                SuiteTicket = ticket,
            }, cancellationToken)
            .ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(resp);
        return (resp!.SuiteAccessToken, resp.ExpiresIn);
    }
}

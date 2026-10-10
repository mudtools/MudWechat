// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 服务商 provider_access_token 令牌管理器（corpid + provider_secret）。
/// </summary>
/// <remarks>
/// <see cref="ISharedTokenManager"/>（Mud.HttpUtils v2.0.9）：全租户共享凭据 →
/// 租户绑定守卫默认豁免。凭据（corpid + provider_secret）无租户属性，来自 <see cref="WechatAppConfig"/>。
/// </remarks>
internal sealed class ProviderTokenManager : WechatAppTokenManagerBase, IWechatProviderTokenManager, ISharedTokenManager
{
    private readonly IWechatWorkProviderAuthentication _auth;

    /// <summary>创建服务商令牌管理器。</summary>
    public ProviderTokenManager(
        IWechatWorkProviderAuthentication auth,
        IOptions<WechatAppConfig> options,
        ILogger<ProviderTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, WechatTokenTypes.ProviderAccessToken)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    /// <summary>唯一模板点：换取服务商令牌。</summary>
    protected override async Task<(string? AccessToken, int ExpireSeconds)> RefreshTokenFromApiAsync(CancellationToken cancellationToken)
    {
        var resp = await _auth
            .GetProviderTokenAsync(new GetProviderTokenRequest
            {
                CorpId = Options.CorpId,
                ProviderSecret = Options.ProviderSecret,
            }, cancellationToken)
            .ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(resp);
        return (resp!.ProviderAccessToken, resp.ExpiresIn);
    }
}

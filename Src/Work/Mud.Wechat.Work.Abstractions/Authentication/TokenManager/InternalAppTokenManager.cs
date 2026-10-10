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
/// 自建应用 access_token 令牌管理器（corpid + corpsecret 直换，无授权流程）。
/// </summary>
internal sealed class InternalAppTokenManager : WechatAppTokenManagerBase, IWechatInternalAppTokenManager
{
    private readonly IWechatWorkInternalAppAuthentication _auth;

    /// <summary>创建自建应用令牌管理器。</summary>
    public InternalAppTokenManager(
        IWechatWorkInternalAppAuthentication auth,
        IOptions<WechatAppConfig> options,
        ILogger<InternalAppTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, WechatTokenTypes.AccessToken)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    /// <summary>唯一模板点：换取令牌；缓存 / 阈值 / 失效级联由基座承担。</summary>
    protected override async Task<(string? AccessToken, int ExpireSeconds)> RefreshTokenFromApiAsync(CancellationToken cancellationToken)
    {
        var resp = await _auth
            .GetTokenAsync(Options.CorpId, Options.AgentSecret, cancellationToken)
            .ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(resp);
        return (resp!.AccessToken, resp.ExpiresIn);
    }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 稳定版通道令牌管理器（<c>POST /cgi-bin/stable_token</c>，官方推荐；默认通道）。
/// </summary>
/// <remarks>
/// <para>
/// <b><c>force_refresh</c> 恒为 <c>false</c></b>：本地缓存失效后调用普通模式即可取回平台当前有效
/// token（平台提前 5 分钟轮换，本地提前刷新阈值更早拦截）。官方「强制刷新模式每天限 20 次且需间隔 30 秒」
/// 属配额约束，本 SDK 不提供强刷开关（无消费场景，避免烧配额）。
/// </para>
/// <para>
/// 通道键前缀 <c>Wechat.Mp.StableAccessToken</c>：与普通通道的持久化键空间隔离
/// （官方「两个接口的凭据完全隔离、互不影响」）。
/// </para>
/// </remarks>
internal sealed class MpStableAccessTokenManager : MpAccessTokenManagerBase, IMpAccessTokenManager
{
    /// <summary>稳定版通道持久化键前缀（与普通通道隔离）。</summary>
    internal const string ChannelKeyPrefix = "Wechat.Mp.StableAccessToken";

    private readonly IMpAuthentication _auth;

    /// <summary>创建稳定版通道令牌管理器。</summary>
    /// <param name="auth">令牌签发客户端（per-app）。</param>
    /// <param name="options">应用配置。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储。</param>
    public MpStableAccessTokenManager(
        IMpAuthentication auth,
        IOptions<MpAppConfig> options,
        ILogger<MpStableAccessTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, ChannelKeyPrefix)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    /// <inheritdoc />
    protected override async Task<(string? AccessToken, int ExpireSeconds)> RefreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new MpStableTokenRequest
        {
            GrantType = "client_credential",
            AppId = Options.AppId,
            Secret = Options.AppSecret,
            ForceRefresh = false,
        };

        var resp = await _auth.GetStableTokenAsync(request, cancellationToken).ConfigureAwait(false);
        MpException.ThrowIfFailed(resp);
        return (resp!.AccessToken, resp.ExpiresIn);
    }
}

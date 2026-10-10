// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 普通通道令牌管理器（<c>GET /cgi-bin/token</c>；官方已推荐改用稳定版通道）。
/// </summary>
/// <remarks>
/// <para>
/// 启用方式：<c>MpAppConfig.UseStableToken = false</c>（注册期装配）。
/// 两个通道的凭据官方声明<b>完全隔离、互不影响</b>，故持久化键前缀独立为
/// <c>Wechat.Mp.StandardAccessToken</c>，与稳定版通道互不串号。
/// </para>
/// <para>
/// 官方对该端点未声明隔离与频率限制细节（隔离声明仅出现在 <c>getStableAccessToken</c> 页）；
/// <c>40001</c> 在本通道语义为「AppSecret 错误」。
/// </para>
/// </remarks>
internal sealed class MpStandardAccessTokenManager : MpAccessTokenManagerBase, IMpAccessTokenManager
{
    /// <summary>普通通道持久化键前缀（与稳定版通道隔离）。</summary>
    internal const string ChannelKeyPrefix = "Wechat.Mp.StandardAccessToken";

    private readonly IMpAuthentication _auth;

    /// <summary>创建普通通道令牌管理器。</summary>
    /// <param name="auth">令牌签发客户端（per-app）。</param>
    /// <param name="options">应用配置。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储。</param>
    public MpStandardAccessTokenManager(
        IMpAuthentication auth,
        IOptions<MpAppConfig> options,
        ILogger<MpStandardAccessTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, ChannelKeyPrefix)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    /// <inheritdoc />
    protected override async Task<(string? AccessToken, int ExpireSeconds)> RefreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var resp = await _auth
            .GetTokenAsync(Options.AppId, Options.AppSecret, "client_credential", cancellationToken)
            .ConfigureAwait(false);
        MpException.ThrowIfFailed(resp);
        return (resp!.AccessToken, resp.ExpiresIn);
    }
}

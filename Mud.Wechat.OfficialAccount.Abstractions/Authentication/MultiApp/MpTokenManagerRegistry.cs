// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;

/// <summary>
/// per-app 令牌管理器注册表（errcode 恢复链路按 <c>TokenRecoveryContext.TokenManagerKey</c> 定位管理器）。
/// </summary>
/// <remarks>
/// 公众号只有一种令牌类型 ⇒ 单槽注册表；未命中键返回 <c>null</c>（恢复链路据此放弃该次重试）。
/// </remarks>
internal sealed class MpTokenManagerRegistry : ITokenManagerRegistry
{
    private readonly IMpAccessTokenManager _accessTokenManager;

    /// <summary>创建注册表。</summary>
    /// <param name="accessTokenManager">本应用的 access_token 令牌管理器。</param>
    public MpTokenManagerRegistry(IMpAccessTokenManager accessTokenManager)
    {
        _accessTokenManager = accessTokenManager ?? throw new ArgumentNullException(nameof(accessTokenManager));
    }

    /// <inheritdoc />
    public ITokenManager? Resolve(string tokenManagerKey)
        => string.Equals(tokenManagerKey, MpTokenTypes.AccessToken, StringComparison.Ordinal)
            ? _accessTokenManager
            : null;
}

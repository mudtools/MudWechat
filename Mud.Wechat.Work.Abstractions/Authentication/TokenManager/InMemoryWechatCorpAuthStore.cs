// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// <see cref="IWechatCorpAuthStore"/> 的进程内默认实现（生产环境建议持久化到数据库）。
/// </summary>
public sealed class InMemoryWechatCorpAuthStore : IWechatCorpAuthStore
{
    private readonly ConcurrentDictionary<(string AppKey, string AuthCorpId), WechatCorpAuthorization> _auths = new();

    /// <inheritdoc />
    public Task<WechatCorpAuthorization?> GetAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default)
        => Task.FromResult(_auths.TryGetValue(Normalize(appKey, authCorpId), out var auth) ? auth : null);

    /// <inheritdoc />
    public Task SetAsync(WechatCorpAuthorization auth, CancellationToken cancellationToken = default)
    {
        if (auth == null) throw new ArgumentNullException(nameof(auth));
        _auths[Normalize(auth.AppKey, auth.AuthCorpId)] = auth;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default)
    {
        _auths.TryRemove(Normalize(appKey, authCorpId), out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<WechatCorpAuthorization>> ListAsync(string appKey, CancellationToken cancellationToken = default)
    {
        var key = appKey ?? string.Empty;
        IReadOnlyList<WechatCorpAuthorization> result = _auths
            .Where(pair => string.Equals(pair.Key.AppKey, key, StringComparison.Ordinal))
            .Select(pair => pair.Value)
            .ToList();
        return Task.FromResult(result);
    }

    private static (string AppKey, string AuthCorpId) Normalize(string appKey, string authCorpId)
        => (appKey ?? string.Empty, authCorpId ?? string.Empty);
}

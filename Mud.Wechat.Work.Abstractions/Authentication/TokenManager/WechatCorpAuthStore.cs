// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 企业授权信息（永久授权码）仓储。
/// </summary>
/// <remarks>
/// <c>get_permanent_code</c> 成功后由业务侧将 authCorpId + permanent_code 持久化到本仓储，
/// <c>CorpTokenManager</c> 据此换取企业 access_token；<c>cancel_auth</c> 回调时清理对应条目。
/// </remarks>
public interface IWechatCorpAuthStore
{
    /// <summary>读取指定企业的授权信息（不存在时返回 null）。</summary>
    Task<CorpAuth?> GetAsync(string authCorpId, CancellationToken cancellationToken = default);

    /// <summary>写入/更新指定企业的授权信息。</summary>
    Task SetAsync(CorpAuth auth, CancellationToken cancellationToken = default);

    /// <summary>移除指定企业的授权信息（取消授权）。</summary>
    Task RemoveAsync(string authCorpId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 企业授权信息条目。
/// </summary>
public sealed class CorpAuth
{
    /// <summary>创建企业授权信息。</summary>
    public CorpAuth(string corpId, string permanentCode)
    {
        CorpId = corpId;
        PermanentCode = permanentCode;
    }

    /// <summary>授权方（企业）CorpId。</summary>
    public string CorpId { get; }

    /// <summary>永久授权码（永不写入日志）。</summary>
    public string PermanentCode { get; }
}

/// <summary>
/// <see cref="IWechatCorpAuthStore"/> 的进程内默认实现（生产环境建议持久化到数据库）。
/// </summary>
public sealed class InMemoryWechatCorpAuthStore : IWechatCorpAuthStore
{
    private readonly ConcurrentDictionary<string, CorpAuth> _auths = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<CorpAuth?> GetAsync(string authCorpId, CancellationToken cancellationToken = default)
        => Task.FromResult(_auths.TryGetValue(authCorpId ?? string.Empty, out var auth) ? auth : null);

    /// <inheritdoc />
    public Task SetAsync(CorpAuth auth, CancellationToken cancellationToken = default)
    {
        if (auth == null) throw new ArgumentNullException(nameof(auth));
        _auths[auth.CorpId] = auth;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string authCorpId, CancellationToken cancellationToken = default)
    {
        _auths.TryRemove(authCorpId ?? string.Empty, out _);
        return Task.CompletedTask;
    }
}

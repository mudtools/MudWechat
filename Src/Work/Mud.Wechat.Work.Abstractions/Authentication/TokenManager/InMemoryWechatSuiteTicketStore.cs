// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// <see cref="IWechatSuiteTicketStore"/> 的进程内默认实现（按 suiteId 分槽）。
/// </summary>
public sealed class InMemoryWechatSuiteTicketStore : IWechatSuiteTicketStore
{
    private readonly ConcurrentDictionary<string, string> _tickets = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<string?> GetAsync(string suiteId, CancellationToken cancellationToken = default)
        => Task.FromResult(_tickets.TryGetValue(suiteId ?? string.Empty, out var ticket) ? ticket : null);

    /// <inheritdoc />
    public Task SetAsync(string suiteId, string ticket, CancellationToken cancellationToken = default)
    {
        _tickets[suiteId ?? string.Empty] = ticket;
        return Task.CompletedTask;
    }
}

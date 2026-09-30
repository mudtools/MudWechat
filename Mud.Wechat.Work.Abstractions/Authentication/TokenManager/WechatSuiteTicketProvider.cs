// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// suite_ticket 仓储接口。suite_ticket 由企业微信每 10 分钟推送到回调地址，
/// 不能主动获取——由回调处理器解析后写入，<c>SuiteTokenManager</c> 经
/// <see cref="IWechatSuiteTicketProvider"/> 读取。
/// </summary>
public interface IWechatSuiteTicketStore
{
    /// <summary>读取最新推送的 suite_ticket（未入库时返回 null）。</summary>
    Task<string?> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>写入最新推送的 suite_ticket（覆盖旧值）。</summary>
    Task SetAsync(string ticket, CancellationToken cancellationToken = default);
}

/// <summary>
/// suite_ticket 供应接口（<c>SuiteTokenManager</c> 刷新时读取）。
/// </summary>
public interface IWechatSuiteTicketProvider
{
    /// <summary>获取当前可用的 suite_ticket；未入库时抛出并提示宿主配置回调接收。</summary>
    Task<string> GetSuiteTicketAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// <see cref="IWechatSuiteTicketStore"/> 的进程内默认实现。
/// </summary>
public sealed class InMemoryWechatSuiteTicketStore : IWechatSuiteTicketStore
{
    private volatile string? _ticket;

    /// <inheritdoc />
    public Task<string?> GetAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_ticket);

    /// <inheritdoc />
    public Task SetAsync(string ticket, CancellationToken cancellationToken = default)
    {
        _ticket = ticket;
        return Task.CompletedTask;
    }
}

/// <summary>
/// <see cref="IWechatSuiteTicketProvider"/> 的默认实现：从 <see cref="IWechatSuiteTicketStore"/> 读取。
/// </summary>
public sealed class WechatSuiteTicketProvider : IWechatSuiteTicketProvider
{
    private readonly IWechatSuiteTicketStore _store;

    /// <summary>创建套件票据供应器。</summary>
    public WechatSuiteTicketProvider(IWechatSuiteTicketStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <inheritdoc />
    public Task<string> GetSuiteTicketAsync(CancellationToken cancellationToken = default)
    {
        var ticket = _store.GetAsync(cancellationToken).GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(ticket))
        {
            throw new InvalidOperationException(
                "suite_ticket 尚未接收：请接入 Mud.Wechat.Work.Callback 回调接收（微信每 10 分钟推送一次），" +
                "或自行实现 IWechatSuiteTicketStore 写入最新票据后再调用套件令牌接口。");
        }

        return Task.FromResult(ticket);
    }
}

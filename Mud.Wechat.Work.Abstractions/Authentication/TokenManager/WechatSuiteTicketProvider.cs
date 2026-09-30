// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

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
    public async Task<string> GetSuiteTicketAsync(string suiteId, CancellationToken cancellationToken = default)
    {
        var ticket = await _store.GetAsync(suiteId, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(ticket))
        {
            throw new InvalidOperationException(
                $"套件 {suiteId} 的 suite_ticket 尚未接收：请接入 Mud.Wechat.Work.Callback 回调接收（微信每 10 分钟推送一次），" +
                "或自行实现 IWechatSuiteTicketStore 写入最新票据后再调用套件令牌接口。");
        }

        return ticket;
    }
}

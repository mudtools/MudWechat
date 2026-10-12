// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 契约注册表默认实现（并发字典；组合根期写入、运行期只读）。
/// </summary>
public sealed class ChannelsPayloadContractRegistry : IChannelsPayloadContractRegistry
{
    private readonly ConcurrentDictionary<string, ChannelsPayloadContract> _contracts =
        new(StringComparer.Ordinal);

    /// <inheritdoc />
    public void Register(ChannelsPayloadContract contract)
    {
        if (contract == null)
        {
            throw new ArgumentNullException(nameof(contract));
        }

        _contracts[contract.EventTypeKey] = contract;
    }

    /// <inheritdoc />
    public bool TryResolve(string eventTypeKey, out ChannelsPayloadContract? contract)
    {
        contract = null;
        if (string.IsNullOrEmpty(eventTypeKey))
        {
            return false;
        }

        return _contracts.TryGetValue(eventTypeKey, out contract);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> RegisteredKeys => _contracts.Keys.ToArray();
}

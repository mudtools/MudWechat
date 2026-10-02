// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// <see cref="IWechatPayloadContractRegistry"/> 默认实现：无锁读 + 组合根期登记。
/// </summary>
/// <remarks>
/// <b>为何不用快照字典</b>：登记仅发生在组合根期且随后只读，
/// <c>System.Collections.Concurrent.ConcurrentDictionary&lt;string, WechatPayloadContract&gt;</c>
/// 已满足无锁读；引入快照会为「登记后不可变」这一已成立的前提增加无收益的复杂度
/// （与 v1.2「注册表无 Freeze」是同一判断：冻结语义在此模型下是死代码）。
/// </remarks>
public sealed class WechatPayloadContractRegistry : IWechatPayloadContractRegistry
{
    private readonly ConcurrentDictionary<string, WechatPayloadContract> _contracts =
        new ConcurrentDictionary<string, WechatPayloadContract>(StringComparer.Ordinal);

    private readonly List<string> _order = new List<string>();

    /// <inheritdoc />
    public void Register(WechatPayloadContract contract)
    {
        if (contract == null)
            throw new ArgumentNullException(nameof(contract));

        if (string.IsNullOrEmpty(contract.EventTypeKey))
            throw new ArgumentException("契约的事件键不得为空。", nameof(contract));

        lock (_order)
        {
            if (_contracts.ContainsKey(contract.EventTypeKey))
            {
                throw new ArgumentException(
                    "事件键 " + contract.EventTypeKey + " 已登记契约；同一键重复登记属接线错误（宿主如需覆盖，请先确保未重复调用 AddPayload）。",
                    nameof(contract));
            }

            if (!_contracts.TryAdd(contract.EventTypeKey, contract))
            {
                throw new ArgumentException("事件键 " + contract.EventTypeKey + " 登记失败。", nameof(contract));
            }

            _order.Add(contract.EventTypeKey);
        }
    }

    /// <inheritdoc />
    public bool TryResolve(string eventTypeKey, out WechatPayloadContract? contract)
    {
        contract = null;
        if (string.IsNullOrEmpty(eventTypeKey))
            return false;

        return _contracts.TryGetValue(eventTypeKey, out contract);
    }

    /// <inheritdoc />
    /// <remarks>返回<b>快照副本</b>（登记序），避免调用方在读取路径上持有内部列表。</remarks>
    public IReadOnlyList<string> RegisteredKeys
    {
        get
        {
            lock (_order)
            {
                return _order.ToArray();
            }
        }
    }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 事件键 → 载荷契约注册表（组合根期登记，运行期只读）。
/// </summary>
public interface IChannelsPayloadContractRegistry
{
    /// <summary>登记（同键重复登记以最后一次为准；特性同键合并由生成器负责）。</summary>
    /// <param name="contract">载荷契约。</param>
    void Register(ChannelsPayloadContract contract);

    /// <summary>按事件键解析契约。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="contract">命中的契约；未命中为 <c>null</c>。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    bool TryResolve(string eventTypeKey, out ChannelsPayloadContract? contract);

    /// <summary>已登记的事件键集合（<b>防静默空跑</b>：守卫与测试据此断言登记面下限）。</summary>
    IReadOnlyCollection<string> RegisteredKeys { get; }
}

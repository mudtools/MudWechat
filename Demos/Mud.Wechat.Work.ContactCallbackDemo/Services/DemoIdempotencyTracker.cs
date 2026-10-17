// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.Work.ContactCallbackDemo.Services;

/// <summary>
/// 处理器幂等去重（Demo 形态）：SDK 的抗重放指纹只挡「同一条报文」的重复投递，
/// 而官方在超时/5xx 后会以<b>新报文</b>重推同一业务变更，故业务侧仍须幂等。
/// </summary>
/// <remarks>
/// <b>仅作教学演示</b>：进程内字典在多实例部署与重启后失效。生产须用数据库唯一键
/// （如 <c>(事件键, 业务主键, 变更序号)</c> 组合唯一索引 + <c>INSERT ... ON CONFLICT DO NOTHING</c>）。
/// </remarks>
public sealed class DemoIdempotencyTracker
{
    /// <summary>条目上限（超出即整体清空，Demo 不追求精确 LRU）。</summary>
    private const int Capacity = 4096;

    private readonly ConcurrentDictionary<string, byte> _seen = new(StringComparer.Ordinal);

    /// <summary>
    /// 首次见到该键返回 <see langword="true"/>（应执行副作用）；重复返回 <see langword="false"/>（应跳过）。
    /// </summary>
    public bool TryBegin(string key)
    {
        if (_seen.Count >= Capacity)
        {
            _seen.Clear();
        }

        return _seen.TryAdd(key, 0);
    }

    /// <summary>
    /// 构造业务幂等键：事件类型键 + 业务主键 + 信封 <c>CreateTime</c>。
    /// </summary>
    /// <param name="eventData">回调事件信封。</param>
    /// <param name="businessId">业务主键（成员 UserId / 部门 PartyId / 标签 TagId）。</param>
    public static string BuildKey(WechatCallbackEvent eventData, string? businessId)
        => string.Concat(eventData.EventTypeKey, "|", businessId, "|", eventData.CreateTime);
}

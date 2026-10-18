// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;

namespace Mud.Wechat.OfficialAccount.Demo.Services;

/// <summary>
/// 被动回复的<b>排重</b>记录器：微信官方「5 秒内收不到响应即断连重推，共 3 次」，
/// 同一条推送被重推时若无条件回包，用户会收到重复消息。
/// </summary>
/// <remarks>
/// <para>
/// 排重键按官方口径分形态（普通消息用 <c>MsgId</c>，事件用 <c>FromUserName + CreateTime</c>），
/// 见 <see cref="BuildKey"/>。SDK 侧的一次性指纹闸拦掉的是「重复投递到达分发器」，
/// 而本记录器拦的是「重复投递已被处理过、不该再回包」——两者层次不同，不互相替代。
/// </para>
/// <para><b>仅适用于 Demo</b>：进程内字典、超容量整体清空。生产必须换成带唯一索引的持久化排重表。</para>
/// </remarks>
public sealed class MpReplyDeduper
{
    private const int Capacity = 4096;

    private readonly ConcurrentDictionary<string, byte> _seen = new();

    /// <summary>首次见到该键返回 <see langword="true"/>（应回包）；重复返回 <see langword="false"/>（应静默）。</summary>
    public bool TryBegin(string key)
    {
        if (_seen.Count >= Capacity)
        {
            _seen.Clear();
        }

        return _seen.TryAdd(key, 0);
    }

    /// <summary>按官方排重口径构造键：事件用「用户 + 创建时间」，消息用 <c>MsgId</c>。</summary>
    public static string BuildKey(MpCallbackEnvelope envelope)
    {
        var businessId = string.IsNullOrEmpty(envelope.MsgId)
            ? string.Concat(envelope.FromUserName, "/", envelope.CreateTime)
            : envelope.MsgId;

        return string.Concat(envelope.EventTypeKey, "|", businessId);
    }
}

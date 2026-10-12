// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 事件键级载荷契约（小店侧，JSON 反序列化形态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微 / 公众号契约的差异（有意为之）</b>：小店回调明文为<b> JSON</b>（官方强制安全模式），
/// 载荷经源生成 JSON 上下文直接反序列化，<b>不经</b> XML 字段映射器（<c>[PayloadContract]</c> /
/// <c>IPayloadContractAccessor</c> 机制不适用）—— 故契约只有「事件键 + 载荷类型」两项。
/// </para>
/// <para>
/// 官方新增事件键若不登记契约，类型化处理器读取不到载荷（分发器记 Error 跳过该处理器），
/// 信封级处理器不受影响；键登记后处理器即获得强类型载荷。
/// </para>
/// </remarks>
public sealed class ChannelsPayloadContract
{
    private ChannelsPayloadContract(string eventTypeKey, Type payloadType)
    {
        EventTypeKey = eventTypeKey;
        PayloadType = payloadType;
    }

    /// <summary>事件类型键（= 回调事件信封的 <c>EventTypeKey</c>）。</summary>
    public string EventTypeKey { get; }

    /// <summary>载荷类型（须登记进本线源生成 JSON 上下文，AOT 可解析）。</summary>
    public Type PayloadType { get; }

    /// <summary>创建契约。</summary>
    /// <param name="eventTypeKey">事件类型键（须为官方 <c>Event</c> 取值）。</param>
    /// <param name="payloadType">载荷类型。</param>
    /// <returns>契约实例。</returns>
    /// <exception cref="ArgumentException"><paramref name="eventTypeKey"/> 为空白。</exception>
    public static ChannelsPayloadContract Create(string eventTypeKey, Type payloadType)
    {
        if (string.IsNullOrWhiteSpace(eventTypeKey))
        {
            throw new ArgumentException("事件键不能为空（契约登记以事件键为唯一索引）。", nameof(eventTypeKey));
        }

        if (payloadType == null)
        {
            throw new ArgumentNullException(nameof(payloadType));
        }

        return new ChannelsPayloadContract(eventTypeKey, payloadType);
    }

}

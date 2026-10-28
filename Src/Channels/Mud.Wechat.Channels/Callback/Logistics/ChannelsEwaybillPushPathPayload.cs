// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 物流族（物流轨迹推送通知）。
// 事件键：ewaybill_push_path。
// 注意：本事件应答为 JSON 回显（含 ResultCode/ResultMsg），与其它事件「回复 success 或空字符串」不同。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 物流轨迹推送通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.EwaybillPushPath"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 当电子面单对应的物流轨迹发生变化时，微信小店将消息通知给供货商。
/// </para>
/// <para>
/// <b>应答差异</b>：官方返回示例为 JSON 回显（ToUserName/FromUserName/CreateTime/MsgType/Event/
/// <c>ResultCode</c>:0/<c>ResultMsg</c>:"success"），与其它事件的「回复 success 或空字符串」不同（照录）。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/merchant_callback/ewaybill_push_path.html">物流轨迹推送通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.EwaybillPushPath })]
public sealed class ChannelsEwaybillPushPathPayload
{
    /// <summary>获取或设置 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>ewaybill_push_path</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>waybill_info</c>）。</summary>
    [JsonPropertyName("waybill_info")]
    public ChannelsWaybillInfo? WaybillInfo { get; set; }
}

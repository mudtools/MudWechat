// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 关注 / 取消关注事件载荷（<c>subscribe</c> / <c>unsubscribe</c>；官方「接收事件推送」页，V1 已核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族合并</b>：官方两键的报文结构一致（信封 + <b>可选</b>的 <c>EventKey</c>/<c>Ticket</c>）——
/// 普通关注与取消关注<b>不携带</b>这两字段，而「扫码关注（未关注）」携带
/// <c>EventKey</c>（<c>qrscene_</c> + 场景值 ID）与 <c>Ticket</c>；具体形态由 <c>EventKey</c> 前缀判别
/// ⇒ 合并为一个载荷，两属性可空。
/// </para>
/// <para>
/// <b>官方注意事项（不得忽略）</b>：① 取消关注事件的处理器<b>必须删除该用户的所有信息</b>（隐私合规）；
/// ② 消息排重推荐 <c>FromUserName</c> + <c>CreateTime</c>（普通消息用 <c>MsgId</c>）；
/// ③ 5 秒内收不到响应会断连并重试共 3 次，无法及时处理可回复空串（SDK 默认回 <c>success</c>）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCallbackEventTypes.Subscribe,
    MpCallbackEventTypes.Unsubscribe,
})]
public sealed partial class MpSubscribeEventPayload : MpCallbackPayload
{
    /// <summary>
    /// 事件 KEY 值（**仅扫码关注携带**）：<c>qrscene_</c> 前缀 + 二维码场景值 ID；
    /// 普通关注 / 取消关注为 <c>null</c>。
    /// </summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>二维码 ticket（仅扫码关注携带；可用于换取二维码图片）；其余形态为 <c>null</c>。</summary>
    [PayloadField("Ticket")]
    public string? Ticket { get; set; }
}

/// <summary>
/// 扫描带参数二维码事件载荷（所关注用户扫码时推送，<c>Event = SCAN</c>；V1 已核验）。
/// </summary>
/// <remarks>
/// 专有字段：<c>EventKey</c>（二维码场景值 ID，<b>不带</b> <c>qrscene_</c> 前缀——该前缀仅出现在
/// 「未关注 + 扫码关注」的 <c>subscribe</c> 报文中）+ <c>Ticket</c>。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackEventTypes.Scan })]
public sealed partial class MpScanEventPayload : MpCallbackPayload
{
    /// <summary>二维码场景值 ID（官方 <c>EventKey</c>；已关注用户扫码时为纯场景值）。</summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>二维码 ticket（官方 <c>Ticket</c>；可用于换取二维码图片）。</summary>
    [PayloadField("Ticket")]
    public string? Ticket { get; set; }
}

/// <summary>
/// 上报地理位置事件载荷（<c>Event = LOCATION</c>；V1 已核验）。
/// </summary>
/// <remarks>
/// <b>与普通 <c>location</c> 消息的结构差异（官方明确区分）</b>：
/// 本事件用 <c>Latitude</c>/<c>Longitude</c>/<c>Precision</c>；
/// 普通消息用 <c>Location_X</c>/<c>Location_Y</c>/<c>Scale</c>/<c>Label</c>（见 <see cref="MpLocationMessagePayload"/>）。
/// 触发频率：进入会话时一次、进入后每 5 秒一次（公众平台可改）。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCallbackEventTypes.Location })]
public sealed partial class MpLocationEventPayload : MpCallbackPayload
{
    /// <summary>地理位置纬度（官方 <c>Latitude</c>）。</summary>
    [PayloadField("Latitude")]
    public string? Latitude { get; set; }

    /// <summary>地理位置经度（官方 <c>Longitude</c>）。</summary>
    [PayloadField("Longitude")]
    public string? Longitude { get; set; }

    /// <summary>地理位置精度（官方 <c>Precision</c>）。</summary>
    [PayloadField("Precision")]
    public string? Precision { get; set; }
}

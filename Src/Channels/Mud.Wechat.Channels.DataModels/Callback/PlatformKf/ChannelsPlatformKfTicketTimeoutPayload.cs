// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 平台客服工单族（工单创建 / 工单超时）。
// 事件键：ec_platform_kf_create_mch_ticket、ec_platform_kf_ticket_timeout。
// 注意：两事件官方参数表仅列 request_data 一行，信封公共字段（ToUserName 等）仅见于官方示例
// —— 按公共信封 + request_data 建模；request_data 字段集相同（仅 ticket_id），共用嵌套类。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 工单超时通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.PlatformKfTicketTimeout"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 商家工单处理超时时推送。<b>信封差异</b>：官方参数表仅列 <c>request_data</c> 一行，
/// 信封公共字段仅见于官方示例（照录补齐建模）。
/// </para>
/// <para>
/// <b>应答差异</b>：页面文字为「回复 success 或空字符串（无需加密）」，但官方返回示例为 JSON 回显
/// （含 <c>ResultCode</c>/<c>ResultMsg</c>）—— 两形态并存，照录。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/platformkf/ec_platform_kf_ticket_timeout.html">工单超时通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.PlatformKfTicketTimeout })]
public sealed class ChannelsPlatformKfTicketTimeoutPayload
{
    /// <summary>获取或设置小店 UserName（官方参数表未列，见官方示例）。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置 OpenID（官方参数表未列，见官方示例）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒；官方参数表未列，见官方示例）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>ec_platform_kf_ticket_timeout</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置请求体（官方 <c>request_data</c>；与工单创建通知字段集相同，共用 <see cref="ChannelsPlatformKfTicketInfo"/>）。</summary>
    [JsonPropertyName("request_data")]
    public ChannelsPlatformKfTicketInfo? RequestData { get; set; }
}

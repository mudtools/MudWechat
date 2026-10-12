// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 售后族（售后单更新 / 纠纷单更新 / 保障单更新）。
// 事件键：channels_ec_aftersale_update、channels_ec_complaint_update、channels_ec_guarantee_update。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 售后单更新通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.AftersaleUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方矛盾照录</b>：<c>wxa_vip_discounted_price</c> 在官方参数表中为<b>顶层</b>字段
/// （与 <c>finder_shop_aftersale_status_update</c> 平级），而官方示例将其嵌在
/// <c>finder_shop_aftersale_status_update</c> 内 —— 按参数表建模为顶层字段。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/aftersale_callback/channels_ec_aftersale_update.html">售后单更新通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.AftersaleUpdate })]
public sealed class ChannelsAftersaleUpdatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置固定的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_aftersale_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>finder_shop_aftersale_status_update</c>）。</summary>
    [JsonPropertyName("finder_shop_aftersale_status_update")]
    public ChannelsAftersaleStatusInfo? AftersaleStatusUpdate { get; set; }

    /// <summary>获取或设置小程序会员已优惠金额，单位分（官方 <c>wxa_vip_discounted_price</c>，参数表为顶层字段、官方示例嵌在事件主体内，按参数表建模）。</summary>
    [JsonPropertyName("wxa_vip_discounted_price")]
    public long? WxaVipDiscountedPrice { get; set; }
}

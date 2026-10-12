// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 小程序会员族（静默关联取会员信息 / 静默关联通知 / 下单页初始权益 /
// 使用会员权益 / 取消使用会员权益；回调的是小程序的回调 URL）。
// 事件键：channels_ec_vip_user_info_get、channels_ec_vip_user_auto_add、channels_ec_gen_order_info_init、
// channels_ec_order_use_vip_discount、channels_ec_order_unuse_vip_discount。
// 注意：info_get / gen_order_info_init / use_vip_discount 三事件官方要求返回结构化 JSON（非 success 文本）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 用户下单使用会员权益回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.OrderUseVipDiscount"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 用户提交订单（未支付成功）且使用会员权益时实时回调，商家确认后才能以此金额下单
/// （该接口需商家小程序侧上线）。
/// </para>
/// <para>
/// <b>应答差异</b>：本事件须返回结构化 JSON（<c>errcode</c>/<c>errmsg</c>/<c>result</c>
/// (number，1 允许下单、2 不允许下单，为空或其他值时拦截下单)），非「success 文本」形态。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/wxa_callback/channels_ec_order_use_vip_discount.html">用户下单使用会员权益</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.OrderUseVipDiscount })]
public sealed class ChannelsOrderUseVipDiscountPayload
{
    /// <summary>获取或设置小程序 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置小程序用户 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_order_use_vip_discount</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置订单信息主体（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsUseVipDiscountOrderInfo? OrderInfo { get; set; }
}

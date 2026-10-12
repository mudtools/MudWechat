// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 优惠券族（领取 / 创建 / 删除 / 过期 / 信息更新 / 作废 / 用户券过期 / 返还 / 核销）。
// 事件键：channels_ec_coupon_*、channels_ec_user_coupon_*。
// 注意：channels_ec_user_coupon_use 的官方参数表 Event 行误写为 unuse（复制粘贴错误），
// 以页面标题「优惠券核销通知」、头部事件英文名与示例三者一致值为准。
// 各事件 coupon_info/use_info 的时间字段名互不相同（create_time/delete_time/expire_time/change_time/
// invalid_time/unuse_time/use_time），JSON 形态各异，按官方逐事件建模、不共用嵌套类。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 领取优惠券通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponReceive"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_receive.html">领取优惠券通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponReceive })]
public sealed class ChannelsCouponReceivePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置领券用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_receive</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>receive_info</c>）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsCouponReceiveInfo? ReceiveInfo { get; set; }
}

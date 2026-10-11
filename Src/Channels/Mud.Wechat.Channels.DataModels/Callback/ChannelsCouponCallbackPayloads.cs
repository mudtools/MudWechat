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

/// <summary>领取优惠券事件主体（官方 <c>receive_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponReceiveInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置领券时间（官方 <c>receive_time</c>，官方示例为秒级时间戳字符串）。</summary>
    [JsonPropertyName("receive_time")]
    public string? ReceiveTime { get; set; }
}

/// <summary>
/// 创建优惠券通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponCreate"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_create.html">创建优惠券通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponCreate })]
public sealed class ChannelsCouponCreatePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信团队的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_create</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponCreateInfo? CouponInfo { get; set; }
}

/// <summary>创建优惠券事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponCreateInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }
}

/// <summary>
/// 删除优惠券通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponDelete"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_delete.html">删除优惠券通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponDelete })]
public sealed class ChannelsCouponDeletePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信团队的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_delete</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponDeleteInfo? CouponInfo { get; set; }
}

/// <summary>删除优惠券事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponDeleteInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置删除时间（官方 <c>delete_time</c>）。</summary>
    [JsonPropertyName("delete_time")]
    public string? DeleteTime { get; set; }
}

/// <summary>
/// 优惠券过期通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponExpire"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_expire.html">优惠券过期通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponExpire })]
public sealed class ChannelsCouponExpirePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信团队的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_expire</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponExpireInfo? CouponInfo { get; set; }
}

/// <summary>优惠券过期事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponExpireInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置过期时间（官方 <c>expire_time</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }
}

/// <summary>
/// 优惠券信息更新通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponInfoChange"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_info_change.html">优惠券信息更新通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponInfoChange })]
public sealed class ChannelsCouponInfoChangePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信团队的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_info_change</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponInfoChangeInfo? CouponInfo { get; set; }
}

/// <summary>优惠券信息更新事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponInfoChangeInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>change_time</c>）。</summary>
    [JsonPropertyName("change_time")]
    public string? ChangeTime { get; set; }
}

/// <summary>
/// 作废优惠券通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.CouponInvalid"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_coupon_invalid.html">作废优惠券通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.CouponInvalid })]
public sealed class ChannelsCouponInvalidPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信团队的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_coupon_invalid</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponInvalidInfo? CouponInfo { get; set; }
}

/// <summary>作废优惠券事件主体（官方 <c>coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsCouponInvalidInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置作废时间（官方 <c>invalid_time</c>）。</summary>
    [JsonPropertyName("invalid_time")]
    public string? InvalidTime { get; set; }
}

/// <summary>
/// 用户优惠券过期通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.UserCouponExpire"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_user_coupon_expire.html">用户优惠券过期通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.UserCouponExpire })]
public sealed class ChannelsUserCouponExpirePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置优惠券所属用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_user_coupon_expire</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>user_coupon_info</c>）。</summary>
    [JsonPropertyName("user_coupon_info")]
    public ChannelsUserCouponExpireInfo? UserCouponInfo { get; set; }
}

/// <summary>用户优惠券过期事件主体（官方 <c>user_coupon_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsUserCouponExpireInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置过期时间（官方 <c>expire_time</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }
}

/// <summary>
/// 优惠券返还通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.UserCouponUnuse"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_user_coupon_unuse.html">优惠券返还通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.UserCouponUnuse })]
public sealed class ChannelsUserCouponUnusePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置优惠券返还用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_user_coupon_unuse</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>use_info</c>）。</summary>
    [JsonPropertyName("use_info")]
    public ChannelsUserCouponUnuseInfo? UseInfo { get; set; }
}

/// <summary>优惠券返还事件主体（官方 <c>use_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsUserCouponUnuseInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置用券的订单 ID（官方 <c>order_id</c>，官方类型为 string）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置返还时间（官方 <c>unuse_time</c>）。</summary>
    [JsonPropertyName("unuse_time")]
    public string? UnuseTime { get; set; }
}

/// <summary>
/// 优惠券核销通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.UserCouponUse"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方矛盾照录</b>：官方参数表 <c>Event</c> 行误写为 <c>channels_ec_user_coupon_unuse</c>
/// （复制粘贴错误），本载荷以页面标题「优惠券核销通知」、头部事件英文名与请求示例三者一致的
/// <c>channels_ec_user_coupon_use</c> 为准。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/coupon_callback/channels_ec_user_coupon_use.html">优惠券核销通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.UserCouponUse })]
public sealed class ChannelsUserCouponUsePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置优惠券所属用户的 OpenID（官方描述原文为「优惠券返还用户的OpenID」）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_user_coupon_use</c>；官方参数表误写 unuse，见类型 remarks）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>use_info</c>）。</summary>
    [JsonPropertyName("use_info")]
    public ChannelsUserCouponUseInfo? UseInfo { get; set; }
}

/// <summary>优惠券核销事件主体（官方 <c>use_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsUserCouponUseInfo
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置用券的订单 ID（官方 <c>order_id</c>，官方类型为 string）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>use_time</c>）。</summary>
    [JsonPropertyName("use_time")]
    public string? UseTime { get; set; }
}

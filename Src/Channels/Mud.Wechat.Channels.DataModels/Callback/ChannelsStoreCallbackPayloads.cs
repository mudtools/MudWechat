// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 店铺族（小店状态变更 / 最终注销 / 修改名称 / 修改头像，均为平铺字段形态）。
// 事件键：channels_ec_change_status、channels_ec_close_store、set_shop_nickname、set_shop_headimg。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 小店状态变更回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ShopChangeStatus"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 小店状态变化时通知商户（不包括开店完成和最终注销）：状态在 <c>open_finished</c> 与 <c>closing</c> 间扭转。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：<c>change_timestamp</c> 参数表为 number、示例为字符串 <c>"1750927919"</c>
/// （按参数表建模）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/storebasics_callback/channels_ec_basic_change_status.html">小店状态变更回调事件</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ShopChangeStatus })]
public sealed class ChannelsShopChangeStatusPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_change_status</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置变更发生的 Unix 时间戳，秒（官方 <c>change_timestamp</c>，参数表为 number、官方示例为字符串）。</summary>
    [JsonPropertyName("change_timestamp")]
    public long? ChangeTimestamp { get; set; }

    /// <summary>获取或设置扭转前的店铺状态（官方 <c>old_status</c>）。</summary>
    [JsonPropertyName("old_status")]
    public string? OldStatus { get; set; }

    /// <summary>获取或设置扭转后的店铺状态（官方 <c>new_status</c>）。</summary>
    [JsonPropertyName("new_status")]
    public string? NewStatus { get; set; }
}

/// <summary>
/// 小店最终注销回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ShopCloseStore"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 小店状态从 <c>closing</c> 扭转至 <c>close_finished</c> 时通知商户。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：<c>close_timestamp</c> 参数表为 number、示例为字符串 <c>"1662480000"</c>
/// （按参数表建模）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/storebasics_callback/channels_ec_basic_close_store.html">小店最终注销回调事件</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ShopCloseStore })]
public sealed class ChannelsShopCloseStorePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_close_store</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置注销发生的 Unix 时间戳，秒（官方 <c>close_timestamp</c>，参数表为 number、官方示例为字符串）。</summary>
    [JsonPropertyName("close_timestamp")]
    public long? CloseTimestamp { get; set; }
}

/// <summary>
/// 小店修改名称回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.SetShopNickname"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/storebasics_callback/set_shop_nickname.html">小店修改名称事件</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.SetShopNickname })]
public sealed class ChannelsShopNicknameChangePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>set_shop_nickname</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小店旧昵称（官方 <c>old_nickname</c>）。</summary>
    [JsonPropertyName("old_nickname")]
    public string? OldNickname { get; set; }

    /// <summary>获取或设置小店新昵称（官方 <c>new_nickname</c>）。</summary>
    [JsonPropertyName("new_nickname")]
    public string? NewNickname { get; set; }
}

/// <summary>
/// 小店修改头像回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.SetShopHeadimg"/>）。
/// </summary>
/// <remarks>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/storebasics_callback/set_headimg.html">小店修改头像事件</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.SetShopHeadimg })]
public sealed class ChannelsShopHeadimgChangePayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置用户的 OpenID。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>set_shop_headimg</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小店旧头像 URL（官方 <c>old_head_img_url</c>）。</summary>
    [JsonPropertyName("old_head_img_url")]
    public string? OldHeadImgUrl { get; set; }

    /// <summary>获取或设置小店新头像 URL（官方 <c>new_head_img_url</c>）。</summary>
    [JsonPropertyName("new_head_img_url")]
    public string? NewHeadImgUrl { get; set; }
}

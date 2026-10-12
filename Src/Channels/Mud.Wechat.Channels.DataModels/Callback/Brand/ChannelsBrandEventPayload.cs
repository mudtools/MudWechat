// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 品牌族（品牌资质事件通知）。
// 事件键：channels_ec_brand。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 品牌资质事件通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.BrandEvent"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 微信小店将品牌资质事件更新的消息通知给商户。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：<c>BrandEvent.status</c> 参数表为 string 枚举、示例为数字 <c>3</c>
/// （按参数表建模 string）；<c>brand_id</c> 参数表为 number(uint64)、示例为字符串
/// （按参数表建模）；官方示例另含参数表未列的 <c>audit_id</c> 字段（未建模，照录）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/brand_callback/brand_event.html">品牌资质事件通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.BrandEvent })]
public sealed class ChannelsBrandEventPayload
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

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_brand</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>BrandEvent</c> 对象，键名为官方 PascalCase 原文）。</summary>
    [JsonPropertyName("BrandEvent")]
    public ChannelsBrandEventInfo? BrandEvent { get; set; }
}

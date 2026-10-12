// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 资金族（结算账户变更 / 提现 / 提现二维码）。
// 事件键：channels_ec_acct_notify、channels_ec_withdraw_notify、qrcode_status。
// 注意：本族官方参数表与示例均无 FromUserName 字段（信封缺省，属性保留以兼容公共形态）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 结算账户变更回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.AcctNotify"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>信封差异</b>：官方参数表与示例均<b>无 <c>FromUserName</c></b> 字段（属性保留，缺省即 <c>null</c>）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/funds_callback/channels_ec_acct_notify.html">结算账户变更回调</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.AcctNotify })]
public sealed class ChannelsAcctNotifyPayload
{
    /// <summary>获取或设置小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置发送方 OpenID（官方参数表与示例均未携带本字段，缺省即 <c>null</c>）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>channels_ec_acct_notify</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>account_info</c>，AccountInfo 对象）。</summary>
    [JsonPropertyName("account_info")]
    public ChannelsAcctEventInfo? AccountInfo { get; set; }
}

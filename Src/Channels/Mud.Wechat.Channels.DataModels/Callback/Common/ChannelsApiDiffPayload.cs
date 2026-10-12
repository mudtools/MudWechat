// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 通用族（API 变更日志）。
// 事件键：api_diff。命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// API 变更日志回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ApiDiff"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 微信小店通过该回调向商户推送 API 变更日志消息，帮助商户及时掌握接口变动情况。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：页面头部「事件英文名」标注为 <c>ApiDiffEvent</c>，但参数表与示例的
/// <c>Event</c> 值均为 <c>api_diff</c>（按参数表取值）；<c>change_time</c> 参数表为 number、
/// 示例为字符串 <c>"1762492842"</c>（按参数表建模）。
/// </para>
/// <para>应答：回复 <c>success</c> 或空字符串（无需加密）。官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/common_callback/API_change_log_callback.html">API变更日志回调</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ApiDiff })]
public sealed class ChannelsApiDiffPayload
{
    /// <summary>获取或设置接收通知的微信小店 UserName。</summary>
    [JsonPropertyName("ToUserName")]
    public string? ToUserName { get; set; }

    /// <summary>获取或设置微信平台标识的 OpenID（固定值）。</summary>
    [JsonPropertyName("FromUserName")]
    public string? FromUserName { get; set; }

    /// <summary>获取或设置事件时间（Unix 时间戳，秒）。</summary>
    [JsonPropertyName("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置消息类型（固定为 <c>event</c>）。</summary>
    [JsonPropertyName("MsgType")]
    public string? MsgType { get; set; }

    /// <summary>获取或设置事件类型（固定为 <c>api_diff</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置 API 变更日志文档链接（官方 <c>changelog_url</c>）。</summary>
    [JsonPropertyName("changelog_url")]
    public string? ChangelogUrl { get; set; }

    /// <summary>获取或设置 API 变更详情数组（官方 <c>api_changes</c>，每个元素为一个 API 变更对象）。</summary>
    [JsonPropertyName("api_changes")]
    public List<ChannelsApiChangeItem>? ApiChanges { get; set; }

    /// <summary>获取或设置 API 变更发生时间（Unix 时间戳，秒；官方 <c>change_time</c>，参数表为 number、官方示例为字符串）。</summary>
    [JsonPropertyName("change_time")]
    public long? ChangeTime { get; set; }
}

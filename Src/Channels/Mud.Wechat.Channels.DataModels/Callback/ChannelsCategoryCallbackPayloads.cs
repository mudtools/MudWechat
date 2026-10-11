// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 类目族（类目审核结果通知）。
// 事件键：product_category_audit。命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 类目审核结果通知回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ProductCategoryAudit"/>）。
/// </summary>
/// <remarks>
/// <para>
/// 微信小店将类目审核结果消息通知给商户。应答：只需直接返回字符串 <c>success</c>。
/// </para>
/// <para>
/// <b>官方矛盾照录</b>：<c>ProductCategoryAudit.status</c> 参数表为 number、示例为字符串 <c>"1"</c>
/// （按参数表建模）。
/// </para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/category_callback/ProductCategoryAudit.html">类目审核结果通知</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ProductCategoryAudit })]
public sealed class ChannelsProductCategoryAuditPayload
{
    /// <summary>获取或设置小店名称。</summary>
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

    /// <summary>获取或设置事件类型（固定为 <c>product_category_audit</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>ProductCategoryAudit</c> 对象，键名为官方 PascalCase 原文）。</summary>
    [JsonPropertyName("ProductCategoryAudit")]
    public ChannelsProductCategoryAuditInfo? ProductCategoryAudit { get; set; }
}

/// <summary>类目审核事件主体（官方 <c>ProductCategoryAudit</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsProductCategoryAuditInfo
{
    /// <summary>获取或设置审核 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public string? AuditId { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>status</c>）：1 审核中；2 审核不通过；3 审核通过；12 主动取消申请单。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置相关信息（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

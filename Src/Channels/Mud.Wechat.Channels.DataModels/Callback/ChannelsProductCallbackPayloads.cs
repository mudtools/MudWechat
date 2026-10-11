// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 商品族（商品审核 / 上下架 / 更新，嵌套对象键为官方 PascalCase 原文）。
// 事件键：product_spu_audit、product_spu_listing、product_spu_update。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 商品审核回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ProductSpuAudit"/>）。
/// </summary>
/// <remarks>
/// <para>微信小店向商户发送商品审核完成的通知。应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/product_callback/ProductSpuAudit.html">商品审核</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ProductSpuAudit })]
public sealed class ChannelsProductSpuAuditPayload
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

    /// <summary>获取或设置事件类型（固定为 <c>product_spu_audit</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>ProductSpuAudit</c> 对象，键名为官方 PascalCase 原文）。</summary>
    [JsonPropertyName("ProductSpuAudit")]
    public ChannelsProductSpuAuditInfo? ProductSpuAudit { get; set; }
}

/// <summary>商品审核事件主体（官方 <c>ProductSpuAudit</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsProductSpuAuditInfo
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置状态（官方 <c>status</c>）：2 审核不通过；3 审核通过；4 撤销审核。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置审核原因，非必填字段（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// 获取或设置审核相关信息及提审时携带商家策略状态，非必填字段（官方 <c>audit_info</c>）。
    /// <para>官方示例中该字段为转义 JSON 字符串（照录，建模为 string）。</para>
    /// </summary>
    [JsonPropertyName("audit_info")]
    public string? AuditInfo { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>）：1 小店自营商品；4 在售赠品；5 非卖商品。</summary>
    [JsonPropertyName("product_type")]
    public long? ProductType { get; set; }
}

/// <summary>
/// 商品上下架回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ProductSpuListing"/>）。
/// </summary>
/// <remarks>
/// <para>微信小店将商品的上下架操作消息通知给商户。应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/product_callback/ProductSpuListing.html">商品上下架</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ProductSpuListing })]
public sealed class ChannelsProductSpuListingPayload
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

    /// <summary>获取或设置事件类型（固定为 <c>product_spu_listing</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>ProductSpuListing</c> 对象，键名为官方 PascalCase 原文）。</summary>
    [JsonPropertyName("ProductSpuListing")]
    public ChannelsProductSpuListingInfo? ProductSpuListing { get; set; }
}

/// <summary>商品上下架事件主体（官方 <c>ProductSpuListing</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsProductSpuListingInfo
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置状态（官方 <c>status</c>）：5 上架；11 自主下架；13 系统下架；14 保证金违规下架；15 品牌到期下架；20 封禁下架。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置下架原因，非必填字段（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>）：1 小店自营商品；4 在售赠品；5 非卖商品。</summary>
    [JsonPropertyName("product_type")]
    public long? ProductType { get; set; }
}

/// <summary>
/// 商品更新回调载荷（事件键 <see cref="ChannelsCallbackEventTypes.ProductSpuUpdate"/>）。
/// </summary>
/// <remarks>
/// <para>微信小店将更新商品信息数据操作消息通知给商户。应答：回复 <c>success</c> 或空字符串（无需加密）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/notify/product_callback/ProductSpuUpdate.html">商品更新</see>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
[ChannelsCallbackContract(EventTypes = new[] { ChannelsCallbackEventTypes.ProductSpuUpdate })]
public sealed class ChannelsProductSpuUpdatePayload
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

    /// <summary>获取或设置事件类型（固定为 <c>product_spu_update</c>）。</summary>
    [JsonPropertyName("Event")]
    public string? Event { get; set; }

    /// <summary>获取或设置事件主体（官方 <c>ProductSpuUpdate</c> 对象，键名为官方 PascalCase 原文）。</summary>
    [JsonPropertyName("ProductSpuUpdate")]
    public ChannelsProductSpuUpdateInfo? ProductSpuUpdate { get; set; }
}

/// <summary>商品更新事件主体（官方 <c>ProductSpuUpdate</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsProductSpuUpdateInfo
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置操作（官方 <c>status</c>）：0 更新；1 新增；2 删除；3 彻底删除。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>）：1 小店自营商品；4 在售赠品；5 非卖商品。</summary>
    [JsonPropertyName("product_type")]
    public long? ProductType { get; set; }
}

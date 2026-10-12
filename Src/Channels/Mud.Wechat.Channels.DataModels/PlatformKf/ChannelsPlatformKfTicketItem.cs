// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>
/// 商家工单条目（官方 <c>ticket_item_list</c> 数组元素，<c>getmchticketlist</c>）——
/// 官方契约「getmchticketdetail 的 merchant_ticket 结构同工单列表元素」⇒ 共用同一 DTO。
/// </summary>
/// <remarks>时间字段均为 Unix 秒级时间戳。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketItem
{
    /// <summary>获取或设置商家工单 id（官方 <c>ticket_id</c>）。</summary>
    [JsonPropertyName("ticket_id")]
    public string? TicketId { get; set; }

    /// <summary>获取或设置工单分类类型（官方 <c>ticket_class_type</c>，见 <see cref="ChannelsPlatformKfTicketClassTypes"/>）。</summary>
    [JsonPropertyName("ticket_class_type")]
    public int? TicketClassType { get; set; }

    /// <summary>获取或设置工单状态（官方 <c>ticket_state</c>，见 <see cref="ChannelsPlatformKfTicketStates"/>）。</summary>
    [JsonPropertyName("ticket_state")]
    public int? TicketState { get; set; }

    /// <summary>获取或设置工单开始时间（官方 <c>ticket_begin_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("ticket_begin_time")]
    public long? TicketBeginTime { get; set; }

    /// <summary>获取或设置工单处理截止时间（官方 <c>ticket_deal_end_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("ticket_deal_end_time")]
    public long? TicketDealEndTime { get; set; }

    /// <summary>获取或设置商户处理时间（官方 <c>merchant_operate_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("merchant_operate_time")]
    public long? MerchantOperateTime { get; set; }

    /// <summary>获取或设置平台处理时间（官方 <c>platform_operate_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("platform_operate_time")]
    public long? PlatformOperateTime { get; set; }

    /// <summary>获取或设置工单问题描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置关联订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置关联售后 id（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }

    /// <summary>获取或设置平台审核结果（官方 <c>platform_review_result</c>，见 <see cref="ChannelsPlatformKfReviewResults"/>）。</summary>
    [JsonPropertyName("platform_review_result")]
    public int? PlatformReviewResult { get; set; }

    /// <summary>获取或设置订单信息（官方 <c>order_info</c>）。</summary>
    [JsonPropertyName("order_info")]
    public ChannelsPlatformKfTicketOrderInfo? OrderInfo { get; set; }

    /// <summary>获取或设置用户信息（官方 <c>user_info</c>）。</summary>
    [JsonPropertyName("user_info")]
    public ChannelsPlatformKfTicketUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置商户处理记录（官方 <c>merchant_operate</c>）。</summary>
    [JsonPropertyName("merchant_operate")]
    public ChannelsPlatformKfTicketMerchantOperate? MerchantOperate { get; set; }

    /// <summary>获取或设置平台处理记录（官方 <c>platform_operate</c>）。</summary>
    [JsonPropertyName("platform_operate")]
    public ChannelsPlatformKfTicketPlatformOperate? PlatformOperate { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}

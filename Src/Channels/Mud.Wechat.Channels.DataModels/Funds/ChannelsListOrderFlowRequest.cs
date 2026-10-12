// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>查询订单流水列表（<c>listorderflow</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>order_settle_state</c> 必填；分页形态为 <c>pagination_info</c>（支持分页上下文，
/// 可保证翻页过程订单不遗漏不重复）。所有子项目的结算状态均与 <c>order_settle_state</c> 相同（官方注意事项原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsListOrderFlowRequest
{
    /// <summary>获取或设置订单结算状态（官方 <c>order_settle_state</c>，必填，见 <see cref="ChannelsOrderSettleStates"/>）。</summary>
    [JsonPropertyName("order_settle_state")]
    public int OrderSettleState { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>order_state</c>，选填，见 <see cref="ChannelsOrderFlowOrderStates"/>）。</summary>
    [JsonPropertyName("order_state")]
    public int? OrderState { get; set; }

    /// <summary>获取或设置订单支付方式（官方 <c>order_pay_method</c>，选填，见 <see cref="ChannelsOrderFlowPayMethods"/>）。</summary>
    [JsonPropertyName("order_pay_method")]
    public int? OrderPayMethod { get; set; }

    /// <summary>获取或设置指定订单 id 查询（官方 <c>order_id</c>，选填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置分页信息（官方 <c>pagination_info</c>，选填）。</summary>
    [JsonPropertyName("pagination_info")]
    public ChannelsOrderFlowPagination? PaginationInfo { get; set; }

    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>，选填）。</summary>
    /// <remarks><b>官方文档矛盾照录</b>：参数表字段名为 <c>create_time_range</c>，代码示例写作
    /// <c>order_create_time_range</c>——SDK 按参数表建模。</remarks>
    [JsonPropertyName("create_time_range")]
    public ChannelsOrderFlowTimeRange? CreateTimeRange { get; set; }
}

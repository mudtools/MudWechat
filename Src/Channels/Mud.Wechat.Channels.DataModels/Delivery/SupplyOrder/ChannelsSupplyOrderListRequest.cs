// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取供货单列表（<c>supplyorder/list</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；时间范围至少填一个且起止距离不可超过 7 天。</para>
/// <para>
/// <b>官方变更日志照录</b>：2026-05-25 起 <c>shop_appid</c> 字段已删除、新增 <c>supplier_appid</c>——
/// SDK 按变更后参数表建模，不登记 <c>shop_appid</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplyOrderListRequest
{
    /// <summary>获取或设置供货单创建时间范围（官方 <c>create_time_range</c>，选填，时间范围至少填一个）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置供货单状态（官方 <c>status</c>，选填；官方页声明「具体枚举值请参考下文」但未列出具体值——照录）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可通过订单号反查供货单）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置供货单更新时间范围（官方 <c>update_time_range</c>，选填）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>拉取代发单列表（<c>dropship/list</c>）请求体。</summary>
/// <remarks>
/// <para>官方契约：<b>POST</b> + JSON 请求体；<c>create_time_range</c>/<c>update_time_range</c> 起止距离不可超过 7 天。</para>
/// <para>
/// <b>官方文档矛盾照录</b>：参数表 <c>order_id</c> 类型为 number，请求示例为字符串
/// <c>"37000204335323251"</c>——SDK 按参数表建模（long）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipListRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，选填，可根据订单号反查代发单；官方参数表类型为 number，示例为字符串——照录）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，选填，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，选填，上一页请求返回）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，选填，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}

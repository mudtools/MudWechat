// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>代发单结构（官方 <c>dropship_order</c> 对象，<c>dropship/get</c>）。</summary>
/// <remarks>
/// <b>官方形态照录</b>：<c>ds_order_id</c>/<c>order_id</c> 在响应侧官方类型为 number，
/// 而请求侧同名参数（<c>dropship/get</c> 的 <c>ds_order_id</c>）为 string——按官方各侧参数表分别建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDropshipOrder
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，官方响应类型为 number）。</summary>
    [JsonPropertyName("ds_order_id")]
    public long? DsOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>，见 <see cref="ChannelsDropshipOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置供货商 appid（官方 <c>supplier_appid</c>）。</summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置供货商名称（官方 <c>supplier_name</c>）。</summary>
    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>，官方响应类型为 number）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置代发单详情（官方 <c>dropship_order_detail</c>）。</summary>
    [JsonPropertyName("dropship_order_detail")]
    public ChannelsDropshipOrderDetail? DropshipOrderDetail { get; set; }
}

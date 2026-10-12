// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>按商品自动分配的商品条目（官方 <c>list</c> 数组元素，<c>get_product_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsProductDistributeStatus
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置分配状态（官方 <c>distribute_status</c>，见 <see cref="ChannelsProductDistributeStatuses"/>）。</summary>
    [JsonPropertyName("distribute_status")]
    public int? DistributeStatus { get; set; }

    /// <summary>
    /// 获取或设置供货商 appid（官方 <c>supplier_appid</c>）——
    /// 当且仅当 <c>distribute_status = 0</c>（已分配）时有意义。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>
    /// 获取或设置供货商名称（官方 <c>supplier_name</c>）——
    /// 当且仅当 <c>distribute_status = 0</c>（已分配）时有意义。
    /// </summary>
    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }
}

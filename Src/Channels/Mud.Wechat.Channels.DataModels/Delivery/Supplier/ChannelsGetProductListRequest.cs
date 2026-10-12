// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取按商品自动分配的商品列表（<c>get_product_list</c>）请求体。</summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>is_filter_product_status</c> / <c>product_status</c>
/// 两字段，但参数表未列出——SDK 按参数表建模，不登记表外字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetProductListRequest
{
    /// <summary>获取或设置是否过滤 <c>distribute_status</c>（官方 <c>is_filter_distribute_status</c>，选填）。</summary>
    [JsonPropertyName("is_filter_distribute_status")]
    public bool? IsFilterDistributeStatus { get; set; }

    /// <summary>
    /// 获取或设置分配状态（官方 <c>distribute_status</c>，选填，见 <see cref="ChannelsProductDistributeStatuses"/>）——
    /// 当且仅当 <c>is_filter_distribute_status = true</c> 时生效。
    /// </summary>
    [JsonPropertyName("distribute_status")]
    public int? DistributeStatus { get; set; }

    /// <summary>获取或设置是否过滤 <c>supplier_appid</c>（官方 <c>is_filter_supplier_appid</c>，选填）。</summary>
    [JsonPropertyName("is_filter_supplier_appid")]
    public bool? IsFilterSupplierAppid { get; set; }

    /// <summary>
    /// 获取或设置供货商 appid（官方 <c>supplier_appid</c>，选填）——
    /// 当且仅当 <c>is_filter_supplier_appid = true</c> 时有效。
    /// </summary>
    [JsonPropertyName("supplier_appid")]
    public string? SupplierAppid { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>page_context</c>，选填，分页用）——首次请求传空串，翻页回传上一页返回值，直到返回空串结束。</summary>
    [JsonPropertyName("page_context")]
    public string? PageContext { get; set; }
}

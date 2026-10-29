// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 代发单结构（官方 <c>dropship_order</c>，<b>供货商视角</b>，<c>dropship/supplier/get</c>）。
/// </summary>
/// <remarks>
/// 与商家侧 <see cref="ChannelsDropshipOrder"/> 的官方参数表存在两处不可合并的差异：
/// ①本视角标识商家的 <c>shop_appid</c>/<c>shop_name</c>（商家侧为 <c>supplier_appid</c>/<c>supplier_name</c>）；
/// ②本视角 <c>ds_order_id</c>/<c>order_id</c> 官方类型为 <b>string</b>（商家侧参数表为 number）——照官方原文各自建模，禁止「对齐」互改。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipOrder
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，本视角为 string）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string? DsOrderId { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>：100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置商家 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置商家名称（官方 <c>shop_name</c>）。</summary>
    [JsonPropertyName("shop_name")]
    public string? ShopName { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>，本视角为 string）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置代发单详情（官方 <c>dropship_order_detail</c>）。</summary>
    [JsonPropertyName("dropship_order_detail")]
    public ChannelsSupplierDropshipOrderDetail? DropshipOrderDetail { get; set; }
}

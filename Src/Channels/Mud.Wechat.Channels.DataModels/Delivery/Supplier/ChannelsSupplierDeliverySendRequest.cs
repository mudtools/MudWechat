// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 代发单 / 供货单发货（<c>dropship/supplier/delivery/send</c>）请求。
/// </summary>
/// <remarks>
/// 官方参数表把 <c>ds_order_id</c> 与 <c>supply_order_id</c> <b>同时标为必填</b>，而说明为「与代发单号 / 供货单号选传其一」
/// ——照录该矛盾，两字段均建模为可空、SDK 不做「必须恰好一个」的本地拦截，非法组合由官方错误码 <c>40097</c> 表达。
/// 代发单的发货参数是商品信息，供货单的发货参数是货品信息（官方同一字段名，语义随单据类型而变）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDeliverySendRequest
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，与 <c>supply_order_id</c> 选传其一）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string? DsOrderId { get; set; }

    /// <summary>获取或设置供货单号（官方 <c>supply_order_id</c>，与 <c>ds_order_id</c> 选传其一；2026-05-18 官方新增）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string? SupplyOrderId { get; set; }

    /// <summary>获取或设置物流信息列表（官方 <c>delivery_list</c>，必填）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsSupplierDeliveryPackage> DeliveryList { get; set; } = new();
}

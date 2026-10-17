// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 修改代发单 / 供货单物流（<c>dropship/supplier/delivery/update</c>）请求。
/// </summary>
/// <remarks>
/// <c>ds_order_id</c> / <c>supply_order_id</c> 官方为选传其一（参数表同标必填，照录矛盾、不做本地拦截）。
/// 官方限制：只允许修改运单号，商品信息必须保持一致，且修改次数有限；只支持修改为通过微信小店电子面单取号的运单号，
/// 不支持修改为外部获取的运单号。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDeliveryUpdateRequest
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，与 <c>supply_order_id</c> 选传其一）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string? DsOrderId { get; set; }

    /// <summary>获取或设置供货单号（官方 <c>supply_order_id</c>，与 <c>ds_order_id</c> 选传其一；2026-05-18 官方新增）。</summary>
    [JsonPropertyName("supply_order_id")]
    public string? SupplyOrderId { get; set; }

    /// <summary>获取或设置物流修改信息列表（官方 <c>change_infos</c>，必填）。</summary>
    [JsonPropertyName("change_infos")]
    public List<ChannelsSupplierDeliveryChangeInfo> ChangeInfos { get; set; } = new();
}

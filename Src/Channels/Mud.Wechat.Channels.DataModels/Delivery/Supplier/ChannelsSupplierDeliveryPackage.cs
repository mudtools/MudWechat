// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 物流包裹信息（官方 <c>delivery_list[]</c> 数组元素，亦作 <c>change_infos[].old</c> / <c>change_infos[].new</c>）。
/// </summary>
/// <remarks>
/// 与商家侧 <c>order/delivery/send</c> 的 <c>delivery_list</c> 元素差异：本视角<b>无</b> <c>deliver_type</c>/
/// <c>course_info</c>/<c>sn_info</c> 字段（官方参数表即无，非省略）⇒ 分建 DTO，不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDeliveryPackage
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，必填，通过「获取快递公司列表」接口获得）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹中的商品 / 货品信息列表（官方 <c>product_infos</c>，必填）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsSupplierDeliveryProductInfo> ProductInfos { get; set; } = new();
}

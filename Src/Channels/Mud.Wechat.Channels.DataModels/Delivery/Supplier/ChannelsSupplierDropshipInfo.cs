// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 代发相关信息（官方 <c>product_infos[].dropship_info</c>，供货商视角，<c>dropship/supplier/get</c>）。
/// </summary>
/// <remarks>
/// 本页参数表 <c>ds_order_id</c> 为 <b>string</b>，商家侧 <see cref="ChannelsDropshipInfo"/> 参数表为 number ⇒ 两视角各建一型，
/// 其余字段官方口径一致（含 <c>preshipment_change_sku_state</c> 官方只列 <c>3</c> 一个取值，照录不做本地拦截）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipInfo
{
    /// <summary>获取或设置代发单号（官方 <c>ds_order_id</c>，本视角为 string）。</summary>
    [JsonPropertyName("ds_order_id")]
    public string? DsOrderId { get; set; }

    /// <summary>获取或设置发货状态（官方 <c>delivery_status</c>：10 待发货 / 19 部分发货 / 20 已发货 / 30 已取消）。</summary>
    [JsonPropertyName("delivery_status")]
    public int? DeliveryStatus { get; set; }

    /// <summary>获取或设置售后状态（官方 <c>aftersale_status</c>：0 无售后/售后被关闭 / 1 商品售后中，详情需联系商家 / 2 商品已售后完成）。</summary>
    [JsonPropertyName("aftersale_status")]
    public int? AftersaleStatus { get; set; }

    /// <summary>获取或设置取消时间（官方 <c>cancel_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("cancel_time")]
    public long? CancelTime { get; set; }

    /// <summary>获取或设置取消原因（官方 <c>cancel_scene</c>：1 售后 / 2 商家取消代发）。</summary>
    [JsonPropertyName("cancel_scene")]
    public int? CancelScene { get; set; }

    /// <summary>获取或设置换货申请状态（官方 <c>preshipment_change_sku_state</c>：3 等待商家处理）。</summary>
    [JsonPropertyName("preshipment_change_sku_state")]
    public int? PreshipmentChangeSkuState { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>sku 预售信息（官方 <c>skus[].sku_deliver_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterSkuDeliverInfo
{
    /// <summary>获取或设置 sku 库存情况（官方 <c>stock_type</c>，0：现货（默认）1：全款预售；部分类目支持全款预售，参考获取类目信息的 <c>attr.pre_sale</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>获取或设置 sku 发货节点（官方 <c>full_payment_presale_delivery_type</c>，仅 <c>stock_type=1</c> 有效；0：付款后 n 天发货 1：预售结束后 n 天发货）。</summary>
    [JsonPropertyName("full_payment_presale_delivery_type")]
    public int? FullPaymentPresaleDeliveryType { get; set; }

    /// <summary>获取或设置 sku 预售周期开始时间（官方 <c>presale_begin_time</c>，秒级时间戳，仅 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("presale_begin_time")]
    public long? PresaleBeginTime { get; set; }

    /// <summary>获取或设置 sku 预售周期结束时间（官方 <c>presale_end_time</c>，秒级时间戳，仅 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("presale_end_time")]
    public long? PresaleEndTime { get; set; }

    /// <summary>获取或设置 sku 发货时效（官方 <c>full_payment_presale_delivery_time</c>，即付款后/预售结束后该天数内发货，仅 <c>stock_type=1</c> 有效）。</summary>
    [JsonPropertyName("full_payment_presale_delivery_time")]
    public int? FullPaymentPresaleDeliveryTime { get; set; }
}

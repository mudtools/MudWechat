// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// SKU 预售信息（官方 <c>sku_deliver_info</c> 对象）。
/// </summary>
/// <remarks>
/// <c>stock_type = 1</c>（全款预售）时其余字段生效；预售时间区间 ≤ 15 天、
/// 预售结束时间距当前 ≤ 30 天（官方限制，见「更新商品免审」文档）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsSkuDeliverInfo
{
    /// <summary>获取或设置 sku 库存情况（官方 <c>stock_type</c>：0 现货（默认），1 全款预售；部分类目支持，参考类目信息 <c>attr.pre_sale</c>）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>
    /// 获取或设置 sku 发货节点（官方 <c>full_payment_presale_delivery_type</c>，仅对 <c>stock_type=1</c> 有效）：
    /// 0 付款后 n 天发货，1 预售结束后 n 天发货。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_type")]
    public int? FullPaymentPresaleDeliveryType { get; set; }

    /// <summary>获取或设置预售周期开始时间（官方 <c>presale_begin_time</c>，秒级时间戳；仅对 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("presale_begin_time")]
    public long? PresaleBeginTime { get; set; }

    /// <summary>获取或设置预售周期结束时间（官方 <c>presale_end_time</c>，秒级时间戳；距现在 ≤ 30 天，预售区间 ≤ 15 天）。</summary>
    [JsonPropertyName("presale_end_time")]
    public long? PresaleEndTime { get; set; }

    /// <summary>
    /// 获取或设置 sku 发货时效（官方 <c>full_payment_presale_delivery_time</c>，仅对 <c>stock_type=1</c> 有效）：
    /// 发货节点 0 时范围 [4, 15]；发货节点 1 时范围 [1, 3]。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_time")]
    public int? FullPaymentPresaleDeliveryTime { get; set; }

    /// <summary>获取或设置是否在预售结束后自动转为现货（官方 <c>spot_after_presale_end</c>：0 否，1 是；仅对 <c>delivery_type=1</c> 有效）。</summary>
    [JsonPropertyName("spot_after_presale_end")]
    public int? SpotAfterPresaleEnd { get; set; }
}

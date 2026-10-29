// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>佣金单基础信息（官方 <c>base_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentOrderBaseInfo
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商品 spu id（官方 <c>spu_id</c>）。</summary>
    [JsonPropertyName("spu_id")]
    public string? SpuId { get; set; }

    /// <summary>获取或设置商品 sku id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置特殊 id（官方 <c>special_id</c>，针对本地生活）。</summary>
    [JsonPropertyName("special_id")]
    public string? SpecialId { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>order_status</c>，见 <see cref="ChannelsTalentOrderStatuses"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>获取或设置实际支付金额（官方 <c>actual_payment</c>，参数表标 string、单位分；官方示例值 <c>"99.99"</c> 为元 ⇒ 矛盾照录，SDK 原样透传不换算）。</summary>
    [JsonPropertyName("actual_payment")]
    public string? ActualPayment { get; set; }

    /// <summary>获取或设置订单创建时间戳（官方 <c>order_create_time</c>）。</summary>
    [JsonPropertyName("order_create_time")]
    public long? OrderCreateTime { get; set; }

    /// <summary>获取或设置订单更新时间戳（官方 <c>order_update_time</c>）。</summary>
    [JsonPropertyName("order_update_time")]
    public long? OrderUpdateTime { get; set; }

    /// <summary>获取或设置订单支付时间戳（官方 <c>order_pay_time</c>，2025-11-17 变更日志新增）。</summary>
    [JsonPropertyName("order_pay_time")]
    public long? OrderPayTime { get; set; }

    /// <summary>获取或设置订单分佣基数（官方 <c>settle_payment</c>，单位分，2026-01-15 变更日志新增，用于外部对齐佣金单结算基数）。</summary>
    [JsonPropertyName("settle_payment")]
    public string? SettlePayment { get; set; }

    /// <summary>获取或设置下单用户信息（官方 <c>buyer_info</c>）。</summary>
    [JsonPropertyName("buyer_info")]
    public ChannelsTalentOrderBuyerInfo? BuyerInfo { get; set; }
}

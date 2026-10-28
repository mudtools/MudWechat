// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 小程序支付订单（<c>/shop/pay/getorder</c> 回包的 <c>order</c> 一层）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayOrder
{
    /// <summary>商家交易单号。</summary>
    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }

    /// <summary>商家合单支付总交易单号。</summary>
    [JsonPropertyName("combine_trade_no")]
    public string? CombineTradeNo { get; set; }

    /// <summary>商户号（敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>微信支付侧交易单号（敏感，不得进日志）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单创建时间，秒级时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>订单更新时间，秒级时间戳。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>订单支付时间，秒级时间戳。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }

    /// <summary>订单过期时间，秒级时间戳。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>订单关闭状态：0 正常、1 已关闭。</summary>
    [JsonPropertyName("close_status")]
    public int? CloseStatus { get; set; }

    /// <summary>订单关闭时间，秒级时间戳。</summary>
    [JsonPropertyName("close_time")]
    public long? CloseTime { get; set; }

    /// <summary>订单金额，<b>单位分</b>。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>订单描述。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>分账信息列表。</summary>
    [JsonPropertyName("profit_sharing_list")]
    public List<WxaShopPayProfitSharingRecord>? ProfitSharingList { get; set; }

    /// <summary>退款信息列表。</summary>
    [JsonPropertyName("refund_list")]
    public List<WxaShopPayOrderRefundRecord>? RefundList { get; set; }

    /// <summary>账期：订单由支付时间开始多少天后可以分账。</summary>
    [JsonPropertyName("profit_sharing_delay")]
    public int? ProfitSharingDelay { get; set; }

    /// <summary>订单当前是否被冻结分账：1 不能分账、0 可正常分账。</summary>
    [JsonPropertyName("profit_sharing_frozen")]
    public int? ProfitSharingFrozen { get; set; }
}

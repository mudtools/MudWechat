// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>券账单条目（官方 <c>bill_list</c> 数组元素，getvoucherbilllist 响应）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；次卡相关金额为单次金额；时间字段为秒级时间戳。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherBill
{
    /// <summary>获取或设置商品名称（官方 <c>voucher_name</c>）。</summary>
    [JsonPropertyName("voucher_name")]
    public string? VoucherName { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置券 code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置购买券的订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置用户在商家品牌 appid 下的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置购买时间（官方 <c>voucher_buy_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("voucher_buy_time")]
    public long? VoucherBuyTime { get; set; }

    /// <summary>获取或设置券售卖金额（官方 <c>voucher_buy_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("voucher_buy_amount")]
    public long? VoucherBuyAmount { get; set; }

    /// <summary>获取或设置用户支付金额（官方 <c>voucher_pay_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("voucher_pay_amount")]
    public long? VoucherPayAmount { get; set; }

    /// <summary>获取或设置视频号小店商家优惠减价金额（官方 <c>mch_favor_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("mch_favor_amount")]
    public long? MchFavorAmount { get; set; }

    /// <summary>获取或设置视频号小店平台优惠减价金额（官方 <c>platform_favor_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("platform_favor_amount")]
    public long? PlatformFavorAmount { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>consume_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("consume_time")]
    public long? ConsumeTime { get; set; }

    /// <summary>获取或设置核销的门店名称（官方 <c>consume_store_name</c>；商户在核销券接口传入后才会返回）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置给商家的已结算金额（官方 <c>mch_settle_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("mch_settle_amount")]
    public long? MchSettleAmount { get; set; }

    /// <summary>获取或设置给达人的结算金额（官方 <c>finder_settle_amount</c>，单位：分；次卡时为单次金额）。</summary>
    [JsonPropertyName("finder_settle_amount")]
    public long? FinderSettleAmount { get; set; }

    /// <summary>获取或设置核销接口传入的商家自定义门店 id（官方 <c>out_store_id</c>）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>获取或设置次序号标识（官方 <c>time_index</c>；2024 年 2 月底次卡上线后返回）。</summary>
    [JsonPropertyName("time_index")]
    public long? TimeIndex { get; set; }

    /// <summary>获取或设置退款时间（官方 <c>refund_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long? RefundTime { get; set; }

    /// <summary>获取或设置商家结算时间（官方 <c>mch_settle_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("mch_settle_time")]
    public long? MchSettleTime { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>获取或设置退款账户（官方 <c>refund_account</c>，见 <see cref="ChannelsVoucherRefundAccounts"/>）。</summary>
    [JsonPropertyName("refund_account")]
    public int? RefundAccount { get; set; }

    /// <summary>获取或设置退款关联的售后单 id（官方 <c>after_sale_id</c>）。</summary>
    [JsonPropertyName("after_sale_id")]
    public string? AfterSaleId { get; set; }

    /// <summary>获取或设置平台技术服务费结算时间（官方 <c>commission_fee_settle_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("commission_fee_settle_time")]
    public long? CommissionFeeSettleTime { get; set; }

    /// <summary>获取或设置平台技术服务费（官方 <c>commission_fee</c>，单位：分）。</summary>
    [JsonPropertyName("commission_fee")]
    public long? CommissionFee { get; set; }
}

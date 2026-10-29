// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 收款订单详情（<c>pay_order</c>，<c>/cgi-bin/paytool/get_order_detail</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolOrderDetail
{
    /// <summary>获取或设置订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置订单创建时间（unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置客户企业的 corpid。</summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpid { get; set; }

    /// <summary>获取或设置购买内容。</summary>
    [JsonPropertyName("buy_content")]
    public string? BuyContent { get; set; }

    /// <summary>获取或设置原价金额（单位分）。</summary>
    [JsonPropertyName("origin_price")]
    public long? OriginPrice { get; set; }

    /// <summary>
    /// 获取或设置实付金额（单位分）。
    /// <para>官方口径：免支付订单实付金额返回 0。</para>
    /// </summary>
    [JsonPropertyName("paid_price")]
    public long? PaidPrice { get; set; }

    /// <summary>
    /// 获取或设置订单状态：<c>1</c>-待支付 / <c>2</c>-已支付 / <c>3</c>-订单取消 / <c>4</c>-支付过期 /
    /// <c>5</c>-退款申请中 / <c>6</c>-已退款 / <c>7</c>-交易完成 / <c>8</c>-待企业确认 / <c>9</c>-已部分退款。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>获取或设置订单来源：<c>1</c>-客户下单 / <c>2</c>-服务商创建。</summary>
    [JsonPropertyName("order_from")]
    public int? OrderFrom { get; set; }

    /// <summary>获取或设置订单创建人。</summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置支付方式：<c>0</c>-客户支付 / <c>1</c>-服务商代支付 / <c>2</c>-免支付。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方契约陷阱</b>：官方返回参数表同时列出 <c>pay_order.pay_from</c> 与 <c>pay_order.pay_type</c>，
    /// 而返回示例只用 <c>pay_type</c>。本 SDK 照官方返回示例承载 <c>pay_type</c>。
    /// </para>
    /// </remarks>
    [JsonPropertyName("pay_type")]
    public int? PayType { get; set; }

    /// <summary>获取或设置客户企业简称。</summary>
    [JsonPropertyName("custom_corp_name")]
    public string? CustomCorpName { get; set; }

    /// <summary>
    /// 获取或设置付款方式：<c>1</c>-微信支付 / <c>2</c>-网银支付；如果未支付该字段为空。
    /// </summary>
    [JsonPropertyName("pay_channel")]
    public int? PayChannel { get; set; }

    /// <summary>获取或设置付款流水号；如果未支付该字段为空。</summary>
    [JsonPropertyName("channel_order_id")]
    public string? ChannelOrderId { get; set; }

    /// <summary>获取或设置付款时间（unix 时间戳）；如果未支付该字段为空。</summary>
    [JsonPropertyName("paid_time")]
    public long? PaidTime { get; set; }

    /// <summary>
    /// 获取或设置业务类型：<c>1</c>-普通第三方应用 / <c>2</c>-代开发应用 / <c>3</c>-行业解决方案。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置收入到账商户号：<c>1</c>-微信支付商户号 / <c>2</c>-财付通商户号；
    /// 如果收入未到账该字段为空。
    /// </summary>
    [JsonPropertyName("income_type")]
    public int? IncomeType { get; set; }

    /// <summary>获取或设置到账时间（unix 时间戳）；如果收入未到账该字段为空。</summary>
    [JsonPropertyName("income_time")]
    public long? IncomeTime { get; set; }

    /// <summary>获取或设置到账金额（单位分）；如果收入未到账该字段为空。</summary>
    [JsonPropertyName("income_amount")]
    public long? IncomeAmount { get; set; }

    /// <summary>
    /// 获取或设置购买明细（不同业务类型的明细不一样，见 <see cref="PayToolProductList"/>）。
    /// </summary>
    [JsonPropertyName("product_list")]
    public PayToolProductList? ProductList { get; set; }
}

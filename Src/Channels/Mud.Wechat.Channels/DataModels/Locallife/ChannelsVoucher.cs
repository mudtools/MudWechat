// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>
/// 用户团购券详情（官方 <c>voucher</c> 对象；getvoucher / findvoucher(get_list) / consumevoucher /
/// withdrawconsume(revoke) 四端点响应的券对象官方示例字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录（示例超集）</b>：getvoucher 参数表仅登记 <c>code</c>/<c>status</c>/<c>send_time</c>/
/// <c>update_time</c>/<c>start_time</c>/<c>end_time</c> 6 字段，但官方示例另含
/// <c>consume_time</c>/<c>refund_time</c>/<c>product_id</c>/<c>consume_store_name</c>/<c>voucher_type</c>/
/// <c>voucher_buy_amount</c>/<c>voucher_actual_amount</c>/<c>telphone_no</c>/<c>openid</c>/<c>code_type</c>/
/// <c>order_id</c>/<c>sku_id</c>（findvoucher 与核销/撤销响应示例同）——按参数表类型口径建模 6 字段，
/// 示例扩展字段照录进 DTO（反序列化容错，缺失不抛错）。
/// </para>
/// <para>金额字段单位为<b>分</b>；时间字段为秒级时间戳。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucher
{
    /// <summary>获取或设置券 code（官方 <c>code</c>；平台生成的 code 是数字加字母组合、最长 32 位，商家自定义 code 不超过 48 位）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置券状态（官方 <c>status</c>，见 <see cref="ChannelsVoucherStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置发放时间（官方 <c>send_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("send_time")]
    public long? SendTime { get; set; }

    /// <summary>获取或设置最近更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置核销生效时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置核销结束时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置核销时间（官方 <c>consume_time</c>，秒级时间戳；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("consume_time")]
    public long? ConsumeTime { get; set; }

    /// <summary>获取或设置退款时间（官方 <c>refund_time</c>，秒级时间戳；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("refund_time")]
    public long? RefundTime { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置核销门店名称（官方 <c>consume_store_name</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置券类型（官方 <c>voucher_type</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_type")]
    public int? VoucherType { get; set; }

    /// <summary>获取或设置券售卖金额（官方 <c>voucher_buy_amount</c>，单位：分；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_buy_amount")]
    public long? VoucherBuyAmount { get; set; }

    /// <summary>获取或设置券实付金额（官方 <c>voucher_actual_amount</c>，单位：分；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("voucher_actual_amount")]
    public long? VoucherActualAmount { get; set; }

    /// <summary>获取或设置用户手机号（官方 <c>telphone_no</c>——官方历史拼写照抄；官方示例字段，参数表未登记）。</summary>
    [JsonPropertyName("telphone_no")]
    public string? TelphoneNo { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置 code 类型（官方 <c>code_type</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("code_type")]
    public int? CodeType { get; set; }

    /// <summary>获取或设置购买券的订单 ID（官方 <c>order_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>；官方示例字段，参数表未登记——照录）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>union_id</c>；仅 getvoucher 官方示例出现——照录）。</summary>
    [JsonPropertyName("union_id")]
    public string? UnionId { get; set; }
}

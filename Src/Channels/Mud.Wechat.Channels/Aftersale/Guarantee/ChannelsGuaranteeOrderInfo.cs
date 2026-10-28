// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>保障单结构（官方 <c>guarantee_order</c> 对象，<c>getguaranteeorder</c> 响应主载荷）。</summary>
/// <remarks>
/// 保障单类型见 <see cref="ChannelsGuaranteeTypes"/>、状态枚举见 <see cref="ChannelsGuaranteeStatuses"/>。
/// 嵌套对象内部字段以官方保障单文档为准（<c>fake_one_pay_four_info</c> 假一赔三详情 / <c>bad_pay_info</c> 坏损包退详情）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeOrderInfo
{
    /// <summary>获取或设置保障单号（官方 <c>guarantee_order_id</c>）。</summary>
    [JsonPropertyName("guarantee_order_id")]
    public long? GuaranteeOrderId { get; set; }

    /// <summary>获取或设置保障单类型（官方 <c>type</c>，见 <see cref="ChannelsGuaranteeTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置保障单创建时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置保障单更新时间戳（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>）。</summary>
    [JsonPropertyName("apply_reason")]
    public string? ApplyReason { get; set; }

    /// <summary>获取或设置商品信息（官方 <c>product_info</c>）。</summary>
    [JsonPropertyName("product_info")]
    public List<ChannelsGuaranteeProductInfo>? ProductInfo { get; set; }

    /// <summary>获取或设置保障单过期时间（官方 <c>expire_time</c>）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>获取或设置买家身份标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置买家在开放平台的唯一标识符（官方 <c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>pay_amount</c>，单位分）。</summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

    /// <summary>获取或设置商家拒绝原因（官方 <c>merchant_refuse_reason</c>）。</summary>
    [JsonPropertyName("merchant_refuse_reason")]
    public string? MerchantRefuseReason { get; set; }

    /// <summary>获取或设置订单支付信息（官方 <c>order_pay_info</c>）。</summary>
    [JsonPropertyName("order_pay_info")]
    public ChannelsGuaranteeOrderPayInfo? OrderPayInfo { get; set; }

    /// <summary>获取或设置保障单完成时间（官方 <c>complete_time</c>）。</summary>
    [JsonPropertyName("complete_time")]
    public long? CompleteTime { get; set; }

    /// <summary>获取或设置申请原因类型（官方 <c>apply_reason_type</c>）。</summary>
    [JsonPropertyName("apply_reason_type")]
    public int? ApplyReasonType { get; set; }

    /// <summary>获取或设置送礼信息（官方 <c>order_present_info</c>）。</summary>
    [JsonPropertyName("order_present_info")]
    public ChannelsGuaranteeOrderPresentInfo? OrderPresentInfo { get; set; }

    /// <summary>获取或设置发起保障的订单类型（官方 <c>order_type</c>）：0 普通订单；10 礼物订单。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置假一赔三保障单详情（官方 <c>fake_one_pay_four_info</c>）。</summary>
    [JsonPropertyName("fake_one_pay_four_info")]
    public ChannelsGuaranteeFakeOnePayFourInfo? FakeOnePayFourInfo { get; set; }

    /// <summary>获取或设置坏损包退保障单详情（官方 <c>bad_pay_info</c>）。</summary>
    [JsonPropertyName("bad_pay_info")]
    public ChannelsGuaranteeBadPayInfo? BadPayInfo { get; set; }

    /// <summary>获取或设置小程序会员已经优惠金额（官方 <c>wxa_vip_discounted_price</c>，单位分）。</summary>
    [JsonPropertyName("wxa_vip_discounted_price")]
    public long? WxaVipDiscountedPrice { get; set; }

    /// <summary>获取或设置协商历史（官方 <c>history_list</c>）。</summary>
    [JsonPropertyName("history_list")]
    public List<ChannelsGuaranteeHistory>? HistoryList { get; set; }

    /// <summary>获取或设置保障单当前状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsGuaranteeStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

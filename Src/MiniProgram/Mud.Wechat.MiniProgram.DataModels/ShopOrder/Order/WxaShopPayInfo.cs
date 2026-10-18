// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单支付信息（官方 <c>PayInfo</c> 表，读侧）。
/// </summary>
/// <remarks>
/// <para><b>支付方式枚举在本域两支不同宽窄</b>：详情表原文「0: 微信支付(目前只有微信支付)」，
/// 而 <c>/shop/order/add</c> 的请求表给 0 微信支付 / 1 货到付款 / 2 商家会员储蓄卡三值 ⇒ 读侧值域以详情表为准，见 <see cref="WxaShopPayMethodTypes"/>。</para>
/// <para><b>敏感值</b>：<c>transaction_id</c>（微信支付流水单号）与 <c>prepay_id</c> 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayInfo
{
    /// <summary>pay_method_type（官方类型 <c>number</c>）：支付方式，见 <see cref="WxaShopPayMethodTypes"/>。</summary>
    [JsonPropertyName("pay_method_type")]
    public int? PayMethodType { get; set; }

    /// <summary>pay_method（官方示例键，详情表未列）：支付方式中文名（如「微信支付」）。</summary>
    [JsonPropertyName("pay_method")]
    public string? PayMethod { get; set; }

    /// <summary>prepay_id（官方类型 <c>string</c>）：预支付 ID（敏感，不得进日志）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>prepay_time（官方类型 <c>string</c>）：预付款时间，即拿到 prepay_id 的时间。</summary>
    [JsonPropertyName("prepay_time")]
    public string? PrepayTime { get; set; }

    /// <summary>transaction_id（官方类型 <c>string</c>）：微信支付流水单号（敏感，不得进日志）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>pay_time（官方类型 <c>string</c>）：付款时间，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }

    /// <summary>promotion_detail_info（官方类型 <c>PromotionDetailInfo</c>）：微信支付券信息。</summary>
    [JsonPropertyName("promotion_detail_info")]
    public WxaShopPromotionDetailInfo? PromotionDetailInfo { get; set; }
}

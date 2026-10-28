// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「生成订单」的支付信息（官方写侧 <c>order_detail.pay_info</c>，三支字段）。
/// </summary>
/// <remarks>
/// <para><b>与读侧 <see cref="WxaShopPayInfo"/> 不共用</b>：写侧无 <c>transaction_id</c> / <c>pay_time</c> /
/// <c>pay_method</c> / <c>promotion_detail_info</c>（支付结果由 <c>/shop/order/pay</c> 回填）⇒ 合并类型会把不该发的键发出去。</para>
/// <para><b>条件必填</b>：官方标注 <c>pay_method_type = 0</c>（微信支付）时 <c>prepay_id</c> 与 <c>prepay_time</c> 必填。</para>
/// <para><b>敏感值</b>：<c>prepay_id</c> 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderAddPayInfo
{
    /// <summary>pay_method_type（官方类型 <c>number</c>，必填）：支付方式，0 微信支付 / 1 货到付款 / 2 商家会员储蓄卡，默认 0，见 <see cref="WxaShopPayMethodTypes"/>。</summary>
    [JsonPropertyName("pay_method_type")]
    public int? PayMethodType { get; set; }

    /// <summary>prepay_id（官方类型 <c>string</c>）：预支付 ID，官方标注「支付方式为微信支付时必填」（敏感，不得进日志）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>prepay_time（官方类型 <c>string</c>）：拿到 prepay_id 的时间，<c>yyyy-MM-dd HH:mm:ss</c>，支付方式为微信支付时必填。</summary>
    [JsonPropertyName("prepay_time")]
    public string? PrepayTime { get; set; }
}

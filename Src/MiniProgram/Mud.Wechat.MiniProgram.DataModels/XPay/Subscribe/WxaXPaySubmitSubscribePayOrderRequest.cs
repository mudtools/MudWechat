// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「发起订阅扣款」请求体（<c>POST /xpay/submit_subscribe_pay_order</c>）。
/// </summary>
/// <remarks>
/// <para><b>唯一性校验不可强依赖</b>（官方原文「不建议业务强依赖平台对这里的唯一性校验」）⇒ 幂等须由调用方自己的单号体系保证。</para>
/// <para><b>单号格式是硬约束</b>：<c>order_id</c> 要求 8-32 字符、仅数字/大小写字母/下划线/短横线。</para>
/// <para><b><c>env</c> 官方标注「仅支持正式环境」</b>（0）⇒ 与本域其余端点的「0 正式 / 1 沙箱」两支在本端点上不等价（沙箱不可用）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_submit_subscribe_pay_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPaySubmitSubscribePayOrderRequest
{
    /// <summary>openid（官方类型 <c>string</c>，<b>必填</b>）：用户的openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>offer_id（官方类型 <c>string</c>，<b>必填</b>）：在米大师侧申请的应用 id, mp-支付基础配置中的offerid</summary>
    [JsonPropertyName("offer_id")]
    public string? OfferId { get; set; }

    /// <summary>buy_quantity（官方类型 <c>number</c>，<b>必填</b>）：购买数量，填：1</summary>
    [JsonPropertyName("buy_quantity")]
    public long? BuyQuantity { get; set; }

    /// <summary>env（官方类型 <c>number</c>）：环境配置, 0 正式环境, 仅支持正式环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>currency_type（官方类型 <c>string</c>，<b>必填</b>）：币种，填：CNY</summary>
    [JsonPropertyName("currency_type")]
    public string? CurrencyType { get; set; }

    /// <summary>product_id（官方类型 <c>string</c>，<b>必填</b>）：订阅道具ID</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>deduct_price（官方类型 <c>number</c>，<b>必填</b>）：扣款金额(分), 属于 [1，道具价格]</summary>
    [JsonPropertyName("deduct_price")]
    public long? DeductPrice { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>，<b>必填</b>）：业务订单号, 每个订单号只能使用一次, 重复使用会失败(不建议业务强依赖平台对这里的唯一性校验)，要求8-32个字符内, 只能是数字、大小写字母、符号 _-\</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>attach（官方类型 <c>string</c>，<b>必填</b>）：透传数据, 发货通知时会透传给开发者</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }
}

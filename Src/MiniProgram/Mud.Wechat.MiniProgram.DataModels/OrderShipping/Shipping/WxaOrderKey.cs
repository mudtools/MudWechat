// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「订单定位键」（上传侧 <c>order_key</c>，发货信息录入与合单录入共用）。
/// </summary>
/// <remarks>
/// <para><b>字段名与查询侧不对称（官方原文照录，不得统一）</b>：本对象用 <c>mchid</c> / <c>out_trade_no</c>，
/// 而「查询订单发货状态」的 <c>order</c> 对象用 <c>merchant_id</c> / <c>merchant_trade_no</c>
/// （见 <see cref="WxaOrderInfo"/>）⇒ 两个方向不可共用一个 DTO，守卫 OS4 锁定该差异。</para>
/// <para><b>子单与主单类型必须一致</b>（官方原文「注意子单和主单的订单单号类型必须一致」）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_uploadshippinginfo.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderKey
{
    /// <summary>order_number_type（官方类型 <c>number</c>，必填）：订单单号类型，用于确认需要上传详情的订单。枚举值1，使用下单商户号和商户侧单号；枚举值2，使用微信支付单号。取值见 <see cref="WxaOrderNumberTypeValues"/></summary>
    [JsonPropertyName("order_number_type")]
    public long? OrderNumberType { get; set; }

    /// <summary>transaction_id（官方类型 <c>string</c>）：原支付交易对应的微信订单号</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>mchid（官方类型 <c>string</c>）：支付下单商户的商户号，由微信支付生成并下发。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>out_trade_no（官方类型 <c>string</c>）：商户系统内部订单号，只能是数字、大小写字母`_-*`且在同一个商户号下唯一</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}

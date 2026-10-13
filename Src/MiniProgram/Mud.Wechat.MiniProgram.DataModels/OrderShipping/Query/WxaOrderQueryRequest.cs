// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询订单发货状态」请求体（<c>POST /wxa/sec/order/get_order</c>）。
/// </summary>
/// <remarks>
/// <para><b>四支定位键官方全部标「否」</b>，实际按注意事项二选一：微信订单号形式取 <c>transaction_id</c>，
/// 商户侧形式取 <c>merchant_id</c> + <c>merchant_trade_no</c>；<c>sub_merchant_id</c> 仅二级商户场景附加。
/// 拼错即 <c>10060001</c>「支付单不存在」。</para>
/// <para><b>与上传侧字段名不同</b>：本处为 <c>merchant_id</c> / <c>merchant_trade_no</c>，
/// 上传侧 <see cref="WxaOrderKey"/> 为 <c>mchid</c> / <c>out_trade_no</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderQueryRequest
{
    /// <summary>transaction_id（官方类型 <c>string</c>）：原支付交易对应的微信订单号。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>merchant_id（官方类型 <c>string</c>）：支付下单商户的商户号，由微信支付生成并下发。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

    /// <summary>sub_merchant_id（官方类型 <c>string</c>）：二级商户号。</summary>
    [JsonPropertyName("sub_merchant_id")]
    public string? SubMerchantId { get; set; }

    /// <summary>merchant_trade_no（官方类型 <c>string</c>）：商户系统内部订单号，只能是数字、大小写字母`_-*`且在同一个商户号下唯一。</summary>
    [JsonPropertyName("merchant_trade_no")]
    public string? MerchantTradeNo { get; set; }
}

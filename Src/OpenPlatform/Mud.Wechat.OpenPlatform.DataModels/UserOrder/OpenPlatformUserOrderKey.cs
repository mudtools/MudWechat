// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 购物订单标识（官方 <c>order_key</c>；普通订单与合单订单的父单/子单共用）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShoppingInfo.html"/></para>
/// <para><c>order_number_type</c> 官方字段表声明为 <c>number</c>（1 = 微信支付单号、2 = 商户单号）；
/// 对应单号字段 <c>transaction_id</c> 与 <c>out_trade_no</c> 按其取值二选一填写。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderKey
{
    /// <summary>订单单号类型（官方必填 <c>order_number_type</c>，number；1 = 微信支付单号、2 = 商户单号）。</summary>
    [JsonPropertyName("order_number_type")]
    public int OrderNumberType { get; set; }

    /// <summary>微信支付订单号（官方 <c>transaction_id</c>；<c>order_number_type = 1</c> 时必填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>支付下单商户的商户号（官方 <c>mchid</c>；官方示例随支付单号一并给出）。</summary>
    [JsonPropertyName("mchid")]
    public string? MerchantId { get; set; }

    /// <summary>原支付交易的商户订单号（官方 <c>out_trade_no</c>；<c>order_number_type = 2</c> 时必填）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNumber { get; set; }
}

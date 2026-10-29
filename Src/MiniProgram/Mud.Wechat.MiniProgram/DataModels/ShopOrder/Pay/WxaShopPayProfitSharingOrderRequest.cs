// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 请求分账（<c>/shop/pay/profitsharingorder</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayProfitSharingOrderRequest
{
    /// <summary>用户 openid（<b>必填</b>；敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>订单对应的商家商户号（<b>必填</b>；敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商家交易单号（<b>必填</b>）。</summary>
    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }

    /// <summary>支付单号（<b>必填</b>；敏感，不得进日志）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商家分账单号（<b>必填</b>）：小程序系统内唯一，长度 6～32；<b>同一分账单号多次请求等同一次</b>。</summary>
    [JsonPropertyName("profit_sharing_no")]
    public string? ProfitSharingNo { get; set; }
}

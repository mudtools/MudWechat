// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 创建支付订单的子单（<c>/shop/pay/createorder</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPaySubOrder
{
    /// <summary>子单对应的商家商户号（<b>必填</b>；敏感，不得进日志）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商品描述（<b>必填</b>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>订单金额（<b>必填</b>），<b>单位分</b>。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>商家交易单号（<b>必填</b>）：只能是数字、大小写字母与 <c>_-|*@</c>，长度 6～32，同一 <c>trade_no</c> 不允许修改价格等参数。</summary>
    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }
}

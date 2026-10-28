// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 创建支付订单请求（<c>/shop/pay/createorder</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayCreateOrderRequest
{
    /// <summary>用户 openid（<b>必填</b>；敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>商家合单支付总交易单号（<b>必填</b>）：长度 6～32，只能是数字、大小写字母与 <c>_-|*@</c>，小程序系统内唯一；<b>同一 <c>combine_trade_no</c> 视为同一请求，不允许修改子单等参数</b>。</summary>
    [JsonPropertyName("combine_trade_no")]
    public string? CombineTradeNo { get; set; }

    /// <summary>订单失效时间，秒级时间戳（选填）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>子单列表（<b>必填</b>）。</summary>
    [JsonPropertyName("sub_orders")]
    public List<WxaShopPaySubOrder>? SubOrders { get; set; }
}

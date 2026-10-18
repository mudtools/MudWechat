// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 代支付结果上报请求（<c>/shop/order/pay</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderPayRequest
{
    /// <summary>订单 ID，与 <c>out_order_id</c> <b>二选一</b>（官方类型 <c>number(uint64)</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>商家自定义订单 ID，与 <c>order_id</c> <b>二选一</b>。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>用户 openid（<b>必填</b>；敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>支付动作（<b>必填</b>）：1 支付成功、2 支付失败、3 用户取消、4 超时未支付、5 商家取消、10 其他原因取消。</summary>
    [JsonPropertyName("action_type")]
    public int? ActionType { get; set; }

    /// <summary>其他具体原因（<c>action_type=10</c> 时说明）。</summary>
    [JsonPropertyName("action_remark")]
    public string? ActionRemark { get; set; }

    /// <summary>支付订单号：<c>action_type=1</c> 且下单时 <c>pay_method_type=0</c> 时必填（敏感，不得进日志）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary><c>action_type=1</c> 时必填，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }
}

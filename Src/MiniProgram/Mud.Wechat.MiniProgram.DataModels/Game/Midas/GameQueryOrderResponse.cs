// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>查询订单状态响应。官方返回为扁平结构，无 data.order_info[] 包装。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameQueryOrderResponse : WxaResponse
{
    /// <summary>道具 id。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>支付状态（用户是否已支付）：1 未支付 / 2 已支付。</summary>
    [JsonPropertyName("pay_state")]
    public int? PayState { get; set; }

    /// <summary>发货状态（游戏币则是余额是否增加）：1 未发货 / 2 已发货。</summary>
    [JsonPropertyName("deliver_state")]
    public int? DeliverState { get; set; }

    /// <summary>支付完成时间。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long? PayFinishTime { get; set; }

    /// <summary>充值时传入的外部订单号。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付商户单号（仅微信支付方式存在）。</summary>
    [JsonPropertyName("mch_order_no")]
    public string? MchOrderNo { get; set; }

    /// <summary>交易单号（微信支付订单号，仅微信支付方式存在）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

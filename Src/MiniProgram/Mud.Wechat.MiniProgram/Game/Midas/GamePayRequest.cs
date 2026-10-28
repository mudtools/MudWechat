// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>扣除游戏币请求（pay_v2.pay，是扣币而非下单收款）。bill_no 长度 ≤63、仅限数字大小写字母及 * -，且不得以下划线开头。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GamePayRequest
{
    /// <summary>用户唯一标识符（官方必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>支付应用 ID（OfferId，官方必填）。</summary>
    [JsonPropertyName("offer_id")]
    public string? OfferId { get; set; }

    /// <summary>当前 UNIX 时间戳，单位秒（官方必填）。</summary>
    [JsonPropertyName("ts")]
    public long Ts { get; set; }

    /// <summary>已发布的分区 ID，需要和 env 对应（官方必填）。</summary>
    [JsonPropertyName("zone_id")]
    public string? ZoneId { get; set; }

    /// <summary>环境配置：0 现网环境 / 1 沙箱环境（官方必填）。</summary>
    [JsonPropertyName("env")]
    public int Env { get; set; }

    /// <summary>用户外网 ip（官方必填）。</summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; set; }

    /// <summary>扣除游戏币数量，需要大于 0（官方必填）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>扣除游戏币订单号，业务须保证全局唯一；同一 bill_no 48 小时内官方保证幂等（官方必填）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>道具信息。</summary>
    [JsonPropertyName("payitem")]
    public string? Payitem { get; set; }

    /// <summary>备注。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

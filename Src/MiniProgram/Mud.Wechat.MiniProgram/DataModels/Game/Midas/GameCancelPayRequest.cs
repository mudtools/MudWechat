// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>退回扣除游戏币请求（pay_v2.cancelPay）。本接口同时需要本次退回单号 bill_no 与被退回的原扣除单号 pay_bill_no，易漏。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.cancelPay.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameCancelPayRequest
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

    /// <summary>本次退回的游戏币数量（支持部分、多次退，总额不超过原扣除额；官方必填）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>本次退回订单号，业务须保证全局唯一（官方必填）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>被退回的扣除游戏币订单号，与扣除接口的 bill_no 对应（官方必填）。</summary>
    [JsonPropertyName("pay_bill_no")]
    public string? PayBillNo { get; set; }
}

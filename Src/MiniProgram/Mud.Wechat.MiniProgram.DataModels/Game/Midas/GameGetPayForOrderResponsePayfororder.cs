// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>索要单信息。pay_time 仅在支付成功后出现。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetPayForOrderResponsePayfororder
{
    /// <summary>游戏业务单号。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信索要单号。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>下单玩家 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>下单创建时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>金额（单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>1 待支付 / 2 已支付 / 3 已超时 / 4 米大师已发放游戏币 / 5 游戏业务已发货 / 6 游戏业务发货失败。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>米大师发货分区。</summary>
    [JsonPropertyName("zone_id")]
    public string? ZoneId { get; set; }

    /// <summary>0 米大师正式环境 / 1 米大师沙箱环境。</summary>
    [JsonPropertyName("env")]
    public int? Env { get; set; }

    /// <summary>支付时间戳，索要单支付后才存在该字段。</summary>
    [JsonPropertyName("pay_time")]
    public long? PayTime { get; set; }
}

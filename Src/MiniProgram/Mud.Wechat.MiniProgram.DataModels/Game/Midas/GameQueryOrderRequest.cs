// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>查询订单状态请求（pay_v2.queryOrder，路由是 queryorderinfo）。以 out_trade_no + biz_id 定位订单，不是 bill_no；本域五支中唯一官方明确不支持云调用的一支。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameQueryOrderRequest
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

    /// <summary>充值时传入的外部订单号（官方必填；未找到回 90019）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>业务类型：1 代币 / 2 道具直购（官方必填）。</summary>
    [JsonPropertyName("biz_id")]
    public int BizId { get; set; }
}

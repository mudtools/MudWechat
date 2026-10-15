// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>扣币/退币共用应答（pay 与 cancelPay 两页官方应答表逐字段相同，退币页仍沿用「扣款后的余额」措辞）。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GamePayResponse : WxaResponse
{
    /// <summary>订单号（扣除或退回）。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>变动后的余额（官方原文「扣款后的余额」）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>本次变动的赠送币数量（原 1.0 的 used_gen_amt）。</summary>
    [JsonPropertyName("used_present_amount")]
    public long? UsedPresentAmount { get; set; }
}

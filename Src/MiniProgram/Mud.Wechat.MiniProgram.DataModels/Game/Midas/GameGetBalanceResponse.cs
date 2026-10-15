// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>查询游戏币余额响应。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.getBalance.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetBalanceResponse : WxaResponse
{
    /// <summary>游戏币总余额，包括现金充值和赠送部分。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>赠送账户的游戏币余额（原 1.0 的 gen_balance）。</summary>
    [JsonPropertyName("present_balance")]
    public long? PresentBalance { get; set; }

    /// <summary>累计现金充值获得的游戏币数量（原 1.0 的 save_amt）。</summary>
    [JsonPropertyName("sum_save")]
    public long? SumSave { get; set; }

    /// <summary>累计赠送的游戏币数量（原 1.0 的 present_sum）。</summary>
    [JsonPropertyName("sum_present")]
    public long? SumPresent { get; set; }

    /// <summary>累计获得的游戏币数量，包括现金充值和赠送（原 1.0 的 save_sum）。</summary>
    [JsonPropertyName("sum_balance")]
    public long? SumBalance { get; set; }

    /// <summary>累计总消耗（即扣除）游戏币数量（原 1.0 的 cost_sum）。</summary>
    [JsonPropertyName("sum_cost")]
    public long? SumCost { get; set; }

    /// <summary>是否满足首充活动标记。</summary>
    [JsonPropertyName("first_save")]
    public bool? FirstSave { get; set; }
}

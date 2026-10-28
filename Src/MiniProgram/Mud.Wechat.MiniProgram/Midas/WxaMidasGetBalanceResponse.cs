// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Midas;

/// <summary>
/// 获取游戏币余额回包（<c>POST /cgi-bin/midas/getbalance</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方参数表类型自相矛盾（照录）</b>：<c>first_save</c> 表中标 <c>boolean</c>，
/// 但其「合法值」明列 1 满足 / 2 不满足 ⇒ SDK 按合法值建模为 <c>int</c>，不按表头 boolean 建模。</para>
/// <para>错误码：90010 未登录/登录态过期、90011 sig 签名错误、90017 无调用权限、90018 参数错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Midas")]
public class WxaMidasGetBalanceResponse : WxaResponse
{
    /// <summary>游戏币个数（<b>包含赠送</b>）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>赠送游戏币数量。</summary>
    [JsonPropertyName("gen_balance")]
    public long? GenBalance { get; set; }

    /// <summary>是否满足历史首次充值，见 <see cref="WxaMidasFirstSaveStatuses"/>。</summary>
    [JsonPropertyName("first_save")]
    public int? FirstSave { get; set; }

    /// <summary>累计充值金额的游戏币数量。</summary>
    [JsonPropertyName("save_amt")]
    public long? SaveAmount { get; set; }

    /// <summary>历史总游戏币金额。</summary>
    [JsonPropertyName("save_sum")]
    public long? SaveSum { get; set; }

    /// <summary>历史总消费游戏币金额。</summary>
    [JsonPropertyName("cost_sum")]
    public long? CostSum { get; set; }

    /// <summary>历史累计收到赠送金额。</summary>
    [JsonPropertyName("present_sum")]
    public long? PresentSum { get; set; }
}

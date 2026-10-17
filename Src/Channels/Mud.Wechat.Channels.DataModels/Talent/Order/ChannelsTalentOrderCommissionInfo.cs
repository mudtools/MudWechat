// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>佣金单佣金信息（官方 <c>commission_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Talent")]
public class ChannelsTalentOrderCommissionInfo
{
    /// <summary>获取或设置佣金单状态（官方 <c>state</c>，见 <see cref="ChannelsTalentCommissionOrderStates"/>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置佣金比例（官方 <c>ratio</c>，参数表标 string）。</summary>
    [JsonPropertyName("ratio")]
    public string? Ratio { get; set; }

    /// <summary>获取或设置预期结算时间戳（官方 <c>expect_settle_time</c>）。</summary>
    [JsonPropertyName("expect_settle_time")]
    public long? ExpectSettleTime { get; set; }

    /// <summary>获取或设置预期结算金额（官方 <c>expect_settlement</c>，参数表标 string）。</summary>
    [JsonPropertyName("expect_settlement")]
    public string? ExpectSettlement { get; set; }

    /// <summary>获取或设置实际结算时间戳（官方 <c>actual_settle_time</c>）。</summary>
    [JsonPropertyName("actual_settle_time")]
    public long? ActualSettleTime { get; set; }

    /// <summary>获取或设置实际结算金额（官方 <c>actual_settlement</c>，参数表标 string）。</summary>
    [JsonPropertyName("actual_settlement")]
    public string? ActualSettlement { get; set; }
}

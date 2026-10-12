// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>获取用户安全等级应答（<c>risk_rank</c>）。</summary>
/// <remarks>
/// <b>等级语义（官方原文，勿把数值当布尔）</b>：<c>0</c> 通过 / <c>1</c> 未知（未命中风险）/
/// <c>2</c> 风险待确认 / <c>3</c> 风险命中 / <c>4</c> 高风险命中。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaGetUserRiskRankResponse : WxaResponse
{
    /// <summary>用户风险等级（<c>risk_rank</c>，0~4；数值越大风险越高）。</summary>
    [JsonPropertyName("risk_rank")]
    public long? RiskRank { get; set; }
}

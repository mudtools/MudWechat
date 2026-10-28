// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取线索订单详情响应（官方 <c>headsupplier/clue/get</c> 返回体）。
/// 官方根字段名同样拼写为 <c>commssion_clue</c>（漏字母 i）⇒ JSON 名照抄，属性名按正确拼写。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetClueResponse
{
    /// <summary>获取或设置线索订单结构（官方 <c>commssion_clue</c>，官方原文拼写漏字母 i）。</summary>
    [JsonPropertyName("commssion_clue")]
    public ChannelsLeagueHeadSupplierCommissionClue? CommissionClue { get; set; }
}

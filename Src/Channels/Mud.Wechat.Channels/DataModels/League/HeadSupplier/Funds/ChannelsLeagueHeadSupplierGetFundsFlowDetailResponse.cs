// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟带货机构资金流水详情（官方 <c>headsupplier/funds/flowdetail/get</c> 响应）。</summary>
/// <remarks>
/// <para>
/// 响应外层字段名与小店商家侧 <c>ChannelsGetFundsFlowDetailResponse</c> 同为 <c>funds_flow</c>，
/// 但元素字段集不同（机构侧有 <c>order_id</c>/<c>withdraw_id</c>，无 <c>related_info_list</c> 等）
/// ⇒ 本响应类分建、内嵌 <see cref="ChannelsLeagueHeadSupplierFundsFlow"/>；
/// 请求侧 <c>flow_id</c> 单字段与商家侧逐字段一致 ⇒ 复用 <c>ChannelsGetFundsFlowDetailRequest</c>（守卫 CLG 锁类型同一）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置流水信息（官方 <c>funds_flow</c>）。</summary>
    [JsonPropertyName("funds_flow")]
    public ChannelsLeagueHeadSupplierFundsFlow? FundsFlow { get; set; }
}

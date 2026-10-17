// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟带货机构账户余额（官方 <c>headsupplier/funds/balance/get</c> 响应）。</summary>
/// <remarks>
/// <para>
/// 与小店商家侧 <c>merchant/funds/getbalance</c>（Funds 域 <c>ChannelsGetBalanceResponse</c>）<b>不同形</b>：
/// 机构侧参数表<b>无</b> <c>sub_mchid</c>（机构自身账户，不按小店区分），故分建 DTO 而非复用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetBalanceResponse : ChannelsResponse
{
    /// <summary>获取或设置可提现余额，单位分（官方 <c>available_amount</c>）。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>获取或设置待结算余额，单位分（官方 <c>pending_amount</c>）。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }
}

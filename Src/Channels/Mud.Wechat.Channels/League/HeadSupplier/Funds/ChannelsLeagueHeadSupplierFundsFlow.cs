// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>联盟带货机构资金流水信息（官方 <c>headsupplier/funds/flowdetail/get</c> 的 <c>funds_flow</c> 对象）。</summary>
/// <remarks>
/// <para>
/// 与小店商家侧 <c>ChannelsFundsFlow</c>（Funds 域，<c>merchant/funds/flowdetail/get</c>）<b>字段集不同</b>：
/// 机构侧多 <c>order_id</c>（关联订单号）与 <c>withdraw_id</c>（关联提现单号），无 <c>flow_type</c>、
/// <c>related_info_list</c>、<c>funds_type_desc</c> ⇒ 分建 DTO，不复用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierFundsFlow
{
    /// <summary>获取或设置流水 id（官方 <c>flow_id</c>）。</summary>
    [JsonPropertyName("flow_id")]
    public string? FlowId { get; set; }

    /// <summary>获取或设置资金类型（官方 <c>funds_type</c>，枚举见 <see cref="ChannelsLeagueHeadSupplierFundsTypes"/>）。</summary>
    [JsonPropertyName("funds_type")]
    public int? FundsType { get; set; }

    /// <summary>获取或设置流水金额，单位分（官方 <c>amount</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置余额，单位分（官方 <c>balance</c>）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>获取或设置记账时间（官方 <c>bookkeeping_time</c>，官方示例为 <c>"2023-02-11 15:45:39"</c> 形态字符串，不做本地解析）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置关联提现单号（官方 <c>withdraw_id</c>）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }
}

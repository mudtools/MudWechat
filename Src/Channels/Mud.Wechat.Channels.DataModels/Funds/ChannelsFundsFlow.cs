// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>资金流水信息（官方 <c>funds_flow</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsFundsFlow
{
    /// <summary>获取或设置流水 id（官方 <c>flow_id</c>）。</summary>
    [JsonPropertyName("flow_id")]
    public string? FlowId { get; set; }

    /// <summary>获取或设置资金类型（官方 <c>funds_type</c>，见 <see cref="ChannelsFundsTypes"/>）。</summary>
    [JsonPropertyName("funds_type")]
    public int? FundsType { get; set; }

    /// <summary>获取或设置流水类型（官方 <c>flow_type</c>：1 收入，2 支出）。</summary>
    [JsonPropertyName("flow_type")]
    public int? FlowType { get; set; }

    /// <summary>获取或设置流水金额（官方 <c>amount</c>，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置余额（官方 <c>balance</c>，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>获取或设置流水关联信息列表（官方 <c>related_info_list</c>）。</summary>
    [JsonPropertyName("related_info_list")]
    public List<ChannelsFundsFlowRelatedInfo>? RelatedInfoList { get; set; }

    /// <summary>获取或设置记账时间（官方 <c>bookkeeping_time</c>）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置资金类型描述（官方 <c>funds_type_desc</c>）。</summary>
    [JsonPropertyName("funds_type_desc")]
    public string? FundsTypeDesc { get; set; }
}

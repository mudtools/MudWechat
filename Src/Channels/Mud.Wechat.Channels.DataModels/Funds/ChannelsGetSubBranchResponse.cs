// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>查询支行列表（<c>getsubbranch</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：参数表中 <c>data</c> 数组元素字段名为 <c>bank_id</c> / <c>bank_name</c>，
/// 代码示例为 <c>branch_id</c> / <c>branch_name</c>——SDK 按参数表建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetSubBranchResponse : ChannelsResponse
{
    /// <summary>获取或设置其他银行信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsSubBranch>? Data { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置当前分页数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置银行编码（官方 <c>account_bank_code</c>）。</summary>
    [JsonPropertyName("account_bank_code")]
    public long? AccountBankCode { get; set; }

    /// <summary>获取或设置银行别名（官方 <c>bank_alias</c>）。</summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>获取或设置银行别名编码（官方 <c>bank_alias_code</c>）。</summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>account_bank</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }
}

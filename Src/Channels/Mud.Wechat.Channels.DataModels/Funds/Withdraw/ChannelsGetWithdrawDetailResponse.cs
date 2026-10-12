// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>获取提现记录（<c>getwithdrawdetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetWithdrawDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置金额（官方 <c>amount</c>，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置失败原因（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置银行附言（官方 <c>bank_memo</c>）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_num</c>）。</summary>
    [JsonPropertyName("bank_num")]
    public string? BankNum { get; set; }

    /// <summary>获取或设置提现状态（官方 <c>status</c>，见 <see cref="ChannelsWithdrawStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

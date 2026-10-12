// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>商户提现（<c>submitwithdraw</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；<c>amount</c> 单位：分。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSubmitWithdrawRequest
{
    /// <summary>获取或设置提现金额（官方 <c>amount</c>，必填，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>获取或设置提现备注（官方 <c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置银行附言（官方 <c>bank_memo</c>，选填）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }
}

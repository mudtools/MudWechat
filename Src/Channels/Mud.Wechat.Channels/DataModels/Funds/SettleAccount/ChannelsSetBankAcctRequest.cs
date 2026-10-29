// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>修改结算账户（<c>setbankacct</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体 <c>{"account_info":{…}}</c>。
/// 账户信息提交形态（必填）与获取形态不同：额外承载 <c>account_bank4show</c>，且 <c>bank_account_type</c> /
/// <c>account_bank</c> / <c>bank_address_code</c> / <c>account_number</c> 为必填。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSetBankAcctRequest
{
    /// <summary>获取或设置待修改的结算账户信息（官方 <c>account_info</c>，必填）。</summary>
    [JsonPropertyName("account_info")]
    public ChannelsSettleAccountModify AccountInfo { get; set; } = new();
}

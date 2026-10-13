// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 广告金充值账户（官方 <c>Res.acct_list</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>state</c> 与 <c>bind_result</c> 是两段独立状态</b>：前者是审核（0 待审核 / 1 通过 / 2 驳回）、后者是绑定（1 成功 / 2 失败），且 <c>bind_result</c> 从 1 开始编号 ⇒ <b>不得</b>把「0」当作绑定失败的取值读。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_transfer_account.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayTransferAccount
{
    /// <summary>transfer_account_name（官方类型 <c>string</c>）：充值账户名称</summary>
    [JsonPropertyName("transfer_account_name")]
    public string? TransferAccountName { get; set; }

    /// <summary>transfer_account_uid（官方类型 <c>number</c>）：充值账户 uid</summary>
    [JsonPropertyName("transfer_account_uid")]
    public long? TransferAccountUid { get; set; }

    /// <summary>transfer_account_agency_id（官方类型 <c>number</c>）：充值账户服务商账号 id</summary>
    [JsonPropertyName("transfer_account_agency_id")]
    public long? TransferAccountAgencyId { get; set; }

    /// <summary>transfer_account_agency_name（官方类型 <c>string</c>）：充值账户服务商账号名称</summary>
    [JsonPropertyName("transfer_account_agency_name")]
    public string? TransferAccountAgencyName { get; set; }

    /// <summary>state（官方类型 <c>number</c>）：0-待审核，1-审核通过，2-审核驳回</summary>
    [JsonPropertyName("state")]
    public long? State { get; set; }

    /// <summary>bind_result（官方类型 <c>number</c>）：1-绑定成功，2-绑定失败</summary>
    [JsonPropertyName("bind_result")]
    public long? BindResult { get; set; }

    /// <summary>error_msg（官方类型 <c>string</c>）：错误信息</summary>
    [JsonPropertyName("error_msg")]
    public string? ErrorMsg { get; set; }
}

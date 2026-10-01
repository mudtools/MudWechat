// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

/// <summary>
/// 分配在职成员的客户请求体（<c>/cgi-bin/externalcontact/transfer_customer</c>）。
/// <para>原跟进成员与接替成员在最近一年内需登录过企业微信；
/// <see cref="ExternalUserid"/> 必须是 <see cref="HandoverUserid"/> 的客户
/// （即配置了客户联系功能的成员所添加的联系人）；
/// 为保障客户服务体验，90 个自然日内在职成员的每位客户仅可被转接 2 次。</para>
/// </summary>
public class TransferCustomerRequest
{
    /// <summary>
    /// 获取或设置原跟进成员的 userid（官方必填）。
    /// </summary>
    [JsonPropertyName("handover_userid")]
    public string? HandoverUserid { get; set; }

    /// <summary>
    /// 获取或设置接替成员的 userid（官方必填；须在应用可见范围内、配置了客户联系功能，
    /// 且已在企业微信激活并实名认证）。
    /// </summary>
    [JsonPropertyName("takeover_userid")]
    public string? TakeoverUserid { get; set; }

    /// <summary>
    /// 获取或设置客户的 external_userid 列表（官方必填；每次最多分配 100 个客户）。
    /// </summary>
    [JsonPropertyName("external_userid")]
    public List<string>? ExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置转移成功后发给客户的消息，最多 200 个字符（不填则使用默认文案）。
    /// </summary>
    [JsonPropertyName("transfer_success_msg")]
    public string? TransferSuccessMsg { get; set; }
}

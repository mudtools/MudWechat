// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfAccount;

/// <summary>
/// 邀请绑定客服账号请求体（<c>invitekfworker</c>，<c>POST /customservice/kfaccount/inviteworker</c>）。
/// </summary>
/// <remarks>
/// <para>官方字段表：<c>kf_account</c>（完整客服账号）+ <c>invite_wx</c>（接收绑定邀请的客服微信号）。</para>
/// <para>
/// 官方错误码（全为该业务的专有码）：<c>65407</c>（已是本账号客服）/ <c>65408</c>（已发送邀请）/
/// <c>65409</c>（无效微信号）/ <c>65410</c>（绑定数量超限）/ <c>65411</c>（存在待确认邀请）/
/// <c>65412</c>（该客服账号已绑定微信号）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfAccount")]
public class MpInviteKfWorkerRequest
{
    /// <summary>完整客服账号（「账号前缀@公众号微信号」）。</summary>
    [JsonPropertyName("kf_account")]
    public string KfAccount { get; set; } = string.Empty;

    /// <summary>接收绑定邀请的客服微信号。</summary>
    [JsonPropertyName("invite_wx")]
    public string InviteWx { get; set; } = string.Empty;
}

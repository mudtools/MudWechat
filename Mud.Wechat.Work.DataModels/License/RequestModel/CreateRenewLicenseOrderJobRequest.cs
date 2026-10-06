// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 创建续期任务请求体（<c>/cgi-bin/license/create_renew_order_job</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class CreateRenewLicenseOrderJobRequest
{
    /// <summary>获取或设置企业 id（官方必填）。</summary>
    [JsonPropertyName("corpid")]
    public string Corpid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置续期的账号列表（官方必填）。
    /// <para>官方约束：每次最多 1000 个；同一个 jobid 最多关联 1000000 个基础账号跟 1000000 个互通账号。</para>
    /// </summary>
    [JsonPropertyName("account_list")]
    public List<LicenseRenewAccount> AccountList { get; set; } = new List<LicenseRenewAccount>();

    /// <summary>
    /// 获取或设置任务 id（官方可选）。
    /// <para>官方约束：若不传则默认创建一个新任务；若指定第一次调用后拿到 jobid，
    /// 可以通过该接口将 jobid 关联多个 userid。</para>
    /// </summary>
    [JsonPropertyName("jobid")]
    public string? Jobid { get; set; }
}

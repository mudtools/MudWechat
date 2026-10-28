// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.License;

/// <summary>
/// 创建续期任务响应体（<c>/cgi-bin/license/create_renew_order_job</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "License")]
public class CreateRenewLicenseOrderJobResponse : WechatWorkResponse
{
    /// <summary>获取或设置任务 id（请求包中未指定 jobid 时，会生成一个新的 jobid 返回）。</summary>
    [JsonPropertyName("jobid")]
    public string? Jobid { get; set; }

    /// <summary>获取或设置不合法的续期账号列表。</summary>
    [JsonPropertyName("invalid_account_list")]
    public List<LicenseInvalidAccount>? InvalidAccountList { get; set; }
}

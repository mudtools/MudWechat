// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「获取最新一次审核提交状态」（<c>get_latest_auditstatus</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getLatestAuditStatus.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpLatestAuditStatusResponse : OpenPlatformResponse
{
    /// <summary>最新审核单号（官方 <c>auditid</c>，number）。</summary>
    [JsonPropertyName("auditid")]
    public long AuditId { get; set; }

    /// <summary>审核状态（官方 <c>status</c>，number）。</summary>
    [JsonPropertyName("status")]
    public long Status { get; set; }

    /// <summary>驳回原因（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>驳回截图（官方 <c>screenshot</c>）。</summary>
    [JsonPropertyName("screenshot")]
    public string? Screenshot { get; set; }

    /// <summary>用户自定义版本号（官方 <c>user_version</c>）。</summary>
    [JsonPropertyName("user_version")]
    public string? UserVersion { get; set; }

    /// <summary>用户自定义版本描述（官方 <c>user_desc</c>）。</summary>
    [JsonPropertyName("user_desc")]
    public string? UserDesc { get; set; }

    /// <summary>提交审核时间（官方 <c>submit_audit_time</c>，number）。</summary>
    [JsonPropertyName("submit_audit_time")]
    public long SubmitAuditTime { get; set; }
}

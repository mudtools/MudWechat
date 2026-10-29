// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「查询审核状态」（<c>get_auditstatus</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getAuditStatus.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpAuditStatusResponse : OpenPlatformResponse
{
    /// <summary>审核状态（官方 <c>status</c>，number：0 排队中 / 1 通过 / 2 不通过 / 3 审核前版本撤回 / 4 审核后版本撤回 / 5 审核中 / 6 撤回审核中）。</summary>
    [JsonPropertyName("status")]
    public long Status { get; set; }

    /// <summary>驳回原因（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>驳回截图（官方 <c>screenshot</c>）。</summary>
    [JsonPropertyName("screenshot")]
    public string? Screenshot { get; set; }
}

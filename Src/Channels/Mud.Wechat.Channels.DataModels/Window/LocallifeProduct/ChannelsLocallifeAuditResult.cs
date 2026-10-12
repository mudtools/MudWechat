// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>团购优惠审核结果（官方 <c>audit_result</c> 对象，<c>getlocalproduct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeAuditResult
{
    /// <summary>获取或设置审核状态（官方 <c>audit_status</c>，见 <see cref="ChannelsLocallifeAuditStatuses"/>；官方返回示例亦出现 0——照录）。</summary>
    [JsonPropertyName("audit_status")]
    public int? AuditStatus { get; set; }

    /// <summary>获取或设置审核失败原因（官方 <c>audit_reason</c>；审核失败时有值）。</summary>
    [JsonPropertyName("audit_reason")]
    public string? AuditReason { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 微盘文件分享范围（获取文件权限信息响应的 share_range 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class WedriveFileShareRange
{
    /// <summary>获取或设置是否为企业内可访问。</summary>
    [JsonPropertyName("enable_corp_internal")]
    public bool? EnableCorpInternal { get; set; }

    /// <summary>
    /// 获取或设置企业内权限信息：普通文档 1 - 仅浏览（可下载）、4 - 仅预览（仅专业版可设）、
    /// 255 - 无权限或需审批；微文档 1 - 仅浏览；不填充则保持原状。
    /// </summary>
    [JsonPropertyName("corp_internal_auth")]
    public ulong? CorpInternalAuth { get; set; }

    /// <summary>获取或设置是否为企业外可访问。</summary>
    [JsonPropertyName("enable_corp_external")]
    public bool? EnableCorpExternal { get; set; }

    /// <summary>获取或设置企业外权限信息（取值同 corp_internal_auth）。</summary>
    [JsonPropertyName("corp_external_auth")]
    public ulong? CorpExternalAuth { get; set; }

    /// <summary>获取或设置是否开启企业内管理员审批。</summary>
    [JsonPropertyName("corp_internal_approve_only_by_admin")]
    public bool? CorpInternalApproveOnlyByAdmin { get; set; }

    /// <summary>获取或设置是否开启企业外管理员审批。</summary>
    [JsonPropertyName("corp_external_approve_only_by_admin")]
    public bool? CorpExternalApproveOnlyByAdmin { get; set; }
}

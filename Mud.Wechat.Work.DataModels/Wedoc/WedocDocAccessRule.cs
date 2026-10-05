// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档的查看规则（官方 <c>access_rule</c>；获取文档权限信息响应体嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocAccessRule
{
    /// <summary>获取或设置是否允许企业内成员浏览文档（官方 <c>enable_corp_internal</c>）。</summary>
    [JsonPropertyName("enable_corp_internal")]
    public bool? EnableCorpInternal { get; set; }

    /// <summary>
    /// 获取或设置企业内成员主动查看文档后获得的权限类型（官方 <c>corp_internal_auth</c>）。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写。
    /// </summary>
    [JsonPropertyName("corp_internal_auth")]
    public uint? CorpInternalAuth { get; set; }

    /// <summary>获取或设置是否允许企业外成员浏览文档（官方 <c>enable_corp_external</c>）。</summary>
    [JsonPropertyName("enable_corp_external")]
    public bool? EnableCorpExternal { get; set; }

    /// <summary>
    /// 获取或设置企业外成员浏览文档后获得的权限类型（官方 <c>corp_external_auth</c>）。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写。
    /// </summary>
    [JsonPropertyName("corp_external_auth")]
    public uint? CorpExternalAuth { get; set; }

    /// <summary>
    /// 获取或设置企业内成员浏览文档是否必须由管理员审批（官方 <c>corp_internal_approve_only_by_admin</c>）；
    /// <c>enable_corp_internal</c> 为 <c>false</c> 时，只能为 <c>true</c>。
    /// </summary>
    [JsonPropertyName("corp_internal_approve_only_by_admin")]
    public bool? CorpInternalApproveOnlyByAdmin { get; set; }

    /// <summary>
    /// 获取或设置企业外成员浏览文档是否必须由管理员审批（官方 <c>corp_external_approve_only_by_admin</c>）；
    /// <c>enable_corp_external</c> 与 <c>ban_share_external</c> 均为 <c>false</c> 时，只能为 <c>true</c>。
    /// </summary>
    [JsonPropertyName("corp_external_approve_only_by_admin")]
    public bool? CorpExternalApproveOnlyByAdmin { get; set; }

    /// <summary>获取或设置是否禁止文档分享到企业外（官方 <c>ban_share_external</c>）。</summary>
    [JsonPropertyName("ban_share_external")]
    public bool? BanShareExternal { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 修改文档加入规则请求体（<c>/cgi-bin/wedoc/mod_doc_join_rule</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：文档需有至少一个管理员，才能将 <c>approve_only_by_admin</c> 类参数设置为 <c>true</c>；
/// <c>enable_corp_internal</c> 为 <c>false</c> 时 <c>corp_internal_approve_only_by_admin</c> 只能为 <c>true</c>；
/// <c>enable_corp_external</c> 与 <c>ban_share_external</c> 均为 <c>false</c> 时 <c>corp_external_approve_only_by_admin</c> 只能为 <c>true</c>。
/// 各开关类字段有值才会覆盖原配置。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UpdateWedocDocJoinRuleRequest
{
    /// <summary>获取或设置操作的文档 id（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置是否允许企业内成员浏览文档（官方 <c>enable_corp_internal</c>），有值则覆盖。</summary>
    [JsonPropertyName("enable_corp_internal")]
    public bool? EnableCorpInternal { get; set; }

    /// <summary>
    /// 获取或设置企业内成员主动查看文档后获得的权限类型（官方 <c>corp_internal_auth</c>），有值则覆盖。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写。
    /// </summary>
    [JsonPropertyName("corp_internal_auth")]
    public uint? CorpInternalAuth { get; set; }

    /// <summary>获取或设置是否允许企业外成员浏览文档（官方 <c>enable_corp_external</c>），有值则覆盖。</summary>
    [JsonPropertyName("enable_corp_external")]
    public bool? EnableCorpExternal { get; set; }

    /// <summary>
    /// 获取或设置企业外成员浏览文档后获得的权限类型（官方 <c>corp_external_auth</c>），有值则覆盖。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写。
    /// </summary>
    [JsonPropertyName("corp_external_auth")]
    public uint? CorpExternalAuth { get; set; }

    /// <summary>
    /// 获取或设置企业内成员加入是否必须由管理员审批（官方 <c>corp_internal_approve_only_by_admin</c>）。
    /// <c>enable_corp_internal</c> 为 <c>false</c> 时只能为 <c>true</c>；设为 <c>true</c> 前文档需至少一个管理员。
    /// </summary>
    [JsonPropertyName("corp_internal_approve_only_by_admin")]
    public bool? CorpInternalApproveOnlyByAdmin { get; set; }

    /// <summary>
    /// 获取或设置企业外成员加入是否必须由管理员审批（官方 <c>corp_external_approve_only_by_admin</c>）。
    /// <c>enable_corp_external</c> 与 <c>ban_share_external</c> 均为 <c>false</c> 时只能为 <c>true</c>；设为 <c>true</c> 前文档需至少一个管理员。
    /// </summary>
    [JsonPropertyName("corp_external_approve_only_by_admin")]
    public bool? CorpExternalApproveOnlyByAdmin { get; set; }

    /// <summary>获取或设置是否禁止文档分享到企业外（官方 <c>ban_share_external</c>），有值则覆盖。</summary>
    [JsonPropertyName("ban_share_external")]
    public bool? BanShareExternal { get; set; }

    /// <summary>获取或设置是否更新文档查看权限的特定部门列表（官方 <c>update_co_auth_list</c>），为 <c>true</c> 时更新列表。</summary>
    [JsonPropertyName("update_co_auth_list")]
    public bool? UpdateCoAuthList { get; set; }

    /// <summary>
    /// 获取或设置文档查看权限特定部门列表（官方 <c>co_auth_list</c>），更新时覆盖之前的部门列表，列表为空则清空。
    /// </summary>
    [JsonPropertyName("co_auth_list")]
    public List<WedocDocCoAuth>? CoAuthList { get; set; }
}

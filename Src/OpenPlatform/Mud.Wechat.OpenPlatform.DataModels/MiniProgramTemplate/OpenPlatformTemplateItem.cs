// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 模板列表项（<c>gettemplatelist</c> 应答 <c>template_list</c> 数组元素）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/getTemplateList.html"/>。
/// </summary>
/// <remarks>
/// <para><c>audit_scene</c>/<c>audit_status</c>/<c>reason</c>/<c>category_list</c> 仅标准模板返回，普通模板不返回或返回空数组。</para>
/// <para><c>audit_status</c> 枚举：0 未提审核 / 1 审核中 / 2 审核驳回 / 3 审核通过 / 4 提审中 / 5 提审失败。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformTemplateItem
{
    /// <summary>模板 ID（官方字段名 <c>template_id</c>，number）。</summary>
    [JsonPropertyName("template_id")]
    public long TemplateId { get; set; }

    /// <summary>草稿 ID（官方字段名 <c>draft_id</c>，number）。</summary>
    [JsonPropertyName("draft_id")]
    public long DraftId { get; set; }

    /// <summary>版本号，开发者自定义字段（官方字段名 <c>user_version</c>，string）。</summary>
    [JsonPropertyName("user_version")]
    public string? UserVersion { get; set; }

    /// <summary>版本描述，开发者自定义字段（官方字段名 <c>user_desc</c>，string）。</summary>
    [JsonPropertyName("user_desc")]
    public string? UserDesc { get; set; }

    /// <summary>开发者上传草稿时间戳（官方字段名 <c>create_time</c>，number）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>开发小程序的 appid（官方字段名 <c>source_miniprogram_appid</c>，string）。</summary>
    [JsonPropertyName("source_miniprogram_appid")]
    public string? SourceMiniprogramAppid { get; set; }

    /// <summary>开发小程序的名称（官方字段名 <c>source_miniprogram</c>，string）。</summary>
    [JsonPropertyName("source_miniprogram")]
    public string? SourceMiniprogram { get; set; }

    /// <summary>模板类型：0 普通模板 / 1 标准模板（官方字段名 <c>template_type</c>，number）。</summary>
    [JsonPropertyName("template_type")]
    public long TemplateType { get; set; }

    /// <summary>标准模板的场景标签，普通模板不返回（官方字段名 <c>audit_scene</c>，number）。</summary>
    [JsonPropertyName("audit_scene")]
    public long AuditScene { get; set; }

    /// <summary>标准模板的审核状态，普通模板不返回（官方字段名 <c>audit_status</c>，number）。</summary>
    [JsonPropertyName("audit_status")]
    public long AuditStatus { get; set; }

    /// <summary>标准模板的审核驳回原因，普通模板不返回（官方字段名 <c>reason</c>，string）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>标准模板的类目信息；普通模板为空数组（官方字段名 <c>category_list</c>，objarray）。</summary>
    [JsonPropertyName("category_list")]
    public List<OpenPlatformTemplateCategoryItem>? CategoryList { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 更新审批模板请求体（<c>/cgi-bin/oa/approval/update_template</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：仅能更新自身应用模板；更新模板后管理后台及审批应用内将更新原模板的内容，已配置的审批流程和规则不变；模板已配置自定义打印格式时不支持 API 修改模板（错误码 301115）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class UpdateApprovalTemplateRequest
{
    /// <summary>
    /// 获取或设置模版id（官方必填）。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 获取或设置模版名称数组（官方必填）。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：模版名称不得和现有模版名称重复；长度不得超过 40 个字符。</para>
    /// </remarks>
    [JsonPropertyName("template_name")]
    public List<ApprovalLangTextItem>? TemplateName { get; set; }

    /// <summary>
    /// 获取或设置审批模版控件设置（官方必填），可以参考创建审批模板的 template_content 参数说明。
    /// </summary>
    [JsonPropertyName("template_content")]
    public ApprovalTemplateSettingContent? TemplateContent { get; set; }
}

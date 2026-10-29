// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 创建审批模板请求体（<c>/cgi-bin/oa/approval/create_template</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：一个模版中只能拥有一类假勤控件类型（Vacation-假期；Attendance-外出/出差/加班 均为假勤控件类型）；当模板的控件为必填属性时，表单中对应的控件必须有值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class CreateApprovalTemplateRequest
{
    /// <summary>
    /// 获取或设置模版名称数组（官方必填）。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：模版名称不得和现有模版名称重复；长度不得超过 40 个字符。</para>
    /// </remarks>
    [JsonPropertyName("template_name")]
    public List<ApprovalLangTextItem>? TemplateName { get; set; }

    /// <summary>
    /// 获取或设置审批模版控件设置（官方必填），由多个表单控件及其内容组成。
    /// </summary>
    [JsonPropertyName("template_content")]
    public ApprovalTemplateSettingContent? TemplateContent { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 复制/更新模板到企业请求体（<c>/cgi-bin/oa/approval/copytemplate</c>）。
/// </summary>
/// <remarks>
/// <para>官方说明：当企业之前未有此服务商模板时，将执行「复制」操作；当企业已有此服务商模板时，将执行「更新」操作，即将服务商模板的最新版本更新到企业审批应用中，覆盖旧版本；企业设为条件审批的控件若被删除，条件审批流程可能失效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class CopyApprovalTemplateRequest
{
    /// <summary>
    /// 获取或设置服务商审批模板的唯一标识id（官方必填；可在「获取审批单据详情」「审批状态变化回调通知」中获得，也可在服务商后台-应用管理-审批模板的模板编辑页面中获得）。
    /// </summary>
    [JsonPropertyName("open_template_id")]
    public string? OpenTemplateId { get; set; }
}

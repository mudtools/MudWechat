// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 获取审批模板详情请求体（<c>/cgi-bin/oa/gettemplatedetail</c>）。
/// </summary>
/// <remarks>
/// <para>较早时间创建的模板，id 为类似「1910324946027731_1688852032423522_1808577376_15111111111」的数字串。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class GetApprovalTemplateDetailRequest
{
    /// <summary>
    /// 获取或设置模板的唯一标识id（官方必填；可在「获取审批单据详情」「审批状态变化回调通知」中获得，也可在审批模板的模板编辑页面链接中获得）。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }
}

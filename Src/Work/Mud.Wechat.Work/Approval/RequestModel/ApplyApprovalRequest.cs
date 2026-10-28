// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 提交审批申请请求体（<c>/cgi-bin/oa/applyevent</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：接口频率限制 600 次/分钟；当模板的控件为必填属性时，表单中对应的控件必须有值。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApplyApprovalRequest
{
    /// <summary>
    /// 获取或设置申请人userid（官方必填），此审批申请将以此员工身份提交，申请人需在应用可见范围内。
    /// </summary>
    [JsonPropertyName("creator_userid")]
    public string? CreatorUserid { get; set; }

    /// <summary>
    /// 获取或设置模板id（官方必填；可在「获取审批申请详情」「审批状态变化回调通知」中获得，也可在审批模板的模板编辑页面链接中获得）。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：暂不支持通过接口提交【打卡补卡】【调班】模板审批单；第三方/代开发场景此 id 为企业内模板的实例id，非服务商后台对应模板的id。</para>
    /// </remarks>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 获取或设置审批人模式（官方必填）：0-通过接口指定审批人、抄送人（此时 process 参数必填）；1-使用此模板在管理后台设置的审批流程（需要保证审批流程中没有「申请人自选」节点），支持条件审批。默认为 0。
    /// </summary>
    [JsonPropertyName("use_template_approver")]
    public int? UseTemplateApprover { get; set; }

    /// <summary>
    /// 获取或设置提单者提单部门id，不填默认为主部门。
    /// </summary>
    [JsonPropertyName("choose_department")]
    public int? ChooseDepartment { get; set; }

    /// <summary>
    /// 获取或设置新版流程列表（use_template_approver 为 0 时必填）。
    /// </summary>
    [JsonPropertyName("process")]
    public ApprovalProcessInfo? Process { get; set; }

    /// <summary>
    /// 获取或设置审批申请数据（官方必填），可定义审批申请中各个控件的值，其中必填项必须有值，选填项可为空。
    /// </summary>
    [JsonPropertyName("apply_data")]
    public ApprovalApplyData? ApplyData { get; set; }

    /// <summary>
    /// 获取或设置摘要信息（官方必填），用于显示在审批通知卡片、审批列表的摘要信息，最多 3 行。
    /// </summary>
    [JsonPropertyName("summary_list")]
    public List<ApprovalSummaryLine>? SummaryList { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 批量获取审批单号的筛选条件（filters 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalInfoFilter
{
    /// <summary>
    /// 获取或设置筛选类型：template_id-模板类型/模板id；creator-申请人；department-审批单提单者所在部门；sp_status-审批状态；record_type-审批单类型属性（1-请假；2-打卡补卡；3-出差；4-外出；5-加班；6-调班；7-会议室预定；8-退款审批；9-红包报销审批）。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：仅「部门」支持同时配置多个筛选条件；不同类型的筛选条件之间为「与」的关系，同类型筛选条件之间为「或」的关系；record_type 筛选类型仅支持 2021/05/31 以后新提交的审批单，历史单不支持表单类型属性过滤。</para>
    /// </remarks>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>
    /// 获取或设置筛选值，对应为：template_id-模板id；creator-申请人userid；department-所在部门id；sp_status-审批单状态（1-审批中；2-已通过；3-已驳回；4-已撤销；6-通过后撤销；7-已删除；10-已支付）。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

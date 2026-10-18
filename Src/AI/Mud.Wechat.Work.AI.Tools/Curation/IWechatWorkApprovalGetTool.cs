// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>approval.get</c>：按审批单号获取企业微信审批申请详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界是契约的一部分</b>：官方 <c>apply_data</c> 是「控件超集结构」，金额、收款账户、身份证号等个人数据
/// 都可能在里面；<c>comments</c>/<c>speech</c> 是审批意见正文。这些一旦进入提示词与日志即不可撤回，故执行器
/// <b>只回填结构与流转事实</b>（状态、模板、申请人、各节点审批人与时间、抄送、备注条数、表单字段名），
/// 不回填控件取值与意见正文。需要具体表单值时，由宿主在自己的授权面内取，不走工具面。
/// </para>
/// <para>
/// <b>已取消的审批单仍可取详情</b>：需由调用方（模型）自行读 <c>sp_status</c> 判断，工具不替它下结论。
/// </para>
/// </remarks>
[AgentTool(
    name: "approval.get",
    Description = "按审批单号获取企业微信审批申请详情。参数 sp_no 为审批单号（由用户从审批应用或审批通知中提供，形如 202310100001）。返回 sp_no、sp_name（模板名）、sp_status（1 审批中 / 2 已通过 / 3 已驳回 / 4 已撤销 / 6 通过后撤销 / 7 已删除 / 10 已支付）、template_id、apply_time、申请人、各审批节点的状态与审批人及操作时间、抄送人、备注条数与表单字段名。不含表单控件取值与审批意见正文（个人数据不外发），需要具体字段值时请让用户到审批应用查看。",
    Source = "IWechatWorkApprovalService.GetApprovalDetailAsync",
    RequiredScopes = ["approval.read"])]
public interface IWechatWorkApprovalGetTool
{
    /// <summary>获取审批申请详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（sp_no、sp_name、sp_status、template_id、apply_time、applyer、batch_applyer、records、notifyer、comment_count、form_fields）。</returns>
    Task<string> GetApprovalDetailAsync(
        [ToolParameter("sp_no", "审批单号（由用户从审批应用/通知中提供）。", Required = true)] string sp_no,
        CancellationToken cancellationToken = default);
}

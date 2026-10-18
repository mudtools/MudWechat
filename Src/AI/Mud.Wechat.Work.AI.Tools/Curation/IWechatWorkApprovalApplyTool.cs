// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>approval.apply</c>：以指定成员身份提交一条审批（受控写，可在审批应用中撤回）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三段式链的终点</b>：<c>approval.template.get</c>（拿控件 ID 与类型）→ 本工具（按控件 ID 填值提交）→
/// <c>approval.get</c>（核对状态与流向）。缺前一段就只能猜控件 ID，缺后一段就无法确认提交结果。
/// </para>
/// <para>
/// <b>为什么要 <c>fields_json</c> 而不是结构化的参数列表</b>：审批表单是<b>动态</b>的——控件数量、ID、类型
/// 全由模板决定，工具声明不可能枚举。JSON 对象（<c>控件ID → 值</c>）是唯一能表达「任意模板」的形态；
/// 执行器会<b>回过头按模板校验</b>：未知控件 ID、必填缺失、控件类型不支持，全部在调用官方接口之前拒绝。
/// 因此它是「结构化 + 服务端权威校验」，不是「让模型手写结构」。
/// </para>
/// <para>
/// <b>只支持可安全映射的控件</b>：Text/Textarea/Number/Money/Date/Selector/Members。附件、明细表、假期、
/// 位置、关联审批等控件<b>明确拒绝</b>并提示到审批应用里填写——静默丢弃一个字段比提交失败更糟
/// （用户会以为附件已经带上了）。
/// </para>
/// <para>
/// <b>风险档为受控写（L2）</b>：提交后可撤回，代价是流程噪声而不是不可逆损害；但仍要求先与用户核对
/// 「模板 / 申请人 / 每个字段的值」。
/// </para>
/// </remarks>
[AgentTool(
    name: "approval.apply",
    Description = "以指定成员身份提交一条审批（可撤回的受控写）。调用前必须先调 approval.template.get 拿控件列表。参数 template_id 为模板 ID；creator_userid 为申请人 UserID（先用 contacts.user.resolve 解析）；fields_json 为 JSON 对象，键是控件 ID、值是控件取值，形如 {\"Text-1700000000000\":\"张三\",\"Number-1700000000001\":\"3\",\"Date-1700000000002\":\"1792308600\",\"Selector-1700000000003\":\"option-1\",\"Members-1700000000004\":[\"zhangsan\"]}（Selector 多选传数组；Members 传 userid 数组；Date 传 Unix 秒或 {\"type\":\"day\",\"timestamp\":\"...\"}）。执行器会按模板校验：未知控件 ID、必填字段缺失、控件类型不在支持范围（支持 Text/Textarea/Number/Money/Date/Selector/Members）都会在提交前报错。summary 可选（审批列表里显示的摘要文字，多个用 | 分隔）；use_template_approver 可选（true 用模板审批人（默认）/ false 需在审批应用中指定）。返回 sp_no。提交前请与用户逐字段核对。",
    Source = "IWechatWorkApprovalService.ApplyApprovalAsync",
    RequiredScopes = ["approval.write"],
    IsWrite = true,
    OutputSchema = "{\"type\":\"object\",\"properties\":{\"ok\":{\"type\":\"boolean\"},\"sp_no\":{\"type\":\"string\"}},\"required\":[\"ok\"]}")]
public interface IWechatWorkApprovalApplyTool
{
    /// <summary>提交审批。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、sp_no、filled_controls）。</returns>
    Task<string> ApplyApprovalAsync(
        [ToolParameter("template_id", "审批模板 ID（由 approval.template.get 使用）。", Required = true)] string template_id,
        [ToolParameter("creator_userid", "申请人 UserID。", Required = true)] string creator_userid,
        [ToolParameter("fields_json", "控件取值 JSON 对象（键 = 控件 ID，值 = 取值）。", Required = true)] string fields_json,
        [ToolParameter("summary", "摘要文字（多个用 | 分隔；可空 ⇒ 由模板决定）。")] string? summary,
        [ToolParameter("use_template_approver", "是否使用模板配置的审批人（true 默认 / false ⇒ 须在审批应用中指定）。")] bool? use_template_approver,
        CancellationToken cancellationToken = default);
}

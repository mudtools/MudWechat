// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>journal.record.get</c>：按汇报 UUID 读取单条汇报详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界</b>：官方 <c>apply_data.contents[].value</c> 是控件超集（成员/部门/文件/位置/表格/关联审批…），
/// 执行器只回填<b>文本类</b>取值（Text / Number / Money / Date / Selector）与字段名，
/// <b>不外发</b>成员与部门清单、文件 media_id、位置、表格子行、打卡/请假/定位等个人数据控件。
/// </para>
/// <para>
/// <b>特殊模板正文</b>：「汇报」类模板的表单不在 <c>apply_data</c>，而在 <c>sys_journal_data</c>（富文本）；
/// 执行器把它作为 <c>journal_text</c> 回填，否则这类模板取回来会是空壳。
/// </para>
/// </remarks>
[AgentTool(
    name: "journal.record.get",
    Description = "按汇报 UUID 获取企业微信汇报记录详情。参数 journaluuid 为汇报记录 ID（先用 journal.record.list 获取）。返回 journal_uuid、template_name、template_id、submitter（提交人 userid）、report_time（提交时间戳）、receiver_count / readed_count（接收人与已读人数）、comment_count（评论条数）、journal_text（特殊「汇报」模板的富文本正文，可能为空）与 fields（表单字段：name 字段名、control 控件类型、text/ number/ money/ date/ selector 取值）。不含成员/部门清单、附件 media_id、位置与表格明细；评论正文也不返回（只给条数）。",
    Source = "IWechatWorkInternalJournalService.GetJournalRecordDetailAsync",
    RequiredScopes = ["journal.record.read"])]
public interface IWechatWorkJournalRecordGetTool
{
    /// <summary>读取汇报详情。</summary>
    /// <param name="journaluuid">汇报记录 UUID。</param>
    void Get(string journaluuid);
}

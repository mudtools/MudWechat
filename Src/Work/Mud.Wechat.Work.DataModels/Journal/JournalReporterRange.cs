// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报人员范围信息（官方 report_range / white_range）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalReporterRange
{
    /// <summary>获取或设置成员列表（官方 user_list）。</summary>
    [JsonPropertyName("user_list")]
    public List<ApprovalUserRef>? UserList { get; set; }

    /// <summary>获取或设置部门列表（官方 party_list，部门以 open_partyid 回传）。</summary>
    [JsonPropertyName("party_list")]
    public List<JournalDepartmentRef>? DepartmentList { get; set; }

    /// <summary>获取或设置标签列表（官方 tag_list，标签以 open_tagid 回传）。</summary>
    [JsonPropertyName("tag_list")]
    public List<JournalTagRef>? TagList { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 接收人员范围信息（官方 receivers）。
/// <para>与汇报人员范围的差异：官方此处不带 party_list，另带 leader_list（按上级级别）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalReceiverRange
{
    /// <summary>获取或设置成员列表（官方 user_list）。</summary>
    [JsonPropertyName("user_list")]
    public List<ApprovalUserRef>? UserList { get; set; }

    /// <summary>获取或设置标签列表（官方 tag_list，标签以 open_tagid 回传）。</summary>
    [JsonPropertyName("tag_list")]
    public List<JournalTagRef>? TagList { get; set; }

    /// <summary>获取或设置上级级别列表（官方 leader_list）。</summary>
    [JsonPropertyName("leader_list")]
    public List<JournalLeaderRef>? LeaderList { get; set; }
}

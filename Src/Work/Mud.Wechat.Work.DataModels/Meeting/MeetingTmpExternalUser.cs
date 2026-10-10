// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会中参会的外部联系人（获取会议详情响应 <c>attendees.tmp_external_user</c> 元素；微信入会用户不返回）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>tmp_external_userid</c> 为外部用户临时 ID，同一用户在不同会议中该 ID 不同，
/// 可通过「tmp_external_userid 的转换」接口转为 <c>external_userid</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingTmpExternalUser
{
    /// <summary>获取或设置外部用户临时 ID（同一用户在不同会议中该 ID 不同）。</summary>
    [JsonPropertyName("tmp_external_userid")]
    public string? TmpExternalUserid { get; set; }

    /// <summary>获取或设置与会状态：1 - 已参与；2 - 未参与。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置首次加入会议时间的 Unix 时间戳。</summary>
    [JsonPropertyName("first_join_time")]
    public long? FirstJoinTime { get; set; }

    /// <summary>获取或设置最后一次离开会议时间的 Unix 时间戳。</summary>
    [JsonPropertyName("last_quit_time")]
    public long? LastQuitTime { get; set; }

    /// <summary>获取或设置入会次数。</summary>
    [JsonPropertyName("total_join_count")]
    public int? TotalJoinCount { get; set; }

    /// <summary>获取或设置累计参会时长（秒）。</summary>
    [JsonPropertyName("cumulative_time")]
    public int? CumulativeTime { get; set; }
}

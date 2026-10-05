// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议受邀成员对象（获取/更新会议受邀成员列表的 <c>invitees</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 注意与创建/修改预约会议请求的 <see cref="MeetingInvitees"/>（<c>userid</c> 字符串数组包裹对象）形态不同：
/// 受邀成员列表端点的 <c>invitees</c> 为本对象数组。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingInvitee
{
    /// <summary>获取或设置受邀成员的 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }
}

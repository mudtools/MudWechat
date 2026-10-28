// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 指定响铃成员列表对象（创建/修改预约会议请求与获取会议详情响应 <c>settings.ring_users</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingRingUsers
{
    /// <summary>
    /// 获取或设置响铃成员 userid 列表。
    /// <para><c>remind_scope</c> 为 4（指定部分成员响铃）且本列表为空时，全部成员不响铃。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }
}

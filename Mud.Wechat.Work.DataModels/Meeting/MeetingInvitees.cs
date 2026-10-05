// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 邀请参会成员对象（创建/修改预约会议请求 <c>invitees</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingInvitees
{
    /// <summary>
    /// 获取或设置参会企业成员 userid 列表；任何 userid 不合法或不在应用可见范围内将直接报错。
    /// <para>参会人数上限由管理员可预约人数决定：普通企业最多 100 人；付费企业由所购在线会议室/高级账号容量决定，最多 300 人，
    /// 超过 300 人需调用「更新会议受邀成员列表」接口。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }
}

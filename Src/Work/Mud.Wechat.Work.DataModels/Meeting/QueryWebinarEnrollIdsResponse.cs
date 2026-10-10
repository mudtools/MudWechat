// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取网络研讨会成员报名 ID 响应体（<c>/cgi-bin/meeting/webinar/enroll/query_by_tmp_openid</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class QueryWebinarEnrollIdsResponse : WechatWorkResponse
{
    /// <summary>获取或设置成员报名 ID 数组（结构同 <see cref="MeetingEnrollId"/>；仅返回已报名成员的报名 ID，若传入的成员无人报名则无该字段）。</summary>
    [JsonPropertyName("enroll_id_list")]
    public List<MeetingEnrollId>? EnrollIdList { get; set; }
}

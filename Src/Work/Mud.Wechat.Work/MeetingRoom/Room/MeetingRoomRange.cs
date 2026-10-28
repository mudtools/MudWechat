// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 会议室使用范围（官方 range，添加 / 编辑 / 查询会议室列表三端点共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomRange
{
    /// <summary>获取或设置使用范围的成员 UserId 列表（官方 user_list）。</summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserIdList { get; set; }

    /// <summary>获取或设置使用范围的部门 ID 列表（官方 department_list）。</summary>
    [JsonPropertyName("department_list")]
    public List<long>? DepartmentIdList { get; set; }
}

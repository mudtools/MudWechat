// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 家校部门管理员信息条目（<c>department_admins</c> 数组元素，获取部门列表响应）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolDepartmentAdminInfo
{
    /// <summary>
    /// 获取或设置部门管理员的 userid。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置管理员类型：1 表示校区负责人，2 表示年级负责人，
    /// 3 表示班主任，4 表示任课老师，5 表示学段负责人。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置教师或班主任的科目。
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
}

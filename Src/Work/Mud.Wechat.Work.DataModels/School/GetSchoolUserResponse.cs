// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 读取学生或家长响应体（<c>/cgi-bin/school/user/get</c>，家校沟通）。
/// <para>
/// 官方形态：按 <see cref="UserType"/> 二选一返回 —— 1 表示学生（返回 <see cref="Student"/>），
/// 2 表示家长（返回 <see cref="Parent"/>）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class GetSchoolUserResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置用户类型：1 表示学生，2 表示家长。
    /// </summary>
    [JsonPropertyName("user_type")]
    public int? UserType { get; set; }

    /// <summary>
    /// 获取或设置学生字段（<see cref="UserType"/> 为 1 时返回）。
    /// </summary>
    [JsonPropertyName("student")]
    public SchoolStudentInfo? Student { get; set; }

    /// <summary>
    /// 获取或设置学生家长字段（<see cref="UserType"/> 为 2 时返回）。
    /// </summary>
    [JsonPropertyName("parent")]
    public SchoolParentInfo? Parent { get; set; }
}

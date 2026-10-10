// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取部门学生详情响应体（<c>/cgi-bin/school/user/list</c>，家校沟通）。
/// <para>
/// 官方业务限制：如需获取该部门及其子部门的所有学生，需先获取该部门下的子部门，
/// 然后再获取子部门下的学生，逐层递归获取；官方无 cursor/limit 分页参数。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolDepartmentStudentsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置学生列表。
    /// </summary>
    [JsonPropertyName("students")]
    public List<SchoolStudentInfo>? Students { get; set; }
}

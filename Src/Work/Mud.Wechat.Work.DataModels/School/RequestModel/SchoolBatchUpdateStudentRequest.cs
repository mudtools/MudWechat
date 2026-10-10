// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 批量更新学生请求体（<c>/cgi-bin/school/user/batch_update_student</c>，家校沟通）。
/// <para>
/// 官方业务限制：<see cref="Students"/> 每次最多 100 个学生；
/// 每个班级下学生总数不能超过 3 万个，建议保证创建 department 对应的部门和创建成员是串行化处理；
/// <c>new_student_userid</c> 每个学生仅能修改一次。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolBatchUpdateStudentRequest
{
    /// <summary>
    /// 获取或设置学生列表（每次最多 100 个学生）。
    /// </summary>
    [JsonPropertyName("students")]
    public List<SchoolUpdateStudentRequest>? Students { get; set; }
}

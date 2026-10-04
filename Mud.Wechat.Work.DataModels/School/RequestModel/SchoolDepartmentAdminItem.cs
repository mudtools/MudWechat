// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 部门管理员条目（<c>department_admins</c> 数组元素，创建部门请求体）。
/// <para>
/// 官方业务限制：管理员类型须与部门类型保持一致（1 校区负责人仅可配置到校区部门，
/// 2 年级负责人仅可配置到年级部门，3 班主任 / 4 任课老师仅可设置到班级，5 学段负责人）；
/// <see cref="Subject"/> 最多 15 个字符，仅支持设置一个科目，仅班主任和任课老师可以设置。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolDepartmentAdminItem
{
    /// <summary>
    /// 获取或设置对应管理端的账号。企业内必须唯一，不区分大小写，长度为 1~64 个字节。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置部门管理员类型（官方必填）：1 表示校区负责人，2 表示年级负责人，
    /// 3 表示班主任，4 表示任课老师，5 表示学段负责人。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置教师的科目（仅班主任和任课老师可以设置，最多 15 个字符，仅支持设置一个科目）。
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
}

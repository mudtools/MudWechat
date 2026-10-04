// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 更新部门请求体（<c>/cgi-bin/school/department/update</c>，家校沟通）。
/// <para>
/// 官方业务限制：班级的父部门必须是年级（type 为 1 的部门的父部门 type 值必须为 2）；
/// 当传入的 <see cref="StandardGrade"/> 为 0 时，表示将此部门转换为非标准年级；
/// 管理员类型须与部门类型保持一致；
/// <see cref="DepartmentAdmins"/> 元素结构与创建部门不同（多 <c>op</c> 操作字段），不共用 DTO。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolUpdateDepartmentRequest
{
    /// <summary>
    /// 获取或设置部门名称。长度限制为 1~32 个字符，字符不能包括 :*?&quot;&lt;&gt;/ 等特殊字符；
    /// 如果部门为标准年级则忽略该字段。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置父部门 id（32 位整型）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int? Parentid { get; set; }

    /// <summary>
    /// 获取或设置部门 id（官方必填，32 位整型，必须大于 0）。
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// 获取或设置修改后的新部门 id。
    /// </summary>
    [JsonPropertyName("new_id")]
    public int? NewId { get; set; }

    /// <summary>
    /// 获取或设置入学年份（32 位整型，格式为 YYYY，输入范围为 1970～2100，
    /// 仅当部门类型为年级（2）时生效）。
    /// </summary>
    [JsonPropertyName("register_year")]
    public int? RegisterYear { get; set; }

    /// <summary>
    /// 获取或设置标准年级（32 位整型，参数值含义详见官方标准年级对照表，
    /// 仅当部门类型为年级（2）时生效；传 0 表示将此部门转换为非标准年级）。
    /// </summary>
    [JsonPropertyName("standard_grade")]
    public int? StandardGrade { get; set; }

    /// <summary>
    /// 获取或设置在父部门中的次序值。order 值大的排序靠前，有效的值范围是 [0, 2^32)。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }

    /// <summary>
    /// 获取或设置部门管理员列表（增删改操作经 <see cref="SchoolUpdateDepartmentAdminItem.Op"/> 表达）。
    /// </summary>
    [JsonPropertyName("department_admins")]
    public List<SchoolUpdateDepartmentAdminItem>? DepartmentAdmins { get; set; }
}

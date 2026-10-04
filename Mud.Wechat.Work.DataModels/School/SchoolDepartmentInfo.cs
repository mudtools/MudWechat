// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 家校部门信息（获取部门列表响应的 <c>departments</c> 数组元素）。
/// <para>
/// 官方形态：根部门 id 固定为 1、parentid 为 0、type 为 5（学校）；
/// 部分字段为条件返回——<see cref="RegisterYear"/> / <see cref="StandardGrade"/> 仅标准年级返回、
/// <see cref="Order"/> 仅在 API 设置后才返回、<see cref="IsGraduated"/> 仅部门类型为年级时返回、
/// <see cref="OpenGroupChat"/> / <see cref="GroupChatId"/> 仅部门类型为班级时返回
/// （group_chat_id 还需 open_group_chat 为 1）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolDepartmentInfo
{
    /// <summary>
    /// 获取或设置部门 id（根部门固定为 1）。
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// 获取或设置部门类型：1 表示班级，2 表示年级，3 表示学段，4 表示校区，5 表示学校（根部门）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置部门名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置父部门 id（根部门该项为 0）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int? Parentid { get; set; }

    /// <summary>
    /// 获取或设置入学年份（格式为 YYYY，仅标准年级返回）。
    /// </summary>
    [JsonPropertyName("register_year")]
    public int? RegisterYear { get; set; }

    /// <summary>
    /// 获取或设置标准年级（具体值参考官方标准年级对照表，当部门为标准年级时返回）。
    /// </summary>
    [JsonPropertyName("standard_grade")]
    public int? StandardGrade { get; set; }

    /// <summary>
    /// 获取或设置在父部门中的次序值（order 值大的排序靠前，有效的值范围是 [0, 2^32)，
    /// 仅在 API 设置后才返回）。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }

    /// <summary>
    /// 获取或设置是否已毕业（1 是，0 否；仅部门类型为年级时才返回）。
    /// </summary>
    [JsonPropertyName("is_graduated")]
    public int? IsGraduated { get; set; }

    /// <summary>
    /// 获取或设置是否开启班级群（1 开启，0 关闭；仅部门类型为班级时才返回）。
    /// </summary>
    [JsonPropertyName("open_group_chat")]
    public int? OpenGroupChat { get; set; }

    /// <summary>
    /// 获取或设置班级群 id（仅部门类型为班级且 open_group_chat 为 1 时才返回）。
    /// </summary>
    [JsonPropertyName("group_chat_id")]
    public string? GroupChatId { get; set; }

    /// <summary>
    /// 获取或设置部门管理员列表。
    /// </summary>
    [JsonPropertyName("department_admins")]
    public List<SchoolDepartmentAdminInfo>? DepartmentAdmins { get; set; }
}

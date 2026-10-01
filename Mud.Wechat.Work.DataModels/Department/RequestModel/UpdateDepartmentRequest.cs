// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Department;

/// <summary>
/// 更新部门请求体（<c>/cgi-bin/department/update</c>；应用须拥有指定部门的管理权限，移动部门还需新父部门的管理权限；第三方仅通讯录应用可调用）。
/// </summary>
/// <remarks>非必须字段未指定则不更新该字段；部门最大层级为 15 层，部门总数不能超过 3 万个。</remarks>
public class UpdateDepartmentRequest
{
    /// <summary>
    /// 获取或设置部门 ID。
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 获取或设置部门名称（1~64 个 UTF-8 字符；不能包含反斜杠、冒号、星号、问号、引号、尖括号、竖线等字符）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置英文名称（需在管理后台开启多语言支持才生效；1~64 字符）。
    /// </summary>
    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    /// <summary>
    /// 获取或设置父部门 ID（移动部门时须拥有新父部门的管理权限）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int? ParentId { get; set; }

    /// <summary>
    /// 获取或设置在父部门中的次序值（值大的排序靠前，官方范围 [0, 2^32)，故以 64 位整数承载）。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }
}

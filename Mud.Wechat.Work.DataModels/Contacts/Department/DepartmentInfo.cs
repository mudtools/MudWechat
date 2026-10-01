// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Department;

/// <summary>
/// 企业微信部门对象（获取部门列表 <c>/cgi-bin/department/list</c> 与获取单个部门详情 <c>/cgi-bin/department/get</c> 的部门负载）。
/// </summary>
/// <remarks>
/// 各应用类型可获取的字段不同：代开发自建应用需管理员授权才返回 name；第三方应用不可获取 name/name_en
/// （以 id 代替，展示需用通讯录展示组件），department_leader 仅第三方通讯录应用或获相应授权的应用可获取。
/// </remarks>
public class DepartmentInfo
{
    /// <summary>
    /// 获取或设置部门 ID（32 位整型）。
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 获取或设置部门名称（同一层级内唯一，1~64 个 UTF-8 字符；第三方不可获取，代开发需管理员授权）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置部门英文名称（需在管理后台开启多语言支持；第三方不可获取）。
    /// </summary>
    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    /// <summary>
    /// 获取或设置部门负责人的 UserID 列表（仅返回应用可见范围内的负责人；第三方仅通讯录应用或获相应授权可获取）。
    /// </summary>
    [JsonPropertyName("department_leader")]
    public List<string>? DepartmentLeader { get; set; } = [];

    /// <summary>
    /// 获取或设置父部门 ID（根部门为 1）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int ParentId { get; set; }

    /// <summary>
    /// 获取或设置在父部门中的次序值（值大的排序靠前，官方范围 [0, 2^32)，故以 64 位整数承载）。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }
}
